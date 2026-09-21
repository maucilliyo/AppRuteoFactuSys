using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Service
{
    public class DevolucionService
    {
        private readonly LocalDevolucionRepository _localDevolucionRepository;
        private readonly DevolucionRepository _devolucionRepository;

        public DevolucionService(LocalDevolucionRepository localDevolucionRepository, DevolucionRepository devolucionRepository)
        {
            _localDevolucionRepository = localDevolucionRepository;
            _devolucionRepository = devolucionRepository;
        }

        public async Task Sincronizar()
        {
            //
            var pendientes = await _localDevolucionRepository.GetLista();

            foreach (var pendiente in pendientes)
            {
                var devolucion = await _localDevolucionRepository.GetById(pendiente.IdDevolucion);


                try
                {
                    await _devolucionRepository.InsertarAsync(devolucion);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

        }
    }
}
