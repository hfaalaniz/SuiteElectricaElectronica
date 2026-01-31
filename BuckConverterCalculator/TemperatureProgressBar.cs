using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public enum TemperatureOrientation
    {
        Horizontal,
        Vertical
    }

    public partial class TemperatureProgressBar : UserControl
    {
        private float _temperature = 0f;
        private float _minTemp = 0f;
        private float _maxTemp = 250f;
        private TemperatureOrientation _orientation = TemperatureOrientation.Horizontal;
        private bool _showScale = true;
        private bool _enableMouseControl = true;
        private bool _isDragging = false;
        private int _borderRadius = 12;
        private ToolTip _toolTip;
        private bool _showTooltip = true;
        private System.Windows.Forms.Timer _blinkTimer;
        private bool _blinkState = true;
        private bool _enableBlinkOnRed = true;
        private float? _blinkThreshold = null;

        [Category("Temperature")]
        [Description("Temperatura actual en grados Celsius")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Temperature
        {
            get => _temperature;
            set
            {
                float newTemp = Math.Max(_minTemp, Math.Min(_maxTemp, value));
                if (_temperature != newTemp)
                {
                    _temperature = newTemp;
                    OnTemperatureChanged(EventArgs.Empty);
                    UpdateBlinkTimer();
                    Invalidate();
                }
            }
        }

        [Category("Temperature")]
        [Description("Temperatura mínima en grados Celsius")]
        [DefaultValue(0f)]
        public float MinTemperature
        {
            get => _minTemp;
            set
            {
                if (_minTemp != value && value < _maxTemp)
                {
                    _minTemp = value;
                    if (_temperature < _minTemp)
                        Temperature = _minTemp;
                    UpdateBlinkTimer();
                    Invalidate();
                }
            }
        }

        [Category("Temperature")]
        [Description("Temperatura máxima en grados Celsius")]
        [DefaultValue(250f)]
        public float MaxTemperature
        {
            get => _maxTemp;
            set
            {
                if (_maxTemp != value && value > _minTemp)
                {
                    _maxTemp = value;
                    if (_temperature > _maxTemp)
                        Temperature = _maxTemp;
                    UpdateBlinkTimer();
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Radio de las esquinas redondeadas")]
        [DefaultValue(12)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (_borderRadius != value && value >= 0)
                {
                    _borderRadius = value;
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Mostrar tooltip con temperatura al pasar el mouse")]
        [DefaultValue(true)]
        public bool ShowTooltip
        {
            get => _showTooltip;
            set
            {
                if (_showTooltip != value)
                {
                    _showTooltip = value;
                    if (!value && _toolTip != null)
                    {
                        _toolTip.Hide(this);
                    }
                }
            }
        }

        [Category("Behavior")]
        [Description("Habilita parpadeo de la etiqueta cuando la temperatura está en zona roja")]
        [DefaultValue(true)]
        public bool EnableBlinkOnRed
        {
            get => _enableBlinkOnRed;
            set
            {
                if (_enableBlinkOnRed != value)
                {
                    _enableBlinkOnRed = value;
                    UpdateBlinkTimer();
                    Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [Description("Temperatura en °C a partir de la cual comienza el parpadeo. Si no se establece, usa 48% del rango (zona roja).")]
        public float? BlinkThreshold
        {
            get => _blinkThreshold;
            set
            {
                if (_blinkThreshold != value)
                {
                    _blinkThreshold = value;
                    UpdateBlinkTimer();
                    Invalidate();
                }
            }
        }

        // Método para serialización del Designer
        private bool ShouldSerializeBlinkThreshold()
        {
            return _blinkThreshold.HasValue;
        }

        // Método para resetear en el Designer
        private void ResetBlinkThreshold()
        {
            _blinkThreshold = null;
        }

        [Category("Temperature")]
        [Description("Orientación de la barra de temperatura")]
        [DefaultValue(TemperatureOrientation.Horizontal)]
        public TemperatureOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    Invalidate();
                }
            }
        }

        [Category("Temperature")]
        [Description("Mostrar escala de temperatura")]
        [DefaultValue(true)]
        public bool ShowScale
        {
            get => _showScale;
            set
            {
                if (_showScale != value)
                {
                    _showScale = value;
                    Invalidate();
                }
            }
        }

        [Category("Temperature")]
        [Description("Permitir control con el mouse")]
        [DefaultValue(true)]
        public bool EnableMouseControl
        {
            get => _enableMouseControl;
            set => _enableMouseControl = value;
        }

        [Category("Temperature")]
        [Description("Se dispara cuando cambia la temperatura")]
        public event EventHandler TemperatureChanged;

        protected virtual void OnTemperatureChanged(EventArgs e)
        {
            TemperatureChanged?.Invoke(this, e);
        }

        public TemperatureProgressBar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

            // Inicializar ToolTip
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 0,
                ReshowDelay = 0,
                ShowAlways = true,
                UseAnimation = false,
                UseFading = false
            };

            // Inicializar timer de parpadeo
            _blinkTimer = new System.Windows.Forms.Timer
            {
                Interval = 500 // Parpadeo cada 500ms
            };
            _blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            _blinkState = !_blinkState;
            Invalidate();
        }

        private void UpdateBlinkTimer()
        {
            if (!_enableBlinkOnRed)
            {
                _blinkTimer.Stop();
                _blinkState = true;
                return;
            }

            bool shouldBlink = false;

            // Si BlinkThreshold está establecido, usar ese valor (prevalencia sobre hardcoded)
            if (_blinkThreshold.HasValue)
            {
                shouldBlink = _temperature >= _blinkThreshold.Value;
            }
            else
            {
                // Valor hardcodeado por defecto: 48% del rango (zona roja)
                float range = _maxTemp - _minTemp;
                float normalized = (_temperature - _minTemp) / range;
                shouldBlink = normalized >= 0.48f;
            }

            if (shouldBlink)
            {
                if (!_blinkTimer.Enabled)
                {
                    _blinkTimer.Start();
                    _blinkState = true;
                }
            }
            else
            {
                _blinkTimer.Stop();
                _blinkState = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_orientation == TemperatureOrientation.Horizontal)
                DrawHorizontal(g);
            else
                DrawVertical(g);
        }

        private void DrawHorizontal(Graphics g)
        {
            int scaleHeight = _showScale ? 18 : 0;
            int barHeight = Height - scaleHeight;

            // Calcular porcentaje de llenado
            float percentage = (_temperature - _minTemp) / (_maxTemp - _minTemp);
            int fillWidth = (int)(Width * percentage);

            // Dibujar sombra
            using (GraphicsPath shadowPath = GetRoundedRect(2, 2, Width - 4, barHeight - 4, _borderRadius))
            {
                using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                {
                    shadowBrush.CenterColor = Color.FromArgb(40, 0, 0, 0);
                    shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Dibujar fondo con borde redondeado
            using (GraphicsPath bgPath = GetRoundedRect(0, 0, Width, barHeight, _borderRadius))
            {
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, Width, barHeight),
                    Color.FromArgb(245, 245, 248),
                    Color.FromArgb(235, 235, 240),
                    90f))
                {
                    g.FillPath(bgBrush, bgPath);
                }

                // Borde interno sutil
                using (Pen innerBorder = new Pen(Color.FromArgb(30, 255, 255, 255), 1.5f))
                {
                    g.DrawPath(innerBorder, bgPath);
                }
            }

            // Dibujar barra de progreso con gradiente
            if (fillWidth > _borderRadius)
            {
                using (GraphicsPath fillPath = GetRoundedRect(0, 0, fillWidth, barHeight, _borderRadius))
                {
                    using (LinearGradientBrush gradientBrush = CreateTemperatureGradient(
                        new Rectangle(0, 0, fillWidth, barHeight), true))
                    {
                        g.FillPath(gradientBrush, fillPath);
                    }

                    // Efecto glass/brillo en la parte superior
                    using (GraphicsPath glassPath = GetRoundedRect(0, 0, fillWidth, barHeight / 2, _borderRadius))
                    {
                        using (LinearGradientBrush glassBrush = new LinearGradientBrush(
                            new Rectangle(0, 0, fillWidth, barHeight / 2),
                            Color.FromArgb(60, 255, 255, 255),
                            Color.FromArgb(10, 255, 255, 255),
                            90f))
                        {
                            g.FillPath(glassBrush, glassPath);
                        }
                    }

                    // Borde de la barra de progreso
                    using (Pen progressBorder = new Pen(Color.FromArgb(40, 0, 0, 0), 1f))
                    {
                        g.DrawPath(progressBorder, fillPath);
                    }
                }
            }

            // Dibujar borde exterior
            using (GraphicsPath borderPath = GetRoundedRect(0, 0, Width, barHeight, _borderRadius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 205), 2f))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }

            // Dibujar texto de temperatura con estilo moderno
            using (Font mainFont = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Font unitFont = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                SizeF mainSize = g.MeasureString(_temperature.ToString("F1"), mainFont);
                SizeF unitSize = g.MeasureString("°C", unitFont);

                float totalWidth = mainSize.Width + unitSize.Width;
                float x = (Width - totalWidth) / 2;
                float y = (barHeight - mainSize.Height) / 2;

                // Verificar si debe parpadear
                bool shouldDraw = true;
                if (_enableBlinkOnRed)
                {
                    bool inBlinkZone = false;

                    if (_blinkThreshold.HasValue)
                    {
                        inBlinkZone = _temperature >= _blinkThreshold.Value;
                    }
                    else
                    {
                        float range = _maxTemp - _minTemp;
                        float normalized = (_temperature - _minTemp) / range;
                        inBlinkZone = normalized >= 0.48f;
                    }

                    if (inBlinkZone && !_blinkState)
                    {
                        shouldDraw = false;
                    }
                }

                if (shouldDraw)
                {
                    // Sombra del texto
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.DrawString(_temperature.ToString("F1"), mainFont, shadowBrush, x + 1, y + 1);
                        g.DrawString("°C", unitFont, shadowBrush, x + mainSize.Width + 1, y + 1);
                    }

                    // Texto principal con color basado en temperatura
                    Color textColor = GetTextColorForTemperature(_temperature);
                    using (SolidBrush textBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(_temperature.ToString("F1"), mainFont, textBrush, x, y);
                        g.DrawString("°C", unitFont, textBrush, x + mainSize.Width, y);
                    }
                }
            }

            // Dibujar escala
            if (_showScale)
            {
                DrawHorizontalScale(g, barHeight);
            }
        }

        private void DrawVertical(Graphics g)
        {
            int scaleWidth = _showScale ? 35 : 0;
            int barWidth = Width - scaleWidth;

            // Calcular porcentaje de llenado (desde abajo)
            float percentage = (_temperature - _minTemp) / (_maxTemp - _minTemp);
            int fillHeight = (int)(Height * percentage);
            int startY = Height - fillHeight;

            // Dibujar sombra
            using (GraphicsPath shadowPath = GetRoundedRect(2, 2, barWidth - 4, Height - 4, _borderRadius))
            {
                using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                {
                    shadowBrush.CenterColor = Color.FromArgb(40, 0, 0, 0);
                    shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Dibujar fondo con borde redondeado
            using (GraphicsPath bgPath = GetRoundedRect(0, 0, barWidth, Height, _borderRadius))
            {
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, barWidth, Height),
                    Color.FromArgb(235, 235, 240),
                    Color.FromArgb(245, 245, 248),
                    0f))
                {
                    g.FillPath(bgBrush, bgPath);
                }

                // Borde interno sutil
                using (Pen innerBorder = new Pen(Color.FromArgb(30, 255, 255, 255), 1.5f))
                {
                    g.DrawPath(innerBorder, bgPath);
                }
            }

            // Dibujar barra de progreso
            if (fillHeight > _borderRadius)
            {
                int adjustedStartY = Math.Max(0, startY);
                int adjustedHeight = Height - adjustedStartY;

                using (GraphicsPath fillPath = GetRoundedRect(0, adjustedStartY, barWidth, adjustedHeight, _borderRadius))
                {
                    using (LinearGradientBrush gradientBrush = CreateTemperatureGradient(
                        new Rectangle(0, adjustedStartY, barWidth, adjustedHeight), false))
                    {
                        g.FillPath(gradientBrush, fillPath);
                    }

                    // Efecto glass
                    using (GraphicsPath glassPath = GetRoundedRect(0, adjustedStartY, barWidth / 2, adjustedHeight, _borderRadius))
                    {
                        using (LinearGradientBrush glassBrush = new LinearGradientBrush(
                            new Rectangle(0, adjustedStartY, barWidth / 2, adjustedHeight),
                            Color.FromArgb(60, 255, 255, 255),
                            Color.FromArgb(10, 255, 255, 255),
                            0f))
                        {
                            g.FillPath(glassBrush, glassPath);
                        }
                    }

                    // Borde de progreso
                    using (Pen progressBorder = new Pen(Color.FromArgb(40, 0, 0, 0), 1f))
                    {
                        g.DrawPath(progressBorder, fillPath);
                    }
                }
            }

            // Dibujar borde exterior
            using (GraphicsPath borderPath = GetRoundedRect(0, 0, barWidth, Height, _borderRadius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 205), 2f))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }

            // Dibujar texto rotado
            using (Font mainFont = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Font unitFont = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                SizeF mainSize = g.MeasureString(_temperature.ToString("F1"), mainFont);
                SizeF unitSize = g.MeasureString("°C", unitFont);

                g.TranslateTransform(barWidth / 2, Height / 2);
                g.RotateTransform(-90);

                // Verificar si debe parpadear
                bool shouldDraw = true;
                if (_enableBlinkOnRed)
                {
                    bool inBlinkZone = false;

                    if (_blinkThreshold.HasValue)
                    {
                        inBlinkZone = _temperature >= _blinkThreshold.Value;
                    }
                    else
                    {
                        float range = _maxTemp - _minTemp;
                        float normalized = (_temperature - _minTemp) / range;
                        inBlinkZone = normalized >= 0.48f;
                    }

                    if (inBlinkZone && !_blinkState)
                    {
                        shouldDraw = false;
                    }
                }

                if (shouldDraw)
                {
                    Color textColor = GetTextColorForTemperature(_temperature);

                    // Sombra
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.DrawString(_temperature.ToString("F1"), mainFont, shadowBrush, -mainSize.Width / 2 + 1, -mainSize.Height / 2 + 1);
                        g.DrawString("°C", unitFont, shadowBrush, mainSize.Width / 2 + 1, -unitSize.Height / 2 + 1);
                    }

                    // Texto
                    using (SolidBrush textBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(_temperature.ToString("F1"), mainFont, textBrush, -mainSize.Width / 2, -mainSize.Height / 2);
                        g.DrawString("°C", unitFont, textBrush, mainSize.Width / 2, -unitSize.Height / 2);
                    }
                }

                g.ResetTransform();
            }

            // Dibujar escala
            if (_showScale)
            {
                DrawVerticalScale(g, barWidth);
            }
        }

        private GraphicsPath GetRoundedRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2;

            path.AddArc(x, y, diameter, diameter, 180, 90);
            path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
            path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private Color GetTextColorForTemperature(float temp)
        {
            float range = _maxTemp - _minTemp;
            float normalized = (temp - _minTemp) / range;

            if (normalized < 0.24f)
                return Color.FromArgb(0, 80, 200); // Azul oscuro
            else if (normalized < 0.48f)
                return Color.FromArgb(200, 150, 0); // Amarillo oscuro
            else
                return Color.FromArgb(180, 0, 0); // Rojo oscuro
        }

        private void DrawHorizontalScale(Graphics g, int barHeight)
        {
            int scaleY = barHeight + 2;
            using (Font font = new Font("Segoe UI", 6f, FontStyle.Regular))
            using (Pen tickPen = new Pen(Color.FromArgb(180, 180, 185), 1f))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(100, 100, 105)))
            {
                // Calcular número de marcas basado en el rango
                float range = _maxTemp - _minTemp;
                int numMarks = 24;
                float step = range / (numMarks - 1);

                for (int i = 0; i < numMarks; i++)
                {
                    float temp = _minTemp + (step * i);
                    float x = (temp - _minTemp) / range * Width;

                    // Línea de marca más corta
                    g.DrawLine(tickPen, x, barHeight, x, scaleY + 3);

                    // Etiqueta
                    string label = temp.ToString("F0") + "°";
                    SizeF labelSize = g.MeasureString(label, font);

                    g.DrawString(label, font, textBrush, x - labelSize.Width / 2, scaleY + 4);
                }
            }
        }

        private void DrawVerticalScale(Graphics g, int barWidth)
        {
            int scaleX = barWidth + 2;
            using (Font font = new Font("Segoe UI", 6.5f, FontStyle.Regular))
            using (Pen tickPen = new Pen(Color.FromArgb(180, 180, 185), 1f))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(100, 100, 105)))
            {
                // Calcular número de marcas basado en el rango
                float range = _maxTemp - _minTemp;
                int numMarks = 24;
                float step = range / (numMarks - 1);

                for (int i = 0; i < numMarks; i++)
                {
                    float temp = _minTemp + (step * i);
                    float y = Height - ((temp - _minTemp) / range * Height);

                    // Línea de marca más corta
                    g.DrawLine(tickPen, barWidth, y, scaleX + 3, y);

                    // Etiqueta
                    string label = temp.ToString("F0") + "°";
                    SizeF labelSize = g.MeasureString(label, font);

                    g.DrawString(label, font, textBrush, scaleX + 4, y - labelSize.Height / 2);
                }
            }
        }

        private LinearGradientBrush CreateTemperatureGradient(Rectangle rect, bool isHorizontal)
        {
            LinearGradientMode mode = isHorizontal ?
                LinearGradientMode.Horizontal :
                LinearGradientMode.Vertical;

            LinearGradientBrush brush = new LinearGradientBrush(
                rect, Color.Blue, Color.Red, mode);

            ColorBlend colorBlend = new ColorBlend();

            // Para vertical, invertir el orden de colores (de abajo hacia arriba)
            if (isHorizontal)
            {
                colorBlend.Colors = new Color[]
                {
                    Color.FromArgb(0, 100, 255),      // Azul inicial (0°C)
                    Color.FromArgb(0, 150, 255),      // Azul medio (30°C)
                    Color.FromArgb(100, 200, 255),    // Azul claro (60°C)
                    Color.FromArgb(255, 255, 0),      // Amarillo (90°C)
                    Color.FromArgb(255, 200, 0),      // Naranja amarillento (120°C)
                    Color.FromArgb(255, 100, 0),      // Naranja (170°C)
                    Color.FromArgb(255, 0, 0),        // Rojo (220°C)
                    Color.FromArgb(200, 0, 0)         // Rojo oscuro (250°C)
                };

                colorBlend.Positions = new float[]
                {
                    0.0f,     // 0°C
                    0.12f,    // 30°C
                    0.24f,    // 60°C
                    0.36f,    // 90°C
                    0.48f,    // 120°C
                    0.68f,    // 170°C
                    0.88f,    // 220°C
                    1.0f      // 250°C
                };
            }
            else
            {
                // Para vertical, invertir (de abajo = 0°C a arriba = 250°C)
                colorBlend.Colors = new Color[]
                {
                    Color.FromArgb(200, 0, 0),        // Rojo oscuro (250°C) - arriba
                    Color.FromArgb(255, 0, 0),        // Rojo (220°C)
                    Color.FromArgb(255, 100, 0),      // Naranja (170°C)
                    Color.FromArgb(255, 200, 0),      // Naranja amarillento (120°C)
                    Color.FromArgb(255, 255, 0),      // Amarillo (90°C)
                    Color.FromArgb(100, 200, 255),    // Azul claro (60°C)
                    Color.FromArgb(0, 150, 255),      // Azul medio (30°C)
                    Color.FromArgb(0, 100, 255)       // Azul inicial (0°C) - abajo
                };

                colorBlend.Positions = new float[]
                {
                    0.0f,     // arriba
                    0.12f,
                    0.32f,
                    0.52f,
                    0.64f,
                    0.76f,
                    0.88f,
                    1.0f      // abajo
                };
            }

            brush.InterpolationColors = colorBlend;
            return brush;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_enableMouseControl && e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                UpdateTemperatureFromMouse(e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && _enableMouseControl)
            {
                UpdateTemperatureFromMouse(e.Location);
            }

            // Actualizar tooltip con temperatura en la posición actual del mouse
            if (_showTooltip)
            {
                float tempAtMouse = GetTemperatureAtPoint(e.Location);
                if (!float.IsNaN(tempAtMouse))
                {
                    string tooltipText = $"{tempAtMouse:F1}°C";

                    // Obtener posición del tooltip relativa al control
                    Point tooltipPos = new Point(e.X + 15, e.Y - 30);

                    // Mostrar tooltip en la posición del cursor
                    _toolTip.Show(tooltipText, this, tooltipPos, 500);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Ocultar tooltip al salir del control
            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
        }

        private float GetTemperatureAtPoint(Point location)
        {
            if (_orientation == TemperatureOrientation.Horizontal)
            {
                int scaleHeight = _showScale ? 18 : 0;
                int barHeight = Height - scaleHeight;

                // Verificar si está dentro del área de la barra
                if (location.Y < 0 || location.Y > barHeight || location.X < 0 || location.X > Width)
                    return float.NaN;

                float percentage = Math.Max(0, Math.Min(1, (float)location.X / Width));
                return _minTemp + (percentage * (_maxTemp - _minTemp));
            }
            else
            {
                int scaleWidth = _showScale ? 35 : 0;
                int barWidth = Width - scaleWidth;

                // Verificar si está dentro del área de la barra
                if (location.X < 0 || location.X > barWidth || location.Y < 0 || location.Y > Height)
                    return float.NaN;

                // Invertir Y (abajo = min, arriba = max)
                float percentage = Math.Max(0, Math.Min(1, 1f - ((float)location.Y / Height)));
                return _minTemp + (percentage * (_maxTemp - _minTemp));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip?.Dispose();
                _blinkTimer?.Stop();
                _blinkTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateTemperatureFromMouse(Point location)
        {
            float newTemp;

            if (_orientation == TemperatureOrientation.Horizontal)
            {
                int scaleHeight = _showScale ? 18 : 0;
                int barHeight = Height - scaleHeight;

                if (location.Y < 0 || location.Y > barHeight)
                    return;

                float percentage = Math.Max(0, Math.Min(1, (float)location.X / Width));
                newTemp = _minTemp + (percentage * (_maxTemp - _minTemp));
            }
            else
            {
                int scaleWidth = _showScale ? 35 : 0;
                int barWidth = Width - scaleWidth;

                if (location.X < 0 || location.X > barWidth)
                    return;

                // Invertir Y (abajo = min, arriba = max)
                float percentage = Math.Max(0, Math.Min(1, 1f - ((float)location.Y / Height)));
                newTemp = _minTemp + (percentage * (_maxTemp - _minTemp));
            }

            Temperature = newTemp;
        }
    }
}
