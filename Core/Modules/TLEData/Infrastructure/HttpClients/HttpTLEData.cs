using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Infrastructure.HttpClients
{
    public class HttpTLEData
    {
        private readonly HttpClient _httpClient;

        public HttpTLEData (HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTLEData(string satellitesCategory)
        {
            // Получаем TLE данные по определенной категории спутников
            string url = $"https://celestrak.org/NORAD/elements/gp.php?GROUP={satellitesCategory}&FORMAT=tle";
            string tle = await _httpClient.GetStringAsync(url);

            return tle;
        }
    }
}
