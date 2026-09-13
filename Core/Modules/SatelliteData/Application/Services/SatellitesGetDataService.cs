using Core.Modules.SatelliteData.Application.DTOs;
using Core.Modules.SatelliteData.Application.Interfaces;
using Core.Modules.SatelliteData.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SatelliteData.Application.Services
{
    public class SatellitesGetDataService : ISatellitesGetService
    {
        private readonly ISatellitesGetDataRepository _satellitesGetDataRepository;

        public SatellitesGetDataService(ISatellitesGetDataRepository satellitesGetDataRepository)
        {
            _satellitesGetDataRepository = satellitesGetDataRepository;
        }

        public async Task<List<SatellitesFilterDTO>> GetSatellitesFiltersById(string category, int page, int pageSize = 25)
        {
            if (string.IsNullOrEmpty(category)) return new List<SatellitesFilterDTO>();

            var satellitesList = await _satellitesGetDataRepository.GetSatellitesFiltersById(category, page, pageSize);

            if (satellitesList == null || satellitesList.Count <= 0 ) return new List<SatellitesFilterDTO>();

            List<SatellitesFilterDTO> satellitesListDTO = new List<SatellitesFilterDTO>();

            foreach (var satellite in satellitesList)
            {
                SatellitesFilterDTO dto = new SatellitesFilterDTO
                {
                    NoradId = satellite.NORAD_CAT_ID,
                    Name = satellite.OBJECT_NAME
                };

                satellitesListDTO.Add(dto);
            }

            return satellitesListDTO;
        }

        public async Task<List<SatellitesFilterDTO>> GetSatellitesFiltersByName(string category, int page, int pageSize = 25)
        {
            if (string.IsNullOrEmpty(category)) return new List<SatellitesFilterDTO>();

            var satellitesList = await _satellitesGetDataRepository.GetSatellitesFiltersByName(category, page, pageSize);

            if (satellitesList == null || satellitesList.Count <= 0) return new List<SatellitesFilterDTO>();

            List<SatellitesFilterDTO> satellitesListDTO = new List<SatellitesFilterDTO>();

            foreach (var satellite in satellitesList)
            {
                SatellitesFilterDTO dto = new SatellitesFilterDTO
                {
                    NoradId = satellite.NORAD_CAT_ID,
                    Name = satellite.OBJECT_NAME
                };

                satellitesListDTO.Add(dto);
            }

            return satellitesListDTO;
        }
    }
}
