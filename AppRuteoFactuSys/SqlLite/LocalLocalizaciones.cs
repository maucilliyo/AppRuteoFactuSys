using AppRuteoFactuSys.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalLocalizaciones
    {
        public async Task<List<string>> GetProvincias()
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT DISTINCT provincia FROM clientes;";
                await conn.OpenAsync();
                var response = await conn.QueryAsync<string>(sql);
                return response.ToList();
            }
        }
        public async Task<List<string>> GetCantones(string provincia)
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT DISTINCT canton FROM clientes where provincia =@provincia;";
                await conn.OpenAsync();
                var response = await conn.QueryAsync<string>(sql, new { provincia });
                return response.ToList();
            }
        }
        public async Task<List<string>> GetDistritos(string canton)
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT DISTINCT distrito FROM clientes where canton=@canton;";
                await conn.OpenAsync();
                var response = await conn.QueryAsync<string>(sql, new { canton });
                return response.ToList();
            }
        }
    }
}
