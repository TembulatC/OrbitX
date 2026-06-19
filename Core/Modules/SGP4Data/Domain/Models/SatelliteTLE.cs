using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Domain.Models
{
    public class SatelliteTLE
    {
        public SatelliteTLE() { }

        public SatelliteTLE(int noradId, string name, string tleLine1, string tleLine2)
        {
            NoradId = noradId;
            Name = name;
            TLELine1 = tleLine1;
            TLELine2 = tleLine2;
        }

        // Первичный ключ. Id спутника
        public int NoradId { get; set; }

        // Название спутника
        public string Name { get; set; } = string.Empty;

        // Первая TLE-строка
        public string TLELine1 { get; set; } = string.Empty;

        // Вторая TLE-строка
        public string TLELine2 { get; set; } = string.Empty;
    }
}
