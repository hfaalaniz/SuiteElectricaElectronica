using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BuckConverterCalculator.PCB
{
    /// <summary>
    /// Router automático para PCB traces usando algoritmo Lee (wave propagation)
    /// </summary>
    public class AutoRouter
    {
        private PCBLayout layout;
        private int[,] grid;
        private int gridWidth;
        private int gridHeight;
        private double gridResolution = 0.5; // mm per grid cell

        public AutoRouterSettings Settings { get; set; } = new AutoRouterSettings();

        public AutoRouter(PCBLayout pcbLayout)
        {
            this.layout = pcbLayout;
            InitializeGrid();
        }

        /// <summary>
        /// Inicializa la grilla de routing
        /// </summary>
        private void InitializeGrid()
        {
            gridWidth = (int)(layout.Width / gridResolution);
            gridHeight = (int)(layout.Height / gridResolution);
            grid = new int[gridWidth, gridHeight];

            // Marcar posiciones de componentes como obstáculos
            MarkComponentsAsObstacles();
        }

        /// <summary>
        /// Marca los componentes como obstáculos en la grilla
        /// </summary>
        private void MarkComponentsAsObstacles()
        {
            const int OBSTACLE = -1;

            foreach (var component in layout.Components)
            {
                int x = (int)(component.X / gridResolution);
                int y = (int)(component.Y / gridResolution);
                int size = 5; // 5 cells around component

                for (int dx = -size; dx <= size; dx++)
                {
                    for (int dy = -size; dy <= size; dy++)
                    {
                        int gx = x + dx;
                        int gy = y + dy;

                        if (gx >= 0 && gx < gridWidth && gy >= 0 && gy < gridHeight)
                        {
                            grid[gx, gy] = OBSTACLE;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rutea todas las conexiones de una red
        /// </summary>
        public List<PCBTrace> RouteNet(Net net)
        {
            var traces = new List<PCBTrace>();

            if (net.Nodes.Count < 2)
                return traces;

            // Usar algoritmo de árbol de expansión mínima (MST) para decidir orden de ruteo
            var connections = CalculateMinimumSpanningTree(net.Nodes);

            foreach (var connection in connections)
            {
                var path = RouteConnection(connection.Item1, connection.Item2);

                if (path != null && path.Count > 0)
                {
                    var trace = new PCBTrace
                    {
                        NetName = net.Name,
                        Layer = "Top",
                        Width = Settings.DefaultTraceWidth,
                        Points = path.Select(p => new Point(
                            (int)(p.X * gridResolution),
                            (int)(p.Y * gridResolution)
                        )).ToList()
                    };

                    traces.Add(trace);
                }
            }

            return traces;
        }

        /// <summary>
        /// Rutea una conexión entre dos puntos usando algoritmo Lee
        /// </summary>
        private List<Point> RouteConnection(NetNode start, NetNode end)
        {
            // Convertir a coordenadas de grilla
            int startX = (int)(start.Position.X / gridResolution);
            int startY = (int)(start.Position.Y / gridResolution);
            int endX = (int)(end.Position.X / gridResolution);
            int endY = (int)(end.Position.Y / gridResolution);

            // Algoritmo Lee (wave propagation)
            var workGrid = (int[,])grid.Clone();
            var queue = new Queue<Point>();

            // Marcar punto inicial
            workGrid[startX, startY] = 1;
            queue.Enqueue(new Point(startX, startY));

            bool found = false;

            // Propagar onda
            while (queue.Count > 0 && !found)
            {
                var current = queue.Dequeue();
                int currentDistance = workGrid[current.X, current.Y];

                // Verificar 4 direcciones (Manhattan routing)
                var directions = new[]
                {
                    new Point(0, 1),   // Norte
                    new Point(1, 0),   // Este
                    new Point(0, -1),  // Sur
                    new Point(-1, 0)   // Oeste
                };

                foreach (var dir in directions)
                {
                    int newX = current.X + dir.X;
                    int newY = current.Y + dir.Y;

                    // Verificar límites
                    if (newX < 0 || newX >= gridWidth || newY < 0 || newY >= gridHeight)
                        continue;

                    // Verificar si llegamos al destino
                    if (newX == endX && newY == endY)
                    {
                        workGrid[newX, newY] = currentDistance + 1;
                        found = true;
                        break;
                    }

                    // Verificar si la celda está libre
                    if (workGrid[newX, newY] == 0)
                    {
                        workGrid[newX, newY] = currentDistance + 1;
                        queue.Enqueue(new Point(newX, newY));
                    }
                }
            }

            if (!found)
                return null; // No se encontró ruta

            // Backtrack para encontrar el camino
            return BacktrackPath(workGrid, startX, startY, endX, endY);
        }

        /// <summary>
        /// Hace backtrack desde el destino al origen para encontrar el camino
        /// </summary>
        private List<Point> BacktrackPath(int[,] workGrid, int startX, int startY, int endX, int endY)
        {
            var path = new List<Point>();
            int currentX = endX;
            int currentY = endY;

            path.Add(new Point(currentX, currentY));

            while (currentX != startX || currentY != startY)
            {
                int currentDistance = workGrid[currentX, currentY];
                bool moved = false;

                // Buscar vecino con distancia menor
                var directions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(0, -1),
                    new Point(-1, 0)
                };

                foreach (var dir in directions)
                {
                    int newX = currentX + dir.X;
                    int newY = currentY + dir.Y;

                    if (newX < 0 || newX >= gridWidth || newY < 0 || newY >= gridHeight)
                        continue;

                    if (workGrid[newX, newY] == currentDistance - 1)
                    {
                        currentX = newX;
                        currentY = newY;
                        path.Add(new Point(currentX, currentY));
                        moved = true;
                        break;
                    }
                }

                if (!moved)
                    break; // Error en backtrack
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Calcula árbol de expansión mínima (MST) usando algoritmo de Prim
        /// </summary>
        private List<Tuple<NetNode, NetNode>> CalculateMinimumSpanningTree(List<NetNode> nodes)
        {
            var connections = new List<Tuple<NetNode, NetNode>>();

            if (nodes.Count < 2)
                return connections;

            var visited = new HashSet<NetNode> { nodes[0] };
            var unvisited = new HashSet<NetNode>(nodes.Skip(1));

            while (unvisited.Count > 0)
            {
                double minDistance = double.MaxValue;
                NetNode nearestVisited = null;
                NetNode nearestUnvisited = null;

                // Encontrar la arista más corta entre visitados y no visitados
                foreach (var v in visited)
                {
                    foreach (var u in unvisited)
                    {
                        double distance = Distance(v.Position, u.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestVisited = v;
                            nearestUnvisited = u;
                        }
                    }
                }

                if (nearestUnvisited != null)
                {
                    connections.Add(Tuple.Create(nearestVisited, nearestUnvisited));
                    visited.Add(nearestUnvisited);
                    unvisited.Remove(nearestUnvisited);
                }
                else
                {
                    break;
                }
            }

            return connections;
        }

        /// <summary>
        /// Calcula distancia euclidiana entre dos puntos
        /// </summary>
        private double Distance(Point p1, Point p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Optimiza las rutas usando smoothing
        /// </summary>
        public void OptimizeRoutes(List<PCBTrace> traces)
        {
            foreach (var trace in traces)
            {
                if (trace.Points == null || trace.Points.Count < 3)
                    continue;

                // Eliminar puntos colineales
                var optimized = new List<Point> { trace.Points[0] };

                for (int i = 1; i < trace.Points.Count - 1; i++)
                {
                    var prev = trace.Points[i - 1];
                    var curr = trace.Points[i];
                    var next = trace.Points[i + 1];

                    // Si no son colineales, mantener el punto
                    if (!AreCollinear(prev, curr, next))
                    {
                        optimized.Add(curr);
                    }
                }

                optimized.Add(trace.Points[trace.Points.Count - 1]);
                trace.Points = optimized;
            }
        }

        /// <summary>
        /// Verifica si tres puntos son colineales
        /// </summary>
        private bool AreCollinear(Point p1, Point p2, Point p3)
        {
            // Usar producto cruz para verificar colinealidad
            int cross = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
            return cross == 0;
        }
    }

    /// <summary>
    /// Configuración del auto-router
    /// </summary>
    public class AutoRouterSettings
    {
        /// <summary>
        /// Ancho de traza por defecto (mm)
        /// </summary>
        public double DefaultTraceWidth { get; set; } = 0.25;

        /// <summary>
        /// Clearance mínimo entre trazas (mm)
        /// </summary>
        public double MinimumClearance { get; set; } = 0.2;

        /// <summary>
        /// Preferir rutas en ángulos de 45 grados
        /// </summary>
        public bool Prefer45Degrees { get; set; } = false;

        /// <summary>
        /// Número máximo de iteraciones para encontrar ruta
        /// </summary>
        public int MaxIterations { get; set; } = 10000;

        /// <summary>
        /// Usar routing en ambas capas (si está disponible)
        /// </summary>
        public bool UseBothLayers { get; set; } = true;
    }
}