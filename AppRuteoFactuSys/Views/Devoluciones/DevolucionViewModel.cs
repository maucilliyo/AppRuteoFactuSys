using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.Service;
using AppRuteoFactuSys.SqlLite;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppRuteoFactuSys.Views.Devoluciones
{
    public partial class DevolucionViewModel : ObservableObject
    {
        private readonly LocalDevolucionRepository _devolucionRepository;
        private readonly ViewFactoryServices _viewFactoryServices;

        private readonly IDialogService _dialogService;

        [ObservableProperty] private Devolucion _devolucionModel;
        [ObservableProperty] private Cliente _clienteModel;
        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private ObservableCollection<DevolucionLinea> _devolucionLineas;

        public DevolucionViewModel(LocalDevolucionRepository devolucionRepository, ViewFactoryServices viewFactoryServices, IDialogService dialogService)
        {
            _devolucionRepository = devolucionRepository;
            _viewFactoryServices = viewFactoryServices;
            _devolucionModel = new();
            _dialogService = dialogService;
        }

        [RelayCommand]
        private async Task AgregarProducto()
        {
            var page = _viewFactoryServices.Create<ProductoAdd2Page>();
            if (page is null) return;

            await Shell.Current.Navigation.PushModalAsync(page);
            var producto = await page.ProductoSeleccionadoTask;

            if (producto is not null)
            {

                if (ClienteModel == null)
                {
                    await _dialogService.ShowAlert("Aviso", "Debe selecionar un cliente primero");
                    return;
                }

                else if (ClienteModel.TipoPrecio != "C")
                {
                    if (ClienteModel.TipoPrecio == "A")
                        producto.PrecioVenta = producto.PrecioVentaA;
                    if (ClienteModel.TipoPrecio == "B")
                        producto.PrecioVenta = producto.PrecioVentaB;
                }

                decimal cantidad = 1;

                // Busca si ya existe un elemento con el mismo Codpro en la lista Lineas
                if (DevolucionModel.Lineas.Any(x => x.Codpro == producto.CodPro))
                {
                    foreach (var item in DevolucionModel.Lineas)
                    {
                        if (item.Codpro == producto.CodPro)
                        {
                            item.Cantidad += cantidad;
                            item.Subtotal = producto.PrecioVenta * item.Cantidad;
                            item.SubtotalDescuento = producto.PrecioVenta * item.Cantidad;
                            item.Impuesto = (producto.PrecioVenta * item.Cantidad) * producto.PorcientoImpuesto;
                            item.ImpuestoNeto = (producto.PrecioVenta * item.Cantidad) * producto.PorcientoImpuesto;
                            item.TotalLinea *= item.Cantidad;
                        }
                    }
                }
                else
                {
                    // Si no existe, crea un nuevo elemento
                    DevolucionLinea linea = new()
                    {
                        Cantidad = cantidad,
                        CodeCabys = producto.CodigoCabys,
                        CodigoImpuesto = producto.CodigoImpuesto,
                        CodigoTarifa = producto.CodigoTarifa,
                        Codpro = producto.CodPro,
                        Descuento = 0,
                        Detalle = producto.Detalle,
                        PrecioUnidad = producto.PrecioVenta,
                        PorExonerado = 0,
                        PorImpuesto = producto.PorcientoImpuesto,
                        Subtotal = producto.PrecioVenta * cantidad,
                        SubtotalDescuento = producto.PrecioVenta * cantidad,
                        Impuesto = (producto.PrecioVenta * cantidad) * producto.PorcientoImpuesto,
                        ImpuestoNeto = (producto.PrecioVenta * cantidad) * producto.PorcientoImpuesto,
                        UnidadMedida = producto.UnidadMedida,
                        UsaInventario = producto.UsaInventario,
                        TotalLinea = ((producto.PrecioVenta * cantidad) * (1 + producto.PorcientoImpuesto))
                    };


                    DevolucionModel.Lineas.Add(linea);

                }

                Totales();
            }
        }
        [RelayCommand]
        private async Task SeleccionarCliente()
        {
            var page = _viewFactoryServices.Create<ClienteAdd2Page>();
            if (page is null) return;

            await Shell.Current.Navigation.PushModalAsync(page);

            var cliente = await page.ClienteSeleccionadoTask;

            if (cliente is not null)
            {

                if (ClienteModel != null && cliente.TipoPrecio != ClienteModel.TipoPrecio && DevolucionLineas?.Count > 0)
                {
                    var responce = await _dialogService.ShowConfirm("CONFIRMAR", "SE CAMBIO EL TIPO DE CLIENTE, DEBE ELIMINAR LAS LINEAS AGREGADAS");

                    if (responce)
                    {
                        DevolucionModel.Lineas.Clear();
                        Totales();
                    }
                    else
                        return;
                }
                ClienteModel = cliente;
            }
        }
        [RelayCommand]
        private async Task Guardar()
        {
            //VALIDACIONES

            //

            try
            {
                DevolucionModel.Nombre = ClienteModel.Nombre;
                DevolucionModel.CedCliente = ClienteModel.Cedula;
                await _devolucionRepository.Nueva(DevolucionModel);

                await Shell.Current.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", ex.Message);
                throw;
            }
        }

        [RelayCommand]
        private async Task EditarLinea(DevolucionLinea devolucionLinea)
        {
            var page = _viewFactoryServices.Create<CantidadEditPage>();
            if (page is null) return;

            page.Configurar(devolucionLinea.Detalle, devolucionLinea.Cantidad);

            await Shell.Current.Navigation.PushModalAsync(page);

            var nuevaCantidad = await page.CantidadTask;

            if (nuevaCantidad is not null)
            {
                if (nuevaCantidad != devolucionLinea.Cantidad && nuevaCantidad > 0)
                {
                    devolucionLinea.Cantidad = nuevaCantidad.Value;
                    devolucionLinea.Subtotal = devolucionLinea.PrecioUnidad * devolucionLinea.Cantidad;
                    devolucionLinea.SubtotalDescuento = devolucionLinea.PrecioUnidad * nuevaCantidad.Value;
                    devolucionLinea.Impuesto = (devolucionLinea.PrecioUnidad * nuevaCantidad.Value) * devolucionLinea.PorImpuesto;
                    devolucionLinea.ImpuestoNeto = (devolucionLinea.PrecioUnidad * nuevaCantidad.Value) * devolucionLinea.PorImpuesto;
                    devolucionLinea.TotalLinea *= nuevaCantidad.Value;
                }
                else if (nuevaCantidad == 0)//PARA ELIMINAR
                {
                    DevolucionModel.Lineas.Remove(devolucionLinea);
                }
                // recalcular el total de la línea si aplica
                Totales();
            }
        }

        private void Totales()
        {
            DevolucionLineas = [.. DevolucionModel.Lineas];

            DevolucionModel.TotalComprobante = 0;
            DevolucionModel.TotalComprobante = DevolucionModel.Lineas.Sum(x => x.TotalLinea);


            OnPropertyChanged(nameof(DevolucionModel));
        }
    }
}
