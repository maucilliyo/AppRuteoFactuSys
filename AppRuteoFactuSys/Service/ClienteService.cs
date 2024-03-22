using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Service
{
    public class ClienteService : IClienteService
    {
        private readonly ClienteRepository  _clienteRepository;
        private readonly LocalClienteRepository _sqlLiteClientesRepository;
        public ClienteService(ClienteRepository clienteRepository, LocalClienteRepository sqlLiteClientesRepository)
        {
             _clienteRepository =  clienteRepository;
            _sqlLiteClientesRepository = sqlLiteClientesRepository;
        }

        public Task Actualizar(Cliente entity)
        {
            throw new NotImplementedException();
        }

        public Task<Cliente> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<Cliente> GetByCedula(string cedula)
        {
            var clienteApp = await _sqlLiteClientesRepository.GetClienteByCedula(cedula);
            return clienteApp;
        }
        public async Task<List<Cliente>> Listar()
        {
            return await _sqlLiteClientesRepository.GetClientes();
        }

        public Task Nuevo(Cliente entity)
        {
            throw new NotImplementedException();
        }

        public async Task Sincronizar()
        {
            var clientesSistema = await _clienteRepository.GetClientes();
            var clientesApp = await _sqlLiteClientesRepository.GetClientes();

            foreach (var cliente in clientesSistema)
            {
                var clienteSistema = await _clienteRepository.GetClienteByCedula(cliente.Cedula);
                var clienteApp = await _sqlLiteClientesRepository.GetClienteByCedula(cliente.Cedula);

                var noExiste = !clientesApp.Any(c => c.Cedula == cliente.Cedula);

                if (noExiste)
                {
                    try
                    {
                        await _sqlLiteClientesRepository.Agregar(clienteSistema);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                }
                else if (clienteApp.FechaUpdate < clienteSistema.FechaUpdate)
                {
                    try
                    {
                        await _sqlLiteClientesRepository.Actualizar(clienteSistema);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                }

            }

        }
    }
}
