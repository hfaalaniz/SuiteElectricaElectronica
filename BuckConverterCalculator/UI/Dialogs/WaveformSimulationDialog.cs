using System;
using System.Windows.Forms;
using System.Drawing;
using BuckConverterCalculator.Simulation;

namespace BuckConverterCalculator.UI.Dialogs
{
    /// <summary>
    /// Diálogo para configurar y ejecutar simulación de formas de onda
    /// </summary>
    public class WaveformSimulationDialog : Form
    {
        private WaveformViewer viewer;
        private Panel controlPanel;
        private TextBox txtInputVoltage;
        private TextBox txtDutyCycle;
        private TextBox txtFrequency;
        private TextBox txtInductance;
        private TextBox txtCapacitance;
        private TextBox txtLoad;
        private TextBox txtSimTime;
        private CheckBox chkParasitics;
        private CheckBox chkRK4;
        private Button btnRun;
        private Button btnExport;
        private Button btnClose;
        private Label lblStatus;
        private ProgressBar progressBar;

        private WaveformSimulator simulator;
        private SimulationResults lastResults;

        public WaveformSimulationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Waveform Simulation";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 600);

            // Panel de control izquierdo
            controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            int y = 10;

            // Título
            var lblTitle = new Label
            {
                Text = "Simulation Parameters",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblTitle);
            y += 30;

            // Input Voltage
            AddLabelAndTextBox(controlPanel, "Input Voltage (V):", ref txtInputVoltage, ref y, "90");

            // Duty Cycle
            AddLabelAndTextBox(controlPanel, "Duty Cycle (0-1):", ref txtDutyCycle, ref y, "0.144");

            // Frequency
            AddLabelAndTextBox(controlPanel, "Frequency (kHz):", ref txtFrequency, ref y, "100");

            // Inductance
            AddLabelAndTextBox(controlPanel, "Inductance (µH):", ref txtInductance, ref y, "47");

            // Capacitance
            AddLabelAndTextBox(controlPanel, "Capacitance (µF):", ref txtCapacitance, ref y, "10");

            // Load Resistance
            AddLabelAndTextBox(controlPanel, "Load (Ω):", ref txtLoad, ref y, "2.6");

            // Simulation Time
            AddLabelAndTextBox(controlPanel, "Sim Time (ms):", ref txtSimTime, ref y, "2");

            y += 10;

            // Include Parasitics
            chkParasitics = new CheckBox
            {
                Text = "Include Parasitics",
                Location = new Point(10, y),
                Width = 250,
                Checked = true
            };
            controlPanel.Controls.Add(chkParasitics);
            y += 30;

            // Use RK4
            chkRK4 = new CheckBox
            {
                Text = "Use RK4 (more accurate)",
                Location = new Point(10, y),
                Width = 250,
                Checked = true
            };
            controlPanel.Controls.Add(chkRK4);
            y += 40;

