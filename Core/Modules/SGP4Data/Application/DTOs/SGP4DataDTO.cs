using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Application.DTOs
{
    public record class SGP4DataDTO
    {
        // Первичный ключ. Id спутника
        [JsonPropertyName("noradId")]
        public int NoradId { get; set; }

        // Название спутника
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Первая TLE-строка
        [JsonPropertyName("tleLine1")]
        public string TLELine1 { get; set; } = string.Empty;

        // Вторая TLE-строка
        [JsonPropertyName("tleLine2")]
        public string TLELine2 { get; set; } = string.Empty;

        // Долгота
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; } = 0;

        // Широта
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; } = 0;

        // Высота
        [JsonPropertyName("altitude")]
        public double Altitude { get; set; } = 0;
    }
}
