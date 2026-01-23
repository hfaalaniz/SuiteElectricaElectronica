using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BuckConverterCalculator.Core;

namespace BuckConverterCalculator
{
    /// <summary>
    /// Formulario con canvas para visualizar el esquemático del Buck Converter - VERSIÓN PROFESIONAL
    /// </summary>
    public partial class SchematicViewerForm : Form
    {
        private DesignParameters parameters;
        private CalculationResults results;

        private Label lblZoom;
        private HScrollBar hScrollBar;
        private VScrollBar vScrollBar;

        private float zoomFactor = 1.0f;
        private Bitmap originalSchematic; // Guardar imagen original para zoom
        private const int CANVAS_WIDTH = 2400;
        private const int CANVAS_HEIGHT = 1400;

        // Variables para arrastre con mouse
        private bool isDragging = false;
        private Point dragStartPoint;
        private Point scrollStartPoint;

        public SchematicViewerForm(DesignParameters param, CalculationResults res)
        {
            parameters = param;
            results = res;
            InitializeComponent();
            // Establecer zoom por defecto
            cmbZoom.SelectedIndex = 3; // 100%
            DrawSchematic();
        }

        

        private void DrawSchematic()
        {
            // Canvas grande para máxima calidad
            Bitmap bitmap = new Bitmap(CANVAS_WIDTH, CANVAS_HEIGHT);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Configuración de máxima calidad
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;

                // Fondo blanco
                g.Clear(Color.White);

                // Dibujar cuadrícula sutil
                DrawGrid(g);

                // Dibujar borde exterior
                using (Pen borderPen = new Pen(Color.FromArgb(0, 51, 102), 4))
                {
                    g.DrawRectangle(borderPen, 15, 15, CANVAS_WIDTH - 30, CANVAS_HEIGHT - 30);
                }

                // Dibujar título principal
                DrawMainTitle(g);

                // Dibujar el circuito con layout profesional
                DrawProfessionalCircuit(g);

                // Dibujar leyenda y notas
                DrawLegendAndNotes(g);

                // Dibujar tabla de valores
                DrawValueTable(g);
            }

            // Guardar imagen original
            originalSchematic = bitmap;
            schemaPictureBox.Image = originalSchematic;
            UpdateZoom();
        }

        private void DrawGrid(Graphics g)
        {
            Pen gridPen = new Pen(Color.FromArgb(15, 180, 200, 220), 1);
            gridPen.DashStyle = DashStyle.Dot;

            // Cuadrícula cada 100px
            for (int x = 100; x < CANVAS_WIDTH; x += 100)
            {
                g.DrawLine(gridPen, x, 30, x, CANVAS_HEIGHT - 30);
            }

            for (int y = 100; y < CANVAS_HEIGHT; y += 100)
            {
                g.DrawLine(gridPen, 30, y, CANVAS_WIDTH - 30, y);
            }

            gridPen.Dispose();
        }

        private void DrawMainTitle(Graphics g)
        {
            // Área del título
            Rectangle titleArea = new Rectangle(30, 30, CANVAS_WIDTH - 60, 100);
            using (LinearGradientBrush titleBrush = new LinearGradientBrush(
                titleArea, Color.FromArgb(0, 51, 102), Color.FromArgb(0, 102, 204), 0f))
            {
                g.FillRectangle(titleBrush, titleArea);
            }

            using (Pen titleBorder = new Pen(Color.FromArgb(0, 51, 102), 2))
            {
                g.DrawRectangle(titleBorder, titleArea);
            }

            using (Font titleFont = new Font("Arial", 28, FontStyle.Bold))
            using (Font subtitleFont = new Font("Arial", 14))
            {
                string title = "BUCK CONVERTER - DC/DC STEP-DOWN REGULATOR";
                string subtitle = $"Input: {parameters.Vin:F1}V  →  Output: {parameters.Vout:F1}V @ {parameters.Iout:F1}A  •  " +
                                $"Power: {results.PowerOutput:F1}W  •  Efficiency: {results.ActualEfficiency:F1}%  •  Frequency: {results.ActualFrequency / 1000:F1}kHz";

                SizeF titleSize = g.MeasureString(title, titleFont);
                SizeF subtitleSize = g.MeasureString(subtitle, subtitleFont);

                g.DrawString(title, titleFont, Brushes.White,
                    (CANVAS_WIDTH - titleSize.Width) / 2, 45);
                g.DrawString(subtitle, subtitleFont, Brushes.LightYellow,
                    (CANVAS_WIDTH - subtitleSize.Width) / 2, 85);
            }
        }

        private void DrawProfessionalCircuit(Graphics g)
        {
            // Definir pens y brushes de alta calidad
            Pen powerPen = new Pen(Color.FromArgb(200, 0, 0), 4);
            Pen signalPen = new Pen(Color.FromArgb(0, 0, 200), 2.5f);
            Pen feedbackPen = new Pen(Color.FromArgb(0, 150, 0), 2.5f);
            feedbackPen.DashStyle = DashStyle.Dash;
            Pen wirePen = new Pen(Color.Black, 2.5f);
            Pen gndPen = new Pen(Color.FromArgb(0, 100, 0), 3);

            Font labelFont = new Font("Arial", 11, FontStyle.Bold);
            Font valueFont = new Font("Arial", 9);
            Font noteFont = new Font("Arial", 8, FontStyle.Italic);

            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush redBrush = new SolidBrush(Color.FromArgb(200, 0, 0));
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(0, 0, 200));
            SolidBrush greenBrush = new SolidBrush(Color.FromArgb(0, 120, 0));

            // ========================================
            // SECCIÓN 1: ENTRADA Y FILTRADO (X: 100-500)
            // ========================================

            DrawSectionLabel(g, new Point(100, 160), "ENTRADA Y FILTRO", redBrush);

            // Terminal VIN
            Point vinPoint = new Point(150, 250);
            DrawPowerTerminal(g, vinPoint, "VIN", $"{parameters.Vin:F1}V", redBrush);

            // Fusible F1
            Point f1End = new Point(270, 250);
            DrawFuse(g, new Point(200, 250), f1End, "F1", "8A / 250V");
            g.DrawLine(powerPen, vinPoint, new Point(200, 250));

            // Capacitor de entrada Cin1
            Point cin1Pos = new Point(350, 250);
            DrawElectrolyticCap(g, cin1Pos, true, "Cin1",
                $"47µF\n160V\nESR<150mΩ");
            g.DrawLine(powerPen, f1End, new Point(cin1Pos.X, 250));
            g.DrawLine(gndPen, new Point(cin1Pos.X, 310), new Point(cin1Pos.X, 350));
            DrawGround(g, new Point(cin1Pos.X, 350));

            // Bus VIN
            Point busStart = new Point(390, 250);
            Point busEnd = new Point(600, 250);
            g.DrawLine(powerPen, busStart, busEnd);
            DrawBusLabel(g, new Point((busStart.X + busEnd.X) / 2, 225), "VIN BUS", redBrush);