            // Run Button
            btnRun = new Button
            {
                Text = "Run Simulation",
                Location = new Point(10, y),
                Width = 250,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnRun.FlatAppearance.BorderSize = 0;
            btnRun.Click += BtnRun_Click;
            controlPanel.Controls.Add(btnRun);
            y += 50;

            // Export Button
            btnExport = new Button
            {
                Text = "Export CSV",
                Location = new Point(10, y),
                Width = 120,
                Height = 30,
                Enabled = false
            };
            btnExport.Click += BtnExport_Click;
            controlPanel.Controls.Add(btnExport);

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(140, y),
                Width = 120,
                Height = 30
            };
            btnClose.Click += (s, e) => this.Close();
            controlPanel.Controls.Add(btnClose);
            y += 40;

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 60,
                Text = "Ready to simulate",
                ForeColor = Color.Gray
            };
            controlPanel.Controls.Add(lblStatus);
            y += 70;

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 20,
                Visible = false
            };
            controlPanel.Controls.Add(progressBar);

            // Viewer
            viewer = new WaveformViewer
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(viewer);
            this.Controls.Add(controlPanel);
        }

        private void AddLabelAndTextBox(Panel panel, string labelText, ref TextBox textBox, ref int y, string defaultValue)
        {
            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, y),
                Width = 250,
                AutoSize = true
            };
            panel.Controls.Add(label);
            y += 20;

            textBox = new TextBox
            {
                Location = new Point(10, y),
                Width = 250,
                Text = defaultValue
            };
            panel.Controls.Add(textBox);
            y += 35;
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar inputs
                if (!ValidateInputs())
                    return;

                // Crear parámetros del circuito
                var circuit = new CircuitParameters
                {
                    InputVoltage = double.Parse(txtInputVoltage.Text),
                    DutyCycle = double.Parse(txtDutyCycle.Text),
                    SwitchingFrequency = double.Parse(txtFrequency.Text) * 1000, // kHz to Hz
                    Inductance = double.Parse(txtInductance.Text) * 1e-6, // µH to H
                    Capacitance = double.Parse(txtCapacitance.Text) * 1e-6, // µF to F
                    LoadResistance = double.Parse(txtLoad.Text),
                    InductorESR = 0.02,
                    CapacitorESR = 0.005,
                    SwitchRDSon = 0.05,
                    DiodeVF = 0.5
                };

                var settings = new SimulationSettings
                {
                    SimulationTime = double.Parse(txtSimTime.Text) * 1e-3, // ms to s
                    StabilizationTime = 0.0005, // 0.5ms
                    SamplesPerCycle = 100,
                    IncludeParasitics = chkParasitics.Checked,
                    UseRK4 = chkRK4.Checked
                };

                // Crear simulador
                simulator = new WaveformSimulator
                {
                    Circuit = circuit,
                    Settings = settings
                };

                // Ejecutar simulación
                lblStatus.Text = "Running simulation...";
                lblStatus.ForeColor = Color.Blue;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                btnRun.Enabled = false;
                Application.DoEvents();

                lastResults = simulator.RunSimulation();

                // Mostrar resultados
                viewer.SetResults(lastResults);

                lblStatus.Text = $"Simulation complete!\n{lastResults.GetSummary()}";
                lblStatus.ForeColor = Color.Green;
                btnExport.Enabled = true;

                MessageBox.Show(lastResults.GetSummary(), "Simulation Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Simulation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                btnRun.Enabled = true;
            }
        }

        private bool ValidateInputs()
        {
            try
            {
                double vin = double.Parse(txtInputVoltage.Text);
                double duty = double.Parse(txtDutyCycle.Text);
                double freq = double.Parse(txtFrequency.Text);
                double L = double.Parse(txtInductance.Text);
                double C = double.Parse(txtCapacitance.Text);
                double R = double.Parse(txtLoad.Text);
                double time = double.Parse(txtSimTime.Text);

                if (vin <= 0 || duty <= 0 || duty >= 1 || freq <= 0 ||
                    L <= 0 || C <= 0 || R <= 0 || time <= 0)
                {
                    MessageBox.Show("All parameters must be positive, and duty cycle must be between 0 and 1.",
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Please enter valid numerical values for all parameters.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (lastResults == null)
                return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                dialog.FileName = "waveforms.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lastResults.ExportToCSV(dialog.FileName);
                        MessageBox.Show($"Waveforms exported to {dialog.FileName}",
                            "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void SetParameters(CircuitParameters parameters)
        {
            txtInputVoltage.Text = parameters.InputVoltage.ToString();
            txtDutyCycle.Text = parameters.DutyCycle.ToString("F3");
            txtFrequency.Text = (parameters.SwitchingFrequency / 1000).ToString();
            txtInductance.Text = (parameters.Inductance * 1e6).ToString();
            txtCapacitance.Text = (parameters.Capacitance * 1e6).ToString();
            txtLoad.Text = parameters.LoadResistance.ToString("F2");
        }
    }
}