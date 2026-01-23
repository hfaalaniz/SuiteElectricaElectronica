using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BuckConverterCalculator.SchematicEditor;
using System.ComponentModel;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Barra de componentes acoplable, expandible y flotante
    /// </summary>
    public class ComponentToolBox : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Button pinButton;
        private Button closeButton;
        private Button expandButton;
        private FlowLayoutPanel componentsPanel;
        private ToolTip toolTip;

        private bool isPinned = true;
        private bool isExpanded = true;
        private bool isDragging = false;
        private Point dragStartPoint;
        private Point originalLocation;

        private const int CollapsedWidth = 50;
        private const int ExpandedWidth = 180;
        private const int ItemHeight = 50;

        // Drag & Drop de componentes
        private ComponentButton draggedButton = null;
        private Form dragPreviewForm = null;

        public event EventHandler<ComponentSelectedEventArgs> ComponentSelected;
        public event EventHandler<ComponentDragEventArgs> ComponentDragStart;
        public event EventHandler<ComponentDragEventArgs> ComponentDragging;
        public event EventHandler<ComponentDragEventArgs> ComponentDragDrop;
        public event EventHandler<EventArgs> ExpandedChanged;

        private Dictionary<ComponentType, ComponentButton> componentButtons;

        public ComponentToolBox()
        {
            InitializeToolBox();
            CreateComponentButtons();
        }

        private void InitializeToolBox()
        {
            this.Size = new Size(ExpandedWidth, 600);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.BorderStyle = BorderStyle.FixedSingle;

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            // Title
            titleLabel = new Label
            {
                Text = "Components",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(5, 0)
            };

            // Expand/Collapse Button
            expandButton = new Button
            {
                Text = "◀",
                Size = new Size(25, 25),
                Location = new Point(ExpandedWidth - 30, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            expandButton.FlatAppearance.BorderSize = 0;
            expandButton.Click += ExpandButton_Click;

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(expandButton);

            // Components Panel
            componentsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            this.Controls.Add(componentsPanel);
            this.Controls.Add(headerPanel);

            // ToolTip
            toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100
            };
        }

        private void CreateComponentButtons()
        {
            componentButtons = new Dictionary<ComponentType, ComponentButton>();

            // TODOS LOS COMPONENTES ORGANIZADOS
            var components = new List<ComponentDefinition>
            {
                // === CONEXIONES ===
                new ComponentDefinition(ComponentType.Terminal, "Terminal", "📍", Color.FromArgb(220, 20, 60)),
                new ComponentDefinition(ComponentType.Wire, "Wire", "━", Color.FromArgb(0, 0, 0)),
                new ComponentDefinition(ComponentType.Node, "Node", "⬤", Color.FromArgb(255, 215, 0)),
                new ComponentDefinition(ComponentType.Ground, "Ground", "⏚", Color.FromArgb(105, 105, 105)),
                new ComponentDefinition(ComponentType.VccSupply, "VCC Supply", "⚡", Color.FromArgb(255, 0, 0)),
                
                // === PASIVOS ===
                new ComponentDefinition(ComponentType.Resistor, "Resistor", "⚡", Color.FromArgb(139, 90, 43)),
                new ComponentDefinition(ComponentType.Capacitor, "Capacitor", "🔋", Color.FromArgb(70, 130, 180)),
                new ComponentDefinition(ComponentType.Inductor, "Inductor", "🌀", Color.FromArgb(184, 115, 51)),
                new ComponentDefinition(ComponentType.Potentiometer, "Potentiometer", "🎚", Color.FromArgb(160, 82, 45)),
                new ComponentDefinition(ComponentType.Fuse, "Fuse", "⚠", Color.FromArgb(255, 165, 0)),
                
                // === SEMICONDUCTORES ===
                new ComponentDefinition(ComponentType.Diode, "Diode", "🔺", Color.FromArgb(178, 34, 34)),
                new ComponentDefinition(ComponentType.SchottkyDiode, "Schottky Diode", "◥", Color.FromArgb(220, 20, 60)),
                new ComponentDefinition(ComponentType.ZenerDiode, "Zener Diode", "◢", Color.FromArgb(255, 69, 0)),
                new ComponentDefinition(ComponentType.LED, "LED", "💡", Color.FromArgb(255, 215, 0)),
                new ComponentDefinition(ComponentType.BridgeDiode, "Bridge Rectifier", "◆", Color.FromArgb(139, 0, 0)),
                new ComponentDefinition(ComponentType.Mosfet, "MOSFET", "🔲", Color.FromArgb(75, 0, 130)),
                new ComponentDefinition(ComponentType.Mosfet, "BJT Transistor", "🔶", Color.FromArgb(255, 140, 0)),
                new ComponentDefinition(ComponentType.Triac, "TRIAC", "◬", Color.FromArgb(128, 0, 128)),
                new ComponentDefinition(ComponentType.Thyristor, "Thyristor (SCR)", "◭", Color.FromArgb(148, 0, 211)),
                
                // === ICs Y LÓGICA ===
                new ComponentDefinition(ComponentType.IC, "IC Generic", "⬛", Color.FromArgb(50, 50, 50)),
                new ComponentDefinition(ComponentType.OpAmp, "Op-Amp", "△", Color.FromArgb(0, 100, 0)),
                new ComponentDefinition(ComponentType.LogicGate, "Logic Gate", "∧", Color.FromArgb(0, 0, 139)),
                new ComponentDefinition(ComponentType.Inverter, "Inverter (NOT)", "¬", Color.FromArgb(25, 25, 112)),
                new ComponentDefinition(ComponentType.FlipFlop, "Flip-Flop", "⊟", Color.FromArgb(72, 61, 139)),
                
                // === DISPLAYS Y SALIDAS ===
                new ComponentDefinition(ComponentType.LED7Segment, "7-Segment Display", "⓪", Color.FromArgb(255, 0, 0)),
                new ComponentDefinition(ComponentType.Buzzer, "Buzzer", "🔊", Color.FromArgb(255, 140, 0)),
                
                // === OTROS ===
                new ComponentDefinition(ComponentType.Optocoupler, "Optocoupler", "⇄", Color.FromArgb(128, 128, 0)),
                new ComponentDefinition(ComponentType.Transformer, "Transformer", "⊗", Color.FromArgb(139, 69, 19)),
                new ComponentDefinition(ComponentType.Battery, "Battery", "🔋", Color.FromArgb(0, 128, 0)),
                new ComponentDefinition(ComponentType.Label, "Label", "📝", Color.FromArgb(0, 100, 0))
            };

            foreach (var comp in components)
            {
                var button = new ComponentButton(comp, isExpanded);

                // Eventos de drag & drop
                button.MouseDown += ComponentButton_MouseDown;
                button.MouseMove += ComponentButton_MouseMove;
                button.MouseUp += ComponentButton_MouseUp;

                button.Width = isExpanded ? ExpandedWidth - 20 : CollapsedWidth - 10;
                componentsPanel.Controls.Add(button);

                // Usar nombre único para componentes duplicados (BJT)
                var key = comp.Type;
                if (comp.Name.Contains("BJT"))
                {
                    // Para BJT usamos un tipo especial o lo manejamos en el evento
                    button.Tag = "BJT"; // Marcar este botón como BJT
                }

                if (!componentButtons.ContainsKey(key))
                {
                    componentButtons[key] = button;
                }

                toolTip.SetToolTip(button, comp.Name);
            }
        }

        private void ComponentButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is ComponentButton btn)
            {
                draggedButton = btn;
                dragStartPoint = e.Location;
            }
        }

        private void ComponentButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedButton != null && e.Button == MouseButtons.Left && sender is ComponentButton btn)
            {
                // Verificar si se movió suficiente para iniciar drag
                int dragThreshold = 5;
                if (Math.Abs(e.X - dragStartPoint.X) > dragThreshold ||
                    Math.Abs(e.Y - dragStartPoint.Y) > dragThreshold)
                {
                    StartComponentDrag(draggedButton);
                }
            }
        }

        private void ComponentButton_MouseUp(object sender, MouseEventArgs e)
        {
            draggedButton = null;
        }

        private void StartComponentDrag(ComponentButton button)
        {
            // Crear objeto de datos para el drag & drop
            var dragData = new ComponentDragData
            {
                ComponentName = button.Definition.Name,
                Icon = button.Definition.Icon
            };
            dragData.SetComponentType(button.Definition.Type);
            dragData.SetColor(button.Definition.Color);

            // Marcar si es BJT
            if (button.Tag != null && button.Tag.ToString() == "BJT")
            {
                dragData.IsBJT = true;
            }

            // Evento de inicio de drag
            ComponentDragStart?.Invoke(this, new ComponentDragEventArgs
            {
                ComponentType = button.Definition.Type,
                ComponentName = button.Definition.Name,
                ScreenLocation = Cursor.Position
            });

            // Crear imagen de arrastre personalizada
            CreateDragImage(button.Definition);

            // Iniciar drag & drop nativo de Windows
            DragDropEffects result = button.DoDragDrop(dragData, DragDropEffects.Copy);

            // Limpiar imagen de arrastre
            CleanupDragImage();

            if (result == DragDropEffects.Copy)
            {
                // Drop exitoso
                ComponentDragDrop?.Invoke(this, new ComponentDragEventArgs
                {
                    ComponentType = dragData.GetComponentType(),
                    ComponentName = dragData.ComponentName,
                    ScreenLocation = Cursor.Position
                });
            }

            draggedButton = null;
        }

        private void CreateDragImage(ComponentDefinition definition)
        {
            // Crear formulario para mostrar imagen durante drag
            dragPreviewForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White,
                Opacity = 0.8,
                Size = new Size(120, 50),
                TopMost = true,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual
            };

            var label = new Label
            {
                Text = $"{definition.Icon} {definition.Name}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = definition.Color,
                BackColor = Color.White
            };

            dragPreviewForm.Controls.Add(label);
            dragPreviewForm.Location = Cursor.Position;
            dragPreviewForm.Show();

            // Timer para actualizar posición
            var timer = new System.Windows.Forms.Timer { Interval = 10 };
            timer.Tick += (s, e) =>
            {
                if (dragPreviewForm != null && !dragPreviewForm.IsDisposed)
                {
                    dragPreviewForm.Location = new Point(
                        Cursor.Position.X + 10,
                        Cursor.Position.Y + 10
                    );
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void CleanupDragImage()
        {
            if (dragPreviewForm != null)
            {
                dragPreviewForm.Close();
                dragPreviewForm.Dispose();
                dragPreviewForm = null;
            }
        }

        private void ExpandButton_Click(object sender, EventArgs e)
        {
            isExpanded = !isExpanded;

            if (isExpanded)
            {
                this.Width = ExpandedWidth;
                expandButton.Text = "◀";
                expandButton.Location = new Point(ExpandedWidth - 30, 2);
                foreach (Control ctrl in componentsPanel.Controls)
                {
                    if (ctrl is ComponentButton btn)
                    {
                        btn.Width = ExpandedWidth - 20;
                        btn.SetExpanded(true);
                    }
                }
            }
            else
            {
                this.Width = CollapsedWidth;
                expandButton.Text = "▶";
                expandButton.Location = new Point(CollapsedWidth - 30, 2);
                foreach (Control ctrl in componentsPanel.Controls)
                {
                    if (ctrl is ComponentButton btn)
                    {
                        btn.Width = CollapsedWidth - 10;
                        btn.SetExpanded(false);
                    }
                }
            }

            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPinned
        {
            get { return isPinned; }
            set { isPinned = value; }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
        }

        public void ToggleExpand()
        {
            ExpandButton_Click(this, EventArgs.Empty);
        }

        public void Show()
        {
            this.Visible = true;
        }

        public void Hide()
        {
            this.Visible = false;
        }

        public void DockToLeft(Control parent, int topMargin = 0)
        {
            this.Parent = parent;
            this.Location = new Point(0, topMargin);
            this.Height = parent.Height - topMargin;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            isPinned = true;
        }

        public void DockToRight(Control parent, int topMargin = 0)
        {
            this.Parent = parent;
            this.Location = new Point(parent.Width - this.Width, topMargin);
            this.Height = parent.Height - topMargin;
            this.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            isPinned = true;
        }

        public void Float(Point location)
        {
            this.Location = location;
            this.Anchor = AnchorStyles.None;
            isPinned = false;
        }
    }

    /// <summary>
    /// Definición de componente para la ToolBox
    /// </summary>
    public class ComponentDefinition
    {
        public ComponentType Type { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Color Color { get; set; }

        public ComponentDefinition(ComponentType type, string name, string icon, Color color)
        {
            Type = type;
            Name = name;
            Icon = icon;
            Color = color;
        }
    }

    /// <summary>
    /// Botón personalizado para componente
    /// </summary>
    public class ComponentButton : Button
    {
        public ComponentDefinition Definition { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsExpanded { get; set; }

        public ComponentButton(ComponentDefinition definition, bool expanded)
        {
            Definition = definition;
            IsExpanded = expanded;

            this.Height = 48;
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = Color.White;
            this.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 240, 255);
            this.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 220, 255);
            this.Cursor = Cursors.Hand;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Font = new Font("Segoe UI", 9);
            this.Margin = new Padding(5, 2, 5, 2);

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (IsExpanded)
            {
                this.Text = $"  {Definition.Icon}  {Definition.Name}";
            }
            else
            {
                this.Text = Definition.Icon;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.Font = new Font("Segoe UI", 16);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsExpanded)
            {
                // Dibujar barra de color en el borde izquierdo
                using (SolidBrush brush = new SolidBrush(Definition.Color))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 4, this.Height);
                }
            }
        }

        public void SetExpanded(bool expanded)
        {
            IsExpanded = expanded;
            UpdateDisplay();
            this.Invalidate();
        }
    }

    /// <summary>
    /// EventArgs para ComponentSelected
    /// </summary>
    public class ComponentSelectedEventArgs : EventArgs
    {
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; }
    }

    /// <summary>
    /// EventArgs para ComponentDrag
    /// </summary>
    public class ComponentDragEventArgs : EventArgs
    {
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; }
        public Point ScreenLocation { get; set; }
    }

    /// <summary>
    /// Datos para drag & drop de componentes
    /// </summary>
    [Serializable]
    public class ComponentDragData
    {
        // Usar string en lugar de enum para garantizar serialización
        public string ComponentTypeName { get; set; }
        public string ComponentName { get; set; }
        public string Icon { get; set; }
        public int ColorArgb { get; set; }
        public bool IsBJT { get; set; } = false; // Nuevo campo para distinguir BJT

        public ComponentType GetComponentType()
        {
            return (ComponentType)Enum.Parse(typeof(ComponentType), ComponentTypeName);
        }

        public void SetComponentType(ComponentType type)
        {
            ComponentTypeName = type.ToString();
        }

        public Color GetColor()
        {
            return Color.FromArgb(ColorArgb);
        }

        public void SetColor(Color color)
        {
            ColorArgb = color.ToArgb();
        }
    }
}















