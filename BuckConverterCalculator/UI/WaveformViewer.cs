using BuckConverterCalculator.Simulation;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator.UI
{
    public class WaveformViewer : UserControl
    {
        private SimulationResults results;
        private List<WaveformPlot> plots;

        public WaveformViewer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;
            this.BackColor = Color.White;

            plots = new List<WaveformPlot>
            {
                new WaveformPlot { Title = "Inductor Current (A)", Color = Color.Blue },
                new WaveformPlot { Title = "Output Voltage (V)", Color = Color.Green },
                new WaveformPlot { Title = "Switch Voltage (V)", Color = Color.Red },
                new WaveformPlot { Title = "Diode Voltage (V)", Color = Color.Orange }
            };
        }

        public void SetResults(SimulationResults res)
        {
            this.results = res;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (results == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int plotHeight = 150;
            int margin = 60;
            int plotWidth = this.Width - 2 * margin;

            // Dibujar cada forma de onda
            for (int i = 0; i < plots.Count; i++)
            {
                int yOffset = i * (plotHeight + 20) + 20;

                List<double> data = null;
                switch (i)
                {
                    case 0: data = results.InductorCurrent; break;
                    case 1: data = results.OutputVoltage; break;
                    case 2: data = results.SwitchVoltage; break;
                    case 3: data = results.DiodeVoltage; break;
                }

                if (data != null)
                {
                    DrawWaveform(g, plots[i], data, margin, yOffset, plotWidth, plotHeight);
                }
            }

            // Mostrar métricas
            DrawMetrics(g, margin, plots.Count * (plotHeight + 20) + 40);
        }

        private void DrawWaveform(Graphics g, WaveformPlot plot, List<double> data,
                                 int x, int y, int width, int height)
        {
            // Título
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            {
                g.DrawString(plot.Title, titleFont, Brushes.Black, x, y - 15);
            }

            // Ejes
            using (Pen axisPen = new Pen(Color.Black, 1))
            {
                g.DrawLine(axisPen, x, y + height, x + width, y + height);
                g.DrawLine(axisPen, x, y, x, y + height);
            }

            // Encontrar min/max para escala
            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            foreach (var val in data)
            {
                if (val < minVal) minVal = val;
                if (val > maxVal) maxVal = val;
            }

            // Agregar margen a la escala
            double range = maxVal - minVal;
            minVal -= range * 0.1;
            maxVal += range * 0.1;

            // Dibujar grid
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                for (int i = 0; i <= 5; i++)
                {
                    int gridY = y + (height * i / 5);
                    g.DrawLine(gridPen, x, gridY, x + width, gridY);

                    double value = maxVal - (maxVal - minVal) * i / 5;
                    g.DrawString($"{value:F2}", new Font("Arial", 7), Brushes.Black, x - 50, gridY - 7);
                }
            }

            // Dibujar forma de onda
            using (Pen wavePen = new Pen(plot.Color, 2))
            {
                for (int i = 0; i < data.Count - 1; i++)
                {
                    int x1 = x + (width * i / data.Count);
                    int y1 = y + (int)(height * (1 - (data[i] - minVal) / (maxVal - minVal)));
                    int x2 = x + (width * (i + 1) / data.Count);
                    int y2 = y + (int)(height * (1 - (data[i + 1] - minVal) / (maxVal - minVal)));

                    g.DrawLine(wavePen, x1, y1, x2, y2);
                }
            }
        }

        private void DrawMetrics(Graphics g, int x, int y)
        {
            using (Font font = new Font("Arial", 9))
            {
                g.DrawString($"Average Output Voltage: {results.AverageOutputVoltage:F3} V",
                           font, Brushes.Black, x, y);
                g.DrawString($"Output Ripple: {results.OutputRipple * 1000:F2} mV",
                           font, Brushes.Black, x, y + 20);
                g.DrawString($"Peak Inductor Current: {results.PeakInductorCurrent:F3} A",
                           font, Brushes.Black, x, y + 40);
                g.DrawString($"Inductor Current Ripple: {results.InductorCurrentRipple:F3} A",
                           font, Brushes.Black, x, y + 60);
            }
        }

        private class WaveformPlot
        {
            public string Title { get; set; }
            public Color Color { get; set; }
        }
    }
}