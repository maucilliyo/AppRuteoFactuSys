using AppRuteoFactuSys.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AppRuteoFactuSys.Service
{
    public class ImpresionService
    {
        public static async void ImprimirTicket(Preventa preventa)
        {
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
            int yPage = 380;//TAMAÑO INICIAL CON UNA SOLA LINEA
            yPage += preventa.Lineas.Count * 35;//aumenta por cada renglon extra por linea de la factura
            PageSize customPageSize = new(320, yPage);

            // Crear un nuevo documento PDF
            PdfWriter writer = new(filePath);
            PdfDocument pdf = new(writer);
            pdf.SetDefaultPageSize(customPageSize);
            // Establecer los márgenes izquierdo y derecho en cero
            Document document = new(pdf, new PageSize(customPageSize));
            document.SetMargins(0, 0, 0, 0);//MARGENES DEL DOCUMENTO
            // Configurar fuentes
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            #endregion

            #region ENCABEZADO
            // TITULO
            document.Add(new Paragraph("Sistema de preventas")
                .SetFont(boldFont)
                .SetMarginTop(30)
                .SetFontSize(20)
                 .SetFixedLeading(15)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
            //cedula
            document.Add(new Paragraph("Cedula:1-1111-1111")
             .SetFont(boldFont)
             .SetFixedLeading(6)
             .SetFontSize(12)
             .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
            //telefonos
            document.Add(new Paragraph("Tel:8888-8888, 6666-6666")
             .SetFont(boldFont)
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
                .SetMarginBottom(20));
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
                               .SetFontSize(10)
                               .SetMarginTop(-10)
                               .SetFixedLeading(10)
                               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                       ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                   );
                //agregamos el detalle
                fila1.AddCell(
                    new iText.Layout.Element.Cell()
                        .Add(new Paragraph(item.Detalle)
                                .SetFont(font)
                                .SetMarginTop(-10)
                                .SetFontSize(10)
                                .SetFixedLeading(10)
                        ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    );
                #endregion

                #region SEGUNDA FILA
                //agregamos el Precio unitario
                fila2.AddCell(
                    new iText.Layout.Element.Cell()
                        .Add(new Paragraph("%IVA: " + (item.Porimpuesto * 100).ToString("N0"))
                               .SetFont(boldFont)
                               .SetFontSize(10)
                               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                               .SetMarginTop(0)

                        ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    );
                //agregamos el Precio unitario
                fila2.AddCell(
                    new iText.Layout.Element.Cell()
                        .Add(new Paragraph(item.PrecioUnidad.ToString("N2"))
                               .SetFont(boldFont)
                               .SetFontSize(10)
                               .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                               .SetMarginTop(0)

                        ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    );
                //agregamos la cantidad
                fila2.AddCell(
                    new iText.Layout.Element.Cell()
                        .Add(new Paragraph(item.Cantidad.ToString())
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetMarginTop(0)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        ).SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    );
                //agregamos el total de la linea
                fila2.AddCell(
                    new iText.Layout.Element.Cell()
                        .Add(new Paragraph(item.Subtotaldescuento.ToString("N2"))
                                .SetFont(boldFont)
                                .SetFontSize(10)
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
                document.Add(new Paragraph("_________________________________________________________________________________________________________________")
                    .SetFont(font)
                    .SetFontSize(5)
                    .SetMarginBottom(10)
                    .SetMarginTop(-7));
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
               .SetMarginLeft(40)
               .SetFontSize(10)
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
            #endregion

            // Cerrar el documento
            document.Close();
            // Abrir el archivo PDF con la aplicación predeterminada
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
