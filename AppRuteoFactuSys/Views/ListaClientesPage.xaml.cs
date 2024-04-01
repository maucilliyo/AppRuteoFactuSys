using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;

namespace AppRuteoFactuSys.Views;

public partial class ListaProductosPage : ContentPage
{
    private readonly IClienteService _clienteService;
    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }
    public ListaProductosPage(IClienteService clienteServeices)
    {
        RefreshCommand = new Command(async () =>
        {
            await CargarClientes();

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });

        BindingContext = this;

        InitializeComponent();

        _clienteService = clienteServeices;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CargarClientes();
    }
    private async Task CargarClientes()
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
        var products = await _clienteService.Listar();
        dgProductos.ItemsSource = products;
    }

    private async void btnBustar_Clicked(object sender, EventArgs e)
    {
        var products = await _clienteService.Listar(txtBuscar.Text);
        dgProductos.ItemsSource = products;
    }
}