/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BuckConverterCalculator.SchematicEditor;
using System.ComponentModel;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Barra de componentes acoplable, expandible y flotante
    /// </summary>
    public class ComponentToolBox : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Button pinButton;
        private Button closeButton;
        private Button expandButton;
        private FlowLayoutPanel componentsPanel;
        private ToolTip toolTip;

        private bool isPinned = true;
        private bool isExpanded = true;
        private bool isDragging = false;
        private Point dragStartPoint;
        private Point originalLocation;

        private const int CollapsedWidth = 50;
        private const int ExpandedWidth = 160;
        private const int ItemHeight = 50;

        // Drag & Drop de componentes
        private ComponentButton draggedButton = null;
        private Form dragPreviewForm = null;

        public event EventHandler<ComponentSelectedEventArgs> ComponentSelected;
        public event EventHandler<ComponentDragEventArgs> ComponentDragStart;
        public event EventHandler<ComponentDragEventArgs> ComponentDragging;
        public event EventHandler<ComponentDragEventArgs> ComponentDragDrop;
        public event EventHandler<EventArgs> ExpandedChanged;  // Nuevo evento

        private Dictionary<ComponentType, ComponentButton> componentButtons;

        public ComponentToolBox()
        {
            InitializeToolBox();
            CreateComponentButtons();
        }

        private void InitializeToolBox()
        {
            this.Size = new Size(ExpandedWidth, 600);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.BorderStyle = BorderStyle.FixedSingle;

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(45, 45, 48),
                Cursor = Cursors.SizeAll
            };
            headerPanel.MouseDown += Header_MouseDown;
            headerPanel.MouseMove += Header_MouseMove;
            headerPanel.MouseUp += Header_MouseUp;

            // Title
            titleLabel = new Label
            {
                Text = "Components",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(5, 0)
            };

            // Expand/Collapse Button
            expandButton = new Button
            {
                Text = "◀",
                Size = new Size(25, 25),
                Location = new Point(ExpandedWidth - 85, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            expandButton.FlatAppearance.BorderSize = 0;
            expandButton.Click += ExpandButton_Click;

            // Pin Button
            pinButton = new Button
            {
                Text = "📌",
                Size = new Size(25, 25),
                Location = new Point(ExpandedWidth - 55, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(62, 120, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            pinButton.FlatAppearance.BorderSize = 0;
            pinButton.Click += PinButton_Click;

            // Close Button
            closeButton = new Button
            {
                Text = "✕",
                Size = new Size(25, 25),
                Location = new Point(ExpandedWidth - 28, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += CloseButton_Click;

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(expandButton);
            headerPanel.Controls.Add(pinButton);
            headerPanel.Controls.Add(closeButton);

            // Components Panel
            componentsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            this.Controls.Add(componentsPanel);
            this.Controls.Add(headerPanel);

            // ToolTip
            toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100
            };
        }

        private void CreateComponentButtons()
        {
            componentButtons = new Dictionary<ComponentType, ComponentButton>();

            // Definir componentes con iconos
            var components = new List<ComponentDefinition>
            {
                new ComponentDefinition(ComponentType.Terminal, "Terminal", "📍", Color.FromArgb(220, 20, 60)),
                new ComponentDefinition(ComponentType.Resistor, "Resistor", "⚡", Color.FromArgb(139, 90, 43)),
                new ComponentDefinition(ComponentType.Capacitor, "Capacitor", "🔋", Color.FromArgb(70, 130, 180)),
                new ComponentDefinition(ComponentType.Inductor, "Inductor", "🌀", Color.FromArgb(184, 115, 51)),
                new ComponentDefinition(ComponentType.Diode, "Diode", "🔺", Color.FromArgb(178, 34, 34)),
                new ComponentDefinition(ComponentType.Mosfet, "MOSFET", "🔲", Color.FromArgb(75, 0, 130)),
                new ComponentDefinition(ComponentType.Mosfet, "BJT", "🔶", Color.FromArgb(255, 140, 0)),
                new ComponentDefinition(ComponentType.IC, "IC", "⬛", Color.FromArgb(50, 50, 50)),
                new ComponentDefinition(ComponentType.Fuse, "Fuse", "⚠", Color.FromArgb(255, 165, 0)),
                new ComponentDefinition(ComponentType.Ground, "Ground", "⏚", Color.FromArgb(105, 105, 105)),
                new ComponentDefinition(ComponentType.VccSupply, "Vcc", "⚡", Color.FromArgb(255, 0, 0)),
                new ComponentDefinition(ComponentType.Wire, "Wire", "━", Color.FromArgb(0, 0, 0)),
                new ComponentDefinition(ComponentType.Node, "Node", "⬤", Color.FromArgb(255, 215, 0)),
                new ComponentDefinition(ComponentType.Label, "Label", "📝", Color.FromArgb(0, 100, 0))
            };

            foreach (var comp in components)
            {
                var button = new ComponentButton(comp, isExpanded);

                // Eventos de drag & drop
                button.MouseDown += ComponentButton_MouseDown;
                button.MouseMove += ComponentButton_MouseMove;
                button.MouseUp += ComponentButton_MouseUp;

                button.Width = isExpanded ? ExpandedWidth - 20 : CollapsedWidth - 10;
                componentsPanel.Controls.Add(button);

                if (!componentButtons.ContainsKey(comp.Type))
                {
                    componentButtons[comp.Type] = button;
                }

                toolTip.SetToolTip(button, comp.Name);
            }
        }

        private void ComponentButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is ComponentButton btn)
            {
                draggedButton = btn;
                dragStartPoint = e.Location;
            }
        }

        private void ComponentButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedButton != null && e.Button == MouseButtons.Left && sender is ComponentButton btn)
            {
                // Verificar si se movió suficiente para iniciar drag
                int dragThreshold = 5;
                if (Math.Abs(e.X - dragStartPoint.X) > dragThreshold ||
                    Math.Abs(e.Y - dragStartPoint.Y) > dragThreshold)
                {
                    StartComponentDrag(draggedButton);
                }
            }
        }

        private void ComponentButton_MouseUp(object sender, MouseEventArgs e)
        {
            draggedButton = null;
        }

        private void StartComponentDrag(ComponentButton button)
        {
            // Crear objeto de datos para el drag & drop
            var dragData = new ComponentDragData
            {
                ComponentName = button.Definition.Name,
                Icon = button.Definition.Icon
            };
            dragData.SetComponentType(button.Definition.Type);
            dragData.SetColor(button.Definition.Color);

            // Evento de inicio de drag
            ComponentDragStart?.Invoke(this, new ComponentDragEventArgs
            {
                ComponentType = button.Definition.Type,
                ComponentName = button.Definition.Name,
                ScreenLocation = Cursor.Position
            });

            // Crear imagen de arrastre personalizada
            CreateDragImage(button.Definition);

            // Iniciar drag & drop nativo de Windows
            DragDropEffects result = button.DoDragDrop(dragData, DragDropEffects.Copy);

            // Limpiar imagen de arrastre
            CleanupDragImage();

            if (result == DragDropEffects.Copy)
            {
                // Drop exitoso
                ComponentDragDrop?.Invoke(this, new ComponentDragEventArgs
                {
                    ComponentType = dragData.GetComponentType(),
                    ComponentName = dragData.ComponentName,
                    ScreenLocation = Cursor.Position
                });
            }

            draggedButton = null;
        }

        private void CreateDragImage(ComponentDefinition definition)
        {
            // Crear formulario para mostrar imagen durante drag
            dragPreviewForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = Color.White,
                TransparencyKey = Color.White,
                Opacity = 0.8,
                Size = new Size(80, 80),
                AllowTransparency = true,
                Location = new Point(Cursor.Position.X - 40, Cursor.Position.Y - 40)
            };

            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            previewPanel.Paint += (s, e) => DrawComponentPreview(e.Graphics, definition);
            dragPreviewForm.Controls.Add(previewPanel);
            dragPreviewForm.Show();

            // Timer para actualizar posición
            System.Windows.Forms.Timer positionTimer = new System.Windows.Forms.Timer { Interval = 10 };
            positionTimer.Tick += (s, e) =>
            {
                if (dragPreviewForm != null && !dragPreviewForm.IsDisposed)
                {
                    dragPreviewForm.Location = new Point(Cursor.Position.X - 40, Cursor.Position.Y - 40);

                    // Evento de dragging
                    ComponentDragging?.Invoke(this, new ComponentDragEventArgs
                    {
                        ComponentType = draggedButton?.Definition.Type ?? ComponentType.Resistor,
                        ComponentName = draggedButton?.Definition.Name ?? "",
                        ScreenLocation = Cursor.Position
                    });
                }
                else
                {
                    positionTimer.Stop();
                }
            };
            positionTimer.Start();
            dragPreviewForm.Tag = positionTimer;
        }

        private void CleanupDragImage()
        {
            if (dragPreviewForm != null && !dragPreviewForm.IsDisposed)
            {
                if (dragPreviewForm.Tag is System.Windows.Forms.Timer timer)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                dragPreviewForm.Close();
                dragPreviewForm.Dispose();
                dragPreviewForm = null;
            }
        }

        private void DrawComponentPreview(Graphics g, ComponentDefinition definition)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Dibujar icono grande centrado
            using (Font iconFont = new Font("Segoe UI", 32))
            using (Brush iconBrush = new SolidBrush(definition.Color))
            using (Pen borderPen = new Pen(definition.Color, 2))
            {
                // Borde
                g.DrawRectangle(borderPen, 2, 2, 76, 76);

                // Icono
                SizeF iconSize = g.MeasureString(definition.Icon, iconFont);
                g.DrawString(definition.Icon, iconFont, iconBrush,
                    40 - iconSize.Width / 2, 40 - iconSize.Height / 2);

                // Nombre
                using (Font nameFont = new Font("Arial", 7, FontStyle.Bold))
                {
                    SizeF nameSize = g.MeasureString(definition.Name, nameFont);
                    g.DrawString(definition.Name, nameFont, Brushes.Black,
                        40 - nameSize.Width / 2, 65);
                }
            }
        }

        private void ExpandButton_Click(object sender, EventArgs e)
        {
            isExpanded = !isExpanded;

            if (isExpanded)
            {
                this.Width = ExpandedWidth;
                expandButton.Text = "◀";
                titleLabel.Visible = true;
            }
            else
            {
                this.Width = CollapsedWidth;
                expandButton.Text = "▶";
                titleLabel.Visible = false;
            }

            // Actualizar posición de botones en header
            expandButton.Location = new Point(this.Width - 85, 2);
            pinButton.Location = new Point(this.Width - 55, 2);
            closeButton.Location = new Point(this.Width - 28, 2);

            // Actualizar botones de componentes
            foreach (Control ctrl in componentsPanel.Controls)
            {
                if (ctrl is ComponentButton btn)
                {
                    btn.IsExpanded = isExpanded;
                    btn.Width = isExpanded ? ExpandedWidth - 20 : CollapsedWidth - 10;
                }
            }

            // Notificar cambio de estado
            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            isPinned = !isPinned;

            if (isPinned)
            {
                pinButton.BackColor = Color.FromArgb(62, 120, 180);
                headerPanel.Cursor = Cursors.Default;
                this.Cursor = Cursors.Default;
            }
            else
            {
                pinButton.BackColor = Color.FromArgb(100, 100, 100);
                headerPanel.Cursor = Cursors.SizeAll;
                this.Cursor = Cursors.Default;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isPinned && e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = e.Location;
                originalLocation = this.Location;
            }
        }

        private void Header_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && !isPinned)
            {
                Point newLocation = new Point(
                    originalLocation.X + (e.X - dragStartPoint.X),
                    originalLocation.Y + (e.Y - dragStartPoint.Y)
                );

                this.Location = newLocation;
            }
        }

        private void Header_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsPinned
        {
            get { return isPinned; }
            set
            {
                isPinned = value;
                PinButton_Click(this, EventArgs.Empty);
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
        }

        public void ToggleExpand()
        {
            ExpandButton_Click(this, EventArgs.Empty);
        }

        public void Show()
        {
            this.Visible = true;
        }

        public void Hide()
        {
            this.Visible = false;
        }

        public void DockToLeft(Control parent, int topMargin = 0)
        {
            this.Parent = parent;
            this.Location = new Point(0, topMargin);
            this.Height = parent.Height - topMargin;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            isPinned = true;
            pinButton.BackColor = Color.FromArgb(62, 120, 180);
        }

        public void DockToRight(Control parent, int topMargin = 0)
        {
            this.Parent = parent;
            this.Location = new Point(parent.Width - this.Width, topMargin);
            this.Height = parent.Height - topMargin;
            this.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            isPinned = true;
            pinButton.BackColor = Color.FromArgb(62, 120, 180);
        }

        public void Float(Point location)
        {
            this.Location = location;
            this.Anchor = AnchorStyles.None;
            isPinned = false;
            pinButton.BackColor = Color.FromArgb(100, 100, 100);
        }
    }

    /// <summary>
    /// Definición de componente para la ToolBox
    /// </summary>
    public class ComponentDefinition
    {
        public ComponentType Type { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Color Color { get; set; }

        public ComponentDefinition(ComponentType type, string name, string icon, Color color)
        {
            Type = type;
            Name = name;
            Icon = icon;
            Color = color;
        }
    }

    /// <summary>
    /// Botón personalizado para componente
    /// </summary>
    public class ComponentButton : Button
    {
        public ComponentDefinition Definition { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsExpanded { get; set; }

        public ComponentButton(ComponentDefinition definition, bool expanded)
        {
            Definition = definition;
            IsExpanded = expanded;

            this.Height = 48;
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = Color.White;
            this.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 240, 255);
            this.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 220, 255);
            this.Cursor = Cursors.Hand;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Font = new Font("Segoe UI", 9);
            this.Margin = new Padding(5, 2, 5, 2);

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (IsExpanded)
            {
                this.Text = $"  {Definition.Icon}  {Definition.Name}";
            }
            else
            {
                this.Text = Definition.Icon;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.Font = new Font("Segoe UI", 16);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsExpanded)
            {
                // Dibujar barra de color en el borde izquierdo
                using (SolidBrush brush = new SolidBrush(Definition.Color))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 4, this.Height);
                }
            }
        }

        public void SetExpanded(bool expanded)
        {
            IsExpanded = expanded;
            UpdateDisplay();
            this.Invalidate();
        }
    }

    /// <summary>
    /// EventArgs para ComponentSelected
    /// </summary>
    public class ComponentSelectedEventArgs : EventArgs
    {
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; }
    }

    /// <summary>
    /// EventArgs para ComponentDrag
    /// </summary>
    public class ComponentDragEventArgs : EventArgs
    {
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; }
        public Point ScreenLocation { get; set; }
    }

    /// <summary>
    /// Datos para drag & drop de componentes
    /// </summary>
    [Serializable]
    public class ComponentDragData
    {
        // Usar string en lugar de enum para garantizar serialización
        public string ComponentTypeName { get; set; }
        public string ComponentName { get; set; }
        public string Icon { get; set; }
        public int ColorArgb { get; set; }

        public ComponentType GetComponentType()
        {
            return (ComponentType)Enum.Parse(typeof(ComponentType), ComponentTypeName);
        }

        public void SetComponentType(ComponentType type)
        {
            ComponentTypeName = type.ToString();
        }

        public Color GetColor()
        {
            return Color.FromArgb(ColorArgb);
        }

        public void SetColor(Color color)
        {
            ColorArgb = color.ToArgb();
        }
    }
}


*/