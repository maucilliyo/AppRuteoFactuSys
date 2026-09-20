using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.Models;

namespace AppRuteoFactuSys.Views
{
    public partial class ClienteAdd2Page : ContentPage
    {
        private readonly IClienteService _clienteService;
        private readonly TaskCompletionSource<Cliente> _tcs = new();

        public event EventHandler<Cliente> ClienteSeleccionadoEvent;
        public Task<Cliente> ClienteSeleccionadoTask => _tcs.Task;
        public bool IsRefreshing { get; set; }
        public Command RefreshCommand { get; set; }
        public ICommand SendSelectedDataCommand { get; private set; }

        public ClienteAdd2Page(IClienteService clienteService)
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
                _tcs.TrySetResult(null);
                await Navigation.PopModalAsync();
                return;
            }

            if (!await Conexion.GetConfig())
            {
                await DisplayAlert("Aviso", "EL ARCHIVO DE CONEXION NO SE ENCUENTRA", "Aceptar");
                _tcs.TrySetResult(null);
                await Navigation.PopModalAsync();
                return;
            }

            //VALIDAR SI LA BD SE CARGO BIEN
            var clientes = await _clienteService.Listar();
            cvClientes.ItemsSource = clientes;
        }

        private void SendSelectedData(object parameter)
        {
            Cliente clienteSeleccionado = (Cliente)parameter;
            // Disparar el evento y pasar el cliente seleccionado como argumento
            ClienteSeleccionadoEvent?.Invoke(this, clienteSeleccionado);
            // Resolver la task para quien está esperando con await
            _tcs.TrySetResult(clienteSeleccionado);
            // Volver a la página anterior (se abrió con PushModalAsync)
            Navigation.PopModalAsync();
        }

        private async void btnBuscar_Clicked(object sender, EventArgs e)
        {
            var clientes = await _clienteService.Listar(txtBuscar.Text);
            cvClientes.ItemsSource = clientes;
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
}