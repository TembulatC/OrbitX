using Core.Modules.TLEData.Application.Services;
using Core.Modules.TLEData.Domain.Interfaces;
using Core.Modules.TLEData.Domain.Models;

namespace OrbitX.BackgroundWorkers.Helper
{
    public partial class SatelliteTLEDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISatellitesDataRepository _satellitesDataRepository;
        private readonly ILogger<SatelliteTLEDownloader> _logger;
        private readonly SatellitesParserService _satellitesParserService;
        private readonly static string[] CelestrakCategories = new[]
        {
            "weather", "resource", "sar", "sarsat", "dmc", "tdrss", "argos",
            "planet", "spire", "geo", "gpz", "gpz-plus", "intelsat", "ses",
            "eutelsat", "telesat", "starlink", "oneweb", "qianfan", "hulianwang",
            "kuiper", "iridium-NEXT", "orbcomm", "globalstar", "amateur",
            "satnogs", "x-comm", "other-comm", "gnss", "gps-ops", "glo-ops",
            "galileo", "beidou", "sbas", "science", "geodetic", "engineering",
            "education", "military", "radar", "cubesat"
        };

        public SatelliteTLEDownloader(IHttpClientFactory httpClientFactory, ISatellitesDataRepository satellitesDataRepository, ILogger<SatelliteTLEDownloader> logger, SatellitesParserService satellitesParserService)
        {
            _httpClientFactory = httpClientFactory;
            _satellitesDataRepository = satellitesDataRepository;
            _logger = logger;
            _satellitesParserService = satellitesParserService;
        }

        public async Task GetTLEData(CancellationToken cancellationToken)
        {
            int requestCount = 1;

            foreach (string category in CelestrakCategories)
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");

                string url = (category == "gpz" || category == "gpz-plus")
                ? $"https://celestrak.org/NORAD/elements/gp.php?SPECIAL={category}&FORMAT=cvs"
                : $"https://celestrak.org/NORAD/elements/gp.php?GROUP={category}&FORMAT=cvs";

                int seconds = Random.Shared.Next(30, 61);

                LogHttpRequest(requestCount, seconds, url);

                await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);

                try
                {
                    await ProcessCategoryResponse(client, url, category, cancellationToken);
                    requestCount++;
                }
                catch (HttpRequestException ex) when (ex.InnerException?.Message == "Критическая ошибка после ожидания")
                {
                    LogCritical403Error(ex);
                    break; // Выходим из foreach совсем. Вернемся к парсингу при следующем запуске сервиса.
                }
                catch (HttpRequestException ex)
                {
                    LogUnknownHttpError(ex);
                    requestCount++;
                    continue;
                }               
                catch (Exception ex)
                {
                    LogOtherError(ex);
                    requestCount++;
                    continue;
                }
            }
        }

        private async Task ProcessCategoryResponse(HttpClient client, string url, string category, CancellationToken cancellationToken)
        {
            int statusCode;
            string? reasonPhrase;

            // Ограничиваем время жизни response только самим запросом и проверкой успеха
            using (HttpResponseMessage response = await client.GetAsync(url, cancellationToken))
            {
                if (response.IsSuccessStatusCode)
                {
                    var (isValid, omm) = await HTTPResponse(response, cancellationToken);
                    if (!isValid) return;

                    await SaveDataToRepository(omm, category, cancellationToken);
                    LogSuccess();
                    return;
                }

                // Запоминаем данные ответа, чтобы использовать их в switch ниже
                statusCode = (int)response.StatusCode;
                reasonPhrase = response.ReasonPhrase;
            }

            switch (statusCode)
            {
                case 403:
                    Log403StatusCode(statusCode, url);

                     // Цикл продолжится через 3 часа
                    await Task.Delay(TimeSpan.FromHours(3), cancellationToken);
                    await HandlingError(category, url, cancellationToken);  // Повторный HTTP запрос

                    break;
                    
                case 404:
                    Log404StatusCode(statusCode); // Цикл просто идет дальше
                    break;
                case 500:
                    Log500StatusCode(statusCode, url);

                    // Цикл продолжится через 10 минут
                    await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
                    await HandlingError(category, url, cancellationToken);  // Повторный HTTP запрос

                    break;
                case 503:
                    Log503StatusCode(statusCode, url);

                    // Цикл продолжится через 30 минут
                    await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
                    await HandlingError(category, url, cancellationToken);  // Повторный HTTP запрос

                    break;
                default:
                    LogUnknownError(statusCode, reasonPhrase); // Цикл просто идет дальше
                    break;
            }
        }

        // Отдельный метод для HTTP-запросов
        private async Task<(bool, string)> HTTPResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string omm = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(omm))
            {
                LogHTTPBodyNull();
                return (false, string.Empty);
            }
            else if (omm.Contains("invalid", StringComparison.OrdinalIgnoreCase) || omm.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                LogInvalid();
                return (false, string.Empty);
            }

            return (true, omm);
        }

        private async Task HandlingError(string category, string url, CancellationToken cancellationToken)
        {
            // Create new Client
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");

                using (HttpResponseMessage responseRetry = await client.GetAsync(url, cancellationToken))
                {
                    if (responseRetry.IsSuccessStatusCode)
                    {
                        // Получаем тело ответа
                        var (isValid, omm) = await HTTPResponse(responseRetry, cancellationToken);

                        if (!isValid) return;
                        else await SaveDataToRepository(omm, category, cancellationToken); // Добавление в бд

                        LogSuccess();
                        return;
                    }
                    else throw new HttpRequestException($"Повторный запрос после ожидания также вернул ошибку. Код ошибки: {(int) responseRetry.StatusCode}", 
                        new Exception("Критическая ошибка после ожидания")); // Передали как InnerException
                }
            }        
        }

        // Отдельный метод для обращения к БД
        private async Task SaveDataToRepository(string omm, string category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(omm))
            {
                LogHTTPBodyNull();
                return;
            }

            // Парсим тело ответа
            List<Satellite> parseData = _satellitesParserService.Parse(omm, category);

            if (parseData == null || parseData.Count <= 0)
            {
                LogParseBodyNull();
                return;
            }

            await _satellitesDataRepository.AddTLEData(parseData, category, cancellationToken);
        }
    }
}
