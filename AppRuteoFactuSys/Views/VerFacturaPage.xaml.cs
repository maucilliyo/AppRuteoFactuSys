using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace AppRuteoFactuSys.Views;

public partial class VerFacturaPage : ContentPage
{
    private readonly Preventa _preventa = new();
    public ObservableCollection<PreventaLineas> Lineas { get; set; } = [];
    public VerFacturaPage(Preventa preventa)
	{
        _preventa = preventa;
		InitializeComponent();
        CargarPreventaMod();
        BindingContext = this;
    }
    private  void CargarPreventaMod()
    {
        try
        {
            //obtenemos la preventa
            //cargamos los datos de la preventa en la pagina
            lblNombreCliente.Text = _preventa.Nombre_Cliente;
            //recorrer las lineas para agregarla a la preventa
            foreach (var linea in _preventa.Lineas)
            {
                Lineas.Add(linea);
            }
            dgLineas.ItemsSource = Lineas;
            //refrescamos los totales
            lblTotal.Text = _preventa.TotalComprobante.ToString("N0");
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }

    }
    private void btnImprimir_Clicked(object sender, EventArgs e)
    {
        ImpresionService.ImprimirTicket(_preventa);
    }

}