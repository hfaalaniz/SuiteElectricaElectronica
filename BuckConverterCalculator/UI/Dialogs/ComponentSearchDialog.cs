using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BuckConverterCalculator.Database;
using BuckConverterCalculator.Services;

namespace BuckConverterCalculator.UI.Dialogs
{
    public class ComponentSearchDialog : Form
    {
        private ComponentDatabase database;
        private ComponentScraperService scraperService;
        private readonly LoggerService _logger = LoggerService.Instance;

        public Database.ElectronicComponent SelectedComponent { get; private set; }

        private ComboBox cboType;
        private ComboBox cboSource;
        private TextBox txtManufacturer;
        private TextBox txtPartNumber;
        private TextBox txtMinVoltage;
        private TextBox txtMinCurrent;
        private TextBox txtMaxRDSon;
        private TextBox txtMinInductance;
        private TextBox txtMinCapacitance;
        private TextBox txtMaxPrice;
        private CheckBox chkInStockOnly;
        private CheckBox chkSortByPrice;
        private DataGridView gridResults;
        private Button btnSearch;
        private Button btnSelect;
        private Button btnCancel;
        private Button btnOpenLog;
        private Label lblResults;
        private ProgressBar progressBar;

        public ComponentSearchDialog(ComponentDatabase db)
        {
            _logger.Info("Initializing ComponentSearchDialog", "SearchDialog");
            this.database = db;
            this.scraperService = new ComponentScraperService();
            InitializeComponent();
            _logger.Info("ComponentSearchDialog initialized successfully", "SearchDialog");
        }

