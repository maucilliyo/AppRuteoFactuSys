using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;
using AppRuteoFactuSys.Views;
using Inventario.Views;

namespace AppRuteoFactuSys
{
    public partial class MainPage : ContentPage
    {
        private readonly IClienteService _clienteService;
        private readonly IPreventaService _preventaService;
        private readonly IProductoService _productoService;
        public MainPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService)
        {
            InitializeComponent();
            _clienteService = clienteService;
            _preventaService = preventaService;
            _productoService = productoService;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // Código que deseas ejecutar cuando la página aparece
            await Conexion.GetConfig();

        }
        private void OnCounterClicked(object sender, EventArgs e)
        {

        }

        private async void btnConexion_Clicked(object sender, EventArgs e)
        {
            try
            {
                string result = await DisplayPromptAsync("Aviso", "Digite la clave para Ingresar al menu de configuracion", "Aceptar", "Cancelar", "Clave maestra",
                                            keyboard: Keyboard.Numeric); //keyboard: Keyboard.Numeric es para que solo acepta numeros
                if (result != null)
                {
                    if (result == "246")
                    {
                        await Navigation.PushAsync(new PopConfServidor());
                    }
                    else
                    {
                        // Aquí puedes manejar el valor ingresado por el usuario
                        await DisplayAlert("Aviso", $"Error en la clave", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Aviso", ex.Message, "Aceptar");
                Console.WriteLine($"Error al navegar a PopConfServidor: {ex.Message}");
            }
            // await Navigation.PushAsync(new PopConfServidor());
        }

        private async void btnListaPro_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
            await Navigation.PushAsync(new ListaProductosPage(_clienteService));
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
        }

        private async void btnAjusta_Clicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new AjustePage());
        }

        private async void btnSincronizar_Clicked(object sender, EventArgs e)
        {
            // Instanciar SincronizarPage
            var sincronizarPage = new SincronizarPage(_clienteService, _preventaService, _productoService);

            // Mostrar SincronizarPage como un diálogo modal
            await Navigation.PushModalAsync(new NavigationPage(sincronizarPage));
        }

        private async void btnPruebas_Clicked(object sender, EventArgs e)
        {
            //SQLiteInitialization.DeleteDataBase();
            var preventas = await _preventaService.Listar();

            var preventa = await _preventaService.GetById(2);
 
            ImpresionService.ImprimirTicket(preventa);
        }

        private async void btnAjusta_Clicked_1(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;

            //await Navigation.PushAsync(new ListaPreventaPage(_preventaService, _clienteService, _productoService));

            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

        }

        private async void btnEliminarDB_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Aviso", "Digite la clave para RESETEAR la base de datos", "Aceptar", "Cancelar", "Clave maestra",
                                                        keyboard: Keyboard.Numeric); //keyboard: Keyboard.Numeric es para que solo acepta numeros
            if (result != null)
            {
                if (result == "246")
                {
                    SQLiteInitialization.DeleteDataBase();
                    SQLiteInitialization.InitializeDatabase();
                    await DisplayAlert("Aviso", $"Base de datos RESETEADA", "Aceptar");
                }
                else
                {
                    // Aquí puedes manejar el valor ingresado por el usuario
                    await DisplayAlert("Aviso", $"Error en la clave", "Aceptar");
                }
            }
        }

        private async void btnFacturadas_Clicked(object sender, EventArgs e)
        {
            ListaFacturadas listaFacturadas = new(_preventaService, _clienteService, _productoService);

            await Navigation.PushAsync(new NavigationPage(listaFacturadas));
        }

        private async void btnPreventa_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new MenuPreventaPage( _preventaService, _clienteService, _productoService)));
        }
    }

}
