using BuckConverterCalculator.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;

namespace BuckConverterCalculator.Rendering
{
    public static class SymbolRenderer
    {
        public static void DrawSymbol(Graphics g, ElectricalSymbol symbol)
        {
            Rectangle bounds = symbol.GetBounds();

            using (Pen pen = new Pen(symbol.IsSelected ? Color.Red : Color.Black, 2))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                switch (symbol.Type)
                {
                    case SymbolType.Breaker:
                        DrawBreaker(g, bounds, pen);
                        break;
                    case SymbolType.Contactor:
                        DrawContactor(g, bounds, pen, symbol.Poles);
                        break;
                    case SymbolType.Motor:
                        DrawMotor(g, bounds, pen);
                        break;
                    case SymbolType.Transformer:
                        DrawTransformer(g, bounds, pen);
                        break;
                    case SymbolType.Fuse:
                        DrawFuse(g, bounds, pen);
                        break;
                    case SymbolType.Switch:
                        DrawSwitch(g, bounds, pen);
                        break;
                    case SymbolType.Load:
                        DrawLoad(g, bounds, pen);
                        break;
                    case SymbolType.Outlet:
                        DrawOutlet(g, bounds, pen);
                        break;
                    case SymbolType.LightBulb:
                        DrawLightBulb(g, bounds, pen);
                        break;
                    case SymbolType.Lamp:
                        DrawLamp(g, bounds, pen);
                        break;
                    case SymbolType.DifferentialBreaker:
                        DrawDifferentialBreaker(g, bounds, pen);
                        break;
                    case SymbolType.ThermalBreaker:
                        DrawThermalBreaker(g, bounds, pen);
                        break;
                    case SymbolType.DistributionPanel:
                        DrawDistributionPanel(g, bounds, pen);
                        break;
                    case SymbolType.Meter:
                        DrawMeter(g, bounds, pen);
                        break;
                    case SymbolType.Fan:
                        DrawFan(g, bounds, pen);
                        break;
                    case SymbolType.WaterHeater:
                        DrawWaterHeater(g, bounds, pen);
                        break;
                    case SymbolType.AirConditioner:
                        DrawAirConditioner(g, bounds, pen);
                        break;
                    case SymbolType.Doorbell:
                        DrawDoorbell(g, bounds, pen);
                        break;
                    case SymbolType.GroundConnection:
                        DrawGroundConnection(g, bounds, pen);
                        break;
                    // Nuevos símbolos normalizados
                    case SymbolType.BipolarSwitch:
                        SymbolRendererExtended.DrawBipolarSwitch(g, bounds, pen);
                        break;
                    case SymbolType.PullSwitch:
                        SymbolRendererExtended.DrawPullSwitch(g, bounds, pen);
                        break;
                    case SymbolType.DoubleSwitch:
                        SymbolRendererExtended.DrawDoubleSwitch(g, bounds, pen);
                        break;
                    case SymbolType.Commutator:
                        SymbolRendererExtended.DrawCommutator(g, bounds, pen);
                        break;
                    case SymbolType.CrossCommutator:
                        SymbolRendererExtended.DrawCrossCommutator(g, bounds, pen);
                        break;
                    case SymbolType.PushButton:
                        SymbolRendererExtended.DrawPushButton(g, bounds, pen);
                        break;
                    case SymbolType.Dimmer:
                        SymbolRendererExtended.DrawDimmer(g, bounds, pen);
                        break;
                    case SymbolType.BlindSwitch:
                        SymbolRendererExtended.DrawBlindSwitch(g, bounds, pen);
                        break;
                    case SymbolType.Plug16A:
                        SymbolRendererExtended.DrawPlug16A(g, bounds, pen);
                        break;
                    case SymbolType.Plug25A:
                        SymbolRendererExtended.DrawPlug25A(g, bounds, pen);
                        break;
                    case SymbolType.PlugTriphasic:
                        SymbolRendererExtended.DrawPlugTriphasic(g, bounds, pen);
                        break;
                    case SymbolType.FluorescentLamp:
                        SymbolRendererExtended.DrawFluorescentLamp(g, bounds, pen);
                        break;
                    case SymbolType.EmergencyLight:
                        SymbolRendererExtended.DrawEmergencyLight(g, bounds, pen);
                        break;
                    case SymbolType.Bell:
                        SymbolRendererExtended.DrawBell(g, bounds, pen);
                        break;
                    case SymbolType.Siren:
                        SymbolRendererExtended.DrawSiren(g, bounds, pen);
                        break;
                    case SymbolType.CGP:
                        SymbolRendererExtended.DrawCGP(g, bounds, pen);
                        break;
                    case SymbolType.ICP:
                        SymbolRendererExtended.DrawICP(g, bounds, pen);
                        break;
                }

                DrawConnectionPoints(g, symbol);
            }

