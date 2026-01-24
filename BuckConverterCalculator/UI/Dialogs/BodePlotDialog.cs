using System;
using System.Windows.Forms;
using System.Drawing;
using BuckConverterCalculator.Analysis;
using BuckConverterCalculator.UI.Controls;

namespace BuckConverterCalculator.UI.Dialogs
{
    /// <summary>
    /// Diálogo para mostrar diagrama de Bode y análisis de estabilidad
    /// </summary>
    public class BodePlotDialog : Form
    {
        private BodePlotControl bodePlotControl;
        private Panel controlPanel;
        private TextBox txtInductance;
        private TextBox txtCapacitance;
        private TextBox txtESR;
        private TextBox txtLoad;
        private Button btnAnalyze;
        private Button btnExport;
        private Button btnClose;
        private Label lblPhaseMargin;
        private Label lblGainMargin;
        private Label lblCrossover;
        private Label lblStability;

        private BodePlot currentPlot;

        public BodePlotDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Bode Plot - Stability Analysis";
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
                Text = "Circuit Parameters",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblTitle);
            y += 30;

            // Inductance
            AddLabelAndTextBox(controlPanel, "Inductance (µH):", ref txtInductance, ref y, "47");

            // Capacitance
            AddLabelAndTextBox(controlPanel, "Capacitance (µF):", ref txtCapacitance, ref y, "10");

            // ESR
            AddLabelAndTextBox(controlPanel, "ESR (mΩ):", ref txtESR, ref y, "5");

            // Load
            AddLabelAndTextBox(controlPanel, "Load (Ω):", ref txtLoad, ref y, "2.6");

            y += 10;

            // Analyze Button
            btnAnalyze = new Button
            {
                Text = "Analyze Stability",
                Location = new Point(10, y),
                Width = 250,
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

            // Results Title
            var lblResults = new Label
            {
                Text = "Stability Metrics",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblResults);
            y += 30;

            // Phase Margin
            lblPhaseMargin = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 40,
                Text = "Phase Margin: -",
                Font = new Font("Arial", 9)
            };
            controlPanel.Controls.Add(lblPhaseMargin);
            y += 45;

            // Gain Margin
            lblGainMargin = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 40,
                Text = "Gain Margin: -",
                Font = new Font("Arial", 9)
            };
            controlPanel.Controls.Add(lblGainMargin);
            y += 45;

            // Crossover Frequency
            lblCrossover = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 40,
                Text = "Crossover Freq: -",
                Font = new Font("Arial", 9)
            };
            controlPanel.Controls.Add(lblCrossover);
            y += 45;

            // Stability Status
            lblStability = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 60,
                Text = "Status: -",
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblStability);
            y += 70;

            // Export Button
            btnExport = new Button
            {
                Text = "Export Data",
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

            // Bode Plot Control
            bodePlotControl = new BodePlotControl
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(bodePlotControl);
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

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse parameters
                double L = double.Parse(txtInductance.Text) * 1e-6; // µH to H
                double C = double.Parse(txtCapacitance.Text) * 1e-6; // µF to F
                double ESR = double.Parse(txtESR.Text) * 1e-3; // mΩ to Ω
                double R = double.Parse(txtLoad.Text);

                // Create analyzer
                var analyzer = new BodeAnalyzer
                {
                    Inductance = L,
                    Capacitance = C,
                    ESR = ESR,
                    LoadResistance = R,
                    InputVoltage = 90,
                    OutputVoltage = 13
                };

                // Generate Bode plot (1 Hz to 1 MHz, 300 points)
                currentPlot = analyzer.GenerateBodePlot(1, 1000000, 300);

                // Update display
                bodePlotControl.SetBodePlot(currentPlot);

                // Update metrics
                lblPhaseMargin.Text = $"Phase Margin:\n{currentPlot.PhaseMargin:F2}°";
                lblPhaseMargin.ForeColor = GetPhaseMarginColor(currentPlot.PhaseMargin);

                lblGainMargin.Text = $"Gain Margin:\n{currentPlot.GainMargin:F2} dB";
                lblGainMargin.ForeColor = GetGainMarginColor(currentPlot.GainMargin);

                lblCrossover.Text = $"Crossover Freq:\n{FormatFrequency(currentPlot.CrossoverFrequency)}";

                string stabilityText = currentPlot.GetStabilityReport();
                lblStability.Text = $"Status:\n{stabilityText}";
                lblStability.ForeColor = currentPlot.IsStable() ? Color.Green : Color.Red;

                btnExport.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analysis error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Color GetPhaseMarginColor(double pm)
        {
            if (pm < 30) return Color.Red;
            if (pm < 45) return Color.Orange;
            return Color.Green;
        }

        private Color GetGainMarginColor(double gm)
        {
            if (gm < 6) return Color.Red;
            if (gm < 10) return Color.Orange;
            return Color.Green;
        }

        private string FormatFrequency(double freq)
        {
            if (double.IsNaN(freq)) return "N/A";

            if (freq >= 1e6)
                return $"{freq / 1e6:F2} MHz";
            else if (freq >= 1e3)
                return $"{freq / 1e3:F2} kHz";
            else
                return $"{freq:F2} Hz";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (currentPlot == null) return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                dialog.FileName = "bode_plot.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportBodePlotToCSV(currentPlot, dialog.FileName);
                        MessageBox.Show($"Bode plot exported to {dialog.FileName}",
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

        private void ExportBodePlotToCSV(BodePlot plot, string filename)
        {
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("# Bode Plot Data");
                writer.WriteLine($"# Phase Margin: {plot.PhaseMargin:F2}°");
                writer.WriteLine($"# Gain Margin: {plot.GainMargin:F2} dB");
                writer.WriteLine($"# Crossover Frequency: {plot.CrossoverFrequency:F2} Hz");
                writer.WriteLine("#");
                writer.WriteLine("Frequency(Hz),Magnitude(dB),Phase(degrees)");

                for (int i = 0; i < plot.Frequencies.Count; i++)
                {
                    writer.WriteLine($"{plot.Frequencies[i]:E6},{plot.MagnitudeDB[i]:F6},{plot.PhaseDegrees[i]:F6}");
                }
            }
        }

        public void SetBodePlot(BodePlot plot)
        {
            currentPlot = plot;
            bodePlotControl.SetBodePlot(plot);
            btnExport.Enabled = true;
        }
    }
}