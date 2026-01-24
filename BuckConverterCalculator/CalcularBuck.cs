using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BuckConverterCalculator.Core;
using BuckConverterCalculator.Components;
using BuckConverterCalculator.SchematicEditor;
using BuckConverterCalculator.UI.Dialogs;
using BuckConverterCalculator.Database;

namespace BuckConverterCalculator
{
    public partial class CalcularBuck : Form
    {
        private BuckCalculator calculator;
        private ComponentSelector componentSelector;

        private DesignParameters lastCalculatedParameters;
        private CalculationResults lastCalculatedResults;

        public CalcularBuck()
        {
            InitializeComponent();

            calculator = new BuckCalculator();
            componentSelector = new ComponentSelector();

            // Configurar colores y estilos
            ConfigureAppearance();
        }

        private void ConfigureAppearance()
        {
            // Configurar colores de los TextBoxes
            txtResults.BackColor = Color.FromArgb(245, 245, 245);
            txtComponents.BackColor = Color.FromArgb(245, 255, 245);
            txtLosses.BackColor = Color.FromArgb(255, 250, 240);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar entradas
                if (!ValidateInputs())
                {
                    return;
                }

                // Recolectar parámetros
                var parameters = new DesignParameters
                {
                    Vin = (double)numVin.Value,
                    Vout = (double)numVout.Value,
                    Iout = (double)numIout.Value,
                    Frequency = (double)numFreq.Value,
                    RippleCurrentPercent = (double)numRippleCurrent.Value,
                    RippleVoltageMv = (double)numRippleVoltage.Value,
                    EfficiencyPercent = (double)numEfficiency.Value
                };

                // Calcular diseño
                var results = calculator.Calculate(parameters);

                // Mostrar resultados
                DisplayResults(results);
                DisplayComponents(results, parameters);
                DisplayLosses(results);

                // Actualizar status
                toolStripStatusLabel.Text = $"Cálculo completado exitosamente - {DateTime.Now:HH:mm:ss}";
                toolStripStatusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al realizar cálculos:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                toolStripStatusLabel.Text = "Error en cálculo";
                toolStripStatusLabel.ForeColor = Color.Red;
            }
        }

