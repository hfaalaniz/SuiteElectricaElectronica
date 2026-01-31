namespace BuckConverterCalculator
{
    partial class TransformadorCalculatorForm
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
            tabControl1 = new TabControl();
            tabEntrada = new TabPage();
            groupBox4 = new GroupBox();
            btnCalculadoraNucleo = new Button();
            txtFrecuencia = new TextBox();
            label14 = new Label();
            txtDensidadFlujo = new TextBox();
            label13 = new Label();
            txtAreaNucleo = new TextBox();
            label12 = new Label();
            groupBox3 = new GroupBox();
            chkSec2Enabled = new CheckBox();
            txtCorrienteSec2 = new TextBox();
            label11 = new Label();
            txtVoltageSec2 = new TextBox();
            label10 = new Label();
            groupBox2 = new GroupBox();
            txtCorrienteSec1 = new TextBox();
            label9 = new Label();
            txtVoltageSec1 = new TextBox();
            label8 = new Label();
            groupBox1 = new GroupBox();
            txtNumDevanadosPrim = new TextBox();
            label7 = new Label();
            txtVoltagePrimario = new TextBox();
            label6 = new Label();
            btnCalcular = new Button();
            btnLimpiar = new Button();
            tabResultados = new TabPage();
            dgvResultados = new DataGridView();
            colParametro = new DataGridViewTextBoxColumn();
            colValor = new DataGridViewTextBoxColumn();
            colUnidad = new DataGridViewTextBoxColumn();
            colObservacion = new DataGridViewTextBoxColumn();
            tabAnalisisTermico = new TabPage();
            dgvTermico = new DataGridView();
            colEscenario = new DataGridViewTextBoxColumn();
            colCoeficiente = new DataGridViewTextBoxColumn();
            colDeltaT = new DataGridViewTextBoxColumn();
            colTempOperacion = new DataGridViewTextBoxColumn();
            colTempHotspot = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            txtRecomendacionesTermicas = new TextBox();
            label15 = new Label();
            tabReporte = new TabPage();
            txtReporte = new TextBox();
            btnExportarTxt = new Button();
            btnExportarExcel = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            label1 = new Label();
            btnVistaPrevia = new Button();
            tabControl1.SuspendLayout();
            tabEntrada.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResultados).BeginInit();
            tabAnalisisTermico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTermico).BeginInit();
            tabReporte.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabEntrada);
            tabControl1.Controls.Add(tabResultados);
            tabControl1.Controls.Add(tabAnalisisTermico);
            tabControl1.Controls.Add(tabReporte);
            tabControl1.Location = new Point(12, 60);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(960, 540);
            tabControl1.TabIndex = 0;
            // 
            // tabEntrada
            // 
            tabEntrada.BackColor = SystemColors.Control;
            tabEntrada.Controls.Add(groupBox4);
            tabEntrada.Controls.Add(groupBox3);
            tabEntrada.Controls.Add(groupBox2);
            tabEntrada.Controls.Add(groupBox1);
            tabEntrada.Controls.Add(btnCalcular);
            tabEntrada.Controls.Add(btnLimpiar);
            tabEntrada.Location = new Point(4, 24);
            tabEntrada.Name = "tabEntrada";
            tabEntrada.Padding = new Padding(3);
            tabEntrada.Size = new Size(952, 512);
            tabEntrada.TabIndex = 0;
            tabEntrada.Text = "Datos de Entrada";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnCalculadoraNucleo);
            groupBox4.Controls.Add(txtFrecuencia);
            groupBox4.Controls.Add(label14);
            groupBox4.Controls.Add(txtDensidadFlujo);
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(txtAreaNucleo);
            groupBox4.Controls.Add(label12);
            groupBox4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox4.Location = new Point(20, 320);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(450, 140);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Parámetros del Núcleo";
            // 
            // btnCalculadoraNucleo
            // 
            btnCalculadoraNucleo.BackColor = Color.FromArgb(108, 117, 125);
            btnCalculadoraNucleo.FlatAppearance.BorderSize = 0;
            btnCalculadoraNucleo.FlatStyle = FlatStyle.Flat;
            btnCalculadoraNucleo.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnCalculadoraNucleo.ForeColor = Color.White;
            btnCalculadoraNucleo.Location = new Point(325, 29);
            btnCalculadoraNucleo.Name = "btnCalculadoraNucleo";
            btnCalculadoraNucleo.Size = new Size(25, 23);
            btnCalculadoraNucleo.TabIndex = 6;
            btnCalculadoraNucleo.Text = "...";
            btnCalculadoraNucleo.UseVisualStyleBackColor = false;
            btnCalculadoraNucleo.Click += btnCalculadoraNucleo_Click;
            // 
            // txtFrecuencia
            // 
            txtFrecuencia.Font = new Font("Segoe UI", 9F);
            txtFrecuencia.Location = new Point(200, 95);
            txtFrecuencia.Name = "txtFrecuencia";
            txtFrecuencia.Size = new Size(150, 23);
            txtFrecuencia.TabIndex = 5;
            txtFrecuencia.Text = "50";
            txtFrecuencia.TextAlign = HorizontalAlignment.Right;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9F);
            label14.Location = new Point(20, 98);
            label14.Name = "label14";
            label14.Size = new Size(92, 15);
            label14.TabIndex = 4;
            label14.Text = "Frecuencia (Hz):";
            // 
            // txtDensidadFlujo
            // 
            txtDensidadFlujo.Font = new Font("Segoe UI", 9F);
            txtDensidadFlujo.Location = new Point(200, 62);
            txtDensidadFlujo.Name = "txtDensidadFlujo";
            txtDensidadFlujo.Size = new Size(150, 23);
            txtDensidadFlujo.TabIndex = 3;
            txtDensidadFlujo.Text = "1,5";
            txtDensidadFlujo.TextAlign = HorizontalAlignment.Right;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9F);
            label13.Location = new Point(20, 65);
            label13.Name = "label13";
            label13.Size = new Size(121, 15);
            label13.TabIndex = 2;
            label13.Text = "Densidad de Flujo (T):";
            // 
            // txtAreaNucleo
            // 
            txtAreaNucleo.Font = new Font("Segoe UI", 9F);
            txtAreaNucleo.Location = new Point(200, 29);
            txtAreaNucleo.Name = "txtAreaNucleo";
            txtAreaNucleo.Size = new Size(120, 23);
            txtAreaNucleo.TabIndex = 1;
            txtAreaNucleo.Text = "51,99";
            txtAreaNucleo.TextAlign = HorizontalAlignment.Right;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F);
            label12.Location = new Point(20, 32);
            label12.Name = "label12";
            label12.Size = new Size(143, 15);
            label12.TabIndex = 0;
            label12.Text = "Área del Núcleo Ae (cm²):";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(chkSec2Enabled);
            groupBox3.Controls.Add(txtCorrienteSec2);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(txtVoltageSec2);
            groupBox3.Controls.Add(label10);
            groupBox3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox3.Location = new Point(490, 170);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(440, 130);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Secundario 2";
            // 
            // chkSec2Enabled
            // 
            chkSec2Enabled.AutoSize = true;
            chkSec2Enabled.Font = new Font("Segoe UI", 9F);
            chkSec2Enabled.Location = new Point(300, 0);
            chkSec2Enabled.Name = "chkSec2Enabled";
            chkSec2Enabled.Size = new Size(71, 19);
            chkSec2Enabled.TabIndex = 4;
            chkSec2Enabled.Text = "Habilitar";
            chkSec2Enabled.UseVisualStyleBackColor = true;
            // 
            // txtCorrienteSec2
            // 
            txtCorrienteSec2.Enabled = false;
            txtCorrienteSec2.Font = new Font("Segoe UI", 9F);
            txtCorrienteSec2.Location = new Point(200, 75);
            txtCorrienteSec2.Name = "txtCorrienteSec2";
            txtCorrienteSec2.Size = new Size(150, 23);
            txtCorrienteSec2.TabIndex = 3;
            txtCorrienteSec2.Text = "30";
            txtCorrienteSec2.TextAlign = HorizontalAlignment.Right;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9F);
            label11.Location = new Point(20, 78);
            label11.Name = "label11";
            label11.Size = new Size(78, 15);
            label11.TabIndex = 2;
            label11.Text = "Corriente (A):";
            // 
            // txtVoltageSec2
            // 
            txtVoltageSec2.Enabled = false;
            txtVoltageSec2.Font = new Font("Segoe UI", 9F);
            txtVoltageSec2.Location = new Point(200, 35);
            txtVoltageSec2.Name = "txtVoltageSec2";
            txtVoltageSec2.Size = new Size(150, 23);
            txtVoltageSec2.TabIndex = 1;
            txtVoltageSec2.Text = "110";
            txtVoltageSec2.TextAlign = HorizontalAlignment.Right;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9F);
            label10.Location = new Point(20, 38);
            label10.Name = "label10";
            label10.Size = new Size(63, 15);
            label10.TabIndex = 0;
            label10.Text = "Voltaje (V):";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtCorrienteSec1);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(txtVoltageSec1);
            groupBox2.Controls.Add(label8);
            groupBox2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox2.Location = new Point(20, 170);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(450, 130);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Secundario 1";
            // 
            // txtCorrienteSec1
            // 
            txtCorrienteSec1.Font = new Font("Segoe UI", 9F);
            txtCorrienteSec1.Location = new Point(200, 75);
            txtCorrienteSec1.Name = "txtCorrienteSec1";
            txtCorrienteSec1.Size = new Size(150, 23);
            txtCorrienteSec1.TabIndex = 3;
            txtCorrienteSec1.Text = "30";
            txtCorrienteSec1.TextAlign = HorizontalAlignment.Right;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F);
            label9.Location = new Point(20, 78);
            label9.Name = "label9";
            label9.Size = new Size(78, 15);
            label9.TabIndex = 2;
            label9.Text = "Corriente (A):";
            // 
            // txtVoltageSec1
            // 
            txtVoltageSec1.Font = new Font("Segoe UI", 9F);
            txtVoltageSec1.Location = new Point(200, 35);
            txtVoltageSec1.Name = "txtVoltageSec1";
            txtVoltageSec1.Size = new Size(150, 23);
            txtVoltageSec1.TabIndex = 1;
            txtVoltageSec1.Text = "110";
            txtVoltageSec1.TextAlign = HorizontalAlignment.Right;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9F);
            label8.Location = new Point(20, 38);
            label8.Name = "label8";
            label8.Size = new Size(63, 15);
            label8.TabIndex = 0;
            label8.Text = "Voltaje (V):";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtNumDevanadosPrim);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtVoltagePrimario);
            groupBox1.Controls.Add(label6);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(20, 20);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(450, 130);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Primario";
            // 
            // txtNumDevanadosPrim
            // 
            txtNumDevanadosPrim.Font = new Font("Segoe UI", 9F);
            txtNumDevanadosPrim.Location = new Point(200, 75);
            txtNumDevanadosPrim.Name = "txtNumDevanadosPrim";
            txtNumDevanadosPrim.Size = new Size(150, 23);
            txtNumDevanadosPrim.TabIndex = 3;
            txtNumDevanadosPrim.Text = "2";
            txtNumDevanadosPrim.TextAlign = HorizontalAlignment.Right;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F);
            label7.Location = new Point(20, 78);
            label7.Name = "label7";
            label7.Size = new Size(131, 15);
            label7.TabIndex = 2;
            label7.Text = "Número de Devanados:";
            // 
            // txtVoltagePrimario
            // 
            txtVoltagePrimario.Font = new Font("Segoe UI", 9F);
            txtVoltagePrimario.Location = new Point(200, 35);
            txtVoltagePrimario.Name = "txtVoltagePrimario";
            txtVoltagePrimario.Size = new Size(150, 23);
            txtVoltagePrimario.TabIndex = 1;
            txtVoltagePrimario.Text = "220";
            txtVoltagePrimario.TextAlign = HorizontalAlignment.Right;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F);
            label6.Location = new Point(20, 38);
            label6.Name = "label6";
            label6.Size = new Size(63, 15);
            label6.TabIndex = 0;
            label6.Text = "Voltaje (V):";
            // 
            // btnCalcular
            // 
            btnCalcular.BackColor = Color.FromArgb(0, 122, 204);
            btnCalcular.FlatAppearance.BorderSize = 0;
            btnCalcular.FlatStyle = FlatStyle.Flat;
            btnCalcular.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCalcular.ForeColor = Color.White;
            btnCalcular.Location = new Point(490, 400);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new Size(200, 50);
            btnCalcular.TabIndex = 1;
            btnCalcular.Text = "CALCULAR";
            btnCalcular.UseVisualStyleBackColor = false;
            btnCalcular.Click += btnCalcular_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.FromArgb(108, 117, 125);
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLimpiar.ForeColor = Color.White;
            btnLimpiar.Location = new Point(730, 400);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(200, 50);
            btnLimpiar.TabIndex = 0;
            btnLimpiar.Text = "LIMPIAR";
            btnLimpiar.UseVisualStyleBackColor = false;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // tabResultados
            // 
            tabResultados.BackColor = SystemColors.Control;
            tabResultados.Controls.Add(dgvResultados);
            tabResultados.Location = new Point(4, 24);
            tabResultados.Name = "tabResultados";
            tabResultados.Padding = new Padding(3);
            tabResultados.Size = new Size(952, 512);
            tabResultados.TabIndex = 1;
            tabResultados.Text = "Resultados de Cálculo";
            // 
            // dgvResultados
            // 
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultados.BackgroundColor = Color.White;
            dgvResultados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResultados.Columns.AddRange(new DataGridViewColumn[] { colParametro, colValor, colUnidad, colObservacion });
            dgvResultados.Location = new Point(10, 10);
            dgvResultados.Name = "dgvResultados";
            dgvResultados.ReadOnly = true;
            dgvResultados.RowHeadersVisible = false;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.Size = new Size(932, 492);
            dgvResultados.TabIndex = 0;
            // 
            // colParametro
            // 
            colParametro.HeaderText = "Parámetro";
            colParametro.Name = "colParametro";
            colParametro.ReadOnly = true;
            // 
            // colValor
            // 
            colValor.HeaderText = "Valor";
            colValor.Name = "colValor";
            colValor.ReadOnly = true;
            // 
            // colUnidad
            // 
            colUnidad.HeaderText = "Unidad";
            colUnidad.Name = "colUnidad";
            colUnidad.ReadOnly = true;
            // 
            // colObservacion
            // 
            colObservacion.HeaderText = "Observación";
            colObservacion.Name = "colObservacion";
            colObservacion.ReadOnly = true;
            // 
            // tabAnalisisTermico
            // 
            tabAnalisisTermico.BackColor = SystemColors.Control;
            tabAnalisisTermico.Controls.Add(dgvTermico);
            tabAnalisisTermico.Controls.Add(txtRecomendacionesTermicas);
            tabAnalisisTermico.Controls.Add(label15);
            tabAnalisisTermico.Location = new Point(4, 24);
            tabAnalisisTermico.Name = "tabAnalisisTermico";
            tabAnalisisTermico.Size = new Size(952, 512);
            tabAnalisisTermico.TabIndex = 2;
            tabAnalisisTermico.Text = "Análisis Térmico";
            // 
            // dgvTermico
            // 
            dgvTermico.AllowUserToAddRows = false;
            dgvTermico.AllowUserToDeleteRows = false;
            dgvTermico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvTermico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTermico.BackgroundColor = Color.White;
            dgvTermico.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTermico.Columns.AddRange(new DataGridViewColumn[] { colEscenario, colCoeficiente, colDeltaT, colTempOperacion, colTempHotspot, colEstado });
            dgvTermico.Location = new Point(10, 10);
            dgvTermico.Name = "dgvTermico";
            dgvTermico.ReadOnly = true;
            dgvTermico.RowHeadersVisible = false;
            dgvTermico.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTermico.Size = new Size(932, 250);
            dgvTermico.TabIndex = 2;
            // 
            // colEscenario
            // 
            colEscenario.HeaderText = "Escenario";
            colEscenario.Name = "colEscenario";
            colEscenario.ReadOnly = true;
            // 
            // colCoeficiente
            // 
            colCoeficiente.HeaderText = "h [W/m²K]";
            colCoeficiente.Name = "colCoeficiente";
            colCoeficiente.ReadOnly = true;
            // 
            // colDeltaT
            // 
            colDeltaT.HeaderText = "ΔT [°C]";
            colDeltaT.Name = "colDeltaT";
            colDeltaT.ReadOnly = true;
            // 
            // colTempOperacion
            // 
            colTempOperacion.HeaderText = "T_op [°C]";
            colTempOperacion.Name = "colTempOperacion";
            colTempOperacion.ReadOnly = true;
            // 
            // colTempHotspot
            // 
            colTempHotspot.HeaderText = "T_hotspot [°C]";
            colTempHotspot.Name = "colTempHotspot";
            colTempHotspot.ReadOnly = true;
            // 
            // colEstado
            // 
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            // 
            // txtRecomendacionesTermicas
            // 
            txtRecomendacionesTermicas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtRecomendacionesTermicas.Font = new Font("Consolas", 9F);
            txtRecomendacionesTermicas.Location = new Point(10, 295);
            txtRecomendacionesTermicas.Multiline = true;
            txtRecomendacionesTermicas.Name = "txtRecomendacionesTermicas";
            txtRecomendacionesTermicas.ReadOnly = true;
            txtRecomendacionesTermicas.ScrollBars = ScrollBars.Vertical;
            txtRecomendacionesTermicas.Size = new Size(932, 207);
            txtRecomendacionesTermicas.TabIndex = 1;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label15.Location = new Point(10, 272);
            label15.Name = "label15";
            label15.Size = new Size(204, 15);
            label15.TabIndex = 0;
            label15.Text = "Recomendaciones y Observaciones:";
            // 
            // tabReporte
            // 
            tabReporte.BackColor = SystemColors.Control;
            tabReporte.Controls.Add(txtReporte);
            tabReporte.Controls.Add(btnVistaPrevia);
            tabReporte.Controls.Add(btnExportarTxt);
            tabReporte.Controls.Add(btnExportarExcel);
            tabReporte.Location = new Point(4, 24);
            tabReporte.Name = "tabReporte";
            tabReporte.Size = new Size(952, 512);
            tabReporte.TabIndex = 3;
            tabReporte.Text = "Reporte Completo";
            // 
            // txtReporte
            // 
            txtReporte.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReporte.Font = new Font("Consolas", 9F);
            txtReporte.Location = new Point(10, 10);
            txtReporte.Multiline = true;
            txtReporte.Name = "txtReporte";
            txtReporte.ReadOnly = true;
            txtReporte.ScrollBars = ScrollBars.Both;
            txtReporte.Size = new Size(932, 432);
            txtReporte.TabIndex = 2;
            txtReporte.WordWrap = false;
            // 
            // btnExportarTxt
            // 
            btnExportarTxt.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExportarTxt.BackColor = Color.FromArgb(40, 167, 69);
            btnExportarTxt.FlatAppearance.BorderSize = 0;
            btnExportarTxt.FlatStyle = FlatStyle.Flat;
            btnExportarTxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportarTxt.ForeColor = Color.White;
            btnExportarTxt.Location = new Point(562, 458);
            btnExportarTxt.Name = "btnExportarTxt";
            btnExportarTxt.Size = new Size(180, 40);
            btnExportarTxt.TabIndex = 1;
            btnExportarTxt.Text = "Exportar a TXT";
            btnExportarTxt.UseVisualStyleBackColor = false;
            btnExportarTxt.Click += btnExportarTxt_Click;
            // 
            // btnExportarExcel
            // 
            btnExportarExcel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExportarExcel.BackColor = Color.FromArgb(0, 122, 204);
            btnExportarExcel.FlatAppearance.BorderSize = 0;
            btnExportarExcel.FlatStyle = FlatStyle.Flat;
            btnExportarExcel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportarExcel.ForeColor = Color.White;
            btnExportarExcel.Location = new Point(762, 458);
            btnExportarExcel.Name = "btnExportarExcel";
            btnExportarExcel.Size = new Size(180, 40);
            btnExportarExcel.TabIndex = 0;
            btnExportarExcel.Text = "Exportar a Excel";
            btnExportarExcel.UseVisualStyleBackColor = false;
            btnExportarExcel.Click += btnExportarExcel_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 608);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(984, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(32, 17);
            toolStripStatusLabel1.Text = "Listo";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            toolStripProgressBar1.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(0, 122, 204);
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(554, 30);
            label1.TabIndex = 2;
            label1.Text = "CALCULADORA DE TRANSFORMADOR DE POTENCIA";
            // 
            // btnVistaPrevia
            // 
            btnVistaPrevia.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnVistaPrevia.BackColor = Color.FromArgb(40, 167, 69);
            btnVistaPrevia.FlatAppearance.BorderSize = 0;
            btnVistaPrevia.FlatStyle = FlatStyle.Flat;
            btnVistaPrevia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnVistaPrevia.ForeColor = Color.White;
            btnVistaPrevia.Location = new Point(370, 458);
            btnVistaPrevia.Name = "btnVistaPrevia";
            btnVistaPrevia.Size = new Size(180, 40);
            btnVistaPrevia.TabIndex = 1;
            btnVistaPrevia.Text = "Vista Previa";
            btnVistaPrevia.UseVisualStyleBackColor = false;
            btnVistaPrevia.Click += btnVistaPrevia_Click;
            // 
            // TransformadorCalculatorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 630);
            Controls.Add(label1);
            Controls.Add(statusStrip1);
            Controls.Add(tabControl1);
            MinimumSize = new Size(1000, 669);
            Name = "TransformadorCalculatorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Calculadora de Transformador de Potencia v2.1";
            tabControl1.ResumeLayout(false);
            tabEntrada.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabResultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResultados).EndInit();
            tabAnalisisTermico.ResumeLayout(false);
            tabAnalisisTermico.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTermico).EndInit();
            tabReporte.ResumeLayout(false);
            tabReporte.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabEntrada;
        private System.Windows.Forms.TabPage tabResultados;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtVoltagePrimario;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNumDevanadosPrim;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtCorrienteSec1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtVoltageSec1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkSec2Enabled;
        private System.Windows.Forms.TextBox txtCorrienteSec2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtVoltageSec2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtFrecuencia;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtDensidadFlujo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtAreaNucleo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnCalculadoraNucleo;
        private System.Windows.Forms.DataGridView dgvResultados;
        private System.Windows.Forms.TabPage tabAnalisisTermico;
        private System.Windows.Forms.TabPage tabReporte;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvTermico;
        private System.Windows.Forms.TextBox txtRecomendacionesTermicas;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtReporte;
        private System.Windows.Forms.Button btnExportarTxt;
        private System.Windows.Forms.Button btnExportarExcel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParametro;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colObservacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEscenario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCoeficiente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeltaT;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTempOperacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTempHotspot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private Button btnVistaPrevia;
    }
}