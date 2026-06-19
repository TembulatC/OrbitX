using Core.Modules.SGP4Data.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Application.Interfaces
{
    public interface ISGP
    {
        Task<SGP4DataDTO?> GetSGP(int noradId);
        Task<SGP4DataDTO?> GetSGP(string satelliteName);
    }
}
