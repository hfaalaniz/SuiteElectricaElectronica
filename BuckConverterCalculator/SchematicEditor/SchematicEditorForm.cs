using BuckConverterCalculator.Analysis;
using BuckConverterCalculator.BOM;
using BuckConverterCalculator.Core;  // ✅ CORRECTO
using BuckConverterCalculator.Database;
using BuckConverterCalculator.PCB;
using BuckConverterCalculator.SchematicEditor;
using BuckConverterCalculator.Services;
using BuckConverterCalculator.Simulation;
using BuckConverterCalculator.UI.Controls;
using BuckConverterCalculator.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static LabelComponent;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Editor completo de esquemáticos con todas las funcionalidades avanzadas
    /// </summary>
    public class SchematicEditorForm : Form
    {
        // ============================================================================
        // VARIABLES DE INSTANCIA - Agregar a la clase SchematicEditorForm
        // ============================================================================
        private int componentPlacementOffset = 0;
        private SchematicComponent placementComponent = null;
        private bool placementMode = false;
        private Point lastPlacementPosition = new Point(100, 100);
        // Agregar estas variables privadas en la parte superior de la clase SchematicEditorForm:
        private ComponentDatabase componentDatabase;
        private BOMGenerator bomGenerator;
        private BOMExporter bomExporter;
        private BodeAnalyzer bodeAnalyzer;
        private WaveformSimulator waveformSimulator;
        private PCBLayoutGenerator pcbGenerator;

        // Diálogos
        private DCMAnalysisDialog dcmDialog;
        private BodePlotDialog bodeDialog;
        private WaveformSimulationDialog waveformDialog;
        private ComponentDatabaseDialog databaseDialog;
        private PCBLayoutDialog pcbDialog;

        // Nuevos componentes
        private DCMAnalyzer dcmAnalyzer;
        // Diálogos
        // Declaraciones originales
        private SchematicDocument document;
        private DesignParameters parameters;
        private CalculationResults results;

        // UI Components
        private Panel canvasPanel;
        private PictureBox canvas;
        private PropertyGrid propertyGrid;
        private ToolStrip toolStrip;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private Panel componentLibraryPanel;
        private ListBox libraryListBox;

        // Component ToolBox (nueva barra acoplable)
        private ComponentToolBox componentToolBox;

        // Grid settings
        private int gridSize = 50;
        private bool snapToGrid = true;
        private bool showGrid = true;

        // Zoom and pan
        private float zoomFactor = 0.75f;
        private bool isPanning = false;
        private Point panStartPoint;
        private Point scrollStartPoint;

        // Selection and dragging
        private bool isDragging = false;
        private Point dragStartPoint;
        private SchematicComponent hoveredComponent = null;
        private Rectangle selectionRectangle;
        private bool isSelectingRectangle = false;

        // Copy/Paste
        private List<SchematicComponent> clipboard = new List<SchematicComponent>();

        // Library drag and drop
        private bool isDraggingFromLibrary = false;
        private SchematicComponent libraryDragComponent = null;

        // Wire routing
        private bool isWiring = false;
        private Point wireStartPoint;
        private Point wireCurrentPoint;
        private ComponentPin wireStartPin = null;
        private List<Point> wirePoints = new List<Point>();
        private WireRoutingMode wireRoutingMode = WireRoutingMode.Orthogonal;

        // Tool mode
        private EditorMode currentMode = EditorMode.Select;

        public enum EditorMode
        {
            Select,
            AddResistor,
            AddCapacitor,
            AddInductor,
            AddWire,
            AddLabel,
            AddTerminal
        }

        public enum WireRoutingMode
        {
            Direct,      // Línea recta
            Orthogonal,  // 90 grados (Manhattan)
            Auto         // Automático con evitación
        }

        public SchematicEditorForm(DesignParameters param, CalculationResults res)
        {
            parameters = param;
            results = res;
            document = new SchematicDocument
            {
                Name = "Buck Converter Schematic",
                Description = $"Vin: {param.InputVoltageMax}V → Vout: {param.OutputVoltage}V @ {param.OutputCurrent}A"
            };

            InitializeComponent();
            CreateComponentToolBox();
            PopulateFromCalculation();
            PopulateComponentLibrary();
            InitializeAdvancedFeatures(); // Inicializar nuevas funcionalidades
        }

        // Se añaden las siguientes partes de edicion----------------------------------------------------------------
        private void InitializeAdvancedFeatures()
        {
            // Inicializar analizadores
            componentDatabase = new ComponentDatabase();
            bomGenerator = new BOMGenerator(document, componentDatabase);

            // Crear menús
            CreateAnalysisMenu();
            CreateComponentsMenu();
            CreatePCBMenu();
            UpdateExportMenu();
        }

        private void CreateAnalysisMenu()
        {
            var analysisMenu = new ToolStripMenuItem("Analysis");

            var dcmItem = new ToolStripMenuItem("DCM Analysis", null, OnDCMAnalysis);
            dcmItem.ShortcutKeys = Keys.Control | Keys.D;

            var bodeItem = new ToolStripMenuItem("Bode Plot", null, OnBodePlot);
            bodeItem.ShortcutKeys = Keys.Control | Keys.B;

            var waveformItem = new ToolStripMenuItem("Waveform Simulation", null, OnWaveformSimulation);
            waveformItem.ShortcutKeys = Keys.Control | Keys.W;
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            var efficiencyItem = new ToolStripMenuItem("Calculate Efficiency", null, OnCalculateEfficiency);

            analysisMenu.DropDownItems.Add(dcmItem);
            analysisMenu.DropDownItems.Add(bodeItem);
            analysisMenu.DropDownItems.Add(waveformItem);
            analysisMenu.DropDownItems.Add(new ToolStripSeparator());
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            analysisMenu.DropDownItems.Add(efficiencyItem);

            menuStrip.Items.Insert(3, analysisMenu); // Después de Edit
        }

        private void CreateComponentsMenu()
        {
            var componentsMenu = new ToolStripMenuItem("Components");

            var databaseItem = new ToolStripMenuItem("Component Database", null, OnComponentDatabase);
            databaseItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.D;

            // ✅ VERSIÓN CORREGIDA CON DEBUG
            var searchMenuItem = new ToolStripMenuItem("Search Components...");
            searchMenuItem.Click += OnSearchComponent;  // ← CRÍTICO
            componentsMenu.DropDownItems.Add(searchMenuItem);

            // ✅ AGREGAR LOG PARA VERIFICAR QUE EL MENÚ SE CREA
            LoggerService.Instance.Info("Search Components menu item created and event connected", "SchematicEditor");

            var addLibraryItem = new ToolStripMenuItem("Add to Library", null, OnAddToLibrary);
            var updatePricesItem = new ToolStripMenuItem("Update Prices", null, OnUpdatePrices);

            componentsMenu.DropDownItems.Add(databaseItem);
            componentsMenu.DropDownItems.Add(searchMenuItem);  // ✅ ASEGURAR QUE SE AGREGA
            componentsMenu.DropDownItems.Add(new ToolStripSeparator());
            componentsMenu.DropDownItems.Add(addLibraryItem);
            componentsMenu.DropDownItems.Add(updatePricesItem);

            menuStrip.Items.Insert(4, componentsMenu);

            // ✅ LOG FINAL
            LoggerService.Instance.Info("Components menu created successfully", "SchematicEditor");
        }

        private void CreatePCBMenu()
        {
            var pcbMenu = new ToolStripMenuItem("PCB");

            var generateItem = new ToolStripMenuItem("Generate Layout", null, OnGeneratePCB);
            generateItem.ShortcutKeys = Keys.Control | Keys.P;

            var exportKicadItem = new ToolStripMenuItem("Export to KiCad", null, OnExportToKiCad);
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            var exportGerberItem = new ToolStripMenuItem("Export to Gerber", null, OnExportToGerber);
            var settingsItem = new ToolStripMenuItem("PCB Settings", null, OnPCBSettings);

            pcbMenu.DropDownItems.Add(generateItem);
            pcbMenu.DropDownItems.Add(new ToolStripSeparator());
            pcbMenu.DropDownItems.Add(exportKicadItem);
            pcbMenu.DropDownItems.Add(exportGerberItem);
            pcbMenu.DropDownItems.Add(new ToolStripSeparator());
            pcbMenu.DropDownItems.Add(settingsItem);

            menuStrip.Items.Insert(5, pcbMenu);
        }

        private void UpdateExportMenu()
        {
            // CORREGIDO: Buscar el menú File y luego Export
            ToolStripMenuItem fileMenu = null;
            ToolStripMenuItem exportMenu = null;

            foreach (ToolStripItem item in menuStrip.Items)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Text == "File")
                {
                    fileMenu = menuItem;
                    break;
                }
            }

            if (fileMenu != null)
            {
                foreach (ToolStripItem item in fileMenu.DropDownItems)
                {
                    if (item is ToolStripMenuItem menuItem && menuItem.Text.Contains("Export"))
                    {
                        exportMenu = menuItem;
                        break;
                    }
                }
            }

            if (exportMenu != null)
            {
                exportMenu.DropDownItems.Add(new ToolStripSeparator());
                exportMenu.DropDownItems.Add(new ToolStripMenuItem("Export BOM (CSV)", null, OnExportBOMCSV));
                exportMenu.DropDownItems.Add(new ToolStripMenuItem("Export BOM (Excel)", null, OnExportBOMExcel));
                exportMenu.DropDownItems.Add(new ToolStripMenuItem("Export BOM (PDF)", null, OnExportBOMPDF));
            }
        }

        // Metodos faltantes para los menús
        private void OnCalculateEfficiency(object sender, EventArgs e)
        {
            var parameters = ExtractCircuitParameters();
            var analyzer = new DCMAnalyzer
            {
                InputVoltage = parameters.InputVoltage,
                OutputVoltage = 13, // Asumiendo 13V de salida
                OutputCurrent = 5,  // Asumiendo 5A
                SwitchingFrequency = parameters.SwitchingFrequency,
                Inductance = parameters.Inductance,
                Capacitance = parameters.Capacitance
            };

            double efficiency = analyzer.CalculateEfficiency(
                parameters.SwitchRDSon,
                parameters.DiodeVF,
                parameters.InductorESR
            );

            MessageBox.Show(
                $"Estimated Efficiency: {efficiency:F2}%\n\n" +
                $"Input: {parameters.InputVoltage}V\n" +
                $"Output: 13V @ 5A",
                "Efficiency Calculation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnAddToLibrary(object sender, EventArgs e)
        {
            MessageBox.Show("Add to Library feature is not yet implemented.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnUpdatePrices(object sender, EventArgs e)
        {
            MessageBox.Show("Update Prices feature is not yet implemented.\n" +
                "This would connect to DigiKey/Mouser API to update component prices.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnExportToGerber(object sender, EventArgs e)
        {
            MessageBox.Show("Gerber export is not yet implemented.\n" +
                "Use 'Export to KiCad' and generate Gerbers from KiCad.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnPCBSettings(object sender, EventArgs e)
        {
            MessageBox.Show("PCB Settings dialog is not yet implemented.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnExportBOMPDF(object sender, EventArgs e)
        {
            MessageBox.Show("PDF export is not yet implemented.\n" +
                "Use Excel export and save as PDF from Excel.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // MÉTODO PRINCIPAL: AddComponentFromDatabase
        // ============================================================================
        // VERSIÓN CON LOGGING EXHAUSTIVO - AddComponentFromDatabase
        // Reemplazar el método completo en SchematicEditorForm.cs
        // ============================================================================

        private void AddComponentFromDatabase(Database.ElectronicComponent component)
        {
            // ✅ LOG 1: Inicio
            LoggerService.Instance.Info("╔════════════════════════════════════════════════════════════════╗", "SchematicEditor");
            LoggerService.Instance.Info("║          COMPONENT DATA FROM DIGIKEY - FULL DIAGNOSTIC         ║", "SchematicEditor");
            LoggerService.Instance.Info("╚════════════════════════════════════════════════════════════════╝", "SchematicEditor");

            LoggerService.Instance.Info($"PartNumber:    '{component.PartNumber}'", "SchematicEditor");
            LoggerService.Instance.Info($"Type:          '{component.Type}'", "SchematicEditor");
            LoggerService.Instance.Info($"Manufacturer:  '{component.Manufacturer}'", "SchematicEditor");
            LoggerService.Instance.Info($"Description:   '{component.Description}'", "SchematicEditor");
            LoggerService.Instance.Info($"Supplier:      '{component.Supplier}'", "SchematicEditor");
            //LoggerService.Instance.Info($"Package:       '{component.Package}'", "SchematicEditor");
            LoggerService.Instance.Info($"Price:         ${component.UnitPrice}", "SchematicEditor");
            LoggerService.Instance.Info($"Stock:         {component.Stock}", "SchematicEditor");

            LoggerService.Instance.Info("", "SchematicEditor");
            LoggerService.Instance.Info("--- SPECIFICATIONS ---", "SchematicEditor");

            if (component.Specifications != null && component.Specifications.Count > 0)
            {
                LoggerService.Instance.Info($"Total specifications: {component.Specifications.Count}", "SchematicEditor");

                foreach (var spec in component.Specifications.OrderBy(s => s.Key))
                {
                    LoggerService.Instance.Info($"  [{spec.Key}] = '{spec.Value}'", "SchematicEditor");
                }
            }
            else
            {
                LoggerService.Instance.Warning("⚠️ NO SPECIFICATIONS AVAILABLE!", "SchematicEditor");
            }

            LoggerService.Instance.Info("════════════════════════════════════════════════════════════════", "SchematicEditor");


            if (component == null)
            {
                LoggerService.Instance.Error("Component is NULL!", "SchematicEditor");
                MessageBox.Show("Component is null.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ✅ LOG 2: Componente recibido
            LoggerService.Instance.Info($"Component received: Type={component.Type}, PartNumber={component.PartNumber}", "SchematicEditor");

            try
            {
                statusLabel.Text = $"Processing {component.Type} - {component.PartNumber}...";

                // ✅ LOG 3: Intentando crear componente esquemático
                LoggerService.Instance.Info("Calling CreateSchematicComponentFromDatabase...", "SchematicEditor");

                SchematicComponent schematicComponent = CreateSchematicComponentFromDatabase(component);

                // ✅ LOG 4: Verificar resultado
                if (schematicComponent == null)
                {
                    LoggerService.Instance.Error("CreateSchematicComponentFromDatabase returned NULL", "SchematicEditor");
                    statusLabel.Text = "Failed to create component";
                    return;
                }

                LoggerService.Instance.Info($"Schematic component created: {schematicComponent.GetType().Name}", "SchematicEditor");

                // ✅ LOG 5: Obtener posición
                LoggerService.Instance.Info("Getting position...", "SchematicEditor");
                Point position = GetNextComponentPosition();
                LoggerService.Instance.Info($"Position obtained: ({position.X}, {position.Y})", "SchematicEditor");

                schematicComponent.X = position.X;
                schematicComponent.Y = position.Y;

                // ✅ LOG 6: Generar designator
                LoggerService.Instance.Info("Generating unique designator...", "SchematicEditor");
                string designator = GenerateUniqueDesignator(schematicComponent.Type);
                LoggerService.Instance.Info($"Designator generated: {designator}", "SchematicEditor");

                schematicComponent.Name = designator;

                // ✅ LOG 7: Actualizar bounds
                LoggerService.Instance.Info("Updating bounds...", "SchematicEditor");
                schematicComponent.Move(0, 0);
                LoggerService.Instance.Info($"Bounds updated: {schematicComponent.Bounds}", "SchematicEditor");

                // ✅ LOG 8: Verificar documento
                if (document == null)
                {
                    LoggerService.Instance.Error("document is NULL!", "SchematicEditor");
                    MessageBox.Show("Document is null!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (document.Components == null)
                {
                    LoggerService.Instance.Error("document.Components is NULL!", "SchematicEditor");
                    MessageBox.Show("Document.Components is null!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int countBefore = document.Components.Count;
                LoggerService.Instance.Info($"Components count BEFORE add: {countBefore}", "SchematicEditor");

                // ✅ LOG 9: Agregar al documento
                LoggerService.Instance.Info("Adding component to document...", "SchematicEditor");
                document.Components.Add(schematicComponent);

                int countAfter = document.Components.Count;
                LoggerService.Instance.Info($"Components count AFTER add: {countAfter}", "SchematicEditor");

                if (countAfter != countBefore + 1)
                {
                    LoggerService.Instance.Error($"Component count mismatch! Before={countBefore}, After={countAfter}", "SchematicEditor");
                    MessageBox.Show(
                        $"Warning: Component may not have been added correctly.\n" +
                        $"Before: {countBefore}, After: {countAfter}",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                // ✅ LOG 10: Refrescar vistas
                LoggerService.Instance.Info("Refreshing canvas...", "SchematicEditor");

                if (canvas == null)
                {
                    LoggerService.Instance.Error("canvas is NULL!", "SchematicEditor");
                }
                else
                {
                    LoggerService.Instance.Info("Calling canvas.Invalidate()...", "SchematicEditor");
                    canvas.Invalidate();
                    LoggerService.Instance.Info("canvas.Invalidate() completed", "SchematicEditor");
                }

                if (canvasPanel == null)
                {
                    LoggerService.Instance.Error("canvasPanel is NULL!", "SchematicEditor");
                }
                else
                {
                    LoggerService.Instance.Info("Calling canvasPanel.Invalidate()...", "SchematicEditor");
                    canvasPanel.Invalidate();
                    LoggerService.Instance.Info("canvasPanel.Invalidate() completed", "SchematicEditor");
                }

                LoggerService.Instance.Info("Calling this.Refresh()...", "SchematicEditor");
                this.Refresh();
                LoggerService.Instance.Info("this.Refresh() completed", "SchematicEditor");

                // ✅ LOG 11: Seleccionar componente
                LoggerService.Instance.Info("Clearing selection...", "SchematicEditor");
                document.ClearSelection();

                LoggerService.Instance.Info("Selecting new component...", "SchematicEditor");
                schematicComponent.IsSelected = true;

                LoggerService.Instance.Info("Updating PropertyGrid...", "SchematicEditor");
                propertyGrid.SelectedObject = schematicComponent;

                // ✅ LOG 12: Mensaje de éxito
                statusLabel.Text = $"Added {designator} ({component.PartNumber}) at ({position.X}, {position.Y})";
                LoggerService.Instance.Info($"Component successfully added: {designator}", "SchematicEditor");

                // ✅ LOG 13: Mostrar mensaje
                LoggerService.Instance.Info("Showing success message...", "SchematicEditor");
                ShowComponentAddedMessage(schematicComponent, component);

                LoggerService.Instance.Info("=== AddComponentFromDatabase COMPLETED SUCCESSFULLY ===", "SchematicEditor");
            }
            catch (Exception ex)
            {
                LoggerService.Instance.Error($"EXCEPTION in AddComponentFromDatabase: {ex.Message}", "SchematicEditor", ex);
                MessageBox.Show(
                    $"Error adding component:\n\n" +
                    $"Type: {component.Type}\n" +
                    $"PartNumber: {component.PartNumber}\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Stack: {ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                statusLabel.Text = "Error adding component - see logs";
            }
        }



        // ============================================================================
        // MÉTODO: Mostrar mensaje de confirmación
        // ============================================================================
        private void ShowComponentAddedMessage(SchematicComponent schematicComp, Database.ElectronicComponent dbComp)
        {
            string message = $"Component added successfully!\n\n" +
                            $"Designator: {schematicComp.Name}\n" +
                            $"Type: {dbComp.Type}\n" +
                            $"Part Number: {dbComp.PartNumber}\n" +
                            $"Manufacturer: {dbComp.Manufacturer}\n\n" +
                            $"Position: ({schematicComp.X}, {schematicComp.Y})\n\n" +
                            $"The component has been placed on the schematic.\n" +
                            $"You can move it by dragging with the mouse.";

            MessageBox.Show(
                message,
                "Component Added",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ============================================================================
        // MÉTODO: Obtener prefijo de designator según tipo
        // ============================================================================
        private string GetDesignatorPrefix(ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.Mosfet:
                case ComponentType.BJT:
                    return "Q";

                case ComponentType.Inductor:
                    return "L";

                case ComponentType.Capacitor:
                    return "C";

                case ComponentType.Diode:
                case ComponentType.SchottkyDiode:
                case ComponentType.ZenerDiode:
                case ComponentType.BridgeDiode:
                    return "D";

                case ComponentType.Resistor:
                case ComponentType.Potentiometer:
                    return "R";

                case ComponentType.IC:
                case ComponentType.OpAmp:
                    return "U";

                case ComponentType.Fuse:
                    return "F";

                case ComponentType.LED:
                    return "LED";

                case ComponentType.Transformer:
                    return "T";

                case ComponentType.Battery:
                    return "BAT";

                case ComponentType.Buzzer:
                    return "BZ";

                case ComponentType.Terminal:
                    return "J";

                case ComponentType.Ground:
                    return "GND";

                case ComponentType.VccSupply:
                    return "VCC";

                default:
                    return "X";
            }
        }

        // ============================================================================
        // MÉTODO: Generar designator único
        // ============================================================================
        private string GenerateUniqueDesignator(ComponentType componentType)
        {
            string prefix = GetDesignatorPrefix(componentType);
            int number = 1;

            // Buscar el siguiente número disponible
            while (document.Components.Any(c => c.Name == $"{prefix}{number}"))
            {
                number++;
            }

            return $"{prefix}{number}";
        }


        // ============================================================================
        // VERSIÓN CORREGIDA - CreateSchematicComponentFromDatabase
        // Maneja correctamente los tipos de DigiKey/Mouser
        // ============================================================================

        private SchematicComponent CreateSchematicComponentFromDatabase(Database.ElectronicComponent dbComponent)
        {
            // ✅ DIAGNÓSTICO COMPLETO
            LoggerService.Instance.Info("========================================", "SchematicEditor");
            LoggerService.Instance.Info("=== COMPONENT DIAGNOSTIC ===", "SchematicEditor");
            LoggerService.Instance.Info($"PartNumber: '{dbComponent.PartNumber}'", "SchematicEditor");
            LoggerService.Instance.Info($"Type (RAW): '{dbComponent.Type}'", "SchematicEditor");
            LoggerService.Instance.Info($"Manufacturer: '{dbComponent.Manufacturer}'", "SchematicEditor");
            LoggerService.Instance.Info($"Description: '{dbComponent.Description}'", "SchematicEditor");

            if (dbComponent.Specifications != null)
            {
                LoggerService.Instance.Info($"Specifications count: {dbComponent.Specifications.Count}", "SchematicEditor");
                foreach (var spec in dbComponent.Specifications)
                {
                    LoggerService.Instance.Debug($"  Spec[{spec.Key}] = '{spec.Value}'", "SchematicEditor");
                }
            }
            else
            {
                LoggerService.Instance.Warning("Specifications is NULL!", "SchematicEditor");
            }

            LoggerService.Instance.Info("========================================", "SchematicEditor");


            SchematicComponent component = null;

            string GetSpec(string key)
            {
                if (dbComponent.Specifications != null && dbComponent.Specifications.ContainsKey(key))
                    return dbComponent.Specifications[key];
                return null;
            }

            // ✅ NORMALIZAR Y TAMBIÉN ANALIZAR LA DESCRIPCIÓN
            string normalizedType = dbComponent.Type?.ToUpper().Trim() ?? "";
            string description = dbComponent.Description?.ToUpper() ?? "";

            // ✅ DETECTAR TRANSISTOR desde Description si Type no lo indica
            if (description.Contains("TRANSISTOR") || description.Contains("BJT") ||
                description.Contains("NPN") || description.Contains("PNP"))
            {
                normalizedType = "TRANSISTOR";
            }
            // ✅ DETECTAR MOSFET desde Description
            else if (description.Contains("MOSFET") || description.Contains("FET") ||
                     description.Contains("N-CHANNEL") || description.Contains("P-CHANNEL"))
            {
                normalizedType = "MOSFET";
            }
            // ✅ DETECTAR DIODO desde Description
            else if (description.Contains("DIODE") && !description.Contains("LIGHT EMITTING"))
            {
                normalizedType = "DIODE";
            }
            // ✅ DETECTAR OPAMP desde Description
            else if (description.Contains("OPAMP") || description.Contains("OP-AMP") ||
                     description.Contains("OPERATIONAL AMP") || description.Contains("AMPLIFIER"))
            {
                normalizedType = "OPAMP";
            }
            // ✅ DETECTAR REGULATOR desde Description
            else if (description.Contains("REGULATOR") || description.Contains("REG LINEAR") ||
                     description.Contains("LDO") || description.Contains("VOLTAGE REG"))
            {
                normalizedType = "REGULATOR";
            }

            LoggerService.Instance.Info($"Normalized type: '{normalizedType}'", "SchematicEditor");

            int detectedPins = DetectPinCountAdvanced(dbComponent);

            switch (normalizedType)
            {
                // ========== MOSFET ==========
                case "MOSFET":
                case "FET":
                case "N-CHANNEL MOSFET":
                case "P-CHANNEL MOSFET":
                case "N-CHANNEL":
                case "P-CHANNEL":
                    LoggerService.Instance.Info("✅ Creating MosfetComponent...", "SchematicEditor");
                    component = new MosfetComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "Unknown",
                        ChannelType = (description.Contains("P-CHANNEL") || GetSpec("ChannelType")?.Contains("P") == true)
                            ? "P-Channel" : "N-Channel",
                        VDS = GetSpec("VDS") ?? GetSpec("VDSS") ?? GetSpec("Drain to Source Voltage") ?? "100V",
                        ID = GetSpec("ID") ?? GetSpec("Drain Current") ?? GetSpec("Continuous Drain Current") ?? "10A",
                        RDSon = GetSpec("RDSon") ?? GetSpec("RDS(on)") ?? GetSpec("On Resistance") ?? "50mΩ",
                        VGS = GetSpec("VGS") ?? GetSpec("VGSS") ?? "±20V",
                        QG = GetSpec("QG") ?? GetSpec("Gate Charge") ?? "10nC",
                        Package = GetSpec("Package") ?? GetSpec("Mounting Type") ?? "TO-252"
                    };
                    break;

                // ========== BJT TRANSISTOR ==========
                case "TRANSISTOR":
                case "BJT":
                case "NPN":
                case "PNP":
                case "BIPOLAR":
                case "BIPOLAR TRANSISTOR":
                    LoggerService.Instance.Info("✅ Creating BJTComponent...", "SchematicEditor");
                    component = new BJTComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "2N3904",
                        TransistorType = (description.Contains("PNP") || GetSpec("TransistorType")?.Contains("PNP") == true)
                            ? "PNP" : "NPN",
                        VCE = GetSpec("VCE") ?? GetSpec("VCEO") ?? GetSpec("Collector-Emitter Voltage") ?? "40V",
                        IC = GetSpec("IC") ?? GetSpec("Collector Current") ?? "200mA",
                        PowerRating = GetSpec("Power") ?? GetSpec("Power Rating") ?? GetSpec("Power Dissipation") ?? "500mW",
                        HFE = GetSpec("HFE") ?? GetSpec("hFE") ?? GetSpec("DC Current Gain") ?? "100-300",
                        Package = GetSpec("Package") ?? GetSpec("Mounting Type") ?? "TO-92"
                    };
                    break;

                // ========== DIODE ==========
                case "DIODE":
                case "SCHOTTKY":
                case "SCHOTTKY DIODE":
                case "ZENER":
                case "ZENER DIODE":
                case "RECTIFIER":
                case "FAST SWITCHING DIODE":
                    LoggerService.Instance.Info("✅ Creating DiodeComponent...", "SchematicEditor");
                    component = new DiodeComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "1N4148",
                        VoltageRating = GetSpec("VR") ?? GetSpec("Reverse Voltage") ?? GetSpec("VRRM") ?? "100V",
                        CurrentRating = GetSpec("IF") ?? GetSpec("Forward Current") ?? "1A",
                        VF = GetSpec("VF") ?? GetSpec("Forward Voltage") ?? "0.7V",
                        DiodeType = description.Contains("SCHOTTKY") ? "Schottky" :
                                   description.Contains("ZENER") ? "Zener" : "Standard",
                        Package = GetSpec("Package") ?? "DO-41"
                    };
                    break;

                // ========== RESISTOR ==========
                case "RESISTOR":
                case "RES":
                case "THICK FILM RESISTOR":
                case "THIN FILM RESISTOR":
                case "METAL FILM RESISTOR":
                    LoggerService.Instance.Info("✅ Creating ResistorComponent...", "SchematicEditor");
                    component = new ResistorComponent
                    {
                        Value = GetSpec("Resistance") ?? GetSpec("Value") ?? "10kΩ",
                        Power = GetSpec("Power") ?? GetSpec("Power Rating") ?? "1/4W",
                        Tolerance = GetSpec("Tolerance") ?? "1%"
                    };
                    break;

                // ========== CAPACITOR ==========
                case "CAPACITOR":
                case "CAP":
                case "CERAMIC CAPACITOR":
                case "ELECTROLYTIC CAPACITOR":
                case "TANTALUM CAPACITOR":
                case "FILM CAPACITOR":
                    LoggerService.Instance.Info("✅ Creating CapacitorComponent...", "SchematicEditor");
                    component = new CapacitorComponent
                    {
                        Capacitance = GetSpec("Capacitance") ?? GetSpec("Value") ?? "100µF",
                        VoltageRating = GetSpec("Voltage") ?? GetSpec("Voltage Rating") ?? "25V",
                        ESR = GetSpec("ESR") ?? "5mΩ",
                        Tolerance = GetSpec("Tolerance") ?? "±20%",
                        Package = GetSpec("Package") ?? "Radial",
                        Manufacturer = dbComponent.Manufacturer,
                        PartNumber = dbComponent.PartNumber
                    };

                    string capType = normalizedType;
                    if (capType.Contains("ELECTROLYTIC"))
                        ((CapacitorComponent)component).CapacitorType = CapacitorType.Electrolytic;
                    else if (capType.Contains("CERAMIC"))
                        ((CapacitorComponent)component).CapacitorType = CapacitorType.Ceramic;
                    else if (capType.Contains("TANTALUM"))
                        ((CapacitorComponent)component).CapacitorType = CapacitorType.Tantalum;
                    break;

                // ========== INDUCTOR ==========
                case "INDUCTOR":
                case "COIL":
                case "CHOKE":
                    LoggerService.Instance.Info("✅ Creating InductorComponent...", "SchematicEditor");
                    component = new InductorComponent
                    {
                        Value = GetSpec("Inductance") ?? GetSpec("Value") ?? "47µH",
                        Isat = GetSpec("Saturation Current") ?? GetSpec("Current Rating") ?? "5A",
                        DCR = GetSpec("DCR") ?? GetSpec("DC Resistance") ?? "20mΩ"
                    };
                    break;

                // ========== LED ==========
                case "LED":
                case "LIGHT EMITTING DIODE":
                    LoggerService.Instance.Info("✅ Creating LEDComponent...", "SchematicEditor");
                    component = new LEDComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.LED),
                        LEDColor = GetSpec("Color") ?? "Red",
                        VF = GetSpec("VF") ?? GetSpec("Forward Voltage") ?? "2.0V",
                      //  IF = GetSpec("IF") ?? GetSpec("Forward Current") ?? "20mA"
                    };
                    break;

                // ========== FUSE ==========
                case "FUSE":
                    LoggerService.Instance.Info("✅ Creating FuseComponent...", "SchematicEditor");
                    component = new FuseComponent
                    {
                        Rating = GetSpec("Current Rating") ?? GetSpec("Rating") ?? "5A",
                        FuseType = GetSpec("Type") ?? "Fast",
                        VoltageRating = GetSpec("Voltage Rating") ?? "250V"
                    };
                    break;

                // ========== OPAMP ==========
                case "OPAMP":
                case "OP-AMP":
                case "OPERATIONAL AMPLIFIER":
                case "AMPLIFIER":
                case "DUAL OPAMP":
                case "QUAD OPAMP":
                    LoggerService.Instance.Info($"✅ Creating ICComponent (OpAmp) with {detectedPins} pins...", "SchematicEditor");

                    var opampComp = new ICComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "Unknown",
                        Function = "Operational Amplifier",
                        SupplyVoltage = GetSpec("Supply Voltage") ?? GetSpec("Voltage - Supply") ?? "±15V",
                        Package = GetSpec("Package") ?? $"{detectedPins}-DIP",
                        Features = GetSpec("Features") ?? "",
                        PinCount = detectedPins
                    };
                    opampComp.Move(0, 0);
                    component = opampComp;
                    break;

                // ========== REGULATOR ==========
                case "REGULATOR":
                case "VOLTAGE REGULATOR":
                case "LDO":
                case "LINEAR REGULATOR":
                case "SWITCHING REGULATOR":
                    LoggerService.Instance.Info($"✅ Creating ICComponent (Regulator) with {detectedPins} pins...", "SchematicEditor");

                    var regComp = new ICComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "Unknown",
                        Function = "Voltage Regulator",
                        SupplyVoltage = GetSpec("Output Voltage") ?? GetSpec("VOut") ?? "N/A",
                        Package = GetSpec("Package") ?? "TO-220",
                        Features = $"Output: {GetSpec("Output Current") ?? "N/A"}",
                        PinCount = detectedPins
                    };
                    regComp.Move(0, 0);
                    component = regComp;
                    break;

                // ========== MICROCONTROLLER ==========
                case "MICROCONTROLLER":
                case "MCU":
                case "MICRO":
                case "PROCESSOR":
                case "MICROPROCESSOR":
                    LoggerService.Instance.Info($"✅ Creating ICComponent (MCU) with {detectedPins} pins...", "SchematicEditor");

                    var mcuComp = new ICComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "Unknown",
                        Function = "Microcontroller",
                        SupplyVoltage = GetSpec("Clock Speed") ?? GetSpec("Speed") ?? "N/A",
                        Package = GetSpec("Package") ?? $"DIP-{detectedPins}",
                        Features = $"Flash: {GetSpec("Flash") ?? "N/A"}",
                        PinCount = detectedPins
                    };
                    mcuComp.Move(0, 0);
                    component = mcuComp;
                    break;

                // ========== IC GENÉRICO (solo para tipos realmente IC) ==========
                case "IC":
                case "INTEGRATED CIRCUIT":
                case "LINEAR":
                case "ANALOG":
                    LoggerService.Instance.Info($"✅ Creating ICComponent (Generic IC) with {detectedPins} pins...", "SchematicEditor");

                    var icComp = new ICComponent
                    {
                        PartNumber = dbComponent.PartNumber ?? "Unknown",
                        Function = dbComponent.Description ?? "IC",
                        SupplyVoltage = GetSpec("Supply Voltage") ?? "N/A",
                        Package = GetSpec("Package") ?? $"{detectedPins}-DIP",
                        Features = GetSpec("Features") ?? "",
                        PinCount = detectedPins
                    };
                    icComp.Move(0, 0);
                    component = icComp;
                    break;

                // ========== DEFAULT - MOSTRAR ERROR Y NO CREAR NADA ==========
                default:
                    LoggerService.Instance.Error($"❌ UNSUPPORTED TYPE: '{dbComponent.Type}'", "SchematicEditor");
                    LoggerService.Instance.Error($"   Description: '{dbComponent.Description}'", "SchematicEditor");
                    LoggerService.Instance.Error($"   Normalized: '{normalizedType}'", "SchematicEditor");

                    MessageBox.Show(
                        $"Tipo de componente no soportado:\n\n" +
                        $"Type: {dbComponent.Type}\n" +
                        $"PartNumber: {dbComponent.PartNumber}\n" +
                        $"Description: {dbComponent.Description}\n\n" +
                        $"Por favor reporta este tipo para que sea agregado.\n\n" +
                        $"Tipos soportados:\n" +
                        $"• MOSFET/FET/Transistor\n" +
                        $"• BJT/NPN/PNP\n" +
                        $"• Diode/Schottky/Zener\n" +
                        $"• Resistor/Capacitor/Inductor\n" +
                        $"• OpAmp/Regulator\n" +
                        $"• IC/Microcontroller\n" +
                        $"• LED/Fuse",
                        "Tipo No Soportado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return null;  // ✅ NO crear IC genérico, retornar null
            }

            if (component != null)
            {
                LoggerService.Instance.Info($"✅ Component created: {component.GetType().Name}", "SchematicEditor");

                if (component is ICComponent ic)
                {
                    LoggerService.Instance.Info($"   IC with {ic.PinCount} pins configured", "SchematicEditor");
                }
            }
            else
            {
                LoggerService.Instance.Error("❌ Component is NULL after creation", "SchematicEditor");
            }

            return component;
        }


        // ============================================================================
        // MÉTODO: Obtener siguiente posición para componente
        // ============================================================================

        private Point GetNextComponentPosition()
        {
            // Calcular posición basada en el último componente agregado
            Point position = new Point(
                lastPlacementPosition.X + componentPlacementOffset,
                lastPlacementPosition.Y
            );

            // Incrementar offset para el siguiente componente
            componentPlacementOffset += 100;

            // Resetear si está muy lejos
            if (componentPlacementOffset > 400)
            {
                componentPlacementOffset = 0;
                lastPlacementPosition.Y += 150;
            }

            // ✅ CORRECCIÓN AQUÍ - Reemplazar 'schematicCanvas' con el nombre correcto
            if (canvasPanel != null)  // <-- CAMBIAR 'canvasPanel' por el nombre real
            {
                if (position.X > canvasPanel.Width - 100 ||   // <-- AQUÍ TAMBIÉN
                    position.Y > canvasPanel.Height - 100)    // <-- Y AQUÍ
                {
                    lastPlacementPosition = new Point(100, 100);
                    componentPlacementOffset = 0;
                    position = lastPlacementPosition;
                }
            }

            // ✅ CORRECCIÓN: Validación simple sin necesidad de canvas
            // Usar límites fijos razonables para un esquemático típico
            const int MAX_WIDTH = 1200;
            const int MAX_HEIGHT = 800;

            if (position.X > MAX_WIDTH - 100 || position.Y > MAX_HEIGHT - 100)
            {
                lastPlacementPosition = new Point(100, 100);
                componentPlacementOffset = 0;
                position = lastPlacementPosition;
            }

            return position;
        }

        // Event Handlers

        private void OnDCMAnalysis(object sender, EventArgs e)
        {
            if (dcmDialog == null || dcmDialog.IsDisposed)
            {
                dcmDialog = new DCMAnalysisDialog();
            }

            // Extraer parámetros del circuito
            var parameters = ExtractCircuitParameters();
            dcmDialog.SetParameters(parameters);
            dcmDialog.Show();
        }

        private void OnBodePlot(object sender, EventArgs e)
        {
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            if (bodeDialog == null || bodeDialog.IsDisposed)
            {
                bodeDialog = new BodePlotDialog();
            }

            var parameters = ExtractCircuitParameters();
            bodeAnalyzer = new BodeAnalyzer
            {
                Inductance = parameters.Inductance,
                Capacitance = parameters.Capacitance,
                ESR = parameters.CapacitorESR,
                LoadResistance = parameters.LoadResistance
            };

            var bodePlot = bodeAnalyzer.GenerateBodePlot(1, 1000000, 200);
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            bodeDialog.SetBodePlot(bodePlot);
            bodeDialog.Show();
        }

        private void OnWaveformSimulation(object sender, EventArgs e)
        {
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            if (waveformDialog == null || waveformDialog.IsDisposed)
            {
                waveformDialog = new WaveformSimulationDialog();
            }

            var parameters = ExtractCircuitParameters();
            waveformSimulator = new WaveformSimulator
            {
                Circuit = parameters,
                Settings = new SimulationSettings
                {
                    SimulationTime = 0.001, // 1ms
                    SamplesPerCycle = 100
                }
            };
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            waveformDialog.SetParameters(parameters);
            waveformDialog.Show();
        }

        private void OnComponentDatabase(object sender, EventArgs e)
        {
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            if (databaseDialog == null || databaseDialog.IsDisposed)
            {
                databaseDialog = new ComponentDatabaseDialog(componentDatabase);
            }

            databaseDialog.Show();
        }

        // ============================================================================
        // VERSIÓN CON LOGGING - OnSearchComponent
        // ============================================================================

        private void OnSearchComponent(object sender, EventArgs e)
        {
            // ✅ PRIMER LOG - SI NO APARECE, EL EVENTO NO ESTÁ CONECTADO
            MessageBox.Show("OnSearchComponent CALLED!", "DEBUG");

            LoggerService.Instance.LogDivider();
            LoggerService.Instance.Info("=== OnSearchComponent CALLED ===", "SchematicEditor");

            try
            {
                statusLabel.Text = "Opening component search dialog...";

                if (componentDatabase == null)
                {
                    LoggerService.Instance.Error("componentDatabase is NULL!", "SchematicEditor");
                    MessageBox.Show("ERROR: componentDatabase is NULL!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoggerService.Instance.Info("Creating ComponentSearchDialog...", "SchematicEditor");
                var searchDialog = new ComponentSearchDialog(componentDatabase);

                LoggerService.Instance.Info("Showing ComponentSearchDialog...", "SchematicEditor");
                statusLabel.Text = "Component search dialog opened. Waiting for selection...";

                var result = searchDialog.ShowDialog();

                LoggerService.Instance.Info($"Dialog closed with result: {result}", "SchematicEditor");
                statusLabel.Text = $"Dialog closed with result: {result}";

                if (result == DialogResult.OK)
                {
                    var selectedComponent = searchDialog.SelectedComponent;

                    if (selectedComponent == null)
                    {
                        LoggerService.Instance.Warning("SelectedComponent is NULL", "SchematicEditor");
                        MessageBox.Show(
                            "No component was selected from the search results.\n\n" +
                            "Please:\n" +
                            "1. Search for components\n" +
                            "2. Select a component from the list\n" +
                            "3. Click 'Select Component' button",
                            "No Selection",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    // ✅ LOG: Info del componente seleccionado
                    LoggerService.Instance.Info($"Component selected: {selectedComponent.PartNumber}", "SchematicEditor");
                    LoggerService.Instance.Info($"  Type: {selectedComponent.Type}", "SchematicEditor");
                    LoggerService.Instance.Info($"  Manufacturer: {selectedComponent.Manufacturer}", "SchematicEditor");
                    LoggerService.Instance.Info($"  Description: {selectedComponent.Description}", "SchematicEditor");

                    if (selectedComponent.Specifications != null)
                    {
                        LoggerService.Instance.Info($"  Specifications count: {selectedComponent.Specifications.Count}", "SchematicEditor");
                        foreach (var spec in selectedComponent.Specifications.Take(5))
                        {
                            LoggerService.Instance.Debug($"    {spec.Key} = {spec.Value}", "SchematicEditor");
                        }
                    }

                    // ✅ LLAMAR A AddComponentFromDatabase
                    LoggerService.Instance.Info("Calling AddComponentFromDatabase...", "SchematicEditor");
                    AddComponentFromDatabase(selectedComponent);
                    LoggerService.Instance.Info("AddComponentFromDatabase returned", "SchematicEditor");
                }
                else
                {
                    LoggerService.Instance.Info("User cancelled component search", "SchematicEditor");
                    statusLabel.Text = "Component search cancelled";
                }
            }
            catch (Exception ex)
            {
                LoggerService.Instance.Error($"Exception in OnSearchComponent: {ex.Message}", "SchematicEditor", ex);
                MessageBox.Show(
                    $"Exception in OnSearchComponent:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                statusLabel.Text = "Error in component search";
            }

            LoggerService.Instance.Info("=== OnSearchComponent COMPLETED ===", "SchematicEditor");
        }

        private void OnExportBOMCSV(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                // CORREGIDO: Usar nombre de archivo por defecto si ProjectName no existe
                dialog.FileName = "BOM.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var bom = bomGenerator.GenerateBOM();
                        var exporter = new BOMExporter();
                        exporter.ExportToCSV(bom, dialog.FileName);

                        MessageBox.Show($"BOM exported successfully to {dialog.FileName}",
                                       "Export BOM",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting BOM: {ex.Message}",
                                       "Export Error",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnExportBOMExcel(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                dialog.DefaultExt = "xlsx";
                // CORREGIDO: Usar nombre de archivo por defecto si ProjectName no existe
                dialog.FileName = "BOM.xlsx";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var bom = bomGenerator.GenerateBOM();
                        var exporter = new BOMExporter();
                        exporter.ExportToExcel(bom, dialog.FileName);

                        MessageBox.Show($"BOM exported successfully to {dialog.FileName}",
                                       "Export BOM",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting BOM: {ex.Message}",
                                       "Export Error",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnGeneratePCB(object sender, EventArgs e)
        {
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            if (pcbDialog == null || pcbDialog.IsDisposed)
            {
                pcbDialog = new PCBLayoutDialog();
            }

            pcbGenerator = new PCBLayoutGenerator(document);
            var settings = new PCBSettings
            {
                BoardWidth = 100,
                BoardHeight = 80,
                Layers = 2,
                DefaultTraceWidth = 0.25
            };

            var layout = pcbGenerator.GenerateLayout(settings);
            // Analizar codigo comentado abajo para usar el nombre del proyecto
            pcbDialog.SetLayout(layout);
            pcbDialog.Show();
        }

        private void OnExportToKiCad(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "KiCad PCB files (*.kicad_pcb)|*.kicad_pcb|All files (*.*)|*.*";
                dialog.DefaultExt = "kicad_pcb";
                // Analizar codigo comentado abajo para usar el nombre del proyecto
                //dialog.FileName = $"{document.ProjectName}.kicad_pcb";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pcbGenerator = new PCBLayoutGenerator(document);
                    var layout = pcbGenerator.GenerateLayout(new PCBSettings());
                    pcbGenerator.ExportToKiCad(layout, dialog.FileName);

                    MessageBox.Show($"PCB layout exported to {dialog.FileName}",
                                   "Export PCB",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                }
            }
        }

        private CircuitParameters ExtractCircuitParameters()
        {
            // Extraer parámetros del circuito desde los componentes del esquemático
            var parameters = new CircuitParameters
            {
                InputVoltage = 90,  // Default, debería extraerse
                DutyCycle = 0.5,
                SwitchingFrequency = 100000,
                Inductance = 0.000047,
                InductorESR = 0.02,
                Capacitance = 0.00001,
                CapacitorESR = 0.005,
                LoadResistance = 2.6,
                SwitchRDSon = 0.05,
                DiodeVF = 0.5
            };

            // Buscar componentes específicos en el esquemático
            foreach (var component in document.Components)
            {
                if (component is InductorComponent inductor)
                {
                    parameters.Inductance = ParseValue(inductor.Value);
                    parameters.InductorESR = ParseValue(inductor.DCR);
                }
                else if (component is CapacitorComponent capacitor)
                {
                    parameters.Capacitance = ParseValue(capacitor.Capacitance);
                }
                else if (component is MosfetComponent mosfet)
                {
                    parameters.SwitchRDSon = ParseValue(mosfet.RDSon);
                }
            }

            return parameters;
        }

        private double ParseValue(string value)
        {
            // Parser de valores con prefijos (µ, m, k, etc.)
            if (string.IsNullOrEmpty(value)) return 0;

            value = value.ToUpper().Trim();
            double multiplier = 1;

            if (value.Contains("M") && !value.Contains("MΩ"))
                multiplier = 0.001;
            else if (value.Contains("µ") || value.Contains("U"))
                multiplier = 0.000001;
            else if (value.Contains("N"))
                multiplier = 0.000000001;
            else if (value.Contains("K"))
                multiplier = 1000;

            value = value.Replace("µ", "").Replace("M", "").Replace("K", "")
                        .Replace("N", "").Replace("U", "").Replace("H", "")
                        .Replace("F", "").Replace("Ω", "").Replace("V", "")
                        .Replace("A", "").Trim();

            if (double.TryParse(value, out double result))
                return result * multiplier;

            return 0;
        }
        // Fin de las partes nuevas añadidas-------------------------------------------------------------------

        private void InitializeComponent()
        {
            this.Text = "Schematic Editor - Buck Converter [Enhanced]";
            this.Size = new Size(1800, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            // MenuStrip
            CreateMenuStrip();

            // ToolStrip
            CreateToolStrip();

            // StatusStrip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready | Snap: ON | Grid: ON");
            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(new ToolStripStatusLabel() { Spring = true });
            statusStrip.Items.Add(new ToolStripStatusLabel($"Zoom: {zoomFactor * 100:F0}%"));

            // Canvas Panel - sin padding, el ToolBox estará acoplado al lado
            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke
            };

            // Canvas
            canvas = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(
                    (int)(document.CanvasWidth * zoomFactor),
                    (int)(document.CanvasHeight * zoomFactor)
                ),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.MouseClick += Canvas_MouseClick;
            canvas.MouseDoubleClick += Canvas_MouseDoubleClick;
            canvas.AllowDrop = true;
            canvas.DragEnter += Canvas_DragEnter;
            canvas.DragDrop += Canvas_DragDrop;
            canvas.DragOver += Canvas_DragOver;

            canvasPanel.Controls.Add(canvas);

            // PropertyGrid
            propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized
            };
            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;

            // Component Library Panel
            CreateComponentLibrary();

            // Main Layout - Estructura: ToolBox | Canvas | RightPanel
            // Panel derecho (ComponentLibrary + PropertyGrid)
            var rightPanel = new SplitContainer
            {
                Dock = DockStyle.Right,
                Width = 320,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 120
            };
            rightPanel.Panel1.Controls.Add(componentLibraryPanel);
            rightPanel.Panel2.Controls.Add(propertyGrid);

            // Agregar controles al formulario en orden correcto
            this.Controls.Add(canvasPanel);      // Canvas ocupa el centro (Fill)
            this.Controls.Add(rightPanel);       // Panel derecho
            // componentToolBox se agregará después en CreateComponentToolBox() como Dock.Left
            this.Controls.Add(toolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(statusStrip);
            this.MainMenuStrip = menuStrip;

            this.KeyDown += SchematicEditorForm_KeyDown;
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // File Menu
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("New", null, OnFileNew);
            fileMenu.DropDownItems.Add("Open...", null, OnFileOpen);
            fileMenu.DropDownItems.Add("Save", null, OnFileSave);
            fileMenu.DropDownItems.Add("Save As...", null, OnFileSaveAs);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Export PNG...", null, OnExportPNG);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());

            // Edit Menu
            var editMenu = new ToolStripMenuItem("Edit");

            var copyMenuItem = new ToolStripMenuItem("Copy", null, OnEditCopy);
            copyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            editMenu.DropDownItems.Add(copyMenuItem);

            var pasteMenuItem = new ToolStripMenuItem("Paste", null, OnEditPaste);
            pasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            editMenu.DropDownItems.Add(pasteMenuItem);

            editMenu.DropDownItems.Add(new ToolStripSeparator());

            var deleteMenuItem = new ToolStripMenuItem("Delete", null, OnEditDelete);
            deleteMenuItem.ShortcutKeys = Keys.Delete;
            editMenu.DropDownItems.Add(deleteMenuItem);

            var duplicateMenuItem = new ToolStripMenuItem("Duplicate", null, OnEditDuplicate);
            duplicateMenuItem.ShortcutKeys = Keys.Control | Keys.D;
            editMenu.DropDownItems.Add(duplicateMenuItem);

            editMenu.DropDownItems.Add(new ToolStripSeparator());

            var selectAllMenuItem = new ToolStripMenuItem("Select All", null, OnEditSelectAll);
            selectAllMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            editMenu.DropDownItems.Add(selectAllMenuItem);

            // View Menu
            var viewMenu = new ToolStripMenuItem("View");

            var zoomInMenuItem = new ToolStripMenuItem("Zoom In", null, OnViewZoomIn);
            zoomInMenuItem.ShortcutKeys = Keys.Control | Keys.Add;
            viewMenu.DropDownItems.Add(zoomInMenuItem);

            var zoomOutMenuItem = new ToolStripMenuItem("Zoom Out", null, OnViewZoomOut);
            zoomOutMenuItem.ShortcutKeys = Keys.Control | Keys.Subtract;
            viewMenu.DropDownItems.Add(zoomOutMenuItem);

            var zoom100MenuItem = new ToolStripMenuItem("Zoom 100%", null, OnViewZoom100);
            zoom100MenuItem.ShortcutKeys = Keys.Control | Keys.D0;
            viewMenu.DropDownItems.Add(zoom100MenuItem);

            viewMenu.DropDownItems.Add(new ToolStripSeparator());

            var gridMenuItem = new ToolStripMenuItem("Show Grid", null, OnViewToggleGrid);
            gridMenuItem.Checked = showGrid;
            gridMenuItem.CheckOnClick = true;
            viewMenu.DropDownItems.Add(gridMenuItem);

            var snapMenuItem = new ToolStripMenuItem("Snap to Grid", null, OnViewToggleSnap);
            snapMenuItem.Checked = snapToGrid;
            snapMenuItem.CheckOnClick = true;
            viewMenu.DropDownItems.Add(snapMenuItem);

            viewMenu.DropDownItems.Add(new ToolStripSeparator());

            // Component ToolBox submenu
            var toolboxMenu = new ToolStripMenuItem("Component ToolBox");

            var showToolboxMenuItem = new ToolStripMenuItem("Show ToolBox", null, (s, e) => componentToolBox.Show());
            toolboxMenu.DropDownItems.Add(showToolboxMenuItem);

            var hideToolboxMenuItem = new ToolStripMenuItem("Hide ToolBox", null, (s, e) => componentToolBox.Hide());
            toolboxMenu.DropDownItems.Add(hideToolboxMenuItem);

            toolboxMenu.DropDownItems.Add(new ToolStripSeparator());

            var expandToolboxMenuItem = new ToolStripMenuItem("Expand/Collapse", null, (s, e) => componentToolBox.ToggleExpand());
            expandToolboxMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            toolboxMenu.DropDownItems.Add(expandToolboxMenuItem);

            var pinToolboxMenuItem = new ToolStripMenuItem("Pin/Unpin", null, (s, e) => componentToolBox.IsPinned = !componentToolBox.IsPinned);
            toolboxMenu.DropDownItems.Add(pinToolboxMenuItem);

            toolboxMenu.DropDownItems.Add(new ToolStripSeparator());

            var dockLeftMenuItem = new ToolStripMenuItem("Dock Left", null, (s, e) => componentToolBox.DockToLeft(this, menuStrip.Height + toolStrip.Height));
            toolboxMenu.DropDownItems.Add(dockLeftMenuItem);

            var dockRightMenuItem = new ToolStripMenuItem("Dock Right", null, (s, e) => componentToolBox.DockToRight(this, menuStrip.Height + toolStrip.Height));
            toolboxMenu.DropDownItems.Add(dockRightMenuItem);

            var floatMenuItem = new ToolStripMenuItem("Float", null, (s, e) => componentToolBox.Float(new Point(300, 200)));
            toolboxMenu.DropDownItems.Add(floatMenuItem);

            viewMenu.DropDownItems.Add(toolboxMenu);

            // Arrange Menu
            var arrangeMenu = new ToolStripMenuItem("Arrange");

            var alignLeftMenuItem = new ToolStripMenuItem("Align Left", null, (s, e) => AlignComponents(AlignmentType.Left));
            arrangeMenu.DropDownItems.Add(alignLeftMenuItem);

            var alignRightMenuItem = new ToolStripMenuItem("Align Right", null, (s, e) => AlignComponents(AlignmentType.Right));
            arrangeMenu.DropDownItems.Add(alignRightMenuItem);

            var alignTopMenuItem = new ToolStripMenuItem("Align Top", null, (s, e) => AlignComponents(AlignmentType.Top));
            arrangeMenu.DropDownItems.Add(alignTopMenuItem);

            var alignBottomMenuItem = new ToolStripMenuItem("Align Bottom", null, (s, e) => AlignComponents(AlignmentType.Bottom));
            arrangeMenu.DropDownItems.Add(alignBottomMenuItem);

            arrangeMenu.DropDownItems.Add(new ToolStripSeparator());

            var distributeHMenuItem = new ToolStripMenuItem("Distribute Horizontally", null, (s, e) => DistributeComponents(true));
            arrangeMenu.DropDownItems.Add(distributeHMenuItem);

            var distributeVMenuItem = new ToolStripMenuItem("Distribute Vertically", null, (s, e) => DistributeComponents(false));
            arrangeMenu.DropDownItems.Add(distributeVMenuItem);

            arrangeMenu.DropDownItems.Add(new ToolStripSeparator());

            var rotateMenuItem = new ToolStripMenuItem("Rotate 90°", null, OnRotate90);
            rotateMenuItem.ShortcutKeys = Keys.Control | Keys.R;
            arrangeMenu.DropDownItems.Add(rotateMenuItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            menuStrip.Items.Add(viewMenu);
            menuStrip.Items.Add(arrangeMenu);


        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.ImageScalingSize = new Size(24, 24);

            var btnSelect = new ToolStripButton("Select", null, OnToolSelect) { CheckOnClick = true, Checked = true };
            var btnResistor = new ToolStripButton("Resistor", null, OnToolResistor) { CheckOnClick = true };
            var btnCapacitor = new ToolStripButton("Capacitor", null, OnToolCapacitor) { CheckOnClick = true };
            var btnInductor = new ToolStripButton("Inductor", null, OnToolInductor) { CheckOnClick = true };
            var btnWire = new ToolStripButton("Wire", null, OnToolWire) { CheckOnClick = true };
            var btnLabel = new ToolStripButton("Label", null, OnToolLabel) { CheckOnClick = true };

            toolStrip.Items.Add(btnSelect);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnResistor);
            toolStrip.Items.Add(btnCapacitor);
            toolStrip.Items.Add(btnInductor);
            toolStrip.Items.Add(btnWire);
            toolStrip.Items.Add(btnLabel);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Delete", null, OnEditDelete));
            toolStrip.Items.Add(new ToolStripButton("Duplicate", null, OnEditDuplicate));
            toolStrip.Items.Add(new ToolStripButton("Rotate", null, OnRotate90));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Align Left", null, (s, e) => AlignComponents(AlignmentType.Left)));
            toolStrip.Items.Add(new ToolStripButton("Align Top", null, (s, e) => AlignComponents(AlignmentType.Top)));
        }

        private void CreateComponentLibrary()
        {
            componentLibraryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = "Component Library",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            libraryListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9)
            };
            libraryListBox.DoubleClick += LibraryListBox_DoubleClick;
            libraryListBox.MouseDown += LibraryListBox_MouseDown;
            libraryListBox.MouseMove += LibraryListBox_MouseMove;
            libraryListBox.MouseUp += LibraryListBox_MouseUp;

            componentLibraryPanel.Controls.Add(libraryListBox);
            componentLibraryPanel.Controls.Add(lblTitle);
        }

        private void CreateComponentToolBox()
        {
            componentToolBox = new ComponentToolBox();

            // Conectar eventos de drag & drop
            componentToolBox.ComponentDragStart += ComponentToolBox_ComponentDragStart;
            componentToolBox.ComponentDragging += ComponentToolBox_ComponentDragging;
            componentToolBox.ComponentDragDrop += ComponentToolBox_ComponentDragDrop;

            // Conectar evento de expansión/colapso
            componentToolBox.ExpandedChanged += ComponentToolBox_ExpandedChanged;

            // Configurar como panel acoplado a la izquierda
            componentToolBox.Dock = DockStyle.Left;
            componentToolBox.Width = 160;  // Ancho inicial (expandido)

            // Agregar al formulario
            this.Controls.Add(componentToolBox);
            componentToolBox.BringToFront();

            // Los eventos DragDrop ya están registrados en InitializeCanvas()
            // Solo necesitamos habilitar AllowDrop si no estaba habilitado
            if (!canvas.AllowDrop)
            {
                canvas.AllowDrop = true;
            }

            if (!canvasPanel.AllowDrop)
            {
                canvasPanel.AllowDrop = true;
                canvasPanel.DragEnter += Canvas_DragEnter;
                canvasPanel.DragOver += Canvas_DragOver;
                canvasPanel.DragDrop += Canvas_DragDrop;
            }
        }

        private void ComponentToolBox_ExpandedChanged(object sender, EventArgs e)
        {
            // Ajustar ancho del ToolBox cuando cambia de estado
            if (componentToolBox.IsExpanded)
            {
                componentToolBox.Width = 160;  // Expandido
            }
            else
            {
                componentToolBox.Width = 50;   // Colapsado
            }
        }

        private bool isComponentDragging = false;

        private void ComponentToolBox_ComponentDragStart(object sender, ComponentDragEventArgs e)
        {
            isComponentDragging = true;
            statusLabel.Text = $"Dragging {e.ComponentName}... Release to place";
        }

        private void ComponentToolBox_ComponentDragging(object sender, ComponentDragEventArgs e)
        {
            // Actualizar visualización mientras se arrastra
            if (isComponentDragging)
            {
                // Convertir a coordenadas del canvas
                Point canvasClientPoint = canvas.PointToClient(e.ScreenLocation);
                if (canvas.ClientRectangle.Contains(canvasClientPoint))
                {
                    Point canvasPoint = new Point(
                        (int)(canvasClientPoint.X / zoomFactor),
                        (int)(canvasClientPoint.Y / zoomFactor)
                    );
                    canvasPoint = SnapToGrid(canvasPoint);
                    statusLabel.Text = $"Dragging... Position: ({canvasPoint.X}, {canvasPoint.Y})";
                }
            }
        }

        private void ComponentToolBox_ComponentDragDrop(object sender, ComponentDragEventArgs e)
        {
            // Este evento solo confirma que el drop terminó
            // El componente ya fue agregado por Canvas_DragDrop
            isComponentDragging = false;
        }

        // Manejadores nativos de DragDrop de Windows Forms
        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(ComponentDragData)))
                {
                    e.Effect = DragDropEffects.Copy;
                    statusLabel.Text = "Drop here to place component";
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    statusLabel.Text = "Cannot drop here";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"DragEnter error: {ex.Message}";
                e.Effect = DragDropEffects.None;
            }
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(ComponentDragData)))
                {
                    e.Effect = DragDropEffects.Copy;

                    // Mostrar coordenadas durante drag
                    Point canvasClientPoint = canvas.PointToClient(new Point(e.X, e.Y));
                    Point canvasPoint = new Point(
                        (int)(canvasClientPoint.X / zoomFactor),
                        (int)(canvasClientPoint.Y / zoomFactor)
                    );
                    canvasPoint = SnapToGrid(canvasPoint);
                    statusLabel.Text = $"Position: ({canvasPoint.X}, {canvasPoint.Y})";
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"DragOver error: {ex.Message}";
                e.Effect = DragDropEffects.None;
            }
        }

        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(ComponentDragData)))
                {
                    var dragData = (ComponentDragData)e.Data.GetData(typeof(ComponentDragData));

                    statusLabel.Text = $"Dropping {dragData.ComponentName}...";

                    // Crear componente
                    var component = CreateComponentFromType(dragData.GetComponentType(), dragData.ComponentName);

                    if (component != null)
                    {
                        // Convertir coordenadas
                        Point canvasClientPoint = canvas.PointToClient(new Point(e.X, e.Y));
                        Point canvasPoint = new Point(
                            (int)(canvasClientPoint.X / zoomFactor),
                            (int)(canvasClientPoint.Y / zoomFactor)
                        );
                        canvasPoint = SnapToGrid(canvasPoint);

                        // Posicionar
                        component.X = canvasPoint.X;
                        component.Y = canvasPoint.Y;

                        // Caso especial para Wire
                        if (component is WireComponent wire)
                        {
                            wire.X1 = canvasPoint.X;
                            wire.Y1 = canvasPoint.Y;
                            wire.X2 = canvasPoint.X + 100;
                            wire.Y2 = canvasPoint.Y;
                        }

                        // Actualizar bounds
                        component.Move(0, 0);

                        // Agregar al documento
                        document.AddComponent(component);
                        document.ClearSelection();
                        component.IsSelected = true;
                        propertyGrid.SelectedObject = component;
                        canvas.Invalidate();

                        statusLabel.Text = $"Added {component.Name} at ({canvasPoint.X}, {canvasPoint.Y})";
                    }
                    else
                    {
                        statusLabel.Text = $"Failed to create component: {dragData.ComponentName}";
                    }
                }
                else
                {
                    statusLabel.Text = "Invalid drop data";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Drop error: {ex.Message}";
                MessageBox.Show($"Error during drop: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Drop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComponentToolBox_ComponentSelected(object sender, ComponentSelectedEventArgs e)
        {
            // Convertir ComponentType a EditorMode
            EditorMode newMode = EditorMode.Select;

            switch (e.ComponentType)
            {
                case ComponentType.Resistor:
                    newMode = EditorMode.AddResistor;
                    break;
                case ComponentType.Capacitor:
                    newMode = EditorMode.AddCapacitor;
                    break;
                case ComponentType.Inductor:
                    newMode = EditorMode.AddInductor;
                    break;
                case ComponentType.Wire:
                    newMode = EditorMode.AddWire;
                    break;
                case ComponentType.Label:
                    newMode = EditorMode.AddLabel;
                    break;
                case ComponentType.Terminal:
                    newMode = EditorMode.AddTerminal;
                    break;
                case ComponentType.Diode:
                case ComponentType.Mosfet:
                case ComponentType.IC:
                case ComponentType.Fuse:
                case ComponentType.Ground:
                case ComponentType.VccSupply:
                case ComponentType.Node:
                    // Para estos componentes, crear directamente desde biblioteca
                    SchematicComponent component = CreateComponentFromType(e.ComponentType, e.ComponentName);
                    if (component != null)
                    {
                        // Posicionar en el centro visible del canvas
                        Point centerPoint = new Point(
                            canvasPanel.HorizontalScroll.Value + canvasPanel.Width / 2,
                            canvasPanel.VerticalScroll.Value + canvasPanel.Height / 2
                        );

                        Point canvasPoint = new Point(
                            (int)(centerPoint.X / zoomFactor),
                            (int)(centerPoint.Y / zoomFactor)
                        );

                        canvasPoint = SnapToGrid(canvasPoint);

                        component.X = canvasPoint.X;
                        component.Y = canvasPoint.Y;
                        component.Move(0, 0);

                        document.AddComponent(component);
                        document.ClearSelection();
                        component.IsSelected = true;
                        propertyGrid.SelectedObject = component;
                        canvas.Invalidate();
                        statusLabel.Text = $"Added {component.Name} from toolbox";
                    }
                    return;
            }

            if (newMode != currentMode)
            {
                currentMode = newMode;
                UpdateToolbarButtons();
                statusLabel.Text = $"Mode: {e.ComponentName} - Click on canvas to place";
            }
        }

        private SchematicComponent CreateComponentFromType(ComponentType type, string name)
        {
            SchematicComponent component = null;

            switch (type)
            {
                case ComponentType.Terminal:
                    component = new TerminalComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Terminal),
                        Voltage = "0V",
                        TerminalColor = Color.Red
                    };
                    break;

                case ComponentType.Resistor:
                    component = new ResistorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Resistor),
                        Value = "10kΩ",
                        Power = "1/4W",
                        Tolerance = "1%"
                    };
                    break;

                case ComponentType.Capacitor:
                    component = new CapacitorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Capacitor),
                        Capacitance = "100µF",
                        VoltageRating = "25V",
                        CapacitorType = CapacitorType.Ceramic,
                        ESR = "<5mΩ"
                    };
                    break;

                case ComponentType.Inductor:
                    component = new InductorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Inductor),
                        Value = "100µH",
                        Isat = "5A",
                        DCR = "20mΩ"
                    };
                    break;

                case ComponentType.Diode:
                    component = new DiodeComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Diode),
                        PartNumber = "1N4148",
                        VoltageRating = "100V",
                        CurrentRating = "200mA",
                        DiodeType = "Fast"
                    };
                    break;

                case ComponentType.Mosfet:
                    // Verificar si es BJT o MOSFET
                    if (name != null && name.Contains("BJT"))
                    {
                        component = new BJTComponent
                        {
                            Name = document.GetNextComponentName(ComponentType.Mosfet),
                            PartNumber = "2N2222",
                            TransistorType = "NPN",
                            VCE = "40V",
                            IC = "800mA",
                            PowerRating = "500mW",
                            HFE = "100-300"
                        };
                    }
                    else
                    {
                        component = new MosfetComponent
                        {
                            Name = document.GetNextComponentName(ComponentType.Mosfet),
                            PartNumber = "IRF540",
                            ChannelType = "N-Channel",
                            VDS = "100V",
                            ID = "28A",
                            RDSon = "44mΩ"
                        };
                    }
                    break;

                case ComponentType.IC:
                    component = new ICComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.IC),
                        PartNumber = "TL494",
                        Function = "PWM Controller",
                        SupplyVoltage = "12V",
                        PinCount = 16,
                        Width = 120,
                        Height = 80
                    };
                    break;

                case ComponentType.Fuse:
                    component = new FuseComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Fuse),
                        Rating = "5A",
                        FuseType = "Fast",
                        VoltageRating = "250V",
                        Length = 60
                    };
                    break;

                case ComponentType.Ground:
                    component = new GroundComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Ground),
                        GroundType = "Earth",
                        Size = 30
                    };
                    break;

                case ComponentType.VccSupply:
                    component = new VccSupplyComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.VccSupply),
                        Voltage = "5V",
                        Size = 20
                    };
                    break;

                case ComponentType.Node:
                    component = new NodeComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Node),
                        NodeName = "N1",
                        NodeColor = Color.Black,
                        Size = 8
                    };
                    break;

                case ComponentType.Wire:
                    component = new WireComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Wire),
                        X1 = 0,
                        Y1 = 0,
                        X2 = 100,
                        Y2 = 0,
                        WireColor = Color.Black,
                        WireType = "Signal",
                        Thickness = 2.5f
                    };
                    break;

                case ComponentType.Label:
                    component = new LabelComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Label),
                        Text = "Label",
                        FontSize = 10,
                        FontName = "Arial",
                        TextColor = Color.Black,
                        IsBold = false
                    };
                    break;

                case ComponentType.Potentiometer:
                    component = new PotentiometerComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Potentiometer),
                        Value = "10kΩ",
                        Power = "1/4W"
                    };
                    break;

                case ComponentType.SchottkyDiode:
                    component = new SchottkyDiodeComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.SchottkyDiode),
                        VF = "0.3V",
                        CurrentRating = "1A"
                    };
                    break;

                case ComponentType.ZenerDiode:
                    component = new ZenerDiodeComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.ZenerDiode),
                        ZenerVoltage = "5.1V",
                        Power = "500mW"
                    };
                    break;

                case ComponentType.LED:
                    component = new LEDComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.LED),
                        LEDColor = "Red",
                        VF = "2.0V"
                    };
                    break;

                case ComponentType.BridgeDiode:
                    component = new BridgeDiodeComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.BridgeDiode),
                        CurrentRating = "1A",
                        VoltageRating = "400V"
                    };
                    break;

                case ComponentType.Triac:
                    component = new TriacComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Triac),
                        CurrentRating = "8A"
                    };
                    break;

                case ComponentType.Thyristor:
                    component = new ThyristorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Thyristor),
                        CurrentRating = "10A"
                    };
                    break;

                case ComponentType.OpAmp:
                    component = new OpAmpComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.OpAmp),
                        PartNumber = "LM358",
                        SupplyVoltage = "±15V"
                    };
                    break;

                case ComponentType.LogicGate:
                    component = new LogicGateComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.LogicGate),
                        GateType = "AND"
                    };
                    break;

                case ComponentType.Inverter:
                    component = new InverterComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Inverter)
                    };
                    break;

                case ComponentType.FlipFlop:
                    component = new FlipFlopComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.FlipFlop),
                        FFType = "D"
                    };
                    break;

                case ComponentType.LED7Segment:
                    component = new LED7SegmentComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.LED7Segment),
                        CommonType = "Common Cathode"
                    };
                    break;

                case ComponentType.Buzzer:
                    component = new BuzzerComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Buzzer),
                        Voltage = "5V",
                        Frequency = "2KHz"
                    };
                    break;

                case ComponentType.Optocoupler:
                    component = new OptocouplerComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Optocoupler),
                        PartNumber = "4N35"
                    };
                    break;

                case ComponentType.Transformer:
                    component = new TransformerComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Transformer),
                        TurnsRatio = "10:1",
                        PowerRating = "10VA"
                    };
                    break;

                case ComponentType.Battery:
                    component = new BatteryComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Battery),
                        Voltage = "9V",
                        Capacity = "500mAh"
                    };
                    break;
            }

            return component;
        }

        private void PopulateComponentLibrary()
        {
            libraryListBox.Items.Clear();
            libraryListBox.Items.Add("📍 Terminal (+/-)");
            libraryListBox.Items.Add("⚡ Resistor 10kΩ 1/4W");
            libraryListBox.Items.Add("⚡ Resistor 100Ω 2W");
            libraryListBox.Items.Add("⚡ Resistor 1kΩ 1W");
            libraryListBox.Items.Add("🔋 Capacitor 100µF/25V");
            libraryListBox.Items.Add("🔋 Capacitor 47µF/160V");
            libraryListBox.Items.Add("🔋 Capacitor 10µF/50V");
            libraryListBox.Items.Add("🌀 Inductor 100µH");
            libraryListBox.Items.Add("🌀 Inductor 220µH");
            libraryListBox.Items.Add("🔺 Diode 1N4148");
            libraryListBox.Items.Add("🔺 Diode Schottky MBRS360");
            libraryListBox.Items.Add("🔲 MOSFET N-Channel IRF540");
            libraryListBox.Items.Add("🔲 MOSFET P-Channel IRF9540");
            libraryListBox.Items.Add("⬛ IC TL494 (PWM Controller)");
            libraryListBox.Items.Add("⬛ IC LM358 (Op-Amp)");
            libraryListBox.Items.Add("━━ Wire (Power)");
            libraryListBox.Items.Add("── Wire (Signal)");
            libraryListBox.Items.Add("⏚ Ground");
            libraryListBox.Items.Add("📝 Label");
        }

        private void LibraryListBox_DoubleClick(object sender, EventArgs e)
        {
            if (libraryListBox.SelectedIndex == -1) return;

            string selected = libraryListBox.SelectedItem.ToString();
            SchematicComponent component = CreateComponentFromLibraryItem(selected);

            if (component != null)
            {
                document.AddComponent(component);
                document.ClearSelection();
                component.IsSelected = true;
                propertyGrid.SelectedObject = component;
                canvas.Invalidate();
                statusLabel.Text = $"Added {component.Name} from library";
            }
        }

        private void LibraryListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && libraryListBox.SelectedIndex != -1)
            {
                // Preparar para arrastrar
                isDraggingFromLibrary = true;
            }
        }

        private void LibraryListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingFromLibrary && e.Button == MouseButtons.Left)
            {
                if (libraryListBox.SelectedIndex == -1) return;

                // Crear el componente que se va a arrastrar
                string selected = libraryListBox.SelectedItem.ToString();
                libraryDragComponent = CreateComponentFromLibraryItem(selected);

                if (libraryDragComponent != null)
                {
                    // Cambiar cursor
                    libraryListBox.Cursor = Cursors.Hand;
                    canvas.AllowDrop = true;

                    // Iniciar DragDrop
                    libraryListBox.DoDragDrop(libraryDragComponent, DragDropEffects.Copy);
                }

                isDraggingFromLibrary = false;
                libraryListBox.Cursor = Cursors.Default;
            }
        }

        private void LibraryListBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggingFromLibrary = false;
            libraryListBox.Cursor = Cursors.Default;
        }

        private SchematicComponent CreateComponentFromLibraryItem(string itemText)
        {
            SchematicComponent component = null;

            if (itemText.Contains("Terminal"))
            {
                component = new TerminalComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Terminal),
                    X = 200,
                    Y = 200,
                    Voltage = "0V"
                };
            }
            else if (itemText.Contains("Resistor 10kΩ"))
            {
                component = new ResistorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Resistor),
                    X = 200,
                    Y = 200,
                    Value = "10kΩ",
                    Power = "1/4W",
                    Tolerance = "1%"
                };
            }
            else if (itemText.Contains("Resistor 100Ω"))
            {
                component = new ResistorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Resistor),
                    X = 200,
                    Y = 200,
                    Value = "100Ω",
                    Power = "2W",
                    Tolerance = "1%"
                };
            }
            else if (itemText.Contains("Resistor 1kΩ"))
            {
                component = new ResistorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Resistor),
                    X = 200,
                    Y = 200,
                    Value = "1kΩ",
                    Power = "1W",
                    Tolerance = "5%"
                };
            }
            else if (itemText.Contains("Capacitor 100µF"))
            {
                component = new CapacitorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Capacitor),
                    X = 200,
                    Y = 200,
                    Capacitance = "100µF",
                    VoltageRating = "25V",
                    CapacitorType = CapacitorType.Ceramic,
                    ESR = "<5mΩ"
                };
            }
            else if (itemText.Contains("Capacitor 47µF"))
            {
                component = new CapacitorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Capacitor),
                    X = 200,
                    Y = 200,
                    Capacitance = "47µF",
                    VoltageRating = "160V",
                    CapacitorType = CapacitorType.Electrolytic,
                    ESR = "<10mΩ"
                };
            }
            else if (itemText.Contains("Capacitor 10µF"))
            {
                component = new CapacitorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Capacitor),
                    X = 200,
                    Y = 200,
                    Capacitance = "10µF",
                    VoltageRating = "50V",
                    CapacitorType = CapacitorType.Ceramic,
                    ESR = "<3mΩ"
                };
            }
            else if (itemText.Contains("Inductor 100µH"))
            {
                component = new InductorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Inductor),
                    X = 200,
                    Y = 200,
                    Value = "100µH",
                    Isat = "5A",
                    DCR = "20mΩ"
                };
            }
            else if (itemText.Contains("Inductor 220µH"))
            {
                component = new InductorComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Inductor),
                    X = 200,
                    Y = 200,
                    Value = "220µH",
                    Isat = "3A",
                    DCR = "50mΩ"
                };
            }
            else if (itemText.Contains("Diode 1N4148"))
            {
                component = new DiodeComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Diode),
                    X = 200,
                    Y = 200,
                    PartNumber = "1N4148",
                    VoltageRating = "100V",
                    CurrentRating = "200mA",
                    DiodeType = "Fast"
                };
            }
            else if (itemText.Contains("Diode Schottky"))
            {
                component = new DiodeComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Diode),
                    X = 200,
                    Y = 200,
                    PartNumber = "MBRS360",
                    VoltageRating = "60V",
                    CurrentRating = "3A",
                    DiodeType = "Schottky"
                };
            }
            else if (itemText.Contains("MOSFET N-Channel"))
            {
                component = new MosfetComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Mosfet),
                    X = 200,
                    Y = 200,
                    PartNumber = "IRF540",
                    ChannelType = "N-Channel",
                    VDS = "100V",
                    ID = "28A",
                    RDSon = "44mΩ"
                };
            }
            else if (itemText.Contains("MOSFET P-Channel"))
            {
                component = new MosfetComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Mosfet),
                    X = 200,
                    Y = 200,
                    PartNumber = "IRF9540",
                    ChannelType = "P-Channel",
                    VDS = "100V",
                    ID = "23A",
                    RDSon = "117mΩ"
                };
            }
            else if (itemText.Contains("IC TL494"))
            {
                component = new ICComponent
                {
                    Name = document.GetNextComponentName(ComponentType.IC),
                    X = 200,
                    Y = 200,
                    PartNumber = "TL494",
                    Function = "PWM Controller",
                    SupplyVoltage = "12V",
                    PinCount = 16
                };
            }
            else if (itemText.Contains("IC LM358"))
            {
                component = new ICComponent
                {
                    Name = document.GetNextComponentName(ComponentType.IC),
                    X = 200,
                    Y = 200,
                    PartNumber = "LM358",
                    Function = "Dual Op-Amp",
                    SupplyVoltage = "5V",
                    PinCount = 8
                };
            }
            else if (itemText.Contains("Wire (Power)"))
            {
                component = new WireComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Wire),
                    X1 = 200,
                    Y1 = 200,
                    X2 = 300,
                    Y2 = 200,
                    WireColor = Color.Red,
                    WireType = "Power",
                    Thickness = 3
                };
            }
            else if (itemText.Contains("Wire (Signal)"))
            {
                component = new WireComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Wire),
                    X1 = 200,
                    Y1 = 200,
                    X2 = 300,
                    Y2 = 200,
                    WireColor = Color.Black,
                    WireType = "Signal",
                    Thickness = 2.5f
                };
            }
            else if (itemText.Contains("Label"))
            {
                component = new LabelComponent
                {
                    Name = document.GetNextComponentName(ComponentType.Label),
                    X = 200,
                    Y = 200,
                    Text = "New Label",
                    FontSize = 10,
                    FontName = "Arial",
                    TextColor = Color.Black
                };
            }

            // IMPORTANTE: Actualizar bounds del componente
            if (component != null)
            {
                component.Move(0, 0);
            }

            return component;
        }

        private void PopulateFromCalculation()
        {
            int y = 200;

            // Terminales
            var vinTerminal = new TerminalComponent
            {
                Name = "VIN",
                X = 100,
                Y = y,
                Voltage = $"{parameters.InputVoltageMax:F0}V",
                TerminalColor = Color.Red
            };
            vinTerminal.Move(0, 0); // Forzar actualización de bounds
            document.AddComponent(vinTerminal);

            var voutTerminal = new TerminalComponent
            {
                Name = "VOUT",
                X = 1400,
                Y = y,
                Voltage = $"{parameters.OutputVoltage:F0}V",
                TerminalColor = Color.Blue
            };
            voutTerminal.Move(0, 0);
            document.AddComponent(voutTerminal);

            // Capacitor de entrada
            var cin = new CapacitorComponent
            {
                Name = "Cin1",
                X = 250,
                Y = y,
                Capacitance = "47µF",
                VoltageRating = "160V",
                CapacitorType = CapacitorType.Electrolytic
            };
            cin.Move(0, 0);
            document.AddComponent(cin);

            // Inductor
            var inductor = new InductorComponent
            {
                Name = "L1",
                X = 600,
                Y = y,
                Value = $"{results.SelectedInductance * 1e6:F0}µH",
                Isat = $"{results.PeakInductorCurrent:F1}A",
                DCR = "20mΩ"
            };
            inductor.Move(0, 0);
            document.AddComponent(inductor);

            // Capacitores de salida
            var cout1 = new CapacitorComponent
            {
                Name = "Cout1",
                X = 1000,
                Y = y,
                Capacitance = "100µF",
                VoltageRating = "25V",
                CapacitorType = CapacitorType.Ceramic
            };
            cout1.Move(0, 0);
            document.AddComponent(cout1);

            var cout2 = new CapacitorComponent
            {
                Name = "Cout2",
                X = 1100,
                Y = y,
                Capacitance = "100µF",
                VoltageRating = "25V",
                CapacitorType = CapacitorType.Ceramic
            };
            cout2.Move(0, 0);
            document.AddComponent(cout2);

            // Resistor de carga
            var rload = new ResistorComponent
            {
                Name = "RL",
                X = 1500,
                Y = y,
                Value = $"{parameters.OutputVoltage / parameters.OutputCurrent:F2}Ω",
                Power = $"{(parameters.OutputVoltage * parameters.OutputCurrent):F1}W",
                Orientation = ComponentOrientation.Vertical
            };
            rload.Move(0, 0);
            document.AddComponent(rload);

            // Wires básicos
            var wire1 = new WireComponent
            {
                Name = "W1",
                X1 = vinTerminal.X + 20,
                Y1 = vinTerminal.Y,
                X2 = cin.X - 30,
                Y2 = cin.Y,
                WireColor = Color.Red,
                Thickness = 3,
                WireType = "Power"
            };
            wire1.Move(0, 0);
            document.AddComponent(wire1);

            var wire2 = new WireComponent
            {
                Name = "W2",
                X1 = cin.X + 30,
                Y1 = cin.Y,
                X2 = inductor.X,
                Y2 = inductor.Y,
                WireColor = Color.Red,
                Thickness = 3,
                WireType = "Power"
            };
            wire2.Move(0, 0);
            document.AddComponent(wire2);

            // Labels
            var titleLabel = new LabelComponent
            {
                Name = "Title",
                X = 600,
                Y = 50,
                Text = $"Buck Converter: {parameters.InputVoltageMax}V → {parameters.OutputVoltage}V @ {parameters.OutputCurrent}A",
                FontSize = 16,
                IsBold = true,
                TextColor = Color.DarkBlue
            };
            titleLabel.Move(0, 0);
            document.AddComponent(titleLabel);

            document.IsDirty = false;

            // Forzar repintado después de que todo esté listo
            canvas.Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            // Aplicar transformación de zoom
            g.ScaleTransform(zoomFactor, zoomFactor);

            // Dibujar grid
            if (showGrid)
            {
                DrawGrid(g);
            }

            // Dibujar componentes
            foreach (var component in document.Components)
            {
                bool isHovered = component == hoveredComponent;
                component.Draw(g, component.IsSelected, isHovered);
            }

            // Dibujar vista previa del wire
            if (isWiring)
            {
                DrawWirePreview(g);
            }

            // Dibujar recuadro de selección rectangular
            if (isSelectingRectangle)
            {
                using (Pen selectionPen = new Pen(Color.Blue, 1 / zoomFactor))
                {
                    selectionPen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(selectionPen, selectionRectangle);
                }
            }
        }

        private void DrawGrid(Graphics g)
        {
            using (Pen gridPen = new Pen(Color.FromArgb(30, 200, 200, 200), 1))
            {
                gridPen.DashStyle = DashStyle.Dot;

                for (int x = 0; x < document.CanvasWidth; x += gridSize)
                {
                    g.DrawLine(gridPen, x, 0, x, document.CanvasHeight);
                }

                for (int y = 0; y < document.CanvasHeight; y += gridSize)
                {
                    g.DrawLine(gridPen, 0, y, document.CanvasWidth, y);
                }
            }
        }

        private Point SnapToGrid(Point p)
        {
            if (!snapToGrid) return p;
            return new Point(
                (int)Math.Round((double)p.X / gridSize) * gridSize,
                (int)Math.Round((double)p.Y / gridSize) * gridSize
            );
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            // Convertir coordenadas del mouse al espacio del canvas (sin zoom)
            Point canvasPoint = new Point(
                (int)(e.X / zoomFactor),
                (int)(e.Y / zoomFactor)
            );

            if (e.Button == MouseButtons.Left)
            {
                // Modo cableado activo
                if (isWiring)
                {
                    HandleWireClick(canvasPoint);
                    canvas.Invalidate();
                    return;
                }

                // Modo wire tool seleccionado
                if (currentMode == EditorMode.AddWire)
                {
                    // Iniciar cableado desde cualquier punto (no requiere pin)
                    var pinClicked = FindPinAtPoint(canvasPoint);
                    if (pinClicked.component != null && pinClicked.pin != null)
                    {
                        StartWiring(pinClicked.pin, pinClicked.component);
                    }
                    else
                    {
                        // Iniciar desde punto libre
                        StartWiring(null, null);
                        wireStartPoint = SnapToGrid(canvasPoint);
                        wirePoints[0] = wireStartPoint;
                    }
                    statusLabel.Text = "Routing wire... Click to add waypoints, right-click to cancel, double-click to finish";
                    canvas.Invalidate();
                    return;
                }

                if (currentMode == EditorMode.Select)
                {
                    // Verificar si se hizo clic en un pin para iniciar cableado
                    var pinClicked = FindPinAtPoint(canvasPoint);
                    if (pinClicked.component != null && pinClicked.pin != null)
                    {
                        StartWiring(pinClicked.pin, pinClicked.component);
                        currentMode = EditorMode.AddWire;
                        statusLabel.Text = "Routing wire... Click to add waypoints, right-click to cancel, double-click to finish";
                        canvas.Invalidate();
                        return;
                    }

                    // Verificar si se hizo clic en un wire para seleccionarlo
                    var clickedWire = FindWireAtPoint(canvasPoint);
                    if (clickedWire != null)
                    {
                        if (!ModifierKeys.HasFlag(Keys.Control))
                        {
                            document.ClearSelection();
                        }
                        clickedWire.IsSelected = true;
                        propertyGrid.SelectedObject = clickedWire;
                        canvas.Invalidate();
                        return;
                    }

                    var clicked = document.SelectComponentAt(canvasPoint,
                        ModifierKeys.HasFlag(Keys.Control));

                    if (clicked != null)
                    {
                        isDragging = true;
                        dragStartPoint = canvasPoint;
                        propertyGrid.SelectedObject = clicked;
                    }
                    else
                    {
                        // Inicio de selección rectangular
                        isDragging = false;
                        isSelectingRectangle = true;
                        dragStartPoint = canvasPoint;
                        selectionRectangle = new Rectangle(canvasPoint, new Size(0, 0));
                        document.ClearSelection();
                    }
                }
                else
                {
                    // Agregar componente
                    Point snappedPoint = SnapToGrid(canvasPoint);
                    AddComponentAt(snappedPoint);
                }

                canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                panStartPoint = e.Location;
                scrollStartPoint = new Point(
                    canvasPanel.HorizontalScroll.Value,
                    canvasPanel.VerticalScroll.Value
                );
                canvas.Cursor = Cursors.Hand;
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Cancelar cableado con clic derecho
                if (isWiring)
                {
                    CancelWiring();
                    canvas.Invalidate();
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Convertir coordenadas
            Point canvasPoint = new Point(
                (int)(e.X / zoomFactor),
                (int)(e.Y / zoomFactor)
            );

            if (isPanning)
            {
                int deltaX = panStartPoint.X - e.Location.X;
                int deltaY = panStartPoint.Y - e.Location.Y;

                canvasPanel.AutoScrollPosition = new Point(
                    Math.Max(0, scrollStartPoint.X + deltaX),
                    Math.Max(0, scrollStartPoint.Y + deltaY)
                );
            }
            else if (isSelectingRectangle)
            {
                // Actualizar rectángulo de selección
                int x = Math.Min(dragStartPoint.X, canvasPoint.X);
                int y = Math.Min(dragStartPoint.Y, canvasPoint.Y);
                int width = Math.Abs(canvasPoint.X - dragStartPoint.X);
                int height = Math.Abs(canvasPoint.Y - dragStartPoint.Y);
                selectionRectangle = new Rectangle(x, y, width, height);

                // Seleccionar componentes dentro del rectángulo
                foreach (var component in document.Components)
                {
                    component.IsSelected = selectionRectangle.IntersectsWith(component.Bounds);
                }

                canvas.Invalidate();
            }
            else if (isDragging && currentMode == EditorMode.Select)
            {
                var selected = document.GetSelectedComponents();
                if (selected.Count > 0)
                {
                    int deltaX = canvasPoint.X - dragStartPoint.X;
                    int deltaY = canvasPoint.Y - dragStartPoint.Y;

                    // Aplicar snap to grid
                    if (snapToGrid)
                    {
                        Point newPos = SnapToGrid(new Point(
                            selected[0].X + deltaX,
                            selected[0].Y + deltaY
                        ));
                        deltaX = newPos.X - selected[0].X;
                        deltaY = newPos.Y - selected[0].Y;
                    }

                    document.MoveSelected(deltaX, deltaY);
                    dragStartPoint = new Point(
                        selected[0].X,
                        selected[0].Y
                    );
                    canvas.Invalidate();
                }
            }
            else if (isWiring)
            {
                // Actualizar posición actual del cable
                wireCurrentPoint = canvasPoint;
                canvas.Invalidate();
            }
            else
            {
                // Hover detection
                SchematicComponent hovered = null;
                for (int i = document.Components.Count - 1; i >= 0; i--)
                {
                    if (document.Components[i].HitTest(canvasPoint))
                    {
                        hovered = document.Components[i];
                        break;
                    }
                }

                if (hovered != hoveredComponent)
                {
                    hoveredComponent = hovered;
                    canvas.Invalidate();
                }

                canvas.Cursor = hovered != null ? Cursors.Hand : Cursors.Default;

                // Cambiar cursor si está sobre un pin
                var pinHover = FindPinAtPoint(canvasPoint);
                if (pinHover.component != null && pinHover.pin != null)
                {
                    canvas.Cursor = Cursors.Cross;
                }
            }

            // Update status
            if (isWiring)
            {
                statusLabel.Text = $"Routing wire... Points: {wirePoints.Count} | " +
                                  $"Right-click to cancel | Double-click to finish";
            }
            else
            {
                string snapStatus = snapToGrid ? "ON" : "OFF";
                string gridStatus = showGrid ? "ON" : "OFF";
                statusLabel.Text = $"Position: ({canvasPoint.X}, {canvasPoint.Y}) | " +
                                  $"Components: {document.Components.Count} | " +
                                  $"Selected: {document.GetSelectedComponents().Count} | " +
                                  $"Snap: {snapStatus} | Grid: {gridStatus}";
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                isSelectingRectangle = false;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                isPanning = false;
                canvas.Cursor = Cursors.Default;
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point canvasPoint = new Point(
                    (int)(e.X / zoomFactor),
                    (int)(e.Y / zoomFactor)
                );
                ShowContextMenu(canvasPoint);
            }
        }

        private void Canvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isWiring)
            {
                // Finalizar wire en punto libre (sin conectar a pin)
                Point canvasPoint = new Point(
                    (int)(e.X / zoomFactor),
                    (int)(e.Y / zoomFactor)
                );

                Point snappedPoint = SnapToGrid(canvasPoint);

                // Verificar si hay un pin en el punto final
                var endPin = FindPinAtPoint(canvasPoint);

                if (endPin.component != null && endPin.pin != null)
                {
                    // Terminar en pin
                    FinishWiring(endPin.pin, endPin.component);
                }
                else
                {
                    // Terminar en punto libre
                    wirePoints.Add(snappedPoint);
                    FinishWiring(null, null);
                }

                statusLabel.Text = "Wire created";
                canvas.Invalidate();
            }
        }

        private void AddComponentAt(Point location)
        {
            SchematicComponent newComponent = null;

            switch (currentMode)
            {
                case EditorMode.AddResistor:
                    newComponent = new ResistorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Resistor),
                        X = location.X,
                        Y = location.Y
                    };
                    break;

                case EditorMode.AddCapacitor:
                    newComponent = new CapacitorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Capacitor),
                        X = location.X,
                        Y = location.Y
                    };
                    break;

                case EditorMode.AddInductor:
                    newComponent = new InductorComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Inductor),
                        X = location.X,
                        Y = location.Y
                    };
                    break;

                case EditorMode.AddLabel:
                    newComponent = new LabelComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Label),
                        X = location.X,
                        Y = location.Y,
                        Text = "New Label"
                    };
                    break;

                case EditorMode.AddTerminal:
                    newComponent = new TerminalComponent
                    {
                        Name = document.GetNextComponentName(ComponentType.Terminal),
                        X = location.X,
                        Y = location.Y
                    };
                    break;
            }

            if (newComponent != null)
            {
                document.AddComponent(newComponent);
                document.ClearSelection();
                newComponent.IsSelected = true;
                propertyGrid.SelectedObject = newComponent;
                canvas.Invalidate();

                currentMode = EditorMode.Select;
                UpdateToolbarButtons();
            }
        }

        private void ShowContextMenu(Point location)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Copy", null, OnEditCopy);
            contextMenu.Items.Add("Paste", null, OnEditPaste);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Delete", null, OnEditDelete);
            contextMenu.Items.Add("Duplicate", null, OnEditDuplicate);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Rotate 90° (R)", null, (s, e) => RotateSelectedComponents());
            contextMenu.Items.Add("Mirror Horizontal (H)", null, (s, e) => MirrorHorizontalSelectedComponents());
            contextMenu.Items.Add("Mirror Vertical (V)", null, (s, e) => MirrorVerticalSelectedComponents());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Properties", null, (s, e) => propertyGrid.Focus());

            // Convertir coordenadas de canvas a screen
            Point screenPoint = canvas.PointToScreen(new Point(
                (int)(location.X * zoomFactor),
                (int)(location.Y * zoomFactor)
            ));
            contextMenu.Show(screenPoint);
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            document.IsDirty = true;

            if (e.ChangedItem != null)
            {
                string propertyName = e.ChangedItem.Label;
                if (propertyName == "Orientation" || propertyName == "X" || propertyName == "Y" ||
                    propertyName == "X1" || propertyName == "Y1" || propertyName == "X2" || propertyName == "Y2" ||
                    propertyName == "Length" || propertyName == "Width")
                {
                    var selected = document.GetSelectedComponents();
                    foreach (var component in selected)
                    {
                        component.Move(0, 0);
                    }
                }
            }

            canvas.Invalidate();
        }

        private void SchematicEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                OnEditDelete(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                OnEditCopy(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                OnEditPaste(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                OnEditDuplicate(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                OnFileSave(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.R)  // R para rotar
            {
                RotateSelectedComponents();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.H)  // H para espejo horizontal
            {
                MirrorHorizontalSelectedComponents();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.V && !e.Control)  // V para espejo vertical (sin Ctrl)
            {
                MirrorVerticalSelectedComponents();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                OnEditSelectAll(sender, e);
                e.Handled = true;
            }
        }

        private void RotateSelectedComponents()
        {
            var selected = document.GetSelectedComponents();
            if (selected.Count == 0)
            {
                statusLabel.Text = "No components selected to rotate";
                return;
            }

            foreach (var component in selected)
            {
                component.RotateClockwise();
            }

            canvas.Invalidate();
            propertyGrid.Refresh();
            statusLabel.Text = $"Rotated {selected.Count} component(s) 90° clockwise";
        }

        private void MirrorHorizontalSelectedComponents()
        {
            var selected = document.GetSelectedComponents();
            if (selected.Count == 0)
            {
                statusLabel.Text = "No components selected to mirror";
                return;
            }

            foreach (var component in selected)
            {
                component.ToggleMirrorHorizontal();
            }

            canvas.Invalidate();
            propertyGrid.Refresh();
            statusLabel.Text = $"Mirrored {selected.Count} component(s) horizontally";
        }

        private void MirrorVerticalSelectedComponents()
        {
            var selected = document.GetSelectedComponents();
            if (selected.Count == 0)
            {
                statusLabel.Text = "No components selected to mirror";
                return;
            }

            foreach (var component in selected)
            {
                component.ToggleMirrorVertical();
            }

            canvas.Invalidate();
            propertyGrid.Refresh();
            statusLabel.Text = $"Mirrored {selected.Count} component(s) vertically";
        }

        // Event Handlers
        private void OnFileNew(object sender, EventArgs e)
        {
            if (ConfirmDiscard())
            {
                document = new SchematicDocument();
                canvas.Invalidate();
            }
        }

        private void OnFileOpen(object sender, EventArgs e)
        {
            if (ConfirmDiscard())
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Schematic Files (*.json)|*.json|All Files (*.*)|*.*";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            document = SchematicDocument.LoadFromFile(dialog.FileName);
                            canvas.Invalidate();
                            statusLabel.Text = "Schematic loaded successfully";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error loading file: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void OnFileSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(document.FilePath))
            {
                OnFileSaveAs(sender, e);
            }
            else
            {
                SaveDocument();
            }
        }

        private void OnFileSaveAs(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Schematic Files (*.json)|*.json|All Files (*.*)|*.*";
                dialog.DefaultExt = "json";
                dialog.FileName = document.Name + ".json";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    document.FilePath = dialog.FileName;
                    SaveDocument();
                }
            }
        }

        private void SaveDocument()
        {
            try
            {
                document.SaveToFile(document.FilePath);
                statusLabel.Text = "Schematic saved successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnExportPNG(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG Image (*.png)|*.png";
                dialog.DefaultExt = "png";
                dialog.FileName = document.Name + ".png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(document.CanvasWidth, document.CanvasHeight);
                    canvas.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(dialog.FileName);
                    statusLabel.Text = "Image exported successfully";
                }
            }
        }

        private void OnEditCopy(object sender, EventArgs e)
        {
            clipboard.Clear();
            var selected = document.GetSelectedComponents();
            foreach (var component in selected)
            {
                clipboard.Add(component.Clone());
            }
            statusLabel.Text = $"Copied {clipboard.Count} component(s) to clipboard";
        }

        private void OnEditPaste(object sender, EventArgs e)
        {
            if (clipboard.Count == 0)
            {
                statusLabel.Text = "Clipboard is empty";
                return;
            }

            document.ClearSelection();
            foreach (var component in clipboard)
            {
                var clone = component.Clone();
                clone.Move(50, 50); // Offset para ver el pegado
                document.AddComponent(clone);
                clone.IsSelected = true;
            }
            canvas.Invalidate();
            statusLabel.Text = $"Pasted {clipboard.Count} component(s)";
        }

        private void OnEditDelete(object sender, EventArgs e)
        {
            int count = document.GetSelectedComponents().Count;
            document.DeleteSelected();
            propertyGrid.SelectedObject = null;
            canvas.Invalidate();
            statusLabel.Text = $"Deleted {count} component(s)";
        }

        private void OnEditDuplicate(object sender, EventArgs e)
        {
            int count = document.GetSelectedComponents().Count;
            document.DuplicateSelected();
            canvas.Invalidate();
            statusLabel.Text = $"Duplicated {count} component(s)";
        }

        private void OnEditSelectAll(object sender, EventArgs e)
        {
            foreach (var component in document.Components)
            {
                component.IsSelected = true;
            }
            canvas.Invalidate();
            statusLabel.Text = $"Selected all {document.Components.Count} components";
        }

        private void OnViewZoomIn(object sender, EventArgs e)
        {
            zoomFactor = Math.Min(4.0f, zoomFactor * 1.25f);
            UpdateZoom();
        }

        private void OnViewZoomOut(object sender, EventArgs e)
        {
            zoomFactor = Math.Max(0.25f, zoomFactor / 1.25f);
            UpdateZoom();
        }

        private void OnViewZoom100(object sender, EventArgs e)
        {
            zoomFactor = 1.0f;
            UpdateZoom();
        }

        private void OnViewToggleGrid(object sender, EventArgs e)
        {
            showGrid = !showGrid;
            canvas.Invalidate();
        }

        private void OnViewToggleSnap(object sender, EventArgs e)
        {
            snapToGrid = !snapToGrid;
            statusLabel.Text = $"Snap to Grid: {(snapToGrid ? "ON" : "OFF")}";
        }

        private void UpdateZoom()
        {
            canvas.Size = new Size(
                (int)(document.CanvasWidth * zoomFactor),
                (int)(document.CanvasHeight * zoomFactor)
            );
            canvasPanel.AutoScrollMinSize = canvas.Size;
            canvas.Invalidate();

            // Actualizar status bar
            if (statusStrip.Items.Count >= 3)
            {
                statusStrip.Items[2].Text = $"Zoom: {zoomFactor * 100:F0}%";
            }
        }

        private void OnRotate90(object sender, EventArgs e)
        {
            var selected = document.GetSelectedComponents();
            foreach (var component in selected)
            {
                if (component.Orientation == ComponentOrientation.Horizontal)
                    component.Orientation = ComponentOrientation.Vertical;
                else
                    component.Orientation = ComponentOrientation.Horizontal;

                component.Move(0, 0); // Actualizar bounds
            }
            canvas.Invalidate();
            statusLabel.Text = $"Rotated {selected.Count} component(s)";
        }

        private enum AlignmentType
        {
            Left,
            Right,
            Top,
            Bottom,
            CenterH,
            CenterV
        }

        private void AlignComponents(AlignmentType alignment)
        {
            var selected = document.GetSelectedComponents();
            if (selected.Count < 2)
            {
                statusLabel.Text = "Select at least 2 components to align";
                return;
            }

            int referenceValue = 0;

            switch (alignment)
            {
                case AlignmentType.Left:
                    referenceValue = selected.Min(c => c.X);
                    foreach (var c in selected)
                    {
                        c.X = referenceValue;
                        c.Move(0, 0);
                    }
                    break;

                case AlignmentType.Right:
                    referenceValue = selected.Max(c => c.X);
                    foreach (var c in selected)
                    {
                        c.X = referenceValue;
                        c.Move(0, 0);
                    }
                    break;

                case AlignmentType.Top:
                    referenceValue = selected.Min(c => c.Y);
                    foreach (var c in selected)
                    {
                        c.Y = referenceValue;
                        c.Move(0, 0);
                    }
                    break;

                case AlignmentType.Bottom:
                    referenceValue = selected.Max(c => c.Y);
                    foreach (var c in selected)
                    {
                        c.Y = referenceValue;
                        c.Move(0, 0);
                    }
                    break;
            }

            canvas.Invalidate();
            statusLabel.Text = $"Aligned {selected.Count} components to {alignment}";
        }

        private void DistributeComponents(bool horizontal)
        {
            var selected = document.GetSelectedComponents();
            if (selected.Count < 3)
            {
                statusLabel.Text = "Select at least 3 components to distribute";
                return;
            }

            var sorted = horizontal
                ? selected.OrderBy(c => c.X).ToList()
                : selected.OrderBy(c => c.Y).ToList();

            int start = horizontal ? sorted.First().X : sorted.First().Y;
            int end = horizontal ? sorted.Last().X : sorted.Last().Y;
            int spacing = (end - start) / (sorted.Count - 1);

            for (int i = 0; i < sorted.Count; i++)
            {
                if (horizontal)
                    sorted[i].X = start + i * spacing;
                else
                    sorted[i].Y = start + i * spacing;

                sorted[i].Move(0, 0);
            }

            canvas.Invalidate();
            statusLabel.Text = $"Distributed {selected.Count} components {(horizontal ? "horizontally" : "vertically")}";
        }

        // Tool handlers
        private void OnToolSelect(object sender, EventArgs e)
        {
            currentMode = EditorMode.Select;
            UpdateToolbarButtons();
        }

        private void OnToolResistor(object sender, EventArgs e)
        {
            currentMode = EditorMode.AddResistor;
            UpdateToolbarButtons();
        }

        private void OnToolCapacitor(object sender, EventArgs e)
        {
            currentMode = EditorMode.AddCapacitor;
            UpdateToolbarButtons();
        }

        private void OnToolInductor(object sender, EventArgs e)
        {
            currentMode = EditorMode.AddInductor;
            UpdateToolbarButtons();
        }

        private void OnToolWire(object sender, EventArgs e)
        {
            currentMode = EditorMode.AddWire;
            UpdateToolbarButtons();
        }

        private void OnToolLabel(object sender, EventArgs e)
        {
            currentMode = EditorMode.AddLabel;
            UpdateToolbarButtons();
        }

        private void UpdateToolbarButtons()
        {
            for (int i = 0; i < 7 && i < toolStrip.Items.Count; i++)
            {
                if (toolStrip.Items[i] is ToolStripButton btn)
                {
                    btn.Checked = i == (int)currentMode;
                }
            }
        }

        private bool ConfirmDiscard()
        {
            if (document.IsDirty)
            {
                var result = MessageBox.Show(
                    "Do you want to save changes to the current schematic?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    OnFileSave(this, EventArgs.Empty);
                    return !document.IsDirty;
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!ConfirmDiscard())
            {
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        #region Wire Routing Methods

        /// <summary>
        /// Encuentra un pin en la posición especificada
        /// </summary>
        private (SchematicComponent component, ComponentPin pin) FindPinAtPoint(Point point)
        {
            const int PIN_TOLERANCE = 8;

            foreach (var component in document.Components)
            {
                List<ComponentPin> pins = null;

                // Obtener pines según el tipo de componente
                if (component is ResistorComponent resistor)
                    pins = resistor.Pins;
                else if (component is CapacitorComponent capacitor)
                    pins = capacitor.Pins;
                else if (component is InductorComponent inductor)
                    pins = inductor.Pins;
                else if (component is DiodeComponent diode)
                    pins = diode.Pins;
                else if (component is MosfetComponent mosfet)
                    pins = mosfet.Pins;
                else if (component is BJTComponent bjt)
                    pins = bjt.Pins;
                else if (component is FuseComponent fuse)
                    pins = fuse.Pins;
                else if (component is ICComponent ic)
                    pins = ic.Pins;
                else if (component is TerminalComponent terminal)
                    pins = terminal.Pins;
                else if (component is GroundComponent ground)
                    pins = ground.Pins;
                else if (component is VccSupplyComponent vcc)
                    pins = vcc.Pins;
                else if (component is NodeComponent node)
                    pins = node.Pins;
                // Nuevos componentes (primeros 8)
                else if (component is BuzzerComponent buzzer)
                    pins = buzzer.Pins;
                else if (component is BridgeDiodeComponent bridge)
                    pins = bridge.Pins;
                else if (component is SchottkyDiodeComponent schottky)
                    pins = schottky.Pins;
                else if (component is ZenerDiodeComponent zener)
                    pins = zener.Pins;
                else if (component is LEDComponent led)
                    pins = led.Pins;
                else if (component is PotentiometerComponent pot)
                    pins = pot.Pins;
                else if (component is BatteryComponent battery)
                    pins = battery.Pins;
                else if (component is TransformerComponent transformer)
                    pins = transformer.Pins;
                // Componentes adicionales
                else if (component is OpAmpComponent opamp)
                    pins = opamp.Pins;
                else if (component is LogicGateComponent gate)
                    pins = gate.Pins;
                else if (component is FlipFlopComponent ff)
                    pins = ff.Pins;
                else if (component is InverterComponent inv)
                    pins = inv.Pins;
                else if (component is LED7SegmentComponent led7)
                    pins = led7.Pins;
                else if (component is OptocouplerComponent opto)
                    pins = opto.Pins;
                else if (component is TriacComponent triac)
                    pins = triac.Pins;
                else if (component is ThyristorComponent scr)
                    pins = scr.Pins;

                // Verificar si algún pin coincide
                if (pins != null)
                {
                    foreach (var pin in pins)
                    {
                        int dx = Math.Abs(pin.Position.X - point.X);
                        int dy = Math.Abs(pin.Position.Y - point.Y);
                        if (dx <= PIN_TOLERANCE && dy <= PIN_TOLERANCE)
                        {
                            return (component, pin);
                        }
                    }
                }
            }

            return (null, null);
        }

        /// <summary>
        /// Encuentra un wire en la posición especificada
        /// </summary>
        private WireComponent FindWireAtPoint(Point point)
        {
            const int WIRE_TOLERANCE = 5;

            foreach (var component in document.Components)
            {
                if (component is WireComponent wire)
                {
                    // Calcular distancia del punto a la línea
                    double distance = DistancePointToLine(point,
                        new Point(wire.X1, wire.Y1),
                        new Point(wire.X2, wire.Y2));

                    if (distance <= WIRE_TOLERANCE)
                    {
                        return wire;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Calcula la distancia de un punto a una línea
        /// </summary>
        private double DistancePointToLine(Point point, Point lineStart, Point lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
            {
                // Línea degenerada (punto)
                dx = point.X - lineStart.X;
                dy = point.Y - lineStart.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            double projX = lineStart.X + t * dx;
            double projY = lineStart.Y + t * dy;

            dx = point.X - projX;
            dy = point.Y - projY;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Inicia el proceso de cableado desde un pin
        /// </summary>
        private void StartWiring(ComponentPin pin, SchematicComponent component)
        {
            isWiring = true;
            wireStartPin = pin;
            wireStartPoint = pin.Position;
            wireCurrentPoint = pin.Position;
            wirePoints.Clear();
            wirePoints.Add(pin.Position);
        }

        /// <summary>
        /// Maneja el clic durante el cableado
        /// </summary>
        private void HandleWireClick(Point point)
        {
            if (!isWiring)
            {
                // Iniciar nuevo wire
                var pinClicked = FindPinAtPoint(point);
                if (pinClicked.component != null && pinClicked.pin != null)
                {
                    StartWiring(pinClicked.pin, pinClicked.component);
                }
                else
                {
                    StartWiring(null, null);
                    wireStartPoint = SnapToGrid(point);
                    wirePoints[0] = wireStartPoint;
                }
                return;
            }

            // Wire en progreso - agregar waypoint o finalizar
            Point snappedPoint = SnapToGrid(point);

            // Verificar si se hizo clic en un pin para terminar
            var endPin = FindPinAtPoint(point);
            if (endPin.component != null && endPin.pin != null)
            {
                FinishWiring(endPin.pin, endPin.component);
                return;
            }

            // Agregar waypoint
            wirePoints.Add(snappedPoint);
            statusLabel.Text = $"Wire waypoint added. Points: {wirePoints.Count} | Double-click to finish";
        }

        /// <summary>
        /// Finaliza el cableado y crea los segmentos de wire
        /// </summary>
        private void FinishWiring(ComponentPin endPin, SchematicComponent endComponent)
        {
            if (!isWiring || wirePoints.Count == 0) return;

            // Agregar punto final (puede ser pin o punto libre)
            if (endPin != null)
            {
                wirePoints.Add(endPin.Position);
            }

            // Crear segmentos de wire según el modo de ruteo
            if (wirePoints.Count >= 2)
            {
                if (wireRoutingMode == WireRoutingMode.Orthogonal)
                {
                    CreateOrthogonalWires();
                }
                else
                {
                    CreateDirectWire();
                }
            }

            // Limpiar estado
            isWiring = false;
            wireStartPin = null;
            wirePoints.Clear();
            currentMode = EditorMode.Select;
            statusLabel.Text = "Wire created successfully";
            canvas.Invalidate();
        }

        /// <summary>
        /// Crea wires ortogonales (Manhattan style)
        /// </summary>
        private void CreateOrthogonalWires()
        {
            for (int i = 0; i < wirePoints.Count - 1; i++)
            {
                Point p1 = wirePoints[i];
                Point p2 = wirePoints[i + 1];

                // Si es el último segmento, ir directo al pin
                if (i == wirePoints.Count - 2)
                {
                    CreateWireSegment(p1, p2);
                }
                else
                {
                    // Crear segmentos ortogonales
                    Point intermediate;

                    // Decidir si ir horizontal primero o vertical
                    int dx = Math.Abs(p2.X - p1.X);
                    int dy = Math.Abs(p2.Y - p1.Y);

                    if (dx > dy)
                    {
                        // Horizontal primero
                        intermediate = new Point(p2.X, p1.Y);
                        CreateWireSegment(p1, intermediate);
                        if (p1.Y != p2.Y)
                        {
                            CreateWireSegment(intermediate, p2);
                        }
                    }
                    else
                    {
                        // Vertical primero
                        intermediate = new Point(p1.X, p2.Y);
                        CreateWireSegment(p1, intermediate);
                        if (p1.X != p2.X)
                        {
                            CreateWireSegment(intermediate, p2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Crea un wire directo
        /// </summary>
        private void CreateDirectWire()
        {
            if (wirePoints.Count < 2) return;

            Point start = wirePoints[0];
            Point end = wirePoints[wirePoints.Count - 1];
            CreateWireSegment(start, end);
        }

        /// <summary>
        /// Crea un segmento de wire
        /// </summary>
        private void CreateWireSegment(Point p1, Point p2)
        {
            var wire = new WireComponent
            {
                Name = document.GetNextComponentName(ComponentType.Wire),
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y
            };

            document.AddComponent(wire);
        }

        /// <summary>
        /// Cancela el cableado en progreso
        /// </summary>
        private void CancelWiring()
        {
            isWiring = false;
            wireStartPin = null;
            wirePoints.Clear();
            currentMode = EditorMode.Select;
            statusLabel.Text = "Wire routing cancelled";
        }

        /// <summary>
        /// Dibuja la vista previa del wire durante el ruteo
        /// </summary>
        private void DrawWirePreview(Graphics g)
        {
            if (!isWiring || wirePoints.Count == 0) return;

            using (Pen previewPen = new Pen(Color.FromArgb(128, 0, 150, 255), 2))
            {
                previewPen.DashStyle = DashStyle.Dash;

                // Dibujar segmentos existentes
                for (int i = 0; i < wirePoints.Count - 1; i++)
                {
                    g.DrawLine(previewPen, wirePoints[i], wirePoints[i + 1]);
                }

                // Dibujar segmento actual (desde último punto hasta mouse)
                if (wirePoints.Count > 0)
                {
                    Point lastPoint = wirePoints[wirePoints.Count - 1];

                    if (wireRoutingMode == WireRoutingMode.Orthogonal)
                    {
                        // Dibujar vista previa ortogonal
                        int dx = Math.Abs(wireCurrentPoint.X - lastPoint.X);
                        int dy = Math.Abs(wireCurrentPoint.Y - lastPoint.Y);

                        Point intermediate;
                        if (dx > dy)
                        {
                            intermediate = new Point(wireCurrentPoint.X, lastPoint.Y);
                            g.DrawLine(previewPen, lastPoint, intermediate);
                            g.DrawLine(previewPen, intermediate, wireCurrentPoint);
                        }
                        else
                        {
                            intermediate = new Point(lastPoint.X, wireCurrentPoint.Y);
                            g.DrawLine(previewPen, lastPoint, intermediate);
                            g.DrawLine(previewPen, intermediate, wireCurrentPoint);
                        }
                    }
                    else
                    {
                        // Vista previa directa
                        g.DrawLine(previewPen, lastPoint, wireCurrentPoint);
                    }
                }

                // Dibujar puntos de waypoint
                using (Brush pointBrush = new SolidBrush(Color.Yellow))
                {
                    foreach (var point in wirePoints)
                    {
                        g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
                    }
                }
            }
        }

        #endregion

        // ============================================================================
        // DETECTOR INTELIGENTE DE PINES - VERSIÓN MEJORADA
        // Reemplazar el método DetectPinCountAdvanced completo
        // ============================================================================

        private int DetectPinCountAdvanced(Database.ElectronicComponent dbComponent)
        {
            LoggerService.Instance.Info("╔══════════════════════════════════════════════════════════╗", "PinDetection");
            LoggerService.Instance.Info("║        ADVANCED PIN DETECTION - INTELLIGENT SEARCH       ║", "PinDetection");
            LoggerService.Instance.Info("╚══════════════════════════════════════════════════════════╝", "PinDetection");

            string GetSpec(string key)
            {
                if (dbComponent.Specifications != null && dbComponent.Specifications.ContainsKey(key))
                    return dbComponent.Specifications[key];
                return null;
            }

            // ========================================================================
            // PASO 1: BUSCAR EN CAMPOS DIRECTOS DE SPECIFICATIONS
            // ========================================================================
            var directFields = new[]
            {
                "Number of Pins",
                "Pin Count",
                "PinCount",
                "Pins",
                "Number of I/O",
                "I/O",
                "Terminals",
                "Number of Terminals",
                "Pin/Package"
            };

            foreach (var field in directFields)
            {
                string value = GetSpec(field);
                if (!string.IsNullOrEmpty(value))
                {
                    LoggerService.Instance.Debug($"Checking field '{field}': '{value}'", "PinDetection");

                    // Extraer número del valor
                    var numberMatch = System.Text.RegularExpressions.Regex.Match(value, @"\d+");
                    if (numberMatch.Success && int.TryParse(numberMatch.Value, out int pins))
                    {
                        LoggerService.Instance.Info($"✅ PIN COUNT FOUND in Spec[{field}]: {pins} pins", "PinDetection");
                        return pins;
                    }
                }
            }

            // ========================================================================
            // PASO 2: ANALIZAR PACKAGE FIELD CON PATRONES AVANZADOS
            // ========================================================================
            var packageFields = new[] { "Package", "Package / Case", "Mounting Type", "Supplier Device Package" };

            foreach (var field in packageFields)
            {
                string package = GetSpec(field);
                if (!string.IsNullOrEmpty(package))
                {
                    LoggerService.Instance.Debug($"Analyzing package field '{field}': '{package}'", "PinDetection");

                    // Patrones de package más comunes
                    var packagePatterns = new[]
                    {
                        // Formato: "28-SOIC" o "SOIC-28" o "28SOIC"
                        @"(\d+)[-\s]*(TSSOP|SOIC|DIP|PDIP|MSOP|QFN|LQFP|PLCC|SOP|SO|SSOP|TQFP|VQFN|QFP|DFN|SON|WSON|VSON|MLF|BGA|WLCSP|TQFN|UQFN|VFQFN)",
                        @"(TSSOP|SOIC|DIP|PDIP|MSOP|QFN|LQFP|PLCC|SOP|SO|SSOP|TQFP|VQFN|QFP|DFN|SON|WSON|VSON|MLF|BGA|WLCSP|TQFN|UQFN|VFQFN)[-\s]*(\d+)",
                
                        // Formato: "TO-220-3" o "TO-92-3"
                        @"TO[-\s]*(\d+)[-\s]*(\d+)",  // TO-220-3 → captura ambos números
                
                        // Formato: "SOT-23-5" o "SC-70-6"
                        @"(SOT|SC)[-\s]*\d+[-\s]*(\d+)",
                
                        // Formato simple: "28-Pin"
                        @"(\d+)[-\s]*PIN",
                
                        // Formato: solo número si es único
                        @"^(\d+)$"
                    };

                    foreach (var pattern in packagePatterns)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(
                            package, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            // Buscar el grupo que contiene el número de pines
                            for (int i = 1; i < match.Groups.Count; i++)
                            {
                                if (int.TryParse(match.Groups[i].Value, out int pins) && pins > 0 && pins <= 500)
                                {
                                    // Validar que el número tiene sentido (entre 2 y 500 pines)
                                    if (pins >= 2)
                                    {
                                        LoggerService.Instance.Info($"✅ PIN COUNT FOUND in Package[{field}]: {pins} pins (pattern: {pattern})", "PinDetection");
                                        return pins;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // ========================================================================
            // PASO 3: ANALIZAR DESCRIPTION CON MÚLTIPLES PATRONES
            // ========================================================================
            if (!string.IsNullOrEmpty(dbComponent.Description))
            {
                LoggerService.Instance.Debug($"Analyzing Description: '{dbComponent.Description}'", "PinDetection");

                var descriptionPatterns = new[]
                {
                    // "2 CIRCUIT 8TSSOP" → 8 pines
                    @"(\d+)\s*CIRCUIT\s*(\d+)",
            
                    // "28-PDIP" o "28PDIP" o "PDIP-28"
                    @"(\d+)[-\s]*(TSSOP|SOIC|DIP|PDIP|MSOP|QFN|LQFP|PLCC|SOP|SO|SSOP|TQFP|VQFN|QFP|DFN|SON|WSON|VSON|MLF|BGA|WLCSP)",
                    @"(TSSOP|SOIC|DIP|PDIP|MSOP|QFN|LQFP|PLCC|SOP|SO|SSOP|TQFP|VQFN|QFP|DFN|SON|WSON|VSON|MLF|BGA|WLCSP)[-\s]*(\d+)",
            
                    // "28 Pin" o "28-Pin"
                    @"(\d+)[-\s]*PIN",
            
                    // "28 I/O" o "28 Input/Output"
                    @"(\d+)[-\s]*(I/O|INPUT/OUTPUT|INPUTS)",
            
                    // Packages específicos
                    @"TO[-\s]*\d+[-\s]*(\d+)",  // TO-220-3
                    @"SOT[-\s]*\d+[-\s]*(\d+)"  // SOT-23-5
                };

                foreach (var pattern in descriptionPatterns)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(
                        dbComponent.Description, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        // Para pattern "CIRCUIT", tomar el segundo número
                        if (pattern.Contains("CIRCUIT") && match.Groups.Count >= 3)
                        {
                            if (int.TryParse(match.Groups[2].Value, out int pins))
                            {
                                LoggerService.Instance.Info($"✅ PIN COUNT FOUND in Description (CIRCUIT pattern): {pins} pins", "PinDetection");
                                return pins;
                            }
                        }

                        // Para otros patrones, buscar cualquier número válido
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            if (int.TryParse(match.Groups[i].Value, out int pins) && pins >= 2 && pins <= 500)
                            {
                                LoggerService.Instance.Info($"✅ PIN COUNT FOUND in Description: {pins} pins (pattern: {pattern})", "PinDetection");
                                return pins;
                            }
                        }
                    }
                }
            }

            // ========================================================================
            // PASO 4: BUSCAR EN PART NUMBER (algunos part numbers incluyen el pin count)
            // ========================================================================
            if (!string.IsNullOrEmpty(dbComponent.PartNumber))
            {
                LoggerService.Instance.Debug($"Analyzing PartNumber: '{dbComponent.PartNumber}'", "PinDetection");

                // Patrones comunes en part numbers
                // Ejemplos: "ATMEGA328P-PU" (28 pines), "STM32F103C8T6" (48 pines)

                // Para PICs: 18F4550 (40 pines), 18F2550 (28 pines)
                // El segundo dígito después de 18F indica: 2=28 pines, 4=40 pines
                if (dbComponent.PartNumber.ToUpper().StartsWith("18F") ||
                    dbComponent.PartNumber.ToUpper().StartsWith("PIC18F"))
                {
                    var picMatch = System.Text.RegularExpressions.Regex.Match(
                        dbComponent.PartNumber, @"18F([24])(\d{3})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (picMatch.Success)
                    {
                        string variant = picMatch.Groups[1].Value;
                        int pins = variant == "2" ? 28 : variant == "4" ? 40 : 0;

                        if (pins > 0)
                        {
                            LoggerService.Instance.Info($"✅ PIN COUNT DETECTED from PIC18F variant: {pins} pins (18F{variant}xxx)", "PinDetection");
                            return pins;
                        }
                    }
                }

                // Para ATmega: ATmega328 = 28 pines, ATmega32 = 40 pines
                if (dbComponent.PartNumber.ToUpper().Contains("ATMEGA"))
                {
                    var atmegaMatch = System.Text.RegularExpressions.Regex.Match(
                        dbComponent.PartNumber, @"ATMEGA(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (atmegaMatch.Success)
                    {
                        string model = atmegaMatch.Groups[1].Value;

                        // Tabla de conocimiento común
                        var atmegaPins = new Dictionary<string, int>
                        {
                            { "8", 28 }, { "16", 40 }, { "32", 40 }, { "48", 28 },
                            { "88", 28 }, { "168", 28 }, { "328", 28 }, { "644", 40 },
                            { "1284", 40 }, { "2560", 100 }
                        };

                        if (atmegaPins.ContainsKey(model))
                        {
                            int pins = atmegaPins[model];
                            LoggerService.Instance.Info($"✅ PIN COUNT DETECTED from ATmega model: {pins} pins (ATmega{model})", "PinDetection");
                            return pins;
                        }
                    }
                }
            }

            // ========================================================================
            // PASO 5: PACKAGES CONOCIDOS CON PIN COUNT FIJO
            // ========================================================================
            var knownPackages = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                // TO packages
                { "TO-220", 3 }, { "TO-220-3", 3 }, { "TO-220-5", 5 },
                { "TO-92", 3 }, { "TO-92-3", 3 },
                { "TO-252", 3 }, { "DPAK", 3 },
                { "TO-263", 3 }, { "D2PAK", 3 },
        
                // SOT packages
                { "SOT-23", 3 }, { "SOT-23-3", 3 },
                { "SOT-23-5", 5 }, { "SOT-23-6", 6 }, { "SOT-23-8", 8 },
                { "SOT-89", 3 }, { "SOT-89-3", 3 },
                { "SC-70", 3 }, { "SC-70-3", 3 },
                { "SC-70-5", 5 }, { "SC-70-6", 6 },
        
                // DO packages (diodos)
                { "DO-35", 2 }, { "DO-41", 2 }, { "DO-214", 2 }, { "DO-214AC", 2 }
            };

            foreach (var field in packageFields)
            {
                string package = GetSpec(field);
                if (!string.IsNullOrEmpty(package))
                {
                    foreach (var knownPkg in knownPackages)
                    {
                        if (package.Contains(knownPkg.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            LoggerService.Instance.Info($"✅ PIN COUNT from known package '{knownPkg.Key}': {knownPkg.Value} pins", "PinDetection");
                            return knownPkg.Value;
                        }
                    }
                }
            }

            // ========================================================================
            // PASO 6: DEFAULTS POR TIPO (ÚLTIMO RECURSO)
            // ========================================================================
            string type = dbComponent.Type?.ToUpper() ?? "";
            string desc = dbComponent.Description?.ToUpper() ?? "";

            var typeDefaults = new Dictionary<string, int>
                {
                    { "OPAMP", 8 }, { "OP-AMP", 8 },
                    { "DUAL OPAMP", 8 }, { "QUAD OPAMP", 14 },
                    { "REGULATOR", 3 }, { "LDO", 3 }, { "LINEAR REGULATOR", 3 },
                    { "MOSFET", 3 }, { "FET", 3 }, { "TRANSISTOR", 3 }, { "BJT", 3 },
                    { "DIODE", 2 }, { "LED", 2 },
                    { "RESISTOR", 2 }, { "CAPACITOR", 2 }, { "INDUCTOR", 2 }
                };

            foreach (var kvp in typeDefaults)
            {
                if (type.Contains(kvp.Key) || desc.Contains(kvp.Key))
                {
                    LoggerService.Instance.Warning($"⚠️ Using TYPE-BASED default for '{kvp.Key}': {kvp.Value} pins", "PinDetection");
                    return kvp.Value;
                }
            }

            // ========================================================================
            // DEFAULT FINAL (MICROCONTROLADORES)
            // ========================================================================
            if (type.Contains("MICROCONTROLLER") || type.Contains("MCU") ||
                desc.Contains("MICROCONTROLLER") || desc.Contains("MCU"))
            {
                LoggerService.Instance.Warning("⚠️ Microcontroller detected but pin count unknown, using default: 28 pins", "PinDetection");
                return 28;
            }

            // Default absoluto
            LoggerService.Instance.Error("❌ COULD NOT DETECT PIN COUNT - Using fallback: 8 pins", "PinDetection");
            LoggerService.Instance.Error($"   Component: {dbComponent.PartNumber}", "PinDetection");
            LoggerService.Instance.Error($"   Type: {dbComponent.Type}", "PinDetection");
            LoggerService.Instance.Error($"   Description: {dbComponent.Description}", "PinDetection");

            return 8;
        }

    }
}


