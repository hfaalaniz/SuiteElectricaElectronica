using BuckConverterCalculator.SchematicEditor;
using BuckConverterDesign;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Tipo de componente en el esquemático
    /// </summary>
    public enum ComponentType
    {
        Terminal,
        Resistor,
        Capacitor,
        Inductor,
        Diode,
        Mosfet,
        IC,
        Fuse,
        Ground,
        VccSupply,
        Wire,
        Node,
        Label,
        // Nuevos componentes
        Buzzer,
        BridgeDiode,
        SchottkyDiode,
        ZenerDiode,
        OpAmp,
        LogicGate,
        FlipFlop,
        Inverter,
        LED,
        LED7Segment,
        Optocoupler,
        Potentiometer,
        Battery,
        Triac,
        Thyristor,
        Transformer,
        BJT
    }

    /// <summary>
    /// Tipo de pin
    /// </summary>
    public enum PinType
    {
        Input,
        Output,
        Bidirectional,
        Power,
        Ground,
        NoConnect,
        Passive
    }

    /// <summary>
    /// Pin de conexión de un componente
    /// </summary>
    [Serializable]
    public class ComponentPin
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public PinType Type { get; set; }
        public Point Position { get; set; }

        public ComponentPin(int number, string name, PinType type, Point position)
        {
            Number = number;
            Name = name;
            Type = type;
            Position = position;
        }

        public ComponentPin()
        {
            Number = 0;
            Name = "";
            Type = PinType.Passive;
            Position = Point.Empty;
        }

        /// <summary>
        /// Dibuja el pin con su número y tipo
        /// </summary>
        public void Draw(Graphics g, bool showNumber, bool showType)
        {
            // Dibujar punto de conexión
            using (Brush pinBrush = new SolidBrush(GetPinColor()))
            {
                g.FillEllipse(pinBrush, Position.X - 3, Position.Y - 3, 6, 6);
            }

            // Dibujar número si está habilitado
            if (showNumber)
            {
                using (Font pinFont = new Font("Arial", 6, FontStyle.Bold))
                {
                    string pinText = Number.ToString();
                    SizeF textSize = g.MeasureString(pinText, pinFont);
                    g.DrawString(pinText, pinFont, Brushes.Black,
                        Position.X - textSize.Width / 2,
                        Position.Y - textSize.Height - 5);
                }
            }

            // Dibujar tipo si está habilitado
            if (showType && !string.IsNullOrEmpty(Name))
            {
                using (Font typeFont = new Font("Arial", 5))
                {
                    g.DrawString(Name, typeFont, Brushes.DarkGray,
                        Position.X + 5,
                        Position.Y - 2);
                }
            }
        }

        private Color GetPinColor()
        {
            switch (Type)
            {
                case PinType.Input:
                    return Color.Green;
                case PinType.Output:
                    return Color.Red;
                case PinType.Bidirectional:
                    return Color.Blue;
                case PinType.Power:
                    return Color.Orange;
                case PinType.Ground:
                    return Color.Black;
                case PinType.NoConnect:
                    return Color.Gray;
                case PinType.Passive:
                default:
                    return Color.DarkBlue;
            }
        }
    }

    /// <summary>
    /// Orientación del componente
    /// </summary>
    public enum ComponentOrientation
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Ángulo de rotación del componente
    /// </summary>
    public enum RotationAngle
    {
        Rotate0 = 0,
        Rotate90 = 90,
        Rotate180 = 180,
        Rotate270 = 270
    }

    /// <summary>
    /// Tipo de capacitor
    /// </summary>
    public enum CapacitorType
    {
        [Description("Electrolytic (Polarized)")]
        Electrolytic,

        [Description("Ceramic (Non-polarized)")]
        Ceramic,

        [Description("Tantalum (Polarized)")]
        Tantalum,

        [Description("Film (Non-polarized)")]
        Film,

        [Description("Polymer (Polarized)")]
        Polymer,

        [Description("Mica (Non-polarized)")]
        Mica,

        [Description("Paper (Non-polarized)")]
        Paper,

        [Description("Supercapacitor (Polarized)")]
        Supercapacitor
    }

    /// <summary>
    /// Clase base para todos los componentes del esquemático
    /// </summary>
    [Serializable]
    public abstract class SchematicComponent
    {
        [Category("Identity")]
        [Description("Unique identifier for this component")]
        public string Id { get; set; }

        [Category("Identity")]
        [Description("Component name/reference (e.g., R1, C1, Q1)")]
        public string Name { get; set; }

        [Category("Position")]
        [Description("X coordinate on canvas")]
        public int X { get; set; }

        [Category("Position")]
        [Description("Y coordinate on canvas")]
        public int Y { get; set; }

        [Category("Appearance")]
        [Description("Component type")]
        public ComponentType Type { get; protected set; }

        [Category("Appearance")]
        [Description("Component orientation (Horizontal/Vertical)")]
        public virtual ComponentOrientation Orientation { get; set; }

        [Category("Appearance")]
        [Description("Rotation angle (0°, 90°, 180°, 270°)")]
        public RotationAngle Rotation { get; set; }

        [Category("Appearance")]
        [Description("Mirror/flip component horizontally")]
        public bool MirrorHorizontal { get; set; }

        [Category("Appearance")]
        [Description("Mirror/flip component vertically")]
        public bool MirrorVertical { get; set; }

        [Category("State")]
        [Browsable(false)]
        public bool IsSelected { get; set; }

        [Category("State")]
        [Browsable(false)]
        public bool IsHovered { get; set; }

        [Browsable(false)]
        public Rectangle Bounds { get; set; }

        protected SchematicComponent()
        {
            Id = Guid.NewGuid().ToString();
            IsSelected = false;
            IsHovered = false;
            Orientation = ComponentOrientation.Horizontal;
            Rotation = RotationAngle.Rotate0;
            MirrorHorizontal = false;
            MirrorVertical = false;
        }

        public abstract void DrawInternal(Graphics g, bool isSelected, bool isHovered);

        /// <summary>
        /// Dibuja el componente aplicando transformaciones de rotación y espejo
        /// </summary>
        public void Draw(Graphics g, bool isSelected, bool isHovered)
        {
            var state = ApplyTransformations(g);
            DrawInternal(g, isSelected, isHovered);
            g.Restore(state);
        }

        public abstract bool HitTest(Point point);
        public abstract SchematicComponent Clone();

        public virtual void Move(int deltaX, int deltaY)
        {
            X += deltaX;
            Y += deltaY;
            UpdateBounds();
        }

        /// <summary>
        /// Actualiza las posiciones de los pines del componente.
        /// Los componentes que tienen pines deben sobrescribir este método.
        /// </summary>
        protected virtual void UpdatePinPositions()
        {
            // Implementación vacía por defecto
            // Los componentes con pines sobrescriben este método
        }

        /// <summary>
        /// Rota el componente 90° en sentido horario
        /// </summary>
        public virtual void RotateClockwise()
        {
            Rotation = (RotationAngle)(((int)Rotation + 90) % 360);
            UpdateBounds();
        }

        /// <summary>
        /// Alterna el estado de espejo horizontal del componente
        /// </summary>
        public virtual void ToggleMirrorHorizontal()
        {
            MirrorHorizontal = !MirrorHorizontal;
            UpdateBounds();
        }

        /// <summary>
        /// Alterna el estado de espejo vertical del componente
        /// </summary>
        public virtual void ToggleMirrorVertical()
        {
            MirrorVertical = !MirrorVertical;
            UpdateBounds();
        }

        /// <summary>
        /// Aplica las transformaciones de rotación y espejo al contexto gráfico
        /// Retorna el estado guardado que debe restaurarse después
        /// </summary>
        protected System.Drawing.Drawing2D.GraphicsState ApplyTransformations(Graphics g)
        {
            // Guardar estado original
            var state = g.Save();

            // Trasladar al punto de origen del componente
            g.TranslateTransform(X, Y);

            // Aplicar rotación en sentido horario
            if (Rotation != RotationAngle.Rotate0)
            {
                g.RotateTransform((float)Rotation);
            }

            // Aplicar espejo horizontal (flip en X)
            if (MirrorHorizontal)
            {
                g.ScaleTransform(-1, 1);
            }

            // Aplicar espejo vertical (flip en Y)
            if (MirrorVertical)
            {
                g.ScaleTransform(1, -1);
            }

            // Devolver al origen negativo para que el dibujo sea relativo a (X,Y)
            g.TranslateTransform(-X, -Y);

            return state;
        }

        protected abstract void UpdateBounds();
    }

    /// <summary>
    /// Terminal de alimentación (VIN, VOUT)
    /// </summary>
    [Serializable]
    public class TerminalComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Voltage value (e.g., 90V, 13V)")]
        public string Voltage { get; set; }

        [Category("Appearance")]
        [Description("Terminal color")]
        public Color TerminalColor { get; set; }

        [Category("Appearance")]
        [Description("Terminal size in pixels")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public TerminalComponent()
        {
            Type = ComponentType.Terminal;
            Size = 16;
            TerminalColor = Color.Red;
            Voltage = "0V";
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Terminal", PinType.Power, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 1) return;
            Pins[0].Position = new Point(X, Y);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();

            using (SolidBrush brush = new SolidBrush(TerminalColor))
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            {
                g.FillEllipse(brush, X - Size / 2, Y - Size / 2, Size, Size);
                g.DrawEllipse(pen, X - Size / 2 - 3, Y - Size / 2 - 3, Size + 6, Size + 6);

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 8, FontStyle.Bold))
                {
                    SizeF labelSize = g.MeasureString(Name, labelFont);
                    g.DrawString(Name, labelFont, Brushes.Black, X - labelSize.Width / 2, Y - 30);
                    g.DrawString(Voltage, valueFont, brush, X - 15, Y + 15);
                }

                // Dibujar pines
                if (Pins != null)
                {
                    foreach (var pin in Pins)
                    {
                        pin.Draw(g, ShowPinNumbers, false);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - Size / 2 - 20, Y - 35, Size + 40, Size + 50);
        }

        public override SchematicComponent Clone()
        {
            return new TerminalComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Voltage = this.Voltage,
                TerminalColor = this.TerminalColor,
                Size = this.Size
            };
        }
    }

    /// <summary>
    /// Resistor
    /// </summary>
    [Serializable]
    public class ResistorComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Resistance value (e.g., 10kΩ, 0.1Ω)")]
        public string Value { get; set; }

        [Category("Electrical")]
        [Description("Power rating (e.g., 1/4W, 2W)")]
        public string Power { get; set; }

        [Category("Electrical")]
        [Description("Tolerance (e.g., 1%, 5%)")]
        public string Tolerance { get; set; }

        [Category("Geometry")]
        [Description("Component length in pixels")]
        public int Length { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public ResistorComponent()
        {
            Type = ComponentType.Resistor;
            Value = "10kΩ";
            Power = "1/4W";
            Tolerance = "1%";
            Orientation = ComponentOrientation.Vertical;
            Length = 80;
            ShowPinNumbers = true;

            // Inicializar pines
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Pin1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "Pin2", PinType.Passive, Point.Empty)
            };

            UpdateBounds();
        }

        /// <summary>
        /// Actualiza las posiciones de los pines según orientación
        /// </summary>
        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;

            if (Orientation == ComponentOrientation.Vertical)
            {
                // Pin 1 arriba
                Pins[0].Position = new Point(X, Y - 20);
                // Pin 2 abajo
                Pins[1].Position = new Point(X, Y + Length + 20);
            }
            else // Horizontal
            {
                // Pin 1 izquierda
                Pins[0].Position = new Point(X - 20, Y);
                // Pin 2 derecha
                Pins[1].Position = new Point(X + Length + 20, Y);
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.FromArgb(139, 90, 43), isSelected ? 3 : 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(210, 180, 140)))
            {
                Rectangle body;

                if (Orientation == ComponentOrientation.Vertical)
                {
                    body = new Rectangle(X - 12, Y, 24, Length);

                    // Conexiones (leads)
                    g.DrawLine(wirePen, X, Y - 20, X, Y);
                    g.DrawLine(wirePen, X, Y + Length, X, Y + Length + 20);
                }
                else
                {
                    body = new Rectangle(X, Y - 12, Length, 24);

                    // Conexiones (leads)
                    g.DrawLine(wirePen, X - 20, Y, X, Y);
                    g.DrawLine(wirePen, X + Length, Y, X + Length + 20, Y);
                }

                // Cuerpo del resistor
                g.FillRectangle(bodyBrush, body);
                g.DrawRectangle(pen, body);

                // Actualizar y dibujar pines
                UpdatePinPositions();
                foreach (var pin in Pins)
                {
                    pin.Draw(g, ShowPinNumbers, false);
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                // Etiquetas
                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    if (Orientation == ComponentOrientation.Vertical)
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 15, Y + Length / 2 - 15);
                        g.DrawString(Value, valueFont, Brushes.Blue, X + 15, Y + Length / 2);
                        g.DrawString(Power, valueFont, Brushes.Blue, X + 15, Y + Length / 2 + 10);
                    }
                    else
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + Length / 2 - 10, Y - 25);
                        g.DrawString(Value, valueFont, Brushes.Blue, X + Length / 2 - 15, Y + 15);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Vertical)
            {
                Bounds = new Rectangle(X - 15, Y - 5, 80, Length + 10);
            }
            else
            {
                Bounds = new Rectangle(X - 5, Y - 30, Length + 10, 60);
            }
        }

        public override SchematicComponent Clone()
        {
            return new ResistorComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Value = this.Value,
                Power = this.Power,
                Tolerance = this.Tolerance,
                Orientation = this.Orientation,
                Length = this.Length,
                ShowPinNumbers = this.ShowPinNumbers
            };
        }
    }

    /// <summary>
    /// Capacitor
    /// </summary>
    [Serializable]
    public class CapacitorComponent : SchematicComponent
    {
        // Propiedades eléctricas básicas
        [Category("Electrical")]
        [Description("Capacitance value (e.g., 100µF, 22nF, 1000pF)")]
        public string Capacitance { get; set; }

        [Category("Electrical")]
        [Description("Voltage rating (e.g., 25V, 160V, 450V)")]
        public string VoltageRating { get; set; }

        [Category("Electrical")]
        [Description("Type of capacitor")]
        public CapacitorType CapacitorType { get; set; }

        // Propiedades eléctricas avanzadas
        [Category("Electrical")]
        [Description("Equivalent Series Resistance (e.g., <5mΩ, 50mΩ)")]
        public string ESR { get; set; }

        [Category("Electrical")]
        [Description("Tolerance (e.g., ±10%, ±20%, ±5%)")]
        public string Tolerance { get; set; }

        [Category("Electrical")]
        [Description("Temperature coefficient (e.g., X7R, Y5V, C0G, NP0)")]
        public string TempCoefficient { get; set; }

        [Category("Electrical")]
        [Description("Ripple current rating (e.g., 1A, 2.5A)")]
        public string RippleCurrent { get; set; }

        [Category("Electrical")]
        [Description("Operating temperature range (e.g., -40°C to +85°C)")]
        public string TempRange { get; set; }

        [Category("Electrical")]
        [Description("Lifetime/Endurance (e.g., 2000h @ 105°C)")]
        public string Lifetime { get; set; }

        // Propiedades físicas
        [Category("Physical")]
        [Description("Package/Case size (e.g., 0805, 1206, Radial 10x16mm)")]
        public string Package { get; set; }

        [Category("Physical")]
        [Description("Lead spacing for through-hole (e.g., 2.5mm, 5mm)")]
        public string LeadSpacing { get; set; }

        // Información adicional
        [Category("Info")]
        [Description("Manufacturer")]
        public string Manufacturer { get; set; }

        [Category("Info")]
        [Description("Part number")]
        public string PartNumber { get; set; }

        [Category("Info")]
        [Description("Application/Purpose")]
        public string Application { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        // Constructor
        public CapacitorComponent()
        {
            Type = ComponentType.Capacitor;

            // Valores por defecto
            Capacitance = "100µF";
            VoltageRating = "25V";
            CapacitorType = CapacitorType.Electrolytic;
            ESR = "<5mΩ";
            Tolerance = "±20%";
            TempCoefficient = "N/A";
            RippleCurrent = "1A";
            TempRange = "-40°C to +105°C";
            Lifetime = "2000h @ 105°C";
            Package = "Radial 8x12mm";
            LeadSpacing = "3.5mm";
            Manufacturer = "";
            PartNumber = "";
            Application = "Bulk filtering";
            ShowPinNumbers = true;

            // Inicializar pines
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Pin1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "Pin2", PinType.Passive, Point.Empty)
            };

            Orientation = ComponentOrientation.Vertical;
            UpdateBounds();
        }

        /// <summary>
        /// Actualiza las posiciones de los pines según orientación
        /// </summary>
        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;

            bool isPolarized = IsPolarized();

            if (Orientation == ComponentOrientation.Vertical)
            {
                Pins[0].Position = new Point(X, Y - 20);
                if (isPolarized)
                {
                    Pins[1].Position = new Point(X, Y + 75);
                }
                else
                {
                    Pins[1].Position = new Point(X, Y + 60);
                }
            }
            else // Horizontal
            {
                Pins[0].Position = new Point(X - 20, Y);
                if (isPolarized)
                {
                    Pins[1].Position = new Point(X + 75, Y);
                }
                else
                {
                    Pins[1].Position = new Point(X + 60, Y);
                }
            }
        }

        /// <summary>
        /// Verifica si el capacitor es polarizado
        /// </summary>
        public bool IsPolarized()
        {
            return CapacitorType == CapacitorType.Electrolytic ||
                   CapacitorType == CapacitorType.Tantalum ||
                   CapacitorType == CapacitorType.Polymer ||
                   CapacitorType == CapacitorType.Supercapacitor;
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen platePen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 4 : 3))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Color componentColor = GetCapacitorColor();
                bool isPolarized = IsPolarized();

                // Actualizar posiciones de pines
                UpdatePinPositions();

                if (Orientation == ComponentOrientation.Vertical)
                {
                    DrawVerticalCapacitor(g, platePen, wirePen, componentColor, isPolarized, isSelected, isHovered);
                }
                else // Horizontal
                {
                    DrawHorizontalCapacitor(g, platePen, wirePen, componentColor, isPolarized, isSelected, isHovered);
                }

                // Dibujar pines
                if (Pins != null)
                {
                    foreach (var pin in Pins)
                    {
                        pin.Draw(g, ShowPinNumbers, false);
                    }
                }
            }
        }

        private void DrawVerticalCapacitor(Graphics g, Pen platePen, Pen wirePen, Color componentColor, bool isPolarized, bool isSelected, bool isHovered)
        {
            // Placas verticales
            if (isPolarized)
            {
                // Placa positiva (recta)
                g.DrawLine(platePen, X - 15, Y, X + 15, Y);

                // Placa negativa (curva para electrolíticos)
                if (CapacitorType == CapacitorType.Electrolytic || CapacitorType == CapacitorType.Polymer)
                {
                    using (Brush bodyBrush = new SolidBrush(Color.FromArgb(200, componentColor)))
                    {
                        g.FillRectangle(bodyBrush, X - 12, Y + 5, 24, 50);
                    }
                    g.DrawLine(platePen, X - 15, Y + 55, X + 15, Y + 55);
                }
                else // Tantalum, Supercapacitor
                {
                    using (Brush bodyBrush = new SolidBrush(Color.FromArgb(200, componentColor)))
                    {
                        g.FillRectangle(bodyBrush, X - 10, Y + 5, 20, 50);
                    }
                    g.DrawLine(platePen, X - 15, Y + 55, X + 15, Y + 55);
                }

                // Marca de polaridad (+)
                using (Pen redPen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(redPen, X - 25, Y - 8, X - 20, Y - 8);
                    g.DrawLine(redPen, X - 22.5f, Y - 10.5f, X - 22.5f, Y - 5.5f);
                }
            }
            else // No polarizado
            {
                // Dos placas rectas
                g.DrawLine(platePen, X - 15, Y + 25, X + 15, Y + 25);
                g.DrawLine(platePen, X - 15, Y + 35, X + 15, Y + 35);
            }

            // Conexiones
            if (isPolarized)
            {
                g.DrawLine(wirePen, X, Y - 20, X, Y);
                g.DrawLine(wirePen, X, Y + 55, X, Y + 75);
            }
            else
            {
                g.DrawLine(wirePen, X, Y, X, Y + 25);
                g.DrawLine(wirePen, X, Y + 35, X, Y + 60);
            }

            if (isHovered)
            {
                using (Pen hoverPen = new Pen(Color.Orange, 2))
                {
                    hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawRectangle(hoverPen, Bounds);
                }
            }

            // Etiquetas
            using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(Name, labelFont, Brushes.Black, X + 20, Y + 15);
                g.DrawString(Capacitance, valueFont, Brushes.Blue, X + 20, Y + 28);
                g.DrawString(VoltageRating, valueFont, Brushes.DarkGreen, X + 20, Y + 38);
            }
        }

        private void DrawHorizontalCapacitor(Graphics g, Pen platePen, Pen wirePen, Color componentColor, bool isPolarized, bool isSelected, bool isHovered)
        {
            // Placas horizontales
            if (isPolarized)
            {
                // Placa positiva (recta)
                g.DrawLine(platePen, X, Y - 15, X, Y + 15);

                // Placa negativa (cuerpo para electrolíticos)
                if (CapacitorType == CapacitorType.Electrolytic || CapacitorType == CapacitorType.Polymer)
                {
                    using (Brush bodyBrush = new SolidBrush(Color.FromArgb(200, componentColor)))
                    {
                        g.FillRectangle(bodyBrush, X + 5, Y - 12, 50, 24);
                    }
                    g.DrawLine(platePen, X + 55, Y - 15, X + 55, Y + 15);
                }
                else // Tantalum, Supercapacitor
                {
                    using (Brush bodyBrush = new SolidBrush(Color.FromArgb(200, componentColor)))
                    {
                        g.FillRectangle(bodyBrush, X + 5, Y - 10, 50, 20);
                    }
                    g.DrawLine(platePen, X + 55, Y - 15, X + 55, Y + 15);
                }

                // Marca de polaridad (+)
                using (Pen redPen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(redPen, X - 8, Y - 25, X - 8, Y - 20);
                    g.DrawLine(redPen, X - 10.5f, Y - 22.5f, X - 5.5f, Y - 22.5f);
                }
            }
            else // No polarizado
            {
                // Dos placas rectas
                g.DrawLine(platePen, X + 25, Y - 15, X + 25, Y + 15);
                g.DrawLine(platePen, X + 35, Y - 15, X + 35, Y + 15);
            }

            // Conexiones
            if (isPolarized)
            {
                g.DrawLine(wirePen, X - 20, Y, X, Y);
                g.DrawLine(wirePen, X + 55, Y, X + 75, Y);
            }
            else
            {
                g.DrawLine(wirePen, X, Y, X + 25, Y);
                g.DrawLine(wirePen, X + 35, Y, X + 60, Y);
            }

            if (isHovered)
            {
                using (Pen hoverPen = new Pen(Color.Orange, 2))
                {
                    hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawRectangle(hoverPen, Bounds);
                }
            }

            // Etiquetas
            using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
            using (Font valueFont = new Font("Arial", 7))
            {
                g.DrawString(Name, labelFont, Brushes.Black, X + 15, Y - 35);
                g.DrawString(Capacitance, valueFont, Brushes.Blue, X + 15, Y + 20);
                g.DrawString(VoltageRating, valueFont, Brushes.DarkGreen, X + 15, Y + 30);
            }
        }

        private Color GetCapacitorColor()
        {
            switch (CapacitorType)
            {
                case CapacitorType.Electrolytic:
                    return Color.FromArgb(100, 100, 150); // Azul oscuro
                case CapacitorType.Tantalum:
                    return Color.FromArgb(180, 140, 70); // Café/Dorado
                case CapacitorType.Ceramic:
                    return Color.FromArgb(200, 180, 140); // Beige
                case CapacitorType.Film:
                    return Color.FromArgb(220, 200, 100); // Amarillo
                case CapacitorType.Polymer:
                    return Color.FromArgb(80, 80, 120); // Azul/Negro
                case CapacitorType.Mica:
                    return Color.FromArgb(180, 180, 180); // Gris
                case CapacitorType.Paper:
                    return Color.FromArgb(230, 220, 200); // Crema
                case CapacitorType.Supercapacitor:
                    return Color.FromArgb(60, 60, 60); // Negro
                default:
                    return Color.Gray;
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Vertical)
            {
                Bounds = new Rectangle(X - 30, Y - 25, 90, 110);
            }
            else
            {
                Bounds = new Rectangle(X - 25, Y - 40, 110, 75);
            }
        }

        public override SchematicComponent Clone()
        {
            return new CapacitorComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Capacitance = this.Capacitance,
                VoltageRating = this.VoltageRating,
                CapacitorType = this.CapacitorType,
                ESR = this.ESR,
                Tolerance = this.Tolerance,
                TempCoefficient = this.TempCoefficient,
                RippleCurrent = this.RippleCurrent,
                TempRange = this.TempRange,
                Lifetime = this.Lifetime,
                Package = this.Package,
                LeadSpacing = this.LeadSpacing,
                Manufacturer = this.Manufacturer,
                PartNumber = this.PartNumber,
                Application = this.Application,
                Orientation = this.Orientation
            };
        }
    }

    /// <summary>
    /// Inductor
    /// </summary>
    [Serializable]
    public class InductorComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Inductance value (e.g., 100µH)")]
        public string Value { get; set; }

        [Category("Electrical")]
        [Description("Saturation current (e.g., 5A)")]
        public string Isat { get; set; }

        [Category("Electrical")]
        [Description("DC resistance (e.g., 20mΩ)")]
        public string DCR { get; set; }

        [Category("Geometry")]
        [Description("Component width in pixels")]
        public int Width { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public InductorComponent()
        {
            Type = ComponentType.Inductor;
            Value = "100µH";
            Isat = "5A";
            DCR = "20mΩ";
            Width = 180;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Pin1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "Pin2", PinType.Passive, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;

            Pins[0].Position = new Point(X, Y);
            Pins[1].Position = new Point(X + Width, Y);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen coilPen = new Pen(isSelected ? Color.Blue : Color.FromArgb(184, 115, 51), isSelected ? 4 : 3))
            {
                int numCoils = 5;
                int coilWidth = (Width - 40) / numCoils;

                // Actualizar posiciones de pines
                UpdatePinPositions();

                g.DrawLine(coilPen, X, Y, X + 20, Y);

                for (int i = 0; i < numCoils; i++)
                {
                    int x = X + 20 + i * coilWidth;
                    g.DrawArc(coilPen, x, Y - 18, coilWidth, 36, 0, 180);
                }

                g.DrawLine(coilPen, X + Width - 20, Y, X + Width, Y);

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 8))
                {
                    g.DrawString(Name, labelFont, Brushes.Black, X + Width / 2 - 15, Y - 45);
                    g.DrawString(Value, valueFont, Brushes.Blue, X + Width / 2 - 20, Y + 25);
                    g.DrawString($"Isat={Isat}", valueFont, Brushes.Blue, X + Width / 2 - 20, Y + 37);
                }

                // Dibujar pines
                if (Pins != null)
                {
                    foreach (var pin in Pins)
                    {
                        pin.Draw(g, ShowPinNumbers, false);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - 5, Y - 50, Width + 10, 95);
        }

        public override SchematicComponent Clone()
        {
            return new InductorComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Value = this.Value,
                Isat = this.Isat,
                DCR = this.DCR,
                Width = this.Width
            };
        }
    }

    /// <summary>
    /// Diode
    /// </summary>
    [Serializable]
    public class DiodeComponent : SchematicComponent
    {
        private string partNumber;

        [Category("Component")]
        [Description("Diode part number (select from dropdown)")]
        [TypeConverter(typeof(DiodePartNumberConverter))]
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                // Auto-actualizar propiedades desde database
                if (!string.IsNullOrEmpty(value) && ComponentDatabase.Diodes.ContainsKey(value))
                {
                    var spec = ComponentDatabase.Diodes[value];
                    DiodeType = spec.DiodeType;
                    VoltageRating = spec.VoltageRating;
                    CurrentRating = spec.CurrentRating;
                    VF = spec.VF;
                    Package = spec.Package;
                }
            }
        }

        [Category("Electrical")]
        [Description("Diode type")]
        [ReadOnly(true)]
        public string DiodeType { get; set; }

        [Category("Electrical")]
        [Description("Reverse voltage")]
        [ReadOnly(true)]
        public string VoltageRating { get; set; }

        [Category("Electrical")]
        [Description("Forward current")]
        [ReadOnly(true)]
        public string CurrentRating { get; set; }

        [Category("Electrical")]
        [Description("Forward voltage drop")]
        [ReadOnly(true)]
        public string VF { get; set; }

        [Category("Physical")]
        [Description("Package type")]
        [ReadOnly(true)]
        public string Package { get; set; }

        [Category("Geometry")]
        [Description("Component size in pixels")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public DiodeComponent()
        {
            Type = ComponentType.Diode;
            PartNumber = "1N4148"; // Trigger auto-update
            Size = 60;
            Orientation = ComponentOrientation.Horizontal;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
            {
                if (Orientation == ComponentOrientation.Horizontal)
                {
                    // Triángulo (ánodo)
                    Point[] triangle = new Point[]
                    {
                        new Point(X, Y - 15),
                        new Point(X, Y + 15),
                        new Point(X + 30, Y)
                    };
                    g.FillPolygon(fillBrush, triangle);
                    g.DrawPolygon(pen, triangle);

                    // Línea vertical (cátodo)
                    g.DrawLine(pen, X + 30, Y - 15, X + 30, Y + 15);

                    // Conexiones
                    g.DrawLine(pen, X - 20, Y, X, Y);
                    g.DrawLine(pen, X + 30, Y, X + 50, Y);

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 5, Y - 30);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X, Y + 20);
                    }
                }
                else // Vertical
                {
                    // Triángulo (ánodo)
                    Point[] triangle = new Point[]
                    {
                        new Point(X - 15, Y),
                        new Point(X + 15, Y),
                        new Point(X, Y + 30)
                    };
                    g.FillPolygon(fillBrush, triangle);
                    g.DrawPolygon(pen, triangle);

                    // Línea horizontal (cátodo)
                    g.DrawLine(pen, X - 15, Y + 30, X + 15, Y + 30);

                    // Conexiones
                    g.DrawLine(pen, X, Y - 20, X, Y);
                    g.DrawLine(pen, X, Y + 30, X, Y + 50);

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 20, Y + 5);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X + 20, Y + 18);
                    }
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Horizontal)
            {
                Bounds = new Rectangle(X - 25, Y - 35, Size + 20, 70);
            }
            else
            {
                Bounds = new Rectangle(X - 25, Y - 25, 70, Size + 20);
            }
        }

        public override SchematicComponent Clone()
        {
            return new DiodeComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                PartNumber = this.PartNumber,
                VoltageRating = this.VoltageRating,
                CurrentRating = this.CurrentRating,
                DiodeType = this.DiodeType,
                Size = this.Size,
                Orientation = this.Orientation
            };
        }
    }

    /// <summary>
    /// MOSFET Component
    /// </summary>
    [Serializable]
    public class MosfetComponent : SchematicComponent
    {
        private string partNumber;

        [Category("Component")]
        [Description("MOSFET part number (select from dropdown)")]
        [TypeConverter(typeof(MosfetPartNumberConverter))]
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                // Auto-actualizar propiedades desde database
                if (!string.IsNullOrEmpty(value) && ComponentDatabase.Mosfets.ContainsKey(value))
                {
                    var spec = ComponentDatabase.Mosfets[value];
                    ChannelType = spec.ChannelType;
                    VDS = spec.VDS;
                    ID = spec.ID;
                    RDSon = spec.RDSon;
                    VGS = spec.VGS;
                    QG = spec.QG;
                    Package = spec.Package;
                }
            }
        }

        private string _channelType;

        [Category("Electrical")]
        [Description("Channel type (N-Channel or P-Channel)")]
        [TypeConverter(typeof(MOSFETChannelTypeConverter))]
        public string ChannelType
        {
            get => _channelType;
            set
            {
                if (_channelType != value)
                {
                    _channelType = value;
                    UpdateBounds();
                }
            }
        }

        [Category("Electrical")]
        [Description("Drain-Source voltage")]
        [ReadOnly(true)]
        public string VDS { get; set; }

        [Category("Electrical")]
        [Description("Drain current")]
        [ReadOnly(true)]
        public string ID { get; set; }

        [Category("Electrical")]
        [Description("RDS(on) resistance")]
        [ReadOnly(true)]
        public string RDSon { get; set; }

        [Category("Electrical")]
        [Description("Gate-Source voltage")]
        [ReadOnly(true)]
        public string VGS { get; set; }

        [Category("Electrical")]
        [Description("Gate charge")]
        [ReadOnly(true)]
        public string QG { get; set; }

        [Category("Physical")]
        [Description("Package type")]
        [ReadOnly(true)]
        public string Package { get; set; }

        [Category("Geometry")]
        [Description("Component size in pixels")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public MosfetComponent()
        {
            Type = ComponentType.Mosfet;
            PartNumber = "IRF540N"; // Trigger auto-update
            Size = 60;
            Orientation = ComponentOrientation.Vertical;
            ShowPinNumbers = true;
            _channelType = "N-Channel";

            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "G", PinType.Input, Point.Empty),
                new ComponentPin(2, "D", PinType.Output, Point.Empty),
                new ComponentPin(3, "S", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                // Gate vertical
                g.DrawLine(pen, this.X - 10, this.Y - 15, this.X - 10, this.Y + 15);

                // Channel (línea vertical punteada)
                using (Pen dashedPen = new Pen(Color.Black, 2))
                {
                    dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawLine(dashedPen, this.X, this.Y - 15, this.X, this.Y + 15);
                }

                // Drain y Source lines
                g.DrawLine(pen, this.X, this.Y - 15, this.X + 10, this.Y - 15);
                g.DrawLine(pen, this.X, this.Y + 15, this.X + 10, this.Y + 15);

                // Flecha indicando tipo de canal
                if (ChannelType == "N-Channel")
                {
                    // Flecha apuntando hacia el canal (→)
                    Point[] arrow = {
                new Point(this.X - 5, this.Y - 5),
                new Point(this.X, this.Y),
                new Point(this.X - 5, this.Y + 5)
            };
                    g.DrawLines(pen, arrow);
                }
                else // P-Channel
                {
                    // Flecha apuntando desde el canal (←)
                    Point[] arrow = {
                new Point(this.X - 15, this.Y - 5),
                new Point(this.X - 10, this.Y),
                new Point(this.X - 15, this.Y + 5)
            };
                    g.DrawLines(pen, arrow);
                }

                // Wires
                g.DrawLine(wirePen, this.X - 30, this.Y, this.X - 10, this.Y);        // Gate
                g.DrawLine(wirePen, this.X + 10, this.Y - 15, this.X + 20, this.Y - 20); // Drain
                g.DrawLine(wirePen, this.X + 10, this.Y + 15, this.X + 20, this.Y + 20); // Source

                // Label
                using (Font font = new Font("Arial", 7))
                {
                    string label = ChannelType == "N-Channel" ? "N" : "P";
                    g.DrawString(label, font, Brushes.Black, this.X - 5, this.Y - 25);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }
        /*public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            using (Pen thinPen = new Pen(Color.Black, 1))
            {
                if (Orientation == ComponentOrientation.Vertical)
                {
                    // Gate (izquierda)
                    g.DrawLine(pen, X - 30, Y + 20, X - 10, Y + 20);

                    // Canal vertical
                    g.DrawLine(pen, X - 10, Y, X - 10, Y + 40);

                    // Drain (arriba)
                    g.DrawLine(pen, X - 10, Y, X + 10, Y);
                    g.DrawLine(pen, X + 10, Y, X + 10, Y - 20);

                    // Source (abajo)
                    g.DrawLine(pen, X - 10, Y + 40, X + 10, Y + 40);
                    g.DrawLine(pen, X + 10, Y + 40, X + 10, Y + 60);

                    // Body diode (pequeño triángulo)
                    if (ChannelType == "N-Channel")
                    {
                        Point[] triangle = new Point[]
                        {
                            new Point(X + 5, Y + 15),
                            new Point(X + 5, Y + 25),
                            new Point(X + 12, Y + 20)
                        };
                        g.DrawPolygon(thinPen, triangle);
                    }

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 20, Y + 10);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X + 20, Y + 22);
                        g.DrawString(ChannelType.Substring(0, 1), valueFont, Brushes.Red, X + 20, Y + 32);
                    }
                }
                else // Horizontal
                {
                    // Gate (arriba)
                    g.DrawLine(pen, X + 20, Y - 30, X + 20, Y - 10);

                    // Canal horizontal
                    g.DrawLine(pen, X, Y - 10, X + 40, Y - 10);

                    // Drain (izquierda)
                    g.DrawLine(pen, X, Y - 10, X, Y + 10);
                    g.DrawLine(pen, X, Y + 10, X - 20, Y + 10);

                    // Source (derecha)
                    g.DrawLine(pen, X + 40, Y - 10, X + 40, Y + 10);
                    g.DrawLine(pen, X + 40, Y + 10, X + 60, Y + 10);

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 5, Y - 45);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X + 5, Y + 20);
                    }
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }*/

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Vertical)
            {
                Bounds = new Rectangle(X - 35, Y - 25, 90, Size + 50);
            }
            else
            {
                Bounds = new Rectangle(X - 25, Y - 50, Size + 50, 90);
            }
        }

        public override SchematicComponent Clone()
        {
            return new MosfetComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                PartNumber = this.PartNumber,
                ChannelType = this.ChannelType,
                VDS = this.VDS,
                ID = this.ID,
                RDSon = this.RDSon,
                Size = this.Size,
                Orientation = this.Orientation
            };
        }
    }

    /// <summary>
    /// BJT (Bipolar Junction Transistor)
    /// </summary>
    [Serializable]
    public class BJTComponent : SchematicComponent
    {
        private string partNumber;

        [Category("Component")]
        [Description("BJT part number (select from dropdown)")]
        [TypeConverter(typeof(BJTPartNumberConverter))]
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                // Auto-actualizar propiedades desde database
                if (!string.IsNullOrEmpty(value) && ComponentDatabase.BJTs.ContainsKey(value))
                {
                    var spec = ComponentDatabase.BJTs[value];
                    TransistorType = spec.TransistorType;
                    VCE = spec.VCE;
                    IC = spec.IC;
                    PowerRating = spec.PowerRating;
                    HFE = spec.HFE;
                    Package = spec.Package;
                }
            }
        }

        private string _transistorType;

        [Category("Electrical")]
        [Description("Transistor type (NPN or PNP)")]
        [TypeConverter(typeof(BJTTransistorTypeConverter))]
        public string TransistorType
        {
            get => _transistorType;
            set
            {
                if (_transistorType != value)
                {
                    _transistorType = value;
                    UpdateBounds();
                }
            }
        }

        [Category("Electrical")]
        [Description("Collector-Emitter voltage")]
        [ReadOnly(true)]
        public string VCE { get; set; }

        [Category("Electrical")]
        [Description("Collector current")]
        [ReadOnly(true)]
        public string IC { get; set; }

        [Category("Electrical")]
        [Description("Power dissipation")]
        [ReadOnly(true)]
        public string PowerRating { get; set; }

        [Category("Electrical")]
        [Description("Current gain hFE")]
        [ReadOnly(true)]
        public string HFE { get; set; }

        [Category("Physical")]
        [Description("Package type")]
        [ReadOnly(true)]
        public string Package { get; set; }

        [Category("Geometry")]
        [Description("Component size in pixels")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public BJTComponent()
        {
            Type = ComponentType.Mosfet; // Usamos Mosfet temporalmente
            PartNumber = "2N2222A"; // Trigger auto-update
            Size = 60;
            Orientation = ComponentOrientation.Vertical;
            ShowPinNumbers = true;
            _transistorType = "NPN";

            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "B", PinType.Input, Point.Empty),
                new ComponentPin(2, "C", PinType.Output, Point.Empty),
                new ComponentPin(3, "E", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }


        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                // Base vertical
                g.DrawLine(pen, this.X - 10, this.Y - 20, this.X - 10, this.Y + 20);

                // Collector y Emitter
                g.DrawLine(pen, this.X - 10, this.Y - 10, this.X + 10, this.Y - 20);
                g.DrawLine(pen, this.X - 10, this.Y + 10, this.X + 10, this.Y + 20);

                // Flecha en emitter indicando tipo
                if (TransistorType == "NPN")
                {
                    // Flecha apuntando hacia afuera (↗)
                    Point[] arrow = {
                new Point(this.X + 5, this.Y + 15),
                new Point(this.X + 10, this.Y + 20),
                new Point(this.X + 5, this.Y + 20)
            };
                    g.DrawPolygon(pen, arrow);
                    g.FillPolygon(Brushes.Black, arrow);
                }
                else // PNP
                {
                    // Flecha apuntando hacia adentro (↙)
                    Point[] arrow = {
                new Point(this.X - 5, this.Y + 10),
                new Point(this.X - 10, this.Y + 10),
                new Point(this.X - 5, this.Y + 15)
            };
                    g.DrawPolygon(pen, arrow);
                    g.FillPolygon(Brushes.Black, arrow);
                }

                // Wires
                g.DrawLine(wirePen, this.X - 30, this.Y, this.X - 10, this.Y);        // Base
                g.DrawLine(wirePen, this.X + 10, this.Y - 20, this.X + 20, this.Y - 20); // Collector
                g.DrawLine(wirePen, this.X + 10, this.Y + 20, this.X + 20, this.Y + 20); // Emitter

                // Label
                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(TransistorType, font, Brushes.Black, this.X - 8, this.Y - 30);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }
        /*public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            using (Pen thinPen = new Pen(Color.Black, 1))
            {
                if (Orientation == ComponentOrientation.Vertical)
                {
                    // Base (izquierda)
                    g.DrawLine(pen, X - 30, Y + 20, X - 10, Y + 20);

                    // Línea vertical central
                    g.DrawLine(pen, X - 10, Y, X - 10, Y + 40);

                    // Collector (arriba derecha)
                    g.DrawLine(pen, X - 10, Y + 10, X + 10, Y);
                    g.DrawLine(pen, X + 10, Y, X + 10, Y - 20);

                    // Emitter (abajo derecha)
                    g.DrawLine(pen, X - 10, Y + 30, X + 10, Y + 40);
                    g.DrawLine(pen, X + 10, Y + 40, X + 10, Y + 60);

                    // Flecha en emisor (indica NPN o PNP)
                    if (TransistorType == "NPN")
                    {
                        // Flecha saliendo (NPN)
                        Point[] arrow = new Point[]
                        {
                            new Point(X + 10, Y + 40),
                            new Point(X + 5, Y + 35),
                            new Point(X + 5, Y + 42)
                        };
                        g.FillPolygon(Brushes.Black, arrow);
                    }
                    else // PNP
                    {
                        // Flecha entrando (PNP)
                        Point[] arrow = new Point[]
                        {
                            new Point(X - 10, Y + 30),
                            new Point(X - 5, Y + 25),
                            new Point(X - 5, Y + 32)
                        };
                        g.FillPolygon(Brushes.Black, arrow);
                    }

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 20, Y + 10);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X + 20, Y + 22);
                        g.DrawString(TransistorType, valueFont, Brushes.Red, X + 20, Y + 32);
                    }
                }
                else // Horizontal
                {
                    // Base (arriba)
                    g.DrawLine(pen, X + 20, Y - 30, X + 20, Y - 10);

                    // Línea horizontal central
                    g.DrawLine(pen, X, Y - 10, X + 40, Y - 10);

                    // Collector (izquierda abajo)
                    g.DrawLine(pen, X + 10, Y - 10, X, Y + 10);
                    g.DrawLine(pen, X, Y + 10, X - 20, Y + 10);

                    // Emitter (derecha abajo)
                    g.DrawLine(pen, X + 30, Y - 10, X + 40, Y + 10);
                    g.DrawLine(pen, X + 40, Y + 10, X + 60, Y + 10);

                    // Flecha
                    if (TransistorType == "NPN")
                    {
                        Point[] arrow = new Point[]
                        {
                            new Point(X + 40, Y + 10),
                            new Point(X + 35, Y + 5),
                            new Point(X + 35, Y + 12)
                        };
                        g.FillPolygon(Brushes.Black, arrow);
                    }
                    else
                    {
                        Point[] arrow = new Point[]
                        {
                            new Point(X + 30, Y - 10),
                            new Point(X + 25, Y - 5),
                            new Point(X + 25, Y - 12)
                        };
                        g.FillPolygon(Brushes.Black, arrow);
                    }

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 5, Y - 45);
                        g.DrawString(PartNumber, valueFont, Brushes.Blue, X + 5, Y + 20);
                    }
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }*/

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Vertical)
            {
                Bounds = new Rectangle(X - 35, Y - 25, 90, Size + 50);
            }
            else
            {
                Bounds = new Rectangle(X - 25, Y - 50, Size + 50, 90);
            }
        }

        public override SchematicComponent Clone()
        {
            return new BJTComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                PartNumber = this.PartNumber,
                TransistorType = this.TransistorType,
                VCE = this.VCE,
                IC = this.IC,
                PowerRating = this.PowerRating,
                HFE = this.HFE,
                Size = this.Size,
                Orientation = this.Orientation
            };
        }
    }

    // ============================================================================
    // SOLUCIÓN COMPLETA - ICComponent con pines ilimitados y pinout específico
    // Reemplazar la clase ICComponent completa en SchematicComponent.cs
    // ============================================================================

    /// <summary>
    /// Integrated Circuit (IC) - VERSIÓN MEJORADA
    /// </summary>
    [Serializable]
    public class ICComponent : SchematicComponent
    {
        private string partNumber;

        [Category("Component")]
        [Description("IC part number (select from dropdown)")]
        [TypeConverter(typeof(ICPartNumberConverter))]
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                // Auto-actualizar propiedades desde database
                if (!string.IsNullOrEmpty(value) && ComponentDatabase.ICs.ContainsKey(value))
                {
                    var spec = ComponentDatabase.ICs[value];
                    Function = spec.Function;
                    SupplyVoltage = spec.SupplyVoltage;
                    PinCount = spec.PinCount;
                    Features = spec.Features;
                    Package = spec.Package;
                    InitializePins();
                }
            }
        }

        [Category("Electrical")]
        [Description("IC function description")]
        [ReadOnly(true)]
        public string Function { get; set; }

        [Category("Electrical")]
        [Description("Supply voltage range")]
        [ReadOnly(true)]
        public string SupplyVoltage { get; set; }

        [Category("Electrical")]
        [Description("Number of pins")]
        [ReadOnly(true)]
        public int PinCount { get; set; }

        [Category("Electrical")]
        [Description("Key features")]
        [ReadOnly(true)]
        public string Features { get; set; }

        [Category("Physical")]
        [Description("Package type")]
        [ReadOnly(true)]
        public string Package { get; set; }

        [Category("Geometry")]
        [Description("IC width in pixels")]
        public int Width { get; set; }

        [Category("Geometry")]
        [Description("IC height in pixels")]
        public int Height { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Category("Display")]
        [Description("Show pin types/names")]
        public bool ShowPinTypes { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public ICComponent()
        {
            Type = ComponentType.IC;
            Width = 120;
            Height = 80;
            ShowPinNumbers = true;
            ShowPinTypes = true;
            Pins = new List<ComponentPin>();
            PinCount = 8; // Default
            PartNumber = "TL494"; // Trigger auto-update
            UpdateBounds();
        }

        /// <summary>
        /// Inicializa los pines del IC según el número de pines
        /// </summary>
        private void InitializePins()
        {
            Pins.Clear();

            // Intentar obtener pinout específico
            var pinDefinitions = GetPinDefinitions(PartNumber);

            if (pinDefinitions != null)
            {
                foreach (var pinDef in pinDefinitions)
                {
                    Pins.Add(new ComponentPin(pinDef.Number, pinDef.Name, pinDef.Type, Point.Empty));
                }
            }
            else
            {
                // Pines genéricos si no hay definición específica
                for (int i = 1; i <= PinCount; i++)
                {
                    Pins.Add(new ComponentPin(i, $"Pin{i}", PinType.Bidirectional, Point.Empty));
                }
            }

            // Ajustar dimensiones según número de pines
            AdjustDimensions();
        }

        /// <summary>
        /// Ajusta las dimensiones del IC según el número de pines
        /// </summary>
        private void AdjustDimensions()
        {
            int pinsPerSide = (int)Math.Ceiling(PinCount / 2.0);

            if (pinsPerSide <= 8)
            {
                Width = 120;
                Height = 80;
            }
            else if (pinsPerSide <= 14)
            {
                Width = 140;
                Height = 120;
            }
            else if (pinsPerSide <= 20)
            {
                Width = 160;
                Height = pinsPerSide * 10;
            }
            else if (pinsPerSide <= 28)
            {
                Width = 180;
                Height = pinsPerSide * 9;
            }
            else
            {
                // Para ICs muy grandes (40+ pines)
                Width = 200;
                Height = Math.Max(200, pinsPerSide * 8);
            }
        }

        /// <summary>
        /// Obtiene las definiciones de pines para ICs conocidos
        /// </summary>
        private List<(int Number, string Name, PinType Type)> GetPinDefinitions(string partNum)
        {
            if (string.IsNullOrEmpty(partNum)) return null;

            var defs = new List<(int, string, PinType)>();

            // Normalizar part number
            string normalized = partNum.ToUpper().Replace("-", "").Replace(" ", "");

            // TL072, LM358 - Dual OpAmp 8 pines
            if (normalized.Contains("TL072") || normalized.Contains("TL071"))
            {
                defs.Add((1, "OUT A", PinType.Output));
                defs.Add((2, "IN- A", PinType.Input));
                defs.Add((3, "IN+ A", PinType.Input));
                defs.Add((4, "V-", PinType.Power));
                defs.Add((5, "IN+ B", PinType.Input));
                defs.Add((6, "IN- B", PinType.Input));
                defs.Add((7, "OUT B", PinType.Output));
                defs.Add((8, "V+", PinType.Power));
            }
            else if (normalized.Contains("LM358") || normalized.Contains("LM324"))
            {
                defs.Add((1, "OUT1", PinType.Output));
                defs.Add((2, "IN1-", PinType.Input));
                defs.Add((3, "IN1+", PinType.Input));
                defs.Add((4, "GND", PinType.Ground));
                defs.Add((5, "IN2+", PinType.Input));
                defs.Add((6, "IN2-", PinType.Input));
                defs.Add((7, "OUT2", PinType.Output));
                defs.Add((8, "VCC", PinType.Power));
            }
            // TL494 - PWM Controller 16 pines
            else if (normalized.Contains("TL494"))
            {
                defs.Add((1, "1IN+", PinType.Input));
                defs.Add((2, "1IN-", PinType.Input));
                defs.Add((3, "FB", PinType.Input));
                defs.Add((4, "DTC", PinType.Input));
                defs.Add((5, "CT", PinType.Input));
                defs.Add((6, "RT", PinType.Input));
                defs.Add((7, "GND", PinType.Ground));
                defs.Add((8, "C1", PinType.Output));
                defs.Add((9, "E1", PinType.Output));
                defs.Add((10, "E2", PinType.Output));
                defs.Add((11, "C2", PinType.Output));
                defs.Add((12, "VCC", PinType.Power));
                defs.Add((13, "OUT", PinType.Output));
                defs.Add((14, "REF", PinType.Output));
                defs.Add((15, "2IN-", PinType.Input));
                defs.Add((16, "2IN+", PinType.Input));
            }
            // LM2596 - Buck Converter 5 pines
            else if (normalized.Contains("LM2596"))
            {
                defs.Add((1, "VIN", PinType.Power));
                defs.Add((2, "OUT", PinType.Output));
                defs.Add((3, "GND", PinType.Ground));
                defs.Add((4, "FB", PinType.Input));
                defs.Add((5, "ON/OFF", PinType.Input));
            }
            // UC3842 - PWM Controller 8 pines
            else if (normalized.Contains("UC3842") || normalized.Contains("UC3843"))
            {
                defs.Add((1, "COMP", PinType.Output));
                defs.Add((2, "VFB", PinType.Input));
                defs.Add((3, "ISENSE", PinType.Input));
                defs.Add((4, "RT/CT", PinType.Input));
                defs.Add((5, "GND", PinType.Ground));
                defs.Add((6, "OUT", PinType.Output));
                defs.Add((7, "VCC", PinType.Power));
                defs.Add((8, "VREF", PinType.Output));
            }
            // 7805, L7805 - Regulador 3 pines
            else if (normalized.Contains("7805") || normalized.Contains("L7805"))
            {
                defs.Add((1, "IN", PinType.Power));
                defs.Add((2, "GND", PinType.Ground));
                defs.Add((3, "OUT", PinType.Output));
            }
            // ATMEGA328P - 28 pines
            else if (normalized.Contains("ATMEGA328"))
            {
                defs.Add((1, "PC6/RESET", PinType.Input));
                defs.Add((2, "PD0/RXD", PinType.Bidirectional));
                defs.Add((3, "PD1/TXD", PinType.Bidirectional));
                defs.Add((4, "PD2/INT0", PinType.Bidirectional));
                defs.Add((5, "PD3/INT1", PinType.Bidirectional));
                defs.Add((6, "PD4/T0", PinType.Bidirectional));
                defs.Add((7, "VCC", PinType.Power));
                defs.Add((8, "GND", PinType.Ground));
                defs.Add((9, "PB6/XTAL1", PinType.Input));
                defs.Add((10, "PB7/XTAL2", PinType.Output));
                defs.Add((11, "PD5/T1", PinType.Bidirectional));
                defs.Add((12, "PD6/AIN0", PinType.Bidirectional));
                defs.Add((13, "PD7/AIN1", PinType.Bidirectional));
                defs.Add((14, "PB0/ICP1", PinType.Bidirectional));
                defs.Add((15, "PB1/OC1A", PinType.Bidirectional));
                defs.Add((16, "PB2/SS", PinType.Bidirectional));
                defs.Add((17, "PB3/MOSI", PinType.Bidirectional));
                defs.Add((18, "PB4/MISO", PinType.Bidirectional));
                defs.Add((19, "PB5/SCK", PinType.Bidirectional));
                defs.Add((20, "AVCC", PinType.Power));
                defs.Add((21, "AREF", PinType.Input));
                defs.Add((22, "GND", PinType.Ground));
                defs.Add((23, "PC0/ADC0", PinType.Bidirectional));
                defs.Add((24, "PC1/ADC1", PinType.Bidirectional));
                defs.Add((25, "PC2/ADC2", PinType.Bidirectional));
                defs.Add((26, "PC3/ADC3", PinType.Bidirectional));
                defs.Add((27, "PC4/ADC4", PinType.Bidirectional));
                defs.Add((28, "PC5/ADC5", PinType.Bidirectional));
            }
            // 18F4550 - 40 pines
            else if (normalized.Contains("18F4550") || normalized.Contains("PIC18F4550"))
            {
                defs.Add((1, "MCLR/VPP/RE3", PinType.Input));
                defs.Add((2, "RA0/AN0", PinType.Bidirectional));
                defs.Add((3, "RA1/AN1", PinType.Bidirectional));
                defs.Add((4, "RA2/AN2/VREF-", PinType.Bidirectional));
                defs.Add((5, "RA3/AN3/VREF+", PinType.Bidirectional));
                defs.Add((6, "RA4/T0CKI/C1OUT", PinType.Bidirectional));
                defs.Add((7, "RA5/AN4/LVDIN", PinType.Bidirectional));
                defs.Add((8, "RE0/AN5", PinType.Bidirectional));
                defs.Add((9, "RE1/AN6", PinType.Bidirectional));
                defs.Add((10, "RE2/AN7", PinType.Bidirectional));
                defs.Add((11, "VDD", PinType.Power));
                defs.Add((12, "VSS", PinType.Ground));
                defs.Add((13, "OSC1/CLKI", PinType.Input));
                defs.Add((14, "OSC2/CLKO/RA6", PinType.Output));
                defs.Add((15, "RC0/T1OSO/T13CKI", PinType.Bidirectional));
                defs.Add((16, "RC1/T1OSI/CCP2", PinType.Bidirectional));
                defs.Add((17, "RC2/CCP1", PinType.Bidirectional));
                defs.Add((18, "RC3/SCK/SCL", PinType.Bidirectional));
                defs.Add((19, "RD0/SPP0", PinType.Bidirectional));
                defs.Add((20, "RD1/SPP1", PinType.Bidirectional));
                defs.Add((21, "RD2/SPP2", PinType.Bidirectional));
                defs.Add((22, "RD3/SPP3", PinType.Bidirectional));
                defs.Add((23, "RC4/D-/VM", PinType.Bidirectional));
                defs.Add((24, "RC5/D+/VP", PinType.Bidirectional));
                defs.Add((25, "RC6/TX/CK", PinType.Bidirectional));
                defs.Add((26, "RC7/RX/DT", PinType.Bidirectional));
                defs.Add((27, "RD4/SPP4", PinType.Bidirectional));
                defs.Add((28, "RD5/SPP5", PinType.Bidirectional));
                defs.Add((29, "RD6/SPP6", PinType.Bidirectional));
                defs.Add((30, "RD7/SPP7", PinType.Bidirectional));
                defs.Add((31, "VSS", PinType.Ground));
                defs.Add((32, "VDD", PinType.Power));
                defs.Add((33, "RB0/AN12/INT0", PinType.Bidirectional));
                defs.Add((34, "RB1/AN10/INT1", PinType.Bidirectional));
                defs.Add((35, "RB2/AN8/INT2", PinType.Bidirectional));
                defs.Add((36, "RB3/AN9/CCP2", PinType.Bidirectional));
                defs.Add((37, "RB4/AN11/KBI0", PinType.Bidirectional));
                defs.Add((38, "RB5/KBI1/PGM", PinType.Bidirectional));
                defs.Add((39, "RB6/KBI2/PGC", PinType.Bidirectional));
                defs.Add((40, "RB7/KBI3/PGD", PinType.Bidirectional));
            }

            return defs.Count > 0 ? defs : null;
        }

        /// <summary>
        /// Actualiza las posiciones de los pines - VERSIÓN MEJORADA SIN LÍMITES
        /// </summary>
        protected override void UpdatePinPositions()
        {
            if (Pins.Count == 0) return;

            // ✅ SIN LÍMITE - Usar todos los pines
            int totalPins = Pins.Count;
            int leftPins = (int)Math.Ceiling(totalPins / 2.0);
            int rightPins = totalPins - leftPins;

            // Calcular espaciado dinámicamente
            int leftSpacing = leftPins > 1 ? Height / (leftPins + 1) : Height / 2;
            int rightSpacing = rightPins > 1 ? Height / (rightPins + 1) : Height / 2;

            // Pines lado izquierdo (numeración ascendente desde arriba)
            for (int i = 0; i < leftPins && i < Pins.Count; i++)
            {
                int py = Y + leftSpacing * (i + 1);
                Pins[i].Position = new Point(X - 10, py);
            }

            // Pines lado derecho (numeración continua descendente desde abajo)
            for (int i = 0; i < rightPins && (leftPins + i) < Pins.Count; i++)
            {
                int py = Y + Height - rightSpacing * (i + 1);
                Pins[leftPins + i].Position = new Point(X + Width + 10, py);
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                // Cuerpo del IC
                Rectangle body = new Rectangle(X, Y, Width, Height);
                g.FillRectangle(bodyBrush, body);
                g.DrawRectangle(pen, body);

                // Muesca de orientación (esquina superior izquierda)
                g.FillEllipse(Brushes.White, X + 5, Y + 5, 8, 8);

                // Actualizar posiciones de pines
                UpdatePinPositions();

                using (Pen pinPen = new Pen(Color.Silver, 2))
                using (Font pinFont = new Font("Arial", 6, FontStyle.Bold))
                using (Font nameFont = new Font("Arial", 5))
                using (Brush pinTextBrush = new SolidBrush(Color.Black))
                using (Brush nameTextBrush = new SolidBrush(Color.Yellow))
                {
                    int totalPins = Pins.Count;
                    int leftPins = (int)Math.Ceiling(totalPins / 2.0);

                    // ✅ DIBUJAR TODOS LOS PINES SIN LÍMITE
                    foreach (var pin in Pins)
                    {
                        bool isLeftSide = pin.Number <= leftPins;

                        // Línea del pin
                        if (isLeftSide)
                        {
                            g.DrawLine(pinPen, X - 10, pin.Position.Y, X, pin.Position.Y);
                        }
                        else
                        {
                            g.DrawLine(pinPen, X + Width, pin.Position.Y, X + Width + 10, pin.Position.Y);
                        }

                        // Círculo del pin
                        g.FillEllipse(Brushes.Red, pin.Position.X - 3, pin.Position.Y - 3, 6, 6);

                        if (ShowPinNumbers)
                        {
                            if (isLeftSide)
                            {
                                // Número a la izquierda del pin
                                string numText = pin.Number.ToString();
                                SizeF numSize = g.MeasureString(numText, pinFont);
                                g.DrawString(numText, pinFont, pinTextBrush,
                                    pin.Position.X - 20, pin.Position.Y - numSize.Height / 2);
                            }
                            else
                            {
                                // Número a la derecha del pin
                                string numText = pin.Number.ToString();
                                g.DrawString(numText, pinFont, pinTextBrush,
                                    pin.Position.X + 8, pin.Position.Y - 6);
                            }
                        }

                        if (ShowPinTypes && !string.IsNullOrEmpty(pin.Name) && pin.Name != $"Pin{pin.Number}")
                        {
                            if (isLeftSide)
                            {
                                // Nombre dentro del IC a la derecha del pin
                                g.DrawString(pin.Name, nameFont, nameTextBrush,
                                    pin.Position.X + 5, pin.Position.Y - 6);
                            }
                            else
                            {
                                // Nombre dentro del IC a la izquierda del pin
                                SizeF nameSize = g.MeasureString(pin.Name, nameFont);
                                g.DrawString(pin.Name, nameFont, nameTextBrush,
                                    pin.Position.X - nameSize.Width - 5, pin.Position.Y - 6);
                            }
                        }
                    }
                }

                // Texto central (PartNumber y Function)
                using (Font labelFont = new Font("Arial", 9, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7))
                {
                    string displayPart = !string.IsNullOrEmpty(PartNumber) ? PartNumber : "IC";
                    SizeF partSize = g.MeasureString(displayPart, labelFont);
                    g.DrawString(displayPart, labelFont, textBrush,
                        X + Width / 2 - partSize.Width / 2, Y + Height / 2 - 15);

                    if (!string.IsNullOrEmpty(Function) && Function.Length < 30)
                    {
                        SizeF funcSize = g.MeasureString(Function, valueFont);
                        g.DrawString(Function, valueFont, textBrush,
                            X + Width / 2 - funcSize.Width / 2, Y + Height / 2 + 5);
                    }
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - 10, Y - 10, Width + 20, Height + 20);
        }

        public override SchematicComponent Clone()
        {
            return new ICComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                PartNumber = this.PartNumber,
                Function = this.Function,
                SupplyVoltage = this.SupplyVoltage,
                Width = this.Width,
                Height = this.Height,
                PinCount = this.PinCount,
                ShowPinNumbers = this.ShowPinNumbers,
                ShowPinTypes = this.ShowPinTypes
            };
        }
    }

    /// <summary>
    /// Fuse (Fusible)
    /// </summary>
    [Serializable]
    public class FuseComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Fuse rating (e.g., 5A, 10A)")]
        public string Rating { get; set; }

        [Category("Electrical")]
        [Description("Fuse type (Fast, Slow, Automotive)")]
        public string FuseType { get; set; }

        [Category("Electrical")]
        [Description("Voltage rating (e.g., 250V, 32V)")]
        public string VoltageRating { get; set; }

        [Category("Geometry")]
        [Description("Component length in pixels")]
        public int Length { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public FuseComponent()
        {
            Type = ComponentType.Fuse;
            Rating = "5A";
            FuseType = "Fast";
            VoltageRating = "250V";
            Length = 60;
            Orientation = ComponentOrientation.Horizontal;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Pin1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "Pin2", PinType.Passive, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(255, 248, 220)))
            {
                if (Orientation == ComponentOrientation.Horizontal)
                {
                    // Conexiones
                    g.DrawLine(wirePen, X - 20, Y, X, Y);
                    g.DrawLine(wirePen, X + Length, Y, X + Length + 20, Y);

                    // Cuerpo del fusible (rectángulo)
                    Rectangle body = new Rectangle(X, Y - 10, Length, 20);
                    g.FillRectangle(bodyBrush, body);
                    g.DrawRectangle(pen, body);

                    // Símbolo interior (línea quebrada)
                    g.DrawLine(pen, X + 10, Y, X + Length / 2, Y - 6);
                    g.DrawLine(pen, X + Length / 2, Y - 6, X + Length - 10, Y);

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + Length / 2 - 10, Y - 28);
                        g.DrawString(Rating, valueFont, Brushes.Red, X + Length / 2 - 8, Y + 15);
                    }
                }
                else // Vertical
                {
                    // Conexiones
                    g.DrawLine(wirePen, X, Y - 20, X, Y);
                    g.DrawLine(wirePen, X, Y + Length, X, Y + Length + 20);

                    // Cuerpo del fusible
                    Rectangle body = new Rectangle(X - 10, Y, 20, Length);
                    g.FillRectangle(bodyBrush, body);
                    g.DrawRectangle(pen, body);

                    // Símbolo interior
                    g.DrawLine(pen, X, Y + 10, X - 6, Y + Length / 2);
                    g.DrawLine(pen, X - 6, Y + Length / 2, X, Y + Length - 10);

                    // Etiquetas
                    using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                    using (Font valueFont = new Font("Arial", 7))
                    {
                        g.DrawString(Name, labelFont, Brushes.Black, X + 15, Y + Length / 2 - 10);
                        g.DrawString(Rating, valueFont, Brushes.Red, X + 15, Y + Length / 2 + 2);
                    }
                }

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            if (Orientation == ComponentOrientation.Horizontal)
            {
                Bounds = new Rectangle(X - 25, Y - 30, Length + 50, 50);
            }
            else
            {
                Bounds = new Rectangle(X - 30, Y - 25, 60, Length + 50);
            }
        }

        public override SchematicComponent Clone()
        {
            return new FuseComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Rating = this.Rating,
                FuseType = this.FuseType,
                VoltageRating = this.VoltageRating,
                Length = this.Length,
                Orientation = this.Orientation
            };
        }
    }

    /// <summary>
    /// Ground Symbol
    /// </summary>
    [Serializable]
    public class GroundComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Ground type (Earth, Chassis, Signal)")]
        public string GroundType { get; set; }

        [Category("Geometry")]
        [Description("Symbol size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public GroundComponent()
        {
            Type = ComponentType.Ground;
            GroundType = "Earth";
            Size = 30;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "GND", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, isSelected ? 3 : 2))
            {
                // Conexión vertical
                g.DrawLine(pen, X, Y - 20, X, Y);

                // Símbolo de tierra (3 líneas horizontales decrecientes)
                g.DrawLine(pen, X - Size, Y, X + Size, Y);
                g.DrawLine(pen, X - Size * 2 / 3, Y + 8, X + Size * 2 / 3, Y + 8);
                g.DrawLine(pen, X - Size / 3, Y + 16, X + Size / 3, Y + 16);

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                // Etiqueta
                using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                {
                    g.DrawString(Name, labelFont, Brushes.Black, X + Size + 5, Y);
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - Size - 5, Y - 25, Size * 2 + 40, 45);
        }

        public override SchematicComponent Clone()
        {
            return new GroundComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                GroundType = this.GroundType,
                Size = this.Size
            };
        }
    }

    /// <summary>
    /// VCC Supply Symbol
    /// </summary>
    [Serializable]
    public class VccSupplyComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Supply voltage (e.g., 5V, 12V, 3.3V)")]
        public string Voltage { get; set; }

        [Category("Geometry")]
        [Description("Symbol size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public VccSupplyComponent()
        {
            Type = ComponentType.VccSupply;
            Voltage = "5V";
            Size = 20;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "VCC", PinType.Power, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Red, isSelected ? 3 : 2))
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(255, 200, 200)))
            {
                // Conexión vertical hacia abajo
                g.DrawLine(pen, X, Y, X, Y + 20);

                // Triángulo apuntando arriba (símbolo de alimentación)
                Point[] triangle = new Point[]
                {
                    new Point(X, Y - Size),
                    new Point(X - Size, Y),
                    new Point(X + Size, Y)
                };
                g.FillPolygon(fillBrush, triangle);
                g.DrawPolygon(pen, triangle);

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                // Etiquetas
                using (Font labelFont = new Font("Arial", 8, FontStyle.Bold))
                using (Font valueFont = new Font("Arial", 7, FontStyle.Bold))
                {
                    g.DrawString(Name, labelFont, Brushes.Black, X + Size + 5, Y - Size);
                    g.DrawString(Voltage, valueFont, Brushes.Red, X + Size + 5, Y - Size + 12);
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - Size - 5, Y - Size - 5, Size * 2 + 50, Size + 30);
        }

        public override SchematicComponent Clone()
        {
            return new VccSupplyComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                Voltage = this.Voltage,
                Size = this.Size
            };
        }
    }

    /// <summary>
    /// Node (Connection Point)
    /// </summary>
    [Serializable]
    public class NodeComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Node name/label")]
        public string NodeName { get; set; }

        [Category("Appearance")]
        [Description("Node color")]
        public Color NodeColor { get; set; }

        [Category("Geometry")]
        [Description("Node size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public NodeComponent()
        {
            Type = ComponentType.Node;
            NodeName = "Node";
            NodeColor = Color.Black;
            Size = 8;
            ShowPinNumbers = false;  // Nodes typically don't show numbers
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Node", PinType.Bidirectional, Point.Empty)
            };
            UpdateBounds();
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (SolidBrush brush = new SolidBrush(isSelected ? Color.Blue : NodeColor))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                // Círculo sólido
                g.FillEllipse(brush, X - Size / 2, Y - Size / 2, Size, Size);
                g.DrawEllipse(pen, X - Size / 2, Y - Size / 2, Size, Size);

                if (isHovered)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }

                // Etiqueta (solo si tiene nombre personalizado)
                if (!string.IsNullOrEmpty(NodeName) && NodeName != "Node")
                {
                    using (Font labelFont = new Font("Arial", 7))
                    {
                        g.DrawString(NodeName, labelFont, Brushes.Black, X + Size, Y - 5);
                    }
                }
            }
        }

        public override bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(X - Size - 2, Y - Size - 2, Size * 2 + 30, Size + 10);
        }

        public override SchematicComponent Clone()
        {
            return new NodeComponent
            {
                Name = this.Name + "_copy",
                X = this.X + 50,
                Y = this.Y + 50,
                NodeName = this.NodeName,
                NodeColor = this.NodeColor,
                Size = this.Size
            };
        }
    }

    /// <summary>
    /// Wire/Connection
    /// </summary>
    [Serializable]
    public class WireComponent : SchematicComponent
    {
        private int selectedHandle = -1; // 0 = start, 1 = end, -1 = none

        [Category("Geometry")]
        [Description("Start X coordinate")]
        public int X1 { get; set; }

        [Category("Geometry")]
        [Description("Start Y coordinate")]
        public int Y1 { get; set; }

        [Category("Geometry")]
        [Description("End X coordinate")]
        public int X2 { get; set; }

        [Category("Geometry")]
        [Description("End Y coordinate")]
        public int Y2 { get; set; }

        [Category("Appearance")]
        [Description("Wire color")]
        public Color WireColor { get; set; }

        [Category("Appearance")]
        [Description("Wire thickness")]
        public float Thickness { get; set; }

        [Category("Electrical")]
        [Description("Wire type (Power, Signal, Ground)")]
        public string WireType { get; set; }

        [Browsable(false)]
        public int SelectedHandle
        {
            get { return selectedHandle; }
            set { selectedHandle = value; }
        }

        public WireComponent()
        {
            Type = ComponentType.Wire;
            WireColor = Color.Black;
            Thickness = 2.5f;
            WireType = "Signal";
            Orientation = ComponentOrientation.Horizontal;
            UpdateBounds();
        }

        public override ComponentOrientation Orientation
        {
            get
            {
                // Determinar orientación automáticamente
                int deltaX = Math.Abs(X2 - X1);
                int deltaY = Math.Abs(Y2 - Y1);
                return deltaX > deltaY ? ComponentOrientation.Horizontal : ComponentOrientation.Vertical;
            }
            set
            {
                // Al cambiar orientación, ajustar coordenadas
                if (value == ComponentOrientation.Horizontal)
                {
                    Y2 = Y1; // Forzar misma Y
                }
                else
                {
                    X2 = X1; // Forzar misma X
                }
                UpdateBounds();
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            using (Pen pen = new Pen(isSelected ? Color.Blue : WireColor, isSelected ? Thickness + 1 : Thickness))
            {
                g.DrawLine(pen, X1, Y1, X2, Y2);

                if (isHovered || isSelected)
                {
                    // Dibujar puntos de control (handles) en los extremos
                    DrawHandle(g, X1, Y1, selectedHandle == 0 || isSelected);
                    DrawHandle(g, X2, Y2, selectedHandle == 1 || isSelected);
                }

                if (isHovered && !isSelected)
                {
                    using (Pen hoverPen = new Pen(Color.Orange, 2))
                    {
                        hoverPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(hoverPen, Bounds);
                    }
                }
            }
        }

        private void DrawHandle(Graphics g, int x, int y, bool highlight)
        {
            int handleSize = 6;
            Color handleColor = highlight ? Color.Blue : Color.Orange;
            using (Brush handleBrush = new SolidBrush(handleColor))
            using (Pen handlePen = new Pen(Color.Black, 1))
            {
                g.FillEllipse(handleBrush, x - handleSize / 2, y - handleSize / 2, handleSize, handleSize);
                g.DrawEllipse(handlePen, x - handleSize / 2, y - handleSize / 2, handleSize, handleSize);
            }
        }

        public override bool HitTest(Point point)
        {
            // Primero verificar si hizo click en un handle
            if (HitTestHandle(point, X1, Y1))
            {
                selectedHandle = 0;
                return true;
            }
            if (HitTestHandle(point, X2, Y2))
            {
                selectedHandle = 1;
                return true;
            }

            // Luego verificar la línea
            double distance = DistanceToLine(point, new Point(X1, Y1), new Point(X2, Y2));
            if (distance <= 5)
            {
                selectedHandle = -1;
                return true;
            }

            return false;
        }

        private bool HitTestHandle(Point point, int handleX, int handleY)
        {
            int handleSize = 8;
            Rectangle handleRect = new Rectangle(handleX - handleSize / 2, handleY - handleSize / 2, handleSize, handleSize);
            return handleRect.Contains(point);
        }

        private double DistanceToLine(Point p, Point lineStart, Point lineEnd)
        {
            double length = Math.Sqrt(Math.Pow(lineEnd.X - lineStart.X, 2) + Math.Pow(lineEnd.Y - lineStart.Y, 2));
            if (length == 0) return Math.Sqrt(Math.Pow(p.X - lineStart.X, 2) + Math.Pow(p.Y - lineStart.Y, 2));

            double t = Math.Max(0, Math.Min(1, ((p.X - lineStart.X) * (lineEnd.X - lineStart.X) +
                                                  (p.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / (length * length)));

            double projX = lineStart.X + t * (lineEnd.X - lineStart.X);
            double projY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);

            return Math.Sqrt(Math.Pow(p.X - projX, 2) + Math.Pow(p.Y - projY, 2));
        }

        protected override void UpdateBounds()
        {
            int minX = Math.Min(X1, X2) - 10;
            int minY = Math.Min(Y1, Y2) - 10;
            int width = Math.Abs(X2 - X1) + 20;
            int height = Math.Abs(Y2 - Y1) + 20;
            Bounds = new Rectangle(minX, minY, width, height);
        }

        public override void Move(int deltaX, int deltaY)
        {
            if (selectedHandle == 0)
            {
                // Mover solo el punto inicial
                X1 += deltaX;
                Y1 += deltaY;
            }
            else if (selectedHandle == 1)
            {
                // Mover solo el punto final
                X2 += deltaX;
                Y2 += deltaY;
            }
            else
            {
                // Mover toda la línea
                X1 += deltaX;
                Y1 += deltaY;
                X2 += deltaX;
                Y2 += deltaY;
            }

            X = X1;
            Y = Y1;
            UpdateBounds();
        }

        public void MoveHandle(int handleIndex, int newX, int newY, bool maintainOrientation)
        {
            if (handleIndex == 0)
            {
                X1 = newX;
                Y1 = newY;
                if (maintainOrientation)
                {
                    if (Orientation == ComponentOrientation.Horizontal)
                        Y1 = Y2;
                    else
                        X1 = X2;
                }
            }
            else if (handleIndex == 1)
            {
                X2 = newX;
                Y2 = newY;
                if (maintainOrientation)
                {
                    if (Orientation == ComponentOrientation.Horizontal)
                        Y2 = Y1;
                    else
                        X2 = X1;
                }
            }
            UpdateBounds();
        }

        public override SchematicComponent Clone()
        {
            return new WireComponent
            {
                Name = this.Name + "_copy",
                X1 = this.X1 + 50,
                Y1 = this.Y1 + 50,
                X2 = this.X2 + 50,
                Y2 = this.Y2 + 50,
                WireColor = this.WireColor,
                Thickness = this.Thickness,
                WireType = this.WireType
            };
        }
    }
}

/// <summary>
/// Label de texto
/// </summary>
[Serializable]
public class LabelComponent : SchematicComponent
{
    [Category("Appearance")]
    [Description("Label text")]
    public string Text { get; set; }

    [Category("Appearance")]
    [Description("Font name")]
    public string FontName { get; set; }

    [Category("Appearance")]
    [Description("Font size")]
    public float FontSize { get; set; }

    [Category("Appearance")]
    [Description("Text color")]
    public Color TextColor { get; set; }

    [Category("Appearance")]
    [Description("Bold text")]
    public bool IsBold { get; set; }

    public LabelComponent()
    {
        Type = ComponentType.Label;
        Text = "Label";
        FontName = "Arial";
        FontSize = 10;
        TextColor = Color.Black;
        IsBold = false;
        UpdateBounds();
    }

    public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
    {
        using (Font font = new Font(FontName, FontSize, IsBold ? FontStyle.Bold : FontStyle.Regular))
        using (Brush brush = new SolidBrush(isSelected ? Color.Blue : TextColor))
        {
            g.DrawString(Text, font, brush, X, Y);

            if (isHovered || isSelected)
            {
                using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Orange, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawRectangle(pen, Bounds);
                }
            }
        }
    }

    public override bool HitTest(Point point)
    {
        return Bounds.Contains(point);
    }

    protected override void UpdateBounds()
    {
        using (Font font = new Font(FontName, FontSize))
        using (Bitmap bmp = new Bitmap(1, 1))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            SizeF size = g.MeasureString(Text, font);
            Bounds = new Rectangle(X, Y, (int)size.Width + 5, (int)size.Height + 5);
        }
    }

    public override SchematicComponent Clone()
    {
        return new LabelComponent
        {
            Name = this.Name + "_copy",
            X = this.X + 50,
            Y = this.Y + 50,
            Text = this.Text,
            FontName = this.FontName,
            FontSize = this.FontSize,
            TextColor = this.TextColor,
            IsBold = this.IsBold
        };
    }

    /// <summary>
    /// Buzzer Component
    /// </summary>
    [Serializable]
    public class BuzzerComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Operating voltage")]
        public string Voltage { get; set; }

        [Category("Electrical")]
        [Description("Frequency (Hz)")]
        public string Frequency { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public BuzzerComponent()
        {
            this.Type = ComponentType.Buzzer;
            this.Name = "BZ1";
            Voltage = "5V";
            Frequency = "2KHz";
            Size = 40;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "+", PinType.Power, Point.Empty),
                new ComponentPin(2, "-", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            Pins[0].Position = new Point(this.X - Size / 2, this.Y + Size);
            Pins[1].Position = new Point(this.X + Size / 2, this.Y + Size);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                g.FillEllipse(bodyBrush, this.X - Size / 2, this.Y - Size / 2, Size, Size);
                g.DrawEllipse(pen, this.X - Size / 2, this.Y - Size / 2, Size, Size);

                using (Pen soundPen = new Pen(Color.Yellow, 2))
                {
                    g.DrawArc(soundPen, this.X + Size / 4, this.Y - Size / 4, 15, 15, -30, 60);
                }

                g.DrawLine(wirePen, this.X - Size / 2, this.Y + Size / 2, this.X - Size / 2, this.Y + Size);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y + Size / 2, this.X + Size / 2, this.Y + Size);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, font, Brushes.Black, this.X - 10, this.Y - Size / 2 - 15);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - Size / 2 - 10, this.Y - Size / 2 - 20, Size + 20, Size + 30);
        public override SchematicComponent Clone() => new BuzzerComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, Voltage = this.Voltage, Frequency = this.Frequency, Size = this.Size };
    }

    /// <summary>
    /// Bridge Rectifier
    /// </summary>
    [Serializable]
    public class BridgeDiodeComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Current rating")]
        public string CurrentRating { get; set; }

        [Category("Electrical")]
        [Description("Voltage rating")]
        public string VoltageRating { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public BridgeDiodeComponent()
        {
            this.Type = ComponentType.BridgeDiode;
            this.Name = "BR1";
            CurrentRating = "1A";
            VoltageRating = "400V";
            Size = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "AC1", PinType.Input, Point.Empty),
                new ComponentPin(2, "AC2", PinType.Input, Point.Empty),
                new ComponentPin(3, "+", PinType.Output, Point.Empty),
                new ComponentPin(4, "-", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 4) return;
            Pins[0].Position = new Point(this.X - Size, this.Y);
            Pins[1].Position = new Point(this.X + Size, this.Y);
            Pins[2].Position = new Point(this.X, this.Y - Size);
            Pins[3].Position = new Point(this.X, this.Y + Size);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] diamond = {
                    new Point(this.X, this.Y - Size/2),
                    new Point(this.X + Size/2, this.Y),
                    new Point(this.X, this.Y + Size/2),
                    new Point(this.X - Size/2, this.Y)
                };
                g.DrawPolygon(pen, diamond);

                g.DrawLine(wirePen, this.X - Size, this.Y, this.X - Size / 2, this.Y);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y, this.X + Size, this.Y);
                g.DrawLine(wirePen, this.X, this.Y - Size / 2, this.X, this.Y - Size);
                g.DrawLine(wirePen, this.X, this.Y + Size / 2, this.X, this.Y + Size);

                using (Font font = new Font("Arial", 6))
                {
                    g.DrawString("+", font, Brushes.Red, this.X - 5, this.Y - Size - 15);
                    g.DrawString("-", font, Brushes.Blue, this.X - 5, this.Y + Size + 5);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - Size - 10, this.Y - Size - 20, Size * 2 + 20, Size * 2 + 30);
        public override SchematicComponent Clone() => new BridgeDiodeComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, CurrentRating = this.CurrentRating, VoltageRating = this.VoltageRating };
    }

    /// <summary>
    /// Schottky Diode
    /// </summary>
    [Serializable]
    public class SchottkyDiodeComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Forward voltage")]
        public string VF { get; set; }

        [Category("Electrical")]
        [Description("Current rating")]
        public string CurrentRating { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public SchottkyDiodeComponent()
        {
            this.Type = ComponentType.SchottkyDiode;
            this.Name = "D1";
            this.Orientation = ComponentOrientation.Horizontal;
            VF = "0.3V";
            CurrentRating = "1A";
            Size = 40;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            if (this.Orientation == ComponentOrientation.Horizontal)
            {
                Pins[0].Position = new Point(this.X - 20, this.Y);
                Pins[1].Position = new Point(this.X + Size + 20, this.Y);
            }
            else
            {
                Pins[0].Position = new Point(this.X, this.Y - 20);
                Pins[1].Position = new Point(this.X, this.Y + Size + 20);
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                if (this.Orientation == ComponentOrientation.Horizontal)
                {
                    Point[] triangle = { new Point(this.X, this.Y - 12), new Point(this.X, this.Y + 12), new Point(this.X + Size - 10, this.Y) };
                    g.DrawPolygon(pen, triangle);
                    g.DrawLine(pen, this.X + Size - 10, this.Y - 12, this.X + Size - 10, this.Y + 12);
                    g.DrawLine(pen, this.X + Size - 10, this.Y - 12, this.X + Size - 15, this.Y - 12);
                    g.DrawLine(pen, this.X + Size - 10, this.Y + 12, this.X + Size - 5, this.Y + 12);
                    g.DrawLine(wirePen, this.X - 20, this.Y, this.X, this.Y);
                    g.DrawLine(wirePen, this.X + Size - 10, this.Y, this.X + Size + 20, this.Y);
                }

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, font, Brushes.Black, this.X + 5, this.Y - 20);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - 30, Size + 60, 60);
        public override SchematicComponent Clone() => new SchottkyDiodeComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, VF = this.VF, CurrentRating = this.CurrentRating, Orientation = this.Orientation };
    }

    /// <summary>
    /// Zener Diode
    /// </summary>
    [Serializable]
    public class ZenerDiodeComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Zener voltage")]
        public string ZenerVoltage { get; set; }

        [Category("Electrical")]
        [Description("Power rating")]
        public string Power { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public ZenerDiodeComponent()
        {
            this.Type = ComponentType.ZenerDiode;
            this.Name = "ZD1";
            this.Orientation = ComponentOrientation.Horizontal;
            ZenerVoltage = "5.1V";
            Power = "500mW";
            Size = 40;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            if (this.Orientation == ComponentOrientation.Horizontal)
            {
                Pins[0].Position = new Point(this.X - 20, this.Y);
                Pins[1].Position = new Point(this.X + Size + 20, this.Y);
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] triangle = { new Point(this.X, this.Y - 12), new Point(this.X, this.Y + 12), new Point(this.X + Size - 10, this.Y) };
                g.DrawPolygon(pen, triangle);
                g.DrawLine(pen, this.X + Size - 10, this.Y - 12, this.X + Size - 10, this.Y + 12);
                g.DrawLine(pen, this.X + Size - 10, this.Y - 12, this.X + Size - 5, this.Y - 12);
                g.DrawLine(pen, this.X + Size - 10, this.Y + 12, this.X + Size - 15, this.Y + 12);
                g.DrawLine(wirePen, this.X - 20, this.Y, this.X, this.Y);
                g.DrawLine(wirePen, this.X + Size - 10, this.Y, this.X + Size + 20, this.Y);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name + " " + ZenerVoltage, font, Brushes.Blue, this.X + 5, this.Y - 20);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - 30, Size + 60, 60);
        public override SchematicComponent Clone() => new ZenerDiodeComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, ZenerVoltage = this.ZenerVoltage, Power = this.Power };
    }

    /// <summary>
    /// LED
    /// </summary>
    [Serializable]
    public class LEDComponent : SchematicComponent
    {
        private string _ledColor;

        [Category("Electrical")]
        [Description("LED color")]
        [TypeConverter(typeof(LEDColorConverter))]
        public string LEDColor
        {
            get => _ledColor;
            set
            {
                if (_ledColor != value)
                {
                    _ledColor = value;
                }
            }
        }

        [Category("Electrical")]
        [Description("Forward voltage")]
        public string VF { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public LEDComponent()
        {
            this.Type = ComponentType.LED;
            this.Name = "LED1";
            this.Orientation = ComponentOrientation.Horizontal;
            _ledColor = "Red";
            VF = "2.0V";
            Size = 40;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        private Color GetLEDDrawColor()
        {
            switch (LEDColor)
            {
                case "Red": return Color.Red;
                case "Green": return Color.Lime;
                case "Blue": return Color.Blue;
                case "Yellow": return Color.Yellow;
                case "White": return Color.White;
                case "Orange": return Color.Orange;
                case "Infrared": return Color.DarkRed;
                case "UV": return Color.Purple;
                default: return Color.Red;
            }
        }
        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            if (this.Orientation == ComponentOrientation.Horizontal)
            {
                Pins[0].Position = new Point(this.X - 20, this.Y);
                Pins[1].Position = new Point(this.X + Size + 20, this.Y);
            }
        }

        private Color GetLEDColor()
        {
            switch (LEDColor.ToLower())
            {
                case "red": return Color.Red;
                case "green": return Color.Green;
                case "blue": return Color.Blue;
                case "yellow": return Color.Yellow;
                case "white": return Color.White;
                default: return Color.Red;
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Color ledColor = GetLEDColor();
                using (Brush ledBrush = new SolidBrush(Color.FromArgb(100, ledColor)))
                {
                    Point[] triangle = { new Point(this.X, this.Y - 12), new Point(this.X, this.Y + 12), new Point(this.X + Size - 10, this.Y) };
                    g.FillPolygon(ledBrush, triangle);
                    g.DrawPolygon(pen, triangle);
                    g.DrawLine(pen, this.X + Size - 10, this.Y - 12, this.X + Size - 10, this.Y + 12);

                    /*using (Pen lightPen = new Pen(ledColor, 1))
                    {
                        g.DrawLine(lightPen, this.X + Size - 5, this.Y - 15, this.X + Size + 5, this.Y - 20);
                        g.DrawLine(lightPen, this.X + Size - 5, this.Y - 10, this.X + Size + 5, this.Y - 15);
                    }*/

                    Color ledColor1 = GetLEDDrawColor();
                    using (Pen ledPen = new Pen(ledColor1, 2))
                    {
                        // Dibujar rayos de luz
                        for (int i = 0; i < 3; i++)
                        {
                            int angle = -45 + i * 30;
                            double rad = angle * Math.PI / 180;
                            int x2 = this.X + 30 + (int)(15 * Math.Cos(rad));
                            int y2 = this.Y - 10 + (int)(15 * Math.Sin(rad));
                            g.DrawLine(ledPen, this.X + 30, this.Y - 10, x2, y2);
                        }
                    }

                    g.DrawLine(wirePen, this.X - 20, this.Y, this.X, this.Y);
                    g.DrawLine(wirePen, this.X + Size - 10, this.Y, this.X + Size + 20, this.Y);
                }

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name + " (" + LEDColor + ")", font, Brushes.Black, this.X + 5, this.Y + 15);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - 30, Size + 60, 60);
        public override SchematicComponent Clone() => new LEDComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, LEDColor = this.LEDColor, VF = this.VF };
    }

    /// <summary>
    /// Potentiometer
    /// </summary>
    [Serializable]
    public class PotentiometerComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Resistance value")]
        public string Value { get; set; }

        [Category("Electrical")]
        [Description("Power rating")]
        public string Power { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public PotentiometerComponent()
        {
            this.Type = ComponentType.Potentiometer;
            this.Name = "POT1";
            Value = "10kΩ";
            Power = "1/4W";
            Size = 50;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "Pin1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "Wiper", PinType.Bidirectional, Point.Empty),
                new ComponentPin(3, "Pin3", PinType.Passive, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 3) return;
            Pins[0].Position = new Point(this.X - Size / 2, this.Y + Size);
            Pins[1].Position = new Point(this.X, this.Y - 20);
            Pins[2].Position = new Point(this.X + Size / 2, this.Y + Size);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(210, 180, 140)))
            {
                Rectangle body = new Rectangle(this.X - Size / 2, this.Y, Size, Size / 2);
                g.FillRectangle(bodyBrush, body);
                g.DrawRectangle(pen, body);

                g.DrawLine(pen, this.X, this.Y - 20, this.X, this.Y);
                g.DrawLine(pen, this.X - 5, this.Y - 5, this.X, this.Y);
                g.DrawLine(pen, this.X + 5, this.Y - 5, this.X, this.Y);

                g.DrawLine(wirePen, this.X - Size / 2, this.Y + Size / 2, this.X - Size / 2, this.Y + Size);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y + Size / 2, this.X + Size / 2, this.Y + Size);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name + " " + Value, font, Brushes.Black, this.X - 20, this.Y + Size + 5);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - Size / 2 - 10, this.Y - 30, Size + 20, Size + 40);
        public override SchematicComponent Clone() => new PotentiometerComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, Value = this.Value, Power = this.Power };
    }

    /// <summary>
    /// Battery
    /// </summary>
    [Serializable]
    public class BatteryComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Voltage")]
        public string Voltage { get; set; }

        [Category("Electrical")]
        [Description("Capacity (mAh)")]
        public string Capacity { get; set; }

        [Category("Geometry")]
        [Description("Component height")]
        public int Height { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public BatteryComponent()
        {
            this.Type = ComponentType.Battery;
            this.Name = "BAT1";
            this.Orientation = ComponentOrientation.Vertical;
            Voltage = "9V";
            Capacity = "500mAh";
            Height = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "+", PinType.Power, Point.Empty),
                new ComponentPin(2, "-", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            Pins[0].Position = new Point(this.X, this.Y - 20);
            Pins[1].Position = new Point(this.X, this.Y + Height + 20);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                g.DrawLine(pen, this.X - 20, this.Y, this.X + 20, this.Y);
                g.DrawLine(pen, this.X - 10, this.Y + 15, this.X + 10, this.Y + 15);
                g.DrawLine(pen, this.X - 20, this.Y + 30, this.X + 20, this.Y + 30);
                g.DrawLine(pen, this.X - 10, this.Y + 45, this.X + 10, this.Y + 45);

                g.DrawLine(wirePen, this.X, this.Y - 20, this.X, this.Y);
                g.DrawLine(wirePen, this.X, this.Y + Height, this.X, this.Y + Height + 20);

                using (Pen redPen = new Pen(Color.Red, 2))
                using (Pen bluePen = new Pen(Color.Blue, 2))
                {
                    g.DrawLine(redPen, this.X - 30, this.Y, this.X - 25, this.Y);
                    g.DrawLine(redPen, this.X - 27.5f, this.Y - 2.5f, this.X - 27.5f, this.Y + 2.5f);
                    g.DrawLine(bluePen, this.X - 30, this.Y + Height, this.X - 25, this.Y + Height);
                }

                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                {
                    g.DrawString(Voltage, font, Brushes.Black, this.X + 25, this.Y + Height / 2 - 10);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 40, this.Y - 30, 80, Height + 60);
        public override SchematicComponent Clone() => new BatteryComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, Voltage = this.Voltage, Capacity = this.Capacity };
    }

    /// <summary>
    /// Transformer
    /// </summary>
    [Serializable]
    public class TransformerComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Turns ratio (e.g., 10:1)")]
        public string TurnsRatio { get; set; }

        [Category("Electrical")]
        [Description("Power rating")]
        public string PowerRating { get; set; }

        [Category("Geometry")]
        [Description("Component height")]
        public int Height { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public TransformerComponent()
        {
            this.Type = ComponentType.Transformer;
            this.Name = "T1";
            TurnsRatio = "10:1";
            PowerRating = "10VA";
            Height = 80;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "P1", PinType.Input, Point.Empty),
                new ComponentPin(2, "P2", PinType.Input, Point.Empty),
                new ComponentPin(3, "S1", PinType.Output, Point.Empty),
                new ComponentPin(4, "S2", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 4) return;
            Pins[0].Position = new Point(this.X - 60, this.Y);
            Pins[1].Position = new Point(this.X - 60, this.Y + Height);
            Pins[2].Position = new Point(this.X + 60, this.Y);
            Pins[3].Position = new Point(this.X + 60, this.Y + Height);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                for (int i = 0; i < 4; i++)
                {
                    g.DrawArc(pen, this.X - 40, this.Y + i * 20, 20, 20, 90, 180);
                }

                for (int i = 0; i < 4; i++)
                {
                    g.DrawArc(pen, this.X + 20, this.Y + i * 20, 20, 20, -90, 180);
                }

                using (Pen corePen = new Pen(Color.Gray, 3))
                {
                    g.DrawLine(corePen, this.X - 5, this.Y - 10, this.X - 5, this.Y + Height + 10);
                    g.DrawLine(corePen, this.X + 5, this.Y - 10, this.X + 5, this.Y + Height + 10);
                }

                g.DrawLine(wirePen, this.X - 60, this.Y, this.X - 40, this.Y);
                g.DrawLine(wirePen, this.X - 60, this.Y + Height, this.X - 40, this.Y + Height);
                g.DrawLine(wirePen, this.X + 40, this.Y, this.X + 60, this.Y);
                g.DrawLine(wirePen, this.X + 40, this.Y + Height, this.X + 60, this.Y + Height);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(TurnsRatio, font, Brushes.Black, this.X - 15, this.Y - 20);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 70, this.Y - 30, 140, Height + 40);
        public override SchematicComponent Clone() => new TransformerComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, TurnsRatio = this.TurnsRatio, PowerRating = this.PowerRating };
    }

    /// <summary>
    /// Operational Amplifier (OpAmp)
    /// </summary>
    [Serializable]
    public class OpAmpComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Part number")]
        public string PartNumber { get; set; }

        [Category("Electrical")]
        [Description("Supply voltage")]
        public string SupplyVoltage { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public OpAmpComponent()
        {
            this.Type = ComponentType.OpAmp;
            this.Name = "U1";
            PartNumber = "LM358";
            SupplyVoltage = "±15V";
            Size = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "V+", PinType.Input, Point.Empty),
                new ComponentPin(2, "V-", PinType.Input, Point.Empty),
                new ComponentPin(3, "OUT", PinType.Output, Point.Empty),
                new ComponentPin(4, "VCC", PinType.Power, Point.Empty),
                new ComponentPin(5, "GND", PinType.Ground, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 5) return;
            Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
            Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);
            Pins[2].Position = new Point(this.X + Size + 20, this.Y);
            Pins[3].Position = new Point(this.X + Size / 2, this.Y - Size / 2 - 20);
            Pins[4].Position = new Point(this.X + Size / 2, this.Y + Size / 2 + 20);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] triangle = {
                    new Point(this.X, this.Y - Size/2),
                    new Point(this.X, this.Y + Size/2),
                    new Point(this.X + Size, this.Y)
                };
                g.DrawPolygon(pen, triangle);

                g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y, this.X + Size + 20, this.Y);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y - Size / 2, this.X + Size / 2, this.Y - Size / 2 - 20);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y + Size / 2, this.X + Size / 2, this.Y + Size / 2 + 20);

                using (Font font = new Font("Arial", 10))
                {
                    g.DrawString("+", font, Brushes.Black, this.X + 5, this.Y - Size / 4 - 8);
                    g.DrawString("-", font, Brushes.Black, this.X + 5, this.Y + Size / 4 - 8);
                }

                using (Font labelFont = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, labelFont, Brushes.Black, this.X + Size / 2 - 10, this.Y - Size / 2 - 35);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 30, Size + 50, Size + 60);
        public override SchematicComponent Clone() => new OpAmpComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, PartNumber = this.PartNumber, SupplyVoltage = this.SupplyVoltage };
    }


    /// <summary>
    /// Logic Gate (AND, OR, NOT, NAND, NOR, XOR, XNOR) - CON TYPECONVERTER FUNCIONAL
    /// </summary>
    [Serializable]
    public class LogicGateComponent : SchematicComponent
    {
        private string _gateType = "AND";

        [Category("Electrical")]
        [Description("Gate type (AND, OR, NOT, NAND, NOR, XOR, XNOR)")]
        [TypeConverter(typeof(LogicGateTypeConverter))]
        public string GateType
        {
            get => _gateType;
            set
            {
                if (_gateType != value)
                {
                    _gateType = value;
                    UpdateBounds();
                }
            }
        }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public LogicGateComponent()
        {
            this.Type = ComponentType.LogicGate;
            this.Name = "U1";
            _gateType = "AND";
            Size = 50;
            ShowPinNumbers = true;

            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "B", PinType.Input, Point.Empty),
                new ComponentPin(3, "Y", PinType.Output, Point.Empty)
            };

            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 3) return;

            if (GateType == "NOT" || GateType == "INVERTER")
            {
                // NOT gate solo usa un input
                Pins[0].Position = new Point(this.X - 20, this.Y);
                Pins[1].Position = new Point(this.X - 20, this.Y); // Oculto
                Pins[2].Position = new Point(this.X + Size + 20, this.Y);
            }
            else
            {
                Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
                Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);

                int bubbleOffset = (GateType.Contains("N") && GateType != "NOT") ? 10 : 0;
                Pins[2].Position = new Point(this.X + Size + bubbleOffset + 20, this.Y);
            }
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                if (GateType == "NOT" || GateType == "INVERTER")
                {
                    // NOT gate - triángulo con círculo
                    Point[] triangle = {
                        new Point(this.X, this.Y - Size/2),
                        new Point(this.X, this.Y + Size/2),
                        new Point(this.X + Size, this.Y)
                    };
                    g.DrawPolygon(pen, triangle);
                    g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);

                    // Wires
                    g.DrawLine(wirePen, this.X - 20, this.Y, this.X, this.Y);
                    g.DrawLine(wirePen, this.X + Size + 10, this.Y, this.X + Size + 20, this.Y);
                }
                else if (GateType == "AND" || GateType == "NAND")
                {
                    // AND gate - rectángulo con semicírculo
                    g.DrawLine(pen, this.X, this.Y - Size / 2, this.X, this.Y + Size / 2);
                    g.DrawLine(pen, this.X, this.Y - Size / 2, this.X + Size / 2, this.Y - Size / 2);
                    g.DrawLine(pen, this.X, this.Y + Size / 2, this.X + Size / 2, this.Y + Size / 2);
                    g.DrawArc(pen, this.X, this.Y - Size / 2, Size, Size, -90, 180);

                    if (GateType == "NAND")
                        g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);

                    // Wires
                    g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                    g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);

                    int bubbleOffset = GateType == "NAND" ? 10 : 0;
                    g.DrawLine(wirePen, this.X + Size + bubbleOffset, this.Y, this.X + Size + 20, this.Y);
                }
                else if (GateType == "OR" || GateType == "NOR")
                {
                    // OR gate - forma curva
                    g.DrawArc(pen, this.X - Size / 4, this.Y - Size / 2, Size / 2, Size, -90, 180);
                    g.DrawArc(pen, this.X, this.Y - Size / 2, Size, Size, -90, 180);

                    if (GateType == "NOR")
                        g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);

                    // Wires
                    g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X + 5, this.Y - Size / 4);
                    g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X + 5, this.Y + Size / 4);

                    int bubbleOffset = GateType == "NOR" ? 10 : 0;
                    g.DrawLine(wirePen, this.X + Size + bubbleOffset, this.Y, this.X + Size + 20, this.Y);
                }
                else if (GateType == "XOR" || GateType == "XNOR")
                {
                    // XOR gate - doble arco de entrada
                    g.DrawArc(pen, this.X - Size / 3, this.Y - Size / 2, Size / 2, Size, -90, 180);
                    g.DrawArc(pen, this.X - Size / 4, this.Y - Size / 2, Size / 2, Size, -90, 180);
                    g.DrawArc(pen, this.X, this.Y - Size / 2, Size, Size, -90, 180);

                    if (GateType == "XNOR")
                        g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);

                    // Wires
                    g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X + 5, this.Y - Size / 4);
                    g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X + 5, this.Y + Size / 4);

                    int bubbleOffset = GateType == "XNOR" ? 10 : 0;
                    g.DrawLine(wirePen, this.X + Size + bubbleOffset, this.Y, this.X + Size + 20, this.Y);
                }

                // Etiqueta del tipo de compuerta
                using (Font font = new Font("Arial", 7, FontStyle.Bold))
                {
                    string label = GateType == "INVERTER" ? "NOT" : GateType;
                    SizeF textSize = g.MeasureString(label, font);
                    g.DrawString(label, font, Brushes.Black,
                        this.X + Size / 2 - textSize.Width / 2,
                        this.Y - textSize.Height / 2);
                }

                // Nombre del componente
                using (Font labelFont = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, labelFont, Brushes.Black, this.X + Size / 2 - 10, this.Y - Size / 2 - 15);
                }

                // Dibujar pines
                if (Pins != null)
                {
                    foreach (var pin in Pins)
                    {
                        // Solo dibujar pines visibles (NOT gate oculta pin 2)
                        if (!(GateType == "NOT" || GateType == "INVERTER") || pin.Number != 2)
                        {
                            pin.Draw(g, ShowPinNumbers, false);
                        }
                    }
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 20, Size + 60, Size + 40);
        public override SchematicComponent Clone() => new LogicGateComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, GateType = this.GateType };
    }

    /*/// <summary>
    /// Logic Gate (AND, OR, NOT, NAND, NOR, XOR, XNOR)
    /// </summary>
    [Serializable]
    public class LogicGateComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Gate type")]
        public string GateType { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public LogicGateComponent()
        {
            this.Type = ComponentType.LogicGate;
            this.Name = "U1";
            GateType = "AND";
            Size = 50;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "B", PinType.Input, Point.Empty),
                new ComponentPin(3, "Y", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 3) return;
            Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
            Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);
            Pins[2].Position = new Point(this.X + Size + 20, this.Y);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                if (GateType == "NOT" || GateType == "INVERTER")
                {
                    Point[] triangle = {
                        new Point(this.X, this.Y - Size/2),
                        new Point(this.X, this.Y + Size/2),
                        new Point(this.X + Size, this.Y)
                    };
                    g.DrawPolygon(pen, triangle);
                    g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);
                }
                else if (GateType == "AND" || GateType == "NAND")
                {
                    g.DrawLine(pen, this.X, this.Y - Size / 2, this.X, this.Y + Size / 2);
                    g.DrawLine(pen, this.X, this.Y - Size / 2, this.X + Size / 2, this.Y - Size / 2);
                    g.DrawLine(pen, this.X, this.Y + Size / 2, this.X + Size / 2, this.Y + Size / 2);
                    g.DrawArc(pen, this.X, this.Y - Size / 2, Size, Size, -90, 180);
                    if (GateType == "NAND")
                        g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);
                }
                else if (GateType == "OR" || GateType == "NOR")
                {
                    g.DrawArc(pen, this.X - Size / 4, this.Y - Size / 2, Size / 2, Size, -90, 180);
                    g.DrawArc(pen, this.X, this.Y - Size / 2, Size, Size, -90, 180);
                    if (GateType == "NOR")
                        g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);
                }

                g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);
                g.DrawLine(wirePen, this.X + Size + (GateType.Contains("N") && GateType != "NOT" ? 10 : 0), this.Y, this.X + Size + 20, this.Y);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(GateType, font, Brushes.Black, this.X + 5, this.Y - 8);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 10, Size + 60, Size + 20);
        public override SchematicComponent Clone() => new LogicGateComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, GateType = this.GateType };
    }*/


    /// <summary>
    /// Flip-Flop (D, JK, SR, T) - CON TYPECONVERTER FUNCIONAL
    /// </summary>
    [Serializable]
    public class FlipFlopComponent : SchematicComponent
    {
        private string _ffType = "D";

        [Category("Electrical")]
        [Description("Flip-Flop type (D, JK, SR, T)")]
        [TypeConverter(typeof(FlipFlopTypeConverter))]
        public string FFType
        {
            get => _ffType;
            set
            {
                if (_ffType != value)
                {
                    _ffType = value;
                    UpdatePinNames();
                    UpdateBounds();
                }
            }
        }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public FlipFlopComponent()
        {
            this.Type = ComponentType.FlipFlop;
            this.Name = "FF1";
            _ffType = "D";
            Size = 60;
            ShowPinNumbers = true;

            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "D", PinType.Input, Point.Empty),
                new ComponentPin(2, "CLK", PinType.Input, Point.Empty),
                new ComponentPin(3, "Q", PinType.Output, Point.Empty),
                new ComponentPin(4, "Q̄", PinType.Output, Point.Empty)
            };

            UpdateBounds();
        }

        private void UpdatePinNames()
        {
            if (Pins == null || Pins.Count < 4) return;

            switch (FFType)
            {
                case "D":
                    Pins[0].Name = "D";
                    break;
                case "JK":
                    Pins[0].Name = "J";
                    // En JK el segundo input también cambia
                    // Pero usamos CLK, así que solo cambiamos el primero
                    break;
                case "SR":
                    Pins[0].Name = "S";
                    // En SR el segundo sería R, pero usamos CLK
                    break;
                case "T":
                    Pins[0].Name = "T";
                    break;
            }

            // CLK siempre es CLK (pin 2)
            Pins[1].Name = "CLK";

            // Outputs siempre son Q y Q̄ (pins 3 y 4)
            Pins[2].Name = "Q";
            Pins[3].Name = "Q̄";
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 4) return;

            Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
            Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);
            Pins[2].Position = new Point(this.X + Size + 20, this.Y - Size / 4);
            Pins[3].Position = new Point(this.X + Size + 20, this.Y + Size / 4);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                // Rectángulo del flip-flop
                g.DrawRectangle(pen, this.X, this.Y - Size / 2, Size, Size);

                // Líneas de conexión (wires)
                g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y - Size / 4, this.X + Size + 20, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y + Size / 4, this.X + Size + 20, this.Y + Size / 4);

                // Tipo de FF en el centro
                using (Font typeFont = new Font("Arial", 10, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(FFType, typeFont);
                    g.DrawString(FFType, typeFont, Brushes.Black,
                        this.X + Size / 2 - textSize.Width / 2,
                        this.Y - textSize.Height / 2);
                }

                // Etiquetas de pines
                using (Font labelFont = new Font("Arial", 7))
                {
                    // Input izquierdo superior
                    g.DrawString(Pins[0].Name, labelFont, Brushes.Black, this.X + 3, this.Y - Size / 4 - 8);

                    // CLK izquierdo inferior (con triángulo)
                    g.DrawString("CLK", labelFont, Brushes.Black, this.X + 3, this.Y + Size / 4 - 8);

                    // Triángulo de clock
                    Point[] clockTriangle = {
                        new Point(this.X + 2, this.Y + Size/4 - 5),
                        new Point(this.X + 2, this.Y + Size/4 + 5),
                        new Point(this.X + 7, this.Y + Size/4)
                    };
                    g.DrawPolygon(pen, clockTriangle);

                    // Output Q derecho superior
                    g.DrawString("Q", labelFont, Brushes.Black, this.X + Size - 15, this.Y - Size / 4 - 8);

                    // Output Q̄ derecho inferior
                    g.DrawString("Q̄", labelFont, Brushes.Black, this.X + Size - 15, this.Y + Size / 4 - 8);
                }

                // Nombre del componente
                using (Font nameFont = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, nameFont, Brushes.Black, this.X + Size / 2 - 10, this.Y - Size / 2 - 15);
                }

                // Dibujar pines
                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 20, Size + 60, Size + 40);
        public override SchematicComponent Clone() => new FlipFlopComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, FFType = this.FFType };
    }

    /*/// <summary>
    /// Flip-Flop (D, JK, SR, T)
    /// </summary>
    [Serializable]
    public class FlipFlopComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Flip-Flop type")]
        public string FFType { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public FlipFlopComponent()
        {
            this.Type = ComponentType.FlipFlop;
            this.Name = "FF1";
            FFType = "D";
            Size = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "D", PinType.Input, Point.Empty),
                new ComponentPin(2, "CLK", PinType.Input, Point.Empty),
                new ComponentPin(3, "Q", PinType.Output, Point.Empty),
                new ComponentPin(4, "Q̄", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 4) return;
            Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
            Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);
            Pins[2].Position = new Point(this.X + Size + 20, this.Y - Size / 4);
            Pins[3].Position = new Point(this.X + Size + 20, this.Y + Size / 4);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Rectangle box = new Rectangle(this.X, this.Y - Size / 2, Size, Size);
                g.DrawRectangle(pen, box);

                g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y - Size / 4, this.X + Size + 20, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y + Size / 4, this.X + Size + 20, this.Y + Size / 4);

                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                {
                    g.DrawString(FFType, font, Brushes.Black, this.X + Size / 2 - 8, this.Y - Size / 4 - 8);
                    g.DrawString("CLK", new Font("Arial", 6), Brushes.Black, this.X + 5, this.Y + Size / 4 - 5);
                    g.DrawString("Q", font, Brushes.Black, this.X + Size - 15, this.Y - Size / 4 - 8);
                    g.DrawString("Q̄", font, Brushes.Black, this.X + Size - 15, this.Y + Size / 4 - 8);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 10, Size + 60, Size + 20);
        public override SchematicComponent Clone() => new FlipFlopComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, FFType = this.FFType };
    }*/

    /// <summary>
    /// Inverter (NOT gate)
    /// </summary>
    [Serializable]
    public class InverterComponent : SchematicComponent
    {
        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public InverterComponent()
        {
            this.Type = ComponentType.Inverter;
            this.Name = "U1";
            Size = 40;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "IN", PinType.Input, Point.Empty),
                new ComponentPin(2, "OUT", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 2) return;
            Pins[0].Position = new Point(this.X - 20, this.Y);
            Pins[1].Position = new Point(this.X + Size + 30, this.Y);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] triangle = {
                    new Point(this.X, this.Y - Size/2),
                    new Point(this.X, this.Y + Size/2),
                    new Point(this.X + Size, this.Y)
                };
                g.DrawPolygon(pen, triangle);
                g.DrawEllipse(pen, this.X + Size, this.Y - 5, 10, 10);

                g.DrawLine(wirePen, this.X - 20, this.Y, this.X, this.Y);
                g.DrawLine(wirePen, this.X + Size + 10, this.Y, this.X + Size + 30, this.Y);

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 10, Size + 60, Size + 20);
        public override SchematicComponent Clone() => new InverterComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, Size = this.Size };
    }

    /// <summary>
    /// 7-Segment LED Display
    /// </summary>
    [Serializable]
    public class LED7SegmentComponent : SchematicComponent
    {
        private string _commonType;

        [Category("Electrical")]
        [Description("Common type (Common Cathode or Common Anode)")]
        [TypeConverter(typeof(SevenSegmentCommonTypeConverter))]
        public string CommonType
        {
            get => _commonType;
            set
            {
                if (_commonType != value)
                {
                    _commonType = value;
                }
            }
        }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public LED7SegmentComponent()
        {
            this.Type = ComponentType.LED7Segment;
            this.Name = "DSP1";
            _commonType = "Common Cathode";
            Size = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "a", PinType.Input, Point.Empty),
                new ComponentPin(2, "b", PinType.Input, Point.Empty),
                new ComponentPin(3, "c", PinType.Input, Point.Empty),
                new ComponentPin(4, "d", PinType.Input, Point.Empty),
                new ComponentPin(5, "e", PinType.Input, Point.Empty),
                new ComponentPin(6, "f", PinType.Input, Point.Empty),
                new ComponentPin(7, "g", PinType.Input, Point.Empty),
                new ComponentPin(8, "dp", PinType.Input, Point.Empty),
                new ComponentPin(9, "COM", CommonType == "Common Cathode" ? PinType.Ground : PinType.Power, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 9) return;
            for (int i = 0; i < 4; i++)
                Pins[i].Position = new Point(this.X - 20, this.Y - Size / 2 + i * 15);
            for (int i = 4; i < 8; i++)
                Pins[i].Position = new Point(this.X + Size + 20, this.Y - Size / 2 + (i - 4) * 15);
            Pins[8].Position = new Point(this.X + Size / 2, this.Y + Size / 2 + 20);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            using (Pen segmentPen = new Pen(Color.Red, 3))
            {
                Rectangle display = new Rectangle(this.X, this.Y - Size / 2, Size, Size);
                g.FillRectangle(Brushes.Black, display);
                g.DrawRectangle(pen, display);

                int w = Size - 20;
                int cx = this.X + Size / 2;
                int cy = this.Y;

                g.DrawLine(segmentPen, cx - w / 2, cy - Size / 3, cx + w / 2, cy - Size / 3);
                g.DrawLine(segmentPen, cx + w / 2, cy - Size / 3, cx + w / 2, cy);
                g.DrawLine(segmentPen, cx + w / 2, cy, cx + w / 2, cy + Size / 3);
                g.DrawLine(segmentPen, cx - w / 2, cy + Size / 3, cx + w / 2, cy + Size / 3);
                g.DrawLine(segmentPen, cx - w / 2, cy, cx - w / 2, cy + Size / 3);
                g.DrawLine(segmentPen, cx - w / 2, cy - Size / 3, cx - w / 2, cy);
                g.DrawLine(segmentPen, cx - w / 2, cy, cx + w / 2, cy);

                for (int i = 0; i < 4; i++)
                    g.DrawLine(wirePen, this.X - 20, this.Y - Size / 2 + i * 15, this.X, this.Y - Size / 2 + i * 15);
                for (int i = 4; i < 8; i++)
                    g.DrawLine(wirePen, this.X + Size, this.Y - Size / 2 + (i - 4) * 15, this.X + Size + 20, this.Y - Size / 2 + (i - 4) * 15);
                g.DrawLine(wirePen, this.X + Size / 2, this.Y + Size / 2, this.X + Size / 2, this.Y + Size / 2 + 20);

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 10, Size + 60, Size + 40);
        public override SchematicComponent Clone() => new LED7SegmentComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, CommonType = this.CommonType };
    }

    /// <summary>
    /// Optocoupler
    /// </summary>
    [Serializable]
    public class OptocouplerComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Part number")]
        public string PartNumber { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public OptocouplerComponent()
        {
            this.Type = ComponentType.Optocoupler;
            this.Name = "U1";
            PartNumber = "4N35";
            Size = 60;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Input, Point.Empty),
                new ComponentPin(3, "E", PinType.Output, Point.Empty),
                new ComponentPin(4, "C", PinType.Output, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 4) return;
            Pins[0].Position = new Point(this.X - 20, this.Y - Size / 4);
            Pins[1].Position = new Point(this.X - 20, this.Y + Size / 4);
            Pins[2].Position = new Point(this.X + Size + 20, this.Y + Size / 4);
            Pins[3].Position = new Point(this.X + Size + 20, this.Y - Size / 4);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            using (Pen dashedPen = new Pen(Color.Gray, 1))
            {
                dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                Rectangle box = new Rectangle(this.X, this.Y - Size / 2, Size, Size);
                g.DrawRectangle(pen, box);

                Point[] ledTriangle = {
                    new Point(this.X + 10, this.Y - 5),
                    new Point(this.X + 10, this.Y + 5),
                    new Point(this.X + 20, this.Y)
                };
                g.DrawPolygon(pen, ledTriangle);
                g.DrawLine(pen, this.X + 20, this.Y - 5, this.X + 20, this.Y + 5);

                g.DrawLine(pen, this.X + Size - 25, this.Y - Size / 4, this.X + Size - 15, this.Y);
                g.DrawLine(pen, this.X + Size - 15, this.Y, this.X + Size - 25, this.Y + Size / 4);
                g.DrawLine(pen, this.X + Size - 25, this.Y - Size / 4, this.X + Size - 25, this.Y + Size / 4);

                for (int i = 0; i < 3; i++)
                    g.DrawLine(dashedPen, this.X + 22 + i * 5, this.Y - 10, this.X + 28 + i * 5, this.Y + 10);

                g.DrawLine(wirePen, this.X - 20, this.Y - Size / 4, this.X, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X - 20, this.Y + Size / 4, this.X, this.Y + Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y - Size / 4, this.X + Size + 20, this.Y - Size / 4);
                g.DrawLine(wirePen, this.X + Size, this.Y + Size / 4, this.X + Size + 20, this.Y + Size / 4);

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - 30, this.Y - Size / 2 - 10, Size + 60, Size + 20);
        public override SchematicComponent Clone() => new OptocouplerComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, PartNumber = this.PartNumber };
    }

    /// <summary>
    /// TRIAC
    /// </summary>
    [Serializable]
    public class TriacComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Current rating")]
        public string CurrentRating { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public TriacComponent()
        {
            this.Type = ComponentType.Triac;
            this.Name = "TRI1";
            CurrentRating = "8A";
            Size = 50;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "MT1", PinType.Passive, Point.Empty),
                new ComponentPin(2, "MT2", PinType.Passive, Point.Empty),
                new ComponentPin(3, "G", PinType.Input, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 3) return;
            Pins[0].Position = new Point(this.X, this.Y - Size - 20);
            Pins[1].Position = new Point(this.X, this.Y + Size + 20);
            Pins[2].Position = new Point(this.X - Size - 20, this.Y);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] triangle1 = {
                    new Point(this.X - 15, this.Y - 5),
                    new Point(this.X + 15, this.Y - 5),
                    new Point(this.X, this.Y - Size)
                };
                g.DrawPolygon(pen, triangle1);

                Point[] triangle2 = {
                    new Point(this.X - 15, this.Y + 5),
                    new Point(this.X + 15, this.Y + 5),
                    new Point(this.X, this.Y + Size)
                };
                g.DrawPolygon(pen, triangle2);

                g.DrawLine(pen, this.X - 15, this.Y - 5, this.X - 15, this.Y + 5);
                g.DrawLine(pen, this.X + 15, this.Y - 5, this.X + 15, this.Y + 5);

                g.DrawLine(wirePen, this.X, this.Y - Size, this.X, this.Y - Size - 20);
                g.DrawLine(wirePen, this.X, this.Y + Size, this.X, this.Y + Size + 20);
                g.DrawLine(wirePen, this.X - Size - 20, this.Y, this.X - 15, this.Y);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, font, Brushes.Black, this.X + 20, this.Y - 8);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - Size - 30, this.Y - Size - 30, Size * 2 + 40, Size * 2 + 60);
        public override SchematicComponent Clone() => new TriacComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, CurrentRating = this.CurrentRating };
    }

    /// <summary>
    /// Thyristor (SCR)
    /// </summary>
    [Serializable]
    public class ThyristorComponent : SchematicComponent
    {
        [Category("Electrical")]
        [Description("Current rating")]
        public string CurrentRating { get; set; }

        [Category("Geometry")]
        [Description("Component size")]
        public int Size { get; set; }

        [Category("Display")]
        [Description("Show pin numbers")]
        public bool ShowPinNumbers { get; set; }

        [Browsable(false)]
        public List<ComponentPin> Pins { get; set; }

        public ThyristorComponent()
        {
            this.Type = ComponentType.Thyristor;
            this.Name = "SCR1";
            this.Orientation = ComponentOrientation.Vertical;
            CurrentRating = "10A";
            Size = 50;
            ShowPinNumbers = true;
            Pins = new List<ComponentPin>
            {
                new ComponentPin(1, "A", PinType.Input, Point.Empty),
                new ComponentPin(2, "K", PinType.Output, Point.Empty),
                new ComponentPin(3, "G", PinType.Input, Point.Empty)
            };
            UpdateBounds();
        }

        protected override void UpdatePinPositions()
        {
            if (Pins == null || Pins.Count < 3) return;
            Pins[0].Position = new Point(this.X, this.Y - Size - 20);
            Pins[1].Position = new Point(this.X, this.Y + Size + 20);
            Pins[2].Position = new Point(this.X - Size - 20, this.Y + Size / 2);
        }

        public override void DrawInternal(Graphics g, bool isSelected, bool isHovered)
        {
            UpdatePinPositions();
            using (Pen pen = new Pen(isSelected ? Color.Blue : Color.Black, 2))
            using (Pen wirePen = new Pen(Color.Black, 2))
            {
                Point[] triangle = {
                    new Point(this.X - 15, this.Y),
                    new Point(this.X + 15, this.Y),
                    new Point(this.X, this.Y + Size)
                };
                g.DrawPolygon(pen, triangle);
                g.DrawLine(pen, this.X - 15, this.Y, this.X + 15, this.Y);

                g.DrawLine(wirePen, this.X, this.Y - Size - 20, this.X, this.Y);
                g.DrawLine(wirePen, this.X, this.Y + Size, this.X, this.Y + Size + 20);
                g.DrawLine(wirePen, this.X - Size - 20, this.Y + Size / 2, this.X - 10, this.Y + Size / 2);

                using (Font font = new Font("Arial", 7))
                {
                    g.DrawString(this.Name, font, Brushes.Black, this.X + 20, this.Y + Size / 2 - 8);
                }

                if (Pins != null)
                {
                    foreach (var pin in Pins)
                        pin.Draw(g, ShowPinNumbers, false);
                }
            }
        }

        public override bool HitTest(Point point) => this.Bounds.Contains(point);
        protected override void UpdateBounds() => this.Bounds = new Rectangle(this.X - Size - 30, this.Y - Size - 30, Size * 2 + 40, Size * 2 + 60);
        public override SchematicComponent Clone() => new ThyristorComponent { Name = this.Name + "_copy", X = this.X + 50, Y = this.Y + 50, CurrentRating = this.CurrentRating, Orientation = this.Orientation };
    }
}

