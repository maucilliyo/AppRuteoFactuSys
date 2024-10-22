

using AppRuteoFactuSys;
using AppRuteoFactuSys.Service.Interfaces;
using CommunityToolkit.Maui.Core.Platform;
using Controls.UserDialogs.Maui;

namespace AppRuteoFactuSys.Views;

public partial class LoginPage : ContentPage
{
    private readonly IClienteService _clienteService;
    private readonly IPreventaService _preventaService;
    private readonly IProductoService _productoService;
    private readonly IUserDialogs _userDialogs;
    public LoginPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService, IUserDialogs userDialogs)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        txtUserName.Text = "Admin";
        //txtPass.Text = "852";

        _clienteService = clienteService;
        _preventaService = preventaService;
        _productoService = productoService; 
        _userDialogs = userDialogs;
    }

    private async void btnIniciar_Clicked(object sender, EventArgs e)
    {
        string user = txtUserName.Text;
        string pass = txtPass.Text;
        if (KeyboardExtensions.IsSoftKeyboardShowing(txtPass))
        {
            await KeyboardExtensions.HideKeyboardAsync(txtPass, default);
        }
        if (user == string.Empty || pass == string.Empty || user == null || pass == null)
        {
            await DisplayAlert("Aviso", "Debe ingresar usuario y contraseņa", "Aceptar");
            return;
        }
        if (txtUserName.Text == "Admin" && txtPass.Text == "852")
        {
            if (Application.Current != null)
            {
                await Navigation.PopAsync();
                Application.Current.MainPage.Navigation.PushAsync(new MainPage(_clienteService, _preventaService, _productoService,_userDialogs));
            }
        }
        else
        {
            await DisplayAlert("Aviso", "Error en usuario o contraseņa", "Aceptar");
            return;
        }
    }
}