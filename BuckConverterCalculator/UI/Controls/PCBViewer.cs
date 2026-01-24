using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuckConverterCalculator.PCB;

namespace BuckConverterCalculator.UI.Controls
{
    /// <summary>
    /// Control para visualizar PCB layout
    /// </summary>
    public class PCBViewer : UserControl
    {
        private PCBLayout layout;
        private float zoomLevel = 1.0f;
        private Point panOffset = Point.Empty;
        private Point lastMousePos;
        private bool isPanning = false;

        private const float MM_TO_PIXELS = 5.0f; // 5 pixels per mm

        public PCBViewer()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 30);

            this.MouseWheel += PCBViewer_MouseWheel;
            this.MouseDown += PCBViewer_MouseDown;
            this.MouseMove += PCBViewer_MouseMove;
            this.MouseUp += PCBViewer_MouseUp;
        }

        public void SetLayout(PCBLayout pcbLayout)
        {
            this.layout = pcbLayout;
            this.zoomLevel = 1.0f;
            this.panOffset = Point.Empty;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (layout == null)
            {
                DrawEmptyState(e.Graphics);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Apply zoom and pan
            g.TranslateTransform(panOffset.X, panOffset.Y);
            g.ScaleTransform(zoomLevel, zoomLevel);

            // Center the board
            float boardWidth = (float)layout.Width * MM_TO_PIXELS;
            float boardHeight = (float)layout.Height * MM_TO_PIXELS;
            float offsetX = (this.Width / zoomLevel - boardWidth) / 2;
            float offsetY = (this.Height / zoomLevel - boardHeight) / 2;
            g.TranslateTransform(offsetX, offsetY);

            // Draw PCB board
            DrawBoard(g);

            // Draw traces (bottom layer)
            DrawTraces(g);

            // Draw components
            DrawComponents(g);

            // Draw grid
            DrawGrid(g);

            // Reset transform for UI elements
            g.ResetTransform();

            // Draw info overlay
            DrawInfoOverlay(g);
        }

        private void DrawEmptyState(Graphics g)
        {
            using (Font font = new Font("Arial", 12))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                string message = "No PCB layout to display.\nGenerate a layout to visualize it here.";
                SizeF size = g.MeasureString(message, font);
                g.DrawString(message, font, brush,
                    (this.Width - size.Width) / 2,
                    (this.Height - size.Height) / 2);
            }
        }

        private void DrawBoard(Graphics g)
        {
            float width = (float)layout.Width * MM_TO_PIXELS;
            float height = (float)layout.Height * MM_TO_PIXELS;

            // Board substrate (FR4 green)
            using (Brush boardBrush = new SolidBrush(Color.FromArgb(34, 139, 34)))
            {
                g.FillRectangle(boardBrush, 0, 0, width, height);
            }

            // Board outline
            using (Pen outlinePen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(outlinePen, 0, 0, width, height);
            }
        }

        private void DrawGrid(Graphics g)
        {
            float width = (float)layout.Width * MM_TO_PIXELS;
            float height = (float)layout.Height * MM_TO_PIXELS;
            float gridSize = 5 * MM_TO_PIXELS; // 5mm grid

            using (Pen gridPen = new Pen(Color.FromArgb(50, 255, 255, 255), 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // Vertical lines
                for (float x = gridSize; x < width; x += gridSize)
                {
                    g.DrawLine(gridPen, x, 0, x, height);
                }

                // Horizontal lines
                for (float y = gridSize; y < height; y += gridSize)
                {
                    g.DrawLine(gridPen, 0, y, width, y);
                }
            }
        }

        private void DrawComponents(Graphics g)
        {
            if (layout.Components == null) return;

            using (Brush componentBrush = new SolidBrush(Color.FromArgb(192, 192, 192)))
            using (Pen componentPen = new Pen(Color.Black, 1))
            using (Font font = new Font("Arial", 6))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                foreach (var component in layout.Components)
                {
                    float x = (float)component.X * MM_TO_PIXELS;
                    float y = (float)component.Y * MM_TO_PIXELS;

                    // Get component size based on footprint
                    SizeF size = GetFootprintSize(component.Footprint);

                    // Draw component rectangle
                    RectangleF rect = new RectangleF(
                        x - size.Width / 2,
                        y - size.Height / 2,
                        size.Width,
                        size.Height);

                    g.FillRectangle(componentBrush, rect);
                    g.DrawRectangle(componentPen, rect.X, rect.Y, rect.Width, rect.Height);

                    // Draw pads
                    DrawComponentPads(g, component, rect);

                    // Draw reference designator
                    SizeF textSize = g.MeasureString(component.Name, font);
                    g.DrawString(component.Name, font, textBrush,
                        x - textSize.Width / 2,
                        y - textSize.Height / 2);
                }
            }
        }

        private void DrawComponentPads(Graphics g, PCBComponent component, RectangleF rect)
        {
            using (Brush padBrush = new SolidBrush(Color.Gold))
            {
                float padSize = 2;

                // Simple 2-pad footprint for most components
                if (component.Type == "Resistor" || component.Type == "Capacitor" ||
                    component.Type == "Inductor" || component.Type == "Diode")
                {
                    // Left pad
                    g.FillEllipse(padBrush,
                        rect.Left - padSize / 2,
                        rect.Top + rect.Height / 2 - padSize / 2,
                        padSize, padSize);

                    // Right pad
                    g.FillEllipse(padBrush,
                        rect.Right - padSize / 2,
                        rect.Top + rect.Height / 2 - padSize / 2,
                        padSize, padSize);
                }
                else if (component.Type == "MOSFET" || component.Type == "IC")
                {
                    // 4+ pads for transistors and ICs
                    int padCount = 8;
                    float spacing = rect.Width / (padCount / 2 + 1);

                    for (int i = 0; i < padCount / 2; i++)
                    {
                        // Top pads
                        g.FillEllipse(padBrush,
                            rect.Left + spacing * (i + 1) - padSize / 2,
                            rect.Top - padSize / 2,
                            padSize, padSize);

                        // Bottom pads
                        g.FillEllipse(padBrush,
                            rect.Left + spacing * (i + 1) - padSize / 2,
                            rect.Bottom - padSize / 2,
                            padSize, padSize);
                    }
                }
            }
        }

        private void DrawTraces(Graphics g)
        {
            if (layout.Traces == null) return;

            using (Pen tracePen = new Pen(Color.FromArgb(184, 115, 51), 1.5f))
            {
                tracePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                tracePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                foreach (var trace in layout.Traces)
                {
                    if (trace.Points != null && trace.Points.Count > 1)
                    {
                        var points = new PointF[trace.Points.Count];
                        for (int i = 0; i < trace.Points.Count; i++)
                        {
                            points[i] = new PointF(
                                trace.Points[i].X * MM_TO_PIXELS,
                                trace.Points[i].Y * MM_TO_PIXELS);
                        }

                        g.DrawLines(tracePen, points);
                    }
                    else
                    {
                        // Simple line from start to end
                        g.DrawLine(tracePen,
                            (float)trace.StartX * MM_TO_PIXELS,
                            (float)trace.StartY * MM_TO_PIXELS,
                            (float)trace.EndX * MM_TO_PIXELS,
                            (float)trace.EndY * MM_TO_PIXELS);
                    }
                }
            }
        }

        private void DrawInfoOverlay(Graphics g)
        {
            if (layout == null) return;

            using (Font font = new Font("Arial", 9))
            using (Brush brush = new SolidBrush(Color.White))
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                string[] info = new string[]
                {
                    $"Board: {layout.Width}mm x {layout.Height}mm",
                    $"Layers: {layout.Layers}",
                    $"Components: {layout.Components?.Count ?? 0}",
                    $"Traces: {layout.Traces?.Count ?? 0}",
                    $"Zoom: {zoomLevel * 100:F0}%",
                    "Mouse wheel: Zoom | Right-drag: Pan"
                };

                float y = 10;
                foreach (var line in info)
                {
                    SizeF size = g.MeasureString(line, font);
                    g.FillRectangle(bgBrush, 8, y - 2, size.Width + 4, size.Height);
                    g.DrawString(line, font, brush, 10, y);
                    y += size.Height + 2;
                }
            }
        }

        private SizeF GetFootprintSize(string footprint)
        {
            // Approximate sizes in mm converted to pixels
            switch (footprint)
            {
                case "0603":
                    return new SizeF(1.6f * MM_TO_PIXELS, 0.8f * MM_TO_PIXELS);
                case "0805":
                    return new SizeF(2.0f * MM_TO_PIXELS, 1.25f * MM_TO_PIXELS);
                case "1206":
                    return new SizeF(3.2f * MM_TO_PIXELS, 1.6f * MM_TO_PIXELS);
                case "SOT-23":
                    return new SizeF(3.0f * MM_TO_PIXELS, 1.4f * MM_TO_PIXELS);
                case "SOIC-8":
                    return new SizeF(5.0f * MM_TO_PIXELS, 4.0f * MM_TO_PIXELS);
                case "TO-252":
                    return new SizeF(6.5f * MM_TO_PIXELS, 6.0f * MM_TO_PIXELS);
                case "SMD_5x5":
                    return new SizeF(5.0f * MM_TO_PIXELS, 5.0f * MM_TO_PIXELS);
                default:
                    return new SizeF(5.0f * MM_TO_PIXELS, 3.0f * MM_TO_PIXELS);
            }
        }

        private void PCBViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta > 0 ? 1.1f : 0.9f;
            float newZoom = zoomLevel * delta;

            // Limit zoom
            if (newZoom >= 0.1f && newZoom <= 10.0f)
            {
                zoomLevel = newZoom;
                this.Invalidate();
            }
        }

        private void PCBViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isPanning = true;
                lastMousePos = e.Location;
                this.Cursor = Cursors.Hand;
            }
        }

        private void PCBViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                panOffset.X += dx;
                panOffset.Y += dy;

                lastMousePos = e.Location;
                this.Invalidate();
            }
        }

        private void PCBViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isPanning = false;
                this.Cursor = Cursors.Default;
            }
        }

        public void ResetView()
        {
            zoomLevel = 1.0f;
            panOffset = Point.Empty;
            this.Invalidate();
        }
    }
}