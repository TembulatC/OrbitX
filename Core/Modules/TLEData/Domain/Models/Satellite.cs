using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.TLEData.Domain.Models
{
    public class Satellite
    {
        public Satellite() { }

        public Satellite(int noradId, string name, string tleLine1, string tleLine2, string category, DateTime epoch, DateTime updatedAt)
        {
            NoradId = noradId;
            Name = name;
            TLELine1 = tleLine1;
            TLELine2 = tleLine2;
            Category = category;
            Epoch = epoch;
            UpdatedAt = updatedAt;
        }

        // Первичный ключ. Id спутника
        public int NoradId { get; set; }

        // Имя спутника
        public string Name { get; set; } = string.Empty;

        // Первая TLE-строка
        public string TLELine1 { get; set; } = string.Empty;
        
        // Вторая TLE-строка
        public string TLELine2 { get; set; } = string.Empty;

        // Категория спутника
        public string Category {  get; set; } = string.Empty;

        // Время, когда данные были актуальны (Epoch)
        public DateTime Epoch { get; set; }

        // Когда мы последний раз обновляли эту запись в своей базе
        public DateTime UpdatedAt { get; set; }
    }
}
