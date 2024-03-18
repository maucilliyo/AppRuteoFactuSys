using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;

namespace AppRuteoFactuSys.Views;

public partial class ListaPreventaPage : ContentPage
{
	private readonly IPreventaService _preventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;
    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }
    public ListaPreventaPage(IPreventaService preventaService, IClienteService clienteService, IProductoService productoService)
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
        //var products = await _clienteRepository.GetClientes();
        var products = await _preventaService.Listar();
        dgPreventas.ItemsSource = products;
    }

    private async void btnNuevaProforma_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new NavigationPage(new PreventaPage(_clienteService, _preventaService, _productoService)));
    }
}