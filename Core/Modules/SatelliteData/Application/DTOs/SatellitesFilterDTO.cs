using System.Text.Json.Serialization;

namespace Core.Modules.SatelliteData.Application.DTOs
{
    public record class SatellitesFilterDTO
    {
        // Первичный ключ. Id спутника
        [JsonPropertyName("noradId")]
        public int NoradId { get; set; }

        // Название спутника
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
