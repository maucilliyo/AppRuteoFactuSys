

using AppRuteoFactuSys;
using AppRuteoFactuSys.Service.Interfaces;

namespace AppRuteoFactuSys.Views;

public partial class LoginPage : ContentPage
{
    private readonly IClienteService _clienteService;
    private readonly IPreventaService _preventaService;
    private readonly IProductoService _productoService;
    public LoginPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        txtUserName.Text = "Admin";
        //txtPass.Text = "852";

        _clienteService = clienteService;
        _preventaService = preventaService;
        _productoService = productoService; 
    }

    private async void btnIniciar_Clicked(object sender, EventArgs e)
    {
        string user = txtUserName.Text;
        string pass = txtPass.Text;
        
        if (user == string.Empty || pass == string.Empty || user == null || pass == null)
        {
            await DisplayAlert("Aviso", "Debe ingresar usuario y contraseña", "Aceptar");
            return;
        }
        if (txtUserName.Text == "Admin" && txtPass.Text == "852")
        {
            if (Application.Current != null)
            {
                await Navigation.PopAsync();
                Application.Current.MainPage.Navigation.PushAsync(new MainPage(_clienteService, _preventaService, _productoService));
            }
        }
        else
        {
            await DisplayAlert("Aviso", "Error en usuario o contraseña", "Aceptar");
            return;
        }
    }
}