            // ========================================
            // SECCIÓN 2: CONMUTACIÓN DE POTENCIA (X: 600-900)
            // ========================================

            DrawSectionLabel(g, new Point(600, 160), "ETAPA DE CONMUTACIÓN", blueBrush);

            // MOSFET Q1
            Point mosfetPos = new Point(650, 300);
            DrawEnhancedMosfet(g, mosfetPos, "Q1", "IRFB4115PBF\n150V/104A\nRds=8.7mΩ");
            g.DrawLine(powerPen, busEnd, new Point(650, 250));
            g.DrawLine(powerPen, new Point(650, 250), new Point(650, 270));

            // Resistor de sense en source
            Point rsenseTop = new Point(650, 330);
            Point rsenseBot = new Point(650, 400);
            DrawPowerResistor(g, rsenseTop, rsenseBot, "Rsns",
                "0.1Ω\n2W\n1%");
            g.DrawLine(gndPen, rsenseBot, new Point(650, 450));
            DrawGround(g, new Point(650, 450));

            // Nodo SW (switching node)
            Point swNode = new Point(720, 250);
            g.DrawLine(powerPen, new Point(680, 250), swNode);
            DrawPowerNode(g, swNode, "SW", redBrush);

            // Diodo de libre circulación D1
            Point diodePos = new Point(800, 250);
            DrawSchottkyDiode(g, new Point(diodePos.X, 380),
                new Point(diodePos.X, 150), "D1", "MBR20200CT\n200V/20A\nVf=0.85V");
            // Cathode a SW
            g.DrawLine(wirePen, new Point(diodePos.X, 150), new Point(diodePos.X, 120));
            g.DrawLine(wirePen, new Point(diodePos.X, 120), new Point(720, 120));
            g.DrawLine(wirePen, new Point(720, 120), new Point(720, 250));
            // Anode a GND
            g.DrawLine(gndPen, new Point(diodePos.X, 380), new Point(diodePos.X, 420));
            DrawGround(g, new Point(diodePos.X, 420));

            // ========================================
            // SECCIÓN 3: FILTRO DE SALIDA (X: 900-1300)
            // ========================================

            DrawSectionLabel(g, new Point(900, 160), "FILTRO LC DE SALIDA", greenBrush);

            // Inductor L1
            Point indStart = new Point(720, 250);
            Point indEnd = new Point(950, 250);
            DrawPowerInductor(g, indStart, indEnd, "L1",
                $"{results.InductanceCommercial * 1e6:F0}µH\nIsat={results.PeakInductorCurrent:F1}A\nDCR=20mΩ");

            // Nodo VOUT
            Point voutNode = new Point(1030, 250);
            g.DrawLine(powerPen, indEnd, voutNode);
            DrawPowerNode(g, voutNode, "VOUT", blueBrush);

            // Capacitores de salida en paralelo
            Point cout1Pos = new Point(1120, 250);
            DrawCeramicCap(g, cout1Pos, true, "Cout1", "100µF\n25V X7R\nESR<5mΩ");
            g.DrawLine(powerPen, voutNode, new Point(cout1Pos.X, 250));
            g.DrawLine(gndPen, new Point(cout1Pos.X, 310), new Point(cout1Pos.X, 350));
            DrawGround(g, new Point(cout1Pos.X, 350));

            Point cout2Pos = new Point(1220, 250);
            DrawCeramicCap(g, cout2Pos, true, "Cout2", "100µF\n25V X7R\nESR<5mΩ");
            g.DrawLine(powerPen, new Point(cout1Pos.X, 250), new Point(cout2Pos.X, 250));
            g.DrawLine(gndPen, new Point(cout2Pos.X, 310), new Point(cout2Pos.X, 350));
            DrawGround(g, new Point(cout2Pos.X, 350));

            // Terminal de salida
            Point voutTerm = new Point(1350, 250);
            g.DrawLine(powerPen, new Point(cout2Pos.X, 250), voutTerm);
            DrawPowerTerminal(g, voutTerm, "VOUT", $"{parameters.Vout:F1}V", blueBrush);

            // Carga
            Point loadTop = new Point(1420, 250);
            Point loadBot = new Point(1420, 350);
            DrawPowerResistor(g, loadTop, loadBot, "LOAD",
                $"{parameters.Vout / parameters.Iout:F2}Ω\n{parameters.Iout:F1}A\n{results.PowerOutput:F1}W");
            g.DrawLine(wirePen, voutTerm, loadTop);
            g.DrawLine(gndPen, loadBot, new Point(1420, 390));
            DrawGround(g, new Point(1420, 390));

            // ========================================
            // SECCIÓN 4: CONTROLADOR PWM (Centro inferior)
            // ========================================

            DrawSectionLabel(g, new Point(850, 580), "CONTROLADOR PWM UC3843", new SolidBrush(Color.DarkViolet));

            // UC3843 - Centrado en la parte inferior
            Rectangle ucRect = new Rectangle(900, 650, 280, 320);
            DrawUC3843Enhanced(g, ucRect);

            // Calcular posiciones de pines (spacing de 60px)
            int pinY = ucRect.Y + 90;
            Point pin1 = new Point(ucRect.Left, pinY);           // COMP
            Point pin2 = new Point(ucRect.Left, pinY + 60);      // FB
            Point pin3 = new Point(ucRect.Left, pinY + 120);     // ISENSE
            Point pin4 = new Point(ucRect.Left, pinY + 180);     // RT/CT
            Point pin5 = new Point(ucRect.Right, pinY + 180);    // GND
            Point pin6 = new Point(ucRect.Right, pinY + 120);    // OUT
            Point pin7 = new Point(ucRect.Right, pinY + 60);     // VCC
            Point pin8 = new Point(ucRect.Right, pinY);          // VREF

            // ========================================
            // CONEXIONES DEL UC3843
            // ========================================

            // PIN 8 - VREF (2.5V)
            DrawVoltageLabel(g, new Point(pin8.X + 10, pin8.Y - 5), "2.5V", redBrush);

            // PIN 7 - VCC (+15V)
            Point vccSupply = new Point(pin7.X + 80, pin7.Y);
            g.DrawLine(wirePen, pin7, vccSupply);
            DrawVccSupply(g, vccSupply, "+15V");

            // Capacitor de desacople VCC
            Point cvccPos = new Point(vccSupply.X + 70, pin7.Y);
            DrawCeramicCap(g, cvccPos, true, "Cvcc", "100nF\n50V");
            g.DrawLine(wirePen, vccSupply, new Point(cvccPos.X, pin7.Y));
            g.DrawLine(gndPen, new Point(cvccPos.X, pin7.Y + 60), new Point(cvccPos.X, pin7.Y + 90));
            DrawGround(g, new Point(cvccPos.X, pin7.Y + 90));

