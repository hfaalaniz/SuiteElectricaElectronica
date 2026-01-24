using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using BuckConverterCalculator.Analysis;

namespace BuckConverterCalculator.UI.Controls
{
    /// <summary>
    /// Control para mostrar diagrama de Bode (magnitud y fase)
    /// </summary>
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
            this.BackColor = Color.White;

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

        /// <summary>
        /// Establece los datos del diagrama de Bode
        /// </summary>
        public void SetBodePlot(BodePlot data)
        {
            this.plotData = data;
            this.Invalidate();
            magnitudePanel?.Invalidate();
            phasePanel?.Invalidate();
        }

        private void MagnitudePanel_Paint(object sender, PaintEventArgs e)
        {
            if (plotData == null || plotData.Frequencies == null || plotData.Frequencies.Count == 0)
            {
                DrawEmptyState(e.Graphics, "Magnitude (dB)");
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

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
            DrawGrid(g, margin, width, height, true);

            // Curva de magnitud
            using (Pen curvePen = new Pen(Color.Blue, 2))
            {
                var points = new List<PointF>();

                for (int i = 0; i < plotData.Frequencies.Count; i++)
                {
                    int x = MapFrequencyToX(plotData.Frequencies[i], margin, width);
                    int y = MapMagnitudeToY(plotData.MagnitudeDB[i], margin, height);
                    points.Add(new PointF(x, y));
                }

                if (points.Count > 1)
                {
                    g.DrawLines(curvePen, points.ToArray());
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
            if (!double.IsNaN(plotData.GainMargin))
            {
                using (Font font = new Font("Arial", 10))
                {
                    string gmText = $"Gain Margin: {plotData.GainMargin:F2} dB";
                    g.DrawString(gmText, font, Brushes.DarkGreen, margin + width - 200, margin + 10);
                }
            }
        }

        private void PhasePanel_Paint(object sender, PaintEventArgs e)
        {
            if (plotData == null || plotData.Frequencies == null || plotData.Frequencies.Count == 0)
            {
                DrawEmptyState(e.Graphics, "Phase (degrees)");
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

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
            DrawGrid(g, margin, width, height, false);

            // Curva de fase
            using (Pen curvePen = new Pen(Color.Green, 2))
            {
                var points = new List<PointF>();

                for (int i = 0; i < plotData.Frequencies.Count; i++)
                {
                    int x = MapFrequencyToX(plotData.Frequencies[i], margin, width);
                    int y = MapPhaseToY(plotData.PhaseDegrees[i], margin, height);
                    points.Add(new PointF(x, y));
                }

                if (points.Count > 1)
                {
                    g.DrawLines(curvePen, points.ToArray());
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
            if (!double.IsNaN(plotData.PhaseMargin))
            {
                using (Font font = new Font("Arial", 10))
                {
                    string pmText = $"Phase Margin: {plotData.PhaseMargin:F2}°";
                    Color pmColor = plotData.PhaseMargin > 45 ? Color.DarkGreen :
                                   plotData.PhaseMargin > 30 ? Color.Orange : Color.Red;
                    g.DrawString(pmText, font, new SolidBrush(pmColor), margin + width - 200, margin + 10);
                }
            }
        }

        private void DrawGrid(Graphics g, int margin, int width, int height, bool isMagnitude)
        {
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Líneas horizontales
                int steps = isMagnitude ? 10 : 8;
                for (int i = 0; i <= steps; i++)
                {
                    int y = margin + (height * i / steps);
                    g.DrawLine(gridPen, margin, y, margin + width, y);

                    // Etiquetas
                    double value;
                    if (isMagnitude)
                    {
                        value = 100 - (200.0 * i / steps); // -100 a +100 dB
                    }
                    else
                    {
                        value = -(360.0 * i / steps); // 0 a -360°
                    }

                    using (Font font = new Font("Arial", 8))
                    {
                        string label = isMagnitude ? $"{value:F0}" : $"{value:F0}°";
                        SizeF size = g.MeasureString(label, font);
                        g.DrawString(label, font, Brushes.Black, margin - size.Width - 5, y - 8);
                    }
                }

                // Líneas verticales (décadas de frecuencia)
                for (int i = 0; i <= 10; i++)
                {
                    int x = margin + (width * i / 10);
                    g.DrawLine(gridPen, x, margin, x, margin + height);

                    if (i % 2 == 0 && plotData.Frequencies.Count > 0)
                    {
                        double minFreq = plotData.Frequencies[0];
                        double maxFreq = plotData.Frequencies[plotData.Frequencies.Count - 1];
                        double logMin = Math.Log10(minFreq);
                        double logMax = Math.Log10(maxFreq);
                        double logFreq = logMin + (logMax - logMin) * i / 10;
                        double freq = Math.Pow(10, logFreq);

                        using (Font font = new Font("Arial", 8))
                        {
                            string label = FormatFrequency(freq);
                            SizeF size = g.MeasureString(label, font);
                            g.DrawString(label, font, Brushes.Black, x - size.Width / 2, margin + height + 5);
                        }
                    }
                }
            }
        }

        private void DrawEmptyState(Graphics g, string title)
        {
            using (Font titleFont = new Font("Arial", 12, FontStyle.Bold))
            using (Font font = new Font("Arial", 10))
            {
                g.DrawString(title, titleFont, Brushes.Black, 60, 10);
                g.DrawString("No data to display", font, Brushes.Gray, 60, 40);
            }
        }

        private int MapFrequencyToX(double freq, int margin, int width)
        {
            if (plotData.Frequencies.Count == 0) return margin;

            double minFreq = plotData.Frequencies[0];
            double maxFreq = plotData.Frequencies[plotData.Frequencies.Count - 1];

            if (minFreq <= 0 || maxFreq <= 0) return margin;

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

        private string FormatFrequency(double freq)
        {
            if (freq >= 1e6)
                return $"{freq / 1e6:F1}M";
            else if (freq >= 1e3)
                return $"{freq / 1e3:F1}k";
            else
                return $"{freq:F1}";
        }
    }
}