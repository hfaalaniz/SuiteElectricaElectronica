
namespace BuckConverterCalculator
{
    partial class TransformerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox grpPrimario;
        private System.Windows.Forms.RadioButton rbPrimarioMonofasico;
        private System.Windows.Forms.RadioButton rbPrimarioBifasico;
        private System.Windows.Forms.RadioButton rbPrimarioTrifasicoEstrella;
        private System.Windows.Forms.RadioButton rbPrimarioTrifasicoDelta;
        private System.Windows.Forms.Label lblInfoPrimario;
        private System.Windows.Forms.GroupBox grpSecundario;
        private System.Windows.Forms.RadioButton rbSecundarioMonofasico;
        private System.Windows.Forms.RadioButton rbSecundarioBifasico;
        private System.Windows.Forms.RadioButton rbSecundarioTrifasicoEstrella;
        private System.Windows.Forms.RadioButton rbSecundarioTrifasicoDelta;
        private System.Windows.Forms.Label lblInfoSecundario;
        private System.Windows.Forms.GroupBox grpParametros;
        private System.Windows.Forms.Label lblVoltajePrimario;
        private System.Windows.Forms.TextBox txtVoltajePrimario;
        private System.Windows.Forms.Label lblVoltajeSecundario;
        private System.Windows.Forms.TextBox txtVoltajeSecundario;
        private System.Windows.Forms.Label lblCorrienteSecundaria;
        private System.Windows.Forms.TextBox txtCorrienteSecundaria;
        private System.Windows.Forms.Label lblFrecuencia;
        private System.Windows.Forms.TextBox txtFrecuencia;
        private System.Windows.Forms.Label lblEficiencia;
        private System.Windows.Forms.TextBox txtEficiencia;
        private System.Windows.Forms.GroupBox grpAvanzado;
        private System.Windows.Forms.Label lblDensidadFlujo;
        private System.Windows.Forms.TextBox txtDensidadFlujo;
        private System.Windows.Forms.Label lblDensidadCorriente;
        private System.Windows.Forms.TextBox txtDensidadCorriente;
        private System.Windows.Forms.Label lblFactorApilamiento;
        private System.Windows.Forms.TextBox txtFactorApilamiento;
        private System.Windows.Forms.Label lblFactorLlenado;
        private System.Windows.Forms.TextBox txtFactorLlenado;
        private System.Windows.Forms.Label lblLaminado;
        private System.Windows.Forms.ComboBox cmbLaminado;
        private System.Windows.Forms.Label lblTempAmbiente;
        private System.Windows.Forms.TextBox txtTempAmbiente;
        private System.Windows.Forms.Label lblElevacionTemp;
        private System.Windows.Forms.TextBox txtElevacionTemp;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnExportarDiagrama;
        private System.Windows.Forms.TextBox txtResultados;
        private System.Windows.Forms.PictureBox picDiagramas;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpPrimario = new System.Windows.Forms.GroupBox();
            this.lblInfoPrimario = new System.Windows.Forms.Label();
            this.rbPrimarioTrifasicoDelta = new System.Windows.Forms.RadioButton();
            this.rbPrimarioTrifasicoEstrella = new System.Windows.Forms.RadioButton();
            this.rbPrimarioBifasico = new System.Windows.Forms.RadioButton();
            this.rbPrimarioMonofasico = new System.Windows.Forms.RadioButton();
            this.grpSecundario = new System.Windows.Forms.GroupBox();
            this.lblInfoSecundario = new System.Windows.Forms.Label();
            this.rbSecundarioTrifasicoDelta = new System.Windows.Forms.RadioButton();
            this.rbSecundarioTrifasicoEstrella = new System.Windows.Forms.RadioButton();
            this.rbSecundarioBifasico = new System.Windows.Forms.RadioButton();
            this.rbSecundarioMonofasico = new System.Windows.Forms.RadioButton();
            this.grpParametros = new System.Windows.Forms.GroupBox();
            this.txtEficiencia = new System.Windows.Forms.TextBox();
            this.lblEficiencia = new System.Windows.Forms.Label();
            this.txtFrecuencia = new System.Windows.Forms.TextBox();
            this.lblFrecuencia = new System.Windows.Forms.Label();
            this.txtCorrienteSecundaria = new System.Windows.Forms.TextBox();
            this.lblCorrienteSecundaria = new System.Windows.Forms.Label();
            this.txtVoltajeSecundario = new System.Windows.Forms.TextBox();
            this.lblVoltajeSecundario = new System.Windows.Forms.Label();
            this.txtVoltajePrimario = new System.Windows.Forms.TextBox();
            this.lblVoltajePrimario = new System.Windows.Forms.Label();
            this.grpAvanzado = new System.Windows.Forms.GroupBox();
            this.txtElevacionTemp = new System.Windows.Forms.TextBox();
            this.lblElevacionTemp = new System.Windows.Forms.Label();
            this.txtTempAmbiente = new System.Windows.Forms.TextBox();
            this.lblTempAmbiente = new System.Windows.Forms.Label();
            this.cmbLaminado = new System.Windows.Forms.ComboBox();
            this.lblLaminado = new System.Windows.Forms.Label();
            this.txtFactorLlenado = new System.Windows.Forms.TextBox();
            this.lblFactorLlenado = new System.Windows.Forms.Label();
            this.txtDensidadCorriente = new System.Windows.Forms.TextBox();
            this.lblDensidadCorriente = new System.Windows.Forms.Label();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnExportarDiagrama = new System.Windows.Forms.Button();
            this.txtResultados = new System.Windows.Forms.TextBox();
            this.picDiagramas = new System.Windows.Forms.PictureBox();
            this.grpPrimario.SuspendLayout();
            this.grpSecundario.SuspendLayout();
            this.grpParametros.SuspendLayout();
            this.grpAvanzado.SuspendLayout();
            this.SuspendLayout();

