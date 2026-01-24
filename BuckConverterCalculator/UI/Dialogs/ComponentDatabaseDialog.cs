using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using BuckConverterCalculator.Database;
using ElectronicComponent = BuckConverterCalculator.Database.ElectronicComponent;


namespace BuckConverterCalculator.UI.Dialogs
{
    /// <summary>
    /// Diálogo para gestionar la base de datos de componentes
    /// </summary>
    public class ComponentDatabaseDialog : Form
    {
        private ComponentDatabase database;
        private DataGridView gridComponents;
        private TextBox txtSearch;
        private ComboBox cboType;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblCount;

        public ComponentDatabaseDialog(ComponentDatabase db)
        {
            this.database = db;
            InitializeComponent();
            LoadComponents();
        }

        private void InitializeComponent()
        {
            this.Text = "Component Database";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 500);

            // Toolbar panel
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Search
            var lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            toolbar.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(70, 12),
                Width = 250
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            toolbar.Controls.Add(txtSearch);

            // Type filter
            var lblType = new Label
            {
                Text = "Type:",
                Location = new Point(340, 15),
                AutoSize = true
            };
            toolbar.Controls.Add(lblType);

            cboType = new ComboBox
            {
                Location = new Point(385, 12),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboType.Items.AddRange(new object[] { "All", "MOSFET", "Inductor", "Capacitor", "Diode", "IC", "Resistor" });
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
            toolbar.Controls.Add(cboType);

            // Buttons
            btnAdd = new Button
            {
                Text = "Add New",
                Location = new Point(10, 45),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            toolbar.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "Edit",
                Location = new Point(120, 45),
                Width = 100,
                Height = 30,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;
            toolbar.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(230, 45),
                Width = 100,
                Height = 30,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            toolbar.Controls.Add(btnDelete);

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(340, 45),
                Width = 100,
                Height = 30
            };
            btnRefresh.Click += (s, e) => LoadComponents();
            toolbar.Controls.Add(btnRefresh);

            lblCount = new Label
            {
                Location = new Point(550, 50),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            toolbar.Controls.Add(lblCount);

            // DataGridView
            gridComponents = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 245)
                }
            };
            gridComponents.SelectionChanged += GridComponents_SelectionChanged;
            gridComponents.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) BtnEdit_Click(s, e); };

            // Bottom panel
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(bottomPanel.Width - 120, 10),
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Right
            };
            btnClose.Click += (s, e) => this.Close();
            bottomPanel.Controls.Add(btnClose);

            this.Controls.Add(gridComponents);
            this.Controls.Add(toolbar);
            this.Controls.Add(bottomPanel);
        }

        private void LoadComponents()
        {
            var components = database.SearchComponents(new ComponentSearchCriteria());

            gridComponents.DataSource = null;
            gridComponents.Columns.Clear();

            // Create columns
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", Width = 80 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Manufacturer", HeaderText = "Manufacturer", Width = 120 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "PartNumber", HeaderText = "Part Number", Width = 150 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 250 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Price", Width = 70 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", Width = 70 });
            gridComponents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Supplier", HeaderText = "Supplier", Width = 100 });

            // Fill rows
            foreach (var comp in components)
            {
                gridComponents.Rows.Add(
                    comp.Type,
                    comp.Manufacturer,
                    comp.PartNumber,
                    comp.Description,
                    $"${comp.UnitPrice:F2}",
                    comp.Stock,
                    comp.Supplier
                );
            }

            lblCount.Text = $"Total: {components.Count} components";
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterComponents();
        }

        private void CboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterComponents();
        }

        private void FilterComponents()
        {
            var criteria = new ComponentSearchCriteria();

            if (cboType.SelectedIndex > 0)
                criteria.Type = cboType.SelectedItem.ToString();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                criteria.PartNumber = txtSearch.Text;

            var filtered = database.SearchComponents(criteria);

            gridComponents.Rows.Clear();
            foreach (var comp in filtered)
            {
                gridComponents.Rows.Add(
                    comp.Type,
                    comp.Manufacturer,
                    comp.PartNumber,
                    comp.Description,
                    $"${comp.UnitPrice:F2}",
                    comp.Stock,
                    comp.Supplier
                );
            }

            lblCount.Text = $"Total: {filtered.Count} components";
        }

        private void GridComponents_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = gridComponents.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var editor = new ComponentEditorDialog(null);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                database.AddComponent(editor.Component);
                LoadComponents();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (gridComponents.SelectedRows.Count == 0) return;

            var row = gridComponents.SelectedRows[0];
            string partNumber = row.Cells["PartNumber"].Value.ToString();

            var component = database.SearchComponents(new ComponentSearchCriteria { PartNumber = partNumber }).FirstOrDefault();
            if (component != null)
            {
                var editor = new ComponentEditorDialog(component);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    database.UpdateComponent(editor.Component);
                    LoadComponents();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gridComponents.SelectedRows.Count == 0) return;

            var row = gridComponents.SelectedRows[0];
            string partNumber = row.Cells["PartNumber"].Value.ToString();

            var result = MessageBox.Show(
                $"Are you sure you want to delete component {partNumber}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                database.DeleteComponent(partNumber);
                LoadComponents();
            }
        }
    }

    /// <summary>
    /// Diálogo para editar/crear componente
    /// </summary>
    internal class ComponentEditorDialog : Form
    {
        public ElectronicComponent Component { get; private set; }


        private TextBox txtType;
        private TextBox txtManufacturer;
        private TextBox txtPartNumber;
        private TextBox txtDescription;
        private TextBox txtPrice;
        private TextBox txtStock;
        private TextBox txtSupplier;
        private TextBox txtDatasheet;
        private Button btnOK;
        private Button btnCancel;

        public ComponentEditorDialog(ElectronicComponent component)
        {
            this.Component = component ?? new ElectronicComponent();
            InitializeComponent();
            LoadComponent();
        }

        private void InitializeComponent()
        {
            this.Text = Component.PartNumber == null ? "Add Component" : "Edit Component";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 20;

            AddField("Type:", ref txtType, ref y);
            AddField("Manufacturer:", ref txtManufacturer, ref y);
            AddField("Part Number:", ref txtPartNumber, ref y);
            AddField("Description:", ref txtDescription, ref y);
            AddField("Unit Price:", ref txtPrice, ref y);
            AddField("Stock:", ref txtStock, ref y);
            AddField("Supplier:", ref txtSupplier, ref y);
            AddField("Datasheet URL:", ref txtDatasheet, ref y);

            y += 20;

            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(280, y),
                Width = 90,
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(380, y),
                Width = 90,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void AddField(string label, ref TextBox textBox, ref int y)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(20, y),
                Width = 120,
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(lbl);

            textBox = new TextBox
            {
                Location = new Point(150, y),
                Width = 320
            };
            this.Controls.Add(textBox);

            y += 35;
        }

        private void LoadComponent()
        {
            txtType.Text = Component.Type ?? "MOSFET";
            txtManufacturer.Text = Component.Manufacturer ?? "";
            txtPartNumber.Text = Component.PartNumber ?? "";
            txtDescription.Text = Component.Description ?? "";
            txtPrice.Text = Component.UnitPrice.ToString("F2");
            txtStock.Text = Component.Stock.ToString();
            txtSupplier.Text = Component.Supplier ?? "DigiKey";
            txtDatasheet.Text = Component.DatasheetURL ?? "";
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPartNumber.Text))
            {
                MessageBox.Show("Part number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Component.Type = txtType.Text;
            Component.Manufacturer = txtManufacturer.Text;
            Component.PartNumber = txtPartNumber.Text;
            Component.Description = txtDescription.Text;
            Component.Supplier = txtSupplier.Text;
            Component.DatasheetURL = txtDatasheet.Text;

            if (double.TryParse(txtPrice.Text, out double price))
                Component.UnitPrice = price;

            if (int.TryParse(txtStock.Text, out int stock))
                Component.Stock = stock;

            Component.LastUpdated = DateTime.Now;
        }
    }
}