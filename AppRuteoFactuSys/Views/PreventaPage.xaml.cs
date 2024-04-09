using Android.Text;
using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.Service.Interfaces;
using Inventario.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRuteoFactuSys.Views
{
    public partial class PreventaPage : ContentPage
    {
        private readonly IClienteService _clienteService;
        private readonly IPreventaService _preventaService;
        private readonly IProductoService _productoService;
        public ObservableCollection<PreventaLineas> Lineas { get; set; } = [];
        public int idPreventa = 0;
        private bool _modificar { get; set; }
        private Cliente cliente = new();
        private bool IsRefreshing { get; set; }
        private bool alreadyLoaded { get; set; }
        public Command RefreshCommand { get; set; }
        public ICommand ModificarLineaCommand { get; }
        public static PreventaPage load;

        public PreventaPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService, bool modificar = false)
        {
            InitializeComponent();
            _clienteService = clienteService;
            _preventaService = preventaService;
            _productoService = productoService;
            _modificar = modificar;

            RefreshCommand = new Command(async () =>
            {
                await Cargarlineas();
                IsRefreshing = false;
                OnPropertyChanged(nameof(IsRefreshing));
            });

            ModificarLineaCommand = new Command<PreventaLineas>(OpenModifyLine);

            BindingContext = this;
            load = this;
        }
        private void OpenModifyLine(PreventaLineas linea)
        {
            Navigation.PushAsync(new EditarLineaPage(linea));
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!alreadyLoaded)
            {
                //si es una preventa para modificar
                if (_modificar)
                {
                    _ = Task.Run(() => Task.FromResult(CargarPreventaMod()));
                }
                else
                {
                    cliente = await _clienteService.GetByCedula("0");
                }
                await Cargarlineas();

                alreadyLoaded = true;
            }
        }
        //VALIDA EL BOTON CERRAR DEL TELEFONO
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var response = await DisplayAlert("AVISO", "¿Está seguro de salir sin guardar?", "Sí", "No");
                if (response)
                {
                    await Navigation.PopAsync();
                }
            });
            // Devuelve true para indicar que has manejado el evento y que no debe cerrarse la página automáticamente
            return true;
        }
        private async Task Cargarlineas()
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
            // dgLineas.ItemsSource = Lineas; No es necesario establecer la fuente de datos aquí si ya lo has hecho en XAML
        }
        private async void btnListaCliente_Clicked(object sender, EventArgs e)
        {
            ShowActivityIndicator();
            var clienteAddPage = new ClienteAddPage(_clienteService);
            // Suscribirte al evento ClienteSeleccionadoEvent
            clienteAddPage.ClienteSeleccionadoEvent += ClienteAddPage_ClienteSeleccionadoEvent;
            await Navigation.PushAsync(clienteAddPage);
            HideActivityIndicator();
        }
        private async void ClienteAddPage_ClienteSeleccionadoEvent(object sender, Cliente clienteSeleccionado)
        {
            if (clienteSeleccionado.TipoPrecio != cliente.TipoPrecio && Lineas.Count > 0)
            {
                var response = await DisplayAlert("AVISO", "El cliente tiene un precio diferente a los agregados, " +
                                                   "desea borrar los productos para agegar el nuevo cliente?", "Sí", "No");

                if (response)
                {
                    Lineas.Clear();
                    Totales();
                }
                else
                {
                    return;
                }

            }
            cliente = clienteSeleccionado;

            lblNombreCliente.Text = clienteSeleccionado.Nombre;
            lblTipoCliente.Text = clienteSeleccionado.TipoPrecio;
        }
        private async void ProductoAddPage_SeleccionadoEvent(object sender, Producto productoSeleccionado)
        {
            bool isNumbre = false;
            decimal cantidad = 1;
            string result;
            while (!isNumbre)
            {
                try
                {
                    result = await DisplayPromptAsync("Aviso", "Digite la cantidad", "Aceptar", "Cancelar", "Cantidad", keyboard: Keyboard.Numeric);
                    if (result == "")
                    {
                        isNumbre = false;
                    }
                    else if (string.IsNullOrEmpty(result))
                    {
                        isNumbre = false;
                    }
                    else if (!int.TryParse(result, out int value) && value > 0)
                    {
                        // El valor es válido, haz algo con él
                        isNumbre = false;
                    }
                    else
                    {
                        isNumbre = true;
                        cantidad = Convert.ToInt16(result);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Aviso", "error en cantidad "+ ex.Message, "Aceptar");
                    isNumbre = false;
                }

            }

            //VALIDAR EL TIPO DE PRECIO DEL CLIENTE
            if (cliente.TipoPrecio == "A")
                productoSeleccionado.PrecioVenta = productoSeleccionado.PrecioVentaA;
            else if (cliente.TipoPrecio == "B")
                productoSeleccionado.PrecioVenta = productoSeleccionado.PrecioVentaB;

            // Busca si ya existe un elemento con el mismo Codpro en la lista Lineas
            if (Lineas.Any(x => x.Codpro == productoSeleccionado.CodPro))
            {
                foreach (var item in Lineas)
                {
                    if (item.Codpro == productoSeleccionado.CodPro)
                    {
                        item.Cantidad += cantidad;
                    }
                }
            }
            else
            {
                // Si no existe, crea un nuevo elemento
                PreventaLineas linea = new()
                {
                    Cantidad = cantidad,
                    CodeCabys = productoSeleccionado.CodigoCabys,
                    CodigoImpuesto = productoSeleccionado.CodigoImpuesto,
                    CodigoTarifa = productoSeleccionado.CodigoTarifa,
                    Codpro = productoSeleccionado.CodPro,
                    Descuento = 0,
                    Detalle = productoSeleccionado.Detalle,
                    PorDescuento = 0,
                    Porexonerado = 0,
                    Porimpuesto = productoSeleccionado.PorcientoImpuesto,
                    PrecioUnidad = productoSeleccionado.PrecioVenta,
                    UnidadMedida = productoSeleccionado.UnidadMedida,
                    UsaInventario = productoSeleccionado.UsaInventario
                };

                // Agrega el nuevo elemento a la lista Lineas
                Lineas.Add(linea);
            }

            Totales();
        }
        private void ShowActivityIndicator()
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
        }
        private void HideActivityIndicator()
        {
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }
        private void Totales()
        {
            // Actualiza el DataGrid estableciendo de nuevo la lista Lineas como ItemsSource
            dgLineas.ItemsSource = null;
            dgLineas.ItemsSource = Lineas;

            decimal total = 0;
            foreach (var item in Lineas)
            {
                total += item.TotalLinea;
            }
            lblTotal.Text = total.ToString("N0");
        }
        public async void ModificarLinea(PreventaLineas linea, string accion)
        {
            if (accion == "mod")//SI LA ACCION ES MODIFICAR
            {
                var lineaModificar = Lineas.FirstOrDefault(l => l.Codpro == linea.Codpro);
                if (lineaModificar != null)
                {
                    // Actualiza la cantidad de la línea
                    lineaModificar.Cantidad = linea.Cantidad;
                }
            }
            else//LA ACCION ES ELIMINAR
            {
                var response = await DisplayAlert("AVISO", "Esta seguro de eliminar la linea?", "Sí", "No");

                if (response)
                {
                    Lineas.Remove(linea);
                }
            }
            Totales();
        }
        private async void btnListaProductos_Clicked(object sender, EventArgs e)
        {
            ShowActivityIndicator();
            var productoAddPage = new ProductoAddPage(_productoService);
            // Suscribirte al evento ClienteSeleccionadoEvent
            productoAddPage.SeleccionadoEvent += ProductoAddPage_SeleccionadoEvent;
            await Navigation.PushAsync(productoAddPage);
            HideActivityIndicator();
        }
        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            await GuardarPreventa();
        }
        private async Task GuardarPreventa(bool entregado = false)
        {
            //validaciones
            if (Lineas.Count == 0)
            {
                await DisplayAlert("Aviso", "Debe haber al menos una linea", "Aceptar");
                return;
            }
            Preventa preventa = new()
            {
                Cedcliente = cliente.Cedula,
                Nombre_Cliente = lblNombreCliente.Text,
                CodigoMoneda = "CRC",
                CondicionVenta = "01",
                DiasPlazo = 0,
                Entregado = entregado,
                Estado = entregado ? "Entregado" : "Pendiente",
                Fecha = DateTime.Now,
                FechaUpdate = DateTime.Now,
                Formapago = "01",
                Id_Usuario = 0,
                Terminal = "00000",
                TipoClienteImpuesto = "GRAVADO",
                Lineas = Lineas.ToList()
            };

            foreach (var item in preventa.Lineas)
            {
                if (item.CodigoTarifa != "01")// 01 es exento
                {
                    preventa.TotalImpuesto += item.Impuestoneto;

                    if (item.Porexonerado > 0 && item.Porimpuesto > 0)
                    {
                        decimal MontoGravado = (1 - (item.Porexonerado) / item.Porimpuesto) * item.Subtotal;
                        preventa.TotalMercanciasGravadas += MontoGravado;
                        preventa.MercanciasExoneradas += (item.Subtotal - MontoGravado);
                    }
                    else
                    {
                        preventa.TotalMercanciasGravadas += item.Subtotal;
                    }
                }
                else
                {
                    preventa.TotalMercanciasExentas += item.Subtotal;
                }
            }

            preventa.TotalGrabado = preventa.TotalServGravados + preventa.TotalMercanciasGravadas;
            preventa.TotalExento = preventa.TotalServExentos + preventa.TotalMercanciasExentas;
            preventa.TotalExonerado = preventa.ServiciosExonerados + preventa.MercanciasExoneradas;

            preventa.TotalVenta = preventa.TotalGrabado + preventa.TotalExento + preventa.TotalExonerado;
            preventa.TotalVentaNeta = preventa.TotalVenta - preventa.TotalDescuento;
            preventa.TotalComprobante = preventa.TotalVentaNeta + preventa.TotalImpuesto;
            //evaluar si hay que modificar o crear una nueva preventa
            if (_modificar)
            {
                preventa.LocalID = idPreventa;
                await _preventaService.ActualizarOnly(preventa);
                await DisplayAlert("Aviso", "Preventa Actualizada con exito!", "Aceptar");
            }
            else
            {
                await _preventaService.Nuevo(preventa);
                await DisplayAlert("Aviso", "Preventa Guardada con exito!", "Aceptar");
            }
            await Navigation.PopAsync();
        }
        private async Task CargarPreventaMod()
        {
            //obtenemos la preventa
            var preventa = await _preventaService.GetById(idPreventa);
            //cargamos el cliente
            cliente = await _clienteService.GetByCedula(preventa.Cedcliente);
            //cargamos los datos de la preventa en la pagina
            lblNombreCliente.Text = preventa.Nombre_Cliente;
            lblTipoCliente.Text = cliente.TipoPrecio;
            //recorrer las lineas para agregarla a la preventa
            foreach (var linea in preventa.Lineas)
            {
                Lineas.Add(linea);
            }
            //refrescamos los totales
            Totales();
            btnFacturar.IsEnabled = true;
            btnFacturar.BackgroundColor = Color.FromRgb(255, 143, 107);
        }
        private async void btnFacturar_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("AVISO", "Esta seguro de facturar esta preventa?", "Sí", "No");

            if (!response)
            {
                return;
            }
            //primero la guardamos por aquello de cambios y le enviamos entregado  = true
            await GuardarPreventa(true);
            //IMPRIMIR
            var preventa = await _preventaService.GetById(idPreventa);
            ImpresionService.ImprimirTicket(preventa);
        }
    }
}