            // grpPrimario
            this.grpPrimario.Controls.Add(this.lblInfoPrimario);
            this.grpPrimario.Controls.Add(this.rbPrimarioTrifasicoDelta);
            this.grpPrimario.Controls.Add(this.rbPrimarioTrifasicoEstrella);
            this.grpPrimario.Controls.Add(this.rbPrimarioBifasico);
            this.grpPrimario.Controls.Add(this.rbPrimarioMonofasico);
            this.grpPrimario.Location = new System.Drawing.Point(12, 12);
            this.grpPrimario.Name = "grpPrimario";
            this.grpPrimario.Size = new System.Drawing.Size(400, 105);
            this.grpPrimario.TabIndex = 0;
            this.grpPrimario.TabStop = false;
            this.grpPrimario.Text = "Configuración Primario";

            // rbPrimarioMonofasico
            this.rbPrimarioMonofasico.AutoSize = true;
            this.rbPrimarioMonofasico.Checked = true;
            this.rbPrimarioMonofasico.Location = new System.Drawing.Point(15, 25);
            this.rbPrimarioMonofasico.Name = "rbPrimarioMonofasico";
            this.rbPrimarioMonofasico.Size = new System.Drawing.Size(85, 19);
            this.rbPrimarioMonofasico.TabIndex = 0;
            this.rbPrimarioMonofasico.TabStop = true;
            this.rbPrimarioMonofasico.Text = "Monofásico";

            // rbPrimarioBifasico
            this.rbPrimarioBifasico.AutoSize = true;
            this.rbPrimarioBifasico.Location = new System.Drawing.Point(145, 25);
            this.rbPrimarioBifasico.Name = "rbPrimarioBifasico";
            this.rbPrimarioBifasico.Size = new System.Drawing.Size(72, 19);
            this.rbPrimarioBifasico.TabIndex = 1;
            this.rbPrimarioBifasico.Text = "Bifásico";

            // rbPrimarioTrifasicoEstrella
            this.rbPrimarioTrifasicoEstrella.AutoSize = true;
            this.rbPrimarioTrifasicoEstrella.Location = new System.Drawing.Point(15, 50);
            this.rbPrimarioTrifasicoEstrella.Name = "rbPrimarioTrifasicoEstrella";
            this.rbPrimarioTrifasicoEstrella.Size = new System.Drawing.Size(105, 19);
            this.rbPrimarioTrifasicoEstrella.TabIndex = 2;
            this.rbPrimarioTrifasicoEstrella.Text = "Trifásico (Y)";

