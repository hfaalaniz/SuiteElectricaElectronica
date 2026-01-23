using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BuckConverterCalculator.Models
{
    public enum SymbolType
    {
        // Existentes
        Breaker,
        Contactor,
        Motor,
        Transformer,
        Fuse,
        Switch,
        Load,
        Outlet,
        LightBulb,
        Lamp,
        DifferentialBreaker,
        ThermalBreaker,
        DistributionPanel,
        Meter,
        Fan,
        WaterHeater,
        AirConditioner,
        Doorbell,
        GroundConnection,
        // Interruptores
        BipolarSwitch,
        PullSwitch,
        DoubleSwitch,
        Commutator,
        CrossCommutator,
        PushButton,
        Dimmer,
        BlindSwitch,
        // Tomas
        Plug16A,
        Plug25A,
        PlugTriphasic,
        // Iluminación
        FluorescentLamp,
        EmergencyLight,
        // Señalización
        Bell,
        Siren,
        // Protecciones
        CGP,
        ICP,
        DifferentialBipolar,
        DifferentialTetrapolar,
        StairTimer,
        Teleruptor,
        // Sensores
        Thermostat,
        PIRDetector,
        FireDetector,
        GasDetector,
        FloodDetector,
        // Control
        IREmitter,
        IRReceiver,
        CardRelay,
        WaterValve,
        GasValve,
        TimeSwitch,
        KeySwitch,
        SurgeProtector,
        // Electrodomésticos
        WashingMachine,
        Dishwasher,
        ElectricHeater,
        Refrigerator,
        Freezer,
        ElectricStove
    }

    [Serializable]
    public class ElectricalSymbol
    {
        [Category("General")]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [Category("General")]
        [DisplayName("Tipo")]
        public SymbolType Type { get; set; }

        [Category("Posición")]
        [DisplayName("X")]
        public int X { get; set; }

        [Category("Posición")]
        [DisplayName("Y")]
        public int Y { get; set; }

        [Category("Dimensiones")]
        [DisplayName("Ancho")]
        public int Width { get; set; } = 60;

        [Category("Dimensiones")]
        [DisplayName("Alto")]
        public int Height { get; set; } = 60;

        [Category("Eléctrico")]
        [DisplayName("Voltaje (V)")]
        public double Voltage { get; set; }

        [Category("Eléctrico")]
        [DisplayName("Corriente (A)")]
        public double Current { get; set; }

        [Category("Eléctrico")]
        [DisplayName("Potencia (kW)")]
        public double Power { get; set; }

        [Category("Eléctrico")]
        [DisplayName("Polos")]
        public int Poles { get; set; } = 1;

        [Browsable(false)]
        public List<ConnectionPoint> ConnectionPoints { get; set; } = new List<ConnectionPoint>();

        [Browsable(false)]
        public bool IsSelected { get; set; }

        public ElectricalSymbol()
        {
            Name = "Nuevo Símbolo";
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public void InitializeConnectionPoints()
        {
            ConnectionPoints.Clear();
            switch (Type)
            {
                case SymbolType.Breaker:
                case SymbolType.Switch:
                case SymbolType.Fuse:
                case SymbolType.ThermalBreaker:
                case SymbolType.DifferentialBreaker:
                case SymbolType.BipolarSwitch:
                case SymbolType.PullSwitch:
                case SymbolType.Commutator:
                case SymbolType.CrossCommutator:
                case SymbolType.PushButton:
                case SymbolType.Dimmer:
                case SymbolType.ICP:
                case SymbolType.DifferentialBipolar:
                case SymbolType.StairTimer:
                case SymbolType.Teleruptor:
                case SymbolType.KeySwitch:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = Height, Parent = this });
                    break;
                case SymbolType.DoubleSwitch:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 3, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width * 2 / 3, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 3, Y = Height, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width * 2 / 3, Y = Height, Parent = this });
                    break;
                case SymbolType.DifferentialTetrapolar:
                    for (int i = 0; i < 4; i++)
                    {
                        int xPos = Width / 5 * (i + 1);
                        ConnectionPoints.Add(new ConnectionPoint { X = xPos, Y = 0, Parent = this });
                        ConnectionPoints.Add(new ConnectionPoint { X = xPos, Y = Height, Parent = this });
                    }
                    break;
                case SymbolType.BlindSwitch:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 3, Y = Height, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width * 2 / 3, Y = Height, Parent = this });
                    break;
                case SymbolType.Motor:
                case SymbolType.Load:
                case SymbolType.LightBulb:
                case SymbolType.Lamp:
                case SymbolType.Fan:
                case SymbolType.WaterHeater:
                case SymbolType.AirConditioner:
                case SymbolType.Doorbell:
                case SymbolType.FluorescentLamp:
                case SymbolType.EmergencyLight:
                case SymbolType.Bell:
                case SymbolType.Siren:
                case SymbolType.WashingMachine:
                case SymbolType.Dishwasher:
                case SymbolType.ElectricHeater:
                case SymbolType.Refrigerator:
                case SymbolType.Freezer:
                case SymbolType.ElectricStove:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    break;
                case SymbolType.Outlet:
                case SymbolType.Plug16A:
                case SymbolType.Plug25A:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = 0, Y = Height / 2, Parent = this });
                    break;
                case SymbolType.PlugTriphasic:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = 0, Y = Height / 3, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = 0, Y = Height * 2 / 3, Parent = this });
                    break;
                case SymbolType.GroundConnection:
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = 0, Parent = this });
                    break;
                case SymbolType.Contactor:
                    for (int i = 0; i < Poles; i++)
                    {
                        int xPos = Width / (Poles + 1) * (i + 1);
                        ConnectionPoints.Add(new ConnectionPoint { X = xPos, Y = 0, Parent = this });
                        ConnectionPoints.Add(new ConnectionPoint { X = xPos, Y = Height, Parent = this });
                    }
                    break;
                case SymbolType.Transformer:
                    ConnectionPoints.Add(new ConnectionPoint { X = 15, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width - 15, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = 15, Y = Height, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width - 15, Y = Height, Parent = this });
                    break;
                case SymbolType.DistributionPanel:
                case SymbolType.Meter:
                case SymbolType.CGP:
                    ConnectionPoints.Add(new ConnectionPoint { X = 15, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width - 15, Y = 0, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = 15, Y = Height, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width / 2, Y = Height, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width - 15, Y = Height, Parent = this });
                    break;
                case SymbolType.Thermostat:
                case SymbolType.PIRDetector:
                case SymbolType.FireDetector:
                case SymbolType.GasDetector:
                case SymbolType.FloodDetector:
                case SymbolType.IREmitter:
                case SymbolType.IRReceiver:
                case SymbolType.CardRelay:
                case SymbolType.WaterValve:
                case SymbolType.GasValve:
                case SymbolType.TimeSwitch:
                case SymbolType.SurgeProtector:
                    ConnectionPoints.Add(new ConnectionPoint { X = 0, Y = Height / 2, Parent = this });
                    ConnectionPoints.Add(new ConnectionPoint { X = Width, Y = Height / 2, Parent = this });
                    break;
            }
        }
    }

    [Serializable]
    public class ConnectionPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ElectricalSymbol Parent { get; set; }

        public Point GetAbsolutePosition()
        {
            return new Point(Parent.X + X, Parent.Y + Y);
        }
    }

    [Serializable]
    public class Wire
    {
        public ConnectionPoint StartPoint { get; set; }
        public ConnectionPoint EndPoint { get; set; }
        public Color Color { get; set; } = Color.Black;
        public float Thickness { get; set; } = 2f;

        public void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Thickness))
            {
                Point start = StartPoint.GetAbsolutePosition();
                Point end = EndPoint.GetAbsolutePosition();
                g.DrawLine(pen, start, end);
            }
        }
    }
}