        private bool ValidateInputs()
        {
            if (numVout.Value >= numVin.Value)
            {
                MessageBox.Show(
                    "La tensión de salida debe ser menor que la tensión de entrada\n" +
                    "para un convertidor Buck (step-down).",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (numIout.Value <= 0)
            {
                MessageBox.Show(
                    "La corriente de salida debe ser mayor que cero.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void DisplayResults(CalculationResults results)
        {
            txtResults.Clear();

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine("    RESULTADOS DE DISEÑO BUCK CONVERTER");
            sb.AppendLine("═══════════════════════════════════════\n");

            sb.AppendLine("▌ PARÁMETROS FUNDAMENTALES");
            sb.AppendLine("─────────────────────────────────────");
            sb.AppendLine($"  Duty Cycle (D):        {results.DutyCycle:F3} ({results.DutyCycle * 100:F1}%)");
            sb.AppendLine($"  Potencia Salida:       {results.PowerOutput:F2} W");
            sb.AppendLine($"  Potencia Entrada:      {results.PowerInput:F2} W");
            sb.AppendLine($"  Eficiencia Real:       {results.ActualEfficiency:F2} %\n");

            sb.AppendLine("▌ COMPONENTES MAGNÉTICOS");
            sb.AppendLine("─────────────────────────────────────");
            sb.AppendLine($"  Inductancia Calculada: {results.Inductance * 1e6:F2} µH");
            sb.AppendLine($"  Inductancia Comercial: {results.InductanceCommercial * 1e6:F0} µH");
            sb.AppendLine($"  Ripple Corriente ΔIL:  {results.RippleCurrent:F3} A");
            sb.AppendLine($"  Corriente Pico IL:     {results.PeakInductorCurrent:F3} A");
            sb.AppendLine($"  Corriente RMS IL:      {results.RmsInductorCurrent:F3} A\n");

            sb.AppendLine("▌ CAPACITORES");
            sb.AppendLine("─────────────────────────────────────");
            sb.AppendLine($"  Capacitancia Calculada:{results.OutputCapacitance * 1e6:F2} µF");
            sb.AppendLine($"  Cap. Comercial:        {results.OutputCapacitanceCommercial * 1e6:F0} µF");
            sb.AppendLine($"  ESR Máximo:            {results.MaxEsr * 1000:F2} mΩ");
            sb.AppendLine($"  Corriente RMS Cap:     {results.RmsCapacitorCurrent:F3} A\n");

            sb.AppendLine("▌ DIVISOR DE REALIMENTACIÓN");
            sb.AppendLine("─────────────────────────────────────");
            sb.AppendLine($"  Resistor Superior R1:  {FormatResistor(results.FeedbackR1)}");
            sb.AppendLine($"  Resistor Inferior R2:  {FormatResistor(results.FeedbackR2)}");
            sb.AppendLine($"  Vout Verificado:       {results.VoutVerified:F3} V\n");

            sb.AppendLine("▌ CONFIGURACIÓN PWM (UC3843)");
            sb.AppendLine("─────────────────────────────────────");
            sb.AppendLine($"  Resistor Rt:           {FormatResistor(results.RtValue)}");
            sb.AppendLine($"  Capacitor Ct:          {FormatCapacitor(results.CtValue)}");
            sb.AppendLine($"  Frecuencia Real:       {results.ActualFrequency / 1000:F2} kHz\n");

            txtResults.Text = sb.ToString();

            // Colorear secciones importantes
            ColorizeResults();
        }

        private void DisplayComponents(CalculationResults results, DesignParameters parameters)
        {
            txtComponents.Clear();

            var components = componentSelector.SelectComponents(results, parameters);

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════");
            sb.AppendLine("       SELECCIÓN DE COMPONENTES RECOMENDADOS");
            sb.AppendLine("═══════════════════════════════════════════════════════\n");

            sb.AppendLine("▌ MOSFET (Transistor de Potencia)");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Part Number:     {components.Mosfet.PartNumber}");
            sb.AppendLine($"  Vds (máximo):    {components.Mosfet.Vds} V");
            sb.AppendLine($"  Id (continua):   {components.Mosfet.Id} A");
            sb.AppendLine($"  Rds(on):         {components.Mosfet.RdsOn * 1000:F1} mΩ");
            sb.AppendLine($"  Package:         {components.Mosfet.Package}");
            sb.AppendLine($"  Supplier:        {components.Mosfet.Supplier}");
            sb.AppendLine($"  ⚠ Disipador:     {(components.Mosfet.RequiresHeatsink ? "REQUERIDO" : "Opcional")}\n");

            sb.AppendLine("▌ DIODO SCHOTTKY");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Part Number:     {components.Diode.PartNumber}");
            sb.AppendLine($"  Vr (reverse):    {components.Diode.Vr} V");
            sb.AppendLine($"  If (forward):    {components.Diode.If} A");
            sb.AppendLine($"  Vf (forward):    {components.Diode.Vf:F2} V @ {components.Diode.IfTest}A");
            sb.AppendLine($"  Package:         {components.Diode.Package}");
            sb.AppendLine($"  Supplier:        {components.Diode.Supplier}");
            sb.AppendLine($"  ⚠ Disipador:     {(components.Diode.RequiresHeatsink ? "REQUERIDO" : "Opcional")}\n");

            sb.AppendLine("▌ INDUCTOR DE POTENCIA");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Part Number:     {components.Inductor.PartNumber}");
            sb.AppendLine($"  Inductancia:     {components.Inductor.Inductance * 1e6:F0} µH");
            sb.AppendLine($"  Corriente Sat:   {components.Inductor.SaturationCurrent} A");
            sb.AppendLine($"  Corriente RMS:   {components.Inductor.RmsCurrent} A");
            sb.AppendLine($"  DCR:             {components.Inductor.Dcr * 1000:F1} mΩ");
            sb.AppendLine($"  Package:         {components.Inductor.Package}");
            sb.AppendLine($"  Supplier:        {components.Inductor.Supplier}\n");

            sb.AppendLine("▌ CAPACITOR DE ENTRADA");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Part Number:     {components.InputCapacitor.PartNumber}");
            sb.AppendLine($"  Capacitancia:    {components.InputCapacitor.Capacitance * 1e6:F0} µF");
            sb.AppendLine($"  Tensión:         {components.InputCapacitor.Voltage} V");
            sb.AppendLine($"  Tipo:            {components.InputCapacitor.Type}");
            sb.AppendLine($"  ESR:             {components.InputCapacitor.Esr * 1000:F0} mΩ");
            sb.AppendLine($"  Ripple Current:  {components.InputCapacitor.RippleCurrent:F2} A");
            sb.AppendLine($"  Package:         {components.InputCapacitor.Package}");
            sb.AppendLine($"  Supplier:        {components.InputCapacitor.Supplier}\n");

            sb.AppendLine("▌ CAPACITORES DE SALIDA");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Cantidad:        {components.OutputCapacitorCount} unidades en paralelo");
            sb.AppendLine($"  Part Number:     {components.OutputCapacitor.PartNumber}");
            sb.AppendLine($"  Capacitancia c/u:{components.OutputCapacitor.Capacitance * 1e6:F0} µF");
            sb.AppendLine($"  Cap. Total:      {components.OutputCapacitor.Capacitance * components.OutputCapacitorCount * 1e6:F0} µF");
            sb.AppendLine($"  Tensión:         {components.OutputCapacitor.Voltage} V");
            sb.AppendLine($"  Tipo:            {components.OutputCapacitor.Type}");
            sb.AppendLine($"  ESR c/u:         {components.OutputCapacitor.Esr * 1000:F1} mΩ");
            sb.AppendLine($"  ESR equivalente: {(components.OutputCapacitor.Esr / components.OutputCapacitorCount) * 1000:F2} mΩ");
            sb.AppendLine($"  Package:         {components.OutputCapacitor.Package}");
            sb.AppendLine($"  Supplier:        {components.OutputCapacitor.Supplier}\n");

            sb.AppendLine("▌ CONTROLADOR PWM");
            sb.AppendLine("───────────────────────────────────────────────────────");
            sb.AppendLine($"  Part Number:     {components.PwmController.PartNumber}");
            sb.AppendLine($"  Descripción:     {components.PwmController.Description}");
            sb.AppendLine($"  Vcc:             {components.PwmController.Vcc} V");
            sb.AppendLine($"  Vref:            {components.PwmController.Vref:F2} V");
            sb.AppendLine($"  Freq. Max:       {components.PwmController.MaxFrequency / 1000} kHz");
            sb.AppendLine($"  Package:         {components.PwmController.Package}");
            sb.AppendLine($"  Supplier:        {components.PwmController.Supplier}\n");

            txtComponents.Text = sb.ToString();
        }

        private void DisplayLosses(CalculationResults results)
        {
            txtLosses.Clear();

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("═══════════════════════");
            sb.AppendLine("  ANÁLISIS TÉRMICO");
            sb.AppendLine("═══════════════════════\n");

            sb.AppendLine("▌ MOSFET (Q1)");
            sb.AppendLine("───────────────────────");
            sb.AppendLine($"Pcond: {results.MosfetConductionLoss:F2} W");
            sb.AppendLine($"Psw:   {results.MosfetSwitchingLoss:F2} W");
            sb.AppendLine($"Total: {results.MosfetTotalLoss:F2} W");
            sb.AppendLine($"Tj:    {results.MosfetJunctionTemp:F1} °C");
            sb.AppendLine();

            sb.AppendLine("▌ DIODO (D1)");
            sb.AppendLine("───────────────────────");
            sb.AppendLine($"Pcond: {results.DiodeConductionLoss:F2} W");
            sb.AppendLine($"Tj:    {results.DiodeJunctionTemp:F1} °C");
            sb.AppendLine();

            sb.AppendLine("▌ INDUCTOR (L1)");
            sb.AppendLine("───────────────────────");
            sb.AppendLine($"Pdcr:  {results.InductorCoreLoss:F2} W");
            sb.AppendLine($"Temp:  {results.InductorTemp:F1} °C");
            sb.AppendLine();

            sb.AppendLine("▌ TOTALES");
            sb.AppendLine("───────────────────────");
            sb.AppendLine($"Pérd:  {results.TotalLosses:F2} W");
            sb.AppendLine($"Efic:  {results.ActualEfficiency:F2} %");
            sb.AppendLine();

            // Advertencias térmicas
            if (results.MosfetJunctionTemp > 100)
            {
                sb.AppendLine("⚠ MOSFET: TEMP ALTA");
            }
            if (results.DiodeJunctionTemp > 100)
            {
                sb.AppendLine("⚠ DIODO: TEMP ALTA");
            }

            txtLosses.Text = sb.ToString();
        }

        private void ColorizeResults()
        {
            // Colorear títulos principales
            ColorizeText(txtResults, "RESULTADOS DE DISEÑO", Color.DarkBlue, FontStyle.Bold);
            ColorizeText(txtResults, "PARÁMETROS FUNDAMENTALES", Color.DarkGreen, FontStyle.Bold);
            ColorizeText(txtResults, "COMPONENTES MAGNÉTICOS", Color.DarkGreen, FontStyle.Bold);
            ColorizeText(txtResults, "CAPACITORES", Color.DarkGreen, FontStyle.Bold);
        }

        private void ColorizeText(RichTextBox rtb, string text, Color color, FontStyle style)
        {
            int index = 0;
            while (index < rtb.Text.Length)
            {
                index = rtb.Text.IndexOf(text, index, StringComparison.Ordinal);
                if (index == -1) break;

                rtb.Select(index, text.Length);
                rtb.SelectionColor = color;
                rtb.SelectionFont = new Font(rtb.Font, style);
                index += text.Length;
            }

            rtb.Select(0, 0);
        }

        private string FormatResistor(double value)
        {
            if (value >= 1e6)
                return $"{value / 1e6:F2} MΩ";
            else if (value >= 1e3)
                return $"{value / 1e3:F2} kΩ";
            else
                return $"{value:F2} Ω";
        }

        private string FormatCapacitor(double value)
        {
            if (value >= 1e-6)
                return $"{value * 1e6:F2} µF";
            else if (value >= 1e-9)
                return $"{value * 1e9:F2} nF";
            else
                return $"{value * 1e12:F2} pF";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Limpiar campos de entrada
            numVin.Value = 90;
            numVout.Value = 13;
            numIout.Value = 5;
            numFreq.Value = 100000;
            numRippleCurrent.Value = 20;
            numRippleVoltage.Value = 50;
            numEfficiency.Value = 85;

            // Limpiar resultados
            txtResults.Clear();
            txtComponents.Clear();
            txtLosses.Clear();

            toolStripStatusLabel.Text = "Campos limpiados - Listo para calcular";
            toolStripStatusLabel.ForeColor = Color.Black;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf|Text Files (*.txt)|*.txt",
                    DefaultExt = "pdf",
                    FileName = $"BuckConverter_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveDialog.FileName.EndsWith(".txt"))
                    {
                        ExportToText(saveDialog.FileName);
                    }
                    else
                    {
                        ExportToPdf(saveDialog.FileName);
                    }

                    MessageBox.Show(
                        "Archivo exportado exitosamente.",
                        "Exportación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al exportar:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportToText(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("═══════════════════════════════════════════════════════════════");
                writer.WriteLine("       REPORTE DE DISEÑO - BUCK CONVERTER");
                writer.WriteLine($"       Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine("═══════════════════════════════════════════════════════════════\n");

                writer.WriteLine("PARÁMETROS DE ENTRADA:");
                writer.WriteLine($"  Vin:  {numVin.Value} V");
                writer.WriteLine($"  Vout: {numVout.Value} V");
                writer.WriteLine($"  Iout: {numIout.Value} A");
                writer.WriteLine($"  Freq: {numFreq.Value / 1000} kHz\n");

                writer.WriteLine("RESULTADOS DE DISEÑO:");
                writer.WriteLine(txtResults.Text);
                writer.WriteLine("\n");

                writer.WriteLine("COMPONENTES SELECCIONADOS:");
                writer.WriteLine(txtComponents.Text);
                writer.WriteLine("\n");

                writer.WriteLine("ANÁLISIS DE PÉRDIDAS:");
                writer.WriteLine(txtLosses.Text);
            }
        }

        private void ExportToPdf(string filename)
        {
            // Para PDF necesitarías una librería como iTextSharp
            // Por ahora exportamos como texto
            ExportToText(filename.Replace(".pdf", ".txt"));
        }

        // Event handlers del menú
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnClear_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Función de apertura de archivos en desarrollo.", "Información");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnExport_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void componentDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Base de datos de componentes:\n\n" +
                "• MOSFETs: 15 modelos\n" +
                "• Diodos: 12 modelos\n" +
                "• Inductores: 20 valores\n" +
                "• Capacitores: 25 valores\n" +
                "• PWM Controllers: 8 modelos",
                "Base de Datos");
        }

        private void presetConfigurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var presetForm = new PresetForm();
            if (presetForm.ShowDialog() == DialogResult.OK)
            {
                // Aplicar preset seleccionado
                var preset = presetForm.SelectedPreset;
                numVin.Value = (decimal)preset.Vin;
                numVout.Value = (decimal)preset.Vout;
                numIout.Value = (decimal)preset.Iout;
                numFreq.Value = (decimal)preset.Frequency;
            }
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "CALCULADORA BUCK CONVERTER\n\n" +
                "Esta herramienta realiza cálculos completos para diseño de\n" +
                "convertidores Buck (step-down) DC-DC.\n\n" +
                "Características:\n" +
                "• Cálculo de duty cycle\n" +
                "• Diseño de inductor y capacitores\n" +
                "• Selección automática de componentes\n" +
                "• Análisis de pérdidas y eficiencia\n" +
                "• Análisis térmico\n" +
                "• Divisor de realimentación\n" +
                "• Configuración PWM (UC3843)\n\n" +
                "Desarrollado para ingenieros de potencia.",
                "Documentación");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "CALCULADORA BUCK CONVERTER\n\n" +
                "Versión 1.0\n" +
                "Copyright © 2026\n\n" +
                "Herramienta profesional para diseño de\n" +
                "fuentes switching tipo Buck.\n\n" +
                "Basado en topología estándar con UC3843.",
                "Acerca de",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnViewSchematic_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que hay resultados calculados
                if (txtResults.Text.Length == 0)
                {
                    MessageBox.Show(
                        "Primero debe realizar un cálculo antes de ver el esquemático.",
                        "Información",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Recolectar parámetros actuales
                var parameters = new DesignParameters
                {
                    Vin = (double)numVin.Value,
                    Vout = (double)numVout.Value,
                    Iout = (double)numIout.Value,
                    Frequency = (double)numFreq.Value,
                    RippleCurrentPercent = (double)numRippleCurrent.Value,
                    RippleVoltageMv = (double)numRippleVoltage.Value,
                    EfficiencyPercent = (double)numEfficiency.Value
                };

                // Calcular resultados
                var calculator = new BuckCalculator();
                var results = calculator.Calculate(parameters);

                // Abrir formulario de visualización
                var schematicForm = new SchematicViewerForm(parameters, results);
                schematicForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir el visor de esquemático:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnOpenEditor_Click(object sender, EventArgs e)
        {
            var parameters = new DesignParameters
            {
                Vin = 90,
                Vout = 13,
                Iout = 5
            };

            var results = new CalculationResults(); // con valores por defecto

            var editor = new SchematicEditorForm(parameters, results);
            editor.Show();
        }

        private void btnCalculateReactor_Click(object sender, EventArgs e)
        {
            CalcularReactoresVFD form = new CalcularReactoresVFD();
            form.Show();
        }

        private void btnTransformerCalculator_Click(object sender, EventArgs e)
        {
            //TransformerCalculator calculator = new TransformerCalculator();
            TransformerForm form = new TransformerForm();
            form.Show();
        }

        private void btnCalcProteccionMotores_Click(object sender, EventArgs e)
        {
            CalculoProteccionMotores calcProt = new CalculoProteccionMotores();
            calcProt.Show();
        }

        private void btnSchematicUnifilar_Click(object sender, EventArgs e)
        {
            SchematicUnifilar schematicUnifilar = new SchematicUnifilar();
            schematicUnifilar.Show();
        }

        private ComponentDatabaseDialog databaseDialog;
        private ComponentDatabase componentDatabase;
        private void btnDatabaseComp_Click(object sender, EventArgs e)
        {
            if (databaseDialog == null || databaseDialog.IsDisposed)
            {
                databaseDialog = new ComponentDatabaseDialog(componentDatabase);
            }

            databaseDialog.Show();
        }
    }
}