using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using static Android.Telecom.Call;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalClienteRepository
    {
        public async Task<Cliente> GetClienteByCedula(string cedula)
        {

            var conn = await SqlLiteDatabase.GetConnection();

            var cliente = await conn.Table<Cliente>()
                                    .Where(x => x.Cedula == cedula)
                                    .FirstOrDefaultAsync();
            return cliente;
        }
        public async Task<List<Cliente>> GetClientes(string? nombre = null)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return await conn.QueryAsync<Cliente>(
                    "SELECT * FROM cliente");
            }

            return await conn.QueryAsync<Cliente>(
                """
                    SELECT *
                    FROM cliente
                    WHERE nombre LIKE ?
                    """,
                $"%{nombre}%");
        }
        public async Task<List<Cliente>> GetAllClientes()
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var clientes = await conn.Table<Cliente>()
                                    .ToListAsync();
            return clientes;


        }
        public async Task Agregar(Cliente cliente)
        {
            try
            {
                var conn = await SqlLiteDatabase.GetConnection();
                // Insertar el producto principal
                await conn.InsertOrReplaceAsync(cliente);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task Actualizar(Cliente cliente)
        {
            try
            {
                var conn = await SqlLiteDatabase.GetConnection();
                // Insertar el producto principal
                await conn.UpdateAsync(cliente);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<string>> FiltroByCedula()
        {
            return null;
        }
    }
}
