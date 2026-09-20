using SQLite;
using System.ComponentModel.DataAnnotations;

namespace AppRuteoFactuSys.Models
{
    public class DevolucionLinea
    {
        [Key, PrimaryKey, AutoIncrement]
        public int IdDevolucionLinea { get; set; }
        public int IdDevolucion { get; set; } 
        public string Codpro { get; set; }
        public string UnidadMedida { get; set; }
        public string Detalle { get; set; }
        public decimal Preciounidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public string CodDescuento { get; set; }
        public decimal ImpuestoNeto { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Totallinea { get; set; }
        public decimal PorImpuesto { get; set; }
        public decimal SubtotalDescuento { get; set; }
        public decimal PorExonerado { get; set; }
        public decimal MontoExonerado { get; set; }
        public string CodigoImpuesto { get; set; }
        public string CodigoTarifa { get; set; }
        public string CodeCabys { get; set; }
        public bool UsaInventario { get; set; }
        public bool IsFarmaceutico { get; set; }
        public bool IsSurtido { get; set; }
    }
}
