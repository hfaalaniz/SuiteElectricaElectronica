using BuckConverterCalculator.Controls;
using BuckConverterCalculator.Models;

namespace BuckConverterCalculator
{
    public partial class SchematicUnifilar : Form
    {
        private DiagramCanvas canvas;
        private SymbolToolbox toolbox;
        private PropertyGrid propertyGrid;

        public SchematicUnifilar()
        {
            InitializeComponent();
            SetupCustomComponents();
        }

        private void SetupCustomComponents()
        {
            this.Text = "Editor de Esquemas Eléctricos Unifilares";
            this.Size = new System.Drawing.Size(1200, 800);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var menuStrip = CreateMenuStrip();
            mainLayout.Controls.Add(menuStrip, 0, 0);
            mainLayout.SetColumnSpan(menuStrip, 3);

            toolbox = new SymbolToolbox
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.LightGray
            };
            mainLayout.Controls.Add(toolbox, 0, 1);

            canvas = new DiagramCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            canvas.SymbolSelected += Canvas_SymbolSelected;
            canvas.SymbolDeselected += Canvas_SymbolDeselected;
            mainLayout.Controls.Add(canvas, 1, 1);

            var propertyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.WhiteSmoke
            };

            var propLabel = new Label
            {
                Text = "Propiedades",
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            propertyPanel.Controls.Add(propLabel);

            propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                ToolbarVisible = false
            };
            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
            propertyPanel.Controls.Add(propertyGrid);
            propLabel.BringToFront();

            mainLayout.Controls.Add(propertyPanel, 2, 1);

            this.Controls.Add(mainLayout);
        }

        private MenuStrip CreateMenuStrip()
        {
            var menuStrip = new MenuStrip { Dock = DockStyle.Fill };

            var fileMenu = new ToolStripMenuItem("Archivo");
            fileMenu.DropDownItems.Add("Nuevo", null, (s, e) => NewDiagram());
            fileMenu.DropDownItems.Add("Salir", null, (s, e) => this.Close());
            menuStrip.Items.Add(fileMenu);

            var editMenu = new ToolStripMenuItem("Edición");
            editMenu.DropDownItems.Add("Eliminar seleccionado", null, (s, e) => canvas.DeleteSelected());
            editMenu.DropDownItems.Add("Limpiar todo", null, (s, e) => canvas.ClearAll());
            menuStrip.Items.Add(editMenu);

            var helpMenu = new ToolStripMenuItem("Ayuda");
            var helpItem = new ToolStripMenuItem("Instrucciones");
            helpItem.Click += (s, e) => MessageBox.Show(
                "CABLEADO:\n" +
                "• Punto a Punto: Clic en puntos azules de conexión\n" +
                "• Libre con Mouse: Mantén Ctrl + Clic izquierdo para trazar\n" +
                "• Finalizar cable libre: Clic derecho\n" +
                "• Cancelar: Esc o clic derecho\n\n" +
                "SÍMBOLOS:\n" +
                "• Arrastrar desde toolbox al canvas\n" +
                "• Seleccionar: clic sobre símbolo\n" +
                "• Editar propiedades en panel derecho",
                "Ayuda - Editor Eléctrico",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            helpMenu.DropDownItems.Add(helpItem);
            menuStrip.Items.Add(helpMenu);

            return menuStrip;
        }

        private void Canvas_SymbolSelected(object sender, ElectricalSymbol symbol)
        {
            propertyGrid.SelectedObject = symbol;
        }

        private void Canvas_SymbolDeselected(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = null;
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            canvas.Invalidate();
        }

        private void NewDiagram()
        {
            if (MessageBox.Show("¿Crear un nuevo diagrama? Se perderán los cambios no guardados.",
                "Nuevo Diagrama", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                canvas.ClearAll();
                propertyGrid.SelectedObject = null;
            }
        }
    }
}