        private void InitializeComponent()
        {
            this.Text = "Search Components";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(800, 600);

            var criteriaPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            int y = 10;

            var lblTitle = new Label
            {
                Text = "Search Criteria",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            criteriaPanel.Controls.Add(lblTitle);

            // Botón para abrir logs
            btnOpenLog = new Button
            {
                Text = "View Logs",
                Location = new Point(criteriaPanel.Width - 120, y - 5),
                Width = 100,
                Height = 25,
                Anchor = AnchorStyles.Right,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            btnOpenLog.Click += (s, e) => _logger.OpenLogFile();
            criteriaPanel.Controls.Add(btnOpenLog);

            y += 30;

            // Fuente de datos
            var lblSource = new Label { Text = "Data Source:", Location = new Point(10, y), Width = 120 };
            criteriaPanel.Controls.Add(lblSource);

            cboSource = new ComboBox
            {
                Location = new Point(140, y - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboSource.Items.AddRange(new object[] { "Base de Datos Local", "DigiKey API", "Ambas opciones" });
            cboSource.SelectedIndex = 1;
            cboSource.SelectedIndexChanged += (s, e) =>
            {
                _logger.Debug($"Data source changed to: {cboSource.SelectedItem}", "SearchDialog");
            };
            criteriaPanel.Controls.Add(cboSource);

            var lblSourceInfo = new Label
            {
                Text = "(DigiKey requiere conexion a internet)",
                Location = new Point(350, y),
                Width = 250,
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8)
            };
            criteriaPanel.Controls.Add(lblSourceInfo);

            y += 35;

            var lblType = new Label { Text = "Component Type:", Location = new Point(10, y), Width = 120 };
            criteriaPanel.Controls.Add(lblType);

            cboType = new ComboBox
            {
                Location = new Point(140, y - 3),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboType.Items.AddRange(new object[] { "All", "MOSFET", "Inductor", "Capacitor", "Diode", "IC", "Resistor" });
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
            criteriaPanel.Controls.Add(cboType);

            var lblMfg = new Label { Text = "Manufacturer:", Location = new Point(320, y), Width = 100 };
            criteriaPanel.Controls.Add(lblMfg);

            txtManufacturer = new TextBox { Location = new Point(430, y - 3), Width = 150 };
            criteriaPanel.Controls.Add(txtManufacturer);

            var lblPN = new Label { Text = "Part Number:", Location = new Point(610, y), Width = 100 };
            criteriaPanel.Controls.Add(lblPN);

            txtPartNumber = new TextBox { Location = new Point(720, y - 3), Width = 150 };
            criteriaPanel.Controls.Add(txtPartNumber);

            y += 35;

            AddElectricalSpecFields(criteriaPanel, ref y);

            var lblPrice = new Label { Text = "Max Price ($):", Location = new Point(10, y), Width = 120 };
            criteriaPanel.Controls.Add(lblPrice);

            txtMaxPrice = new TextBox { Location = new Point(140, y - 3), Width = 100 };
            criteriaPanel.Controls.Add(txtMaxPrice);

            y += 35;

            chkInStockOnly = new CheckBox
            {
                Text = "In Stock Only",
                Location = new Point(10, y),
                Width = 150
            };
            criteriaPanel.Controls.Add(chkInStockOnly);

            chkSortByPrice = new CheckBox
            {
                Text = "Sort by Price",
                Location = new Point(180, y),
                Width = 150,
                Checked = true
            };
            criteriaPanel.Controls.Add(chkSortByPrice);

            btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(720, y - 5),
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;
            criteriaPanel.Controls.Add(btnSearch);

            var resultsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            lblResults = new Label
            {
                Text = "Search results will appear here",
                Location = new Point(10, 5),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            resultsPanel.Controls.Add(lblResults);

            progressBar = new ProgressBar
            {
                Location = new Point(10, 25),
                Width = resultsPanel.Width - 20,
                Height = 20,
                Visible = false,
                Style = ProgressBarStyle.Marquee,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            resultsPanel.Controls.Add(progressBar);

            gridResults = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(resultsPanel.Width - 20, resultsPanel.Height - 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            gridResults.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) BtnSelect_Click(s, e); };
            resultsPanel.Controls.Add(gridResults);

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            btnSelect = new Button
            {
                Text = "Select Component",
                Location = new Point(bottomPanel.Width - 280, 10),
                Width = 150,
                Height = 30,
                Anchor = AnchorStyles.Right,
                Enabled = false
            };
            btnSelect.Click += BtnSelect_Click;
            bottomPanel.Controls.Add(btnSelect);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(bottomPanel.Width - 120, 10),
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };
            bottomPanel.Controls.Add(btnCancel);

            gridResults.SelectionChanged += (s, e) => btnSelect.Enabled = gridResults.SelectedRows.Count > 0;

            this.Controls.Add(resultsPanel);
            this.Controls.Add(criteriaPanel);
            this.Controls.Add(bottomPanel);
        }

        private void AddElectricalSpecFields(Panel panel, ref int y)
        {
            var lblMinV = new Label { Text = "Min Voltage (V):", Location = new Point(10, y), Width = 120 };
            panel.Controls.Add(lblMinV);

            txtMinVoltage = new TextBox { Location = new Point(140, y - 3), Width = 100 };
            panel.Controls.Add(txtMinVoltage);

            var lblMinI = new Label { Text = "Min Current (A):", Location = new Point(260, y), Width = 120 };
            panel.Controls.Add(lblMinI);

            txtMinCurrent = new TextBox { Location = new Point(390, y - 3), Width = 100 };
            panel.Controls.Add(txtMinCurrent);

            var lblRDSon = new Label { Text = "Max RDSon (mΩ):", Location = new Point(510, y), Width = 120 };
            panel.Controls.Add(lblRDSon);

            txtMaxRDSon = new TextBox { Location = new Point(640, y - 3), Width = 100 };
            panel.Controls.Add(txtMaxRDSon);

            y += 35;

            var lblL = new Label { Text = "Min Inductance (µH):", Location = new Point(10, y), Width = 120 };
            panel.Controls.Add(lblL);

            txtMinInductance = new TextBox { Location = new Point(140, y - 3), Width = 100 };
            panel.Controls.Add(txtMinInductance);

            var lblC = new Label { Text = "Min Capacitance (µF):", Location = new Point(260, y), Width = 130 };
            panel.Controls.Add(lblC);

            txtMinCapacitance = new TextBox { Location = new Point(390, y - 3), Width = 100 };
            panel.Controls.Add(txtMinCapacitance);

            y += 35;
        }

        private void CboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _logger.Debug($"Component type changed to: {cboType.SelectedItem}", "SearchDialog");
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            _logger.LogDivider();
            _logger.Info("=== SEARCH BUTTON CLICKED ===", "SearchDialog");

            btnSearch.Enabled = false;
            progressBar.Visible = true;
            lblResults.Text = "Searching...";

            try
            {
                var criteria = BuildSearchCriteria();
                _logger.Info($"Search criteria built. Source: {cboSource.SelectedItem}", "SearchDialog");

                var results = await PerformSearchAsync(criteria);

                _logger.Info($"Search completed. Total results: {results.Count}", "SearchDialog");
                DisplayResults(results);
            }
            catch (Exception ex)
            {
                _logger.Error("Search failed with exception", "SearchDialog", ex);
                MessageBox.Show($"Error during search: {ex.Message}\n\nCheck logs for details.", "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblResults.Text = "Search failed - check logs";
            }
            finally
            {
                btnSearch.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private ComponentSearchCriteria BuildSearchCriteria()
        {
            var criteria = new ComponentSearchCriteria
            {
                SortByPrice = chkSortByPrice.Checked,
                InStockOnly = chkInStockOnly.Checked
            };

            if (cboType.SelectedIndex > 0)
            {
                criteria.Type = cboType.SelectedItem.ToString();
                _logger.Debug($"Criteria.Type = {criteria.Type}", "SearchDialog");
            }

            if (!string.IsNullOrWhiteSpace(txtManufacturer.Text))
            {
                criteria.Manufacturer = txtManufacturer.Text;
                _logger.Debug($"Criteria.Manufacturer = {criteria.Manufacturer}", "SearchDialog");
            }

            if (!string.IsNullOrWhiteSpace(txtPartNumber.Text))
            {
                criteria.PartNumber = txtPartNumber.Text;
                _logger.Debug($"Criteria.PartNumber = {criteria.PartNumber}", "SearchDialog");
            }

            if (double.TryParse(txtMinVoltage.Text, out double minV))
            {
                criteria.MinVoltage = minV;
                _logger.Debug($"Criteria.MinVoltage = {minV}V", "SearchDialog");
            }

            if (double.TryParse(txtMinCurrent.Text, out double minI))
            {
                criteria.MinCurrent = minI;
                _logger.Debug($"Criteria.MinCurrent = {minI}A", "SearchDialog");
            }

            if (double.TryParse(txtMaxRDSon.Text, out double maxR))
            {
                criteria.MaxRDSon = maxR / 1000;
                _logger.Debug($"Criteria.MaxRDSon = {maxR}mΩ ({criteria.MaxRDSon}Ω)", "SearchDialog");
            }

            if (double.TryParse(txtMinInductance.Text, out double minL))
            {
                criteria.MinInductance = minL * 1e-6;
                _logger.Debug($"Criteria.MinInductance = {minL}µH", "SearchDialog");
            }

            if (double.TryParse(txtMinCapacitance.Text, out double minC))
            {
                criteria.MinCapacitance = minC * 1e-6;
                _logger.Debug($"Criteria.MinCapacitance = {minC}µF", "SearchDialog");
            }

            if (double.TryParse(txtMaxPrice.Text, out double maxP))
            {
                criteria.MaxPrice = maxP;
                _logger.Debug($"Criteria.MaxPrice = ${maxP}", "SearchDialog");
            }

            return criteria;
        }

        private async Task<System.Collections.Generic.List<Database.ElectronicComponent>> PerformSearchAsync(ComponentSearchCriteria criteria)
        {
            var selectedSource = cboSource.SelectedItem.ToString();
            var allResults = new System.Collections.Generic.List<Database.ElectronicComponent>();

            _logger.Info($"Starting search with source: {selectedSource}", "SearchDialog");

            // Buscar en base de datos local
            if (selectedSource == "Local Database" || selectedSource == "Both")
            {
                _logger.Info("Searching local database...", "SearchDialog");
                try
                {
                    var localResults = database.SearchComponents(criteria);
                    _logger.Info($"Local database returned {localResults.Count} results", "SearchDialog");
                    allResults.AddRange(localResults);
                }
                catch (Exception ex)
                {
                    _logger.Error("Local database search failed", "SearchDialog", ex);
                }
            }

            // Buscar en DigiKey API
            if (selectedSource == "DigiKey API" || selectedSource == "Both")
            {
                _logger.Info("Searching DigiKey API...", "SearchDialog");
                try
                {
                    var digikeyResults = await scraperService.SearchDigiKeyAsync(criteria);
                    _logger.Info($"DigiKey API returned {digikeyResults.Count} results", "SearchDialog");
                    allResults.AddRange(digikeyResults);
                }
                catch (Exception ex)
                {
                    _logger.Error("DigiKey API search failed", "SearchDialog", ex);
                    MessageBox.Show($"DigiKey search failed: {ex.Message}\n\nShowing local results only.\n\nCheck logs for details.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            _logger.Info($"Total results before deduplication: {allResults.Count}", "SearchDialog");

            // Eliminar duplicados por PartNumber
            allResults = allResults
                .GroupBy(c => c.PartNumber)
                .Select(g => g.First())
                .ToList();

            _logger.Info($"Results after deduplication: {allResults.Count}", "SearchDialog");

            // Aplicar ordenamiento
            if (criteria.SortByPrice)
            {
                allResults = allResults.OrderBy(c => c.UnitPrice).ToList();
                _logger.Debug("Results sorted by price", "SearchDialog");
            }

            return allResults;
        }

        private void DisplayResults(System.Collections.Generic.List<Database.ElectronicComponent> results)
        {
            _logger.Info($"Displaying {results.Count} results in grid", "SearchDialog");

            gridResults.DataSource = null;
            gridResults.Columns.Clear();

            if (results.Count == 0)
            {
                lblResults.Text = "No components found matching criteria";
                _logger.Warning("No results to display", "SearchDialog");
                return;
            }

            lblResults.Text = $"Found {results.Count} component(s)";

            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Source", HeaderText = "Source", Width = 80 });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", Width = 80 });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Manufacturer", HeaderText = "Manufacturer", Width = 120 });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "PartNumber", HeaderText = "Part Number", Width = 150 });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description" });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Price", Width = 80 });
            gridResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", Width = 70 });

            foreach (var comp in results)
            {
                gridResults.Rows.Add(
                    comp.Supplier ?? "Local",
                    comp.Type,
                    comp.Manufacturer,
                    comp.PartNumber,
                    comp.Description,
                    $"${comp.UnitPrice:F2}",
                    comp.Stock
                );
            }

            _logger.Info("Results displayed successfully in grid", "SearchDialog");
        }

        // ✅ LÍNEAS CRÍTICAS PARA SELECCIÓN DE COMPONENTE
        // Estas líneas aseguran que el componente seleccionado se devuelva correctamente al formulario padre.
        // ============================================================================
        // VERSIÓN CORREGIDA - BtnSelect_Click
        // Maneja columnas que pueden no existir en el DataGridView
        // Reemplazar en ComponentSearchDialog.cs
        // ============================================================================

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (gridResults.SelectedRows.Count == 0)
            {
                _logger.Warning("Select clicked but no row selected", "SearchDialog");
                return;
            }

            var row = gridResults.SelectedRows[0];

            // ✅ MÉTODO 1: Intentar obtener desde DataBoundItem (MEJOR OPCIÓN)
            if (row.DataBoundItem is ElectronicComponent component)
            {
                SelectedComponent = component;

                _logger.Info($"Component selected from DataBoundItem: {component.Manufacturer} {component.PartNumber}", "SearchDialog");
                _logger.Info($"  Type: {component.Type}", "SearchDialog");
                _logger.Info($"  Description: {component.Description}", "SearchDialog");

                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // ✅ MÉTODO 2: Crear desde las celdas (FALLBACK)
            _logger.Warning("DataBoundItem not available, creating component from cells", "SearchDialog");

            // Helper para obtener valor de celda de forma segura
            string GetCellValue(string columnName)
            {
                try
                {
                    // Verificar si la columna existe
                    if (gridResults.Columns.Contains(columnName))
                    {
                        return row.Cells[columnName].Value?.ToString() ?? "";
                    }
                    else
                    {
                        _logger.Debug($"Column '{columnName}' not found in grid", "SearchDialog");
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error getting cell value for column '{columnName}': {ex.Message}", "SearchDialog");
                    return "";
                }
            }

            decimal GetCellDecimal(string columnName)
            {
                string value = GetCellValue(columnName).Replace("$", "").Replace(",", ".");
                return decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal result) ? result : 0;
            }

            int GetCellInt(string columnName)
            {
                string value = GetCellValue(columnName);
                return int.TryParse(value, out int result) ? result : 0;
            }

            // Crear el componente desde las celdas
            SelectedComponent = new ElectronicComponent
            {
                PartNumber = GetCellValue("PartNumber"),
                Manufacturer = GetCellValue("Manufacturer"),
                Type = GetCellValue("Type"),
                Description = GetCellValue("Description"),
                UnitPrice = (double)GetCellDecimal("Price"),
                Stock = GetCellInt("Stock"),
                Supplier = GetCellValue("Supplier"),  // Ahora maneja si no existe
                DatasheetURL = GetCellValue("Datasheet")
            };

            // Intentar obtener Specifications si existe una columna para ello
            // (Probablemente no, pero verificamos)
            var specsValue = GetCellValue("Specifications");
            if (!string.IsNullOrEmpty(specsValue))
            {
                // Si las specs están serializadas como JSON, deserializar
                try
                {
                    SelectedComponent.Specifications =
                        System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(specsValue);
                }
                catch
                {
                    // Si falla, crear diccionario vacío
                    SelectedComponent.Specifications = new Dictionary<string, string>();
                }
            }
            else
            {
                // Inicializar diccionario vacío
                SelectedComponent.Specifications = new Dictionary<string, string>();
            }

            _logger.Info($"Component created from grid cells: {SelectedComponent.Manufacturer} {SelectedComponent.PartNumber}", "SearchDialog");
            _logger.Info($"  Type: {SelectedComponent.Type}", "SearchDialog");
            _logger.Info($"  Price: ${SelectedComponent.UnitPrice}", "SearchDialog");
            _logger.Info($"  Stock: {SelectedComponent.Stock}", "SearchDialog");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ============================================================================
        // MÉTODO ALTERNATIVO SI EL ANTERIOR NO FUNCIONA
        // Este método usa índices de columna en lugar de nombres
        // ============================================================================

        private void BtnSelect_Click_AlternativeVersion(object sender, EventArgs e)
        {
            if (gridResults.SelectedRows.Count == 0)
            {
                _logger.Warning("Select clicked but no row selected", "SearchDialog");
                return;
            }

            var row = gridResults.SelectedRows[0];

            // ✅ Intentar DataBoundItem primero
            if (row.DataBoundItem is ElectronicComponent component)
            {
                SelectedComponent = component;
                _logger.Info($"Component selected: {component.PartNumber}", "SearchDialog");
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // ✅ Usar índices de columna en lugar de nombres
            try
            {
                SelectedComponent = new ElectronicComponent
                {
                    // Ajustar estos índices según el orden real de tus columnas
                    PartNumber = row.Cells[0].Value?.ToString() ?? "",      // Columna 0
                    Manufacturer = row.Cells[1].Value?.ToString() ?? "",    // Columna 1
                    Type = row.Cells[2].Value?.ToString() ?? "",            // Columna 2
                    Description = row.Cells[3].Value?.ToString() ?? "",     // Columna 3
                    UnitPrice = (double)(decimal.TryParse(row.Cells[4].Value?.ToString()?.Replace("$", ""),
                        out decimal price) ? price : 0),                     // Columna 4
                    Stock = int.TryParse(
                        row.Cells[5].Value?.ToString(),
                        out int stock) ? stock : 0,                         // Columna 5
                    Specifications = new Dictionary<string, string>()
                };

                // Supplier y Datasheet pueden estar en columnas adicionales
                if (row.Cells.Count > 6)
                    SelectedComponent.Supplier = row.Cells[6].Value?.ToString() ?? "";
                if (row.Cells.Count > 7)
                    SelectedComponent.DatasheetURL = row.Cells[7].Value?.ToString() ?? "";

                _logger.Info($"Component created from cells by index: {SelectedComponent.PartNumber}", "SearchDialog");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating component from cells: {ex.Message}", "SearchDialog", ex);
                MessageBox.Show(
                    $"Error selecting component:\n\n{ex.Message}\n\nPlease try again.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        // ============================================================================
        // VERIFICACIÓN DE COLUMNAS (MÉTODO DE DIAGNÓSTICO)
        // Ejecutar esto para ver qué columnas tienes realmente
        // ============================================================================

        private void DiagnoseGridColumns()
        {
            _logger.Info("=== GRID COLUMNS DIAGNOSTIC ===", "SearchDialog");
            _logger.Info($"Total columns: {gridResults.Columns.Count}", "SearchDialog");

            for (int i = 0; i < gridResults.Columns.Count; i++)
            {
                var col = gridResults.Columns[i];
                _logger.Info($"Column {i}: Name='{col.Name}', HeaderText='{col.HeaderText}'", "SearchDialog");
            }

            _logger.Info("=== END DIAGNOSTIC ===", "SearchDialog");
        }

        // Llamar este método al final de DisplayResults() para ver las columnas:
        // DiagnoseGridColumns();
    }
}





















