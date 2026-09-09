using Core.Modules.SGP4Data.Application.DTOs;
using Core.Modules.SGP4Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Application.Interfaces
{
    public interface ISatelliteSGPServices
    {
        Task<SGP4DataDTO?> GetSGPByID(int noradId);
        Task<SGP4DataDTO?> GetSGPByName(string satelliteName);
    }
}
