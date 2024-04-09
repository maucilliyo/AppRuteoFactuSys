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
    private async void ExecuteSendSelectedDataCommand(object parameter)
    {
        // Aquí puedes manejar la lógica para procesar los datos de la fila seleccionada.
        var rowData = (Preventa)parameter; // Asegúrate de cambiar YourDataType al tipo de objeto de tus datos
                                           // Hacer algo con rowData
        var preventa = await _preventaService.GetById(rowData.LocalID);

        await ImpresionService.ImprimirTicket(preventa);

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