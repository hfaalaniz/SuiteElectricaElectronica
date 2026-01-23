using BuckConverterCalculator.SchematicEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BuckConverterCalculator.PCB
{
    public class PCBLayoutGenerator
    {
        private SchematicDocument schematic;

        public PCBLayoutGenerator(SchematicDocument doc)
        {
            this.schematic = doc;
        }

        public PCBLayout GenerateLayout(PCBSettings settings)
        {
            var layout = new PCBLayout
            {
                Width = settings.BoardWidth,
                Height = settings.BoardHeight,
                Layers = settings.Layers,
                Components = new List<PCBComponent>(),
                Traces = new List<PCBTrace>(),
                Nets = new List<PCBNet>()
            };

            // 1. Extraer netlist del esquemático
            var netlist = ExtractNetlist();
            layout.Nets = netlist;

            // 2. Colocar componentes automáticamente
            PlaceComponents(layout, settings);

            // 3. Rutear trazas
            RouteTraces(layout, settings);

            return layout;
        }

        private List<PCBNet> ExtractNetlist()
        {
            var nets = new Dictionary<string, PCBNet>();
            var nodeConnections = new Dictionary<Point, List<(SchematicComponent, ComponentPin)>>();

            // Encontrar todas las conexiones a través de wires
            foreach (var component in schematic.Components)
            {
                if (component is WireComponent) continue;

                var pins = GetComponentPins(component);
                foreach (var pin in pins)
                {
                    // Buscar wires conectados a este pin
                    var connectedWires = FindConnectedWires(pin.Position);

                    foreach (var wire in connectedWires)
                    {
                        string netName = $"NET{nets.Count + 1}";

                        if (!nets.ContainsKey(netName))
                        {
                            nets[netName] = new PCBNet
                            {
                                Name = netName,
                                Connections = new List<NetConnection>()
                            };
                        }

                        nets[netName].Connections.Add(new NetConnection
                        {
                            ComponentName = component.Name,
                            PinName = pin.Name,
                            Position = pin.Position
                        });
                    }
                }
            }

            return nets.Values.ToList();
        }

        private void PlaceComponents(PCBLayout layout, PCBSettings settings)
        {
            // Estrategia simple: colocar componentes en grid
            int gridX = 10; // mm
            int gridY = 10; // mm
            int currentX = gridX;
            int currentY = gridY;
            int componentsPerRow = (int)(layout.Width / gridX) - 2;
            int count = 0;

            foreach (var component in schematic.Components)
            {
                if (component is WireComponent || component is LabelComponent) continue;

                var pcbComp = new PCBComponent
                {
                    Name = component.Name,
                    Type = component.Type.ToString(),
                    X = currentX,
                    Y = currentY,
                    Rotation = 0,
                    Layer = "Top",
                    Footprint = GetFootprint(component)
                };

                layout.Components.Add(pcbComp);

                // Avanzar posición
                count++;
                if (count % componentsPerRow == 0)
                {
                    currentX = gridX;
                    currentY += gridY;
                }
                else
                {
                    currentX += gridX;
                }
            }
        }

        private void RouteTraces(PCBLayout layout, PCBSettings settings)
        {
            // Router simple: líneas rectas entre conexiones
            // Un router profesional usaría A* o Lee algorithm

            foreach (var net in layout.Nets)
            {
                var connections = net.Connections;

                // Conectar cada par de puntos en la red
                for (int i = 0; i < connections.Count - 1; i++)
                {
                    var start = connections[i];
                    var end = connections[i + 1];

                    var startComp = layout.Components.FirstOrDefault(c => c.Name == start.ComponentName);
                    var endComp = layout.Components.FirstOrDefault(c => c.Name == end.ComponentName);

                    if (startComp != null && endComp != null)
                    {
                        var trace = new PCBTrace
                        {
                            NetName = net.Name,
                            Layer = "Top",
                            Width = settings.DefaultTraceWidth,
                            StartX = startComp.X,
                            StartY = startComp.Y,
                            EndX = endComp.X,
                            EndY = endComp.Y,
                            Points = CreateRoutePoints(startComp, endComp)
                        };

                        layout.Traces.Add(trace);
                    }
                }
            }
        }

        private List<Point> CreateRoutePoints(PCBComponent start, PCBComponent end)
        {
            var points = new List<Point>();

            // Router Manhattan (solo 90 grados)
            points.Add(new Point((int)start.X, (int)start.Y));

            // Punto intermedio
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                // Horizontal primero
                points.Add(new Point((int)end.X, (int)start.Y));
            }
            else
            {
                // Vertical primero
                points.Add(new Point((int)start.X, (int)end.Y));
            }

            points.Add(new Point((int)end.X, (int)end.Y));

            return points;
        }

        private string GetFootprint(SchematicComponent component)
        {
            // Mapeo de componentes a footprints estándar
            if (component is ResistorComponent || component is CapacitorComponent)
                return "0805";
            else if (component is InductorComponent)
                return "SMD_5x5";
            else if (component is DiodeComponent)
                return "SOD-123";
            else if (component is MosfetComponent)
                return "SOT-23";
            else if (component is ICComponent)
                return "SOIC-8";

            return "UNKNOWN";
        }

        private List<ComponentPin> GetComponentPins(SchematicComponent component)
        {
            // Extraer pines del componente usando reflection o interfaces
            var pinsProperty = component.GetType().GetProperty("Pins");
            if (pinsProperty != null)
            {
                var pins = pinsProperty.GetValue(component) as List<ComponentPin>;
                return pins ?? new List<ComponentPin>();
            }
            return new List<ComponentPin>();
        }

        private List<WireComponent> FindConnectedWires(Point pinPosition)
        {
            var wires = new List<WireComponent>();
            const int tolerance = 5;

            foreach (var component in schematic.Components)
            {
                if (component is WireComponent wire)
                {
                    // Verificar si el wire toca este pin
                    if (IsPointNearLine(pinPosition,
                        new Point(wire.X1, wire.Y1),
                        new Point(wire.X2, wire.Y2),
                        tolerance))
                    {
                        wires.Add(wire);
                    }
                }
            }

            return wires;
        }

        private bool IsPointNearLine(Point point, Point lineStart, Point lineEnd, int tolerance)
        {
            // Distancia punto-línea
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
                return Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2)) <= tolerance;

            double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            double projX = lineStart.X + t * dx;
            double projY = lineStart.Y + t * dy;

            double distance = Math.Sqrt(Math.Pow(point.X - projX, 2) + Math.Pow(point.Y - projY, 2));

            return distance <= tolerance;
        }

        public void ExportToKiCad(PCBLayout layout, string filename)
        {
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("(kicad_pcb (version 4) (host pcbnew)");
                writer.WriteLine($"  (general (thickness 1.6))");
                writer.WriteLine($"  (page A4)");
                writer.WriteLine($"  (layers");
                writer.WriteLine($"    (0 F.Cu signal)");
                writer.WriteLine($"    (31 B.Cu signal)");
                writer.WriteLine($"  )");

                // Componentes (modules)
                foreach (var comp in layout.Components)
                {
                    writer.WriteLine($"  (module {comp.Footprint} (layer F.Cu)");
                    writer.WriteLine($"    (at {comp.X} {comp.Y} {comp.Rotation})");
                    writer.WriteLine($"    (fp_text reference {comp.Name} (at 0 0) (layer F.SilkS) (effects (font (size 1 1) (thickness 0.15))))");
                    writer.WriteLine($"  )");
                }

                // Trazas (segments)
                foreach (var trace in layout.Traces)
                {
                    for (int i = 0; i < trace.Points.Count - 1; i++)
                    {
                        writer.WriteLine($"  (segment (start {trace.Points[i].X} {trace.Points[i].Y}) " +
                                       $"(end {trace.Points[i + 1].X} {trace.Points[i + 1].Y}) " +
                                       $"(width {trace.Width}) (layer {trace.Layer}) (net {trace.NetName}))");
                    }
                }

                writer.WriteLine(")");
            }
        }
    }

    public class PCBLayout
    {
        public double Width { get; set; } // mm
        public double Height { get; set; } // mm
        public int Layers { get; set; }
        public List<PCBComponent> Components { get; set; }
        public List<PCBTrace> Traces { get; set; }
        public List<PCBNet> Nets { get; set; }
    }

    public class PCBComponent
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double X { get; set; } // mm
        public double Y { get; set; } // mm
        public double Rotation { get; set; } // degrees
        public string Layer { get; set; }
        public string Footprint { get; set; }
    }

    public class PCBTrace
    {
        public string NetName { get; set; }
        public string Layer { get; set; }
        public double Width { get; set; } // mm
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public List<Point> Points { get; set; }
    }

    public class PCBNet
    {
        public string Name { get; set; }
        public List<NetConnection> Connections { get; set; }
    }

    public class NetConnection
    {
        public string ComponentName { get; set; }
        public string PinName { get; set; }
        public Point Position { get; set; }
    }

    public class PCBSettings
    {
        public double BoardWidth { get; set; } = 100; // mm
        public double BoardHeight { get; set; } = 80; // mm
        public int Layers { get; set; } = 2;
        public double DefaultTraceWidth { get; set; } = 0.25; // mm
        public double MinimumClearance { get; set; } = 0.2; // mm
        public bool AutoRoute { get; set; } = true;
    }
}