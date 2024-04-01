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
        Task EliminarFacturadas();
        Task<List<Preventa>> Listar(bool entregado);
        Task<List<Preventa>> Listar(string provincia, string canton, string distrito, bool? entregado = null);
        Task<Preventa> GetPreventaByNProforma(int nProforma);
        Task ActualizarOnly(Preventa entity);
    }
}
