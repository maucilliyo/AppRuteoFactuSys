using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Views
{
    public partial class SincronizarPage : ContentPage
    {
        private readonly IClienteService _clienteService;
        private readonly IPreventaService _preventaService;
        private readonly IProductoService _productoService;
        private readonly LocalDevolucionRepository _devolucionRepository;
        private readonly DevolucionService _devolucionService;

        public SincronizarPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService,
                               LocalDevolucionRepository devolucionRepository, DevolucionService devolucionService)
        {
            InitializeComponent();
            _clienteService = clienteService;
            _preventaService = preventaService;
            _productoService = productoService;
            _devolucionRepository = devolucionRepository;
            _devolucionService = devolucionService;
        }

        private async Task Sincronizar(string service)
        {
            try
            {
                this.IsEnabled = false;
                // Mostrar el ActivityIndicator y el texto
                MostrarIndicadorEspera(true);

                // Deshabilitar los botones de sincronización
                DeshabilitarBotonesSincronizacion();

                // Realizar sincronización
                if (service == "Clientes")
                    await _clienteService.Sincronizar();
                else if (service == "Preventa")
                    await _preventaService.Sincronizar();
                else if (service == "Productos")
                    await _productoService.Sincronizar();
                else
                {
                    await _clienteService.Sincronizar();
                    await _preventaService.Sincronizar();
                    await _productoService.Sincronizar();
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción mostrando un mensaje de error
                await DisplayAlert("Error", $"Error durante la sincronización: {ex.Message}", "Aceptar");
            }
            finally
            {
                // Ocultar el ActivityIndicator y habilitar los botones de sincronización
                MostrarIndicadorEspera(false);
                HabilitarBotonesSincronizacion();
                this.IsEnabled = true;
            }
        }

        private void MostrarIndicadorEspera(bool mostrar)
        {
            activityIndicator.IsRunning = mostrar;
            activityIndicator.IsVisible = mostrar;
            lblActivityIndicatorText.IsVisible = mostrar;
        }

        private void DeshabilitarBotonesSincronizacion()
        {
            btnSincronizarClientes1.IsEnabled = false;
            btnSincronizarPreventa.IsEnabled = false;
            btnSincronizarProducto.IsEnabled = false;
            btnSincronizarTodos.IsEnabled = false;
        }

        private void HabilitarBotonesSincronizacion()
        {
            btnSincronizarClientes1.IsEnabled = true;
            btnSincronizarPreventa.IsEnabled = true;
            btnSincronizarProducto.IsEnabled = true;
            btnSincronizarTodos.IsEnabled = true;
        }

        private async void btnSincronizarClientes_Clicked(object sender, EventArgs e)
        {
            await Sincronizar("Clientes");
        }

        private async void btnSincronizarPreventa_Clicked(object sender, EventArgs e)
        {
            await Sincronizar("Preventa");
        }

        private async void btnSincronizarProducto_Clicked(object sender, EventArgs e)
        {
            await Sincronizar("Productos");
        }

        private async void btnSincronizarTodos_Clicked(object sender, EventArgs e)
        {
            await Sincronizar("Todo");
        }

        private void btnSincronizarSistema_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnEliminarFacturadas_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("AVISO", "Esta seguro de eliminar las preventas facturadas?", "Sí", "No");

            if (response)
            {
                await _preventaService.EliminarFacturadas();
            }
        }

        private async void btnEliminarPreventas_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("AVISO", "Esta seguro de eliminar las preventas?", "Sí", "No");

            if (response)
            {
                await _preventaService.EliminarPreventas();
            }
        }

        private async void btnEliminarDevoluciones_Clicked(object sender, EventArgs e)
        {
            await _devolucionRepository.EliminarTodas();
        }

        private async void btnSincronizarDevoluciones_Clicked(object sender, EventArgs e)
        {
            try
            {
                MostrarIndicadorEspera(true);
                this.IsEnabled = false;
                await _devolucionService.Sincronizar();
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                this.IsEnabled = true;
                MostrarIndicadorEspera(false);
            }
        }
    }
}
