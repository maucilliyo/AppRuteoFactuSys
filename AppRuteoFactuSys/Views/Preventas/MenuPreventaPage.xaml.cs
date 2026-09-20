using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;
using Controls.UserDialogs.Maui;

namespace AppRuteoFactuSys.Views;

public partial class MenuPreventaPage : ContentPage
{
    private readonly LocalLocalizaciones localLocalizaciones = new();
    private readonly IPreventaService _preventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;
    private readonly IUserDialogs _userDialogs;

    private bool isInitialPageLoad = true;

    public MenuPreventaPage(IPreventaService preventaService, IClienteService clienteService, IProductoService productoServic, IUserDialogs userDialogs)
    {
        InitializeComponent();
        _preventaService = preventaService;
        _clienteService = clienteService;
        _productoService = productoServic;
         _userDialogs= userDialogs;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (isInitialPageLoad)
        {
            cbProvincias.ItemsSource = await localLocalizaciones.GetProvincias();
            cbProvincias.SelectedIndex = 0;
            cbCantones.SelectedIndex = 1;
            cbDistritos.SelectedIndex = 0;
            isInitialPageLoad = false;
        }
        btnVerClientes.Focus( );
    }

    private async void cbProvincias_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbCantones.ItemsSource = await localLocalizaciones.GetCantones(cbProvincias.SelectedItem.ToString());
    }

    private async void cbCantones_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbCantones.SelectedItem == null)
        {
            cbDistritos.ItemsSource = null;
            return;
        }
        cbDistritos.ItemsSource = await localLocalizaciones.GetDistritos(cbCantones.SelectedItem.ToString());
    }

    private void cbDistritos_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private async void btnVerClientes_Clicked(object sender, EventArgs e)
    {
        //validaciones
        if (cbProvincias.SelectedItem == null)
        {
            await DisplayAlert("Aviso", "Debe selecionar una provincia", "Aceptar");
            return;
        }
        if (cbCantones.SelectedItem == null)
        {
            await DisplayAlert("Aviso", "Debe selecionar un canton", "Aceptar");
            return;
        }
        if (cbDistritos.SelectedItem == null)
        {
            await DisplayAlert("Aviso", "Debe selecionar un distrito", "Aceptar");
            return;
        }
        //llamar a la lista
        _userDialogs.ShowLoading();

        await Navigation.PushAsync(new ListaPreventaPage(_preventaService, _clienteService, _productoService,
                                   cbProvincias.SelectedItem.ToString(), cbCantones.SelectedItem.ToString(), cbDistritos.SelectedItem.ToString()));
        _userDialogs.HideHud();
    }

    private async void btnNueva_Clicked(object sender, EventArgs e)
    {
        _userDialogs.ShowLoading();
        await Navigation.PushAsync(new PreventaPage(_clienteService, _preventaService, _productoService));
        _userDialogs.HideHud();
    }

    private async void btnVerTodos_Clicked(object sender, EventArgs e)
    {
        _userDialogs.ShowLoading();
        await Navigation.PushAsync(new ListaPreventaPage(_preventaService, _clienteService, _productoService,null, null, null));
        _userDialogs.HideHud();
    }
}