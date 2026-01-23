using BuckConverterCalculator.Analysis;
using BuckConverterCalculator.Simulation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuckConverterCalculator.UI
{
    public class DCMAnalysisDialog : Form
    {
        private DCMVisualizationPanel visualizationPanel;
        private TextBox txtInputVoltage;
        private TextBox txtOutputVoltage;
        private TextBox txtOutputCurrent;
        private TextBox txtInductance;
        private TextBox txtFrequency;
        private Button btnAnalyze;
        private Label lblMode;
        private Label lblCriticalCurrent;

        public DCMAnalysisDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "DCM Analysis";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Panel de visualización
            visualizationPanel = new DCMVisualizationPanel
            {
                Dock = DockStyle.Fill
            };

            // Panel de controles
            var controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                Padding = new Padding(10)
            };

            int y = 10;

            controlPanel.Controls.Add(new Label { Text = "Input Voltage (V):", Location = new Point(10, y), AutoSize = true });
            txtInputVoltage = new TextBox { Location = new Point(10, y + 20), Width = 200 };
            controlPanel.Controls.Add(txtInputVoltage);
            y += 50;

            controlPanel.Controls.Add(new Label { Text = "Output Voltage (V):", Location = new Point(10, y), AutoSize = true });
            txtOutputVoltage = new TextBox { Location = new Point(10, y + 20), Width = 200 };
            controlPanel.Controls.Add(txtOutputVoltage);
            y += 50;

            controlPanel.Controls.Add(new Label { Text = "Output Current (A):", Location = new Point(10, y), AutoSize = true });
            txtOutputCurrent = new TextBox { Location = new Point(10, y + 20), Width = 200 };
            controlPanel.Controls.Add(txtOutputCurrent);
            y += 50;

            controlPanel.Controls.Add(new Label { Text = "Inductance (µH):", Location = new Point(10, y), AutoSize = true });
            txtInductance = new TextBox { Location = new Point(10, y + 20), Width = 200 };
            controlPanel.Controls.Add(txtInductance);
            y += 50;

            controlPanel.Controls.Add(new Label { Text = "Frequency (kHz):", Location = new Point(10, y), AutoSize = true });
            txtFrequency = new TextBox { Location = new Point(10, y + 20), Width = 200 };
            controlPanel.Controls.Add(txtFrequency);
            y += 50;

            btnAnalyze = new Button
            {
                Text = "Analyze",
                Location = new Point(10, y),
                Width = 200,
                Height = 35
            };
            btnAnalyze.Click += BtnAnalyze_Click;
            controlPanel.Controls.Add(btnAnalyze);
            y += 50;

            lblMode = new Label
            {
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblMode);
            y += 30;

            lblCriticalCurrent = new Label
            {
                Location = new Point(10, y),
                AutoSize = true
            };
            controlPanel.Controls.Add(lblCriticalCurrent);

            this.Controls.Add(visualizationPanel);
            this.Controls.Add(controlPanel);
        }

        public void SetParameters(CircuitParameters parameters)
        {
            txtInputVoltage.Text = parameters.InputVoltage.ToString();
            txtOutputVoltage.Text = (parameters.InputVoltage * parameters.DutyCycle).ToString();
            txtOutputCurrent.Text = "5";
            txtInductance.Text = (parameters.Inductance * 1000000).ToString(); // µH
            txtFrequency.Text = (parameters.SwitchingFrequency / 1000).ToString(); // kHz
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            var analyzer = new DCMAnalyzer
            {
                InputVoltage = double.Parse(txtInputVoltage.Text),
                OutputVoltage = double.Parse(txtOutputVoltage.Text),
                OutputCurrent = double.Parse(txtOutputCurrent.Text),
                Inductance = double.Parse(txtInductance.Text) / 1000000, // Convert µH to H
                SwitchingFrequency = double.Parse(txtFrequency.Text) * 1000 // Convert kHz to Hz
            };

            var mode = analyzer.DetectMode();
            var parameters = analyzer.CalculateDCMParameters();

            lblMode.Text = mode == OperatingMode.DCM ? "Mode: DCM" : "Mode: CCM";
            lblMode.ForeColor = mode == OperatingMode.DCM ? Color.Orange : Color.Green;

            double criticalCurrent = analyzer.CalculateBoundaryCurrent();
            lblCriticalCurrent.Text = $"Critical Current: {criticalCurrent:F3} A";

            visualizationPanel.UpdateVisualization(parameters);
        }
    }
}
