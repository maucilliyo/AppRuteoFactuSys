using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppRuteoFactuSys.Service
{
    public class ViewFactoryServices
    {
        private readonly IServiceProvider _provider;
        private readonly IDialogService _dialogService;

        public ViewFactoryServices(IServiceProvider provider, IDialogService dialogService)
        {
            _provider = provider;
            _dialogService = dialogService;
        }

        public T Create<T>() where T : class
        {
            try
            {
                return _provider.GetRequiredService<T>();
            }
            catch (InvalidOperationException ex)
            {
                HandleServiceNotFound(typeof(T), ex);
                return null;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(typeof(T), ex);
                return null;
            }
        }

        public object Create(Type type)
        {
            try
            {
                return _provider.GetRequiredService(type);
            }
            catch (InvalidOperationException ex)
            {
                HandleServiceNotFound(type, ex);
                return null;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(type, ex);
                return null;
            }
        }

        private void HandleServiceNotFound(Type type, Exception ex)
        {
            _dialogService.ShowAlert(
                "Servicio no registrado",
                $"La vista '{type.Name}' no está registrada en los servicios.\n\nDetalle: {ex.Message}"
            );
        }

        private void HandleUnexpectedError(Type type, Exception ex)
        {
            _dialogService.ShowAlert(
                "Error inesperado",
                $"Ocurrió un error al crear '{type.Name}'.\n\nDetalle: {ex.Message}"
            );
        }
    }
}
