using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views;

public partial class ProductoAdd2Page : ContentPage
{
    private readonly IProductoService _productoService;
    private readonly TaskCompletionSource<Producto> _tcs = new();
    private bool _cargado;

    public event EventHandler<Producto> SeleccionadoEvent;
    public Task<Producto> ProductoSeleccionadoTask => _tcs.Task;
    public ICommand SendSelectedDataCommand { get; private set; }
    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }

    public ProductoAdd2Page(IProductoService productoService)
    {
        InitializeComponent();

        _productoService = productoService;
        SendSelectedDataCommand = new Command(SendSelectedData);
        RefreshCommand = new Command(async () =>
        {
            await CargarProductos(txtBuscar.Text);

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });

        BindingContext = this;

        // Ya no se carga nada aquí: la página se muestra de inmediato
        // y los datos se cargan en OnAppearing.
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_cargado) return;
        _cargado = true;

        await CargarProductos();
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

    private async Task CargarProductos(string filtro = null)
    {
        try
        {
            IsBusy = true;

            // Validaciones y consulta fuera del hilo de UI
            var (error, products) = await Task.Run<(string Error, List<Producto> Lista)>(async () =>
            {
                //VALIDAR SI HAY CONEXION
                if (Conexion.GetConnection() == null)
                    return ("ERROR DE CONEXION CON EL SERVIDOR", null);

                if (!await Conexion.GetConfig())
                    return ("EL ARCHIVO DE CONEXION NO SE ENCUENTRA", null);

                //VALIDAR SI LA BD SE CARGO BIEN
                var lista = string.IsNullOrWhiteSpace(filtro)
                    ? await _productoService.Listar()
                    : await _productoService.Listar(filtro);

                return (null, lista?.ToList() ?? new List<Producto>());
            });

            if (error != null)
            {
                await DisplayAlert("Aviso", error, "Aceptar");
                _tcs.TrySetResult(null);
                await Navigation.PopModalAsync();
                return;
            }

            dgProductos.ItemsSource = products;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void btnBuscar_Clicked(object sender, EventArgs e)
    {
        await CargarProductos(txtBuscar.Text);
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