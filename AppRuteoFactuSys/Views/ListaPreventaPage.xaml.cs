using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views;

public partial class ListaPreventaPage : ContentPage
{
    private readonly IPreventaService _preventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;
    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }
    public ICommand SendSelectedDataCommand { get; private set; }
    private string _canton, _provincia, _distrito;
    public ListaPreventaPage(IPreventaService preventaService, IClienteService clienteService, IProductoService productoService,
                             string provincia, string canton, string distrito)
    {
        RefreshCommand = new Command(async () =>
        {
            await CargarPreventas();

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });

        BindingContext = this;
        InitializeComponent();
        _preventaService = preventaService;
        _clienteService = clienteService;
        _productoService = productoService;
        SendSelectedDataCommand = new Command<object>(ExecuteSendSelectedDataCommand);
        _canton = canton;
        _provincia = provincia;
        _distrito=distrito;
    }
    private async void ExecuteSendSelectedDataCommand(object parameter)
    {
        // Aquí puedes manejar la lógica para procesar los datos de la fila seleccionada.
        var rowData = (Preventa)parameter; // Asegúrate de cambiar YourDataType al tipo de objeto de tus datos
        PreventaPage preventaPage = new(_clienteService, _preventaService, _productoService, true)
        {
            idPreventa = rowData.LocalID
        };
        await Navigation.PushAsync(preventaPage);
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CargarPreventas();
    }
    private async Task CargarPreventas()
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
        var products = await _preventaService.Listar( _provincia,_canton,_distrito, false);
        dgPreventas.ItemsSource = products;
    }
    private async void btnNuevaProforma_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PreventaPage(_clienteService, _preventaService, _productoService));
    }
}