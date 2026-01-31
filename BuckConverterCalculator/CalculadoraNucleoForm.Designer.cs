namespace BuckConverterCalculator
{
    partial class CalculadoraNucleoForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPorPotencia = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblNucleoSugerido = new System.Windows.Forms.Label();
            this.lblAreaRecomendada = new System.Windows.Forms.Label();
            this.lblAreaMinima = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvNucleosSugeridos = new System.Windows.Forms.DataGridView();
            this.colModelo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAw = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPotencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDimensiones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDensidadFlujo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFrecuencia = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCalcularPorPotencia = new System.Windows.Forms.Button();
            this.txtPotencia = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPorMedidas = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblResultadoMedidas = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFactorApilamiento = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCalcularMedidas = new System.Windows.Forms.Button();
            this.txtEspesorStack = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAnchoColumna = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabNucleosDisponibles = new System.Windows.Forms.TabPage();
            this.dgvNucleosDisponibles = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLongitud = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblAreaSeleccionada = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPorPotencia.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNucleosSugeridos)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPorMedidas.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabNucleosDisponibles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNucleosDisponibles)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPorPotencia);
            this.tabControl1.Controls.Add(this.tabPorMedidas);
            this.tabControl1.Controls.Add(this.tabNucleosDisponibles);
            this.tabControl1.Location = new System.Drawing.Point(12, 60);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 470);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPorPotencia
            // 
            this.tabPorPotencia.BackColor = System.Drawing.SystemColors.Control;
            this.tabPorPotencia.Controls.Add(this.groupBox3);
            this.tabPorPotencia.Controls.Add(this.dgvNucleosSugeridos);
            this.tabPorPotencia.Controls.Add(this.label8);
            this.tabPorPotencia.Controls.Add(this.groupBox1);
            this.tabPorPotencia.Location = new System.Drawing.Point(4, 24);
            this.tabPorPotencia.Name = "tabPorPotencia";
            this.tabPorPotencia.Padding = new System.Windows.Forms.Padding(3);
            this.tabPorPotencia.Size = new System.Drawing.Size(752, 442);
            this.tabPorPotencia.TabIndex = 0;
            this.tabPorPotencia.Text = "Calcular por Potencia";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblNucleoSugerido);
            this.groupBox3.Controls.Add(this.lblAreaRecomendada);
            this.groupBox3.Controls.Add(this.lblAreaMinima);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(20, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 140);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Resultados";
            // 
            // lblNucleoSugerido
            // 
            this.lblNucleoSugerido.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.lblNucleoSugerido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblNucleoSugerido.Location = new System.Drawing.Point(150, 95);
            this.lblNucleoSugerido.Name = "lblNucleoSugerido";
            this.lblNucleoSugerido.Size = new System.Drawing.Size(180, 20);
            this.lblNucleoSugerido.TabIndex = 5;
            this.lblNucleoSugerido.Text = "-";
            // 
            // lblAreaRecomendada
            // 
            this.lblAreaRecomendada.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.lblAreaRecomendada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblAreaRecomendada.Location = new System.Drawing.Point(150, 65);
            this.lblAreaRecomendada.Name = "lblAreaRecomendada";
            this.lblAreaRecomendada.Size = new System.Drawing.Size(180, 20);
            this.lblAreaRecomendada.TabIndex = 4;
            this.lblAreaRecomendada.Text = "-";
            // 
            // lblAreaMinima
            // 
            this.lblAreaMinima.Font = new System.Drawing.Font("Consolas", 9F);
            this.lblAreaMinima.Location = new System.Drawing.Point(150, 35);
            this.lblAreaMinima.Name = "lblAreaMinima";
            this.lblAreaMinima.Size = new System.Drawing.Size(180, 20);
            this.lblAreaMinima.TabIndex = 3;
            this.lblAreaMinima.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(20, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Núcleo Sugerido:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(20, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Área Recomendada:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(20, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Área Mínima:";
            // 
            // dgvNucleosSugeridos
            // 
            this.dgvNucleosSugeridos.AllowUserToAddRows = false;
            this.dgvNucleosSugeridos.AllowUserToDeleteRows = false;
            this.dgvNucleosSugeridos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNucleosSugeridos.BackgroundColor = System.Drawing.Color.White;
            this.dgvNucleosSugeridos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNucleosSugeridos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colModelo,
            this.colAe,
            this.colAw,
            this.colPotencia,
            this.colDimensiones});
            this.dgvNucleosSugeridos.Location = new System.Drawing.Point(20, 330);
            this.dgvNucleosSugeridos.Name = "dgvNucleosSugeridos";
            this.dgvNucleosSugeridos.ReadOnly = true;
            this.dgvNucleosSugeridos.RowHeadersVisible = false;
            this.dgvNucleosSugeridos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNucleosSugeridos.Size = new System.Drawing.Size(712, 95);
            this.dgvNucleosSugeridos.TabIndex = 3;
            this.dgvNucleosSugeridos.SelectionChanged += new System.EventHandler(this.dgvNucleosSugeridos_SelectionChanged);
            // 
            // colModelo
            // 
            this.colModelo.HeaderText = "Modelo";
            this.colModelo.Name = "colModelo";
            this.colModelo.ReadOnly = true;
            // 
            // colAe
            // 
            this.colAe.HeaderText = "Ae [cm²]";
            this.colAe.Name = "colAe";
            this.colAe.ReadOnly = true;
            // 
            // colAw
            // 
            this.colAw.HeaderText = "Aw [cm²]";
            this.colAw.Name = "colAw";
            this.colAw.ReadOnly = true;
            // 
            // colPotencia
            // 
            this.colPotencia.HeaderText = "Potencia [VA]";
            this.colPotencia.Name = "colPotencia";
            this.colPotencia.ReadOnly = true;
            // 
            // colDimensiones
            // 
            this.colDimensiones.HeaderText = "Dimensiones";
            this.colDimensiones.Name = "colDimensiones";
            this.colDimensiones.ReadOnly = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(20, 310);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Núcleos Comerciales Sugeridos:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDensidadFlujo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtFrecuencia);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnCalcularPorPotencia);
            this.groupBox1.Controls.Add(this.txtPotencia);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(20, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos de Entrada";
            // 
            // txtDensidadFlujo
            // 
            this.txtDensidadFlujo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDensidadFlujo.Location = new System.Drawing.Point(250, 62);
            this.txtDensidadFlujo.Name = "txtDensidadFlujo";
            this.txtDensidadFlujo.Size = new System.Drawing.Size(80, 23);
            this.txtDensidadFlujo.TabIndex = 6;
            this.txtDensidadFlujo.Text = "1.5";
            this.txtDensidadFlujo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.Location = new System.Drawing.Point(185, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "B máx (T):";
            // 
            // txtFrecuencia
            // 
            this.txtFrecuencia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFrecuencia.Location = new System.Drawing.Point(90, 62);
            this.txtFrecuencia.Name = "txtFrecuencia";
            this.txtFrecuencia.Size = new System.Drawing.Size(80, 23);
            this.txtFrecuencia.TabIndex = 4;
            this.txtFrecuencia.Text = "60";
            this.txtFrecuencia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(20, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "Frec (Hz):";
            // 
            // btnCalcularPorPotencia
            // 
            this.btnCalcularPorPotencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnCalcularPorPotencia.FlatAppearance.BorderSize = 0;
            this.btnCalcularPorPotencia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalcularPorPotencia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCalcularPorPotencia.ForeColor = System.Drawing.Color.White;
            this.btnCalcularPorPotencia.Location = new System.Drawing.Point(250, 28);
            this.btnCalcularPorPotencia.Name = "btnCalcularPorPotencia";
            this.btnCalcularPorPotencia.Size = new System.Drawing.Size(80, 23);
            this.btnCalcularPorPotencia.TabIndex = 2;
            this.btnCalcularPorPotencia.Text = "Calcular";
            this.btnCalcularPorPotencia.UseVisualStyleBackColor = false;
            this.btnCalcularPorPotencia.Click += new System.EventHandler(this.btnCalcularPorPotencia_Click);
            // 
            // txtPotencia
            // 
            this.txtPotencia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPotencia.Location = new System.Drawing.Point(90, 28);
            this.txtPotencia.Name = "txtPotencia";
            this.txtPotencia.Size = new System.Drawing.Size(150, 23);
            this.txtPotencia.TabIndex = 1;
            this.txtPotencia.Text = "6600";
            this.txtPotencia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(20, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Potencia (VA):";
            // 
            // tabPorMedidas
            // 
            this.tabPorMedidas.BackColor = System.Drawing.SystemColors.Control;
            this.tabPorMedidas.Controls.Add(this.groupBox4);
            this.tabPorMedidas.Controls.Add(this.groupBox2);
            this.tabPorMedidas.Controls.Add(this.label9);
            this.tabPorMedidas.Location = new System.Drawing.Point(4, 24);
            this.tabPorMedidas.Name = "tabPorMedidas";
            this.tabPorMedidas.Padding = new System.Windows.Forms.Padding(3);
            this.tabPorMedidas.Size = new System.Drawing.Size(752, 442);
            this.tabPorMedidas.TabIndex = 1;
            this.tabPorMedidas.Text = "Calcular por Medidas";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblResultadoMedidas);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox4.Location = new System.Drawing.Point(20, 220);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(400, 100);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Resultado";
            // 
            // lblResultadoMedidas
            // 
            this.lblResultadoMedidas.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.lblResultadoMedidas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblResultadoMedidas.Location = new System.Drawing.Point(180, 45);
            this.lblResultadoMedidas.Name = "lblResultadoMedidas";
            this.lblResultadoMedidas.Size = new System.Drawing.Size(200, 25);
            this.lblResultadoMedidas.TabIndex = 1;
            this.lblResultadoMedidas.Text = "-";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label13.Location = new System.Drawing.Point(20, 45);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(154, 19);
            this.label13.TabIndex = 0;
            this.label13.Text = "Área del Núcleo (Ae):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtFactorApilamiento);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.btnCalcularMedidas);
            this.groupBox2.Controls.Add(this.txtEspesorStack);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtAnchoColumna);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(20, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 130);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Medidas del Núcleo (Tipo EI)";
            // 
            // txtFactorApilamiento
            // 
            this.txtFactorApilamiento.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFactorApilamiento.Location = new System.Drawing.Point(200, 85);
            this.txtFactorApilamiento.Name = "txtFactorApilamiento";
            this.txtFactorApilamiento.Size = new System.Drawing.Size(100, 23);
            this.txtFactorApilamiento.TabIndex = 6;
            this.txtFactorApilamiento.Text = "0.95";
            this.txtFactorApilamiento.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label12.Location = new System.Drawing.Point(20, 88);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(168, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "Factor de Apilamiento (0.9-1.0):";
            // 
            // btnCalcularMedidas
            // 
            this.btnCalcularMedidas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnCalcularMedidas.FlatAppearance.BorderSize = 0;
            this.btnCalcularMedidas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalcularMedidas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCalcularMedidas.ForeColor = System.Drawing.Color.White;
            this.btnCalcularMedidas.Location = new System.Drawing.Point(310, 85);
            this.btnCalcularMedidas.Name = "btnCalcularMedidas";
            this.btnCalcularMedidas.Size = new System.Drawing.Size(70, 23);
            this.btnCalcularMedidas.TabIndex = 4;
            this.btnCalcularMedidas.Text = "Calcular";
            this.btnCalcularMedidas.UseVisualStyleBackColor = false;
            this.btnCalcularMedidas.Click += new System.EventHandler(this.btnCalcularMedidas_Click);
            // 
            // txtEspesorStack
            // 
            this.txtEspesorStack.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEspesorStack.Location = new System.Drawing.Point(200, 56);
            this.txtEspesorStack.Name = "txtEspesorStack";
            this.txtEspesorStack.Size = new System.Drawing.Size(100, 23);
            this.txtEspesorStack.TabIndex = 3;
            this.txtEspesorStack.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label11.Location = new System.Drawing.Point(20, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(174, 15);
            this.label11.TabIndex = 2;
            this.label11.Text = "Espesor del Stack (d) en cm:";
            // 
            // txtAnchoColumna
            // 
            this.txtAnchoColumna.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAnchoColumna.Location = new System.Drawing.Point(200, 27);
            this.txtAnchoColumna.Name = "txtAnchoColumna";
            this.txtAnchoColumna.Size = new System.Drawing.Size(100, 23);
            this.txtAnchoColumna.TabIndex = 1;
            this.txtAnchoColumna.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.Location = new System.Drawing.Point(20, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(179, 15);
            this.label10.TabIndex = 0;
            this.label10.Text = "Ancho Columna Central (b) en cm:";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label9.Location = new System.Drawing.Point(20, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(700, 40);
            this.label9.TabIndex = 0;
            this.label9.Text = "Mide el ancho de la columna central del núcleo E y el espesor del stack de lámi" +
    "nas.\r\nLa fórmula es: Ae = ancho × espesor × factor_apilamiento";
            // 
            // tabNucleosDisponibles
            // 
            this.tabNucleosDisponibles.BackColor = System.Drawing.SystemColors.Control;
            this.tabNucleosDisponibles.Controls.Add(this.dgvNucleosDisponibles);
            this.tabNucleosDisponibles.Location = new System.Drawing.Point(4, 24);
            this.tabNucleosDisponibles.Name = "tabNucleosDisponibles";
            this.tabNucleosDisponibles.Size = new System.Drawing.Size(752, 442);
            this.tabNucleosDisponibles.TabIndex = 2;
            this.tabNucleosDisponibles.Text = "Núcleos Disponibles";
            // 
            // dgvNucleosDisponibles
            // 
            this.dgvNucleosDisponibles.AllowUserToAddRows = false;
            this.dgvNucleosDisponibles.AllowUserToDeleteRows = false;
            this.dgvNucleosDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNucleosDisponibles.BackgroundColor = System.Drawing.Color.White;
            this.dgvNucleosDisponibles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNucleosDisponibles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.colLongitud,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.dgvNucleosDisponibles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNucleosDisponibles.Location = new System.Drawing.Point(0, 0);
            this.dgvNucleosDisponibles.Name = "dgvNucleosDisponibles";
            this.dgvNucleosDisponibles.ReadOnly = true;
            this.dgvNucleosDisponibles.RowHeadersVisible = false;
            this.dgvNucleosDisponibles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNucleosDisponibles.Size = new System.Drawing.Size(752, 442);
            this.dgvNucleosDisponibles.TabIndex = 0;
            this.dgvNucleosDisponibles.SelectionChanged += new System.EventHandler(this.dgvNucleosDisponibles_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Modelo";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Ae [cm²]";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Aw [cm²]";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // colLongitud
            // 
            this.colLongitud.HeaderText = "Lm [cm]";
            this.colLongitud.Name = "colLongitud";
            this.colLongitud.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Potencia [VA]";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Dimensiones";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "CALCULADORA DE ÁREA DEL NÚCLEO";
            // 
            // btnAplicar
            // 
            this.btnAplicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAplicar.Enabled = false;
            this.btnAplicar.FlatAppearance.BorderSize = 0;
            this.btnAplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAplicar.ForeColor = System.Drawing.Color.White;
            this.btnAplicar.Location = new System.Drawing.Point(560, 548);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(100, 35);
            this.btnAplicar.TabIndex = 2;
            this.btnAplicar.Text = "Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = false;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(672, 548);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 35);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // lblAreaSeleccionada
            // 
            this.lblAreaSeleccionada.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAreaSeleccionada.Location = new System.Drawing.Point(12, 545);
            this.lblAreaSeleccionada.Name = "lblAreaSeleccionada";
            this.lblAreaSeleccionada.Size = new System.Drawing.Size(500, 40);
            this.lblAreaSeleccionada.TabIndex = 4;
            this.lblAreaSeleccionada.Text = "Área seleccionada: -";
            // 
            // CalculadoraNucleoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 595);
            this.Controls.Add(this.lblAreaSeleccionada);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnAplicar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculadoraNucleoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calculadora de Área del Núcleo";
            this.Load += new System.EventHandler(this.CalculadoraNucleoForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPorPotencia.ResumeLayout(false);
            this.tabPorPotencia.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNucleosSugeridos)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPorMedidas.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabNucleosDisponibles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNucleosDisponibles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPorPotencia;
        private System.Windows.Forms.TabPage tabPorMedidas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPotencia;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCalcularPorPotencia;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgvNucleosSugeridos;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblNucleoSugerido;
        private System.Windows.Forms.Label lblAreaRecomendada;
        private System.Windows.Forms.Label lblAreaMinima;
        private System.Windows.Forms.TextBox txtFrecuencia;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDensidadFlujo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtAnchoColumna;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtEspesorStack;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnCalcularMedidas;
        private System.Windows.Forms.TextBox txtFactorApilamiento;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblResultadoMedidas;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TabPage tabNucleosDisponibles;
        private System.Windows.Forms.DataGridView dgvNucleosDisponibles;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblAreaSeleccionada;
        private System.Windows.Forms.DataGridViewTextBoxColumn colModelo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAe;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAw;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPotencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDimensiones;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLongitud;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    }
}
