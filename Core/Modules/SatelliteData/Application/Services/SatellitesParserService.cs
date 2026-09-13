using Core.Modules.SatelliteData.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SatelliteData.Application.Services
{
    public partial class SatellitesParserService
    {
        private readonly ILogger<SatellitesParserService> _logger;

        public SatellitesParserService(ILogger<SatellitesParserService> logger)
        {
            _logger = logger;
        }

        /*

        public List<Satellite> Parse(string httpTLEstring, string satellitesCategory)
        {
            LogLaunch();

            if (string.IsNullOrEmpty(httpTLEstring))
            {
                LogCancellationParserProcessing();
                return new List<Satellite>();
            }

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

        private List<Satellite> DataFormatting(List<List<string>> tleLines, string satellitesCategory)
        {
            List<Satellite> satellites = new List<Satellite>();

            foreach (var line in tleLines)
            {
                // Проверка на контрольную сумму перед добавлением строки
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
                        NORAD_CAT_ID = noradId,
                        Category = satellitesCategory,
                        EPOCH = epochFormatting,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    satellites.Add(satellite);
                }

                else continue;
            }

            LogParserFormatting();
            return satellites;
        }

        private DateTime DateFormatting(ReadOnlySpan<char> line1Span)
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
        private bool CheckSum(List<string> tleLine)
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

        */

        public List<Satellite> Parse(string httpOMMstring, string satellitesCategory)
        {
            LogLaunch();

            if (string.IsNullOrEmpty(httpOMMstring))
            {
                LogCancellationParserProcessing();
                return new List<Satellite>();
            }

            // 1. Очищаем текст от \r и разбиваем на строки
            string[] lines = httpOMMstring.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= 1)
                return new List<Satellite>(); // База пустая или содержит только заголовок

            // 2. Парсим заголовок (первая строка CSV), чтобы составить карту индексов
            var headers = lines[0].Split(',');
            var _csvHeaders = headers
                .Select((value, index) => new { value = value.Trim().ToUpper(), index })
                .ToDictionary(pair => pair.value, pair => pair.index);

            // 3. Передаем строки с данными (начиная с индекса 1) в метод форматирования
            // Пропускаем заголовок
            var dataLines = lines.Skip(1).ToList();

            return DataFormatting(dataLines, satellitesCategory, _csvHeaders);
        }

        private List<Satellite> DataFormatting(List<string> ommLines, string satellitesCategory, Dictionary<string, int> csvHeaders)
        {
            List<Satellite> satellites = new List<Satellite>();

            foreach (var line in ommLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Разбиваем текущую строку на элементы
                string[] values = line.Split(',');

                try
                {
                    // Безопасно извлекаем значения по имени колонки
                    int noradId = int.Parse(GetCsvValue(values, "NORAD_CAT_ID", csvHeaders));

                    // 1. Сначала парсим как обычно
                    DateTime parsedEpoch = DateTime.Parse(GetCsvValue(values, "EPOCH", csvHeaders), CultureInfo.InvariantCulture);

                    // 2. Явно принудительно задаем таймзону UTC
                    DateTime epochFormatting = DateTime.SpecifyKind(parsedEpoch, DateTimeKind.Utc);

                    Satellite satellite = new Satellite
                    {
                        NORAD_CAT_ID = noradId,
                        Category = satellitesCategory,
                        EPOCH = epochFormatting,
                        UpdatedAt = DateTime.UtcNow,

                        // Строковые поля
                        OBJECT_NAME = GetCsvValue(values, "OBJECT_NAME", csvHeaders),
                        OBJECT_ID = GetCsvValue(values, "OBJECT_ID", csvHeaders),
                        CLASSIFICATION_TYPE = GetCsvValue(values, "CLASSIFICATION_TYPE", csvHeaders),

                        // Числовые поля (double / float)
                        MEAN_MOTION = double.Parse(GetCsvValue(values, "MEAN_MOTION", csvHeaders), CultureInfo.InvariantCulture),
                        ECCENTRICITY = double.Parse(GetCsvValue(values, "ECCENTRICITY", csvHeaders), CultureInfo.InvariantCulture),
                        INCLINATION = double.Parse(GetCsvValue(values, "INCLINATION", csvHeaders), CultureInfo.InvariantCulture),
                        RA_OF_ASC_NODE = double.Parse(GetCsvValue(values, "RA_OF_ASC_NODE", csvHeaders), CultureInfo.InvariantCulture),
                        ARG_OF_PERICENTER = double.Parse(GetCsvValue(values, "ARG_OF_PERICENTER", csvHeaders), CultureInfo.InvariantCulture),
                        MEAN_ANOMALY = double.Parse(GetCsvValue(values, "MEAN_ANOMALY", csvHeaders), CultureInfo.InvariantCulture),

                        // Поля с научной нотацией (например .47803E-4)
                        BSTAR = ParseScientificNotation(GetCsvValue(values, "BSTAR", csvHeaders)),
                        MEAN_MOTION_DOT = ParseScientificNotation(GetCsvValue(values, "MEAN_MOTION_DOT", csvHeaders)),
                        MEAN_MOTION_DDOT = ParseScientificNotation(GetCsvValue(values, "MEAN_MOTION_DDOT", csvHeaders)),

                        // Целочисленные поля
                        EPHEMERIS_TYPE = int.Parse(GetCsvValue(values, "EPHEMERIS_TYPE", csvHeaders)),
                        ELEMENT_SET_NO = int.Parse(GetCsvValue(values, "ELEMENT_SET_NO", csvHeaders)),
                        REV_AT_EPOCH = int.Parse(GetCsvValue(values, "REV_AT_EPOCH", csvHeaders))
                    };

                    satellites.Add(satellite);
                }
                catch
                {
                    continue;
                }               
            }

            LogParserFormatting();
            return satellites;
        }

        // Вспомогательный метод для безопасного получения значения из массива по имени колонки
        private string GetCsvValue(string[] values, string columnName, Dictionary<string, int> csvHeaders)
        {
            if (csvHeaders.TryGetValue(columnName.ToUpper(), out int index) && index < values.Length)
            {
                return values[index].Trim();
            }
            return string.Empty;
        }

        private double ParseScientificNotation(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            value = value.Trim();

            // 1. Если начинается просто с точки: .5228 -> 0.5228
            if (value.StartsWith("."))
            {
                value = "0" + value;
            }
            // 2. Если начинается с минус-точки: -.5228 -> -0.5228
            else if (value.StartsWith("-."))
            {
                value = "-0" + value.Substring(1); // Отрезаем только минус, приклеиваем "-0" и получаем "-0.5228"
            }
            // 3. Если начинается с плюс-точки: +.5228 -> 0.5228 (плюс можно просто опустить)
            else if (value.StartsWith("+."))
            {
                value = "0" + value.Substring(1); // Отрезаем плюс, приклеиваем "0" и получаем "0.5228"
            }

            return double.Parse(value, CultureInfo.InvariantCulture);
        }

    }
}
