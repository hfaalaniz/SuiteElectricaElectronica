using System;
using System.Windows.Forms;
using System.Drawing;
using BuckConverterCalculator.PCB;
using BuckConverterCalculator.UI.Controls;

namespace BuckConverterCalculator.UI.Dialogs
{
    /// <summary>
    /// Diálogo para configurar y visualizar PCB layout
    /// </summary>
    public class PCBLayoutDialog : Form
    {
        private PCBViewer viewer;
        private Panel controlPanel;
        private TextBox txtBoardWidth;
        private TextBox txtBoardHeight;
        private ComboBox cboLayers;
        private TextBox txtTraceWidth;
        private TextBox txtClearance;
        private CheckBox chkAutoRoute;
        private Button btnGenerate;
        private Button btnExportKiCad;
        private Button btnExportGerber;
        private Button btnResetView;
        private Button btnClose;
        private Label lblStatus;

        private PCBLayout currentLayout;
        private PCBLayoutGenerator generator;

        public PCBLayoutDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "PCB Layout Generator";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 600);

            // Control panel (left side)
            controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            int y = 10;

            // Title
            var lblTitle = new Label
            {
                Text = "PCB Settings",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblTitle);
            y += 30;

            // Board dimensions
            AddLabelAndTextBox(controlPanel, "Board Width (mm):", ref txtBoardWidth, ref y, "100");
            AddLabelAndTextBox(controlPanel, "Board Height (mm):", ref txtBoardHeight, ref y, "80");

            // Layers
            var lblLayers = new Label
            {
                Text = "Layers:",
                Location = new Point(10, y),
                Width = 250,
                AutoSize = true
            };
            controlPanel.Controls.Add(lblLayers);
            y += 20;

            cboLayers = new ComboBox
            {
                Location = new Point(10, y),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboLayers.Items.AddRange(new object[] { "1 Layer", "2 Layers", "4 Layers" });
            cboLayers.SelectedIndex = 1;
            controlPanel.Controls.Add(cboLayers);
            y += 35;

            // Trace width
            AddLabelAndTextBox(controlPanel, "Trace Width (mm):", ref txtTraceWidth, ref y, "0.25");

            // Clearance
            AddLabelAndTextBox(controlPanel, "Min Clearance (mm):", ref txtClearance, ref y, "0.2");

            // Auto route
            chkAutoRoute = new CheckBox
            {
                Text = "Auto-route traces",
                Location = new Point(10, y),
                Width = 250,
                Checked = true
            };
            controlPanel.Controls.Add(chkAutoRoute);
            y += 40;

            // Generate button
            btnGenerate = new Button
            {
                Text = "Generate Layout",
                Location = new Point(10, y),
                Width = 250,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;
            controlPanel.Controls.Add(btnGenerate);
            y += 50;

            // Export section
            var lblExport = new Label
            {
                Text = "Export Options",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblExport);
            y += 30;

            btnExportKiCad = new Button
            {
                Text = "Export to KiCad",
                Location = new Point(10, y),
                Width = 250,
                Height = 30,
                Enabled = false
            };
            btnExportKiCad.Click += BtnExportKiCad_Click;
            controlPanel.Controls.Add(btnExportKiCad);
            y += 35;

            btnExportGerber = new Button
            {
                Text = "Export to Gerber",
                Location = new Point(10, y),
                Width = 250,
                Height = 30,
                Enabled = false
            };
            btnExportGerber.Click += BtnExportGerber_Click;
            controlPanel.Controls.Add(btnExportGerber);
            y += 50;

            // View controls
            var lblView = new Label
            {
                Text = "View Controls",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controlPanel.Controls.Add(lblView);
            y += 30;

            btnResetView = new Button
            {
                Text = "Reset View",
                Location = new Point(10, y),
                Width = 250,
                Height = 30
            };
            btnResetView.Click += (s, e) => viewer.ResetView();
            controlPanel.Controls.Add(btnResetView);
            y += 50;

            // Status
            lblStatus = new Label
            {
                Location = new Point(10, y),
                Width = 250,
                Height = 80,
                Text = "Configure settings and click 'Generate Layout'",
                ForeColor = Color.Gray
            };
            controlPanel.Controls.Add(lblStatus);
            y += 90;

            // Close button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(10, y),
                Width = 250,
                Height = 30
            };
            btnClose.Click += (s, e) => this.Close();
            controlPanel.Controls.Add(btnClose);

            // PCB Viewer
            viewer = new PCBViewer
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(viewer);
            this.Controls.Add(controlPanel);
        }

        private void AddLabelAndTextBox(Panel panel, string labelText, ref TextBox textBox, ref int y, string defaultValue)
        {
            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, y),
                Width = 250,
                AutoSize = true
            };
            panel.Controls.Add(label);
            y += 20;

            textBox = new TextBox
            {
                Location = new Point(10, y),
                Width = 250,
                Text = defaultValue
            };
            panel.Controls.Add(textBox);
            y += 35;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (!ValidateInputs())
                    return;