            // rbPrimarioTrifasicoDelta
            this.rbPrimarioTrifasicoDelta.AutoSize = true;
            this.rbPrimarioTrifasicoDelta.Location = new System.Drawing.Point(145, 50);
            this.rbPrimarioTrifasicoDelta.Name = "rbPrimarioTrifasicoDelta";
            this.rbPrimarioTrifasicoDelta.Size = new System.Drawing.Size(105, 19);
            this.rbPrimarioTrifasicoDelta.TabIndex = 3;
            this.rbPrimarioTrifasicoDelta.Text = "Trifásico (Δ)";

            // lblInfoPrimario
            this.lblInfoPrimario.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblInfoPrimario.Location = new System.Drawing.Point(15, 75);
            this.lblInfoPrimario.Name = "lblInfoPrimario";
            this.lblInfoPrimario.Size = new System.Drawing.Size(370, 23);
            this.lblInfoPrimario.TabIndex = 4;
            this.lblInfoPrimario.Text = "1φ - Voltaje Línea-Neutro";

            // grpSecundario
            this.grpSecundario.Controls.Add(this.lblInfoSecundario);
            this.grpSecundario.Controls.Add(this.rbSecundarioTrifasicoDelta);
            this.grpSecundario.Controls.Add(this.rbSecundarioTrifasicoEstrella);
            this.grpSecundario.Controls.Add(this.rbSecundarioBifasico);
            this.grpSecundario.Controls.Add(this.rbSecundarioMonofasico);
            this.grpSecundario.Location = new System.Drawing.Point(12, 123);
            this.grpSecundario.Name = "grpSecundario";
            this.grpSecundario.Size = new System.Drawing.Size(400, 105);
            this.grpSecundario.TabIndex = 1;
            this.grpSecundario.TabStop = false;
            this.grpSecundario.Text = "Configuración Secundario";

            // rbSecundarioMonofasico
            this.rbSecundarioMonofasico.AutoSize = true;
            this.rbSecundarioMonofasico.Checked = true;
            this.rbSecundarioMonofasico.Location = new System.Drawing.Point(15, 25);
            this.rbSecundarioMonofasico.Name = "rbSecundarioMonofasico";
            this.rbSecundarioMonofasico.Size = new System.Drawing.Size(85, 19);
            this.rbSecundarioMonofasico.TabIndex = 0;
            this.rbSecundarioMonofasico.TabStop = true;
            this.rbSecundarioMonofasico.Text = "Monofásico";

            // rbSecundarioBifasico
            this.rbSecundarioBifasico.AutoSize = true;
            this.rbSecundarioBifasico.Location = new System.Drawing.Point(145, 25);
            this.rbSecundarioBifasico.Name = "rbSecundarioBifasico";
            this.rbSecundarioBifasico.Size = new System.Drawing.Size(72, 19);
            this.rbSecundarioBifasico.TabIndex = 1;
            this.rbSecundarioBifasico.Text = "Bifásico";

            // rbSecundarioTrifasicoEstrella
            this.rbSecundarioTrifasicoEstrella.AutoSize = true;
            this.rbSecundarioTrifasicoEstrella.Location = new System.Drawing.Point(15, 50);
            this.rbSecundarioTrifasicoEstrella.Name = "rbSecundarioTrifasicoEstrella";
            this.rbSecundarioTrifasicoEstrella.Size = new System.Drawing.Size(105, 19);
            this.rbSecundarioTrifasicoEstrella.TabIndex = 2;
            this.rbSecundarioTrifasicoEstrella.Text = "Trifásico (Y)";

            // rbSecundarioTrifasicoDelta
            this.rbSecundarioTrifasicoDelta.AutoSize = true;
            this.rbSecundarioTrifasicoDelta.Location = new System.Drawing.Point(145, 50);
            this.rbSecundarioTrifasicoDelta.Name = "rbSecundarioTrifasicoDelta";
            this.rbSecundarioTrifasicoDelta.Size = new System.Drawing.Size(105, 19);
            this.rbSecundarioTrifasicoDelta.TabIndex = 3;
            this.rbSecundarioTrifasicoDelta.Text = "Trifásico (Δ)";

