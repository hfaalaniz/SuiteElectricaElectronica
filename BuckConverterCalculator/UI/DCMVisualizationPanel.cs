using System;
using System.Drawing;
using System.Windows.Forms;
using BuckConverterCalculator.Analysis;

namespace BuckConverterCalculator.UI
{
    /// <summary>
    /// Panel personalizado para visualizar forma de onda en DCM
    /// </summary>
    public class DCMVisualizationPanel : Panel
    {
        private DCMParameters parameters;

        public DCMVisualizationPanel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        public void SetParameters(DCMParameters dcmParams)
        {
            this.parameters = dcmParams;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (parameters == null)
            {
                // CORREGIDO: Método renombrado correctamente
                DrawEmptyState(e.Graphics);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawInductorCurrent(g);
            DrawModeIndicator(g);
        }

        // CORREGIDO: Nombre del método
        private void DrawEmptyState(Graphics g)
        {
            using (Font font = new Font("Arial", 10))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                string message = "No DCM analysis data to display.\nRun DCM analysis to see waveform.";
                SizeF size = g.MeasureString(message, font);
                g.DrawString(message, font, brush,
                    (this.Width - size.Width) / 2,
                    (this.Height - size.Height) / 2);
            }
        }

        private void DrawInductorCurrent(Graphics g)
        {
            int margin = 40;
            int width = this.Width - 2 * margin;
            int height = this.Height - 2 * margin - 60; // Espacio para indicador

            // Título
            using (Font titleFont = new Font("Arial", 11, FontStyle.Bold))
            {
                g.DrawString("Inductor Current Waveform", titleFont, Brushes.Black, margin, 10);
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

                for (int i = 0; i <= 4; i++)
                {
                    int y = margin + (height * i / 4);
                    g.DrawLine(gridPen, margin, y, margin + width, y);
                }

                for (int i = 0; i <= 4; i++)
                {
                    int x = margin + (width * i / 4);
                    g.DrawLine(gridPen, x, margin, x, margin + height);
                }
            }

            // Forma de onda triangular
            using (Pen wavePen = new Pen(Color.Blue, 3))
            {
                double d1 = parameters.DutyCycle;
                double d2 = parameters.DischargeDuty;
                double d3 = parameters.DeadTime;
                double iPeak = parameters.PeakInductorCurrent;

                int x0 = margin;
                int y0 = margin + height;

                int x1 = margin + (int)(width * d1);
                int y1 = margin + (int)(height * (1 - iPeak / (iPeak * 1.2)));

                int x2 = margin + (int)(width * (d1 + d2));
                int y2 = y0;

                int x3 = margin + width;
                int y3 = y0;

                // D1: Subida
                g.DrawLine(wavePen, x0, y0, x1, y1);

                // D2: Bajada
                g.DrawLine(wavePen, x1, y1, x2, y2);

                // D3: Cero
                g.DrawLine(wavePen, x2, y2, x3, y3);

                // Etiquetas de tiempos
                using (Font font = new Font("Arial", 9))
                {
                    g.DrawString($"D1={d1:F3}", font, Brushes.Blue, x0 + (x1 - x0) / 2 - 25, y0 + 10);
                    g.DrawString($"D2={d2:F3}", font, Brushes.Blue, x1 + (x2 - x1) / 2 - 25, y0 + 10);
                    g.DrawString($"D3={d3:F3}", font, Brushes.Gray, x2 + (x3 - x2) / 2 - 25, y0 + 10);
                }
            }

            // Etiquetas de corriente
            using (Font font = new Font("Arial", 8))
            {
                double iPeak = parameters.PeakInductorCurrent;

                for (int i = 0; i <= 4; i++)
                {
                    double current = iPeak * 1.2 * (4 - i) / 4;
                    int y = margin + (height * i / 4);
                    string label = $"{current:F2}A";
                    SizeF size = g.MeasureString(label, font);
                    g.DrawString(label, font, Brushes.Black, margin - size.Width - 5, y - 8);
                }
            }

            // Eje de tiempo
            using (Font font = new Font("Arial", 8))
            {
                g.DrawString("Time (one switching period)", font, Brushes.Black, margin + width / 2 - 80, margin + height + 25);
            }
        }

        private void DrawModeIndicator(Graphics g)
        {
            int y = this.Height - 50;
            int x = 40;

            Color modeColor;
            string modeText;

            switch (parameters.Mode)
            {
                case OperatingMode.DCM:
                    modeColor = Color.Orange;
                    modeText = "DCM (Discontinuous)";
                    break;
                case OperatingMode.CCM:
                    modeColor = Color.Green;
                    modeText = "CCM (Continuous)";
                    break;
                case OperatingMode.BCM:
                    modeColor = Color.Yellow;
                    modeText = "BCM (Boundary)";
                    break;
                default:
                    modeColor = Color.Gray;
                    modeText = "Unknown";
                    break;
            }

            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(modeColor))
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.FillEllipse(brush, x, y, 30, 30);
                g.DrawEllipse(pen, x, y, 30, 30);

                g.DrawString(modeText, font, Brushes.Black, x + 40, y + 5);
            }

            // Métricas adicionales
            using (Font font = new Font("Arial", 9))
            {
                string metrics = $"Peak Current: {parameters.PeakInductorCurrent:F3}A  |  " +
                               $"Avg Current: {parameters.AverageInductorCurrent:F3}A  |  " +
                               $"Ripple: {parameters.OutputRipple * 1000:F2}mV";
                g.DrawString(metrics, font, Brushes.DarkBlue, x + 250, y + 8);
            }
        }
    }
}