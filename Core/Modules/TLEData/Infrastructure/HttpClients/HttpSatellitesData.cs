using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Infrastructure.HttpClients
{
    public class HttpSatellitesData
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpSatellitesData> _logger;

        public HttpSatellitesData (HttpClient httpClient, ILogger<HttpSatellitesData> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetTLEData(string satellitesCategory)
        {
            _logger.LogInformation($"Создаем HTTP запрос для поиска спутников");

            // Получаем TLE данные по определенной категории спутников
            string url = satellitesCategory == "gpz" || satellitesCategory == "gpz-plus"
                ? $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={satellitesCategory}&FORMAT=tle"
                : $"https://celestrak.org/NORAD/elements/gp.php?GROUP={satellitesCategory}&FORMAT=tle";

            _logger.LogInformation($"URL запроса: {url}");

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);

                // Проверка на статус коды
                if(!response.IsSuccessStatusCode)
                {
                    int statusCode = (int) response.StatusCode;

                    switch (statusCode)
                    {
                        case 403:
                            _logger.LogError($"(HTTP {statusCode}) Доступ временно заблокирован из-за слишком частых запросов");
                            break;
                        case 404:
                            _logger.LogWarning($"(HTTP {statusCode}) Страницы к которой был HTTP-запрос не существует");
                            break;
                        case 500:
                            _logger.LogError($"(HTTP {statusCode}) На внешнем сревере запросов произошел сбой");
                            break;
                        case 503:
                            _logger.LogWarning($"(HTTP {statusCode}) Внешний сервер запросов временно недоступен");
                            break;
                        default:
                            _logger.LogError($"(HTTP {statusCode}) Сетевой запрос завершился с неизвестной ошибкой: {response.ReasonPhrase}");
                            break;
                    }

                    return string.Empty; // Если статус-код плохой, возвращаем пустую строку в парсер
                }

                string tle = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(tle)) // Проверка на пустой контент
                {
                    _logger.LogWarning($"Тело HTTP-запроса пришло пустым");
                    return string.Empty;
                }

                _logger.LogInformation($"HTTP-запрос был завершен успешно");
                return tle;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Неизвестная сетевая ошибка при HTTP-запросе");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Неизвестная ошибка при HTTP-запросе");
                return string.Empty;
            }
        }
    }
}
