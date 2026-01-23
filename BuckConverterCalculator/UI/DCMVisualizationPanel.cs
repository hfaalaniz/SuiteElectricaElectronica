using BuckConverterCalculator.Analysis;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator.UI
{
    public class DCMVisualizationPanel : Panel
    {
        private DCMParameters parameters;

        
        public DCMVisualizationPanel()
        {
            this.AutoScroll = true;
            this.BackColor = Color.White;
        }
        public void SetParameters(DCMParameters paramsDCM)
        {
            this.parameters = paramsDCM;
            this.Invalidate();
        }

        // Analizar codigo comentado si se desea agregar más visualizaciones
        public void UpdateVisualization(DCMParameters paramsDCM)
        {
            this.parameters = paramsDCM;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (parameters == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dibujar forma de onda del inductor en DCM
            DrawInductorCurrent(g);
            // Analizar codigo comentado si se desea agregar más visualizaciones
            //DrawOutputVoltage(g);
            DrawModeIndicator(g);
        }

        private void DrawInductorCurrent(Graphics g)
        {
            int width = this.Width - 40;
            int height = 150;
            int startX = 20;
            int startY = 30;

            // Ejes
            using (Pen axisPen = new Pen(Color.Black, 1))
            {
                g.DrawLine(axisPen, startX, startY + height, startX + width, startY + height);
                g.DrawLine(axisPen, startX, startY, startX, startY + height);
            }

            // Forma de onda DCM
            using (Pen wavePen = new Pen(Color.Blue, 2))
            {
                int d1Width = (int)(width * parameters.DutyCycle);
                int d2Width = (int)(width * parameters.DischargeDuty);
                int d3Width = width - d1Width - d2Width;

                // D1: Rampa ascendente
                g.DrawLine(wavePen,
                    startX, startY + height,
                    startX + d1Width, startY);

                // D2: Rampa descendente
                g.DrawLine(wavePen,
                    startX + d1Width, startY,
                    startX + d1Width + d2Width, startY + height);

                // D3: Cero (tiempo muerto)
                g.DrawLine(wavePen,
                    startX + d1Width + d2Width, startY + height,
                    startX + width, startY + height);

                // Etiquetas
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString($"D1={parameters.DutyCycle:F3}", font, Brushes.Black,
                        startX + d1Width / 2 - 20, startY + height + 5);
                    g.DrawString($"D2={parameters.DischargeDuty:F3}", font, Brushes.Black,
                        startX + d1Width + d2Width / 2 - 20, startY + height + 5);
                    g.DrawString($"D3={parameters.DeadTime:F3}", font, Brushes.Black,
                        startX + d1Width + d2Width + d3Width / 2 - 20, startY + height + 5);
                }
            }

            // Etiqueta de título
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            {
                g.DrawString("Inductor Current (DCM)", titleFont, Brushes.Black, startX, startY - 20);
            }
        }

        private void DrawModeIndicator(Graphics g)
        {
            string modeText = parameters.Mode == OperatingMode.DCM ?
                "DCM - Discontinuous Conduction Mode" :
                "CCM - Continuous Conduction Mode";

            Color modeColor = parameters.Mode == OperatingMode.DCM ?
                Color.Orange : Color.Green;

            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            using (Brush brush = new SolidBrush(modeColor))
            {
                g.DrawString(modeText, font, brush, 20, this.Height - 40);
            }
        }
    }
}