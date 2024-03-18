using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Models
{
    [Table("lineasproforma")]
    public class PreventaLineas
    {
        public int id_linea { get; set; }
        public int LocalID { get; set; }

        private decimal _Porexonerado;
        public int NProforma { get; set; }
        public int Linea { get; set; }
        public string Codpro { get; set; }
        public string UnidadMedida { get; set; }
        public string Detalle { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Subtotal { get => Math.Round(PrecioUnidad * Cantidad, 2); }
        public decimal PorDescuento { get; set; }
        public decimal Descuento { get => Math.Round(Subtotal * PorDescuento, 2); set { } }
        public decimal Subtotaldescuento { get => Math.Round(Subtotal - Descuento, 2); }
        public decimal Porimpuesto { get; set; }
        public decimal Impuesto { get => Math.Round(Subtotaldescuento * Porimpuesto, 2); }
        public decimal Porexonerado
        {
            get => _Porexonerado;
            set
            {
                if (value > Porimpuesto)
                    _Porexonerado = Porimpuesto;
                else
                    _Porexonerado = value;
            }
        }
        public decimal Montoexonerado { get => Math.Round(Subtotaldescuento * Porexonerado, 2); }
        public decimal Impuestoneto { get => Math.Round(Impuesto - Montoexonerado, 2); }
        public decimal TotalLinea { get => Math.Round(Subtotaldescuento + Impuestoneto, 2); }
        public string CodigoImpuesto { get; set; }
        public string CodigoTarifa { get; set; }
        public string CodeCabys { get; set; }
        public bool UsaInventario { get; set; }
    }
}