            // PIN 6 - OUT (PWM hacia gate del MOSFET)
            Point pwmPath1 = new Point(pin6.X + 50, pin6.Y);
            Point pwmPath2 = new Point(pwmPath1.X, 500);
            Point pwmPath3 = new Point(500, 500);
            Point pwmPath4 = new Point(500, 300);

            g.DrawLine(signalPen, pin6, pwmPath1);
            g.DrawLine(signalPen, pwmPath1, pwmPath2);
            g.DrawLine(signalPen, pwmPath2, pwmPath3);
            g.DrawLine(signalPen, pwmPath3, pwmPath4);

            // Resistor de gate
            Point rgEnd = new Point(600, 300);
            DrawResistor(g, pwmPath4, rgEnd, "Rg", "10Ω\n1W", false);
            g.DrawLine(signalPen, rgEnd, new Point(635, 300));
            DrawSignalArrow(g, new Point(630, 300), new Point(635, 300), signalPen);

            g.DrawString("PWM Signal", noteFont, blueBrush, 510, 480);
            g.DrawString($"D={results.DutyCycle * 100:F1}%", noteFont, blueBrush, 510, 495);

            // PIN 5 - GND
            g.DrawLine(gndPen, pin5, new Point(pin5.X + 50, pin5.Y));
            DrawGround(g, new Point(pin5.X + 50, pin5.Y));

            // PIN 4 - RT/CT (Timing components)
            // Mover más a la izquierda para evitar superposición con RC
            Point rtctPoint = new Point(pin4.X - 150, pin4.Y);
            g.DrawLine(wirePen, pin4, rtctPoint);

            // Resistor Rt
            Point rtTop = new Point(rtctPoint.X, rtctPoint.Y - 100);
            DrawResistor(g, rtTop, rtctPoint, "Rt",
                $"{results.RtValue / 1000:F2}kΩ\n1%", true);

            // Conexión Rt a VCC - path más claro
            Point rtVccPath1 = new Point(rtTop.X, rtTop.Y - 30);
            Point rtVccPath2 = new Point(rtTop.X - 50, rtVccPath1.Y);
            Point rtVccPath3 = new Point(rtVccPath2.X, pin7.Y - 120);
            Point rtVccPath4 = new Point(vccSupply.X, rtVccPath3.Y);

            g.DrawLine(wirePen, rtTop, rtVccPath1);
            g.DrawLine(wirePen, rtVccPath1, rtVccPath2);
            g.DrawLine(wirePen, rtVccPath2, rtVccPath3);
            g.DrawLine(wirePen, rtVccPath3, rtVccPath4);
            g.DrawLine(wirePen, rtVccPath4, new Point(vccSupply.X, pin7.Y));

            // Etiqueta del path
            g.DrawString("→VCC", new Font("Arial", 6, FontStyle.Italic), Brushes.Red,
                rtVccPath2.X - 20, rtVccPath3.Y - 15);

            // Capacitor Ct
            Point ctPos = new Point(rtctPoint.X, rtctPoint.Y + 40);
            DrawCeramicCap(g, ctPos, true, "Ct", $"{results.CtValue * 1e9:F2}nF\nNP0");
            g.DrawLine(wirePen, rtctPoint, new Point(ctPos.X, rtctPoint.Y));
            g.DrawLine(gndPen, new Point(ctPos.X, rtctPoint.Y + 100), new Point(ctPos.X, rtctPoint.Y + 130));
            DrawGround(g, new Point(ctPos.X, rtctPoint.Y + 130));

            g.DrawString($"f={results.ActualFrequency / 1000:F1}kHz", noteFont, blackBrush,
                rtctPoint.X - 80, rtctPoint.Y - 10);

            // PIN 3 - ISENSE (Current sensing)
            Point isensePickup = new Point(650, 360);
            Point isensePath1 = new Point(400, 360);
            Point isensePath2 = new Point(400, pin3.Y);

            g.DrawLine(signalPen, isensePickup, isensePath1);
            g.DrawLine(signalPen, isensePath1, isensePath2);
            g.DrawLine(signalPen, isensePath2, pin3);
            DrawSignalArrow(g, new Point(pin3.X - 10, pin3.Y), pin3, signalPen);

            g.DrawString("Current Sense", noteFont, blueBrush, 410, 360);

            // PIN 2 - FB (Feedback from divider)
            // Divisor de realimentación R1/R2
            Point fbTop = new Point(1600, 250);
            Point fbMid = new Point(1600, 600);
            Point fbBot = new Point(1600, 750);

            g.DrawLine(wirePen, new Point(1350, 250), fbTop);

            // R1 superior
            DrawResistor(g, new Point(1600, 400), fbMid, "R1",
                $"{results.FeedbackR1 / 1000:F1}kΩ\n1%", true);
            g.DrawLine(wirePen, fbTop, new Point(1600, 400));

            DrawPowerNode(g, fbMid, "FB", greenBrush);

            // R2 inferior
            DrawResistor(g, fbMid, fbBot, "R2",
                $"{results.FeedbackR2 / 1000:F1}kΩ\n1%", true);
            g.DrawLine(gndPen, fbBot, new Point(1600, 800));
            DrawGround(g, new Point(1600, 800));

            // Conexión FB al pin 2
            Point fbPath1 = new Point(1450, fbMid.Y);
            Point fbPath2 = new Point(1450, pin2.Y);

            g.DrawLine(feedbackPen, fbMid, fbPath1);
            g.DrawLine(feedbackPen, fbPath1, fbPath2);
            g.DrawLine(feedbackPen, fbPath2, pin2);
            DrawSignalArrow(g, new Point(pin2.X - 10, pin2.Y), pin2, feedbackPen);

            g.DrawString("Feedback Loop", noteFont, greenBrush, 1460, fbMid.Y - 5);
            g.DrawString($"Vref=2.5V", noteFont, greenBrush, 1460, fbMid.Y + 10);

            // PIN 1 - COMP (Compensation network)
            // Mover hacia arriba para evitar superposición con RT
            Point compPoint = new Point(pin1.X - 120, pin1.Y);
            g.DrawLine(wirePen, pin1, compPoint);

            // Rc en serie
            Point rcBot = new Point(compPoint.X, compPoint.Y + 70);
            DrawResistor(g, compPoint, rcBot, "Rc", "10kΩ", true);

            // Cc en paralelo - mover más a la izquierda
            Point ccPos = new Point(compPoint.X - 70, compPoint.Y + 35);
            DrawCeramicCap(g, ccPos, true, "Cc", "22nF");
            g.DrawLine(wirePen, new Point(ccPos.X, compPoint.Y), new Point(compPoint.X, compPoint.Y));
            g.DrawLine(wirePen, new Point(ccPos.X, rcBot.Y), new Point(compPoint.X, rcBot.Y));

