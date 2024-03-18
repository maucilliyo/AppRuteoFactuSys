using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
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

        private Preventa preventa = new();
        private Cliente cliente = new();
        public bool IsRefreshing { get; set; }
        public Command RefreshCommand { get; set; }
        public ICommand ModificarLineaCommand { get; }
        public static PreventaPage load;
        public PreventaPage(IClienteService clienteService, IPreventaService preventaService, IProductoService productoService)
        {
            InitializeComponent();
            _clienteService = clienteService;
            _preventaService = preventaService;
            _productoService = productoService;

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
            await Cargarlineas();
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
            if (clienteSeleccionado.TipoPrecio != cliente.TipoPrecio)
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
                        item.Cantidad += 1;
                    }
                }
            }
            else
            {
                // Si no existe, crea un nuevo elemento
                PreventaLineas linea = new()
                {
                    Cantidad = 1,
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
            lblTotal.Text = total.ToString("N2");
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
            Preventa preventa = new Preventa
            {
                Cedcliente = cliente.Cedula,
                Nombre_Cliente = lblNombreCliente.Text,
                CodigoMoneda = "CRC",
                CondicionVenta = "01",
                DiasPlazo = 0,
                Entregado = false,
                Facturado = false,
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

            await _preventaService.Nuevo(preventa);

            await DisplayAlert("Aviso", "Preventa Guardada con exito!", "Aceptar");

            await Navigation.PopAsync();
        }
    }
}
