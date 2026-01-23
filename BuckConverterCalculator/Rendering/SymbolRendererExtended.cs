using System;
using System.Collections.Generic;
using System.Text;

namespace BuckConverterCalculator.Rendering
{
    public static class SymbolRendererExtended
    {
        // Interruptores
        public static void DrawBipolarSwitch(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawLine(pen, cx - 5, bounds.Y + 15, cx + 15, bounds.Bottom - 20);
            g.DrawLine(pen, cx - 5, bounds.Y + 20, cx + 15, bounds.Bottom - 15);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
            g.DrawEllipse(pen, cx - 3, bounds.Y + 12, 6, 6);
        }

        public static void DrawPullSwitch(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawRectangle(pen, bounds.X + 15, bounds.Y + 10, bounds.Width - 30, 15);
            g.DrawLine(pen, cx, bounds.Y + 25, cx, bounds.Y + 35);
            g.DrawLine(pen, cx, bounds.Y + 35, cx + 12, bounds.Bottom - 15);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
        }

        public static void DrawDoubleSwitch(Graphics g, Rectangle bounds, Pen pen)
        {
            int x1 = bounds.X + bounds.Width / 3;
            int x2 = bounds.X + bounds.Width * 2 / 3;

            g.DrawLine(pen, x1, bounds.Y, x1, bounds.Y + 15);
            g.DrawLine(pen, x1, bounds.Y + 15, x1 + 10, bounds.Bottom - 20);
            g.DrawLine(pen, x1, bounds.Bottom - 15, x1, bounds.Bottom);

            g.DrawLine(pen, x2, bounds.Y, x2, bounds.Y + 15);
            g.DrawLine(pen, x2, bounds.Y + 15, x2 + 10, bounds.Bottom - 20);
            g.DrawLine(pen, x2, bounds.Bottom - 15, x2, bounds.Bottom);
        }

        public static void DrawCommutator(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawLine(pen, cx - 8, bounds.Y + 25, cx + 8, bounds.Y + 25);
            g.DrawLine(pen, cx, bounds.Y + 18, cx + 10, bounds.Bottom - 20);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
            g.DrawEllipse(pen, cx - 3, bounds.Y + 12, 6, 6);
            g.DrawEllipse(pen, cx - 10, bounds.Y + 22, 6, 6);
            g.DrawEllipse(pen, cx + 4, bounds.Y + 22, 6, 6);
        }

        public static void DrawCrossCommutator(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - 10);
            g.DrawLine(pen, cx - 15, cy - 10, cx + 15, cy - 10);
            g.DrawLine(pen, cx - 15, cy + 10, cx + 15, cy + 10);
            g.DrawLine(pen, cx, cy + 10, cx, bounds.Bottom);

            g.DrawLine(pen, cx - 8, cy - 3, cx + 8, cy + 3);
            g.DrawLine(pen, cx - 8, cy + 3, cx + 8, cy - 3);
        }

        public static void DrawPushButton(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawEllipse(pen, cx - 8, bounds.Y + 15, 16, 16);
            g.DrawLine(pen, cx, bounds.Y + 31, cx, bounds.Bottom - 15);
            g.DrawLine(pen, cx - 10, bounds.Bottom - 15, cx + 10, bounds.Bottom - 15);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
        }

