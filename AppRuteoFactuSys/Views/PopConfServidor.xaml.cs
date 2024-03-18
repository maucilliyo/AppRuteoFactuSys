using AppRuteoFactuSys;
using AppRuteoFactuSys.MySql;
using Newtonsoft.Json;

namespace Inventario.Views;

public partial class PopConfServidor : ContentPage
{
    public PopConfServidor()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        SaveConection();
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // Código que deseas ejecutar cuando la página aparece
        await Conexion.GetConfig();
        txtBD.Text = Conexion.DataBase;
        txtIP.Text = Conexion.IP;
        txtPuerto.Text = Conexion.Puerto;
    }
    private async void SaveConection()
    {
        var conn = new { DataBase = txtBD.Text, IP = txtIP.Text, PUERTO = txtPuerto.Text };

        string data = JsonConvert.SerializeObject(conn);

        await SecureStorage.SetAsync("conexion.json", data);

        Conexion.IP = conn.IP;
        await DisplayAlert("Aviso", "DATOS DE CONEXIÓN GUARDADOS CON ÉXITO", "Aceptar");
    }
    private async void btnTestConection_Clicked(object sender, EventArgs e)
    {
        await this.DisplayAlert("Aviso", await Conexion.TestConexion(), "Aceptar");
    }
}