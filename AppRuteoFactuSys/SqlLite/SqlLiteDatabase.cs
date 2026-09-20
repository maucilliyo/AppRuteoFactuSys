using AppRuteoFactuSys.Models;
using SQLite;

namespace AppRuteoFactuSys.SqlLite
{
    public static class SqlLiteDatabase
    {
        private static SQLiteAsyncConnection _conexion;
        public static async Task<SQLiteAsyncConnection> GetConnection()
        {
            if (_conexion != null) return _conexion;
            string nombreBaseDatos = "RuteroApp.db3";
            string rutaBaseDatos = Path.Combine(FileSystem.AppDataDirectory, nombreBaseDatos);

            _conexion = new SQLiteAsyncConnection(rutaBaseDatos);

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
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.RunInTransactionAsync(tran =>
            {
                // Primero las tablas hijas, luego las padres
                tran.Execute("DELETE FROM PreventaLineas");
                tran.Execute("DELETE FROM Preventa");
                tran.Execute("DELETE FROM DevolucionLinea");
                tran.Execute("DELETE FROM Devolucion");
                tran.Execute("DELETE FROM cliente");
                tran.Execute("DELETE FROM producto");
                //  reinicia los autoincrement
                tran.Execute("DELETE FROM sqlite_sequence");
            });

            // Fuera de la transacción
            await conexion.ExecuteAsync("VACUUM");
        }
    }
}
