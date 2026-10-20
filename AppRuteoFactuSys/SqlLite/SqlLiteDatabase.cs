using AppRuteoFactuSys.Models;
using SQLite;

namespace AppRuteoFactuSys.SqlLite
{
    public static class SqlLiteDatabase
    {
        private static readonly string RutaBaseDatos =  Path.Combine(FileSystem.AppDataDirectory, "RuteroApp.db3");
        private static SQLiteAsyncConnection _conexion;
        public static async Task<SQLiteAsyncConnection> GetConnection()
        {
            if (_conexion != null) return _conexion;
 

            _conexion = new SQLiteAsyncConnection(RutaBaseDatos);

            // Crea la tabla usando las entidades
            await _conexion.CreateTableAsync<Cliente>();
            await _conexion.CreateTableAsync<Producto>();
            await _conexion.CreateTableAsync<Preventa>();
            await _conexion.CreateTableAsync<PreventaLineas>();
            await _conexion.CreateTableAsync<Devolucion>();
            await _conexion.CreateTableAsync<DevolucionLinea>();

            return _conexion;
        }
        public static async Task ClearDatabase()
        {
            try
            {
                await BorrarBaseDatos();
                var conexion = await SqlLiteDatabase.GetConnection();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public static async Task BorrarBaseDatos()
        {
            if (_conexion != null)
            {
                await _conexion.CloseAsync();
                _conexion = null; // fuerza que GetConnection() la vuelva a crear
            }

            foreach (var archivo in new[] { RutaBaseDatos, RutaBaseDatos + "-wal", RutaBaseDatos + "-shm", RutaBaseDatos + "-journal" })
            {
                if (File.Exists(archivo))
                    File.Delete(archivo);
            }
        }
    }
}
