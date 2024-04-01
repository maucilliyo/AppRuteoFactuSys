using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views;

public partial class ProductoAddPage : ContentPage
{
	private  readonly IProductoService _productoService;
    public event EventHandler<Producto>  SeleccionadoEvent;
    public ICommand SendSelectedDataCommand { get; private set; }
    public ProductoAddPage(IProductoService productoService )
	{
		InitializeComponent();
		_productoService = productoService;
        SendSelectedDataCommand = new Command(SendSelectedData);
        BindingContext = this;

        CargarProductos();
    }
    private void SendSelectedData(object parameter)
    {
        Producto Seleccionado = (Producto)parameter;
        // Disparar el evento y pasar el cliente seleccionado como argumento
        SeleccionadoEvent?.Invoke(this, Seleccionado);
        // Volver a la página anterior (PreventaPage)
        Navigation.PopAsync();
    }
    private async Task CargarProductos()
    {
        //VALIDAR SI HAY CONEXION
        if (Conexion.GetConnection() == null)
        {
            await DisplayAlert("Aviso", "ERROR DE CONEXION CON EL SERVIDOR", "Aceptar");
            await Navigation.PopAsync();
            return;
        }

        if (!await Conexion.GetConfig())
        {
            await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
            await Navigation.PopAsync();
            return;
        }

        //VALIDAR SI LA BD SE CARGO BIEN
        var products = await _productoService.Listar();
        dgProductos.ItemsSource = products;
    }

    private async void btnBuscar_Clicked(object sender, EventArgs e)
    {
        var products = await _productoService.Listar(txtBuscar.Text);
        dgProductos.ItemsSource = products;
    }
}