            // Conexión a FB - path más claro
            Point compPath1 = new Point(rcBot.X, rcBot.Y + 20);
            Point compPath2 = new Point(rcBot.X - 80, compPath1.Y);
            Point compPath3 = new Point(compPath2.X, pin2.Y);

            g.DrawLine(wirePen, rcBot, compPath1);
            g.DrawLine(wirePen, compPath1, compPath2);
            g.DrawLine(wirePen, compPath2, compPath3);
            g.DrawLine(wirePen, compPath3, pin2);

            g.DrawString("Compensation", noteFont, blackBrush, compPoint.X - 110, compPoint.Y + 10);
            g.DrawString("Loop Filter", new Font("Arial", 6, FontStyle.Italic), Brushes.Purple,
                compPath2.X - 35, compPath1.Y + 5);

            // ========================================
            // INDICADORES DE CORRIENTE Y VOLTAJE
            // ========================================

            DrawCurrentIndicator(g, new Point(230, 210), "Iin",
                $"{results.PowerInput / parameters.Vin:F2}A", redBrush);
            DrawCurrentIndicator(g, new Point(1250, 210), "Iout",
                $"{parameters.Iout:F2}A", blueBrush);
            DrawCurrentIndicator(g, new Point(830, 210), "IL(avg)",
                $"{parameters.Iout:F2}A", blackBrush);
            DrawCurrentIndicator(g, new Point(830, 280), "IL(pk)",
                $"{results.PeakInductorCurrent:F2}A", new SolidBrush(Color.OrangeRed));

