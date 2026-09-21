using Dommel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IgnoreAttribute = SQLite.IgnoreAttribute;
using TableAttribute = System.ComponentModel.DataAnnotations.Schema.TableAttribute;


namespace AppRuteoFactuSys.Models
{
    [Table("devoluciones")]
    public class Devolucion
    {
 
        [Key,PrimaryKey, AutoIncrement]
        public int IdDevolucion { get; set; }
        public string CedCliente { get; set; }
        public string Nombre { get; set; }
        [IgnoreInsert]
        public DateTime FechaEmision { get; set; }
        public decimal TotalComprobante { get; set; }
        public bool Enviado { get; set; }
        [NotMapped, Ignore]
        public List<DevolucionLinea> Lineas { get; set; } = [];
    }
}
