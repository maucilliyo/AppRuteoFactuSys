

using AppRuteoFactuSys;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;
using CommunityToolkit.Maui.Core.Platform;
using Controls.UserDialogs.Maui;
using Microsoft.Maui.ApplicationModel;

namespace AppRuteoFactuSys.Views;

public partial class LoginPage : ContentPage
{
    private readonly IClienteService _clienteService;
    private readonly IPreventaService _preventaService;
    private readonly IProductoService _productoService;
    private readonly IUserDialogs _userDialogs;
    private readonly ViewFactoryServices _factory;
    public LoginPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService, IUserDialogs userDialogs, ViewFactoryServices viewFactory)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);



        txtUserName.Text = "Admin";
        txtPass.Text = "852";

        _clienteService = clienteService;
        _preventaService = preventaService;
        _productoService = productoService;
        _userDialogs = userDialogs;
        _factory = viewFactory;
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
            await DisplayAlert("Aviso", "Debe ingresar usuario y contraseña", "Aceptar");
            return;
        }
        if (txtUserName.Text == "Admin" && txtPass.Text == "852")
        {
            //OBTENER CONECION SQL LITE LOCAL
            await SqlLiteDatabase.GetConnection();
            //
            if (Application.Current != null)
            {
                await Navigation.PopAsync();

                var page = _factory.Create<MainPage>();

                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }
        else
        {
            await DisplayAlert("Aviso", "Error en usuario o contraseña", "Aceptar");
            return;
        }
    }
}