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
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();
            try
            {
                var id = await conn.InsertAsync(devolucion);
                devolucion.IdDevolucion = Convert.ToInt32(id);

                foreach (var linea in devolucion.Lineas)
                {
                    linea.IdDevolucion = devolucion.IdDevolucion;

                    await conn.InsertAsync(linea);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}
