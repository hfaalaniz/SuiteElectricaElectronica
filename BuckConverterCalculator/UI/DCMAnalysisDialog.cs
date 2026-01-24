using BuckConverterCalculator.Analysis;
using BuckConverterCalculator.Simulation;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator.UI.Dialogs
{
    /// <summary>
    /// Diálogo para análisis DCM/CCM
    /// </summary>
    public class DCMAnalysisDialog : Form
    {
        private Panel controlPanel;
        private DCMVisualizationPanel visualizationPanel;
        private TextBox txtInputVoltage;
        private TextBox txtOutputVoltage;
        private TextBox txtOutputCurrent;
        private TextBox txtFrequency;
        private TextBox txtInductance;
        private TextBox txtCapacitance;
        private Button btnAnalyze;
        private Button btnClose;
        private Label lblStatus;

        private DCMAnalyzer analyzer;
        private CircuitParameters parameters;

        public DCMAnalysisDialog()
        {
            InitializeComponent();
            analyzer = new DCMAnalyzer();
        }

        private void InitializeComponent()
        {
            this.Text = "DCM/CCM Analysis";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(800, 600);

            // Panel de control (izquierda)
            controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            int y = 10;

            // Título
            var lblTitle = new Label
            {
                Text = "Circuit Parameters",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblTitle);
            y += 35;

            // Parámetros de entrada
            AddLabelAndTextBox(controlPanel, "Input Voltage (V):", ref txtInputVoltage, ref y, "90");
            AddLabelAndTextBox(controlPanel, "Output Voltage (V):", ref txtOutputVoltage, ref y, "13");
            AddLabelAndTextBox(controlPanel, "Output Current (A):", ref txtOutputCurrent, ref y, "5");
            AddLabelAndTextBox(controlPanel, "Frequency (kHz):", ref txtFrequency, ref y, "100");
            AddLabelAndTextBox(controlPanel, "Inductance (µH):", ref txtInductance, ref y, "47");
            AddLabelAndTextBox(controlPanel, "Capacitance (µF):", ref txtCapacitance, ref y, "100");

            y += 20;

            // Botón de análisis
            btnAnalyze = new Button
            {
                Text = "Analyze",
                Location = new Point(10, y),
                Width = 270,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnAnalyze.FlatAppearance.BorderSize = 0;
            btnAnalyze.Click += BtnAnalyze_Click;
            controlPanel.Controls.Add(btnAnalyze);
            y += 50;

            // Estado
            lblStatus = new Label
            {
                Location = new Point(10, y),
                Width = 270,
                Height = 200,
                Text = "Enter circuit parameters and click 'Analyze'",
                ForeColor = Color.Gray
            };
            controlPanel.Controls.Add(lblStatus);
            y += 210;

            // Botón cerrar
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(10, y),
                Width = 270,
                Height = 30
            };
            btnClose.Click += (s, e) => this.Close();
            controlPanel.Controls.Add(btnClose);

            // Panel de visualización (derecha)
            visualizationPanel = new DCMVisualizationPanel
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(visualizationPanel);
            this.Controls.Add(controlPanel);
        }

        private void AddLabelAndTextBox(Panel panel, string labelText, ref TextBox textBox, ref int y, string defaultValue)
        {
            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, y),
                Width = 270,
                AutoSize = true
            };
            panel.Controls.Add(label);
            y += 20;

            textBox = new TextBox
            {
                Location = new Point(10, y),
                Width = 270,
                Text = defaultValue
            };
            panel.Controls.Add(textBox);
            y += 35;
        }

        public void SetParameters(CircuitParameters param)
        {
            this.parameters = param;

            txtInputVoltage.Text = param.InputVoltage.ToString();
            txtOutputVoltage.Text = "13"; // Default
            txtOutputCurrent.Text = "5";  // Default
            txtFrequency.Text = (param.SwitchingFrequency / 1000).ToString();
            txtInductance.Text = (param.Inductance * 1e6).ToString("F1");
            txtCapacitance.Text = (param.Capacitance * 1e6).ToString("F1");
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar entradas
                if (!ValidateInputs())
                    return;

                // Configurar analizador
                analyzer.InputVoltage = double.Parse(txtInputVoltage.Text);
                analyzer.OutputVoltage = double.Parse(txtOutputVoltage.Text);
                analyzer.OutputCurrent = double.Parse(txtOutputCurrent.Text);
                analyzer.SwitchingFrequency = double.Parse(txtFrequency.Text) * 1000;
                analyzer.Inductance = double.Parse(txtInductance.Text) * 1e-6;
                analyzer.Capacitance = double.Parse(txtCapacitance.Text) * 1e-6;

                // Detectar modo
                var mode = analyzer.DetectMode();

                // Calcular parámetros según el modo
                DCMParameters dcmParams;
                if (mode == OperatingMode.DCM)
                {
                    dcmParams = analyzer.CalculateDCMParameters();
                }
                else
                {
                    // CalculateCCMParameters es privado, usar CalculateDCMParameters para todos
                    dcmParams = analyzer.CalculateDCMParameters();
                }

                // Actualizar visualización
                visualizationPanel.SetParameters(dcmParams);

                // Actualizar estado
                UpdateStatus(dcmParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analysis error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            try
            {
                double vin = double.Parse(txtInputVoltage.Text);
                double vout = double.Parse(txtOutputVoltage.Text);
                double iout = double.Parse(txtOutputCurrent.Text);
                double freq = double.Parse(txtFrequency.Text);
                double L = double.Parse(txtInductance.Text);
                double C = double.Parse(txtCapacitance.Text);

                if (vin <= 0 || vout <= 0 || iout <= 0 || freq <= 0 || L <= 0 || C <= 0)
                {
                    MessageBox.Show("All parameters must be positive.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (vout >= vin)
                {
                    MessageBox.Show("Output voltage must be less than input voltage for buck converter.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Please enter valid numerical values.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void UpdateStatus(DCMParameters dcmParams)
        {
            Color statusColor;
            string modeText;

            switch (dcmParams.Mode)
            {
                case OperatingMode.DCM:
                    statusColor = Color.Orange;
                    modeText = "Discontinuous Conduction Mode";
                    break;
                case OperatingMode.CCM:
                    statusColor = Color.Green;
                    modeText = "Continuous Conduction Mode";
                    break;
                case OperatingMode.BCM:
                    statusColor = Color.Yellow;
                    modeText = "Boundary Conduction Mode";
                    break;
                default:
                    statusColor = Color.Gray;
                    modeText = "Unknown Mode";
                    break;
            }

            lblStatus.ForeColor = statusColor;
            lblStatus.Text = $"Operating Mode:\n{modeText}\n\n" +
                           $"Duty Cycle: {dcmParams.DutyCycle:F3}\n" +
                           $"Peak Current: {dcmParams.PeakInductorCurrent:F3} A\n" +
                           $"Avg Current: {dcmParams.AverageInductorCurrent:F3} A\n" +
                           $"RMS Current: {dcmParams.RMSInductorCurrent:F3} A\n" +
                           $"Output Ripple: {dcmParams.OutputRipple * 1000:F2} mV";
        }
    }
}