using AppRuteoFactuSys.Models;
using SQLite;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalProductoRepository
    {
        public async Task<Producto?> GetProductoByCodigo(string codPro)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            return await conexion.FindAsync<Producto>(codPro);
        }

        public async Task<List<Producto>> GetProductos(
            string? detalle = null)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            if (string.IsNullOrWhiteSpace(detalle))
            {
                return await conexion.Table<Producto>()
                                      .Take(600)
                                      .ToListAsync();
            }

            return await conexion.Table<Producto>()
                                  .Where(x => x.Detalle.Contains(detalle))
                                  .Take(600)
                                  .ToListAsync();
        }

        public async Task<List<Producto>> GetAllProductos(
            string? detalle = null)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            return await conexion.Table<Producto>()
                .Take(100)
                                 .ToListAsync();
        }

        public async Task Agregar(Producto producto)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.InsertAsync(producto);
        }

        public async Task Actualizar(Producto producto)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.UpdateAsync(producto);
        }
    }
}