namespace BuckConverterCalculator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBoxInput = new GroupBox();
            labelEfficiency = new Label();
            numEfficiency = new NumericUpDown();
            labelRippleVoltage = new Label();
            numRippleVoltage = new NumericUpDown();
            labelRippleCurrent = new Label();
            numRippleCurrent = new NumericUpDown();
            labelFreq = new Label();
            numFreq = new NumericUpDown();
            labelIout = new Label();
            numIout = new NumericUpDown();
            labelVout = new Label();
            numVout = new NumericUpDown();
            labelVin = new Label();
            numVin = new NumericUpDown();
            btnCalculate = new Button();
            btnClear = new Button();
            btnExport = new Button();
            groupBoxResults = new GroupBox();
            txtResults = new RichTextBox();
            groupBoxComponents = new GroupBox();
            txtComponents = new RichTextBox();
            groupBoxLosses = new GroupBox();
            txtLosses = new RichTextBox();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            componentDatabaseToolStripMenuItem = new ToolStripMenuItem();
            presetConfigurationsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            documentationToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnViewSchematic = new Button();
            btnOpenEditor = new Button();
            btnCalculateReactor = new Button();
            btnTransformerCalculator = new Button();
            btnCalcProteccionMotores = new Button();
            btnSchematicUnifilar = new Button();
            groupBoxInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numEfficiency).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRippleVoltage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRippleCurrent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVin).BeginInit();
            groupBoxResults.SuspendLayout();
            groupBoxComponents.SuspendLayout();
            groupBoxLosses.SuspendLayout();
            statusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxInput
            // 
            groupBoxInput.Controls.Add(labelEfficiency);
            groupBoxInput.Controls.Add(numEfficiency);
            groupBoxInput.Controls.Add(labelRippleVoltage);
            groupBoxInput.Controls.Add(numRippleVoltage);
            groupBoxInput.Controls.Add(labelRippleCurrent);
            groupBoxInput.Controls.Add(numRippleCurrent);
            groupBoxInput.Controls.Add(labelFreq);
            groupBoxInput.Controls.Add(numFreq);
            groupBoxInput.Controls.Add(labelIout);
            groupBoxInput.Controls.Add(numIout);
            groupBoxInput.Controls.Add(labelVout);
            groupBoxInput.Controls.Add(numVout);
            groupBoxInput.Controls.Add(labelVin);
            groupBoxInput.Controls.Add(numVin);
            groupBoxInput.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBoxInput.Location = new Point(12, 27);
            groupBoxInput.Name = "groupBoxInput";
            groupBoxInput.Size = new Size(350, 320);
            groupBoxInput.TabIndex = 0;
            groupBoxInput.TabStop = false;
            groupBoxInput.Text = "Parámetros de Diseño";
            // 
            // labelEfficiency
            // 
            labelEfficiency.AutoSize = true;
            labelEfficiency.Font = new Font("Segoe UI", 9F);
            labelEfficiency.Location = new Point(20, 247);
            labelEfficiency.Name = "labelEfficiency";
            labelEfficiency.Size = new Size(132, 15);
            labelEfficiency.TabIndex = 12;
            labelEfficiency.Text = "Eficiencia Estimada (%):";
            // 
            // numEfficiency
            // 
            numEfficiency.DecimalPlaces = 1;
            numEfficiency.Font = new Font("Segoe UI", 9F);
            numEfficiency.Location = new Point(200, 245);
            numEfficiency.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            numEfficiency.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numEfficiency.Name = "numEfficiency";
            numEfficiency.Size = new Size(120, 23);
            numEfficiency.TabIndex = 13;
            numEfficiency.Value = new decimal(new int[] { 85, 0, 0, 0 });
            // 
            // labelRippleVoltage
            // 
            labelRippleVoltage.AutoSize = true;
            labelRippleVoltage.Font = new Font("Segoe UI", 9F);
            labelRippleVoltage.Location = new Point(20, 212);
            labelRippleVoltage.Name = "labelRippleVoltage";
            labelRippleVoltage.Size = new Size(145, 15);
            labelRippleVoltage.TabIndex = 10;
            labelRippleVoltage.Text = "Ripple de Tensión (mVpp):";
            // 
            // numRippleVoltage
            // 
            numRippleVoltage.Font = new Font("Segoe UI", 9F);
            numRippleVoltage.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numRippleVoltage.Location = new Point(200, 210);
            numRippleVoltage.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numRippleVoltage.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numRippleVoltage.Name = "numRippleVoltage";
            numRippleVoltage.Size = new Size(120, 23);
            numRippleVoltage.TabIndex = 11;
            numRippleVoltage.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // labelRippleCurrent
            // 
            labelRippleCurrent.AutoSize = true;
            labelRippleCurrent.Font = new Font("Segoe UI", 9F);
            labelRippleCurrent.Location = new Point(20, 177);
            labelRippleCurrent.Name = "labelRippleCurrent";
            labelRippleCurrent.Size = new Size(145, 15);
            labelRippleCurrent.TabIndex = 8;
            labelRippleCurrent.Text = "Ripple de Corriente (% Io):";
            // 
            // numRippleCurrent
            // 
            numRippleCurrent.DecimalPlaces = 1;
            numRippleCurrent.Font = new Font("Segoe UI", 9F);
            numRippleCurrent.Location = new Point(200, 175);
            numRippleCurrent.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            numRippleCurrent.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numRippleCurrent.Name = "numRippleCurrent";
            numRippleCurrent.Size = new Size(120, 23);
            numRippleCurrent.TabIndex = 9;
            numRippleCurrent.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // labelFreq
            // 
            labelFreq.AutoSize = true;
            labelFreq.Font = new Font("Segoe UI", 9F);
            labelFreq.Location = new Point(20, 142);
            labelFreq.Name = "labelFreq";
            labelFreq.Size = new Size(92, 15);
            labelFreq.TabIndex = 6;
            labelFreq.Text = "Frecuencia (Hz):";
            // 
            // numFreq
            // 
            numFreq.Font = new Font("Segoe UI", 9F);
            numFreq.Increment = new decimal(new int[] { 10000, 0, 0, 0 });
            numFreq.Location = new Point(200, 140);
            numFreq.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numFreq.Minimum = new decimal(new int[] { 10000, 0, 0, 0 });
            numFreq.Name = "numFreq";
            numFreq.Size = new Size(120, 23);
            numFreq.TabIndex = 7;
            numFreq.Value = new decimal(new int[] { 100000, 0, 0, 0 });
            // 
            // labelIout
            // 
            labelIout.AutoSize = true;
            labelIout.Font = new Font("Segoe UI", 9F);
            labelIout.Location = new Point(20, 107);
            labelIout.Name = "labelIout";
            labelIout.Size = new Size(144, 15);
            labelIout.TabIndex = 4;
            labelIout.Text = "Corriente de Salida (ADC):";
            // 
            // numIout
            // 
            numIout.DecimalPlaces = 2;
            numIout.Font = new Font("Segoe UI", 9F);
            numIout.Location = new Point(200, 105);
            numIout.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            numIout.Name = "numIout";
            numIout.Size = new Size(120, 23);
            numIout.TabIndex = 5;
            numIout.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // labelVout
            // 
            labelVout.AutoSize = true;
            labelVout.Font = new Font("Segoe UI", 9F);
            labelVout.Location = new Point(20, 72);
            labelVout.Name = "labelVout";
            labelVout.Size = new Size(134, 15);
            labelVout.TabIndex = 2;
            labelVout.Text = "Tensión de Salida (VDC):";
            // 
            // numVout
            // 
            numVout.DecimalPlaces = 1;
            numVout.Font = new Font("Segoe UI", 9F);
            numVout.Location = new Point(200, 70);
            numVout.Maximum = new decimal(new int[] { 400, 0, 0, 0 });
            numVout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numVout.Name = "numVout";
            numVout.Size = new Size(120, 23);
            numVout.TabIndex = 3;
            numVout.Value = new decimal(new int[] { 13, 0, 0, 0 });
            // 
            // labelVin
            // 
            labelVin.AutoSize = true;
            labelVin.Font = new Font("Segoe UI", 9F);
            labelVin.Location = new Point(20, 37);
            labelVin.Name = "labelVin";
            labelVin.Size = new Size(143, 15);
            labelVin.TabIndex = 0;
            labelVin.Text = "Tensión de Entrada (VDC):";
            // 
            // numVin
            // 
            numVin.DecimalPlaces = 1;
            numVin.Font = new Font("Segoe UI", 9F);
            numVin.Location = new Point(200, 35);
            numVin.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numVin.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numVin.Name = "numVin";
            numVin.Size = new Size(120, 23);
            numVin.TabIndex = 1;
            numVin.Value = new decimal(new int[] { 90, 0, 0, 0 });
            // 
            // btnCalculate
            // 
            btnCalculate.BackColor = Color.FromArgb(0, 120, 215);
            btnCalculate.FlatStyle = FlatStyle.Flat;
            btnCalculate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCalculate.ForeColor = Color.White;
            btnCalculate.Location = new Point(799, 39);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(118, 40);
            btnCalculate.TabIndex = 14;
            btnCalculate.Text = "Calcular";
            btnCalculate.UseVisualStyleBackColor = false;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // btnClear
            // 
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 9F);
            btnClear.Location = new Point(799, 88);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(118, 40);
            btnClear.TabIndex = 15;
            btnClear.Text = "Limpiar";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnExport
            // 
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Segoe UI", 9F);
            btnExport.Location = new Point(799, 134);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(118, 40);
            btnExport.TabIndex = 16;
            btnExport.Text = "Exportar PDF";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // groupBoxResults
            // 
            groupBoxResults.Controls.Add(txtResults);
            groupBoxResults.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBoxResults.Location = new Point(380, 27);
            groupBoxResults.Name = "groupBoxResults";
            groupBoxResults.Size = new Size(400, 320);
            groupBoxResults.TabIndex = 17;
            groupBoxResults.TabStop = false;
            groupBoxResults.Text = "Resultados de Diseño";
            // 
            // txtResults
            // 
            txtResults.BorderStyle = BorderStyle.FixedSingle;
            txtResults.Font = new Font("Consolas", 9F);
            txtResults.Location = new Point(15, 25);
            txtResults.Name = "txtResults";
            txtResults.ReadOnly = true;
            txtResults.Size = new Size(370, 286);
            txtResults.TabIndex = 0;
            txtResults.Text = "";
            // 
            // groupBoxComponents
            // 
            groupBoxComponents.Controls.Add(txtComponents);
            groupBoxComponents.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBoxComponents.Location = new Point(12, 353);
            groupBoxComponents.Name = "groupBoxComponents";
            groupBoxComponents.Size = new Size(520, 280);
            groupBoxComponents.TabIndex = 18;
            groupBoxComponents.TabStop = false;
            groupBoxComponents.Text = "Selección de Componentes";
            // 
            // txtComponents
            // 
            txtComponents.BorderStyle = BorderStyle.FixedSingle;
            txtComponents.Font = new Font("Consolas", 9F);
            txtComponents.Location = new Point(15, 25);
            txtComponents.Name = "txtComponents";
            txtComponents.ReadOnly = true;
            txtComponents.Size = new Size(490, 240);
            txtComponents.TabIndex = 0;
            txtComponents.Text = "";
            // 
            // groupBoxLosses
            // 
            groupBoxLosses.Controls.Add(txtLosses);
            groupBoxLosses.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBoxLosses.Location = new Point(550, 353);
            groupBoxLosses.Name = "groupBoxLosses";
            groupBoxLosses.Size = new Size(230, 280);
            groupBoxLosses.TabIndex = 19;
            groupBoxLosses.TabStop = false;
            groupBoxLosses.Text = "Análisis de Pérdidas";
            // 
            // txtLosses
            // 
            txtLosses.BorderStyle = BorderStyle.FixedSingle;
            txtLosses.Font = new Font("Consolas", 9F);
            txtLosses.Location = new Point(15, 25);
            txtLosses.Name = "txtLosses";
            txtLosses.ReadOnly = true;
            txtLosses.Size = new Size(200, 240);
            txtLosses.TabIndex = 0;
            txtLosses.Text = "";
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip.Location = new Point(0, 662);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(934, 22);
            statusStrip.TabIndex = 20;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(174, 17);
            toolStripStatusLabel.Text = "Listo para realizar cálculos | v1.0";
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(934, 24);
            menuStrip.TabIndex = 21;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(60, 20);
            fileToolStripMenuItem.Text = "&Archivo";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(156, 22);
            newToolStripMenuItem.Text = "&Nuevo";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(156, 22);
            openToolStripMenuItem.Text = "&Abrir";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(156, 22);
            saveToolStripMenuItem.Text = "&Guardar";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(153, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(156, 22);
            exitToolStripMenuItem.Text = "&Salir";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { componentDatabaseToolStripMenuItem, presetConfigurationsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(90, 20);
            toolsToolStripMenuItem.Text = "&Herramientas";
            // 
            // componentDatabaseToolStripMenuItem
            // 
            componentDatabaseToolStripMenuItem.Name = "componentDatabaseToolStripMenuItem";
            componentDatabaseToolStripMenuItem.Size = new Size(241, 22);
            componentDatabaseToolStripMenuItem.Text = "Base de Datos de Componentes";
            componentDatabaseToolStripMenuItem.Click += componentDatabaseToolStripMenuItem_Click;
            // 
            // presetConfigurationsToolStripMenuItem
            // 
            presetConfigurationsToolStripMenuItem.Name = "presetConfigurationsToolStripMenuItem";
            presetConfigurationsToolStripMenuItem.Size = new Size(241, 22);
            presetConfigurationsToolStripMenuItem.Text = "Configuraciones Predefinidas";
            presetConfigurationsToolStripMenuItem.Click += presetConfigurationsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { documentationToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(53, 20);
            helpToolStripMenuItem.Text = "Ay&uda";
            // 
            // documentationToolStripMenuItem
            // 
            documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            documentationToolStripMenuItem.Size = new Size(159, 22);
            documentationToolStripMenuItem.Text = "&Documentación";
            documentationToolStripMenuItem.Click += documentationToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(159, 22);
            aboutToolStripMenuItem.Text = "&Acerca de...";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // btnViewSchematic
            // 
            btnViewSchematic.BackColor = Color.FromArgb(16, 124, 16);
            btnViewSchematic.FlatStyle = FlatStyle.Flat;
            btnViewSchematic.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnViewSchematic.ForeColor = Color.White;
            btnViewSchematic.Location = new Point(799, 180);
            btnViewSchematic.Name = "btnViewSchematic";
            btnViewSchematic.Size = new Size(118, 45);
            btnViewSchematic.TabIndex = 17;
            btnViewSchematic.Text = "📋 Ver Esquemático";
            btnViewSchematic.UseVisualStyleBackColor = false;
            btnViewSchematic.Click += btnViewSchematic_Click;
            // 
            // btnOpenEditor
            // 
            btnOpenEditor.FlatStyle = FlatStyle.Flat;
            btnOpenEditor.Font = new Font("Segoe UI", 9F);
            btnOpenEditor.Location = new Point(799, 237);
            btnOpenEditor.Name = "btnOpenEditor";
            btnOpenEditor.Size = new Size(118, 40);
            btnOpenEditor.TabIndex = 16;
            btnOpenEditor.Text = "Editar Esquema";
            btnOpenEditor.UseVisualStyleBackColor = true;
            btnOpenEditor.Click += btnOpenEditor_Click;
            // 
            // btnCalculateReactor
            // 
            btnCalculateReactor.FlatStyle = FlatStyle.Flat;
            btnCalculateReactor.Font = new Font("Segoe UI", 9F);
            btnCalculateReactor.Location = new Point(799, 329);
            btnCalculateReactor.Name = "btnCalculateReactor";
            btnCalculateReactor.Size = new Size(118, 40);
            btnCalculateReactor.TabIndex = 16;
            btnCalculateReactor.Text = "Calcular Reactor";
            btnCalculateReactor.UseVisualStyleBackColor = true;
            btnCalculateReactor.Click += btnCalculateReactor_Click;
            // 
            // btnTransformerCalculator
            // 
            btnTransformerCalculator.FlatStyle = FlatStyle.Flat;
            btnTransformerCalculator.Font = new Font("Segoe UI", 9F);
            btnTransformerCalculator.Location = new Point(799, 375);
            btnTransformerCalculator.Name = "btnTransformerCalculator";
            btnTransformerCalculator.Size = new Size(118, 40);
            btnTransformerCalculator.TabIndex = 16;
            btnTransformerCalculator.Text = "Calcular Trafo";
            btnTransformerCalculator.UseVisualStyleBackColor = true;
            btnTransformerCalculator.Click += btnTransformerCalculator_Click;
            // 
            // btnCalcProteccionMotores
            // 
            btnCalcProteccionMotores.FlatStyle = FlatStyle.Flat;
            btnCalcProteccionMotores.Font = new Font("Segoe UI", 9F);
            btnCalcProteccionMotores.Location = new Point(799, 424);
            btnCalcProteccionMotores.Name = "btnCalcProteccionMotores";
            btnCalcProteccionMotores.Size = new Size(118, 40);
            btnCalcProteccionMotores.TabIndex = 16;
            btnCalcProteccionMotores.Text = "Proteccion Motores";
            btnCalcProteccionMotores.UseVisualStyleBackColor = true;
            btnCalcProteccionMotores.Click += btnCalcProteccionMotores_Click;
            // 
            // btnSchematicUnifilar
            // 
            btnSchematicUnifilar.FlatStyle = FlatStyle.Flat;
            btnSchematicUnifilar.Font = new Font("Segoe UI", 9F);
            btnSchematicUnifilar.Location = new Point(799, 283);
            btnSchematicUnifilar.Name = "btnSchematicUnifilar";
            btnSchematicUnifilar.Size = new Size(118, 40);
            btnSchematicUnifilar.TabIndex = 16;
            btnSchematicUnifilar.Text = "Unifilares";
            btnSchematicUnifilar.UseVisualStyleBackColor = true;
            btnSchematicUnifilar.Click += btnSchematicUnifilar_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 684);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            Controls.Add(groupBoxLosses);
            Controls.Add(groupBoxComponents);
            Controls.Add(groupBoxResults);
            Controls.Add(btnSchematicUnifilar);
            Controls.Add(btnOpenEditor);
            Controls.Add(btnCalcProteccionMotores);
            Controls.Add(btnTransformerCalculator);
            Controls.Add(btnCalculateReactor);
            Controls.Add(btnExport);
            Controls.Add(btnClear);
            Controls.Add(btnCalculate);
            Controls.Add(groupBoxInput);
            Controls.Add(btnViewSchematic);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Calculadora Buck Converter - Diseño de Fuentes Switching";
            groupBoxInput.ResumeLayout(false);
            groupBoxInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numEfficiency).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRippleVoltage).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRippleCurrent).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVout).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVin).EndInit();
            groupBoxResults.ResumeLayout(false);
            groupBoxComponents.ResumeLayout(false);
            groupBoxLosses.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.NumericUpDown numVin;
        private System.Windows.Forms.NumericUpDown numVout;
        private System.Windows.Forms.NumericUpDown numIout;
        private System.Windows.Forms.NumericUpDown numFreq;
        private System.Windows.Forms.NumericUpDown numRippleCurrent;
        private System.Windows.Forms.NumericUpDown numRippleVoltage;
        private System.Windows.Forms.NumericUpDown numEfficiency;
        private System.Windows.Forms.Label labelVin;
        private System.Windows.Forms.Label labelVout;
        private System.Windows.Forms.Label labelIout;
        private System.Windows.Forms.Label labelFreq;
        private System.Windows.Forms.Label labelRippleCurrent;
        private System.Windows.Forms.Label labelRippleVoltage;
        private System.Windows.Forms.Label labelEfficiency;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnViewSchematic;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.RichTextBox txtResults;
        private System.Windows.Forms.GroupBox groupBoxComponents;
        private System.Windows.Forms.RichTextBox txtComponents;
        private System.Windows.Forms.GroupBox groupBoxLosses;
        private System.Windows.Forms.RichTextBox txtLosses;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem componentDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem presetConfigurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private Button btnOpenEditor;
        private Button btnCalculateReactor;
        private Button btnTransformerCalculator;
        private Button btnCalcProteccionMotores;
        private Button btnSchematicUnifilar;
    }
}