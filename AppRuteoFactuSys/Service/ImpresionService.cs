using Android.Bluetooth;
using Android.Graphics.Fonts;
using Android.PrintServices;
using AppRuteoFactuSys.Models;
using Controls.UserDialogs.Maui;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Java.Util;
using System.Text;

namespace AppRuteoFactuSys.Service
{
    public class ImpresionService
    {

        public static async Task<bool> ImprimirTicket(Preventa preventa)
        {
            try
            {
                UserDialogs.Instance.ShowLoading();
                await Task.Delay(5);
                int catidadArticulos = 0;

                //
                #region PARAMETROS PARA LA IMPRESION
                //todo esto es para guardar el pdf
                string fileName = "Ticket.pdf";
                var docsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var documentsDirectory = System.IO.Path.Combine(docsDirectory, "Documents");

                // Verificar si el directorio de "Documentos" existe, si no, crearlo
                if (!Directory.Exists(documentsDirectory))
                {
                    Directory.CreateDirectory(documentsDirectory);
                }

                var filePath = System.IO.Path.Combine(documentsDirectory, fileName);

                // CREAMOS LA ESTRUCTURA DEL PDF
                // Define el tamaño personalizado para el papel
                int yPage = 400;//TAMAÑO INICIAL CON UNA SOLA LINEA
                yPage += preventa.Lineas.Count * 40;//aumenta por cada renglon extra por linea de la factura
                PageSize customPageSize = new(330, yPage);

                // Crear un nuevo documento PDF
                PdfWriter writer = new(filePath);
                PdfDocument pdf = new(writer);
                pdf.SetDefaultPageSize(customPageSize);
                // Establecer los márgenes izquierdo y derecho en cero
                Document document = new(pdf, new PageSize(customPageSize));
                document.SetMargins(0, 0, 0, 0);//MARGENES DEL DOCUMENTO
                                                // Configurar fuentes
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                #endregion

                #region ENCABEZADO
                // TITULO
                document.Add(new Paragraph("Distribuidora la Familia")
                    .SetFont(boldFont)
                    .SetMarginTop(30)
                    .SetFontSize(20)
                     .SetFixedLeading(15)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                //cedula
                document.Add(new Paragraph("Cedula:1-0725-0673")
                 .SetFont(boldFont)
                 .SetFixedLeading(6)
                 .SetFontSize(12)
                 .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                //telefonos
                document.Add(new Paragraph("Tel:3733-4394")
                 .SetFont(boldFont)
                 .SetFixedLeading(10)
                 .SetFontSize(12)
                 .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                //Email
                document.Add(new Paragraph("Email:arlenespinozaf@gmail.com")
                 .SetFont(font)
                 .SetFixedLeading(10)
                 .SetFontSize(12)
                 .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));


                document.Add(new Paragraph($"Referencia: {preventa.Nproforma}")
                 .SetFixedLeading(5)
                 .SetFontSize(12));

                document.Add(new Paragraph($"Fecha: {preventa.Fecha}")
                    .SetMarginLeft(25)
                    .SetFontSize(10));

                document.Add(new Paragraph($"Cliente: {preventa.Nombre_Cliente}")
                    .SetMarginLeft(25)
                    .SetFixedLeading(12)
                .SetFontSize(12)
                .SetMarginTop(1));

                document.Add(new Paragraph("--- Codigo ---- Detalle ---- PrecioU ---- Cant ---- Total ---")
                            .SetFont(boldFont)
                            .SetFixedLeading(2)
                            .SetMarginTop(10));
                document.Add(new Paragraph("____________________________________________________")
                    .SetFixedLeading(0)
                    .SetMarginBottom(15));
                //LINEAS
                // Ordenar preventa.Lineas alfabéticamente por el detalle
                var lineasOrdenadas = preventa.Lineas.OrderBy(linea => linea.Detalle);
                #endregion

                #region LINEAS
                // Iterar sobre las lineas ordenadas
                foreach (var item in lineasOrdenadas)
                {
                    // Crear tablas para usarlas como filas
                    Table fila1 = new(new UnitValue[] { UnitValue.CreatePointValue(40), UnitValue.CreatePointValue(250) });

                    Table fila2 = new(new UnitValue[] { UnitValue.CreatePointValue(100), UnitValue.CreatePointValue(100),
                                                    UnitValue.CreatePointValue(100), UnitValue.CreatePointValue(150) });


                    #region PRIMERA FILA
                    //agregamos el Codigo
                    fila1.AddCell(
                       new iText.Layout.Element.Cell()
                           .Add(new Paragraph(item.Codpro)
                                   .SetFont(font)
                                   .SetFontSize(11)
                                   .SetMarginTop(-5)
                                   .SetFixedLeading(10)
                                   .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                           ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                       );
                    //agregamos el detalle
                    fila1.AddCell(
                        new iText.Layout.Element.Cell()
                            .Add(new Paragraph(item.Detalle)
                                    .SetFont(font)
                                    .SetMarginTop(-5)
                                    .SetFontSize(11)
                                    .SetFixedLeading(10)
                            ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        );
                    #endregion

                    #region SEGUNDA FILA
                    //agregamos el Precio unitario
                    fila2.AddCell(
                        new iText.Layout.Element.Cell()
                            .Add(new Paragraph("%IVA: " + (item.Porimpuesto * 100).ToString("N0"))
                                   .SetFont(font)
                                   .SetFontSize(12)
                                   .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                   .SetMarginTop(0)

                            ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        );
                    //agregamos el Precio unitario
                    fila2.AddCell(
                        new iText.Layout.Element.Cell()
                            .Add(new Paragraph(item.PrecioUnidad.ToString("N2"))
                                   .SetFont(font)
                                   .SetFontSize(12)
                                   .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                                   .SetMarginTop(0)

                            ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        );
                    //agregamos la cantidad
                    fila2.AddCell(
                        new iText.Layout.Element.Cell()
                            .Add(new Paragraph(item.Cantidad.ToString())
                                    .SetFont(font)
                                    .SetFontSize(12)
                                    .SetMarginTop(0)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        );
                    //agregamos el total de la linea
                    fila2.AddCell(
                        new iText.Layout.Element.Cell()
                            .Add(new Paragraph(item.Subtotaldescuento.ToString("N2"))
                                    .SetFont(font)
                                    .SetFontSize(12)
                                    .SetMarginRight(30)
                                    .SetMarginTop(0)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                            ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        );
                    #endregion

                    // Agregar las tablas al documento
                    document.Add(fila1);
                    document.Add(fila2);
                    // Añadir la línea después de la tabla
                    document.Add(new Paragraph("_______________________________________________________________________________________________________")
                        .SetFont(font)
                        .SetFontSize(5)
                        .SetMarginBottom(8)
                        .SetMarginTop(-5));
                    //valorar la cantidad
                    if (item.UnidadMedida == "Unid")
                    {
                        catidadArticulos += Convert.ToInt32(item.Cantidad);
                    }
                    else
                    {
                        catidadArticulos++;
                    }
                }
                #endregion

                #region PIE DE PAGINA
                //Cantiad articulos
                document.Add(
                     new Paragraph($"Cantidad Aritculos: {catidadArticulos}")
                    .SetFont(boldFont)
                    .SetMarginTop(-10)
                    .SetFontSize(12)
                    .SetMarginRight(30)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    );
                //subtotal
                Table filaSubtotal = new(new UnitValue[] { UnitValue.CreatePointValue(220), UnitValue.CreatePointValue(90) });
                filaSubtotal.AddCell(
                   new iText.Layout.Element.Cell()
                       .Add(new Paragraph($"Subtotal:")
                        .SetFont(boldFont)
                        .SetMarginBottom(10)
                        .SetFixedLeading(0)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                       ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                   );

                filaSubtotal.AddCell(
                   new iText.Layout.Element.Cell()
                       .Add(new Paragraph(preventa.TotalVentaNeta.ToString("N2"))
                       .SetMarginBottom(10)
                       .SetFont(boldFont)
                       .SetFixedLeading(0)
                       .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                       .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT)
                       ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)

                   );

                document.Add(filaSubtotal);

                #region IMPUESTOS PARA MOSTRAR
                // Agrupa los impuestos por tipo
                var impuestosAgrupados = preventa.Lineas.Where(l => l.Porimpuesto > 0).GroupBy(impuesto => impuesto.Porimpuesto);
                // Recorre cada grupo de impuestos
                foreach (var grupoImpuestos in impuestosAgrupados.OrderBy(l => l.Key))
                {
                    Table fila1 = new(new UnitValue[] { UnitValue.CreatePointValue(220), UnitValue.CreatePointValue(90) });

                    fila1.AddCell(
                      new iText.Layout.Element.Cell()
                          .Add(new Paragraph($"Impuesto: {grupoImpuestos.Key * 100:N0}")
                           .SetMarginBottom(10)
                           .SetFixedLeading(0)
                           .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                          ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                      );

                    // Calcula el total del impuesto sumando los montos de todas las líneas de impuestos en este grupo
                    decimal totalImpuesto = grupoImpuestos.Sum(impuesto => impuesto.Impuestoneto);

                    fila1.AddCell(
                       new iText.Layout.Element.Cell()
                           .Add(new Paragraph($"{totalImpuesto:N2}")
                            .SetMarginBottom(10)
                           .SetFixedLeading(0)
                           .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                           .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT)
                           ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)

                       );

                    document.Add(fila1);
                }
                #endregion

                //TOTAL
                document.Add(
                     new Paragraph($"Total: {preventa.TotalComprobante:N0}")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetMarginRight(30)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    );

                //saltos
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                //final de documento
                document.Add(new Paragraph("Comprobante de entrega de mercancía")
                   .SetFont(font)
                   .SetWidth(250)
                   .SetMarginLeft(20)
                   .SetFontSize(11)
                   .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                //saltos
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph("."));
                #endregion

                // Cerrar el documento
                document.Close();
               // UserDialogs.Instance.HideHud();

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
                UserDialogs.Instance.HideHud();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        //busca en la lista de dispositivos 
        public static async Task<List<PrinterInfo>> GetAvailablePrinters()
        {
            var printerList = new List<PrinterInfo>();

            try
            {
                // Obtener el adaptador Bluetooth
                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                if (bluetoothAdapter == null)
                {
                    throw new Exception("El dispositivo no es compatible con Bluetooth.");
                }

                // Verificar si el Bluetooth está habilitado
                if (!bluetoothAdapter.IsEnabled)
                {
                    throw new Exception("El Bluetooth no está habilitado.");
                }

                // Obtener dispositivos emparejados
                var pairedDevices = bluetoothAdapter.BondedDevices;
                foreach (var device in pairedDevices)
                {
                    var printerInfo = new PrinterInfo
                    {
                        Name = device.Name,
                        // Agrega más propiedades según sea necesario, como dirección MAC, etc.
                    };
                    printerList.Add(printerInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las impresoras disponibles: " + ex.Message);
            }

            return printerList;
        }
        //imprime
        public static async Task PrintTicket(string deviceName, Preventa ticket)
        {
            // Crear una lista que incluya el encabezado y las líneas de contenido
            List<string> ticketContent =
                [
                    ticket.Nombre_Cliente,
                    ticket.TotalComprobante.ToString()
                ];

            // Convertir las líneas de texto en bytes utilizando la codificación UTF-8
            byte[] ticketData = Encoding.UTF8.GetBytes(string.Join("\n", ticketContent));

            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                          where bd?.Name == deviceName
                                          select bd).FirstOrDefault();
                try
                {
                    using (BluetoothSocket bluetoothSocket = device?.CreateRfcommSocketToServiceRecord(
                            UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                    {
                        bluetoothSocket?.Connect();

                        // Escribir los datos del ticket en el flujo de salida Bluetooth
                        bluetoothSocket?.OutputStream.Write(ticketData, 0, ticketData.Length);
                        bluetoothSocket.Close();
                    }
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
        }

    }
    public class PrinterInfo
    {
        public string Name { get; set; }
        // Agrega más propiedades según sea necesario, como dirección MAC, etc.
    }
}
