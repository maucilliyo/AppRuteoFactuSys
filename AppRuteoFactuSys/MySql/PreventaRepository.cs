using AppRuteoFactuSys.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.MySql
{
    public class PreventaRepository
    {
        public async Task<List<Preventa>> GetPreventasSinEntregar()
        {
            using (var conn = await Conexion.GetConnection())
            {
                string query = @"SELECT * FROM proforma where entregado = false and estado = 'Pendiente';";
                var response = await conn.QueryAsync<Preventa>(query);

                return response.ToList();
            }
        }
        public async Task<Preventa> GetPreventaById(int id)
        {
            using (var conn = await Conexion.GetConnection())
            {
                //traer la proforma
                string queryPrebenta = @"SELECT * FROM proforma where nproforma = @id;";
                var response = await conn.QueryAsync<Preventa>(queryPrebenta, new { id });
                var prefactura = response.FirstOrDefault();
                //si viene nula paramos el codigo y retornamos null
                if (prefactura == null) return null;
                //traer las lineas
                string queryLineas = @"SELECT * FROM lineasproforma 
                                       where n_proforma = @NProforma order by linea asc;";
                var responseLineas = await conn.QueryAsync<PreventaLineas>(queryLineas, new { prefactura.Nproforma });

                prefactura.Lineas = responseLineas.ToList();

                return prefactura;
            }
        }
        public async Task<int> Guardar(Preventa preventa)
        {
            using (var conn = await Conexion.GetConnection())
            {
                using (var Transaction = conn.BeginTransaction())
                {
                    //sql para insert de proforma
                    string sql = @"INSERT INTO proforma (cedcliente, fecha, condicionventa, formapago, totalcomprobante, totaldescuento, totalgrabado, totalexento, 
                               totalimpuesto, totalventa, codvendedor, codigomoneda, totalservgravados, totalservexentos, diasplazo, estado, notas, serviciosexonerados, 
                               mercanciasexoneradas, totalmercanciasgravadas, totalmercanciasexentas, totalventaneta, totalexonerado, nombre_cliente, terminal, 
                               id_usuario, fecha_update, entregado) 
                            VALUES (@Cedcliente, @Fecha, @CondicionVenta, @Formapago, @TotalComprobante, @TotalDescuento, @TotalGrabado, @TotalExento, @TotalImpuesto, 
                               @TotalVentaNeta, @Id_Usuario, @CodigoMoneda, @TotalServGravados, @TotalServExentos, @DiasPlazo, @estado, @Notas, @ServiciosExonerados, 
                               @mercanciasexoneradas, @totalmercanciasgravadas, @totalmercanciasexentas, @totalventaneta, @totalexonerado, @nombre_cliente, @terminal,
                                @id_usuario, @FechaUpdate, @Entregado);
                            SELECT LAST_INSERT_ID();";
                    //sql para insert de las lineas
                    string insertLineas = @"INSERT INTO lineasproforma
                            (n_proforma,linea,codpro,unidadmedida,detalle,preciounidad,cantidad,subtotal,descuento,impuesto,totallinea,montoexonerado,porimpuesto,
                            subtotaldescuento, impuestoneto,porexonerado,codigo_impuesto,codigo_tarifa,codecabys)
                            VALUES(@NProforma,@linea,@codpro,@unidadmedida,@detalle,@preciounidad,@cantidad,@subtotal,@descuento,@impuesto,@totallinea,@montoexonerado,
                            @porimpuesto, @subtotaldescuento,@impuestoneto,@porexonerado,@CodigoImpuesto,@CodigoTarifa,@codecabys);";
                    try
                    {

                        var nProforma = await conn.ExecuteScalarAsync(sql, preventa, transaction: Transaction);

                        int nLinea = 1;
                        foreach (var linea in preventa.Lineas)
                        {
                            linea.NProforma = Convert.ToInt32(nProforma);
                            linea.Linea = nLinea;
                            await conn.ExecuteAsync(insertLineas,linea, transaction: Transaction);
                            nLinea++;
                        }

                        //si todo salio bien se hace el commit
                        Transaction.Commit();
                        //retornamos el ID
                        return Convert.ToInt32(nProforma);

                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        throw new Exception("Error al guardar la proforma " + ex.Message);
                    }
                }
            }
        }
        public async Task Actuazar(Preventa preventa)
        {
            using (var conn = await Conexion.GetConnection())
            {
                using (var Transaction = conn.BeginTransaction())
                {
                    // SQL para actualizar la proforma
                    string updateProforma = @"UPDATE proforma 
                                  SET cedcliente = @Cedcliente, 
                                      fecha = @Fecha, 
                                      condicionventa = @CondicionVenta, 
                                      formapago = @Formapago, 
                                      totalcomprobante = @TotalComprobante, 
                                      totaldescuento = @TotalDescuento, 
                                      totalgrabado = @TotalGrabado, 
                                      totalexento = @TotalExento, 
                                      totalimpuesto = @TotalImpuesto, 
                                      totalventa = @TotalVentaNeta, 
                                      codvendedor = @Id_Usuario, 
                                      codigomoneda = @CodigoMoneda, 
                                      totalservgravados = @TotalServGravados, 
                                      totalservexentos = @TotalServExentos, 
                                      diasplazo = @DiasPlazo, 
                                      estado = @Estado, 
                                      notas = @Notas, 
                                      serviciosexonerados = @ServiciosExonerados, 
                                      mercanciasexoneradas = @mercanciasexoneradas, 
                                      totalmercanciasgravadas = @totalmercanciasgravadas, 
                                      totalmercanciasexentas = @totalmercanciasexentas, 
                                      totalventaneta = @totalventaneta, 
                                      totalexonerado = @totalexonerado, 
                                      nombre_cliente = @nombre_cliente, 
                                      terminal = @terminal, 
                                      id_usuario = @id_usuario, 
                                      fecha_update = @FechaUpdate, 
                                      entregado = @Entregado
                                  WHERE nproforma = @Nproforma and estado = 'Pendiente';";

                    // SQL para eliminar las líneas existentes de la proforma
                    string deleteLineas = @"DELETE FROM lineasproforma WHERE n_proforma = @Nproforma;";

                    // SQL para insertar las nuevas líneas de la proforma
                    string insertLineas = @"INSERT INTO lineasproforma
                                (n_proforma,linea,codpro,unidadmedida,detalle,preciounidad,cantidad,subtotal,descuento,impuesto,totallinea,montoexonerado,porimpuesto,
                                subtotaldescuento, impuestoneto,porexonerado,codigo_impuesto,codigo_tarifa,codecabys)
                                VALUES(@Nproforma,@linea,@codpro,@unidadmedida,@detalle,@preciounidad,@cantidad,@subtotal,@descuento,@impuesto,@totallinea,@montoexonerado,
                                @porimpuesto, @subtotaldescuento,@impuestoneto,@porexonerado,@CodigoImpuesto,@CodigoTarifa,@codecabys);";

                    try
                    {
                        // Actualizar la proforma
                        var result = await conn.ExecuteAsync(updateProforma, preventa, transaction: Transaction);
                        //validamos si hay filas afectadas
                        if(result > 0)
                        {
                            // Eliminar las líneas existentes de la proforma
                            await conn.ExecuteAsync(deleteLineas, new { preventa.Nproforma }, transaction: Transaction);

                            // Insertar las nuevas líneas de la proforma
                            int nLinea = 1;
                            foreach (var linea in preventa.Lineas)
                            {
                                linea.NProforma = preventa.Nproforma; // Usar el ID de la proforma existente
                                linea.Linea = nLinea;
                                await conn.ExecuteAsync(insertLineas, linea, transaction: Transaction);
                                nLinea++;
                            }
                        }
                        // Hacer el commit si todo salió bien
                        Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Transaction.Rollback();
                        throw new Exception("Error al modificar la proforma " + ex.Message);
                    }
                }
            }

        }

    }
}
