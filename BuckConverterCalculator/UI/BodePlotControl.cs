using BuckConverterCalculator.Analysis;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator.UI
{
    public class BodePlotControl : UserControl
    {
        private BodePlot plotData;
        private Panel magnitudePanel;
        private Panel phasePanel;

        public BodePlotControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);

            // Panel superior: Magnitud
            magnitudePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = this.Height / 2,
                BorderStyle = BorderStyle.FixedSingle
            };
            magnitudePanel.Paint += MagnitudePanel_Paint;

            // Panel inferior: Fase
            phasePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            phasePanel.Paint += PhasePanel_Paint;

            this.Controls.Add(phasePanel);
            this.Controls.Add(magnitudePanel);
        }

        public void SetData(BodePlot data)
        {
            this.plotData = data;
            this.Invalidate();
            magnitudePanel.Invalidate();
            phasePanel.Invalidate();
        }

        private void MagnitudePanel_Paint(object sender, PaintEventArgs e)
        {
            if (plotData == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int margin = 60;
            int width = magnitudePanel.Width - 2 * margin;
            int height = magnitudePanel.Height - 2 * margin;

            // Título
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString("Magnitude (dB)", titleFont, Brushes.Black, margin, 10);
            }

            // Ejes
            using (Pen axisPen = new Pen(Color.Black, 2))
            {
                // Eje X (frecuencia)
                g.DrawLine(axisPen, margin, margin + height, margin + width, margin + height);
                // Eje Y (magnitud)
                g.DrawLine(axisPen, margin, margin, margin, margin + height);
            }

            // Grid
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Líneas horizontales (cada 20 dB)
                for (int db = -100; db <= 100; db += 20)
                {
                    int y = MapMagnitudeToY(db, margin, height);
                    if (y >= margin && y <= margin + height)
                    {
                        g.DrawLine(gridPen, margin, y, margin + width, y);
                        g.DrawString($"{db} dB", new Font("Arial", 8), Brushes.Black, margin - 50, y - 8);
                    }
                }
            }

            // Curva de magnitud
            using (Pen curvePen = new Pen(Color.Blue, 2))
            {
                for (int i = 0; i < plotData.Frequencies.Count - 1; i++)
                {
                    int x1 = MapFrequencyToX(plotData.Frequencies[i], margin, width);
                    int y1 = MapMagnitudeToY(plotData.MagnitudeDB[i], margin, height);
                    int x2 = MapFrequencyToX(plotData.Frequencies[i + 1], margin, width);
                    int y2 = MapMagnitudeToY(plotData.MagnitudeDB[i + 1], margin, height);

                    g.DrawLine(curvePen, x1, y1, x2, y2);
                }
            }

            // Línea de 0 dB (crossover)
            using (Pen crossoverPen = new Pen(Color.Red, 1))
            {
                crossoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                int y0dB = MapMagnitudeToY(0, margin, height);
                g.DrawLine(crossoverPen, margin, y0dB, margin + width, y0dB);
            }

            // Mostrar gain margin
            using (Font font = new Font("Arial", 10))
            {
                string gmText = $"Gain Margin: {plotData.GainMargin:F2} dB";
                g.DrawString(gmText, font, Brushes.DarkGreen, margin + width - 200, margin + 10);
            }
        }

        private void PhasePanel_Paint(object sender, PaintEventArgs e)
        {
            if (plotData == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int margin = 60;
            int width = phasePanel.Width - 2 * margin;
            int height = phasePanel.Height - 2 * margin;

            // Título
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString("Phase (degrees)", titleFont, Brushes.Black, margin, 10);
            }

            // Ejes
            using (Pen axisPen = new Pen(Color.Black, 2))
            {
                g.DrawLine(axisPen, margin, margin + height, margin + width, margin + height);
                g.DrawLine(axisPen, margin, margin, margin, margin + height);
            }

            // Grid
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                for (int deg = -360; deg <= 0; deg += 45)
                {
                    int y = MapPhaseToY(deg, margin, height);
                    if (y >= margin && y <= margin + height)
                    {
                        g.DrawLine(gridPen, margin, y, margin + width, y);
                        g.DrawString($"{deg}°", new Font("Arial", 8), Brushes.Black, margin - 50, y - 8);
                    }
                }
            }

            // Curva de fase
            using (Pen curvePen = new Pen(Color.Green, 2))
            {
                for (int i = 0; i < plotData.Frequencies.Count - 1; i++)
                {
                    int x1 = MapFrequencyToX(plotData.Frequencies[i], margin, width);
                    int y1 = MapPhaseToY(plotData.PhaseDegrees[i], margin, height);
                    int x2 = MapFrequencyToX(plotData.Frequencies[i + 1], margin, width);
                    int y2 = MapPhaseToY(plotData.PhaseDegrees[i + 1], margin, height);

                    g.DrawLine(curvePen, x1, y1, x2, y2);
                }
            }

            // Línea de -180° (phase crossover)
            using (Pen crossoverPen = new Pen(Color.Red, 1))
            {
                crossoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                int y180 = MapPhaseToY(-180, margin, height);
                g.DrawLine(crossoverPen, margin, y180, margin + width, y180);
            }

            // Mostrar phase margin
            using (Font font = new Font("Arial", 10))
            {
                string pmText = $"Phase Margin: {plotData.PhaseMargin:F2}°";
                Color pmColor = plotData.PhaseMargin > 45 ? Color.DarkGreen :
                               plotData.PhaseMargin > 30 ? Color.Orange : Color.Red;
                g.DrawString(pmText, font, new SolidBrush(pmColor), margin + width - 200, margin + 10);
            }
        }

        private int MapFrequencyToX(double freq, int margin, int width)
        {
            double minFreq = plotData.Frequencies[0];
            double maxFreq = plotData.Frequencies[plotData.Frequencies.Count - 1];
            double logMin = Math.Log10(minFreq);
            double logMax = Math.Log10(maxFreq);
            double logFreq = Math.Log10(freq);

            double ratio = (logFreq - logMin) / (logMax - logMin);
            return margin + (int)(ratio * width);
        }

        private int MapMagnitudeToY(double mag, int margin, int height)
        {
            double minMag = -100;
            double maxMag = 100;
            double ratio = (maxMag - mag) / (maxMag - minMag);
            return margin + (int)(ratio * height);
        }

        private int MapPhaseToY(double phase, int margin, int height)
        {
            double minPhase = -360;
            double maxPhase = 0;
            double ratio = (maxPhase - phase) / (maxPhase - minPhase);
            return margin + (int)(ratio * height);
        }
    }
}