            using (Font font = new Font("Arial", 8))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                g.DrawString(symbol.Name, font, brush, bounds.X, bounds.Bottom + 2);
            }
        }

        private static void DrawBreaker(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawRectangle(pen, bounds.X + 15, bounds.Y + 15, bounds.Width - 30, bounds.Height - 30);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
        }

        private static void DrawContactor(Graphics g, Rectangle bounds, Pen pen, int poles)
        {
            int spacing = bounds.Width / (poles + 1);
            for (int i = 0; i < poles; i++)
            {
                int x = bounds.X + spacing * (i + 1);
                g.DrawLine(pen, x, bounds.Y, x, bounds.Y + 15);
                g.DrawLine(pen, x - 10, bounds.Y + 15, x + 10, bounds.Y + 15);
                g.DrawLine(pen, x - 8, bounds.Y + 20, x + 8, bounds.Bottom - 20);
                g.DrawLine(pen, x - 10, bounds.Bottom - 15, x + 10, bounds.Bottom - 15);
                g.DrawLine(pen, x, bounds.Bottom - 15, x, bounds.Bottom);
            }
        }

        private static void DrawMotor(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 2 - 5;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - radius);
            g.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);

            using (Font font = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("M", font, brush, cx, cy, sf);
            }
        }

        private static void DrawTransformer(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int r = bounds.Width / 4;

            g.DrawLine(pen, bounds.X + 15, bounds.Y, bounds.X + 15, bounds.Y + 15);
            g.DrawLine(pen, bounds.Right - 15, bounds.Y, bounds.Right - 15, bounds.Y + 15);

            g.DrawEllipse(pen, bounds.X + 5, bounds.Y + 10, r * 2, bounds.Height - 20);
            g.DrawEllipse(pen, bounds.Right - 5 - r * 2, bounds.Y + 10, r * 2, bounds.Height - 20);

            g.DrawLine(pen, bounds.X + 15, bounds.Bottom - 15, bounds.X + 15, bounds.Bottom);
            g.DrawLine(pen, bounds.Right - 15, bounds.Bottom - 15, bounds.Right - 15, bounds.Bottom);
        }

        private static void DrawFuse(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height - 20);
            g.DrawLine(pen, cx, bounds.Bottom - 10, cx, bounds.Bottom);
        }

        private static void DrawSwitch(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawLine(pen, cx, bounds.Y + 15, cx + 15, bounds.Bottom - 20);
            g.DrawLine(pen, cx, bounds.Bottom - 15, cx, bounds.Bottom);
            g.DrawEllipse(pen, cx - 3, bounds.Y + 12, 6, 6);
            g.DrawEllipse(pen, cx - 3, bounds.Bottom - 18, 6, 6);
        }

        private static void DrawLoad(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - 5);

            Point[] zigzag = new Point[]
            {
                new Point(cx, cy - 15),
                new Point(cx + 8, cy - 10),
                new Point(cx - 8, cy - 5),
                new Point(cx + 8, cy),
                new Point(cx - 8, cy + 5),
                new Point(cx + 8, cy + 10),
                new Point(cx, cy + 15)
            };
            g.DrawLines(pen, zigzag);
        }

        private static void DrawOutlet(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height - 20);
            g.DrawLine(pen, cx - 8, cy - 8, cx - 8, cy + 8);
            g.DrawLine(pen, cx + 8, cy - 8, cx + 8, cy + 8);
            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
        }

        private static void DrawLightBulb(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 3;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - radius);
            g.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);

            g.DrawLine(pen, cx - 6, cy + radius, cx + 6, cy + radius);
            g.DrawLine(pen, cx - 4, cy + radius + 3, cx + 4, cy + radius + 3);
        }

        private static void DrawLamp(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 15);
            g.DrawEllipse(pen, bounds.X + 10, bounds.Y + 15, bounds.Width - 20, bounds.Height - 25);

            using (Brush brush = new SolidBrush(Color.FromArgb(100, Color.Yellow)))
            {
                g.FillEllipse(brush, bounds.X + 10, bounds.Y + 15, bounds.Width - 20, bounds.Height - 25);
            }
        }

        private static void DrawDifferentialBreaker(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 12);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 12, bounds.Width - 20, bounds.Height - 24);
            g.DrawLine(pen, cx, bounds.Bottom - 12, cx, bounds.Bottom);

            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("ID", font, brush, cx, bounds.Y + bounds.Height / 2, sf);
            }
        }

        private static void DrawThermalBreaker(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 12);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 12, bounds.Width - 20, bounds.Height - 24);
            g.DrawLine(pen, cx, bounds.Bottom - 12, cx, bounds.Bottom);

            int cy = bounds.Y + bounds.Height / 2;
            g.DrawLine(pen, cx - 10, cy - 8, cx + 10, cy + 8);
        }

        private static void DrawDistributionPanel(Graphics g, Rectangle bounds, Pen pen)
        {
            g.DrawRectangle(pen, bounds.X + 5, bounds.Y + 5, bounds.Width - 10, bounds.Height - 10);

            for (int i = 0; i < 3; i++)
            {
                int y = bounds.Y + 15 + i * 12;
                g.DrawRectangle(pen, bounds.X + 10, y, bounds.Width - 20, 8);
            }

            using (Font font = new Font("Arial", 7, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                g.DrawString("TD", font, brush, bounds.X + 8, bounds.Y + 8);
            }
        }

        private static void DrawMeter(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawEllipse(pen, bounds.X + 8, bounds.Y + 8, bounds.Width - 16, bounds.Height - 16);

            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("kWh", font, brush, cx, cy, sf);
            }
        }

        private static void DrawFan(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;
            int radius = Math.Min(bounds.Width, bounds.Height) / 2 - 8;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - radius);
            g.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);

            for (int i = 0; i < 3; i++)
            {
                double angle = i * Math.PI * 2 / 3;
                int x1 = cx + (int)(Math.Cos(angle) * radius * 0.3);
                int y1 = cy + (int)(Math.Sin(angle) * radius * 0.3);
                int x2 = cx + (int)(Math.Cos(angle) * radius * 0.9);
                int y2 = cy + (int)(Math.Sin(angle) * radius * 0.9);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private static void DrawWaterHeater(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 10);
            g.DrawRectangle(pen, bounds.X + 10, bounds.Y + 10, bounds.Width - 20, bounds.Height - 20);

            using (Pen wavePen = new Pen(Color.Blue, 1.5f))
            {
                int waveY = bounds.Y + bounds.Height / 2;
                for (int x = bounds.X + 15; x < bounds.Right - 15; x += 6)
                {
                    g.DrawLine(wavePen, x, waveY - 3, x + 3, waveY + 3);
                    g.DrawLine(wavePen, x + 3, waveY + 3, x + 6, waveY - 3);
                }
            }
        }

        private static void DrawAirConditioner(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, bounds.Y + 8);
            g.DrawRectangle(pen, bounds.X + 8, bounds.Y + 8, bounds.Width - 16, bounds.Height - 16);

            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Blue))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("AC", font, brush, cx, bounds.Y + bounds.Height / 2, sf);
            }

            int y1 = bounds.Y + bounds.Height / 2 + 10;
            g.DrawLine(pen, bounds.X + 15, y1, bounds.Right - 15, y1);
        }

        private static void DrawDoorbell(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy - 8);
            g.DrawEllipse(pen, cx - 10, cy - 10, 20, 20);

            using (Brush brush = new SolidBrush(Color.Yellow))
            {
                g.FillEllipse(brush, cx - 4, cy - 4, 8, 8);
            }

            g.DrawArc(pen, cx - 15, cy - 20, 30, 25, 200, 140);
            g.DrawArc(pen, cx - 18, cy - 23, 36, 28, 200, 140);
        }

        private static void DrawGroundConnection(Graphics g, Rectangle bounds, Pen pen)
        {
            int cx = bounds.X + bounds.Width / 2;
            int cy = bounds.Y + bounds.Height / 2;

            g.DrawLine(pen, cx, bounds.Y, cx, cy);
            g.DrawLine(pen, cx - 15, cy, cx + 15, cy);
            g.DrawLine(pen, cx - 10, cy + 5, cx + 10, cy + 5);
            g.DrawLine(pen, cx - 5, cy + 10, cx + 5, cy + 10);
        }

        private static void DrawConnectionPoints(Graphics g, ElectricalSymbol symbol)
        {
            using (Brush brush = new SolidBrush(Color.Blue))
            {
                foreach (var cp in symbol.ConnectionPoints)
                {
                    Point pos = cp.GetAbsolutePosition();
                    g.FillEllipse(brush, pos.X - 3, pos.Y - 3, 6, 6);
                }
            }
        }
    }
}