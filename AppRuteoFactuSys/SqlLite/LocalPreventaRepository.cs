using AppRuteoFactuSys.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalPreventaRepository
    {
        public async Task GuardarPreventa(Preventa preventa)
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // SQL Insertar la Preventa
                        string sqlInsertPreventa = @"
                        INSERT INTO proforma
                        (nproforma,cedcliente, fecha, condicionventa, formapago, totalcomprobante, totaldescuento, totalgrabado, totalexento, 
                        totalimpuesto, totalventa,  codigomoneda, codvendedor, totalservgravados, totalservexentos, facturado,
                        notas, serviciosexonerados, mercanciasexoneradas, totalmercanciasgravadas, diasplazo,
                        totalmercanciasexentas, totalventaneta, totalexonerado, nombre_cliente, terminal, id_usuario,fecha_update,entregado)
                        VALUES
                        (@Nproforma,@Cedcliente, datetime('now'), @CondicionVenta, @Formapago, @TotalComprobante, @TotalDescuento, @TotalGrabado,
                        @TotalExento, @TotalImpuesto, @TotalVenta, @CodigoMoneda, 0, @TotalServGravados, @TotalServExentos, 0,
                        @Notas, @ServiciosExonerados, @MercanciasExoneradas, @TotalMercanciasGravadas, 0, @TotalMercanciasExentas,
                        @TotalVentaNeta, @TotalExonerado, @Nombre_Cliente, @Terminal, @Id_Usuario,@FechaUpdate,@Entregado);
                        SELECT last_insert_rowid();";
                        //INSERTA LA PREVENTA Y RETORNA EL UNTIMO ID
                        int ultimoIdInsertado = await connection.ExecuteScalarAsync<int>(sqlInsertPreventa, preventa);
                        //RECORRE LAS LINEAS PARA AGREGARLAS 
                        foreach (var linea in preventa.Lineas)
                        {
                            //asignanado las claves
                            linea.NProforma = preventa.Nproforma;
                            linea.LocalID = ultimoIdInsertado;
                            //METODO PARA AGREGAR LINEA
                            await AgregaLinea(linea, connection);
                        }
                        // Commit de la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback en caso de error
                        transaction.Rollback();
                        throw new Exception("Error al guardar la preventa " + ex.Message);
                    }
                }
            }
        }
        public async Task<List<Preventa>> GetPreventas()
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                string sql = "SELECT * FROM proforma;";
                connection.Open();

                var response = await connection.QueryAsync<Preventa>(sql);

                return response.ToList();
            }
        }
        public async Task<Preventa> GetPreventaByNProforma(int nProforma)
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM proforma WHERE nproforma = @nProforma;";
                var response = await connection.QueryAsync<Preventa>(sql, new { nProforma });

                var proforma = response.FirstOrDefault();

                string sqlLineas = "SELECT * FROM lineasproforma where n_proforma =@nProforma;";

                var lineas = await connection.QueryAsync<PreventaLineas>(sqlLineas, new { nProforma });

                proforma.Lineas = lineas.ToList();


                return proforma;
            }
        }
        public async Task<Preventa> GetPreventaById(int LocalID)
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM proforma WHERE LocalID = @LocalID;";
                var response = await connection.QueryAsync<Preventa>(sql, new { LocalID });

                var proforma = response.FirstOrDefault();

                string sqlLineas = "SELECT * FROM lineasproforma where Local_ID =@LocalID;";

                var lineas = await connection.QueryAsync<PreventaLineas>(sqlLineas, new { LocalID });

                proforma.Lineas = lineas.ToList();

                return proforma;
            }
        }
        public async Task ActualizarPreventa(Preventa preventa)
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Actualizar la Preventa
                        string sqlUpdatePreventa = @"
                        UPDATE proforma
                        SET
                            cedcliente = @Cedcliente,
                            fecha = @Fecha,
                            condicionventa = @CondicionVenta,
                            formapago = @Formapago,
                            totalcomprobante = @TotalComprobante,
                            totaldescuento = @TotalDescuento,
                            totalgrabado = @TotalGrabado,
                            totalexento = @TotalExento,
                            totalimpuesto = @TotalImpuesto,
                            totalventa = @TotalVenta,
                            codigomoneda = @CodigoMoneda,
                            totalservgravados = @TotalServGravados,
                            totalservexentos = @TotalServExentos,
                            notas = @Notas,
                            serviciosexonerados = @ServiciosExonerados,
                            mercanciasexoneradas = @MercanciasExoneradas,
                            totalmercanciasgravadas = @TotalMercanciasGravadas,
                            totalmercanciasexentas = @TotalMercanciasExentas,
                            totalventaneta = @TotalVentaNeta,
                            totalexonerado = @TotalExonerado,
                            nombre_cliente = @Nombre_Cliente,
                            terminal = @Terminal,
                            id_usuario = @Id_Usuario,
                            fecha_update = @FechaUpdate,
                            entregado = @Entregado
                        WHERE Local_ID = @LocalID";

                        await connection.ExecuteAsync(sqlUpdatePreventa, preventa);
                        // Actualizar las líneas de la proforma
                        //obtengo la preventa de la base de datos de la app
                        var preventaApp = await GetPreventaByNProforma(preventa.Nproforma);

                        //ELIMINA LAS LINEAS QUE ESTEN DE LADO DE LA APP PERO YA NO EN EL LADO DEL SERVIDOR
                        foreach (var linea in preventaApp.Lineas)
                        {
                            if (!preventa.Lineas.Any(l => l.Codpro == linea.Codpro))
                            {
                                await EliminaLinea(linea, connection);
                            }

                        }
                        //recorro las lineas la BD del servidor
                        foreach (var linea in preventa.Lineas)
                        {
                            // Comparar con las nuevas líneas y actualizar, agregar o eliminar según sea necesario
                            //reviso si la linea existe
                            if (preventaApp.Lineas.Any(l => l.Codpro == linea.Codpro))
                            {
                                await AcualizaLinea(linea, connection);
                            }
                            else// si no existe la agrega 
                            {
                                linea.LocalID = preventaApp.LocalID;
                                await AgregaLinea(linea, connection);
                            }
                        }
                        // Commit de la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback en caso de error
                        transaction.Rollback();
                        throw new Exception("Error al actualizar la preventa " + ex.Message);
                    }
                }
            }
        }
        public async Task ActualizarNProforma(int Nproforma, int LocalID)
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();
                // Actualizar la Preventa
                string sqlUpdatePreventa = @"
                        UPDATE proforma
                        SET Nproforma = @Nproforma
                        WHERE LocalID = @LocalID";

              await  connection.ExecuteAsync(sqlUpdatePreventa, new { Nproforma, LocalID });
            }
        }

        #region METODOS PARA LAS LINEAS
        private async Task EliminaLinea(PreventaLineas linea, SqliteConnection connection)
        {
            string sqlEliminarLiea = @"DELETE FROM lineasproforma WHERE n_proforma = @NProforma AND Codpro = @Codpro;";

            await connection.ExecuteAsync(sqlEliminarLiea, linea);
        }
        private async Task AcualizaLinea(PreventaLineas linea, SqliteConnection connection)
        {
            // Actualizar las líneas de la proforma
            string sqlUpdateLinea = @"

                             UPDATE lineasproforma
                             SET
                                 Codpro = @Codpro,
                                 Unidadmedida = @UnidadMedida,
                                 Detalle = @Detalle,
                                 Preciounidad = @PrecioUnidad,
                                 Cantidad = @Cantidad,
                                 Subtotal = @Subtotal,
                                 Descuento = @Descuento,
                                 Impuesto = @Impuesto,
                                 Totallinea = @TotalLinea,
                                 Codigo_impuesto = @CodigoImpuesto,
                                 Codigo_tarifa = @CodigoTarifa,
                                 Codecabys = @CodeCabys

                             WHERE n_proforma = @NProforma AND Codpro = @Codpro";


            await connection.ExecuteAsync(sqlUpdateLinea, linea);
        }
        private async Task AgregaLinea(PreventaLineas linea, SqliteConnection connection)
        {
            // Insertar las líneas de la proforma
            string sqlInsertLinea = @"INSERT INTO lineasproforma 
                  (Local_ID, n_proforma, linea, codpro, unidadmedida, detalle, preciounidad, cantidad, subtotal, descuento, impuesto, totallinea, montoexonerado, 
                    porimpuesto, subtotaldescuento, impuestoneto, porexonerado, codigo_impuesto, codigo_tarifa, codecabys) 
              VALUES 
                  (@LocalID, @NProforma, @Linea, @Codpro, @UnidadMedida, @Detalle, @PrecioUnidad, @Cantidad, @Subtotal, @Descuento, @Impuesto, @TotalLinea,
                  @Montoexonerado, @Porimpuesto, @Subtotaldescuento, @Impuestoneto, @Porexonerado, @CodigoImpuesto, @CodigoTarifa, @CodeCabys);";
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
                linea.Descuento,
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
