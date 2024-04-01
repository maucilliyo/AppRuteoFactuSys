using AppRuteoFactuSys.Models;
using Dapper;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalProductoRepository
    {
        public async Task<Producto> GetProductoByCodigo(string CodPro)
        {
            SQLiteInitialization.InitializeDatabase();

            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT * FROM Productos WHERE codpro = @CodPro";

                await conn.OpenAsync();
                var response = await conn.QueryAsync<Producto>(sql, new { CodPro });

                return response.FirstOrDefault();
            }
        }
        public async Task<List<Producto>> GetProductos(string? detalle = null)
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT * FROM Productos WHERE detalle LIKE '%' || COALESCE(@detalle, detalle) || '%' LIMIT 600";
                await conn.OpenAsync();
                var response = await conn.QueryAsync<Producto>(sql, new { detalle });
                return response.ToList();
            }
        }
        public async Task Agregar(Producto producto)
        {
            // Consulta SQL para la inserción
            string sqlInsert = @"
            INSERT INTO productos (
                Nombre, CodPro, Cod_Proveedor, Detalle, CodBarras, Granel, 
                ExitenciaMinima, Lote, UnidadMedida, Descripcion, PorcientoImpuesto,
                VenderNegativo, ExistenciaMaxima, CodigoImpuesto, CodigoTarifa,
                CodigoCabys, UsaInventario, Agroinsumo, Oferta, IdDepartamento,
                PrecioCompra, Ganacia, PrecioVenta, PrecioVentaA, PrecioVentaB,
                PorGanacia, Stock, Bodega, Pasillo, Estante, Ubicacion, Caja,
                Activo, Imagen, TipoUpdateProduc, ImpreBodegaCocina, FechaUpdate,
                FechaUltimaVenta
            )
            VALUES (
                @Nombre, @CodPro, @CodProveedor, @Detalle, @CodBarras, @Granel, 
                @ExitenciaMinima, @Lote, @UnidadMedida, @Descripcion, @PorcientoImpuesto,
                @VenderNegativo, @ExistenciaMaxima, @CodigoImpuesto, @CodigoTarifa,
                @CodigoCabys, @UsaInventario, @Agroinsumo, @Oferta, @IdDepartamento,
                @PrecioCompra, @Ganacia, @PrecioVenta, @PrecioVentaA, @PrecioVentaB,
                @PorGanacia, @Stock, @Bodega, @Pasillo, @Estante, @Ubicacion, @Caja,
                @Activo, @Imagen, @TipoUpdateProduc, @ImpreBodegaCocina, @FechaUpdate,
                @FechaUltimaVenta
            )";

            // Crear una conexión a la base de datos SQLite
            using (var connection = SqlLiteConexion.GetConnection())
            {
                // Ejecutar la consulta SQL utilizando Dapper
               await connection.ExecuteAsync(sqlInsert, producto);
            }
        }
        public async Task Actualizar(Producto producto)
        {
            // Consulta SQL para la actualización
            string sqlUpdate = @"
             UPDATE productos
             SET 
                 Nombre = @Nombre,
                 Cod_Proveedor = @CodProveedor,
                 Detalle = @Detalle,
                 CodBarras = @CodBarras,
                 Granel = @Granel,
                 ExitenciaMinima = @ExitenciaMinima,
                 Lote = @Lote,
                 UnidadMedida = @UnidadMedida,
                 Descripcion = @Descripcion,
                 PorcientoImpuesto = @PorcientoImpuesto,
                 VenderNegativo = @VenderNegativo,
                 ExistenciaMaxima = @ExistenciaMaxima,
                 CodigoImpuesto = @CodigoImpuesto,
                 CodigoTarifa = @CodigoTarifa,
                 CodigoCabys = @CodigoCabys,
                 UsaInventario = @UsaInventario,
                 Agroinsumo = @Agroinsumo,
                 Oferta = @Oferta,
                 IdDepartamento = @IdDepartamento,
                 PrecioCompra = @PrecioCompra,
                 Ganacia = @Ganacia,
                 PrecioVenta = @PrecioVenta,
                 PrecioVentaA = @PrecioVentaA,
                 PrecioVentaB = @PrecioVentaB,
                 PorGanacia = @PorGanacia,
                 Stock = @Stock,
                 Bodega = @Bodega,
                 Pasillo = @Pasillo,
                 Estante = @Estante,
                 Ubicacion = @Ubicacion,
                 Caja = @Caja,
                 Activo = @Activo,
                 Imagen = @Imagen,
                 TipoUpdateProduc = @TipoUpdateProduc,
                 ImpreBodegaCocina = @ImpreBodegaCocina,
                 FechaUpdate = @FechaUpdate,
                 FechaUltimaVenta = @FechaUltimaVenta
             WHERE
                   CodPro = @CodPro;"; // Asumiendo que hay un campo 'Id' que identifica de forma única al producto

            // Crear una conexión a la base de datos SQLite
            using (var connection = SqlLiteConexion.GetConnection())
            {
                // Ejecutar la consulta SQL utilizando Dapper
                await connection.ExecuteAsync(sqlUpdate, producto);
            }
        }
    }
}
