using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Models
{
    public class Cliente
    {
        [Key]
        public string Cedula { get; set; }
        public string TipoCedula { get; set; }
        public string Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        public string? CodigoProvincia { get; set; }
        public string? CodigoCanton { get; set; }
        public string? CodigoDistrito { get; set; }
        public string? OtrasSenas { get; set; }
        public string? Contacto { get; set; }
        public Nullable<bool> Credito { get; set; }
        public Nullable<int> TipoCliente { get; set; }
        public Nullable<int> PorDescuento { get; set; }
        public Nullable<int> DiasCredito { get; set; }
        public string? TipoClienteImpuesto { get; set; }
        public string? TipoDocPreferido { get; set; }
        public string? TipoPrecio { get; set; }
        public DateTime FechaUpdate { get; set; }
    }
}
