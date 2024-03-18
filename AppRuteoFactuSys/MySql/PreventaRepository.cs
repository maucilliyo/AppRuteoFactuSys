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
                string query = @"SELECT * FROM proforma where entregado = false and facturado = false;";
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
                               totalimpuesto, totalventa, codvendedor, codigomoneda, totalservgravados, totalservexentos, diasplazo, facturado, notas, serviciosexonerados, 
                               mercanciasexoneradas, totalmercanciasgravadas, totalmercanciasexentas, totalventaneta, totalexonerado, nombre_cliente, terminal, 
                               id_usuario, fecha_update, entregado) 
                            VALUES (@Cedcliente, @Fecha, @CondicionVenta, @Formapago, @TotalComprobante, @TotalDescuento, @TotalGrabado, @TotalExento, @TotalImpuesto, 
                               @TotalVentaNeta, @Id_Usuario, @CodigoMoneda, @TotalServGravados, @TotalServExentos, @DiasPlazo, @Facturado, @Notas, @ServiciosExonerados, 
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

    }
}
