using Core.Modules.TLEData.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Application.Services
{
    public static class TLEParser
    {
        public static List<Satellite> Parse(string httpTLEstring, string satellitesCategory)
        {
            // 1. Очищаем текст от \r и \n, убираем пустые строки
            string[] lines = httpTLEstring.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // 1.1 Строка для TLE данных
            List<List<string>> tleLines = new List<List<string>>();

            // 2. Идем по массиву с шагом 3, так как TLE от Celestrak идет строгими блоками:
            // итерация 0: lines[0] = Имя, lines[1] = Строка 1, lines[2] = Строка 2
            for (int i = 0; i < lines.Length; i += 3)
            {
                // Страховка: проверяем, что у нас остался полный блок из 3 строк
                if (i + 2 >= lines.Length) break;

                string name = lines[i].Trim();
                string line1 = lines[i + 1].Trim();
                string line2 = lines[i + 2].Trim();

                // Фильтрация: жесткая проверка стандартов NORAD
                if (line1.Length == 69 && line1.StartsWith('1') &&
                    line2.Length == 69 && line2.StartsWith('2'))
                {
                    tleLines.Add(new List<string> { name, line1, line2 });
                }
            }

            return DataFormatting(tleLines, satellitesCategory);
        }

        private static List<Satellite> DataFormatting(List<List<string>> tleLines, string satellitesCategory)
        {
            List<Satellite> satellites = new List<Satellite>();

            foreach (var line in tleLines)
            {
                bool checkSum = CheckSum(line);

                if (checkSum == true)
                {
                    // 1. Превращаем обычные строки в Span
                    ReadOnlySpan<char> line1Span = line[1].AsSpan();
                    ReadOnlySpan<char> line2Span = line[2].AsSpan();

                    // 2. Парсим noradId через Slice
                    int noradId = int.Parse(line1Span.Slice(2, 5));

                    // 3. Переводим эпоху в формат даты
                    DateTime epochFormatting = DateFormatting(line1Span);

                    Satellite satellite = new Satellite
                    {
                        NoradId = noradId,
                        Name = line[0],
                        TLELine1 = line[1],
                        TLELine2 = line[2],
                        Category = satellitesCategory,
                        Epoch = epochFormatting,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    satellites.Add(satellite);
                }

                else continue;
            }
          
            return satellites;
        }

        private static DateTime DateFormatting(ReadOnlySpan<char> line1Span)
        {
            // Определяем полный год (граница 1957 год — запуск Первого ИСЗ)
            int year = int.Parse(line1Span.Slice(18, 2));
            year = (year < 57) ? 2000 + year : 1900 + year;

            // Извлекаем оставшуюся часть дня
            double dayFrac = double.Parse(line1Span.Slice(20, 12), System.Globalization.CultureInfo.InvariantCulture);

            // Создаем точку отсчета — начало года (1 января, 00:00:00)
            DateTime startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Высчитываем точную дату и время
            DateTime epoch = startOfYear.AddDays(dayFrac - 1);
            
            return epoch;
        }

        // Подсчет контрольной суммы
        private static bool CheckSum(List<string> tleLine)
        {
            // 2 Tle строки
            int tleLine1Sum = 0;
            int tleLine2Sum = 0;

            // Проходимся по каждой цифре первой строки и складываем их
            for(int i = 0; i < tleLine[1].Length - 1; i++)
            {
                if (int.TryParse(tleLine[1][i].ToString(), out int intChar))
                {
                    tleLine1Sum += intChar;
                }
                else if (tleLine[1][i] == '-')
                {
                    tleLine1Sum += 1;
                }
                else tleLine1Sum += 0;
            }

            // Проходимся по каждой цифре второй строки и складываем их
            for (int i = 0; i < tleLine[2].Length - 1; i++)
            {
                if (int.TryParse(tleLine[2][i].ToString(), out int intChar))
                {
                    tleLine2Sum += intChar;
                }
                else if (tleLine[2][i] == '-')
                {
                    tleLine2Sum += 1;
                }
                else tleLine2Sum += 0;
            }

            Console.WriteLine(tleLine1Sum);
            Console.WriteLine(tleLine2Sum);

            // Проверяем сумму всех строк
            if (tleLine1Sum % 10 == Convert.ToInt32(tleLine[1][68].ToString()) && tleLine2Sum % 10 == Convert.ToInt32(tleLine[2][68].ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
