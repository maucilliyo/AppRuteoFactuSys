using Dommel;
using SQLite;
using System.ComponentModel.DataAnnotations.Schema;
using IgnoreAttribute = SQLite.IgnoreAttribute;

namespace AppRuteoFactuSys.Models
{
    public class Devolucion
    {
 
        [PrimaryKey, AutoIncrement]
        public int IdDevolucion { get; set; }
        public string Cedcliente { get; set; }
        public string Nombre { get; set; }
        [IgnoreInsert]
        public DateTime FechaEmision { get; set; }
        public decimal TotalComprobante { get; set; }
        public bool Enviado { get; set; }
        [NotMapped, Ignore]
        public List<DevolucionLinea> Lineas { get; set; } = [];
    }
}
