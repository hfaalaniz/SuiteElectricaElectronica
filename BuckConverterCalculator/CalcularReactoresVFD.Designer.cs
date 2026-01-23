using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    partial class CalcularReactoresVFD
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.cboTipo = new System.Windows.Forms.ComboBox();
            this.cboFrecuencia = new System.Windows.Forms.ComboBox();
            this.cboFases = new System.Windows.Forms.ComboBox();
            this.cboImpedancia = new System.Windows.Forms.ComboBox();
            this.txtPotencia = new System.Windows.Forms.TextBox();
            this.txtVoltaje = new System.Windows.Forms.TextBox();
            this.txtCorriente = new System.Windows.Forms.TextBox();
            this.txtInductancia = new System.Windows.Forms.TextBox();
            this.txtVoltajeCaida = new System.Windows.Forms.TextBox();
            this.txtReactancia = new System.Windows.Forms.TextBox();
            this.txtNucleo = new System.Windows.Forms.TextBox();
            this.txtEspiras = new System.Windows.Forms.TextBox();
            this.txtSeccionCable = new System.Windows.Forms.TextBox();
            this.txtDiametroCable = new System.Windows.Forms.TextBox();
            this.txtPesoAprox = new System.Windows.Forms.TextBox();
            this.txtLongitudCable = new System.Windows.Forms.TextBox();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.lblResultados = new System.Windows.Forms.Label();
            this.panelResultados = new System.Windows.Forms.Panel();
            this.panelDiseno = new System.Windows.Forms.Panel();
            this.panelEntrada = new System.Windows.Forms.Panel();
            this.panelGrafico = new System.Windows.Forms.Panel();
            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1100, 820);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CalcularReactoresVFD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculadora de Reactores de Línea VFD - Avanzada";

            // panelEntrada
            this.panelEntrada.BackColor = System.Drawing.Color.White;
            this.panelEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEntrada.Location = new System.Drawing.Point(10, 15);
            this.panelEntrada.Name = "panelEntrada";
            this.panelEntrada.Size = new System.Drawing.Size(715, 260);
            this.panelEntrada.TabIndex = 0;

            // lblTituloEntrada
            var lblTituloEntrada = new Label
            {
                Text = "PARÁMETROS DE ENTRADA",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            this.panelEntrada.Controls.Add(lblTituloEntrada);

            // Tipo de reactor
            var lblTipo = CreateLabel("Tipo de Reactor:", 20, 35);
            this.panelEntrada.Controls.Add(lblTipo);

            this.cboTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipo.FormattingEnabled = true;
            this.cboTipo.Items.AddRange(new object[] {
                "Entrada (AC Line)",
                "Salida (Load)",
                "DC Bus"});
            this.cboTipo.Location = new System.Drawing.Point(180, 35);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(220, 23);
            this.cboTipo.TabIndex = 0;
            this.cboTipo.SelectedIndex = 0;
            this.panelEntrada.Controls.Add(this.cboTipo);

            // Frecuencia
            var lblFrecuencia = CreateLabel("Frecuencia (Hz):", 20, 70);
            this.panelEntrada.Controls.Add(lblFrecuencia);

            this.cboFrecuencia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrecuencia.FormattingEnabled = true;
            this.cboFrecuencia.Items.AddRange(new object[] { "50", "60" });
            this.cboFrecuencia.Location = new System.Drawing.Point(180, 70);
            this.cboFrecuencia.Name = "cboFrecuencia";
            this.cboFrecuencia.Size = new System.Drawing.Size(100, 23);
            this.cboFrecuencia.TabIndex = 1;
            this.cboFrecuencia.SelectedIndex = 1;
            this.panelEntrada.Controls.Add(this.cboFrecuencia);

            // Sistema
            var lblFases = CreateLabel("Sistema:", 320, 70);
            this.panelEntrada.Controls.Add(lblFases);

            this.cboFases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFases.FormattingEnabled = true;
            this.cboFases.Items.AddRange(new object[] { "Monofásico", "Trifásico" });
            this.cboFases.Location = new System.Drawing.Point(420, 70);
            this.cboFases.Name = "cboFases";
            this.cboFases.Size = new System.Drawing.Size(130, 23);
            this.cboFases.TabIndex = 2;
            this.cboFases.SelectedIndex = 1;
            this.panelEntrada.Controls.Add(this.cboFases);

            // Potencia
            var lblPotencia = CreateLabel("Potencia Motor (kW):", 20, 105);
            this.panelEntrada.Controls.Add(lblPotencia);

            this.txtPotencia.Location = new System.Drawing.Point(180, 105);
            this.txtPotencia.Name = "txtPotencia";
            this.txtPotencia.Size = new System.Drawing.Size(100, 23);
            this.txtPotencia.TabIndex = 3;
            this.panelEntrada.Controls.Add(this.txtPotencia);

            // Voltaje
            var lblVoltaje = CreateLabel("Voltaje (V):", 320, 105);
            this.panelEntrada.Controls.Add(lblVoltaje);

            this.txtVoltaje.Location = new System.Drawing.Point(420, 105);
            this.txtVoltaje.Name = "txtVoltaje";
            this.txtVoltaje.Size = new System.Drawing.Size(100, 23);
            this.txtVoltaje.TabIndex = 4;
            this.txtVoltaje.Text = "380";
            this.panelEntrada.Controls.Add(this.txtVoltaje);

            // Impedancia
            var lblImpedancia = CreateLabel("Impedancia (%):", 20, 140);
            this.panelEntrada.Controls.Add(lblImpedancia);

            this.cboImpedancia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImpedancia.FormattingEnabled = true;
            this.cboImpedancia.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "7", "10" });
            this.cboImpedancia.Location = new System.Drawing.Point(180, 140);
            this.cboImpedancia.Name = "cboImpedancia";
            this.cboImpedancia.Size = new System.Drawing.Size(100, 23);
            this.cboImpedancia.TabIndex = 5;
            this.cboImpedancia.SelectedIndex = 4;
            this.panelEntrada.Controls.Add(this.cboImpedancia);

            var lblImpedanciaInfo = CreateLabel("(3% salida, 5% entrada típico)", 290, 140);
            lblImpedanciaInfo.ForeColor = Color.Gray;
            lblImpedanciaInfo.Font = new Font("Segoe UI", 8F);
            this.panelEntrada.Controls.Add(lblImpedanciaInfo);

            // Botones
            this.btnCalcular.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnCalcular.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCalcular.FlatAppearance.BorderSize = 0;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalcular.ForeColor = System.Drawing.Color.White;
            this.btnCalcular.Location = new System.Drawing.Point(180, 185);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(100, 35);
            this.btnCalcular.TabIndex = 6;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcular_Click);
            this.panelEntrada.Controls.Add(this.btnCalcular);

            this.btnLimpiar.BackColor = System.Drawing.Color.Gray;
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(290, 185);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 35);
            this.btnLimpiar.TabIndex = 7;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);
            this.panelEntrada.Controls.Add(this.btnLimpiar);

            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(400, 185);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(100, 35);
            this.btnExportar.TabIndex = 8;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.BtnExportar_Click);
            this.panelEntrada.Controls.Add(this.btnExportar);

            // panelResultados
            this.panelResultados.BackColor = System.Drawing.Color.White;
            this.panelResultados.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResultados.Location = new System.Drawing.Point(10, 285);
            this.panelResultados.Name = "panelResultados";
            this.panelResultados.Size = new System.Drawing.Size(715, 220);
            this.panelResultados.TabIndex = 1;

            this.lblResultados.AutoSize = true;
            this.lblResultados.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResultados.Location = new System.Drawing.Point(10, 8);
            this.lblResultados.Name = "lblResultados";
            this.lblResultados.Size = new System.Drawing.Size(186, 19);
            this.lblResultados.TabIndex = 0;
            this.lblResultados.Text = "RESULTADOS ELÉCTRICOS";
            this.panelResultados.Controls.Add(this.lblResultados);

            InitializeResultadosPanel();

            // panelDiseno
            this.panelDiseno.BackColor = System.Drawing.Color.White;
            this.panelDiseno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDiseno.Location = new System.Drawing.Point(10, 515);
            this.panelDiseno.Name = "panelDiseno";
            this.panelDiseno.Size = new System.Drawing.Size(715, 260);
            this.panelDiseno.TabIndex = 2;

            var lblDiseno = new Label
            {
                Text = "DISEÑO FÍSICO APROXIMADO",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            this.panelDiseno.Controls.Add(lblDiseno);

            InitializeDisenoPanel();

            // panelGrafico
            this.panelGrafico.BackColor = System.Drawing.Color.White;
            this.panelGrafico.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGrafico.Location = new System.Drawing.Point(735, 15);
            this.panelGrafico.Name = "panelGrafico";
            this.panelGrafico.Size = new System.Drawing.Size(350, 760);
            this.panelGrafico.TabIndex = 3;
            this.panelGrafico.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelGrafico_Paint);

            var lblGrafico = new Label
            {
                Text = "DISEÑO GRÁFICO",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            this.panelGrafico.Controls.Add(lblGrafico);

            // Add controls to form
            this.Controls.Add(this.panelEntrada);
            this.Controls.Add(this.panelResultados);
            this.Controls.Add(this.panelDiseno);
            this.Controls.Add(this.panelGrafico);

            this.ResumeLayout(false);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSize = true
            };
        }

        private TextBox CreateReadOnlyTextBox(int x, int y, int width, string tag = null)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = tag
            };
            return txt;
        }

        private void InitializeResultadosPanel()
        {
            int y = 35;
            int leftMargin = 20;

            var lblCorriente = CreateLabel("Corriente Nominal (A):", leftMargin, y);
            this.panelResultados.Controls.Add(lblCorriente);

            this.txtCorriente = CreateReadOnlyTextBox(180, y, 100);
            this.txtCorriente.Name = "txtCorriente";
            this.panelResultados.Controls.Add(this.txtCorriente);

            var lblReactancia = CreateLabel("Reactancia (Ω):", 320, y);
            this.panelResultados.Controls.Add(lblReactancia);

            this.txtReactancia = CreateReadOnlyTextBox(450, y, 100);
            this.txtReactancia.Name = "txtReactancia";
            this.panelResultados.Controls.Add(this.txtReactancia);

            y += 35;
            var lblInductancia = CreateLabel("Inductancia (mH):", leftMargin, y);
            this.panelResultados.Controls.Add(lblInductancia);

            this.txtInductancia = CreateReadOnlyTextBox(180, y, 100);
            this.txtInductancia.Name = "txtInductancia";
            this.panelResultados.Controls.Add(this.txtInductancia);

            var lblInductanciaH = CreateLabel("Inductancia (H):", 320, y);
            this.panelResultados.Controls.Add(lblInductanciaH);

            var txtInductanciaH = CreateReadOnlyTextBox(450, y, 100, "inductanciaH");
            txtInductanciaH.Name = "txtInductanciaH";
            this.panelResultados.Controls.Add(txtInductanciaH);

            y += 35;
            var lblVoltajeCaida = CreateLabel("Caída Voltaje (V):", leftMargin, y);
            this.panelResultados.Controls.Add(lblVoltajeCaida);

            this.txtVoltajeCaida = CreateReadOnlyTextBox(180, y, 100);
            this.txtVoltajeCaida.Name = "txtVoltajeCaida";
            this.panelResultados.Controls.Add(this.txtVoltajeCaida);

            var lblCaidaPct = CreateLabel("Caída (%):", 320, y);
            this.panelResultados.Controls.Add(lblCaidaPct);

            var txtCaidaPorcentaje = CreateReadOnlyTextBox(450, y, 100, "caidaPct");
            txtCaidaPorcentaje.Name = "txtCaidaPorcentaje";
            this.panelResultados.Controls.Add(txtCaidaPorcentaje);

            y += 35;
            var lblPerdidas = CreateLabel("Pérdidas (W):", leftMargin, y);
            this.panelResultados.Controls.Add(lblPerdidas);

            var txtPerdidas = CreateReadOnlyTextBox(180, y, 100, "perdidas");
            txtPerdidas.Name = "txtPerdidas";
            this.panelResultados.Controls.Add(txtPerdidas);

            var lblEnergia = CreateLabel("Energía (kWh/año):", 320, y);
            this.panelResultados.Controls.Add(lblEnergia);

            var txtEnergia = CreateReadOnlyTextBox(450, y, 100, "energia");
            txtEnergia.Name = "txtEnergia";
            this.panelResultados.Controls.Add(txtEnergia);
        }

        private void InitializeDisenoPanel()
        {
            int y = 35;
            int leftMargin = 20;

            var lblNucleo = CreateLabel("Núcleo (mm):", leftMargin, y);
            this.panelDiseno.Controls.Add(lblNucleo);

            this.txtNucleo = CreateReadOnlyTextBox(180, y, 150);
            this.txtNucleo.Name = "txtNucleo";
            this.panelDiseno.Controls.Add(this.txtNucleo);

            var lblArea = CreateLabel("Área (cm²):", 370, y);
            this.panelDiseno.Controls.Add(lblArea);

            var txtArea = CreateReadOnlyTextBox(480, y, 100, "area");
            txtArea.Name = "txtArea";
            this.panelDiseno.Controls.Add(txtArea);

            y += 35;
            var lblEspiras = CreateLabel("Espiras por fase:", leftMargin, y);
            this.panelDiseno.Controls.Add(lblEspiras);

            this.txtEspiras = CreateReadOnlyTextBox(180, y, 100);
            this.txtEspiras.Name = "txtEspiras";
            this.panelDiseno.Controls.Add(this.txtEspiras);

            var lblVentana = CreateLabel("Ventana (cm²):", 320, y);
            this.panelDiseno.Controls.Add(lblVentana);

            var txtVentana = CreateReadOnlyTextBox(480, y, 100, "ventana");
            txtVentana.Name = "txtVentana";
            this.panelDiseno.Controls.Add(txtVentana);

            y += 35;
            var lblSeccionCable = CreateLabel("Sección cable (mm²):", leftMargin, y);
            this.panelDiseno.Controls.Add(lblSeccionCable);

            this.txtSeccionCable = CreateReadOnlyTextBox(180, y, 100);
            this.txtSeccionCable.Name = "txtSeccionCable";
            this.panelDiseno.Controls.Add(this.txtSeccionCable);

            var lblAWG = CreateLabel("AWG equiv.:", 320, y);
            this.panelDiseno.Controls.Add(lblAWG);

            var txtAWG = CreateReadOnlyTextBox(480, y, 100, "awg");
            txtAWG.Name = "txtAWG";
            this.panelDiseno.Controls.Add(txtAWG);

            y += 35;
            var lblDiametroCable = CreateLabel("Diámetro cable (mm):", leftMargin, y);
            this.panelDiseno.Controls.Add(lblDiametroCable);

            this.txtDiametroCable = CreateReadOnlyTextBox(180, y, 100);
            this.txtDiametroCable.Name = "txtDiametroCable";
            this.panelDiseno.Controls.Add(this.txtDiametroCable);

            var lblLongitudCable = CreateLabel("Long. cable (m):", 320, y);
            this.panelDiseno.Controls.Add(lblLongitudCable);

            this.txtLongitudCable = CreateReadOnlyTextBox(480, y, 100);
            this.txtLongitudCable.Name = "txtLongitudCable";
            this.panelDiseno.Controls.Add(this.txtLongitudCable);

            y += 35;
            var lblPesoAprox = CreateLabel("Peso aprox. (kg):", leftMargin, y);
            this.panelDiseno.Controls.Add(lblPesoAprox);

            this.txtPesoAprox = CreateReadOnlyTextBox(180, y, 100);
            this.txtPesoAprox.Name = "txtPesoAprox";
            this.panelDiseno.Controls.Add(this.txtPesoAprox);

            var lblTemp = CreateLabel("Temp. rise (°C):", 320, y);
            this.panelDiseno.Controls.Add(lblTemp);

            var txtTemp = CreateReadOnlyTextBox(480, y, 100, "temp");
            txtTemp.Name = "txtTemp";
            this.panelDiseno.Controls.Add(txtTemp);
        }

        #endregion

        private ComboBox cboTipo;
        private ComboBox cboFrecuencia;
        private ComboBox cboFases;
        private ComboBox cboImpedancia;
        private TextBox txtPotencia;
        private TextBox txtVoltaje;
        private TextBox txtCorriente;
        private TextBox txtInductancia;
        private TextBox txtVoltajeCaida;
        private TextBox txtReactancia;
        private TextBox txtNucleo;
        private TextBox txtEspiras;
        private TextBox txtSeccionCable;
        private TextBox txtDiametroCable;
        private TextBox txtPesoAprox;
        private TextBox txtLongitudCable;
        private Button btnCalcular;
        private Button btnLimpiar;
        private Button btnExportar;
        private Label lblResultados;
        private Panel panelResultados;
        private Panel panelDiseno;
        private Panel panelEntrada;
        private Panel panelGrafico;
    }
}