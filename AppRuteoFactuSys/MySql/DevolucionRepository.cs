using AppRuteoFactuSys.Models;
using Dommel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.MySql
{
    public class DevolucionRepository
    {

        public DevolucionRepository()
        {

        }

        public async Task InsertarAsync(Devolucion devolucion)
        {
            using var conn = await Conexion.GetConnection();
    
            using var transaction = conn.BeginTransaction();
            try
            {
                var id = await conn.InsertAsync(devolucion,transaction);
                devolucion.IdDevolucion = Convert.ToInt32(id);

                foreach (var linea in devolucion.Lineas)
                {
                    linea.IdDevolucion = devolucion.IdDevolucion;

                    await conn.InsertAsync(linea,transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        //public async Task<Devolucion> GetDevolucionById(int idDevolucion)
        //{
        //    using var conn = await Conexion.GetConnection();

        //    var devolucion = await conn.From<Devolucion>()
        //                                 .Where(x => x.IdDevolucion == idDevolucion)
        //                                 .FirstOrDefaultAsync();
        //    if (devolucion != null)
        //        devolucion.Lineas = await conn.From<DevolucionLinea>()
        //                                      .Where(x => x.IdDevolucion == idDevolucion)
        //                                      .OrderByDescending(x=> x.IdDevolucionLinea)
        //                                      .ToListAsync();
        //    return devolucion;
        //}

    }
}
