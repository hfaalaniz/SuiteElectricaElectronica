using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BuckConverterCalculator.Simulation;

namespace BuckConverterCalculator.UI
{
    /// <summary>
    /// Control para visualizar formas de onda de la simulación
    /// </summary>
    public class WaveformViewer : UserControl
    {
        private SimulationResults results;
        private List<WaveformPlot> plots;
        private VScrollBar scrollBar;

        public WaveformViewer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = false;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;

            scrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Visible = false
            };
            scrollBar.Scroll += (s, e) => this.Invalidate();

            this.Controls.Add(scrollBar);

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

            if (results == null || results.Time == null || results.Time.Count == 0)
            {
                DrawEmptyState(e.Graphics);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int plotHeight = 150;
            int margin = 60;
            int plotWidth = this.Width - 2 * margin - scrollBar.Width;

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
                    DrawWaveform(g, plots[i], data, results.Time, margin, yOffset, plotWidth, plotHeight);
                }
            }

            // Mostrar métricas en la parte inferior
            DrawMetrics(g, margin, plots.Count * (plotHeight + 20) + 40);
        }

        private void DrawEmptyState(Graphics g)
        {
            using (Font font = new Font("Arial", 12))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                string message = "No simulation results to display.\nRun a simulation to see waveforms.";
                SizeF size = g.MeasureString(message, font);
                g.DrawString(message, font, brush,
                    (this.Width - size.Width) / 2,
                    (this.Height - size.Height) / 2);
            }
        }

        private void DrawWaveform(Graphics g, WaveformPlot plot, List<double> data, List<double> time,
                                 int x, int y, int width, int height)
        {
            // Título
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            {
                g.DrawString(plot.Title, titleFont, Brushes.Black, x, y - 15);
            }

            // Borde del área de ploteo
            using (Pen borderPen = new Pen(Color.Black, 1))
            {
                g.DrawRectangle(borderPen, x, y, width, height);
            }

            // Encontrar min/max para escala
            double minVal = data.Min();
            double maxVal = data.Max();

            // Agregar margen a la escala
            double range = maxVal - minVal;
            if (range < 1e-6) range = 1; // Evitar división por cero
            minVal -= range * 0.1;
            maxVal += range * 0.1;

            // Dibujar grid
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Líneas horizontales
                for (int i = 0; i <= 5; i++)
                {
                    int gridY = y + (height * i / 5);
                    g.DrawLine(gridPen, x, gridY, x + width, gridY);

                    double value = maxVal - (maxVal - minVal) * i / 5;
                    using (Font font = new Font("Arial", 7))
                    {
                        string label = FormatValue(value);
                        SizeF size = g.MeasureString(label, font);
                        g.DrawString(label, font, Brushes.Black, x - size.Width - 5, gridY - 7);
                    }
                }

                // Líneas verticales (tiempo)
                for (int i = 0; i <= 10; i++)
                {
                    int gridX = x + (width * i / 10);
                    g.DrawLine(gridPen, gridX, y, gridX, y + height);

                    if (i % 2 == 0 && time.Count > 0)
                    {
                        double timeValue = time[0] + (time[time.Count - 1] - time[0]) * i / 10;
                        using (Font font = new Font("Arial", 7))
                        {
                            string label = FormatTime(timeValue);
                            SizeF size = g.MeasureString(label, font);
                            g.DrawString(label, font, Brushes.Black, gridX - size.Width / 2, y + height + 5);
                        }
                    }
                }
            }

            // Dibujar forma de onda
            if (data.Count > 1)
            {
                using (Pen wavePen = new Pen(plot.Color, 2))
                {
                    var points = new List<Point>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        int px = x + (int)((double)width * i / (data.Count - 1));
                        int py = y + (int)(height * (1 - (data[i] - minVal) / (maxVal - minVal)));

                        // Clamp
                        py = Math.Max(y, Math.Min(y + height, py));

                        points.Add(new Point(px, py));
                    }

                    // Dibujar línea
                    if (points.Count > 1)
                    {
                        g.DrawLines(wavePen, points.ToArray());
                    }
                }
            }

            // Etiqueta de tiempo
            using (Font font = new Font("Arial", 8))
            {
                g.DrawString("Time (s)", font, Brushes.Black, x + width / 2 - 20, y + height + 20);
            }
        }

        private void DrawMetrics(Graphics g, int x, int y)
        {
            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 9))
            {
                g.DrawString("Simulation Metrics:", font, Brushes.Black, x, y);
                y += 20;

                g.DrawString($"Average Output Voltage: ", font, Brushes.Black, x, y);
                g.DrawString($"{results.AverageOutputVoltage:F3} V", valueFont, Brushes.Blue, x + 180, y);
                y += 20;

                g.DrawString($"Output Ripple: ", font, Brushes.Black, x, y);
                g.DrawString($"{results.OutputRipple * 1000:F2} mV", valueFont, Brushes.Blue, x + 180, y);
                y += 20;

                g.DrawString($"Peak Inductor Current: ", font, Brushes.Black, x, y);
                g.DrawString($"{results.PeakInductorCurrent:F3} A", valueFont, Brushes.Blue, x + 180, y);
                y += 20;

                g.DrawString($"RMS Inductor Current: ", font, Brushes.Black, x, y);
                g.DrawString($"{results.RMSInductorCurrent:F3} A", valueFont, Brushes.Blue, x + 180, y);
                y += 20;

                g.DrawString($"Inductor Current Ripple: ", font, Brushes.Black, x, y);
                g.DrawString($"{results.InductorCurrentRipple:F3} A", valueFont, Brushes.Blue, x + 180, y);
            }
        }

        private string FormatValue(double value)
        {
            if (Math.Abs(value) >= 1000)
                return $"{value / 1000:F1}k";
            else if (Math.Abs(value) >= 1)
                return $"{value:F2}";
            else if (Math.Abs(value) >= 0.001)
                return $"{value * 1000:F1}m";
            else
                return $"{value * 1e6:F1}µ";
        }

        private string FormatTime(double time)
        {
            if (time >= 1)
                return $"{time:F2}s";
            else if (time >= 0.001)
                return $"{time * 1000:F2}ms";
            else
                return $"{time * 1e6:F2}µs";
        }

        private class WaveformPlot
        {
            public string Title { get; set; }
            public Color Color { get; set; }
        }
    }
}