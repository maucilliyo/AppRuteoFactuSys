using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Models
{
    public class Preventa
    {

        public Preventa()
        {
            this.Lineas = [];
        }
        public int LocalID { get; set; } 
        public int Nproforma { get; set; }
        public bool Modificar { get; set; }
        public string Cedcliente { get; set; }
        public string Nombre_Cliente { get; set; }
        public string TipoClienteImpuesto { get; set; }
        public DateTime Fecha { get; set; }
        public string CondicionVenta { get; set; }
        public string Formapago { get; set; }
        public string CodigoMoneda { get; set; }
        public decimal TotalServGravados { get; set; }
        public decimal TotalServExentos { get; set; }
        public decimal ServiciosExonerados { get; set; }
        public decimal TotalMercanciasGravadas { get; set; }
        public decimal TotalMercanciasExentas { get; set; }
        public decimal MercanciasExoneradas { get; set; }
        public decimal TotalGrabado { get; set; }
        public decimal TotalExento { get; set; }
        public decimal TotalExonerado { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal TotalVentaNeta { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalComprobante { get; set; }
        public bool Facturado { get; set; }
        public string Terminal { get; set; }
        public int Id_Usuario { get; set; }
        public int? DiasPlazo { get; set; }
        public string Notas { get; set; }
        public DateTime FechaUpdate { get; set; }
        public bool Entregado { get; set; }
        public List<PreventaLineas> Lineas { get; set; }

    }
}
