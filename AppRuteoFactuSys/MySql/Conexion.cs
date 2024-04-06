using AppRuteoFactuSys.Models;
using Dapper;
using MySqlConnector;
using Newtonsoft.Json;
using System.Data;

namespace AppRuteoFactuSys.MySql
{
    public class Conexion
    {
        private static string usuario = "root"; //Usuario de acceso a MySQL
        private static string password = "3380"; //Contraseña de usuario de acceso a MySQL
        private static string TimeOut = "5";
        public static string? DataBase;
        public static string? Puerto;
        public static string? IP;
        public static async Task<MySqlConnection> GetConnection()
        {
            await GetConfig();

            string cadenaConexion = @"Database=" + DataBase + ";Port=" + Puerto + "; Data Source=" + IP + "; User Id=" + usuario + ";" +
                " Password=" + password + "; AllowUserVariables=True;Connect Timeout=" + TimeOut + ";";

            MySqlConnection connection = new()
            {
                ConnectionString = cadenaConexion,
            };

            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                else
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return connection;
        }
        public static async Task<bool> GetConfig()
        {
            var getFile = await SecureStorage.GetAsync("conexion.json"); ;
            if (getFile == null)
            {
                return false;
            }
            else
            {
                var conn = JsonConvert.DeserializeObject<Setings>(getFile.ToString());
                if (conn != null)
                {
                    Puerto = conn.Puerto;
                    IP = conn.IP;
                    DataBase = conn.DataBase;
                }
            }
            return true;
        }
        public static async Task<bool> IsSyncApp()
        {
            using (var conn = await GetConnection())
            {
                string sql = "SELECT COUNT(*) FROM info_data_base WHERE syncApp = true";
                // Utilizamos el método ExecuteScalarAsync<T> de Dapper para obtener el recuento.
                // Si es mayor que cero, significa que hay al menos un registro
                // donde syncApp es 'true', entonces devolvemos true. 
                // De lo contrario, devolvemos false.
                return await conn.ExecuteScalarAsync<int>(sql) > 0;
            }
        }
        public static async Task<string> TestConexion()
        {
            var getConnFile = await GetConfig();

            if (getConnFile)
            {
                var conn = await GetConnection();
                if (conn == null)
                {
                    return "ERROR DE CONEXIÓN";
                }
                else
                {
                    return "CONEXIÓN EXITOSA!";
                }
            }
            else
            {
                return "ERROR CON EL ARCHIVO CONFIG";
            }
        }
    }
}
