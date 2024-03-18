using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Models
{
    public class Producto
    {
        public string Nombre { get; set; }
        public string CodPro { get; set; }
        public string CodProveedor { get; set; }
        public string Detalle { get; set; }
        public string CodBarras { get; set; }
        public bool Granel { get; set; }
        public decimal ExitenciaMinima { get; set; }
        public bool Lote { get; set; }
        public string UnidadMedida { get; set; }
        public string Descripcion { get; set; }
        public decimal PorcientoImpuesto { get; set; }
        public bool VenderNegativo { get; set; }
        public decimal ExistenciaMaxima { get; set; }
        public string CodigoImpuesto { get; set; }
        public string CodigoTarifa { get; set; }
        public string CodigoCabys { get; set; }
        public bool UsaInventario { get; set; }
        public bool Agroinsumo { get; set; }
        public bool Oferta { get; set; }
        public string IdDepartamento { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Ganacia { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaA { get; set; }
        public decimal PrecioVentaB { get; set; }
        public decimal PorGanacia { get; set; }
        public decimal Stock { get; set; }
        public string Bodega { get; set; }
        public string Pasillo { get; set; }
        public string Estante { get; set; }
        public string Ubicacion { get; set; }
        public string Caja { get; set; }
        public bool Activo { get; set; }
        public byte[] Imagen { get; set; }
        public string TipoUpdateProduc { get; set; }
        public bool ImpreBodegaCocina { get; set; }
        public DateTime FechaUpdate { get; set; }
        public DateTime FechaUltimaVenta { get; set; }
    }
}
