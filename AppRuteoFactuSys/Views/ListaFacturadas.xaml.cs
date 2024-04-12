using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using Controls.UserDialogs.Maui;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views;

public partial class ListaFacturadas : ContentPage
{
    private readonly IPreventaService _preventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;

    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }
    public ICommand SendSelectedDataCommand { get; private set; }
    public ListaFacturadas(IPreventaService preventaService, IClienteService clienteService, IProductoService productoService)
    {
        InitializeComponent();
        _preventaService = preventaService;
        _clienteService = clienteService;
        _productoService = productoService;

        RefreshCommand = new Command(async () =>
        {
            await CargarFacturadas();

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });
        SendSelectedDataCommand = new Command<object>(ExecuteSendSelectedDataCommand);
        BindingContext = this;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CargarFacturadas();
    }
    private bool _isLoading = false; // Variable para controlar si la aplicación está cargando o no

    private async void ExecuteSendSelectedDataCommand(object parameter)
    {
        // Verifica si ya se está cargando para evitar iniciar múltiples instancias de la misma tarea
        if (!_isLoading)
        {
            _isLoading = true; // Marca la aplicación como cargando
          
            // Muestra un indicador visual de carga, por ejemplo, mostrando una barra de progreso
            // o un indicador de carga giratorio en la interfaz de usuario

            try
            {
                var rowData = (Preventa)parameter;
                var preventa = await _preventaService.GetById(rowData.LocalID);
                var impresionExitosa = await ImpresionService.ImprimirTicket(preventa);

            }
            finally
            {
                _isLoading = false; // Marca la aplicación como ya no cargando
                //UserDialogs.Instance.HideHud();
                // Oculta el indicador visual de carga una vez que la tarea ha terminado
                // Por ejemplo, ocultando la barra de progreso o el indicador de carga giratorio
            }
        }
    }
    private async Task CargarFacturadas()
    {
        //VALIDAR SI HAY CONEXION
        if (Conexion.GetConnection() == null)
        {
            await DisplayAlert("Aviso", "ERROR DE CONEXION CON EL SERVIDOR", "Aceptar");
            await Navigation.PopAsync();
        }
        if (!await Conexion.GetConfig())
        {
            await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
            await Navigation.PopAsync();
        }
        //VALIDAR SI LA BD SE CARGO BIEN
        var products = await _preventaService.Listar(true);
        dgPreventas.ItemsSource = products;
    }

}