using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views;

public partial class ProductoAdd2Page : ContentPage
{
    private readonly IProductoService _productoService;
    private readonly TaskCompletionSource<Producto> _tcs = new();

    public event EventHandler<Producto> SeleccionadoEvent;
    public Task<Producto> ProductoSeleccionadoTask => _tcs.Task;
    public ICommand SendSelectedDataCommand { get; private set; }
    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }

    public ProductoAdd2Page(IProductoService productoService)
    {
        RefreshCommand = new Command(async () =>
        {
            await CargarProductos();

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });
        InitializeComponent();
        _productoService = productoService;
        SendSelectedDataCommand = new Command(SendSelectedData);
        BindingContext = this;
        _ = CargarProductos();
    }

    private async void SendSelectedData(object parameter)
    {
        Producto Seleccionado = (Producto)parameter;
        // Disparar el evento y pasar el producto seleccionado como argumento
        SeleccionadoEvent?.Invoke(this, Seleccionado);
        // Resolver la task para quien está esperando con await
        _tcs.TrySetResult(Seleccionado);
        // Volver a la página anterior (se abrió con PushModalAsync)
        await Navigation.PopModalAsync();
    }

    private async Task CargarProductos()
    {
        //VALIDAR SI HAY CONEXION
        if (Conexion.GetConnection() == null)
        {
            await DisplayAlert("Aviso", "ERROR DE CONEXION CON EL SERVIDOR", "Aceptar");
            _tcs.TrySetResult(null);
            await Navigation.PopModalAsync();
            return;
        }

        if (!await Conexion.GetConfig())
        {
            await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
            _tcs.TrySetResult(null);
            await Navigation.PopModalAsync();
            return;
        }

        //VALIDAR SI LA BD SE CARGO BIEN
        var products = await _productoService.Listar();
        dgProductos.ItemsSource = products ?? new List<Producto>();
    }

    private async void btnBuscar_Clicked(object sender, EventArgs e)
    {
        var products = await _productoService.Listar(txtBuscar.Text);
        dgProductos.ItemsSource = products;
    }

    private void txtBuscar_Completed(object sender, EventArgs e)
    {
        btnBuscar_Clicked(sender, e);
    }

    protected override bool OnBackButtonPressed()
    {
        // Si el usuario cierra sin seleccionar nada (botón atrás en Android)
        _tcs.TrySetResult(null);
        return base.OnBackButtonPressed();
    }
}