            // lblInfoSecundario
            this.lblInfoSecundario.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblInfoSecundario.Location = new System.Drawing.Point(15, 75);
            this.lblInfoSecundario.Name = "lblInfoSecundario";
            this.lblInfoSecundario.Size = new System.Drawing.Size(370, 23);
            this.lblInfoSecundario.TabIndex = 4;
            this.lblInfoSecundario.Text = "1φ - Voltaje Línea-Neutro";

            // grpParametros
            this.grpParametros.Controls.Add(this.txtEficiencia);
            this.grpParametros.Controls.Add(this.lblEficiencia);
            this.grpParametros.Controls.Add(this.txtFrecuencia);
            this.grpParametros.Controls.Add(this.lblFrecuencia);
            this.grpParametros.Controls.Add(this.txtCorrienteSecundaria);
            this.grpParametros.Controls.Add(this.lblCorrienteSecundaria);
            this.grpParametros.Controls.Add(this.txtVoltajeSecundario);
            this.grpParametros.Controls.Add(this.lblVoltajeSecundario);
            this.grpParametros.Controls.Add(this.txtVoltajePrimario);
            this.grpParametros.Controls.Add(this.lblVoltajePrimario);
            this.grpParametros.Location = new System.Drawing.Point(12, 234);
            this.grpParametros.Name = "grpParametros";
            this.grpParametros.Size = new System.Drawing.Size(400, 200);
            this.grpParametros.TabIndex = 2;
            this.grpParametros.TabStop = false;
            this.grpParametros.Text = "Parámetros Eléctricos";

            // lblVoltajePrimario
            this.lblVoltajePrimario.Location = new System.Drawing.Point(15, 30);
            this.lblVoltajePrimario.Name = "lblVoltajePrimario";
            this.lblVoltajePrimario.Size = new System.Drawing.Size(180, 20);
            this.lblVoltajePrimario.Text = "Voltaje Primario (V):";

            // txtVoltajePrimario
            this.txtVoltajePrimario.Location = new System.Drawing.Point(230, 27);
            this.txtVoltajePrimario.Name = "txtVoltajePrimario";
            this.txtVoltajePrimario.Size = new System.Drawing.Size(150, 23);
            this.txtVoltajePrimario.TabIndex = 0;

            // lblVoltajeSecundario
            this.lblVoltajeSecundario.Location = new System.Drawing.Point(15, 65);
            this.lblVoltajeSecundario.Name = "lblVoltajeSecundario";
            this.lblVoltajeSecundario.Size = new System.Drawing.Size(180, 20);
            this.lblVoltajeSecundario.Text = "Voltaje Secundario (V):";

            // txtVoltajeSecundario
            this.txtVoltajeSecundario.Location = new System.Drawing.Point(230, 62);
            this.txtVoltajeSecundario.Name = "txtVoltajeSecundario";
            this.txtVoltajeSecundario.Size = new System.Drawing.Size(150, 23);
            this.txtVoltajeSecundario.TabIndex = 1;

            // lblCorrienteSecundaria
            this.lblCorrienteSecundaria.Location = new System.Drawing.Point(15, 100);
            this.lblCorrienteSecundaria.Name = "lblCorrienteSecundaria";
            this.lblCorrienteSecundaria.Size = new System.Drawing.Size(200, 20);
            this.lblCorrienteSecundaria.Text = "Corriente Secundaria (A):";

            // txtCorrienteSecundaria
            this.txtCorrienteSecundaria.Location = new System.Drawing.Point(230, 97);
            this.txtCorrienteSecundaria.Name = "txtCorrienteSecundaria";
            this.txtCorrienteSecundaria.Size = new System.Drawing.Size(150, 23);
            this.txtCorrienteSecundaria.TabIndex = 2;

            // lblFrecuencia
            this.lblFrecuencia.Location = new System.Drawing.Point(15, 135);
            this.lblFrecuencia.Name = "lblFrecuencia";
            this.lblFrecuencia.Size = new System.Drawing.Size(180, 20);
            this.lblFrecuencia.Text = "Frecuencia (Hz):";

