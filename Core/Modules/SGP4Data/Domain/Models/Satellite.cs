using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Domain.Models
{
    public class Satellite
    {
        public Satellite() { }

        public Satellite(int NORAD_CAT_ID, string OBJECT_NAME, string OBJECT_ID, double MEAN_MOTION, double ECCENTRICITY, double INCLINATION, double RA_OF_ASC_NODE, double ARG_OF_PERICENTER,
            double MEAN_ANOMALY, int EPHEMERIS_TYPE, string CLASSIFICATION_TYPE, int ELEMENT_SET_NO, int REV_AT_EPOCH, double BSTAR, double MEAN_MOTION_DOT, int MEAN_MOTION_DDOT, string category, DateTime EPOCH, DateTime updatedAt)
        {
            this.NORAD_CAT_ID = NORAD_CAT_ID;
            this.OBJECT_NAME = OBJECT_NAME;
            this.OBJECT_ID = OBJECT_ID;
            this.MEAN_MOTION = MEAN_MOTION;
            this.ECCENTRICITY = ECCENTRICITY;
            this.INCLINATION = INCLINATION;
            this.RA_OF_ASC_NODE = RA_OF_ASC_NODE;
            this.ARG_OF_PERICENTER = ARG_OF_PERICENTER;
            this.MEAN_ANOMALY = MEAN_ANOMALY;
            this.EPHEMERIS_TYPE = EPHEMERIS_TYPE;
            this.CLASSIFICATION_TYPE = CLASSIFICATION_TYPE;
            this.ELEMENT_SET_NO = ELEMENT_SET_NO;
            this.REV_AT_EPOCH = REV_AT_EPOCH;
            this.BSTAR = BSTAR;
            this.MEAN_MOTION_DOT = MEAN_MOTION_DOT;
            this.MEAN_MOTION_DDOT = MEAN_MOTION_DDOT;
            Category = category;
            this.EPOCH = EPOCH;
            UpdatedAt = updatedAt;
        }

        // Первичный ключ. Международный Id спутника
        public int NORAD_CAT_ID { get; set; }

        // Имя спутника
        public string OBJECT_NAME { get; set; } = string.Empty;

        // Id спутника
        public string OBJECT_ID { get; set; } = string.Empty;

        // Среднее движение
        public double MEAN_MOTION { get; set; } = 0;

        // Эксцентриситет орбиты
        public double ECCENTRICITY { get; set; } = 0;

        // Наклонение орбиты
        public double INCLINATION { get; set; } = 0;

        // Долгота восходящего узла
        public double RA_OF_ASC_NODE { get; set; } = 0;

        // Аргумент перицентра
        public double ARG_OF_PERICENTER { get; set; } = 0;

        // Средняя аномалия
        public double MEAN_ANOMALY { get; set; } = 0;

        // Тип эфемерид
        public int EPHEMERIS_TYPE { get; set; } = 0;

        // Тип классификации спутника
        public string CLASSIFICATION_TYPE { get; set; } = string.Empty;

        // Номер набора элементов
        public int ELEMENT_SET_NO { get; set; } = 0;

        // Номер витка на момент эпохи
        public int REV_AT_EPOCH { get; set; } = 0;

        // Коэффициент торможения BSTAR
        public double BSTAR { get; set; } = 0;

        // Первая производная среднего движения
        public double MEAN_MOTION_DOT { get; set; } = 0;

        // Вторая производная среднего движения
        public double MEAN_MOTION_DDOT { get; set; } = 0;

        // Категория спутника
        public string Category { get; set; } = string.Empty;

        // Время, когда данные были актуальны (Epoch)
        public DateTime EPOCH { get; set; }

        // Когда мы последний раз обновляли эту запись в своей базе
        public DateTime UpdatedAt { get; set; }

    }
}
