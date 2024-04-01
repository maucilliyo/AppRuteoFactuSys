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
    public class ClienteRepository
    {
        public async Task<Cliente> GetClienteByCedula(string cedula)
        {
            Cliente cliente = new Cliente();
            using (var conn = await Conexion.GetConnection())
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@cedula", cedula);
                string query = @"SELECT json_merge(
                    json_object('cedula', c.cedula,'tipoCedula',c.tipocedula,'nombre',c.nombre,'tel',c.tel,'email',c.email,'provincia',
                        pr.provincia,'canton',ct.canton,'distrito',dt.distrito,'otrassenas',c.otrassenas,'contacto',
                        c.contacto,'credito',c.credito,'tipocliente',c.tipocliente,'pordescuento',c.pordescuento,'diascredito',
                        c.diascredito,'tipo_cliente_impuesto',c.tipo_cliente_impuesto,'tipodocpreferido',c.tipodocpreferido,'TipoPrecio',c.tipo_precio,
                        'FechaUpdate',c.fecha_update), 
                    json_object('creditoCliente',json_object('estado', cc.estado,'credito_maximo',credito_maximo,'saldodeuda',cc.saldodeuda,'usuarioautorizo',
			            cc.usuarioautorizo,'notas',cc.notas,'saldofavor',cc.saldofavor,'porinteresmora',cc.porinteresmora,'lockinsmora',cc.lockinsmora)),
                        case  when tipo_cliente_impuesto ='Exonerado' then
                    json_object('ExoneracionCliente',  json_object('tipoDocumento',e.tipoDocumento,'numerodocumento',e.numerodocumento,'nombreinstitucion',
                        e.nombreinstitucion,'fechaemision', e.fechaemision,'FechaVencimiento',e.FechaVencimiento,'porcentajeExoneracion',
                        e.PorcentajeExoneracion,'mag',e.mag,'PoseeCabys',e.PoseeCabys,
                        'ListCabys',json_merge_preserve('[]',concat('[',group_concat(json_object('Cabys', ce.cabys ,'Num_Exoneracion',ce.Num_Exoneracion)SEPARATOR ','),']'))) ) else 
                        json_object('ExoneracionCliente',null) end
                    ) as json FROM clientes c
                 left join creditoclientes cc on c.cedula= cc.cedcliente
                 left join exoneraciones e on  c.cedula=e.cedula
                 left join cabysexoneradoscliente ce on e.id = ce.id_Exoneracion
                 inner join provincia pr on c.codigoprovincia = pr.codigo
                 inner join canton ct on c.codigocanton = ct.id
                 inner join distrito dt on c.codigodistrito = dt.codigo and c.codigocanton =dt.codigocanton
                 where c.cedula=@cedula;";
                var response = await conn.ExecuteReaderAsync(query, p);

                while (response.Read())
                {
                    cliente = JsonConvert.DeserializeObject<Cliente>(response.GetValue(0).ToString(),
                                                                     new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                if (cliente == null)//VALIDA QUE EL JSON VENGA NULL PARA ENVIAR UN OJETO VACIDO
                {
                    return cliente = new Cliente();
                }
                return cliente;
            }
        }
        public async Task<List<Cliente>> GetClientes()
        {
            using (var conn = await Conexion.GetConnection())
            {
                string query = @"SELECT * FROM clientes";
                var response = await conn.QueryAsync<Cliente>(query);

                return response.ToList();
            }
        }
 
    } 
}