            // txtFrecuencia
            this.txtFrecuencia.Location = new System.Drawing.Point(230, 132);
            this.txtFrecuencia.Name = "txtFrecuencia";
            this.txtFrecuencia.Size = new System.Drawing.Size(150, 23);
            this.txtFrecuencia.TabIndex = 3;
            this.txtFrecuencia.Text = "60";

            // lblEficiencia
            this.lblEficiencia.Location = new System.Drawing.Point(15, 170);
            this.lblEficiencia.Name = "lblEficiencia";
            this.lblEficiencia.Size = new System.Drawing.Size(180, 20);
            this.lblEficiencia.Text = "Eficiencia (%):";

            // txtEficiencia
            this.txtEficiencia.Location = new System.Drawing.Point(230, 167);
            this.txtEficiencia.Name = "txtEficiencia";
            this.txtEficiencia.Size = new System.Drawing.Size(150, 23);
            this.txtEficiencia.TabIndex = 4;
            this.txtEficiencia.Text = "95";

            // grpAvanzado
            this.grpAvanzado.Controls.Add(this.txtElevacionTemp);
            this.grpAvanzado.Controls.Add(this.lblElevacionTemp);
            this.grpAvanzado.Controls.Add(this.txtTempAmbiente);
            this.grpAvanzado.Controls.Add(this.lblTempAmbiente);
            this.grpAvanzado.Controls.Add(this.cmbLaminado);
            this.grpAvanzado.Controls.Add(this.lblLaminado);
            this.grpAvanzado.Controls.Add(this.txtFactorLlenado);
            this.grpAvanzado.Controls.Add(this.lblFactorLlenado);
            this.grpAvanzado.Controls.Add(this.txtDensidadCorriente);
            this.grpAvanzado.Controls.Add(this.lblDensidadCorriente);
            this.grpAvanzado.Location = new System.Drawing.Point(12, 440);
            this.grpAvanzado.Name = "grpAvanzado";
            this.grpAvanzado.Size = new System.Drawing.Size(400, 215);
            this.grpAvanzado.TabIndex = 3;
            this.grpAvanzado.TabStop = false;
            this.grpAvanzado.Text = "Parámetros Avanzados";

            // lblDensidadCorriente
            this.lblDensidadCorriente.Location = new System.Drawing.Point(15, 30);
            this.lblDensidadCorriente.Name = "lblDensidadCorriente";
            this.lblDensidadCorriente.Size = new System.Drawing.Size(200, 20);
            this.lblDensidadCorriente.Text = "Densidad de Corriente (A/mm²):";

            // txtDensidadCorriente
            this.txtDensidadCorriente.Location = new System.Drawing.Point(230, 27);
            this.txtDensidadCorriente.Name = "txtDensidadCorriente";
            this.txtDensidadCorriente.Size = new System.Drawing.Size(150, 23);
            this.txtDensidadCorriente.TabIndex = 0;
            this.txtDensidadCorriente.Text = "3.0";

            // lblFactorLlenado
            this.lblFactorLlenado.Location = new System.Drawing.Point(15, 65);
            this.lblFactorLlenado.Name = "lblFactorLlenado";
            this.lblFactorLlenado.Size = new System.Drawing.Size(200, 20);
            this.lblFactorLlenado.Text = "Factor de Llenado:";

            // txtFactorLlenado
            this.txtFactorLlenado.Location = new System.Drawing.Point(230, 62);
            this.txtFactorLlenado.Name = "txtFactorLlenado";
            this.txtFactorLlenado.Size = new System.Drawing.Size(150, 23);
            this.txtFactorLlenado.TabIndex = 1;
            this.txtFactorLlenado.Text = "0.4";

            // lblLaminado
            this.lblLaminado.Location = new System.Drawing.Point(15, 100);
            this.lblLaminado.Name = "lblLaminado";
            this.lblLaminado.Size = new System.Drawing.Size(200, 20);
            this.lblLaminado.Text = "Tipo de Laminado:";

