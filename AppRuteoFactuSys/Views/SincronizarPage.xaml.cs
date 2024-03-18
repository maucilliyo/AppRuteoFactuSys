using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;

namespace AppRuteoFactuSys.Views
{
    public partial class SincronizarPage : ContentPage
    {
        private readonly IClienteService _clienteService;
        private readonly IPreventaService _preventaService;
        private readonly IProductoService _productoService;

        public SincronizarPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService)
        {
            InitializeComponent();
            _clienteService = clienteService;
            _preventaService = preventaService;
            _productoService = productoService;
        }

        private async Task Sincronizar(string service)
        {
            try
            {
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
    }
}
