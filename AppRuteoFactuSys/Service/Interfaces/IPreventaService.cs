using AppRuteoFactuSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Service.Interfaces
{
    public interface IPreventaService:IBaseService<Preventa>
    {
        Task<Preventa> GetPreventaByNProforma(int nProforma);
    }
}
