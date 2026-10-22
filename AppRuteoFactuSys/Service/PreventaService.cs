using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Service
{
    public class PreventaService : IPreventaService
    {

        private readonly LocalPreventaRepository _sqlLitePreventaRepository;
        private readonly PreventaRepository _preventaRepository;
        public PreventaService(LocalPreventaRepository sqlLitePreventaRepository, PreventaRepository preventaRepository)
        {
            _sqlLitePreventaRepository = sqlLitePreventaRepository;
            _preventaRepository = preventaRepository;
        }
        public async Task Actualizar(Preventa entity)
        {
            await _sqlLitePreventaRepository.ActualizarPreventaSync(entity);
        }
        public async Task ActualizarOnly(Preventa entity)
        {
            await _sqlLitePreventaRepository.ActualizarPreventaAppOnly(entity);
        }
        public Task<Preventa> GetById(int id)
        {
            return _sqlLitePreventaRepository.GetPreventaById(id);
        }
        public async Task<List<Preventa>> Listar(bool entregado)
        {
            return await _sqlLitePreventaRepository.GetPreventas(entregado);
        }
        public async Task Nuevo(Preventa entity)
        {
            await _sqlLitePreventaRepository.GuardarPreventa(entity);
        }
        public async Task Sincronizar()
        {
            await SincronizarDBs();
 
        }
        private async Task SincronizarDBs()
        {
            var preventasServer = await _preventaRepository.GetPreventasSinEntregar();

            var preventasApp = await _sqlLitePreventaRepository.GetPreventas();

            var serverPorNproforma = preventasServer.ToDictionary(p => p.Nproforma);

            var appPorNproforma = preventasApp.ToDictionary(p => p.Nproforma);

            foreach (var preventaServer in preventasServer)
            {
                if (!appPorNproforma.TryGetValue(preventaServer.Nproforma, out var preventaLocal))
                {
                    // Existe en servidor, pero no en SQLite.
                    var completa = await _preventaRepository.GetPreventaById(preventaServer.Nproforma);

                    await _sqlLitePreventaRepository.GuardarPreventa(completa);
                    continue;
                }

                if (preventaServer.FechaUpdate > preventaLocal.FechaUpdate)
                {
                    // El servidor tiene la versión más reciente.
                    var completa = await _preventaRepository.GetPreventaById(preventaServer.Nproforma);
                    completa.LocalID = preventaLocal.LocalID; // Mantener el LocalID de la versión local.

                    await _sqlLitePreventaRepository.ActualizarPreventaSync(completa);
                }
                else if (preventaLocal.FechaUpdate > preventaServer.FechaUpdate)
                {
                    // Android tiene la versión más reciente.
                    var completa = await _sqlLitePreventaRepository.GetPreventaById(preventaLocal.LocalID);
                    completa.LocalID = preventaLocal.LocalID; // Mantener el LocalID de la versión local.

                    await _preventaRepository.Actualizar(completa);
                }
            }

            foreach (var preventaLocal in preventasApp)
            {
                if (!serverPorNproforma.ContainsKey(preventaLocal.Nproforma))
                {
                    // Existe únicamente en SQLite.
                    var completa = await _sqlLitePreventaRepository.GetPreventaById(preventaLocal.LocalID);

                    await _preventaRepository.Guardar(completa);
                }
            }
        }
        public async Task<Preventa> GetPreventaByNProforma(int nProforma)
        {
            return await _sqlLitePreventaRepository.GetPreventaByNProforma(nProforma);
        }
        public async Task EliminarFacturadas()
        {
            await _sqlLitePreventaRepository.EliminarFacturadas();
        }
        public async Task<List<Preventa>> Listar(bool? entregado = null)
        {
            return await _sqlLitePreventaRepository.GetPreventas(entregado);
        }
        public async Task<List<Preventa>> Listar(string provincia, string canton, string distrito, bool? entregado = null)
        {
            return await _sqlLitePreventaRepository.GetPreventas(provincia, canton, distrito, entregado);
        }
        public async Task<List<Preventa>> Listar()
        {
            return await _sqlLitePreventaRepository.GetPreventas();
        }
        public async Task EliminarPreventas()
        {
            await _sqlLitePreventaRepository.EliminarPreventas();
        }
    }
}