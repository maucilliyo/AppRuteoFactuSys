using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Service
{
    public class DevolucionService
    {
        private readonly LocalDevolucionRepository _localDevolucionRepository;

        public DevolucionService(LocalDevolucionRepository localDevolucionRepository)
        {
            _localDevolucionRepository = localDevolucionRepository;
        }

        public async Task Sincronizar()
        { 
            //
        
        }
    }
}
