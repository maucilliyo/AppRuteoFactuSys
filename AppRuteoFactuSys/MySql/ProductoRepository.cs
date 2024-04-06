using AppRuteoFactuSys.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.MySql
{
    public class ProductoRepository
    {
        public async Task<List<Producto>> GetProductos()
        {
            //VALIDAR SI TIENE SYNC
            if (!await Conexion.IsSyncApp()) return null;

            using (var conn = await Conexion.GetConnection())
            {
                string sql = @"select * from productos p 
                               inner join inventario i on p.codpro = i.codpro
                               where activo = true";

                var response = await conn.QueryAsync<Producto>(sql);

                return response.ToList();
            }
        }
        public async Task<Producto> GetProductoByCodigo()
        {
            using (var conn = await Conexion.GetConnection())
            {
                string sql =  @"select * from productos p 
                                inner join inventario i on p.codpro = i.codpro
                                where p.codpro = @codpro and activo = true";

                var response = await conn.QueryAsync<Producto>(sql);

                return response.FirstOrDefault();
            }
        }
    }
}
