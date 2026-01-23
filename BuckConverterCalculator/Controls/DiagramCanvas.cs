using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BuckConverterCalculator.Models;
using BuckConverterCalculator.Rendering;

namespace BuckConverterCalculator.Controls
{
    public partial class DiagramCanvas : UserControl
    {
        private List<ElectricalSymbol> symbols = new List<ElectricalSymbol>();
        private List<Wire> wires = new List<Wire>();
        private ElectricalSymbol draggedSymbol = null;
        private Point dragOffset;
        private ElectricalSymbol selectedSymbol = null;
        private ConnectionPoint wireStartPoint = null;
        private Point currentMousePos;
        private bool isWiring = false;

        // Modo de cableado con mouse
        private bool mouseWiringMode = false;
        private List<Point> wirePoints = new List<Point>();

        public event EventHandler<ElectricalSymbol> SymbolSelected;
        public event EventHandler SymbolDeselected;

        public DiagramCanvas()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.AllowDrop = true;
            //this.KeyPreview = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Color.White);

            foreach (var wire in wires)
            {
                wire.Draw(e.Graphics);
            }

            // Dibujar cable en progreso (punto a punto)
            if (isWiring && wireStartPoint != null)
            {
                using (Pen pen = new Pen(Color.Gray, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    Point start = wireStartPoint.GetAbsolutePosition();
                    e.Graphics.DrawLine(pen, start, currentMousePos);
                }
            }

            // Dibujar cable con mouse (múltiples puntos)
            if (mouseWiringMode && wirePoints.Count > 0)
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    for (int i = 0; i < wirePoints.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(pen, wirePoints[i], wirePoints[i + 1]);
                    }

                    if (wirePoints.Count > 0)
                    {
                        e.Graphics.DrawLine(pen, wirePoints[wirePoints.Count - 1], currentMousePos);
                    }
                }
            }

            foreach (var symbol in symbols)
            {
                SymbolRenderer.DrawSymbol(e.Graphics, symbol);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SymbolType)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SymbolType)))
            {
                SymbolType type = (SymbolType)e.Data.GetData(typeof(SymbolType));
                Point pos = this.PointToClient(new Point(e.X, e.Y));

                var symbol = new ElectricalSymbol
                {
                    Type = type,
                    X = pos.X - 30,
                    Y = pos.Y - 30,
                    Name = $"{type}_{symbols.Count + 1}",
                    Voltage = 220,
                    Current = 10,
                    Power = 2.2
                };

                symbol.InitializeConnectionPoints();
                symbols.Add(symbol);
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                // Modo cableado con mouse (tecla Ctrl)
                if (Control.ModifierKeys == Keys.Control)
                {
                    if (!mouseWiringMode)
                    {
                        mouseWiringMode = true;
                        wirePoints.Clear();
                        wirePoints.Add(e.Location);
                    }
                    else
                    {
                        wirePoints.Add(e.Location);
                    }
                    this.Invalidate();
                    return;
                }

                // Cableado normal (punto de conexión a punto de conexión)
                var clickedConnection = GetConnectionPointAt(e.Location);

                if (clickedConnection != null)
                {
                    if (!isWiring)
                    {
                        wireStartPoint = clickedConnection;
                        isWiring = true;
                        currentMousePos = e.Location;
                    }
                    else
                    {
                        if (wireStartPoint != clickedConnection && wireStartPoint.Parent != clickedConnection.Parent)
                        {
                            wires.Add(new Wire
                            {
                                StartPoint = wireStartPoint,
                                EndPoint = clickedConnection
                            });
                        }
                        wireStartPoint = null;
                        isWiring = false;
                    }
                    this.Invalidate();
                    return;
                }

                var clickedSymbol = GetSymbolAt(e.Location);

                if (clickedSymbol != null)
                {
                    if (selectedSymbol != null)
                        selectedSymbol.IsSelected = false;

                    selectedSymbol = clickedSymbol;
                    selectedSymbol.IsSelected = true;
                    draggedSymbol = clickedSymbol;
                    dragOffset = new Point(e.X - clickedSymbol.X, e.Y - clickedSymbol.Y);

                    SymbolSelected?.Invoke(this, selectedSymbol);
                }
                else
                {
                    if (selectedSymbol != null)
                    {
                        selectedSymbol.IsSelected = false;
                        selectedSymbol = null;
                        SymbolDeselected?.Invoke(this, EventArgs.Empty);
                    }
                }

                this.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Cancelar cableado
                if (isWiring)
                {
                    wireStartPoint = null;
                    isWiring = false;
                    this.Invalidate();
                }

                // Finalizar cableado con mouse
                if (mouseWiringMode)
                {
                    if (wirePoints.Count >= 2)
                    {
                        CreateFreeWire(wirePoints);
                    }
                    mouseWiringMode = false;
                    wirePoints.Clear();
                    this.Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            currentMousePos = e.Location;

            if (isWiring || mouseWiringMode)
            {
                this.Invalidate();
            }
            else if (draggedSymbol != null)
            {
                draggedSymbol.X = e.X - dragOffset.X;
                draggedSymbol.Y = e.Y - dragOffset.Y;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            draggedSymbol = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                if (isWiring)
                {
                    wireStartPoint = null;
                    isWiring = false;
                    this.Invalidate();
                }

                if (mouseWiringMode)
                {
                    mouseWiringMode = false;
                    wirePoints.Clear();
                    this.Invalidate();
                }
            }
        }

        private void CreateFreeWire(List<Point> points)
        {
            // Crear cable libre que sigue los puntos dibujados
            for (int i = 0; i < points.Count - 1; i++)
            {
                var fakeStart = new ConnectionPoint
                {
                    X = points[i].X,
                    Y = points[i].Y,
                    Parent = new ElectricalSymbol { X = 0, Y = 0 }
                };

                var fakeEnd = new ConnectionPoint
                {
                    X = points[i + 1].X,
                    Y = points[i + 1].Y,
                    Parent = new ElectricalSymbol { X = 0, Y = 0 }
                };

                wires.Add(new Wire
                {
                    StartPoint = fakeStart,
                    EndPoint = fakeEnd,
                    Color = Color.Blue
                });
            }
        }

        private ElectricalSymbol GetSymbolAt(Point location)
        {
            for (int i = symbols.Count - 1; i >= 0; i--)
            {
                if (symbols[i].GetBounds().Contains(location))
                    return symbols[i];
            }
            return null;
        }

        private ConnectionPoint GetConnectionPointAt(Point location)
        {
            const int tolerance = 6;
            foreach (var symbol in symbols)
            {
                foreach (var cp in symbol.ConnectionPoints)
                {
                    Point pos = cp.GetAbsolutePosition();
                    if (Math.Abs(pos.X - location.X) <= tolerance &&
                        Math.Abs(pos.Y - location.Y) <= tolerance)
                        return cp;
                }
            }
            return null;
        }

        public void DeleteSelected()
        {
            if (selectedSymbol != null)
            {
                wires.RemoveAll(w => w.StartPoint.Parent == selectedSymbol || w.EndPoint.Parent == selectedSymbol);
                symbols.Remove(selectedSymbol);
                selectedSymbol = null;
                SymbolDeselected?.Invoke(this, EventArgs.Empty);
                this.Invalidate();
            }
        }

        public void ClearAll()
        {
            symbols.Clear();
            wires.Clear();
            selectedSymbol = null;
            this.Invalidate();
        }
    }
}