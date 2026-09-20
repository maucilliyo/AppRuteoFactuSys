using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalPreventaRepository
    {
        public async Task GuardarPreventa(Preventa preventa)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            await conn.RunInTransactionAsync(tran =>
            {
                tran.Insert(preventa);

                foreach (var linea in preventa.Lineas)
                {
                    linea.NProforma = preventa.Nproforma;
                    linea.LocalID = preventa.LocalID;

                    tran.Insert(linea);
                }
            });
            //using var transaction = conn.BeginTransaction();
            //try
            //{
            //    // SQL Insertar la Preventa
            //    string sqlInsertPreventa = @"
            //            INSERT INTO proforma
            //            (nproforma,cedcliente, fecha, condicionventa, formapago, totalcomprobante, totaldescuento, totalgrabado, totalexento, 
            //            totalimpuesto, totalventa,  codigomoneda, codvendedor, totalservgravados, totalservexentos, estado,
            //            notas, serviciosexonerados, mercanciasexoneradas, totalmercanciasgravadas, diasplazo,
            //            totalmercanciasexentas, totalventaneta, totalexonerado, nombre_cliente, terminal, id_usuario,fecha_update,entregado)
            //            VALUES
            //            (@Nproforma,@Cedcliente, @Fecha, @CondicionVenta, @Formapago, @TotalComprobante, @TotalDescuento, @TotalGrabado,
            //            @TotalExento, @TotalImpuesto, @TotalVenta, @CodigoMoneda, 0, @TotalServGravados, @TotalServExentos, 'Pendiente',
            //            @Notas, @ServiciosExonerados, @MercanciasExoneradas, @TotalMercanciasGravadas, 0, @TotalMercanciasExentas,
            //            @TotalVentaNeta, @TotalExonerado, @Nombre_Cliente, @Terminal, @Id_Usuario,@FechaUpdate,@Entregado);
            //            SELECT last_insert_rowid();";
            //    //INSERTA LA PREVENTA Y RETORNA EL UNTIMO ID
            //    int ultimoIdInsertado = await connection.ExecuteScalarAsync<int>(sqlInsertPreventa, preventa);
            //    //RECORRE LAS LINEAS PARA AGREGARLAS 
            //    foreach (var linea in preventa.Lineas)
            //    {
            //        //asignanado las claves
            //        linea.NProforma = preventa.Nproforma;
            //        linea.LocalID = ultimoIdInsertado;
            //        //METODO PARA AGREGAR LINEA
            //        await AgregaLinea(linea, connection);
            //    }
            //    // Commit de la transacción
            //    transaction.Commit();
            //}
            //catch (Exception ex)
            //{
            //    // Rollback en caso de error
            //    transaction.Rollback();
            //    throw new Exception("Error al guardar la preventa " + ex.Message);
            //}
        }
        public async Task<List<Preventa>> GetPreventas(bool? entregado = null)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            string sql = """
                        SELECT *
                        FROM preventa
                        WHERE entregado = COALESCE(?, entregado)
                          AND estado != 'Facturado'
                        ORDER BY fecha DESC
                        """;

            int? entregadoValue = entregado.HasValue ? entregado.Value ? 1 : 0 : null;

            return await conn.QueryAsync<Preventa>(sql, entregadoValue);
            //string sql = @"SELECT * FROM proforma 
            //                   WHERE entregado = COALESCE(@entregado, entregado) and estado != 'Facturado' 
            //                   ORDER BY fecha DESC;";
            //connection.Open();

            //var response = await connection.QueryAsync<Preventa>(sql, new { entregado });

            //return response.ToList();
        }
        public async Task<List<Preventa>> GetPreventas(string provincia, string canton, string distrito, bool? entregado = null)
        {
            var connection = await SqlLiteDatabase.GetConnection();

            string sql = """
                            SELECT p.*
                            FROM Preventa p
                            INNER JOIN cliente c ON p.cedcliente = c.cedula
                            WHERE p.entregado = COALESCE(?, p.entregado)
                              AND p.estado != 'Facturado'
                              AND c.provincia = ?
                              AND c.canton = ?
                              AND c.distrito = ?
                            ORDER BY p.fecha DESC
                            """;

            return await connection.QueryAsync<Preventa>(sql, entregado, provincia, canton, distrito);
        }
        public async Task<Preventa> GetPreventaByNProforma(int nProforma)
        {

            var conexion = await SqlLiteDatabase.GetConnection();

            var preventa = await conexion.Table<Preventa>().Where(p => p.Nproforma == nProforma).FirstOrDefaultAsync();

            if (preventa == null) return null;

            preventa.Lineas = await conexion.Table<PreventaLineas>().Where(l => l.NProforma == nProforma).ToListAsync();

            return preventa;

            //using var connection = SqlLiteConexion.GetConnection();
            //connection.Open();

            //string sql = "SELECT * FROM proforma WHERE nproforma = @nProforma;";
            //var response = await connection.QueryAsync<Preventa>(sql, new { nProforma });

            //var proforma = response.FirstOrDefault();

            //if (proforma == null) return null;

            //string sqlLineas = "SELECT * FROM lineasproforma where n_proforma =@nProforma;";

            //var lineas = await connection.QueryAsync<PreventaLineas>(sqlLineas, new { nProforma });

            //proforma.Lineas = lineas.ToList();


            //return proforma;
        }
        public async Task<Preventa> GetPreventaById(int LocalID)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            var preventa = await conexion.Table<Preventa>().Where(p => p.LocalID == LocalID).FirstOrDefaultAsync();

            if (preventa == null) return null;

            preventa.Lineas = await conexion.Table<PreventaLineas>().Where(l => l.LocalID == LocalID).ToListAsync();

            return preventa;


            //using var connection = SqlLiteConexion.GetConnection();
            //connection.Open();

            //string sql = "SELECT * FROM proforma WHERE LocalID = @LocalID;";
            //var response = await connection.QueryAsync<Preventa>(sql, new { LocalID });

            //var proforma = response.FirstOrDefault();

            //string sqlLineas = "SELECT * FROM lineasproforma where Local_ID =@LocalID;";

            //var lineas = await connection.QueryAsync<PreventaLineas>(sqlLineas, new { LocalID });

            //proforma.Lineas = lineas.ToList();

            //return proforma;
        }
        public async Task EliminarFacturadas()
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.RunInTransactionAsync(tran =>
            {
                tran.Execute(@"DELETE FROM PreventaLineas 
                        WHERE LocalID IN (SELECT LocalID FROM Preventa WHERE entregado = true);");

                tran.Execute("DELETE FROM Preventa WHERE entregado = true;");
            });
        }
        public async Task EliminarPreventas()
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.RunInTransactionAsync(tran =>
            {
                tran.Execute("DELETE FROM PreventaLineas WHERE LocalID IN (SELECT LocalID FROM Preventa);");
                tran.Execute("DELETE FROM Preventa;");
            });
        }
        public async Task ActualizarPreventaSync(Preventa preventa)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.RunInTransactionAsync(tran =>
            {
                tran.Update(preventa);
                tran.Execute("DELETE FROM PreventaLineas WHERE LocalID = ?", preventa.LocalID);

                foreach (var linea in preventa.Lineas)
                {
                    linea.NProforma = preventa.Nproforma;
                    linea.LocalID = preventa.LocalID;
                    tran.Insert(linea);
                }
            });

            //using var transaction = connection.BeginTransaction();
            //try
            //{
            //    // Actualizar la Preventa
            //    string sqlUpdatePreventa = @"
            //            UPDATE proforma
            //            SET
            //                cedcliente = @Cedcliente,
            //                fecha = @Fecha,
            //                condicionventa = @CondicionVenta,
            //                formapago = @Formapago,
            //                totalcomprobante = @TotalComprobante,
            //                totaldescuento = @TotalDescuento,
            //                totalgrabado = @TotalGrabado,
            //                totalexento = @TotalExento,
            //                totalimpuesto = @TotalImpuesto,
            //                totalventa = @TotalVenta,
            //                codigomoneda = @CodigoMoneda,
            //                totalservgravados = @TotalServGravados,
            //                totalservexentos = @TotalServExentos,
            //                notas = @Notas,
            //                serviciosexonerados = @ServiciosExonerados,
            //                mercanciasexoneradas = @MercanciasExoneradas,
            //                totalmercanciasgravadas = @TotalMercanciasGravadas,
            //                totalmercanciasexentas = @TotalMercanciasExentas,
            //                totalventaneta = @TotalVentaNeta,
            //                totalexonerado = @TotalExonerado,
            //                nombre_cliente = @Nombre_Cliente,
            //                terminal = @Terminal,
            //                id_usuario = @Id_Usuario,
            //                fecha_update = @FechaUpdate,
            //                estado=@Estado,
            //                entregado = @Entregado
            //            WHERE LocalID = @LocalID";

            //    await connection.ExecuteAsync(sqlUpdatePreventa, preventa);
            //    // Actualizar las líneas de la proforma
            //    string sqlDeleteLineas = @"DELETE FROM lineasproforma WHERE Local_ID = @LocalID";
            //    await connection.ExecuteAsync(sqlDeleteLineas, new { preventa.LocalID });
            //    //recorro las lineas la BD del servidor
            //    foreach (var linea in preventa.Lineas)
            //    {
            //        linea.LocalID = preventa.LocalID;
            //        await AgregaLinea(linea, connection);
            //    }
            //    // Commit de la transacción
            //    transaction.Commit();
            //}
            //catch (Exception ex)
            //{
            //    // Rollback en caso de error
            //    transaction.Rollback();
            //    throw new Exception("Error al actualizar la preventa " + ex.Message);
            //}
        }
        public async Task ActualizarPreventaAppOnly(Preventa preventa)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            await conexion.RunInTransactionAsync(tran =>
            {
                tran.Update(preventa);
                tran.Execute("DELETE FROM PreventaLineas WHERE LocalID = ?", preventa.LocalID);

                foreach (var linea in preventa.Lineas)
                {
                    linea.NProforma = preventa.Nproforma;
                    linea.LocalID = preventa.LocalID;
                    tran.Insert(linea);
                }
            });
            //using var connection = SqlLiteConexion.GetConnection();
            //connection.Open();

            //using var transaction = connection.BeginTransaction();
            //try
            //{
            //    // Actualizar la Preventa
            //    string sqlUpdatePreventa = @"
            //            UPDATE proforma
            //            SET
            //                cedcliente = @Cedcliente,
            //                fecha = @Fecha,
            //                condicionventa = @CondicionVenta,
            //                formapago = @Formapago,
            //                totalcomprobante = @TotalComprobante,
            //                totaldescuento = @TotalDescuento,
            //                totalgrabado = @TotalGrabado,
            //                totalexento = @TotalExento,
            //                totalimpuesto = @TotalImpuesto,
            //                totalventa = @TotalVenta,
            //                codigomoneda = @CodigoMoneda,
            //                totalservgravados = @TotalServGravados,
            //                totalservexentos = @TotalServExentos,
            //                notas = @Notas,
            //                serviciosexonerados = @ServiciosExonerados,
            //                mercanciasexoneradas = @MercanciasExoneradas,
            //                totalmercanciasgravadas = @TotalMercanciasGravadas,
            //                totalmercanciasexentas = @TotalMercanciasExentas,
            //                totalventaneta = @TotalVentaNeta,
            //                totalexonerado = @TotalExonerado,
            //                nombre_cliente = @Nombre_Cliente,
            //                terminal = @Terminal,
            //                id_usuario = @Id_Usuario,
            //                fecha_update = @FechaUpdate,
            //                estado=@Estado,
            //                entregado = @Entregado
            //            WHERE LocalID = @LocalID";

            //    await connection.ExecuteAsync(sqlUpdatePreventa, preventa);
            //    // Actualizar las líneas de la proforma
            //    //ELIMINA TODAS LAS LINEAS 
            //    await EliminaTodasLineasById(preventa.LocalID, connection);
            //    foreach (var linea in preventa.Lineas)
            //    {
            //        linea.LocalID = preventa.LocalID;
            //        //LA VUELVE A AGREGAR
            //        await AgregaLinea(linea, connection);
            //    }
            //    // Commit de la transacción
            //    transaction.Commit();
            //}
            //catch (Exception ex)
            //{
            //    // Rollback en caso de error
            //    transaction.Rollback();
            //    throw new Exception("Error al actualizar la preventa " + ex.Message);
            //}
        }
        public async Task ActualizarEntregado(int localID)
        {
            var conexion = await SqlLiteDatabase.GetConnection();

            var preventa = await conexion.FindAsync<Preventa>(localID);

            if (preventa == null)
                return;

            preventa.Entregado = true;
            preventa.Estado = "Entregado";

            await conexion.UpdateAsync(preventa);
        }
        public async Task ActualizarNumeroProforma(int Nproforma, int LocalID)
        {
            try
            {
                var conexion = await SqlLiteDatabase.GetConnection();

                string sqlUpdatePreventa = @"UPDATE Preventa
                                             SET Nproforma = ?
                                             WHERE LocalID = ?";

                await conexion.ExecuteAsync(sqlUpdatePreventa, Nproforma, LocalID);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #region METODOS PARA LAS LINEAS
        private async Task EliminaTodasLineasById(int LocalID, SqliteConnection connection)
        {
            string sqlEliminarLiea = @"DELETE FROM lineasproforma WHERE Local_ID = @LocalID;";

            await connection.ExecuteAsync(sqlEliminarLiea, new { LocalID });
        }
        private async Task AgregaLinea(PreventaLineas linea, SqliteConnection connection)
        {
            // Insertar las líneas de la proforma
            string sqlInsertLinea = @"INSERT INTO lineasproforma 
                  (Local_ID, n_proforma, linea, codpro, unidadmedida, detalle, preciounidad, cantidad, subtotal, descuento,cod_descuento, impuesto, totallinea, montoexonerado, 
                    porimpuesto, subtotaldescuento, impuestoneto, porexonerado, codigo_impuesto, codigo_tarifa, codecabys,por_descuento) 
              VALUES 
                  (@LocalID, @NProforma, @Linea, @Codpro, @UnidadMedida, @Detalle, @PrecioUnidad, @Cantidad, @Subtotal, @Descuento,@CodDescuento, @Impuesto, @TotalLinea,
                  @Montoexonerado, @Porimpuesto, @Subtotaldescuento, @Impuestoneto, @Porexonerado, @CodigoImpuesto, @CodigoTarifa, @CodeCabys ,@PorDescuento);";
            //insertanto la linea
            await connection.ExecuteAsync(sqlInsertLinea, new
            {
                linea.LocalID,
                linea.NProforma,
                linea.Linea,
                linea.Codpro,
                linea.UnidadMedida,
                linea.Detalle,
                linea.PrecioUnidad,
                linea.Cantidad,
                linea.Subtotal,
                linea.PorDescuento,
                linea.Descuento,
                linea.CodDescuento,
                linea.Impuesto,
                linea.TotalLinea,
                linea.Montoexonerado,
                linea.Porimpuesto,
                linea.Subtotaldescuento,
                linea.Impuestoneto,
                linea.Porexonerado,
                linea.CodigoImpuesto,
                linea.CodigoTarifa,
                linea.CodeCabys
            });
        }

        #endregion
    }
}