            // Cleanup
            powerPen.Dispose();
            signalPen.Dispose();
            feedbackPen.Dispose();
            wirePen.Dispose();
            gndPen.Dispose();
            labelFont.Dispose();
            valueFont.Dispose();
            noteFont.Dispose();
            blackBrush.Dispose();
            redBrush.Dispose();
            blueBrush.Dispose();
            greenBrush.Dispose();
        }

        private void DrawSectionLabel(Graphics g, Point pos, string text, Brush brush)
        {
            using (Font font = new Font("Arial", 10, FontStyle.Bold | FontStyle.Italic))
            {
                SizeF size = g.MeasureString(text, font);
                Rectangle labelRect = new Rectangle(pos.X, pos.Y, (int)size.Width + 20, (int)size.Height + 10);

                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 240)), labelRect);
                g.DrawRectangle(new Pen(brush, 2), labelRect);
                g.DrawString(text, font, brush, pos.X + 10, pos.Y + 5);
            }
        }

        private void DrawBusLabel(Graphics g, Point pos, string text, Brush brush)
        {
            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            {
                g.DrawString(text, font, brush, pos);
            }
        }

        private void DrawVoltageLabel(Graphics g, Point pos, string text, Brush brush)
        {
            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            {
                SizeF size = g.MeasureString(text, font);
                g.FillEllipse(Brushes.LightYellow, pos.X - 5, pos.Y - 5, size.Width + 10, size.Height + 10);
                g.DrawEllipse(new Pen(brush, 1.5f), pos.X - 5, pos.Y - 5, size.Width + 10, size.Height + 10);
                g.DrawString(text, font, brush, pos);
            }
        }

        private void DrawCurrentIndicator(Graphics g, Point pos, string label, string value, Brush brush)
        {
            Pen arrowPen = new Pen(brush, 3);
            arrowPen.CustomEndCap = new AdjustableArrowCap(6, 6);
            g.DrawLine(arrowPen, pos, new Point(pos.X + 40, pos.Y));

            using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(label, labelFont, brush, pos.X + 10, pos.Y - 20);
                g.DrawString(value, valueFont, brush, pos.X + 10, pos.Y + 8);
            }

            arrowPen.Dispose();
        }

        private void DrawSignalArrow(Graphics g, Point start, Point end, Pen pen)
        {
            Pen arrowPen = (Pen)pen.Clone();
            arrowPen.CustomEndCap = new AdjustableArrowCap(5, 5);
            g.DrawLine(arrowPen, start, end);
            arrowPen.Dispose();
        }

        private void DrawPowerTerminal(Graphics g, Point pos, string label, string value, Brush brush)
        {
            int size = 16;
            g.FillEllipse(brush, pos.X - size / 2, pos.Y - size / 2, size, size);
            g.DrawEllipse(new Pen(Color.Black, 3), pos.X - size / 2 - 3, pos.Y - size / 2 - 3, size + 6, size + 6);

            using (Font labelFont = new Font("Arial", 12, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 10, FontStyle.Bold))
            {
                SizeF labelSize = g.MeasureString(label, labelFont);
                g.DrawString(label, labelFont, Brushes.Black, pos.X - labelSize.Width / 2, pos.Y - 35);
                g.DrawString(value, valueFont, brush, pos.X - 20, pos.Y + 20);
            }
        }

        private void DrawPowerNode(Graphics g, Point pos, string label, Brush brush)
        {
            g.FillEllipse(Brushes.Black, pos.X - 6, pos.Y - 6, 12, 12);
            g.DrawEllipse(new Pen(brush, 2), pos.X - 8, pos.Y - 8, 16, 16);

            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            {
                g.DrawString(label, font, brush, pos.X + 12, pos.Y - 10);
            }
        }

        // Métodos de dibujo de componentes mejorados continuarán en el siguiente mensaje...
        // (Por límite de caracteres, necesito dividir el código)

        private void DrawFuse(Graphics g, Point start, Point end, string label, string value)
        {
            Pen pen = new Pen(Color.Black, 2.5f);
            g.DrawLine(pen, start, end);

            Rectangle rect = new Rectangle(
                (start.X + end.X) / 2 - 20, start.Y - 12, 40, 24);
            g.FillRectangle(Brushes.LightYellow, rect);
            g.DrawRectangle(pen, rect);

            g.DrawLine(pen, rect.Left + 5, rect.Top + 5, rect.Right - 5, rect.Bottom - 5);
            g.DrawLine(pen, rect.Left + 10, rect.Top + 5, rect.Right - 10, rect.Bottom - 5);

            using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(label, labelFont, Brushes.Black, rect.X + 12, rect.Y - 25);
                g.DrawString(value, valueFont, Brushes.Blue, rect.X + 5, rect.Y + rect.Height + 5);
            }

            pen.Dispose();
        }

        private void DrawElectrolyticCap(Graphics g, Point pos, bool vertical, string label, string value)
        {
            if (vertical)
            {
                // Cuerpo del capacitor
                Rectangle body = new Rectangle(pos.X - 18, pos.Y, 36, 60);
                using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                    body, Color.FromArgb(50, 50, 150), Color.FromArgb(100, 100, 200), 90f))
                {
                    g.FillRectangle(bodyBrush, body);
                }
                g.DrawRectangle(new Pen(Color.Black, 2), body);

                // Placas
                Pen platePen = new Pen(Color.Black, 4);
                g.DrawLine(platePen, pos.X - 18, pos.Y, pos.X + 18, pos.Y);
                g.DrawLine(platePen, pos.X - 18, pos.Y + 60, pos.X + 18, pos.Y + 60);
                platePen.Dispose();

                // Conexiones
                Pen connPen = new Pen(Color.Black, 2.5f);
                g.DrawLine(connPen, pos.X, pos.Y - 30, pos.X, pos.Y);
                g.DrawLine(connPen, pos.X, pos.Y + 60, pos.X, pos.Y + 90);
                connPen.Dispose();

                // Marca de polaridad (+)
                Pen redPen = new Pen(Color.Red, 2.5f);
                g.DrawLine(redPen, pos.X - 28, pos.Y + 15, pos.X - 22, pos.Y + 15);
                g.DrawLine(redPen, pos.X - 25, pos.Y + 12, pos.X - 25, pos.Y + 18);
                redPen.Dispose();

                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    g.DrawString(label, labelFont, Brushes.Black, pos.X + 22, pos.Y + 15);
                    string[] lines = value.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        g.DrawString(lines[i], valueFont, Brushes.Blue, pos.X + 22, pos.Y + 30 + i * 10);
                    }
                }
            }
        }

        private void DrawCeramicCap(Graphics g, Point pos, bool vertical, string label, string value)
        {
            if (vertical)
            {
                // Placas cerámicas
                Pen platePen = new Pen(Color.FromArgb(139, 69, 19), 4);
                g.DrawLine(platePen, pos.X - 15, pos.Y, pos.X + 15, pos.Y);
                g.DrawLine(platePen, pos.X - 15, pos.Y + 60, pos.X + 15, pos.Y + 60);
                platePen.Dispose();

                // Cuerpo cerámico
                Rectangle body = new Rectangle(pos.X - 12, pos.Y + 3, 24, 54);
                g.FillRectangle(new SolidBrush(Color.FromArgb(245, 222, 179)), body);
                g.DrawRectangle(new Pen(Color.FromArgb(139, 69, 19), 1.5f), body);

                // Conexiones
                Pen connPen = new Pen(Color.Black, 2.5f);
                g.DrawLine(connPen, pos.X, pos.Y - 30, pos.X, pos.Y);
                g.DrawLine(connPen, pos.X, pos.Y + 60, pos.X, pos.Y + 90);
                connPen.Dispose();

                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    g.DrawString(label, labelFont, Brushes.Black, pos.X + 20, pos.Y + 15);
                    string[] lines = value.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        g.DrawString(lines[i], valueFont, Brushes.Blue, pos.X + 20, pos.Y + 30 + i * 10);
                    }
                }
            }
        }

        private void DrawPowerInductor(Graphics g, Point start, Point end, string label, string value)
        {
            Pen coilPen = new Pen(Color.FromArgb(184, 115, 51), 3);
            int numCoils = 6;
            int coilWidth = (end.X - start.X - 40) / numCoils;

            g.DrawLine(coilPen, start, new Point(start.X + 20, start.Y));

            for (int i = 0; i < numCoils; i++)
            {
                int x = start.X + 20 + i * coilWidth;
                g.DrawArc(coilPen, x, start.Y - 18, coilWidth, 36, 0, 180);
            }

            // CORREGIDO: Usar dos puntos en vez de cuatro argumentos
            g.DrawLine(coilPen, new Point(end.X - 10, end.Y), end);
            coilPen.Dispose();

            // Núcleo del inductor
            Pen corePen = new Pen(Color.Gray, 2);
            corePen.DashStyle = DashStyle.Dash;
            g.DrawLine(corePen, start.X + 25, start.Y - 25, end.X - 25, start.Y - 25);
            g.DrawLine(corePen, start.X + 25, start.Y + 25, end.X - 25, start.Y + 25);
            corePen.Dispose();

            using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 8))
            {
                g.DrawString(label, labelFont, Brushes.Black,
                    (start.X + end.X) / 2 - 15, start.Y - 50);
                string[] lines = value.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    g.DrawString(lines[i], valueFont, Brushes.Blue,
                        (start.X + end.X) / 2 - 30, start.Y + 30 + i * 12);
                }
            }
        }

        private void DrawEnhancedMosfet(Graphics g, Point pos, string label, string partNumber)
        {
            Pen mosfetPen = new Pen(Color.Black, 3);

            // Canal
            g.DrawLine(mosfetPen, pos.X, pos.Y - 30, pos.X, pos.Y + 30);

            // Gate con aislamiento
            g.DrawLine(mosfetPen, pos.X - 35, pos.Y, pos.X - 10, pos.Y);
            Rectangle gateOxide = new Rectangle(pos.X - 10, pos.Y - 25, 6, 50);
            g.FillRectangle(Brushes.LightBlue, gateOxide);
            g.DrawRectangle(new Pen(Color.Blue, 1.5f), gateOxide);

            // Drain
            g.DrawLine(mosfetPen, pos.X, pos.Y - 30, pos.X + 15, pos.Y - 30);
            g.DrawLine(mosfetPen, pos.X + 15, pos.Y - 30, pos.X + 15, pos.Y - 40);

            // Source
            g.DrawLine(mosfetPen, pos.X, pos.Y + 30, pos.X + 15, pos.Y + 30);
            g.DrawLine(mosfetPen, pos.X + 15, pos.Y + 30, pos.X + 15, pos.Y + 40);

            // Body diode
            Point[] triangle = {
                new Point(pos.X + 10, pos.Y - 12),
                new Point(pos.X + 28, pos.Y),
                new Point(pos.X + 10, pos.Y + 12)
            };
            g.FillPolygon(Brushes.Black, triangle);
            g.DrawLine(mosfetPen, pos.X + 28, pos.Y - 12, pos.X + 28, pos.Y + 12);

            mosfetPen.Dispose();

            using (Font labelFont = new Font("Arial", 11, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(label, labelFont, Brushes.Black, pos.X - 18, pos.Y - 65);
                string[] lines = partNumber.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    g.DrawString(lines[i], valueFont, Brushes.Blue, pos.X - 35, pos.Y - 50 + i * 10);
                }
            }
        }

        private void DrawSchottkyDiode(Graphics g, Point anode, Point cathode, string label, string partNumber)
        {
            Pen diodePen = new Pen(Color.Black, 3);
            Point mid = new Point(anode.X, (anode.Y + cathode.Y) / 2);

            // Triángulo del diodo
            Point[] triangle = {
                new Point(mid.X - 18, mid.Y + 18),
                new Point(mid.X, mid.Y - 18),
                new Point(mid.X + 18, mid.Y + 18)
            };
            g.FillPolygon(new SolidBrush(Color.FromArgb(100, 100, 100)), triangle);
            g.DrawPolygon(diodePen, triangle);

            // Barra del cátodo con marcas Schottky
            g.DrawLine(diodePen, mid.X - 18, mid.Y - 18, mid.X + 18, mid.Y - 18);
            g.DrawLine(new Pen(Color.Black, 2), mid.X - 18, mid.Y - 18, mid.X - 18, mid.Y - 25);
            g.DrawLine(new Pen(Color.Black, 2), mid.X + 18, mid.Y - 18, mid.X + 18, mid.Y - 11);

            // Conexiones
            Pen connPen = new Pen(Color.Black, 2.5f);
            g.DrawLine(connPen, cathode, new Point(mid.X, mid.Y - 18));
            g.DrawLine(connPen, new Point(mid.X, mid.Y + 18), anode);
            connPen.Dispose();
            diodePen.Dispose();

            using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(label, labelFont, Brushes.Black, mid.X + 25, mid.Y - 15);
                string[] lines = partNumber.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    g.DrawString(lines[i], valueFont, Brushes.Blue, mid.X + 25, mid.Y + i * 10);
                }
            }
        }

        private void DrawResistor(Graphics g, Point start, Point end, string label, string value, bool vertical)
        {
            Pen resPen = new Pen(Color.FromArgb(139, 90, 43), 2);

            if (vertical)
            {
                int height = end.Y - start.Y;
                Rectangle body = new Rectangle(start.X - 12, start.Y, 24, height);

                // Cuerpo del resistor con gradiente
                using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                    body, Color.FromArgb(210, 180, 140), Color.FromArgb(139, 90, 43), 0f))
                {
                    g.FillRectangle(bodyBrush, body);
                }
                g.DrawRectangle(resPen, body);

                // Bandas de código de colores
                g.DrawLine(new Pen(Color.Brown, 3), body.Left, body.Top + height / 4, body.Right, body.Top + height / 4);
                g.DrawLine(new Pen(Color.Black, 3), body.Left, body.Top + height / 2, body.Right, body.Top + height / 2);
                g.DrawLine(new Pen(Color.Gold, 3), body.Left, body.Top + 3 * height / 4, body.Right, body.Top + 3 * height / 4);

                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    g.DrawString(label, labelFont, Brushes.Black, start.X + 18, start.Y + height / 2 - 15);
                    string[] lines = value.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        g.DrawString(lines[i], valueFont, Brushes.Blue,
                            start.X + 18, start.Y + height / 2 + 5 + i * 10);
                    }
                }
            }
            else
            {
                int width = end.X - start.X;
                Rectangle body = new Rectangle(start.X, start.Y - 12, width, 24);

                using (LinearGradientBrush bodyBrush = new LinearGradientBrush(
                    body, Color.FromArgb(210, 180, 140), Color.FromArgb(139, 90, 43), 0f))
                {
                    g.FillRectangle(bodyBrush, body);
                }
                g.DrawRectangle(resPen, body);

                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    g.DrawString(label, labelFont, Brushes.Black, body.X + width / 2 - 12, body.Y - 25);
                    string[] lines = value.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        g.DrawString(lines[i], valueFont, Brushes.Blue,
                            body.X + width / 2 - 15, body.Y + body.Height + 5 + i * 10);
                    }
                }
            }

            resPen.Dispose();
        }

        private void DrawPowerResistor(Graphics g, Point start, Point end, string label, string value)
        {
            DrawResistor(g, start, end, label, value, true);
        }

        private void DrawGround(Graphics g, Point pos)
        {
            Pen gndPen = new Pen(Color.FromArgb(0, 100, 0), 3);
            g.DrawLine(gndPen, pos.X - 20, pos.Y, pos.X + 20, pos.Y);
            g.DrawLine(gndPen, pos.X - 14, pos.Y + 6, pos.X + 14, pos.Y + 6);
            g.DrawLine(gndPen, pos.X - 8, pos.Y + 12, pos.X + 8, pos.Y + 12);
            gndPen.Dispose();
        }

        private void DrawUC3843Enhanced(Graphics g, Rectangle rect)
        {
            // Cuerpo del IC con gradiente
            using (LinearGradientBrush icBrush = new LinearGradientBrush(
                rect, Color.FromArgb(40, 40, 60), Color.FromArgb(80, 80, 120), 45f))
            {
                g.FillRectangle(icBrush, rect);
            }
            g.DrawRectangle(new Pen(Color.Black, 3), rect);

            // Marca de orientación
            g.FillEllipse(Brushes.White, rect.X + 10, rect.Y + 10, 12, 12);

            using (Font titleFont = new Font("Arial", 16, FontStyle.Bold))
            using (Font subtitleFont = new Font("Arial", 9))
            using (Font pinFont = new Font("Arial", 8, FontStyle.Bold))
            {
                g.DrawString("UC3843", titleFont, Brushes.White,
                    rect.X + rect.Width / 2 - 45, rect.Y + 30);
                g.DrawString("Current Mode PWM", subtitleFont, Brushes.LightYellow,
                    rect.X + rect.Width / 2 - 60, rect.Y + 55);

                string[] leftPins = { "COMP", "FB", "ISENSE", "RT/CT" };
                string[] rightPins = { "VREF", "VCC", "OUT", "GND" };
                int pinSpacing = 60;

                for (int i = 0; i < leftPins.Length; i++)
                {
                    int y = rect.Y + 90 + i * pinSpacing;

                    // Pin metálico
                    g.FillRectangle(Brushes.Silver, rect.Left - 25, y - 3, 25, 6);
                    g.DrawRectangle(new Pen(Color.Black, 1.5f), rect.Left - 25, y - 3, 25, 6);

                    // Círculo de soldadura
                    g.FillEllipse(Brushes.White, rect.Left - 6, y - 6, 12, 12);
                    g.DrawEllipse(new Pen(Color.Black, 2), rect.Left - 6, y - 6, 12, 12);

                    // Número y nombre del pin
                    g.DrawString($"{i + 1}", pinFont, Brushes.Black, rect.Left - 45, y - 6);
                    g.DrawString(leftPins[i], pinFont, Brushes.White, rect.Left + 12, y - 6);
                }

                for (int i = 0; i < rightPins.Length; i++)
                {
                    int y = rect.Y + 90 + i * pinSpacing;

                    g.FillRectangle(Brushes.Silver, rect.Right, y - 3, 25, 6);
                    g.DrawRectangle(new Pen(Color.Black, 1.5f), rect.Right, y - 3, 25, 6);

                    g.FillEllipse(Brushes.White, rect.Right - 6, y - 6, 12, 12);
                    g.DrawEllipse(new Pen(Color.Black, 2), rect.Right - 6, y - 6, 12, 12);

                    g.DrawString($"{8 - i}", pinFont, Brushes.Black, rect.Right + 28, y - 6);
                    g.DrawString(rightPins[i], pinFont, Brushes.White, rect.Right - 85, y - 6);
                }
            }
        }

        private void DrawVccSupply(Graphics g, Point pos, string voltage)
        {
            int size = 28;
            g.FillEllipse(Brushes.Red, pos.X - size / 2, pos.Y - size / 2, size, size);
            g.DrawEllipse(new Pen(Color.DarkRed, 2.5f), pos.X - size / 2, pos.Y - size / 2, size, size);

            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(voltage, font);
                g.DrawString(voltage, font, Brushes.White,
                    pos.X - textSize.Width / 2, pos.Y - textSize.Height / 2);
            }
        }

        private void DrawLegendAndNotes(Graphics g)
        {
            int x = 100;
            int y = 1150;

            Rectangle legendBox = new Rectangle(x, y, 600, 160);
            g.FillRectangle(new SolidBrush(Color.FromArgb(250, 250, 245)), legendBox);
            g.DrawRectangle(new Pen(Color.Black, 2), legendBox);

            using (Font titleFont = new Font("Arial", 11, FontStyle.Bold))
            using (Font textFont = new Font("Arial", 9))
            {
                g.DrawString("LEYENDA Y CONVENCIONES", titleFont, Brushes.Black, x + 10, y + 10);

                int yPos = y + 35;
                int lineHeight = 22;

                g.DrawLine(new Pen(Color.FromArgb(200, 0, 0), 4), x + 15, yPos, x + 50, yPos);
                g.DrawString("= Líneas de potencia (alta corriente)", textFont, Brushes.Black, x + 60, yPos - 6);

                yPos += lineHeight;
                g.DrawLine(new Pen(Color.FromArgb(0, 0, 200), 2.5f), x + 15, yPos, x + 50, yPos);
                g.DrawString("= Señales de control PWM", textFont, Brushes.Black, x + 60, yPos - 6);

                yPos += lineHeight;
                Pen fbPen = new Pen(Color.FromArgb(0, 150, 0), 2.5f);
                fbPen.DashStyle = DashStyle.Dash;
                g.DrawLine(fbPen, x + 15, yPos, x + 50, yPos);
                g.DrawString("= Lazo de realimentación", textFont, Brushes.Black, x + 60, yPos - 6);
                fbPen.Dispose();

                yPos += lineHeight;
                g.DrawLine(new Pen(Color.FromArgb(0, 100, 0), 3), x + 15, yPos, x + 50, yPos);
                g.DrawString("= Conexión a tierra (GND)", textFont, Brushes.Black, x + 60, yPos - 6);

                yPos += lineHeight;
                g.DrawString("Todos los valores son calculados para las condiciones nominales.",
                    new Font("Arial", 8, FontStyle.Italic), Brushes.DarkBlue, x + 15, yPos);
            }

            // Notas de seguridad
            Rectangle notesBox = new Rectangle(x + 620, y, 500, 160);
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 250, 240)), notesBox);
            g.DrawRectangle(new Pen(Color.DarkRed, 2), notesBox);

            using (Font titleFont = new Font("Arial", 11, FontStyle.Bold))
            using (Font textFont = new Font("Arial", 8))
            {
                g.DrawString("⚠ NOTAS DE SEGURIDAD", titleFont, Brushes.DarkRed, notesBox.X + 10, notesBox.Y + 10);

                string[] notes = {
                    "• Verificar polaridad de capacitores electrolíticos",
                    "• Usar disipadores en Q1 y D1",
                    "• Mantener trazas cortas en nodo SW",
                    "• Aislar señales sensibles del ruido de conmutación",
                    "• Verificar ratings de voltaje con margen 1.5x",
                    "• Protección contra sobrecorriente recomendada"
                };

                int yPos = notesBox.Y + 35;
                foreach (string note in notes)
                {
                    g.DrawString(note, textFont, Brushes.Black, notesBox.X + 15, yPos);
                    yPos += 18;
                }
            }
        }

        private void DrawValueTable(Graphics g)
        {
            Rectangle tableBox = new Rectangle(1180, 1150, 1150, 160);

            using (LinearGradientBrush tableBrush = new LinearGradientBrush(
                tableBox, Color.FromArgb(240, 248, 255), Color.FromArgb(220, 235, 255), 90f))
            {
                g.FillRectangle(tableBrush, tableBox);
            }
            g.DrawRectangle(new Pen(Color.DarkBlue, 2), tableBox);

            using (Font titleFont = new Font("Arial", 11, FontStyle.Bold))
            using (Font headerFont = new Font("Consolas", 8, FontStyle.Bold))
            using (Font dataFont = new Font("Consolas", 8))
            {
                g.DrawString("PARÁMETROS CALCULADOS DEL DISEÑO", titleFont, Brushes.DarkBlue,
                    tableBox.X + 10, tableBox.Y + 8);

                int col1 = tableBox.X + 15;
                int col2 = col1 + 200;
                int col3 = col2 + 200;
                int col4 = col3 + 200;
                int col5 = col4 + 200;
                int col6 = col5 + 200;

                int row = tableBox.Y + 35;
                int rowHeight = 16;

                // Headers
                g.DrawString("ELECTRICAL", headerFont, Brushes.DarkBlue, col1, row);
                g.DrawString("POWER", headerFont, Brushes.DarkRed, col2, row);
                g.DrawString("CURRENT", headerFont, Brushes.DarkGreen, col3, row);
                g.DrawString("THERMAL", headerFont, Brushes.DarkOrange, col4, row);
                g.DrawString("TIMING", headerFont, Brushes.DarkMagenta, col5, row);
                g.DrawString("FEEDBACK", headerFont, Brushes.DarkCyan, col6, row);

                row += rowHeight + 5;

                // Valores
                g.DrawString($"Vin  = {parameters.Vin:F2} V", dataFont, Brushes.Black, col1, row);
                g.DrawString($"Pout = {results.PowerOutput:F2} W", dataFont, Brushes.Black, col2, row);
                g.DrawString($"Iout = {parameters.Iout:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"Tj(Q1) = {results.MosfetJunctionTemp:F1}°C", dataFont, Brushes.Black, col4, row);
                g.DrawString($"f = {results.ActualFrequency / 1000:F2} kHz", dataFont, Brushes.Black, col5, row);
                g.DrawString($"Vref = 2.500 V", dataFont, Brushes.Black, col6, row);

                row += rowHeight;
                g.DrawString($"Vout = {parameters.Vout:F2} V", dataFont, Brushes.Black, col1, row);
                g.DrawString($"Pin  = {results.PowerInput:F2} W", dataFont, Brushes.Black, col2, row);
                g.DrawString($"IL(avg) = {parameters.Iout:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"Tj(D1) = {results.DiodeJunctionTemp:F1}°C", dataFont, Brushes.Black, col4, row);
                g.DrawString($"D = {results.DutyCycle * 100:F2} %", dataFont, Brushes.Black, col5, row);
                g.DrawString($"Vfb = {results.VoutVerified:F3} V", dataFont, Brushes.Black, col6, row);

                row += rowHeight;
                g.DrawString($"ΔV = {parameters.RippleVoltageMv:F0} mV", dataFont, Brushes.Black, col1, row);
                g.DrawString($"Ploss = {results.TotalLosses:F2} W", dataFont, Brushes.Black, col2, row);
                g.DrawString($"IL(pk) = {results.PeakInductorCurrent:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"T(L) = {results.InductorTemp:F1}°C", dataFont, Brushes.Black, col4, row);
                g.DrawString($"Rt = {results.RtValue / 1000:F2} kΩ", dataFont, Brushes.Black, col5, row);
                g.DrawString($"R1 = {results.FeedbackR1 / 1000:F1} kΩ", dataFont, Brushes.Black, col6, row);

                row += rowHeight;
                g.DrawString($"L = {results.InductanceCommercial * 1e6:F0} µH", dataFont, Brushes.Black, col1, row);
                g.DrawString($"η = {results.ActualEfficiency:F2} %", dataFont, Brushes.Black, col2, row);
                g.DrawString($"IL(rms) = {results.RmsInductorCurrent:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"Pq = {results.MosfetTotalLoss:F2} W", dataFont, Brushes.Black, col4, row);
                g.DrawString($"Ct = {results.CtValue * 1e9:F2} nF", dataFont, Brushes.Black, col5, row);
                g.DrawString($"R2 = {results.FeedbackR2 / 1000:F1} kΩ", dataFont, Brushes.Black, col6, row);

                row += rowHeight;
                g.DrawString($"Cout = {results.OutputCapacitanceCommercial * 1e6:F0} µF", dataFont, Brushes.Black, col1, row);
                g.DrawString($"Pin/Pout = {results.PowerInput / results.PowerOutput:F3}", dataFont, Brushes.Black, col2, row);
                g.DrawString($"ΔIL = {results.RippleCurrent:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"Pd = {results.DiodeConductionLoss:F2} W", dataFont, Brushes.Black, col4, row);
                g.DrawString($"Tsw = {1 / results.ActualFrequency * 1e6:F2} µs", dataFont, Brushes.Black, col5, row);
                g.DrawString($"R1/R2 = {results.FeedbackR1 / results.FeedbackR2:F2}", dataFont, Brushes.Black, col6, row);

                row += rowHeight;
                g.DrawString($"ESR = {results.MaxEsr * 1000:F2} mΩ", dataFont, Brushes.Black, col1, row);
                g.DrawString($"", dataFont, Brushes.Black, col2, row);
                g.DrawString($"Ic(rms) = {results.RmsCapacitorCurrent:F3} A", dataFont, Brushes.Black, col3, row);
                g.DrawString($"Pl = {results.InductorCoreLoss:F2} W", dataFont, Brushes.Black, col4, row);
                g.DrawString($"", dataFont, Brushes.Black, col5, row);
                g.DrawString($"Gain = {results.VoutVerified / 2.5:F3}", dataFont, Brushes.Black, col6, row);
            }
        }

        private void UpdateZoom()
        {
            if (originalSchematic == null) return;

            // Calcular nuevo tamaño basado en el zoom
            int newWidth = (int)(CANVAS_WIDTH * zoomFactor);
            int newHeight = (int)(CANVAS_HEIGHT * zoomFactor);

            // Si el zoom es 100%, usar la imagen original sin escalar
            if (Math.Abs(zoomFactor - 1.0f) < 0.01f)
            {
                // Limpiar imagen escalada anterior si existe
                if (schemaPictureBox.Image != originalSchematic && schemaPictureBox.Image != null)
                {
                    schemaPictureBox.Image.Dispose();
                }

                schemaPictureBox.Image = originalSchematic;
                schemaPictureBox.Size = new Size(CANVAS_WIDTH, CANVAS_HEIGHT);
                canvasPanel.AutoScrollMinSize = new Size(CANVAS_WIDTH, CANVAS_HEIGHT);
                return;
            }

            // Limpiar imagen escalada anterior si existe
            if (schemaPictureBox.Image != originalSchematic && schemaPictureBox.Image != null)
            {
                schemaPictureBox.Image.Dispose();
            }

            // Crear imagen escalada con alta calidad
            Bitmap scaledImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(originalSchematic, 0, 0, newWidth, newHeight);
            }

            // Actualizar PictureBox
            schemaPictureBox.Image = scaledImage;
            schemaPictureBox.Size = new Size(newWidth, newHeight);

            // Actualizar scroll
            canvasPanel.AutoScrollMinSize = new Size(newWidth, newHeight);
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbZoom.SelectedItem != null)
            {
                string selectedZoom = cmbZoom.SelectedItem.ToString();
                zoomFactor = float.Parse(selectedZoom.Replace("%", "")) / 100f;
                UpdateZoom();
            }
        }

        private void btnExportImage_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp",
                    DefaultExt = "png",
                    FileName = $"BuckConverter_Professional_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Guardar imagen original (sin zoom, máxima calidad)
                    originalSchematic.Save(saveDialog.FileName);
                    MessageBox.Show($"Esquemático exportado exitosamente.\n\nResolución: {CANVAS_WIDTH}×{CANVAS_HEIGHT} px",
                        "Exportación Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar imagen:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ========================================
        // EVENT HANDLERS PARA ARRASTRE CON MOUSE
        // ========================================

        private void schemaPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
                scrollStartPoint = new Point(
                    canvasPanel.HorizontalScroll.Value,
                    canvasPanel.VerticalScroll.Value
                );

                // Cambiar cursor a mano cerrada
                schemaPictureBox.Cursor = Cursors.Hand;
            }
        }

        private void schemaPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Calcular desplazamiento
                int deltaX = dragStartPoint.X - e.Location.X;
                int deltaY = dragStartPoint.Y - e.Location.Y;

                // Aplicar nuevo scroll
                int newScrollX = scrollStartPoint.X + deltaX;
                int newScrollY = scrollStartPoint.Y + deltaY;

                // Limitar a los valores válidos del scroll
                if (newScrollX < 0) newScrollX = 0;
                if (newScrollY < 0) newScrollY = 0;
                if (newScrollX > canvasPanel.HorizontalScroll.Maximum)
                    newScrollX = canvasPanel.HorizontalScroll.Maximum;
                if (newScrollY > canvasPanel.VerticalScroll.Maximum)
                    newScrollY = canvasPanel.VerticalScroll.Maximum;

                // Establecer nuevas posiciones de scroll
                canvasPanel.AutoScrollPosition = new Point(newScrollX, newScrollY);
            }
            else
            {
                // Cursor normal cuando no está arrastrando
                schemaPictureBox.Cursor = Cursors.Default;
            }
        }

        private void schemaPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                schemaPictureBox.Cursor = Cursors.Default;
            }
        }
    }
}



