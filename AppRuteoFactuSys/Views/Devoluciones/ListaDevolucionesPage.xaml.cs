using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;
using Controls.UserDialogs.Maui;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views.Devoluciones;

public partial class ListaDevolucionesPage : ContentPage
{

    private readonly IClienteService _clienteService;

    private readonly LocalDevolucionRepository _localNotaCreditoRepository;
    private readonly ViewFactoryServices _viewFactoryServices;


    public bool IsRefreshing { get; set; }
    public Command RefreshCommand { get; set; }
    public ICommand SendSelectedDataCommand { get; private set; }
    public ICommand verPdfCommand { get; private set; }
    private string? _canton, _provincia, _distrito;
    public ListaDevolucionesPage(IClienteService clienteService, LocalDevolucionRepository localNotaCreditoRepository, ViewFactoryServices viewFactoryServices,
                                 string provincia = null, string canton = null, string distrito = null)
    {
        _viewFactoryServices = viewFactoryServices;

        RefreshCommand = new Command(async () =>
        {
            await CargarNotas();

            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        });

        BindingContext = this;
        InitializeComponent();
        _clienteService = clienteService;
        _localNotaCreditoRepository = localNotaCreditoRepository;
        SendSelectedDataCommand = new Command<object>(ExecuteSendSelectedDataCommand);
        verPdfCommand = new Command<object>(ExecuteVerPdfCommand);
        _canton = canton;
        _provincia = provincia;
        _distrito = distrito;
    }
    private async void ExecuteSendSelectedDataCommand(object parameter)
    {
        UserDialogs.Instance.ShowLoading();

        //// Aquí puedes manejar la lógica para procesar los datos de la fila seleccionada.
        //var rowData = (Preventa)parameter; // Asegúrate de cambiar YourDataType al tipo de objeto de tus datos
        //PreventaPage preventaPage = new(_clienteService, _preventaService, _productoService, true)
        //{
        //    idPreventa = rowData.LocalID
        //};
        //await Navigation.PushAsync(preventaPage);

        UserDialogs.Instance.HideHud();
    }
    private async void ExecuteVerPdfCommand(object parameter)
    {
        UserDialogs.Instance.ShowLoading();
        // Aquí puedes manejar la lógica para procesar los datos de la fila seleccionada.
        //var rowData = (Preventa)parameter; // Asegúrate de cambiar YourDataType al tipo de objeto de tus datos

        ////IMPRIMIR
        //var preventa = await _preventaService.GetById(rowData.LocalID);
        //await ImpresionService.ImprimirTicket(preventa);

        UserDialogs.Instance.HideHud();
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CargarNotas();
    }


    private async void btnNuevaNota_Clicked(object sender, EventArgs e)
    {
        var page = _viewFactoryServices.Create<DevolucionPage>();
        await Navigation.PushAsync(page);
    }

    private async Task CargarNotas()
    {
        //VALIDAR SI HAY CONEXION
        if (Conexion.GetConnection() == null)
        {
            await DisplayAlert("Aviso", "ERROR DE CONEXION CON EL SERVIDOR", "Aceptar");
            await Navigation.PopAsync();
        }
        if (!await Conexion.GetConfig())
        {
            await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
            await Navigation.PopAsync();
        }
        //VALIDAR SI LA BD SE CARGO BIEN
        var data = await _localNotaCreditoRepository.GetLista();
        cvPreventas.ItemsSource = data;
    }
    private async void btnNuevaProforma_Clicked(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new PreventaPage(_clienteService, _preventaService, _productoService));
    }
}