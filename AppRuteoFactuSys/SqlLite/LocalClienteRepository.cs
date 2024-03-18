using AppRuteoFactuSys.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalClienteRepository
    {
        public async Task<Cliente> GetClienteByCedula(string cedula)
        {
            SQLiteInitialization.InitializeDatabase();

            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT * FROM Clientes WHERE cedula = @cedula";

                await conn.OpenAsync();
                var response = await conn.QueryAsync<Cliente>(sql, new { cedula });

                return response.FirstOrDefault();
            }
        }
        public async Task<List<Cliente>> GetClientes()
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                string sql = @"SELECT * FROM clientes";
                await conn.OpenAsync();
                var response = await conn.QueryAsync<Cliente>(sql);
                return response.ToList();
            }
        }
        public async Task Agregar(Cliente cliente)
        {
            using (var conn = SqlLiteConexion.GetConnection())
            {
                // Consulta SQL para la inserción
                string sqlInsert = @"
                INSERT INTO clientes (
                    cedula, tipocedula, nombre, apellido, tel, email, codigoprovincia,
                    codigocanton, codigodistrito, otrassenas, contacto, credito, tipocliente,
                    pordescuento, diascredito, tipo_cliente_impuesto, tipodocpreferido, tipo_precio,
                    fecha_update
                )
                VALUES (
                    @Cedula, @TipoCedula, @Nombre, @Apellido, @Tel, @Email, @CodigoProvincia,
                    @CodigoCanton, @CodigoDistrito, @OtrasSenas, @Contacto, @Credito, @TipoCliente,
                    @PorDescuento, @DiasCredito, @TipoClienteImpuesto, @TipoDocPreferido, @TipoPrecio,
                    @FechaUpdate
                )";
                await conn.OpenAsync();

                await conn.ExecuteAsync(sqlInsert, cliente);
            }
        }
        public async Task Actualizar(Cliente cliente)
        {
            // Consulta SQL para la actualización
            string sqlUpdate = @"
            UPDATE clientes 
            SET 
                tipocedula = @TipoCedula,
                nombre = @Nombre,
                apellido = @Apellido,
                tel = @Tel,
                email = @Email,
                codigoprovincia = @CodigoProvincia,
                codigocanton = @CodigoCanton,
                codigodistrito = @CodigoDistrito,
                otrassenas = @OtrasSenas,
                contacto = @Contacto,
                credito = @Credito,
                tipocliente = @TipoCliente,
                pordescuento = @PorDescuento,
                diascredito = @DiasCredito,
                tipo_cliente_impuesto = @TipoClienteImpuesto,
                tipodocpreferido = @TipoDocPreferido,
                tipo_precio = @TipoPrecio,
                fecha_update = @FechaUpdate
            WHERE cedula = @Cedula";

            // Crear una conexión a la base de datos SQLite
            using (var connection = SqlLiteConexion.GetConnection())
            {
                // Ejecutar la consulta SQL utilizando Dapper
                await connection.ExecuteAsync(sqlUpdate, cliente);
            }
        }

        public async Task<IEnumerable<string>> FiltroByCedula()
        {
            return null;
        }
    }
}