        public static void DrawDimmer(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawEllipse(pen, bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height - 30);
            g.DrawLine(pen, cx, bounds.Bottom - 10, cx, bounds.Bottom);

            using (Font font = new Font("Arial", 8))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("R", font, brush, cx, bounds.Y + bounds.Height / 2, sf);
            }
        }

        public static void DrawBlindSwitch(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 15, bounds.Width - 20, bounds.Height - 35);

            g.DrawLine(pen, bounds.X + bounds.Width / 3, bounds.Bottom - 20, bounds.X + bounds.Width / 3, bounds.Bottom);
            g.DrawLine(pen, bounds.X + bounds.Width * 2 / 3, bounds.Bottom - 20, bounds.X + bounds.Width * 2 / 3, bounds.Bottom);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                g.FillPolygon(brush, new Point[] {
                    new Point(cx - 6, bounds.Y + 25),
                    new Point(cx + 6, bounds.Y + 25),
                    new Point(cx, bounds.Y + 32)
                });
                g.FillPolygon(brush, new Point[] {
                    new Point(cx - 6, bounds.Bottom - 28),
                    new Point(cx + 6, bounds.Bottom - 28),
                    new Point(cx, bounds.Bottom - 35)
                });
            }
        }

        // Tomas
        public static void DrawPlug16A(Graphics g, Rectangle bounds, Pen pen)
        {
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height - 20);
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.FillEllipse(Brushes.Black, cx - 4, cy - 8, 3, 10);
            g.FillEllipse(Brushes.Black, cx + 1, cy - 8, 3, 10);
            g.FillRectangle(Brushes.Black, cx - 5, cy + 5, 10, 3);

            using (Font font = new Font("Arial", 6))
            {
                g.DrawString("16A", font, Brushes.Black, bounds.X + 12, bounds.Bottom - 18);
            }
        }

        public static void DrawPlug25A(Graphics g, Rectangle bounds, Pen pen)
        {
            g.DrawRectangle(pen, bounds.X + 8, bounds.Y + 8, bounds.Width - 16, bounds.Height - 16);
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.FillEllipse(Brushes.Black, cx - 6, cy - 10, 4, 12);
            g.FillEllipse(Brushes.Black, cx + 2, cy - 10, 4, 12);
            g.FillRectangle(Brushes.Black, cx - 7, cy + 6, 14, 4);

            using (Font font = new Font("Arial", 7, FontStyle.Bold))
            {
                g.DrawString("25A", font, Brushes.Black, bounds.X + 10, bounds.Bottom - 16);
            }
        }

        public static void DrawPlugTriphasic(Graphics g, Rectangle bounds, Pen pen)
        {
            g.DrawEllipse(pen, bounds.X + 8, bounds.Y + 8, bounds.Width - 16, bounds.Height - 16);
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            for (int i = 0; i < 3; i++)
            {
                double angle = i * Math.PI * 2 / 3 - Math.PI / 2;
                int x = cx + (int)(Math.Cos(angle) * 12);
                int y = cy + (int)(Math.Sin(angle) * 12);
                g.FillEllipse(Brushes.Black, x - 2, y - 2, 4, 4);
            }
            g.FillEllipse(Brushes.Black, cx - 2, cy + 8, 4, 4);
        }

        // Iluminación
        public static void DrawFluorescentLamp(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawRectangle(pen, bounds.X + 8, bounds.Y + 10, bounds.Width - 16, bounds.Height - 20);

            for (int i = 0; i < 3; i++)
            {
                int y = bounds.Y + 15 + i * 10;
                g.DrawLine(pen, bounds.X + 12, y, bounds.Right - 12, y);
            }
        }

        public static void DrawEmergencyLight(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 8);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 8, bounds.Width - 20, bounds.Height - 18);

            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("E", font, Brushes.Black, cx, bounds.Y + bounds.Height / 2, sf);
            }

            using (Brush brush = new SolidBrush(Color.FromArgb(100, Color.Red)))
            {
                g.FillRectangle(brush, bounds.X + 10, bounds.Y + 8, bounds.Width - 20, bounds.Height - 18);
            }
        }

        // Señalización
        public static void DrawBell(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - 5);

            Point[] bell = new Point[] {
                new Point(cx - 12, cy - 5),
                new Point(cx - 15, cy + 10),
                new Point(cx + 15, cy + 10),
                new Point(cx + 12, cy - 5)
            };
            g.DrawPolygon(pen, bell);
            g.DrawLine(pen, cx - 15, cy + 10, cx + 15, cy + 10);
            g.FillEllipse(Brushes.Black, cx - 3, cy + 2, 6, 6);
        }

        public static void DrawSiren(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - 8);
            g.DrawEllipse(pen, bounds.X + 8, bounds.Y + 15, bounds.Width - 16, bounds.Height - 25);

            for (int i = 0; i < 3; i++)
            {
                int r = 15 + i * 5;
                g.DrawArc(pen, cx - r, cy - r, r * 2, r * 2, 45, 90);
            }
        }

        // Otros símbolos normalizados continúan...
        public static void DrawCGP(Graphics g, Rectangle bounds, Pen pen)
        {
            g.DrawRectangle(pen, bounds.X + 5, bounds.Y + 5, bounds.Width - 10, bounds.Height - 10);

            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("CGP", font, brush, bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, sf);
            }
        }

        public static void DrawICP(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawRectangle(pen, bounds.X + 8, bounds.Y + 10, bounds.Width - 16, bounds.Height - 20);
            g.DrawLine(pen, cx, bounds.Bottom - 10, cx, bounds.Bottom);

            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("ICP", font, Brushes.Black, cx, bounds.Y + bounds.Height / 2, sf);
            }
        }
    }
}