            // cmbLaminado
            this.cmbLaminado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLaminado.Location = new System.Drawing.Point(230, 97);
            this.cmbLaminado.Name = "cmbLaminado";
            this.cmbLaminado.Size = new System.Drawing.Size(150, 23);
            this.cmbLaminado.TabIndex = 2;

            // lblTempAmbiente
            this.lblTempAmbiente.Location = new System.Drawing.Point(15, 135);
            this.lblTempAmbiente.Name = "lblTempAmbiente";
            this.lblTempAmbiente.Size = new System.Drawing.Size(200, 20);
            this.lblTempAmbiente.Text = "Temp. Ambiente (°C):";

            // txtTempAmbiente
            this.txtTempAmbiente.Location = new System.Drawing.Point(230, 132);
            this.txtTempAmbiente.Name = "txtTempAmbiente";
            this.txtTempAmbiente.Size = new System.Drawing.Size(150, 23);
            this.txtTempAmbiente.TabIndex = 3;
            this.txtTempAmbiente.Text = "25";

            // lblElevacionTemp
            this.lblElevacionTemp.Location = new System.Drawing.Point(15, 170);
            this.lblElevacionTemp.Name = "lblElevacionTemp";
            this.lblElevacionTemp.Size = new System.Drawing.Size(200, 20);
            this.lblElevacionTemp.Text = "Elevación Temp. (°C):";

            // txtElevacionTemp
            this.txtElevacionTemp.Location = new System.Drawing.Point(230, 167);
            this.txtElevacionTemp.Name = "txtElevacionTemp";
            this.txtElevacionTemp.Size = new System.Drawing.Size(150, 23);
            this.txtElevacionTemp.TabIndex = 4;
            this.txtElevacionTemp.Text = "50";

            // btnCalcular
            this.btnCalcular.Location = new System.Drawing.Point(12, 665);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(125, 35);
            this.btnCalcular.TabIndex = 4;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.UseVisualStyleBackColor = true;

            // btnLimpiar
            this.btnLimpiar.Location = new System.Drawing.Point(152, 665);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(125, 35);
            this.btnLimpiar.TabIndex = 5;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;

            // btnExportarDiagrama
            this.btnExportarDiagrama.Location = new System.Drawing.Point(287, 665);
            this.btnExportarDiagrama.Name = "btnExportarDiagrama";
            this.btnExportarDiagrama.Size = new System.Drawing.Size(125, 35);
            this.btnExportarDiagrama.TabIndex = 6;
            this.btnExportarDiagrama.Text = "Exportar Diagrama";
            this.btnExportarDiagrama.UseVisualStyleBackColor = true;

            // txtResultados
            this.txtResultados.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtResultados.Location = new System.Drawing.Point(425, 12);
            this.txtResultados.Multiline = true;
            this.txtResultados.Name = "txtResultados";
            this.txtResultados.ReadOnly = true;
            this.txtResultados.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResultados.Size = new System.Drawing.Size(620, 420);
            this.txtResultados.TabIndex = 7;

            // picDiagramas
            this.picDiagramas.BackColor = System.Drawing.Color.White;
            this.picDiagramas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDiagramas.Location = new System.Drawing.Point(425, 438);
            this.picDiagramas.Name = "picDiagramas";
            this.picDiagramas.Size = new System.Drawing.Size(620, 262);
            this.picDiagramas.TabIndex = 8;
            this.picDiagramas.TabStop = false;

            // TransformerForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 712);
            this.Controls.Add(this.picDiagramas);
            this.Controls.Add(this.txtResultados);
            this.Controls.Add(this.btnExportarDiagrama);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.grpAvanzado);
            this.Controls.Add(this.grpParametros);
            this.Controls.Add(this.grpSecundario);
            this.Controls.Add(this.grpPrimario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TransformerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculador Profesional de Transformadores - Precisión Industrial";
            this.grpPrimario.ResumeLayout(false);
            this.grpPrimario.PerformLayout();
            this.grpSecundario.ResumeLayout(false);
            this.grpSecundario.PerformLayout();
            this.grpParametros.ResumeLayout(false);
            this.grpParametros.PerformLayout();
            this.grpAvanzado.ResumeLayout(false);
            this.grpAvanzado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDiagramas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}