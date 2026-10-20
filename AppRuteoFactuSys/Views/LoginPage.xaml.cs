using AppRuteoFactuSys.Service;
using CommunityToolkit.Maui.Core.Platform;

namespace AppRuteoFactuSys.Views;

public partial class LoginPage : ContentPage
{

    private readonly ViewFactoryServices _factory;
    public LoginPage(ViewFactoryServices viewFactory)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        txtUserName.Text = "Admin";
        //txtPass.Text = "852";

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
            await DisplayAlertAsync("Aviso", "Debe ingresar usuario y contraseña", "Aceptar");
            return;
        }
        if (txtUserName.Text == "Admin" && txtPass.Text == "852")
        {
            if (Application.Current != null)
            {
                await Navigation.PopAsync();

                var page = _factory.Create<MainPage>();

               await Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }
        else
        {
            await DisplayAlertAsync("Aviso", "Error en usuario o contraseña", "Aceptar");
            return;
        }
    }
}