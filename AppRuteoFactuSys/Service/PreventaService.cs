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
            await SincronizarDBApp();
            await SincronizarDBSistema();
        }
        private async Task SincronizarDBApp()
        {
            var preventasServer = await _preventaRepository.GetPreventasSinEntregar();
            //lista de preventas en la app
            //var preventasApp = await Listar();
            //recorer las prventas en el sistema para evaluar cambios u otras
            foreach (var preventaServer in preventasServer)
            {
                //traer la proforma del sistema
                var proforma = await _preventaRepository.GetPreventaById(preventaServer.Nproforma);
                //verificar si no existe la preventa en al app
                var preventasApp =  await _sqlLitePreventaRepository.GetPreventaByNProforma(preventaServer.Nproforma);
                //si no existe la agregamos a la app
                if (preventasApp==null)
                {
                    try
                    {
                        await _sqlLitePreventaRepository.GuardarPreventa(proforma);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                }
                // si existe se evalua si algo cambio
                else
                {
                    var proformaApp = await GetPreventaByNProforma(proforma.Nproforma);

                    TimeSpan diferencia = proforma.FechaUpdate - proformaApp.FechaUpdate;

                    if (diferencia.TotalSeconds > 1)
                        try
                        {
                            proforma.LocalID = proformaApp.LocalID;
                            await _sqlLitePreventaRepository.ActualizarPreventaSync(proforma);
                        }
                        catch (Exception ex)
                        {

                            throw new Exception(ex.Message);
                        }
                }
            }
        }
        private async Task SincronizarDBSistema()
        {
            //listra de preventas en la app
            var preventasApp = await Listar();
            //recorrer las lista del la app
            foreach (var preventa in preventasApp)
            {
                //traer la preventa de la app
                var preventaApp = await GetById(preventa.LocalID);
                //verificar si no existe la preventa en el servidor
                var preventaServer = await _preventaRepository.GetPreventaById(preventa.Nproforma);
                //validar si no existe y si la proforma es 0 es porque fue creada del lado de la app entonces se guarda en el sistema
                if (preventaServer == null)
                {
                    try
                    {
                        var id = await _preventaRepository.Guardar(preventaApp);
                        await _sqlLitePreventaRepository.ActualizarNumeroProforma(id, preventa.LocalID);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                }
                // si existe se evalua si algo cambio
                else
                {
                    TimeSpan diferencia = preventa.FechaUpdate - preventaServer.FechaUpdate;

                    if (diferencia.TotalSeconds > 2)
                        try
                        {

                            await _preventaRepository.Actuazar(preventaApp);
                        }
                        catch (Exception ex)
                        {

                            throw new Exception(ex.Message);
                        }
                }
            }
        }
        public async Task<Preventa> GetPreventaByNProforma(int nProforma)
        {
            return await _sqlLitePreventaRepository.GetPreventaByNProforma(nProforma);
        }
        public async Task<List<Preventa>> Listar(bool? entregado = null)
        {
            return await _sqlLitePreventaRepository.GetPreventas(entregado);
        }
        public async Task EliminarFacturadas()
        {
           await _sqlLitePreventaRepository.EliminarFacturadas();
        }
        public async Task<List<Preventa>> Listar(string provincia, string canton, string distrito, bool? entregado = null)
        {
           return await _sqlLitePreventaRepository.GetPreventas(provincia,canton, distrito, entregado);
        }
        public async Task<List<Preventa>> Listar()
        {
            return await _sqlLitePreventaRepository.GetPreventas();
        }
    }
}