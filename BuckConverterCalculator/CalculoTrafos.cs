using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace BuckConverterCalculator
{
    // Sistema de Logging para Debug
    

    public class CalculoTrafos : Form
    {
        private ComboBox cmbTipoEntrada;
        private NumericUpDown numVoltajeEntrada;
        private NumericUpDown numNumSalidas;
        private NumericUpDown numVoltajeSalida1;
        private NumericUpDown numCorrienteSalida1;
        private NumericUpDown numVoltajeSalida2;
        private NumericUpDown numCorrienteSalida2;
        private NumericUpDown numFrecuencia;
        private NumericUpDown numDensidadFlujo;
        private NumericUpDown numDensidadCorriente;
        private ComboBox cmbTipoNucleo;
        private ComboBox cmbMaterialNucleo;
        private NumericUpDown numEficiencia;
        private NumericUpDown numFactorLlenado;
        private NumericUpDown numTempAmbiente;
        private Button btnCalcular;
        private Button btnVistaPrevia;
        private Button btnExportarPDF;
        private Button btnVerLog;
        private RichTextBox txtResultados;
        private string resultadoActual = "";

        public CalculoTrafos()
        {
            DebugLogger.Log("INIT", "Iniciando aplicación de cálculo de transformadores");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Calculadora de Transformadores de Potencia v2.0";
            this.Size = new Size(900, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            int yPos = 20;
            int controlWidth = 150;

            AddLabel("Tipo de Entrada:", 20, yPos);
            cmbTipoEntrada = new ComboBox { Location = new Point(230, yPos), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoEntrada.Items.AddRange(new object[] { "Monofásico", "Trifásico", "Trifásico + Neutro" });
            cmbTipoEntrada.SelectedIndex = 2;
            this.Controls.Add(cmbTipoEntrada);
            yPos += 35;

            AddLabel("Voltaje Entrada (V):", 20, yPos);
            numVoltajeEntrada = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 1, Maximum = 50000, Value = 220, DecimalPlaces = 1 };
            this.Controls.Add(numVoltajeEntrada);
            yPos += 35;

            AddLabel("Frecuencia (Hz):", 20, yPos);
            numFrecuencia = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 50, Maximum = 400, Value = 60, DecimalPlaces = 0 };
            this.Controls.Add(numFrecuencia);
            yPos += 35;

            AddLabel("Número de Salidas:", 20, yPos);
            numNumSalidas = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 1, Maximum = 2, Value = 2 };
            numNumSalidas.ValueChanged += (s, e) => ActualizarVisibilidadSalidas();
            this.Controls.Add(numNumSalidas);
            yPos += 35;

            AddLabel("Salida 1 - Voltaje (V):", 20, yPos);
            numVoltajeSalida1 = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 1, Maximum = 10000, Value = 110, DecimalPlaces = 1 };
            this.Controls.Add(numVoltajeSalida1);
            yPos += 35;

            AddLabel("Salida 1 - Corriente (A):", 20, yPos);
            numCorrienteSalida1 = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 0.1m, Maximum = 10000, Value = 30, DecimalPlaces = 2 };
            this.Controls.Add(numCorrienteSalida1);
            yPos += 35;

            AddLabel("Salida 2 - Voltaje (V):", 20, yPos);
            numVoltajeSalida2 = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 1, Maximum = 10000, Value = 110, DecimalPlaces = 1 };
            this.Controls.Add(numVoltajeSalida2);
            yPos += 35;

            AddLabel("Salida 2 - Corriente (A):", 20, yPos);
            numCorrienteSalida2 = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 0.1m, Maximum = 10000, Value = 30, DecimalPlaces = 2 };
            this.Controls.Add(numCorrienteSalida2);
            yPos += 35;

            AddLabel("Tipo de Núcleo:", 20, yPos);
            cmbTipoNucleo = new ComboBox { Location = new Point(230, yPos), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoNucleo.Items.AddRange(new object[] { "E-I Laminado", "Toroidal", "C-Core", "UI Laminado" });
            cmbTipoNucleo.SelectedIndex = 0;
            this.Controls.Add(cmbTipoNucleo);
            yPos += 35;

            AddLabel("Material Núcleo:", 20, yPos);
            cmbMaterialNucleo = new ComboBox { Location = new Point(230, yPos), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMaterialNucleo.Items.AddRange(new object[] { "Acero Silicio M19", "Acero Silicio M27", "Acero Silicio Grano Orientado", "Ferrita" });
            cmbMaterialNucleo.SelectedIndex = 0;
            cmbMaterialNucleo.SelectedIndexChanged += (s, e) => ActualizarDensidadFlujo();
            this.Controls.Add(cmbMaterialNucleo);
            yPos += 35;

            AddLabel("Densidad Flujo (Tesla):", 20, yPos);
            numDensidadFlujo = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 0.1m, Maximum = 2.0m, Value = 1.5m, DecimalPlaces = 3, Increment = 0.01m };
            this.Controls.Add(numDensidadFlujo);
            yPos += 35;

            AddLabel("Densidad Corriente (A/mm²):", 20, yPos);
            numDensidadCorriente = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 1.5m, Maximum = 6.0m, Value = 3.5m, DecimalPlaces = 2, Increment = 0.1m };
            this.Controls.Add(numDensidadCorriente);
            yPos += 35;

            AddLabel("Factor Llenado (%):", 20, yPos);
            numFactorLlenado = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 20, Maximum = 60, Value = 40, DecimalPlaces = 0 };
            this.Controls.Add(numFactorLlenado);
            yPos += 35;

            AddLabel("Temperatura Ambiente (°C):", 20, yPos);
            numTempAmbiente = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 0, Maximum = 60, Value = 25, DecimalPlaces = 0 };
            this.Controls.Add(numTempAmbiente);
            yPos += 35;

            AddLabel("Eficiencia Esperada (%):", 20, yPos);
            numEficiencia = new NumericUpDown { Location = new Point(230, yPos), Width = controlWidth, Minimum = 80, Maximum = 99, Value = 95, DecimalPlaces = 1 };
            this.Controls.Add(numEficiencia);
            yPos += 35;

            btnCalcular = new Button { Text = "CALCULAR", Location = new Point(20, yPos), Width = 110, Height = 35, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCalcular.Click += BtnCalcular_Click;
            this.Controls.Add(btnCalcular);

            btnVistaPrevia = new Button { Text = "VISTA PREVIA", Location = new Point(140, yPos), Width = 110, Height = 35, BackColor = Color.FromArgb(16, 124, 16), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnVistaPrevia.Click += BtnVistaPrevia_Click;
            this.Controls.Add(btnVistaPrevia);

            btnExportarPDF = new Button { Text = "EXPORTAR PDF", Location = new Point(260, yPos), Width = 110, Height = 35, BackColor = Color.FromArgb(200, 0, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExportarPDF.Click += BtnExportarPDF_Click;
            this.Controls.Add(btnExportarPDF);

            btnVerLog = new Button { Text = "VER LOG", Location = new Point(380, yPos), Width = 90, Height = 35, BackColor = Color.FromArgb(100, 100, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnVerLog.Click += BtnVerLog_Click;
            this.Controls.Add(btnVerLog);
            yPos += 50;

            txtResultados = new RichTextBox { Location = new Point(20, yPos), Width = 840, Height = 280, ReadOnly = true, Font = new Font("Consolas", 9F) };
            this.Controls.Add(txtResultados);
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y + 3), Width = 200, TextAlign = ContentAlignment.MiddleLeft };
            this.Controls.Add(lbl);
        }

        private void ActualizarVisibilidadSalidas()
        {
            bool mostrarSalida2 = numNumSalidas.Value == 2;
            numVoltajeSalida2.Enabled = mostrarSalida2;
            numCorrienteSalida2.Enabled = mostrarSalida2;
            DebugLogger.Log("UI", "Salida 2 {0}", mostrarSalida2 ? "habilitada" : "deshabilitada");
        }

        private void ActualizarDensidadFlujo()
        {
            decimal nuevoValor = cmbMaterialNucleo.SelectedIndex switch
            {
                0 => 1.5m,   // M19
                1 => 1.4m,   // M27
                2 => 1.7m,   // Grano orientado
                3 => 0.35m,  // Ferrita
                _ => 1.5m
            };
            numDensidadFlujo.Value = nuevoValor;
            DebugLogger.Log("UI", "Densidad de flujo ajustada a {0} T para material {1}",
                nuevoValor, cmbMaterialNucleo.SelectedItem);
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                DebugLogger.Log("CALC", "════════════════ INICIO DE CÁLCULO ════════════════");

                var calc = new TransformerCalculatore
                {
                    TipoEntrada = cmbTipoEntrada.SelectedIndex,
                    VoltajeEntrada = (double)numVoltajeEntrada.Value,
                    Frecuencia = (double)numFrecuencia.Value,
                    NumSalidas = (int)numNumSalidas.Value,
                    VoltajeSalida1 = (double)numVoltajeSalida1.Value,
                    CorrienteSalida1 = (double)numCorrienteSalida1.Value,
                    VoltajeSalida2 = (double)numVoltajeSalida2.Value,
                    CorrienteSalida2 = (double)numCorrienteSalida2.Value,
                    DensidadFlujo = (double)numDensidadFlujo.Value,
                    DensidadCorriente = (double)numDensidadCorriente.Value,
                    TipoNucleo = cmbTipoNucleo.SelectedIndex,
                    MaterialNucleo = cmbMaterialNucleo.SelectedIndex,
                    Eficiencia = (double)numEficiencia.Value / 100.0,
                    FactorLlenado = (double)numFactorLlenado.Value / 100.0,
                    TempAmbiente = (double)numTempAmbiente.Value
                };

                resultadoActual = calc.Calcular();
                txtResultados.Text = resultadoActual;

                DebugLogger.Log("CALC", "════════════════ FIN DE CÁLCULO ════════════════");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("Error durante el cálculo", ex);
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVistaPrevia_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(resultadoActual))
            {
                MessageBox.Show("Primero debes calcular el transformador.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += PrintDoc_PrintPage;

                PrintPreviewDialog preview = new PrintPreviewDialog
                {
                    Document = printDoc,
                    Width = 800,
                    Height = 600
                };
                preview.ShowDialog();
                DebugLogger.Log("UI", "Vista previa mostrada correctamente");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("Error en vista previa", ex);
            }
        }

        private void BtnExportarPDF_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(resultadoActual))
            {
                MessageBox.Show("Primero debes calcular el transformador.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Guardar como PDF",
                FileName = $"Transformador_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportarAPDF(saveDialog.FileName);
                    MessageBox.Show("PDF exportado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DebugLogger.Log("FILE", "PDF exportado: {0}", saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError("Error al exportar PDF", ex);
                    MessageBox.Show($"Error al exportar PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnVerLog_Click(object sender, EventArgs e)
        {
            try
            {
                string logPath = DebugLogger.GetLogPath();
                if (File.Exists(logPath))
                {
                    Process.Start("notepad.exe", logPath);
                }
                else
                {
                    MessageBox.Show("No se ha generado log aún.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Courier New", 9);
            Font titleFont = new Font("Courier New", 12, FontStyle.Bold);
            Brush brush = Brushes.Black;

            float yPos = 50;
            float leftMargin = 50;

            e.Graphics.DrawString("CÁLCULO DE TRANSFORMADOR DE POTENCIA", titleFont, brush, leftMargin, yPos);
            yPos += 40;

            string[] lines = resultadoActual.Split('\n');
            foreach (string line in lines)
            {
                e.Graphics.DrawString(line, font, brush, leftMargin, yPos);
                yPos += 14;

                if (yPos > e.PageBounds.Height - 100)
                {
                    e.HasMorePages = true;
                    return;
                }
            }
            e.HasMorePages = false;
        }

        private void ExportarAPDF(string fileName)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            printDoc.PrinterSettings.PrintToFile = true;
            printDoc.PrinterSettings.PrintFileName = fileName;

            if (!printDoc.PrinterSettings.IsValid)
            {
                File.WriteAllText(fileName.Replace(".pdf", ".txt"), resultadoActual, Encoding.UTF8);
                MessageBox.Show("No se encontró impresora PDF. Se guardó como TXT.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            printDoc.Print();
        }
    }

    public class TransformerCalculatore
    {
        public int TipoEntrada { get; set; }
        public double VoltajeEntrada { get; set; }
        public double Frecuencia { get; set; }
        public int NumSalidas { get; set; }
        public double VoltajeSalida1 { get; set; }
        public double CorrienteSalida1 { get; set; }
        public double VoltajeSalida2 { get; set; }
        public double CorrienteSalida2 { get; set; }
        public double DensidadFlujo { get; set; }
        public double DensidadCorriente { get; set; }
        public int TipoNucleo { get; set; }
        public int MaterialNucleo { get; set; }
        public double Eficiencia { get; set; }
        public double FactorLlenado { get; set; }
        public double TempAmbiente { get; set; }

        // Constantes físicas precisas
        private const double RESISTIVIDAD_COBRE_20C = 1.7241e-8; // Ohm·m a 20°C
        private const double COEF_TEMP_COBRE = 0.00393; // 1/°C
        private const double PERMEABILIDAD_VACIO = 4 * Math.PI * 1e-7; // H/m

        public string Calcular()
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("              CÁLCULO DE TRANSFORMADOR DE POTENCIA v2.0");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            // PASO 1: Cálculo de potencias
            DebugLogger.Log("CALC", "--- PASO 1: Cálculo de Potencias ---");
            double potenciaSalida1 = VoltajeSalida1 * CorrienteSalida1;
            double potenciaSalida2 = NumSalidas == 2 ? VoltajeSalida2 * CorrienteSalida2 : 0;
            double potenciaTotal = potenciaSalida1 + potenciaSalida2;
            double potenciaEntrada = potenciaTotal / Eficiencia;
            double perdidaEstimada = potenciaEntrada - potenciaTotal;

            DebugLogger.LogCalculation("Potencia Salida 1", potenciaSalida1, "VA");
            DebugLogger.LogCalculation("Potencia Salida 2", potenciaSalida2, "VA");
            DebugLogger.LogCalculation("Potencia Total Salida", potenciaTotal, "VA");
            DebugLogger.LogCalculation("Potencia Entrada", potenciaEntrada, "VA");
            DebugLogger.LogCalculation("Pérdidas Estimadas", perdidaEstimada, "W");

            sb.AppendLine("POTENCIAS:");
            sb.AppendLine($"  Salida 1:        {potenciaSalida1:F2} VA ({potenciaSalida1 / 1000:F3} kVA)");
            if (NumSalidas == 2)
                sb.AppendLine($"  Salida 2:        {potenciaSalida2:F2} VA ({potenciaSalida2 / 1000:F3} kVA)");
            sb.AppendLine($"  Total Salida:    {potenciaTotal:F2} VA ({potenciaTotal / 1000:F3} kVA)");
            sb.AppendLine($"  Entrada Req.:    {potenciaEntrada:F2} VA ({potenciaEntrada / 1000:F3} kVA)");
            sb.AppendLine($"  Pérdidas Est.:   {perdidaEstimada:F2} W ({(perdidaEstimada / potenciaTotal * 100):F2}%)");
            sb.AppendLine();

            // PASO 2: Cálculo del área del núcleo con fórmula mejorada
            DebugLogger.Log("CALC", "--- PASO 2: Dimensionamiento del Núcleo ---");

            // Fórmula de Faraday: E = 4.44 * f * N * Bmax * Ae
            // Producto de áreas: Ap = Ae * Aw (método del producto de áreas)
            double kNucleo = ObtenerConstanteNucleo();
            double areaNucleo = kNucleo * Math.Sqrt(potenciaTotal);
            double areaVentana = areaNucleo * ObtenerRelacionVentana();
            double productoAreas = areaNucleo * areaVentana;

            DebugLogger.LogCalculation("Constante K núcleo", kNucleo);
            DebugLogger.LogCalculation("Área Núcleo (Ae)", areaNucleo, "cm²");
            DebugLogger.LogCalculation("Área Ventana (Aw)", areaVentana, "cm²");
            DebugLogger.LogCalculation("Producto Áreas (Ap)", productoAreas, "cm⁴");

            // Dimensiones físicas estimadas
            double ladoNucleo = Math.Sqrt(areaNucleo);
            double longitudMagnetica = CalcularLongitudMagnetica(areaNucleo);
            double volumenNucleo = areaNucleo * longitudMagnetica / 10; // cm³

            DebugLogger.LogCalculation("Lado Núcleo Estimado", ladoNucleo, "cm");
            DebugLogger.LogCalculation("Longitud Magnética", longitudMagnetica, "cm");
            DebugLogger.LogCalculation("Volumen Núcleo", volumenNucleo, "cm³");

            sb.AppendLine("DIMENSIONES DEL NÚCLEO:");
            sb.AppendLine($"  Tipo:            {ObtenerNombreNucleo()}");
            sb.AppendLine($"  Material:        {ObtenerNombreMaterial()}");
            sb.AppendLine($"  Área Núcleo Ae:  {areaNucleo:F2} cm²");
            sb.AppendLine($"  Área Ventana Aw: {areaVentana:F2} cm²");
            sb.AppendLine($"  Producto Áreas:  {productoAreas:F2} cm⁴");
            sb.AppendLine($"  Dimensión Aprox: {ladoNucleo:F2} × {ladoNucleo:F2} cm");
            sb.AppendLine($"  Long. Magnética: {longitudMagnetica:F2} cm");
            sb.AppendLine($"  Volumen Aprox:   {volumenNucleo:F2} cm³");
            sb.AppendLine();

            // PASO 3: Cálculo de espiras por voltio (Fórmula de Faraday corregida)
            DebugLogger.Log("CALC", "--- PASO 3: Cálculo de Espiras/Voltio ---");

            // Fórmula de Faraday: E = 4.44 * f * N * Bmax * Ae
            // Despejando N/V: N/V = E / (4.44 * f * Bmax * Ae)
            // donde:
            // - E: voltaje en voltios (para obtener N por cada voltio, E=1)
            // - f: frecuencia en Hz
            // - N: número de espiras
            // - Bmax: densidad de flujo en Tesla
            // - Ae: área del núcleo en m² (NO en cm²)

            // CRÍTICO: Convertir cm² a m²
            double aeMetrosCuadrados = areaNucleo * 1e-4; // cm² a m²

            // N/V = 1 / (4.44 * f * Bmax * Ae_m²)
            double espirasVolt = 1.0 / (4.44 * Frecuencia * DensidadFlujo * aeMetrosCuadrados);

            DebugLogger.LogCalculation("Área Núcleo", areaNucleo, "cm²");
            DebugLogger.LogCalculation("Área Núcleo", aeMetrosCuadrados, "m²");
            DebugLogger.LogCalculation("Frecuencia", Frecuencia, "Hz");
            DebugLogger.LogCalculation("Densidad Flujo", DensidadFlujo, "T");
            DebugLogger.Log("CALC", "Fórmula: N/V = 1 / (4.44 × {0} × {1} × {2})",
                Frecuencia, DensidadFlujo, aeMetrosCuadrados);
            DebugLogger.LogCalculation("Espiras/Voltio", espirasVolt, "N/V");

            // Verificación con ejemplo
            double voltajePrueba = 100;
            double espirasPrueba = voltajePrueba * espirasVolt;
            double voltajeRecalculado = espirasPrueba / espirasVolt;
            DebugLogger.Log("CALC", "Prueba: {0}V × {1:F4}N/V = {2:F0} espiras → {3:F2}V",
                voltajePrueba, espirasVolt, espirasPrueba, voltajeRecalculado);

            sb.AppendLine("DEVANADOS:");
            sb.AppendLine($"  Espiras/Volt:    {espirasVolt:F4} N/V");
            sb.AppendLine($"  Frecuencia:      {Frecuencia:F0} Hz");
            sb.AppendLine($"  Densidad Flujo:  {DensidadFlujo:F3} T");
            sb.AppendLine($"  Área Núcleo:     {areaNucleo:F2} cm² ({aeMetrosCuadrados:F6} m²)");
            sb.AppendLine($"  Fórmula:         N/V = 1/(4.44×f×B×Ae)");
            sb.AppendLine();

            // PASO 4: Cálculo del primario
            DebugLogger.Log("CALC", "--- PASO 4: Devanado Primario ---");

            double espirasPrimario = Math.Ceiling(VoltajeEntrada * espirasVolt);

            // VALIDACIÓN DE CORDURA - Las espiras típicas están entre 50 y 10000
            if (espirasPrimario < 10 || espirasPrimario > 50000)
            {
                DebugLogger.LogError($"ERROR CRÍTICO: Espiras primario fuera de rango razonable: {espirasPrimario}");
                DebugLogger.Log("CALC", "Valores: V={0}, N/V={1}, Ae={2}cm²={3}m², f={4}, B={5}",
                    VoltajeEntrada, espirasVolt, areaNucleo, aeMetrosCuadrados, Frecuencia, DensidadFlujo);
            }

            // Verificación del cálculo
            double voltajeVerificacion = espirasPrimario / espirasVolt;
            DebugLogger.Log("CALC", "PRIMARIO: {0}V × {1:F4}N/V = {2:F0} espiras",
                VoltajeEntrada, espirasVolt, espirasPrimario);
            DebugLogger.Log("CALC", "Verificación: {0:F0} espiras / {1:F4} N/V = {2:F2} V",
                espirasPrimario, espirasVolt, voltajeVerificacion);

            double corrientePrimario1 = potenciaSalida1 / (VoltajeEntrada * Eficiencia);
            double corrientePrimario2 = NumSalidas == 2 ? potenciaSalida2 / (VoltajeEntrada * Eficiencia) : 0;
            double corrientePrimarioTotal = corrientePrimario1 + corrientePrimario2;

            // Cálculo de sección de conductor considerando factor de llenado
            double seccionPrimarioCalculada = corrientePrimarioTotal / DensidadCorriente;
            int awgPrimario = CalcularAWG(seccionPrimarioCalculada);
            double seccionPrimarioReal = ObtenerSeccionAWG(awgPrimario);
            double diametroPrimario = ObtenerDiametroAWG(awgPrimario);

            // Verificación de área de ventana
            double areaCobrePrimario = espirasPrimario * seccionPrimarioReal;

            DebugLogger.LogCalculation("Espiras Primario", espirasPrimario);
            DebugLogger.LogCalculation("Corriente Primario", corrientePrimarioTotal, "A");
            DebugLogger.LogCalculation("Sección Calculada", seccionPrimarioCalculada, "mm²");
            DebugLogger.LogCalculation("AWG Seleccionado", awgPrimario);
            DebugLogger.LogCalculation("Sección Real AWG", seccionPrimarioReal, "mm²");
            DebugLogger.LogCalculation("Área Cobre Primario", areaCobrePrimario, "mm²");

            sb.AppendLine("PRIMARIO:");
            if (TipoEntrada == 2)
            {
                sb.AppendLine($"  Configuración:   {(NumSalidas == 2 ? "2 devanados independientes" : "1 devanado")}");
                sb.AppendLine($"  Espiras/dev:     {espirasPrimario:F0}");
                sb.AppendLine($"  Corriente/dev:   {(NumSalidas == 2 ? corrientePrimario1 : corrientePrimarioTotal):F3} A");
            }
            else
            {
                sb.AppendLine($"  Espiras:         {espirasPrimario:F0}");
                sb.AppendLine($"  Corriente:       {corrientePrimarioTotal:F3} A");
            }
            sb.AppendLine($"  Sección Calc.:   {seccionPrimarioCalculada:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgPrimario} ({seccionPrimarioReal:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {diametroPrimario:F3} mm");
            sb.AppendLine($"  Densidad Real:   {(corrientePrimarioTotal / seccionPrimarioReal):F2} A/mm²");
            sb.AppendLine($"  Área Cobre:      {areaCobrePrimario:F2} mm²");
            sb.AppendLine();

            // PASO 5: Cálculo del secundario 1
            DebugLogger.Log("CALC", "--- PASO 5: Devanado Secundario 1 ---");

            double espirasSecundario1 = Math.Ceiling(VoltajeSalida1 * espirasVolt * 1.03); // +3% para compensar caída
            double seccionSecundario1Calculada = CorrienteSalida1 / DensidadCorriente;
            int awgSecundario1 = CalcularAWG(seccionSecundario1Calculada);
            double seccionSecundario1Real = ObtenerSeccionAWG(awgSecundario1);
            double diametroSecundario1 = ObtenerDiametroAWG(awgSecundario1);
            double areaCobreSecundario1 = espirasSecundario1 * seccionSecundario1Real;

            DebugLogger.LogCalculation("Espiras Secundario 1", espirasSecundario1);
            DebugLogger.LogCalculation("Corriente Secundario 1", CorrienteSalida1, "A");
            DebugLogger.LogCalculation("Sección Calculada S1", seccionSecundario1Calculada, "mm²");
            DebugLogger.LogCalculation("AWG S1", awgSecundario1);
            DebugLogger.LogCalculation("Área Cobre S1", areaCobreSecundario1, "mm²");

            sb.AppendLine("SECUNDARIO 1:");
            sb.AppendLine($"  Voltaje:         {VoltajeSalida1:F1} V");
            sb.AppendLine($"  Corriente:       {CorrienteSalida1:F2} A");
            sb.AppendLine($"  Espiras:         {espirasSecundario1:F0} (+3% compensación)");
            sb.AppendLine($"  Sección Calc.:   {seccionSecundario1Calculada:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgSecundario1} ({seccionSecundario1Real:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {diametroSecundario1:F3} mm");
            sb.AppendLine($"  Densidad Real:   {(CorrienteSalida1 / seccionSecundario1Real):F2} A/mm²");
            sb.AppendLine($"  Área Cobre:      {areaCobreSecundario1:F2} mm²");
            sb.AppendLine();

            // PASO 6: Cálculo del secundario 2 (si existe)
            double espirasSecundario2 = 0;
            double seccionSecundario2Real = 0;
            double areaCobreSecundario2 = 0;
            int awgSecundario2 = 0;
            double diametroSecundario2 = 0;

            if (NumSalidas == 2)
            {
                DebugLogger.Log("CALC", "--- PASO 6: Devanado Secundario 2 ---");

                espirasSecundario2 = Math.Ceiling(VoltajeSalida2 * espirasVolt * 1.03);
                double seccionSecundario2Calculada = CorrienteSalida2 / DensidadCorriente;
                awgSecundario2 = CalcularAWG(seccionSecundario2Calculada);
                seccionSecundario2Real = ObtenerSeccionAWG(awgSecundario2);
                diametroSecundario2 = ObtenerDiametroAWG(awgSecundario2);
                areaCobreSecundario2 = espirasSecundario2 * seccionSecundario2Real;

                DebugLogger.LogCalculation("Espiras Secundario 2", espirasSecundario2);
                DebugLogger.LogCalculation("Corriente Secundario 2", CorrienteSalida2, "A");
                DebugLogger.LogCalculation("Sección Calculada S2", seccionSecundario2Calculada, "mm²");
                DebugLogger.LogCalculation("Área Cobre S2", areaCobreSecundario2, "mm²");

                sb.AppendLine("SECUNDARIO 2:");
                sb.AppendLine($"  Voltaje:         {VoltajeSalida2:F1} V");
                sb.AppendLine($"  Corriente:       {CorrienteSalida2:F2} A");
                sb.AppendLine($"  Espiras:         {espirasSecundario2:F0} (+3% compensación)");
                sb.AppendLine($"  Sección Calc.:   {seccionSecundario2Calculada:F3} mm²");
                sb.AppendLine($"  Calibre:         AWG {awgSecundario2} ({seccionSecundario2Real:F3} mm²)");
                sb.AppendLine($"  Diámetro:        {diametroSecundario2:F3} mm");
                sb.AppendLine($"  Densidad Real:   {(CorrienteSalida2 / seccionSecundario2Real):F2} A/mm²");
                sb.AppendLine($"  Área Cobre:      {areaCobreSecundario2:F2} mm²");
                sb.AppendLine();
            }

            // PASO 7: Verificación del factor de llenado
            DebugLogger.Log("CALC", "--- PASO 7: Verificación Factor de Llenado ---");

            double areaTotalCobre = areaCobrePrimario + areaCobreSecundario1 + areaCobreSecundario2;
            double areaVentanaMM2 = areaVentana * 100; // cm² a mm²
            double factorLlenadoReal = (areaTotalCobre / areaVentanaMM2) * 100;
            bool ventanaAdecuada = factorLlenadoReal <= (FactorLlenado * 100);

            DebugLogger.LogCalculation("Área Total Cobre", areaTotalCobre, "mm²");
            DebugLogger.LogCalculation("Área Ventana", areaVentanaMM2, "mm²");
            DebugLogger.LogCalculation("Factor Llenado Real", factorLlenadoReal, "%");
            DebugLogger.Log("CALC", "Ventana {0}", ventanaAdecuada ? "ADECUADA" : "INSUFICIENTE");

            sb.AppendLine("VERIFICACIÓN DE VENTANA:");
            sb.AppendLine($"  Área Cobre Prim: {areaCobrePrimario:F2} mm²");
            sb.AppendLine($"  Área Cobre Sec1: {areaCobreSecundario1:F2} mm²");
            if (NumSalidas == 2)
                sb.AppendLine($"  Área Cobre Sec2: {areaCobreSecundario2:F2} mm²");
            sb.AppendLine($"  Total Cobre:     {areaTotalCobre:F2} mm²");
            sb.AppendLine($"  Ventana Disp.:   {areaVentanaMM2:F2} mm²");
            sb.AppendLine($"  Factor Llenado:  {factorLlenadoReal:F1}% (objetivo: {FactorLlenado * 100:F0}%)");
            sb.AppendLine($"  Estado:          {(ventanaAdecuada ? "✓ ADECUADA" : "✗ INSUFICIENTE - Aumentar núcleo")}");
            sb.AppendLine();

            // PASO 8: Cálculo de protecciones
            DebugLogger.Log("CALC", "--- PASO 8: Dimensionamiento de Protecciones ---");

            double fusiblePrimario = (NumSalidas == 2 ? corrientePrimario1 : corrientePrimarioTotal) * 1.25;
            double fusibleSec1 = CorrienteSalida1 * 1.25;
            double fusibleSec2 = NumSalidas == 2 ? CorrienteSalida2 * 1.25 : 0;

            DebugLogger.LogCalculation("Fusible Primario", fusiblePrimario, "A");
            DebugLogger.LogCalculation("Fusible Sec 1", fusibleSec1, "A");
            if (NumSalidas == 2)
                DebugLogger.LogCalculation("Fusible Sec 2", fusibleSec2, "A");

            sb.AppendLine("PROTECCIONES RECOMENDADAS (125%):");
            sb.AppendLine($"  Primario:        {Math.Ceiling(fusiblePrimario):F0} A");
            sb.AppendLine($"  Secundario 1:    {Math.Ceiling(fusibleSec1):F0} A");
            if (NumSalidas == 2)
                sb.AppendLine($"  Secundario 2:    {Math.Ceiling(fusibleSec2):F0} A");
            sb.AppendLine();

            // PASO 9: Cálculo de pérdidas y análisis térmico mejorado
            DebugLogger.Log("CALC", "--- PASO 9: Análisis de Pérdidas y Térmica ---");

            double longMediaPrimario = CalcularLongitudMediaBobinado(areaNucleo, 1);
            double longMediaSecundario1 = CalcularLongitudMediaBobinado(areaNucleo, 2);
            double longMediaSecundario2 = NumSalidas == 2 ? CalcularLongitudMediaBobinado(areaNucleo, 3) : 0;

            DebugLogger.LogCalculation("Long. Media Primario", longMediaPrimario, "cm");
            DebugLogger.LogCalculation("Long. Media Sec1", longMediaSecundario1, "cm");

            // Pérdidas en el cobre con temperatura
            double tempCobre = TempAmbiente + 40; // Estimación inicial
            double resistividadCu = RESISTIVIDAD_COBRE_20C * (1 + COEF_TEMP_COBRE * (tempCobre - 20));

            DebugLogger.LogCalculation("Temp. Cobre Estimada", tempCobre, "°C");
            DebugLogger.LogCalculation("Resistividad Cu", resistividadCu * 1e8, "Ω·cm");

            double perdidaCobrePrimario = CalcularPerdidaCobre(espirasPrimario, corrientePrimarioTotal,
                seccionPrimarioReal, longMediaPrimario, resistividadCu);
            double perdidaCobreSecundario1 = CalcularPerdidaCobre(espirasSecundario1, CorrienteSalida1,
                seccionSecundario1Real, longMediaSecundario1, resistividadCu);
            double perdidaCobreSecundario2 = NumSalidas == 2 ?
                CalcularPerdidaCobre(espirasSecundario2, CorrienteSalida2, seccionSecundario2Real,
                longMediaSecundario2, resistividadCu) : 0;

            double perdidaCobreTotal = perdidaCobrePrimario + perdidaCobreSecundario1 + perdidaCobreSecundario2;

            DebugLogger.LogCalculation("Pérdida Cu Primario", perdidaCobrePrimario, "W");
            DebugLogger.LogCalculation("Pérdida Cu Sec1", perdidaCobreSecundario1, "W");
            if (NumSalidas == 2)
                DebugLogger.LogCalculation("Pérdida Cu Sec2", perdidaCobreSecundario2, "W");
            DebugLogger.LogCalculation("Pérdida Cu Total", perdidaCobreTotal, "W");

            // Pérdidas en el núcleo (histéresis + corrientes parásitas)
            double perdidaNucleo = CalcularPerdidaNucleo(volumenNucleo, DensidadFlujo, Frecuencia);

            DebugLogger.LogCalculation("Pérdida Núcleo", perdidaNucleo, "W");

            double perdidaTotal = perdidaCobreTotal + perdidaNucleo;
            double eficienciaReal = (potenciaTotal / (potenciaTotal + perdidaTotal)) * 100;

            DebugLogger.LogCalculation("Pérdida Total", perdidaTotal, "W");
            DebugLogger.LogCalculation("Eficiencia Real", eficienciaReal, "%");

            // Cálculo térmico mejorado
            double superficieRadiacion = CalcularSuperficieRadiacion(areaNucleo);
            double elevacionTemperatura = CalcularElevacionTemperatura(perdidaTotal, superficieRadiacion);
            double tempMaximaOperacion = TempAmbiente + elevacionTemperatura;

            DebugLogger.LogCalculation("Superficie Radiación", superficieRadiacion, "cm²");
            DebugLogger.LogCalculation("Elevación Temperatura", elevacionTemperatura, "°C");
            DebugLogger.LogCalculation("Temp. Máx. Operación", tempMaximaOperacion, "°C");

            sb.AppendLine("ANÁLISIS DE PÉRDIDAS:");
            sb.AppendLine($"  Pérd. Cu Prim:   {perdidaCobrePrimario:F2} W");
            sb.AppendLine($"  Pérd. Cu Sec1:   {perdidaCobreSecundario1:F2} W");
            if (NumSalidas == 2)
                sb.AppendLine($"  Pérd. Cu Sec2:   {perdidaCobreSecundario2:F2} W");
            sb.AppendLine($"  Total Cobre:     {perdidaCobreTotal:F2} W");
            sb.AppendLine($"  Pérdida Núcleo:  {perdidaNucleo:F2} W");
            sb.AppendLine($"  Pérdida Total:   {perdidaTotal:F2} W ({(perdidaTotal / potenciaTotal * 100):F2}%)");
            sb.AppendLine($"  Eficiencia Real: {eficienciaReal:F2}%");
            sb.AppendLine();

            sb.AppendLine("ANÁLISIS TÉRMICO:");
            sb.AppendLine($"  Temp. Ambiente:  {TempAmbiente:F0} °C");
            sb.AppendLine($"  Elev. Temp Est:  {elevacionTemperatura:F1} °C");
            sb.AppendLine($"  Temp. Operación: {tempMaximaOperacion:F1} °C");
            sb.AppendLine($"  Superficie Rad.: {superficieRadiacion:F0} cm²");
            sb.AppendLine($"  Ventilación:     {ObtenerTipoVentilacion(tempMaximaOperacion)}");
            sb.AppendLine($"  Clase Aislam.:   {ObtenerClaseAislamiento(tempMaximaOperacion)}");
            sb.AppendLine();

            // PASO 10: Regulación de voltaje
            DebugLogger.Log("CALC", "--- PASO 10: Regulación de Voltaje ---");

            double regulacionSec1 = CalcularRegulacion(espirasPrimario, espirasSecundario1,
                seccionPrimarioReal, seccionSecundario1Real, longMediaPrimario,
                longMediaSecundario1, corrientePrimarioTotal, CorrienteSalida1, resistividadCu);

            DebugLogger.LogCalculation("Regulación Sec1", regulacionSec1, "%");

            sb.AppendLine("REGULACIÓN DE VOLTAJE:");
            sb.AppendLine($"  Secundario 1:    {regulacionSec1:F2}% (@ carga nominal)");
            if (NumSalidas == 2)
            {
                double regulacionSec2 = CalcularRegulacion(espirasPrimario, espirasSecundario2,
                    seccionPrimarioReal, seccionSecundario2Real, longMediaPrimario,
                    longMediaSecundario2, corrientePrimarioTotal, CorrienteSalida2, resistividadCu);
                sb.AppendLine($"  Secundario 2:    {regulacionSec2:F2}% (@ carga nominal)");
                DebugLogger.LogCalculation("Regulación Sec2", regulacionSec2, "%");
            }
            sb.AppendLine();

            // PASO 11: Recomendaciones finales
            DebugLogger.Log("CALC", "--- PASO 11: Recomendaciones ---");

            sb.AppendLine("RECOMENDACIONES DE CONSTRUCCIÓN:");
            sb.AppendLine($"  • Número de capas primario: {Math.Ceiling(espirasPrimario / (areaVentana * 10 / diametroPrimario)):F0}");
            sb.AppendLine($"  • Aislamiento entre capas: {ObtenerAislamientoRecomendado(VoltajeEntrada)}");
            sb.AppendLine($"  • Aislamiento prim-sec: {ObtenerAislamientoPrimSec(VoltajeEntrada, VoltajeSalida1)}");
            sb.AppendLine($"  • Material aislante: {ObtenerMaterialAislante(tempMaximaOperacion)}");
            if (!ventanaAdecuada)
                sb.AppendLine($"  • ⚠ ADVERTENCIA: Aumentar núcleo ~{Math.Ceiling((factorLlenadoReal / (FactorLlenado * 100) - 1) * 100)}% o reducir densidad de corriente");
            if (tempMaximaOperacion > 100)
                sb.AppendLine($"  • ⚠ ADVERTENCIA: Temperatura elevada - Considerar ventilación forzada");
            sb.AppendLine();

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine($"Cálculo realizado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        private double ObtenerConstanteNucleo()
        {
            // Constante K ajustada según tipo de núcleo y frecuencia
            double kBase = TipoNucleo switch
            {
                0 => 0.64,  // E-I Laminado
                1 => 0.58,  // Toroidal (más eficiente)
                2 => 0.60,  // C-Core
                3 => 0.62,  // UI Laminado
                _ => 0.64
            };

            // Ajuste por frecuencia
            double factorFrecuencia = Math.Sqrt(60.0 / Frecuencia);

            return kBase * factorFrecuencia;
        }

        private double CalcularLongitudMagnetica(double areaCm2)
        {
            // Longitud magnética según geometría del núcleo
            double lado = Math.Sqrt(areaCm2);

            return TipoNucleo switch
            {
                0 => lado * 3.6,    // E-I: perímetro magnético típico
                1 => lado * 2.8,    // Toroidal: más corto
                2 => lado * 3.2,    // C-Core
                3 => lado * 3.5,    // UI
                _ => lado * 3.6
            };
        }

        private double CalcularLongitudMediaBobinado(double areaCm2, int capa)
        {
            // Longitud media de una espira según la capa del bobinado
            double radioInterior = Math.Sqrt(areaCm2 / Math.PI);
            double incrementoPorCapa = 0.3; // cm por capa
            double radioEfectivo = radioInterior + (capa * incrementoPorCapa);

            return 2 * Math.PI * radioEfectivo;
        }

        private double ObtenerRelacionVentana()
        {
            return TipoNucleo switch
            {
                0 => 1.8,   // E-I
                1 => 1.2,   // Toroidal (ventana más pequeña)
                2 => 1.5,   // C-Core
                3 => 1.7,   // UI
                _ => 1.8
            };
        }

        private string ObtenerNombreNucleo()
        {
            return TipoNucleo switch
            {
                0 => "E-I Laminado",
                1 => "Toroidal",
                2 => "C-Core",
                3 => "UI Laminado",
                _ => "Desconocido"
            };
        }

        private string ObtenerNombreMaterial()
        {
            return MaterialNucleo switch
            {
                0 => "Acero Silicio M19 (0.014\")",
                1 => "Acero Silicio M27 (0.011\")",
                2 => "Acero Silicio Grano Orientado",
                3 => "Ferrita MnZn",
                _ => "Desconocido"
            };
        }

        private int CalcularAWG(double seccionMM2)
        {
            // Tabla AWG con secciones transversales precisas
            var awgTable = new (int awg, double mm2)[]
            {
                (0, 53.49), (1, 42.41), (2, 33.62), (3, 26.67), (4, 21.15),
                (5, 16.77), (6, 13.30), (7, 10.55), (8, 8.37), (9, 6.63),
                (10, 5.26), (11, 4.17), (12, 3.31), (13, 2.62), (14, 2.08),
                (15, 1.65), (16, 1.31), (17, 1.04), (18, 0.823), (19, 0.653),
                (20, 0.518), (21, 0.410), (22, 0.326), (23, 0.258), (24, 0.205),
                (25, 0.162), (26, 0.129), (27, 0.102), (28, 0.081), (29, 0.0642),
                (30, 0.0509)
            };

            // Buscar el AWG más cercano que cumpla con la sección requerida
            for (int i = 0; i < awgTable.Length; i++)
            {
                if (seccionMM2 >= awgTable[i].mm2)
                {
                    DebugLogger.Log("AWG", "Sección {0:F3} mm² -> AWG {1} ({2:F3} mm²)",
                        seccionMM2, awgTable[i].awg, awgTable[i].mm2);
                    return awgTable[i].awg;
                }
            }

            DebugLogger.Log("AWG", "Sección {0:F3} mm² -> AWG 30 (mínimo)", seccionMM2);
            return 30;
        }

        private double ObtenerSeccionAWG(int awg)
        {
            var secciones = new Dictionary<int, double>
            {
                {0, 53.49}, {1, 42.41}, {2, 33.62}, {3, 26.67}, {4, 21.15},
                {5, 16.77}, {6, 13.30}, {7, 10.55}, {8, 8.37}, {9, 6.63},
                {10, 5.26}, {11, 4.17}, {12, 3.31}, {13, 2.62}, {14, 2.08},
                {15, 1.65}, {16, 1.31}, {17, 1.04}, {18, 0.823}, {19, 0.653},
                {20, 0.518}, {21, 0.410}, {22, 0.326}, {23, 0.258}, {24, 0.205},
                {25, 0.162}, {26, 0.129}, {27, 0.102}, {28, 0.081}, {29, 0.0642},
                {30, 0.0509}
            };

            return secciones.ContainsKey(awg) ? secciones[awg] : 1.0;
        }

        private double ObtenerDiametroAWG(int awg)
        {
            // Diámetro en mm (incluyendo aislamiento esmaltado típico)
            var diametros = new Dictionary<int, double>
            {
                {0, 8.25}, {1, 7.35}, {2, 6.54}, {3, 5.83}, {4, 5.19},
                {5, 4.62}, {6, 4.11}, {7, 3.67}, {8, 3.26}, {9, 2.91},
                {10, 2.59}, {11, 2.30}, {12, 2.05}, {13, 1.83}, {14, 1.63},
                {15, 1.45}, {16, 1.29}, {17, 1.15}, {18, 1.02}, {19, 0.912},
                {20, 0.812}, {21, 0.723}, {22, 0.644}, {23, 0.573}, {24, 0.511},
                {25, 0.455}, {26, 0.405}, {27, 0.361}, {28, 0.321}, {29, 0.286},
                {30, 0.255}
            };

            return diametros.ContainsKey(awg) ? diametros[awg] : 1.0;
        }

        private double CalcularPerdidaCobre(double espiras, double corriente, double seccionMM2,
            double longitudMediaCm, double resistividad)
        {
            // Longitud total del conductor en metros
            double longitudTotalM = (espiras * longitudMediaCm) / 100.0;

            // Resistencia del devanado en Ohmios
            // R = ρ * L / A (donde A está en m²)
            double seccionM2 = seccionMM2 * 1e-6;
            double resistencia = resistividad * longitudTotalM / seccionM2;

            // Pérdida por efecto Joule: P = I² * R
            double perdida = corriente * corriente * resistencia;

            DebugLogger.Log("PERDIDA_CU", "N={0}, I={1:F2}A, S={2:F3}mm², Lm={3:F2}cm, Lt={4:F2}m, R={5:F4}Ω, P={6:F2}W",
                espiras, corriente, seccionMM2, longitudMediaCm, longitudTotalM, resistencia, perdida);

            return perdida;
        }

        private double CalcularPerdidaNucleo(double volumenCm3, double densidadFlujotesla, double frecuenciaHz)
        {
            // Pérdidas por histéresis y corrientes parásitas
            // P = Ph + Pe
            // Ph = Kh * f * B^n * Vol (histéresis, n ≈ 1.6-2.0)
            // Pe = Ke * f² * B² * t² * Vol (parásitas)

            double kh = 0, ke = 0, espesor = 0, exponente = 1.6;

            switch (MaterialNucleo)
            {
                case 0: // M19
                    kh = 0.0045; ke = 0.0008; espesor = 0.014 * 2.54; exponente = 1.7;
                    break;
                case 1: // M27
                    kh = 0.0055; ke = 0.0010; espesor = 0.011 * 2.54; exponente = 1.7;
                    break;
                case 2: // Grano orientado
                    kh = 0.0030; ke = 0.0005; espesor = 0.012 * 2.54; exponente = 1.6;
                    break;
                case 3: // Ferrita
                    kh = 0.010; ke = 0.0003; espesor = 0; exponente = 2.0;
                    break;
            }

            // Pérdida por histéresis
            double perdidaHisteresis = kh * frecuenciaHz * Math.Pow(densidadFlujotesla, exponente) * volumenCm3;

            // Pérdida por corrientes parásitas (solo en laminados)
            double perdidaParasitas = ke * frecuenciaHz * frecuenciaHz * densidadFlujotesla * densidadFlujotesla
                * espesor * espesor * volumenCm3;

            double perdidaTotal = perdidaHisteresis + perdidaParasitas;

            DebugLogger.Log("PERDIDA_FE", "Vol={0:F2}cm³, B={1:F3}T, f={2}Hz, Ph={3:F2}W, Pe={4:F2}W, Total={5:F2}W",
                volumenCm3, densidadFlujotesla, frecuenciaHz, perdidaHisteresis, perdidaParasitas, perdidaTotal);

            return perdidaTotal;
        }

        private double CalcularSuperficieRadiacion(double areaNucleoCm2)
        {
            // Estimación de superficie de radiación según geometría
            double lado = Math.Sqrt(areaNucleoCm2);

            double superficie = TipoNucleo switch
            {
                0 => lado * lado * 6 * 1.2,  // E-I: 6 caras + bobinados
                1 => Math.PI * lado * lado * 3,  // Toroidal
                2 => lado * lado * 5 * 1.3,  // C-Core
                3 => lado * lado * 6 * 1.15, // UI
                _ => lado * lado * 6
            };

            return superficie;
        }

        private double CalcularElevacionTemperatura(double perdidaW, double superficieCm2)
        {
            // Modelo térmico simplificado
            // ΔT = P / (h * A)
            // donde h es el coeficiente de transferencia térmica (W/cm²·°C)

            // Coeficiente de convección natural: ~0.0008 W/cm²·°C
            double coeficienteConveccion = 0.0008;

            // Elevación de temperatura
            double deltaT = perdidaW / (coeficienteConveccion * superficieCm2);

            DebugLogger.Log("TERMICA", "P={0:F2}W, A={1:F0}cm², h={2:F6}, ΔT={3:F1}°C",
                perdidaW, superficieCm2, coeficienteConveccion, deltaT);

            return deltaT;
        }

        private double CalcularRegulacion(double espirasPrim, double espirasSecundario,
            double seccionPrim, double seccionSec, double longMediaPrim, double longMediaSec,
            double corrientePrim, double corrienteSec, double resistividad)
        {
            // Regulación = (Vnl - Vfl) / Vfl * 100%
            // donde Vnl = voltaje sin carga, Vfl = voltaje con carga

            // Resistencia del primario referida al secundario
            double resistenciaPrim = resistividad * (espirasPrim * longMediaPrim / 100) / (seccionPrim * 1e-6);
            double resistenciaSec = resistividad * (espirasSecundario * longMediaSec / 100) / (seccionSec * 1e-6);

            double relacionTransformacion = espirasSecundario / espirasPrim;
            double resistenciaEquivalente = resistenciaPrim * relacionTransformacion * relacionTransformacion + resistenciaSec;

            // Caída de voltaje
            double caidaVoltaje = corrienteSec * resistenciaEquivalente;

            // Voltaje del secundario
            double voltajeSecundario = espirasSecundario * (100.0 / espirasPrim) * 10; // Estimación

            double regulacion = (caidaVoltaje / voltajeSecundario) * 100;

            DebugLogger.Log("REGULACION", "Req={0:F4}Ω, ΔV={1:F2}V, Reg={2:F2}%",
                resistenciaEquivalente, caidaVoltaje, regulacion);

            return regulacion;
        }

        private string ObtenerClaseAislamiento(double tempOperacion)
        {
            if (tempOperacion < 105) return "A (105°C) - Papel, algodón";
            if (tempOperacion < 130) return "B (130°C) - Mica, fibra vidrio";
            if (tempOperacion < 155) return "F (155°C) - Mica, asbesto, fibra vidrio";
            if (tempOperacion < 180) return "H (180°C) - Silicona, mica";
            return "C (>180°C) - Cerámica, mica, cuarzo";
        }

        private string ObtenerTipoVentilacion(double tempOperacion)
        {
            if (tempOperacion < 60) return "Natural (sin ventilador)";
            if (tempOperacion < 80) return "Natural mejorada (aletas)";
            if (tempOperacion < 100) return "Forzada (ventilador)";
            return "Forzada + Refrigeración activa";
        }

        private string ObtenerAislamientoRecomendado(double voltaje)
        {
            if (voltaje < 250) return "Papel kraft 0.05mm o barniz aislante";
            if (voltaje < 600) return "Papel kraft 0.08mm + barniz";
            if (voltaje < 1000) return "Presspahn 0.3mm";
            return "Presspahn 0.5mm + múltiples capas";
        }

        private string ObtenerAislamientoPrimSec(double voltajePrim, double voltajeSec)
        {
            double voltajeMax = Math.Max(voltajePrim, voltajeSec);

            if (voltajeMax < 250) return "3 capas papel kraft + cinta aislante";
            if (voltajeMax < 600) return "Presspahn 0.5mm + papel kraft";
            if (voltajeMax < 1000) return "Presspahn 1.0mm + cinta Nomex";
            return "Presspahn 1.5mm + múltiples barreras + barniz";
        }

        private string ObtenerMaterialAislante(double temperatura)
        {
            if (temperatura < 105) return "Barniz acrílico, papel kraft";
            if (temperatura < 130) return "Barniz poliéster, papel Nomex";
            if (temperatura < 155) return "Barniz poliamida-imida, fibra vidrio";
            return "Barniz silicona, mica, Kapton";
        }
    }

    public static class DebugLogger
    {
        private static string logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"TransformerCalc_Log_{DateTime.Now:yyyyMMdd}.txt"
        );

        public static void Log(string categoria, string mensaje, params object[] args)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] [{categoria}] {string.Format(mensaje, args)}";
            Debug.WriteLine(logEntry);

            try
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch { /* Evitar errores en logging */ }
        }

        public static void LogCalculation(string nombre, double valor, string unidad = "")
        {
            Log("CALC", $"{nombre} = {valor:F6} {unidad}");
        }

        public static void LogError(string mensaje, Exception ex = null)
        {
            Log("ERROR", mensaje);
            if (ex != null)
                Log("ERROR", $"Excepción: {ex.Message}\n{ex.StackTrace}");
        }

        public static string GetLogPath() => logFilePath;
    }
}