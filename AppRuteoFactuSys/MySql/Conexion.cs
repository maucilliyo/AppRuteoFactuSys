using AppRuteoFactuSys.Models;
using Microsoft.VisualBasic;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            MySqlConnection connection = new MySqlConnection
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
        public static async Task<string> TestConexion()
        {
            var getConnFile = await GetConfig();

            if (getConnFile)
            {
                var conn = GetConnection();
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
