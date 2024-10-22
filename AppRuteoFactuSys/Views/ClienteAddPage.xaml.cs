using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.Models;

namespace AppRuteoFactuSys.Views
{
    public partial class ClienteAddPage : ContentPage
    {
        private readonly IClienteService _clienteService;

        public event EventHandler<Cliente> ClienteSeleccionadoEvent;
        public bool IsRefreshing { get; set; }
        public Command RefreshCommand { get; set; }
        public ICommand SendSelectedDataCommand { get; private set; }

        public ClienteAddPage(IClienteService clienteService)
        {
            RefreshCommand = new Command(async () =>
            {
                await CargarClientes();

                IsRefreshing = false;
                OnPropertyChanged(nameof(IsRefreshing));
            });
            InitializeComponent();

            _clienteService = clienteService;

            SendSelectedDataCommand = new Command(SendSelectedData);
            BindingContext = this;

            CargarClientes();
        }

        private async Task CargarClientes()
        {
            //VALIDAR SI HAY CONEXION
            if (Conexion.GetConnection() == null)
            {
                await DisplayAlert("Aviso", "ERROR DE CONEXION CON EL SERVIDOR", "Aceptar");
                await Navigation.PopAsync();
                return;
            }

            if (!await Conexion.GetConfig())
            {
                await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
                await Navigation.PopAsync();
                return;
            }

            //VALIDAR SI LA BD SE CARGO BIEN
            var clientes = await _clienteService.Listar();
            dgClientes.ItemsSource = clientes;
        }

        private void SendSelectedData(object parameter)
        {
            Cliente clienteSeleccionado = (Cliente)parameter;
            // Disparar el evento y pasar el cliente seleccionado como argumento
            ClienteSeleccionadoEvent?.Invoke(this, clienteSeleccionado);
            // Volver a la p�gina anterior (PreventaPage)
            Navigation.PopAsync();
        }

        private async void btnBuscar_Clicked(object sender, EventArgs e)
        {
            var clientes = await _clienteService.Listar(txtBuscar.Text);
            dgClientes.ItemsSource = clientes;
        }

        private void txtBuscar_Completed(object sender, EventArgs e)
        {
            btnBuscar_Clicked(sender, e);
        }
    }
}
