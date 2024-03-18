using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.SqlLite
{
    public class SqlLiteConexion
    {
        public SqlLiteConexion()
        { 

        }
        public static SqliteConnection GetConnection()
        {

            // Obtener la ruta al directorio LocalApplicationData
            string directorySqlLite = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Nombre de tu archivo de base de datos SQLite
            string nombreArchivo = "RuteroApp.db"; // base de datos SQLite

            // Ruta completa al archivo de base de datos SQLite
            string databasePath = Path.Combine(directorySqlLite, nombreArchivo);

            // Cadena de conexión SQLite
            string connectionString = $"Data Source={databasePath}";

            // Crear y retornar la conexión a la base de datos SQLite
            return new SqliteConnection(connectionString);
        }
    }
}