                // Create settings
                var settings = new PCBSettings
                {
                    BoardWidth = double.Parse(txtBoardWidth.Text),
                    BoardHeight = double.Parse(txtBoardHeight.Text),
                    Layers = cboLayers.SelectedIndex == 0 ? 1 : cboLayers.SelectedIndex == 1 ? 2 : 4,
                    DefaultTraceWidth = double.Parse(txtTraceWidth.Text),
                    MinimumClearance = double.Parse(txtClearance.Text),
                    AutoRoute = chkAutoRoute.Checked
                };

                // Note: generator would need to be passed a SchematicDocument
                // For now, create a simple demo layout
                currentLayout = CreateDemoLayout(settings);

                // Display layout
                viewer.SetLayout(currentLayout);

                lblStatus.Text = $"Layout generated!\n" +
                               $"Components: {currentLayout.Components.Count}\n" +
                               $"Traces: {currentLayout.Traces.Count}";
                lblStatus.ForeColor = Color.Green;

                btnExportKiCad.Enabled = true;
                btnExportGerber.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Layout generation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            try
            {
                double width = double.Parse(txtBoardWidth.Text);
                double height = double.Parse(txtBoardHeight.Text);
                double trace = double.Parse(txtTraceWidth.Text);
                double clear = double.Parse(txtClearance.Text);

                if (width <= 0 || height <= 0 || trace <= 0 || clear <= 0)
                {
                    MessageBox.Show("All dimensions must be positive.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Please enter valid numerical values.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private PCBLayout CreateDemoLayout(PCBSettings settings)
        {
            // Create a simple demo layout (in real use, this would come from PCBLayoutGenerator)
            var layout = new PCBLayout
            {
                Width = settings.BoardWidth,
                Height = settings.BoardHeight,
                Layers = settings.Layers,
                Components = new System.Collections.Generic.List<PCBComponent>(),
                Traces = new System.Collections.Generic.List<PCBTrace>()
            };

            // Add some demo components
            layout.Components.Add(new PCBComponent
            {
                Name = "U1",
                Type = "IC",
                X = 20,
                Y = 20,
                Footprint = "SOIC-8",
                Layer = "Top"
            });

            layout.Components.Add(new PCBComponent
            {
                Name = "L1",
                Type = "Inductor",
                X = 50,
                Y = 20,
                Footprint = "SMD_5x5",
                Layer = "Top"
            });

            layout.Components.Add(new PCBComponent
            {
                Name = "C1",
                Type = "Capacitor",
                X = 70,
                Y = 20,
                Footprint = "1206",
                Layer = "Top"
            });

            layout.Components.Add(new PCBComponent
            {
                Name = "R1",
                Type = "Resistor",
                X = 20,
                Y = 40,
                Footprint = "0805",
                Layer = "Top"
            });

            // Add some demo traces
            layout.Traces.Add(new PCBTrace
            {
                NetName = "NET1",
                Layer = "Top",
                Width = settings.DefaultTraceWidth,
                StartX = 20,
                StartY = 20,
                EndX = 50,
                EndY = 20
            });

            layout.Traces.Add(new PCBTrace
            {
                NetName = "NET2",
                Layer = "Top",
                Width = settings.DefaultTraceWidth,
                StartX = 50,
                StartY = 20,
                EndX = 70,
                EndY = 20
            });

            return layout;
        }

        private void BtnExportKiCad_Click(object sender, EventArgs e)
        {
            if (currentLayout == null) return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "KiCad PCB files (*.kicad_pcb)|*.kicad_pcb|All files (*.*)|*.*";
                dialog.DefaultExt = "kicad_pcb";
                dialog.FileName = "layout.kicad_pcb";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // In real implementation, use PCBLayoutGenerator.ExportToKiCad
                        ExportToKiCadFormat(currentLayout, dialog.FileName);

                        MessageBox.Show($"PCB layout exported to {dialog.FileName}",
                            "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnExportGerber_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gerber export is not yet implemented.\nUse KiCad export and generate Gerbers from KiCad.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToKiCadFormat(PCBLayout layout, string filename)
        {
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("(kicad_pcb (version 20171130) (host pcbnew)");
                writer.WriteLine("  (general");
                writer.WriteLine($"    (thickness 1.6)");
                writer.WriteLine("  )");
                writer.WriteLine($"  (page A4)");
                writer.WriteLine("  (layers");
                writer.WriteLine("    (0 F.Cu signal)");
                writer.WriteLine("    (31 B.Cu signal)");
                writer.WriteLine("  )");

                // Modules (components)
                foreach (var comp in layout.Components)
                {
                    writer.WriteLine($"  (module {comp.Footprint} (layer {comp.Layer})");
                    writer.WriteLine($"    (at {comp.X} {comp.Y})");
                    writer.WriteLine($"    (fp_text reference {comp.Name} (at 0 0) (layer F.SilkS))");
                    writer.WriteLine("  )");
                }

                // Segments (traces)
                foreach (var trace in layout.Traces)
                {
                    writer.WriteLine($"  (segment (start {trace.StartX} {trace.StartY}) (end {trace.EndX} {trace.EndY})");
                    writer.WriteLine($"    (width {trace.Width}) (layer {trace.Layer}) (net 1))");
                }

                writer.WriteLine(")");
            }
        }

        public void SetLayout(PCBLayout layout)
        {
            currentLayout = layout;
            viewer.SetLayout(layout);
            btnExportKiCad.Enabled = true;
            btnExportGerber.Enabled = true;
        }
    }
}