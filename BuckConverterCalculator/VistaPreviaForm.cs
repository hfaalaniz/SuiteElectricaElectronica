using BuckConverterCalculator;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class VistaPreviaForm : Form
    {
        private string contenidoReporte;
        private TransformadorCalculos calculadora;
        private double zoomActual = 1.0;

        public VistaPreviaForm(string contenido, TransformadorCalculos calc)
        {
            InitializeComponent();
            contenidoReporte = contenido;
            calculadora = calc;

            // Configurar documento de impresión
            printDocument1.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
        }

        private void VistaPreviaForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Configurar vista previa
                printPreviewControl1.Document = printDocument1;
                printPreviewControl1.Zoom = 1.0;
                zoomActual = 1.0;
                ActualizarLabelZoom();

                // Forzar actualización de vista previa
                printPreviewControl1.InvalidatePreview();

                lblEstado.Text = "Vista previa generada correctamente";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar vista previa: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                float rightMargin = e.MarginBounds.Right;
                float pageWidth = e.MarginBounds.Width;

                // Fuentes - usar tamaños absolutos
                Font fontTitulo = new Font("Arial", 14, FontStyle.Bold);
                Font fontSeccion = new Font("Arial", 10, FontStyle.Bold);
                Font fontNormal = new Font("Courier New", 7);
                Font fontPequena = new Font("Arial", 7);

                // Colores
                Brush brushAzul = new SolidBrush(Color.FromArgb(0, 122, 204));
                Brush brushNegro = Brushes.Black;
                Brush brushGris = Brushes.Gray;
                Pen penAzul = new Pen(Color.FromArgb(0, 122, 204), 2);

                // ENCABEZADO
                string titulo = "REPORTE DE CÁLCULO DE TRANSFORMADOR";
                SizeF sizeTitulo = g.MeasureString(titulo, fontTitulo);
                float xCentrado = leftMargin + (pageWidth - sizeTitulo.Width) / 2;
                g.DrawString(titulo, fontTitulo, brushAzul, xCentrado, yPos);
                yPos += sizeTitulo.Height + 5;

                // Línea decorativa
                g.DrawLine(penAzul, leftMargin, yPos, rightMargin, yPos);
                yPos += 15;

                // Fecha
                g.DrawString($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontPequena, brushGris, leftMargin, yPos);
                yPos += 25;

                // Verificar que calculadora no sea null
                if (calculadora != null)
                {
                    // RECUADRO DE ESPECIFICACIONES
                    float recuadroY = yPos;
                    float recuadroAlto = 110;
                    RectangleF recuadro = new RectangleF(leftMargin, recuadroY, pageWidth, recuadroAlto);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(240, 248, 255)), recuadro);
                    g.DrawRectangle(new Pen(Color.FromArgb(0, 122, 204), 1), recuadro);

                    yPos += 10;
                    g.DrawString("ESPECIFICACIONES PRINCIPALES", fontSeccion, brushNegro, leftMargin + 10, yPos);
                    yPos += 20;

                    // Dos columnas
                    float col1X = leftMargin + 15;
                    float col2X = leftMargin + pageWidth / 2 + 10;
                    float yTemp = yPos;

                    // Columna izquierda
                    DibujarLinea(g, "Potencia Total:", $"{calculadora.PotenciaTotalSalida:F2} VA",
                        col1X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Voltaje Primario:", $"{calculadora.VoltagePrimario:F0} V",
                        col1X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Voltaje Secundario:", $"{calculadora.VoltageSec1:F0} V",
                        col1X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Frecuencia:", $"{calculadora.Frecuencia:F0} Hz",
                        col1X, yTemp, fontNormal, brushNegro);

                    // Columna derecha
                    yTemp = yPos;
                    DibujarLinea(g, "Área Núcleo:", $"{calculadora.AreaNucleo:F2} cm²",
                        col2X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Eficiencia:", $"{calculadora.EficienciaReal:F2}%",
                        col2X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Factor Llenado:", $"{calculadora.FactorLlenado:F1}%",
                        col2X, yTemp, fontNormal, brushNegro);
                    yTemp += 14;
                    DibujarLinea(g, "Temp. Operación:", $"{calculadora.TempOperacionForzada:F1}°C",
                        col2X, yTemp, fontNormal, brushNegro);

                    yPos = recuadroY + recuadroAlto + 20;
                }

                // CONTENIDO DETALLADO
                g.DrawString("CÁLCULOS DETALLADOS", fontSeccion, brushNegro, leftMargin, yPos);
                yPos += 25;

                // Imprimir contenido del reporte
                if (!string.IsNullOrEmpty(contenidoReporte))
                {
                    string[] lineas = contenidoReporte.Split(new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None);

                    float lineHeight = fontNormal.GetHeight(g);
                    int maxLineas = (int)((e.MarginBounds.Bottom - yPos) / lineHeight);
                    int lineasImpresas = 0;

                    foreach (string linea in lineas)
                    {
                        if (lineasImpresas >= maxLineas)
                        {
                            e.HasMorePages = true;
                            break;
                        }

                        // Saltar líneas de separación
                        if (linea.Contains("═══"))
                            continue;

                        // Imprimir línea
                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            // Truncar líneas muy largas
                            string lineaParaImprimir = linea.Length > 90 ? linea.Substring(0, 90) : linea;
                            g.DrawString(lineaParaImprimir, fontNormal, brushNegro, leftMargin, yPos);
                            yPos += lineHeight;
                            lineasImpresas++;
                        }
                    }
                }

                // PIE DE PÁGINA
                float yPie = e.MarginBounds.Bottom + 10;
                g.DrawLine(new Pen(Color.LightGray, 1), leftMargin, yPie, rightMargin, yPie);
                yPie += 8;

                string pie = "Calculadora de Transformadores v2.1";
                SizeF sizePie = g.MeasureString(pie, fontPequena);
                float xPie = leftMargin + (pageWidth - sizePie.Width) / 2;
                g.DrawString(pie, fontPequena, brushGris, xPie, yPie);

                // Liberar recursos
                fontTitulo.Dispose();
                fontSeccion.Dispose();
                fontNormal.Dispose();
                fontPequena.Dispose();
                brushAzul.Dispose();
                penAzul.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al renderizar página: {ex.Message}\n\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DibujarLinea(Graphics g, string etiqueta, string valor, float x, float y,
            Font font, Brush brush)
        {
            g.DrawString(etiqueta, font, brush, x, y);
            g.DrawString(valor, font, brush, x + 140, y);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                    lblEstado.Text = "Documento enviado a impresora";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConfigPagina_Click(object sender, EventArgs e)
        {
            try
            {
                if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
                {
                    printPreviewControl1.InvalidatePreview();
                    lblEstado.Text = "Configuración de página actualizada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar página: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = $"Transformador_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ExportarAPDF(saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar PDF: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarAPDF(string rutaArchivo)
        {
            try
            {
                lblEstado.Text = "Generando PDF...";
                Application.DoEvents();

                using (var writer = new iText.Kernel.Pdf.PdfWriter(rutaArchivo))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                using (var document = new iText.Layout.Document(pdf))
                {
                    // Configurar documento
                    pdf.GetDocumentInfo().SetTitle("Reporte de Transformador");
                    pdf.GetDocumentInfo().SetAuthor("Calculadora de Transformadores v2.1");
                    pdf.GetDocumentInfo().SetSubject("Cálculo de Transformador de Potencia");

                    // Márgenes
                    document.SetMargins(50, 50, 50, 50);

                    // Colores
                    var colorAzul = new iText.Kernel.Colors.DeviceRgb(0, 122, 204);
                    var colorGris = new iText.Kernel.Colors.DeviceRgb(128, 128, 128);
                    var colorCeleste = new iText.Kernel.Colors.DeviceRgb(240, 248, 255);

                    // TÍTULO
                    var titulo = new iText.Layout.Element.Paragraph("REPORTE DE CÁLCULO DE TRANSFORMADOR")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(14)
                        .SetBold()
                        .SetFontColor(colorAzul);
                    document.Add(titulo);

                    // Línea separadora
                    var linea = new iText.Layout.Element.LineSeparator(
                        new iText.Kernel.Pdf.Canvas.Draw.SolidLine(2))
                        .SetMarginTop(5)
                        .SetMarginBottom(10);
                    document.Add(linea);

                    // Fecha
                    var fecha = new iText.Layout.Element.Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .SetFontSize(7)
                        .SetFontColor(colorGris);
                    document.Add(fecha);

                    if (calculadora != null)
                    {
                        // TABLA DE ESPECIFICACIONES
                        var tabla = new iText.Layout.Element.Table(
                            iText.Layout.Properties.UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                            .UseAllAvailableWidth()
                            .SetMarginTop(10)
                            .SetBackgroundColor(colorCeleste)
                            .SetBorder(new iText.Layout.Borders.SolidBorder(colorAzul, 1));

                        // Encabezado
                        var cellHeader = new iText.Layout.Element.Cell(1, 2)
                            .Add(new iText.Layout.Element.Paragraph("ESPECIFICACIONES PRINCIPALES")
                                .SetBold()
                                .SetFontSize(10))
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                        tabla.AddHeaderCell(cellHeader);

                        // Columna izquierda
                        var colIzq = new iText.Layout.Element.Cell()
                            .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                        colIzq.Add(new iText.Layout.Element.Paragraph($"Potencia Total: {calculadora.PotenciaTotalSalida:F2} VA").SetFontSize(8));
                        colIzq.Add(new iText.Layout.Element.Paragraph($"Voltaje Primario: {calculadora.VoltagePrimario:F0} V").SetFontSize(8));
                        colIzq.Add(new iText.Layout.Element.Paragraph($"Voltaje Secundario: {calculadora.VoltageSec1:F0} V").SetFontSize(8));
                        colIzq.Add(new iText.Layout.Element.Paragraph($"Frecuencia: {calculadora.Frecuencia:F0} Hz").SetFontSize(8));
                        tabla.AddCell(colIzq);

                        // Columna derecha
                        var colDer = new iText.Layout.Element.Cell()
                            .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                        colDer.Add(new iText.Layout.Element.Paragraph($"Área Núcleo: {calculadora.AreaNucleo:F2} cm²").SetFontSize(8));
                        colDer.Add(new iText.Layout.Element.Paragraph($"Eficiencia: {calculadora.EficienciaReal:F2}%").SetFontSize(8));
                        colDer.Add(new iText.Layout.Element.Paragraph($"Factor Llenado: {calculadora.FactorLlenado:F1}%").SetFontSize(8));
                        colDer.Add(new iText.Layout.Element.Paragraph($"Temp. Operación: {calculadora.TempOperacionForzada:F1}°C").SetFontSize(8));
                        tabla.AddCell(colDer);

                        document.Add(tabla);
                    }

                    // CÁLCULOS DETALLADOS
                    document.Add(new iText.Layout.Element.Paragraph("CÁLCULOS DETALLADOS")
                        .SetBold()
                        .SetFontSize(10)
                        .SetMarginTop(15));

                    // Contenido del reporte - usar fuente Courier
                    if (!string.IsNullOrEmpty(contenidoReporte))
                    {
                        string[] lineas = contenidoReporte.Split(new[] { "\r\n", "\r", "\n" },
                            StringSplitOptions.None);

                        foreach (string lineaPdf in lineas)
                        {
                            if (lineaPdf.Contains("═══")) continue;
                            if (!string.IsNullOrWhiteSpace(lineaPdf))
                            {
                                string lineaLimpia = lineaPdf.Length > 95 ? lineaPdf.Substring(0, 95) : lineaPdf;
                                var p = new iText.Layout.Element.Paragraph(lineaLimpia)
                                    .SetFontSize(7)
                                    .SetMargin(0)
                                    .SetMultipliedLeading(1.0f);

                                try
                                {
                                    p.SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(
                                        iText.IO.Font.Constants.StandardFonts.COURIER));
                                }
                                catch
                                {
                                    // Si falla Courier, usar fuente por defecto
                                }

                                document.Add(p);
                            }
                        }
                    }

                    // PIE DE PÁGINA (fijo al final)
                    var pie = new iText.Layout.Element.Paragraph("Calculadora de Transformadores v2.1")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(7)
                        .SetFontColor(colorGris)
                        .SetMarginTop(20);
                    document.Add(pie);
                }

                lblEstado.Text = $"PDF guardado: {Path.GetFileName(rutaArchivo)}";

                var result = MessageBox.Show($"PDF generado exitosamente:\n{rutaArchivo}\n\n¿Desea abrirlo ahora?",
                    "PDF Generado", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = rutaArchivo,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar PDF";
                MessageBox.Show($"Error al generar PDF:\n{ex.Message}\n\nDetalles:\n{ex.InnerException?.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DibujarPDFProfesional(object gfx, object page)
        {
            // Método obsoleto - ahora se usa iText7 directamente en ExportarAPDF
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (zoomActual < 3.0)
            {
                zoomActual += 0.25;
                printPreviewControl1.Zoom = zoomActual;
                ActualizarLabelZoom();
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (zoomActual > 0.25)
            {
                zoomActual -= 0.25;
                printPreviewControl1.Zoom = zoomActual;
                ActualizarLabelZoom();
            }
        }

        private void btnZoomAjustar_Click(object sender, EventArgs e)
        {
            printPreviewControl1.AutoZoom = true;
            zoomActual = printPreviewControl1.Zoom;
            ActualizarLabelZoom();
        }

        private void ActualizarLabelZoom()
        {
            lblZoom.Text = $"{(zoomActual * 100):F0}%";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
