using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Infrastructure.HttpClients
{
    public partial class HttpSatellitesData
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpSatellitesData> _logger;

        public HttpSatellitesData(HttpClient httpClient, ILogger<HttpSatellitesData> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetTLEData(string satellitesCategory)
        {
            LogCreateHTTP();

            // Получаем TLE данные по определенной категории спутников
            string url = satellitesCategory == "gpz" || satellitesCategory == "gpz-plus"
                ? $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={satellitesCategory}&FORMAT=tle"
                : $"https://celestrak.org/NORAD/elements/gp.php?GROUP={satellitesCategory}&FORMAT=tle";

            LogURL(url);

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
                            Log403StatusCode(statusCode);
                            break;
                        case 404:
                            Log404StatusCode(statusCode);
                            break;
                        case 500:
                            Log500StatusCode(statusCode);
                            break;
                        case 503:
                            Log503StatusCode(statusCode);
                            break;
                        default:
                            LogUnknownError(statusCode, response.ReasonPhrase);
                            break;
                    }

                    return string.Empty; // Если статус-код плохой, возвращаем пустую строку в парсер
                }

                string tle = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(tle)) // Проверка на пустой контент
                {
                    LogNullOrWhitSpace();
                    return string.Empty;
                }
                else if (tle.Contains("invalid", StringComparison.OrdinalIgnoreCase) || tle.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    LogInvalid();
                    return string.Empty;
                }

                LogSuccessful();
                return tle;
            }
            catch (HttpRequestException ex)
            {
                LogUnknownHttpError(ex);
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogOtherError(ex);
                return string.Empty;
            }
        }
    }
}
