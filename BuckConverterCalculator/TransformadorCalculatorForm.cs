using BuckConverterCalculator;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class TransformadorCalculatorForm : Form
    {
        private TransformadorCalculos calculadora;

        public TransformadorCalculatorForm()
        {
            InitializeComponent();
            InicializarFormulario();
        }

        /// <summary>
        /// Inicializa el formulario con valores por defecto
        /// </summary>
        private void InicializarFormulario()
        {
            calculadora = new TransformadorCalculos();

            // Configurar eventos del checkbox
            chkSec2Enabled.CheckedChanged += ChkSec2Enabled_CheckedChanged;

            // Estado inicial
            chkSec2Enabled.Checked = true;

            // Configurar DataGridViews
            ConfigurarDataGridViews();
        }

        /// <summary>
        /// Configura el formato de las grillas
        /// </summary>
        private void ConfigurarDataGridViews()
        {
            // Resultados
            dgvResultados.DefaultCellStyle.Font = new Font("Consolas", 9F);
            dgvResultados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvResultados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvResultados.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResultados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvResultados.EnableHeadersVisualStyles = false;

            // Térmico
            dgvTermico.DefaultCellStyle.Font = new Font("Consolas", 9F);
            dgvTermico.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvTermico.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvTermico.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTermico.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvTermico.EnableHeadersVisualStyles = false;
        }

        /// <summary>
        /// Evento para habilitar/deshabilitar secundario 2
        /// </summary>
        private void ChkSec2Enabled_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkSec2Enabled.Checked;
            txtVoltageSec2.Enabled = enabled;
            txtCorrienteSec2.Enabled = enabled;
        }

        /// <summary>
        /// Evento click del botón Calcular
        /// </summary>
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar y obtener datos de entrada
                if (!ValidarEntradas())
                    return;

                ActualizarStatusBar("Calculando...", true);

                // Obtener valores del formulario
                calculadora.VoltagePrimario = double.Parse(txtVoltagePrimario.Text);
                calculadora.NumDevanadosPrimario = int.Parse(txtNumDevanadosPrim.Text);
                calculadora.VoltageSec1 = double.Parse(txtVoltageSec1.Text);
                calculadora.CorrienteSec1 = double.Parse(txtCorrienteSec1.Text);
                calculadora.VoltageSec2 = double.Parse(txtVoltageSec2.Text);
                calculadora.CorrienteSec2 = double.Parse(txtCorrienteSec2.Text);
                calculadora.Sec2Habilitado = chkSec2Enabled.Checked;
                calculadora.AreaNucleo = double.Parse(txtAreaNucleo.Text);
                calculadora.DensidadFlujo = double.Parse(txtDensidadFlujo.Text);
                calculadora.Frecuencia = double.Parse(txtFrecuencia.Text);

                // Realizar cálculos
                calculadora.Calcular();

                // Mostrar resultados
                MostrarResultados();
                MostrarAnalisisTermico();
                MostrarReporte();

                // Cambiar a la pestaña de resultados
                tabControl1.SelectedTab = tabResultados;

                ActualizarStatusBar("Cálculo completado exitosamente", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarStatusBar("Error en el cálculo", false);
            }
        }

        /// <summary>
        /// Valida las entradas del usuario
        /// </summary>
        private bool ValidarEntradas()
        {
            try
            {
                // Validar primario
                if (!ValidarNumero(txtVoltagePrimario.Text, "Voltaje Primario", 1, 10000))
                    return false;
                if (!ValidarEntero(txtNumDevanadosPrim.Text, "Número de Devanados", 1, 10))
                    return false;

                // Validar secundario 1
                if (!ValidarNumero(txtVoltageSec1.Text, "Voltaje Secundario 1", 1, 10000))
                    return false;
                if (!ValidarNumero(txtCorrienteSec1.Text, "Corriente Secundario 1", 0.1, 1000))
                    return false;

                // Validar secundario 2 si está habilitado
                if (chkSec2Enabled.Checked)
                {
                    if (!ValidarNumero(txtVoltageSec2.Text, "Voltaje Secundario 2", 1, 10000))
                        return false;
                    if (!ValidarNumero(txtCorrienteSec2.Text, "Corriente Secundario 2", 0.1, 1000))
                        return false;
                }

                // Validar núcleo
                if (!ValidarNumero(txtAreaNucleo.Text, "Área del Núcleo", 0.1, 10000))
                    return false;
                if (!ValidarNumero(txtDensidadFlujo.Text, "Densidad de Flujo", 0.1, 2.0))
                    return false;
                if (!ValidarNumero(txtFrecuencia.Text, "Frecuencia", 50, 400))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida un número decimal
        /// </summary>
        private bool ValidarNumero(string texto, string campo, double min, double max)
        {
            if (!double.TryParse(texto, out double valor))
            {
                MessageBox.Show($"El campo '{campo}' debe ser un número válido.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (valor < min || valor > max)
            {
                MessageBox.Show($"El campo '{campo}' debe estar entre {min} y {max}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida un número entero
        /// </summary>
        private bool ValidarEntero(string texto, string campo, int min, int max)
        {
            if (!int.TryParse(texto, out int valor))
            {
                MessageBox.Show($"El campo '{campo}' debe ser un número entero válido.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (valor < min || valor > max)
            {
                MessageBox.Show($"El campo '{campo}' debe estar entre {min} y {max}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Muestra los resultados en la grilla
        /// </summary>
        private void MostrarResultados()
        {
            dgvResultados.Rows.Clear();

            // Sección: Potencias
            AgregarEncabezado(dgvResultados, "POTENCIAS");
            AgregarFila(dgvResultados, "Salida 1", calculadora.PotenciaSec1, "VA", "");
            AgregarFila(dgvResultados, "Salida 2", calculadora.PotenciaSec2, "VA", "");
            AgregarFila(dgvResultados, "Total Salida", calculadora.PotenciaTotalSalida, "VA", "");
            AgregarFila(dgvResultados, "Entrada Requerida", calculadora.PotenciaEntrada, "VA", "");
            AgregarFila(dgvResultados, "Pérdidas Estimadas", calculadora.PerdidasEstimadas, "W", "");

            // Sección: Devanados
            AgregarEncabezado(dgvResultados, "DEVANADOS");
            AgregarFila(dgvResultados, "Espiras/Volt", calculadora.EspirasPorVolt, "N/V", "");
            AgregarFila(dgvResultados, "Espiras Primario", calculadora.EspirasPrimario, "", "");
            AgregarFila(dgvResultados, "Espiras Secundarios", calculadora.EspirasSecundario, "", "+3% compensación");
            AgregarFila(dgvResultados, "Corriente Prim/dev", calculadora.CorrientePorDevanadoPrim, "A", "");
            AgregarFila(dgvResultados, "Densidad J Primario", calculadora.DensidadCorrientePrim, "A/mm²",
                ObtenerEstadoDensidad(calculadora.DensidadCorrientePrim));
            AgregarFila(dgvResultados, "Densidad J Secundario", calculadora.DensidadCorrienteSec, "A/mm²",
                ObtenerEstadoDensidad(calculadora.DensidadCorrienteSec));

            // Sección: Ventana
            AgregarEncabezado(dgvResultados, "VERIFICACIÓN DE VENTANA");
            AgregarFila(dgvResultados, "Área Cobre Primario", calculadora.AreaCobrePrimario, "mm²", "");
            AgregarFila(dgvResultados, "Área Cobre Sec1", calculadora.AreaCobreSec1, "mm²", "");
            if (calculadora.Sec2Habilitado)
                AgregarFila(dgvResultados, "Área Cobre Sec2", calculadora.AreaCobreSec2, "mm²", "");
            AgregarFila(dgvResultados, "Área Cobre Total", calculadora.AreaCobreTotal, "mm²", "");
            AgregarFila(dgvResultados, "Factor de Llenado", calculadora.FactorLlenado, "%",
                ObtenerEstadoFactorLlenado(calculadora.FactorLlenado));

            // Sección: Pérdidas
            AgregarEncabezado(dgvResultados, "ANÁLISIS DE PÉRDIDAS");
            AgregarFila(dgvResultados, "Pérdidas Cu Primario", calculadora.PerdidasCuPrimario, "W", "");
            AgregarFila(dgvResultados, "Pérdidas Cu Sec1", calculadora.PerdidasCuSec1, "W", "");
            if (calculadora.Sec2Habilitado)
                AgregarFila(dgvResultados, "Pérdidas Cu Sec2", calculadora.PerdidasCuSec2, "W", "");
            AgregarFila(dgvResultados, "Total Pérdidas Cobre", calculadora.PerdidasCuTotal, "W", "");
            AgregarFila(dgvResultados, "Pérdidas Núcleo", calculadora.PerdidasNucleo, "W", "");
            AgregarFila(dgvResultados, "Pérdidas Totales", calculadora.PerdidasTotales, "W", "");
            AgregarFila(dgvResultados, "Eficiencia Real", calculadora.EficienciaReal, "%", "");
        }

        /// <summary>
        /// Muestra el análisis térmico
        /// </summary>
        private void MostrarAnalisisTermico()
        {
            dgvTermico.Rows.Clear();

            var escenarios = calculadora.ObtenerEscenariosTermicos();
            foreach (var esc in escenarios)
            {
                int rowIndex = dgvTermico.Rows.Add(
                    esc.Nombre,
                    esc.Coeficiente.ToString("F0"),
                    esc.DeltaT.ToString("F1"),
                    esc.TempOperacion.ToString("F1"),
                    esc.TempHotspot.ToString("F1"),
                    esc.Estado
                );

                // Colorear según estado
                var row = dgvTermico.Rows[rowIndex];
                switch (esc.Estado)
                {
                    case "Excelente":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(144, 238, 144);
                        break;
                    case "Bueno":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(173, 216, 230);
                        break;
                    case "Aceptable":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 153);
                        break;
                    case "Límite":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 124);
                        break;
                    case "Crítico":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 153, 153);
                        break;
                }

                // Resaltar escenario recomendado
                if (esc.Nombre.Contains("★"))
                {
                    row.DefaultCellStyle.Font = new Font(dgvTermico.Font, FontStyle.Bold);
                }
            }

            // Recomendaciones
            txtRecomendacionesTermicas.Text = GenerarRecomendacionesTermicas();
        }

        /// <summary>
        /// Genera el texto de recomendaciones térmicas
        /// </summary>
        private string GenerarRecomendacionesTermicas()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("RECOMENDACIONES TÉRMICAS:");
            sb.AppendLine();
            sb.AppendLine($"Superficie radiante estimada: {calculadora.SuperficieRadiante:F0} cm²");
            sb.AppendLine($"Pérdidas totales: {calculadora.PerdidasTotales:F2} W");
            sb.AppendLine();
            sb.AppendLine("VENTILACIÓN:");
            sb.AppendLine($"• Natural: T_op = {calculadora.TempOperacionNatural:F1}°C, T_hotspot = {calculadora.TempHotspotNatural:F1}°C");

            if (calculadora.TempHotspotNatural > 130)
                sb.AppendLine("  ⚠ NO RECOMENDADA - Temperatura excesiva");
            else if (calculadora.TempHotspotNatural > 105)
                sb.AppendLine("  ⚠ Aceptable pero requiere aislamiento clase B o superior");
            else
                sb.AppendLine("  ✓ Aceptable");

            sb.AppendLine();
            sb.AppendLine($"• Forzada: T_op = {calculadora.TempOperacionForzada:F1}°C, T_hotspot = {calculadora.TempHotspotForzada:F1}°C");
            sb.AppendLine("  ★ RECOMENDADA - Ventilador 120mm @ 30-50 CFM");
            sb.AppendLine();
            sb.AppendLine("CLASE DE AISLAMIENTO:");

            string claseAislamiento = ObtenerClaseAislamiento(calculadora.TempHotspotForzada);
            sb.AppendLine($"• Mínima requerida: {claseAislamiento}");
            sb.AppendLine("• Recomendada: Clase F (155°C) por seguridad");
            sb.AppendLine();
            sb.AppendLine("MATERIALES:");
            sb.AppendLine("• Barniz poliéster o resina epoxy");
            sb.AppendLine("• Fibra de vidrio");
            sb.AppendLine("• Papel kraft clase F entre capas");
            sb.AppendLine();
            sb.AppendLine("CONSTRUCCIÓN:");
            sb.AppendLine("• Dejar espacio mínimo 5 cm alrededor para circulación");
            sb.AppendLine("• Orientación núcleo vertical preferible");
            sb.AppendLine("• Termostato opcional @ 95-100°C como seguridad");

            return sb.ToString();
        }

        /// <summary>
        /// Muestra el reporte completo
        /// </summary>
        private void MostrarReporte()
        {
            txtReporte.Text = calculadora.GenerarReporteCompleto();
        }

        /// <summary>
        /// Evento click del botón Limpiar
        /// </summary>
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Está seguro de limpiar todos los datos?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LimpiarFormulario();
            }
        }

        /// <summary>
        /// Evento click del botón Calculadora de Núcleo
        /// </summary>
        private void btnCalculadoraNucleo_Click(object sender, EventArgs e)
        {
            using (var form = new CalculadoraNucleoForm())
            {
                if (form.ShowDialog() == DialogResult.OK && form.AplicoValor)
                {
                    txtAreaNucleo.Text = form.AreaSeleccionada.ToString("F2");
                    MessageBox.Show($"Área del núcleo actualizada a {form.AreaSeleccionada:F2} cm²",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Limpia todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            // Restaurar valores por defecto
            txtVoltagePrimario.Text = "220";
            txtNumDevanadosPrim.Text = "2";
            txtVoltageSec1.Text = "110";
            txtCorrienteSec1.Text = "30";
            txtVoltageSec2.Text = "110";
            txtCorrienteSec2.Text = "30";
            chkSec2Enabled.Checked = true;
            txtAreaNucleo.Text = "51.99";
            txtDensidadFlujo.Text = "1.5";
            txtFrecuencia.Text = "60";

            // Limpiar resultados
            dgvResultados.Rows.Clear();
            dgvTermico.Rows.Clear();
            txtRecomendacionesTermicas.Text = "";
            txtReporte.Text = "";

            // Volver a la primera pestaña
            tabControl1.SelectedTab = tabEntrada;

            ActualizarStatusBar("Formulario limpiado", false);
        }

        /// <summary>
        /// Exportar a archivo de texto
        /// </summary>
        private void btnExportarTxt_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Archivos de texto (*.txt)|*.txt";
                    sfd.FileName = $"Transformador_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, txtReporte.Text);
                        MessageBox.Show("Archivo exportado exitosamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento click del botón Vista Previa / PDF
        /// </summary>
        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            try
            {
                if (calculadora == null)
                {
                    MessageBox.Show("Primero debe realizar un cálculo.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var formVistaPrevia = new VistaPreviaForm(txtReporte.Text, calculadora))
                {
                    formVistaPrevia.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir vista previa: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exportar a Excel
        /// </summary>
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Para exportar a Excel, necesitarás la biblioteca EPPlus o similar.\n" +
                "Esta funcionalidad requiere referencias adicionales no incluidas por defecto.",
                "Información",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #region Métodos auxiliares

        private void AgregarEncabezado(DataGridView dgv, string texto)
        {
            int rowIndex = dgv.Rows.Add(texto, "", "", "");
            var row = dgv.Rows[rowIndex];
            row.DefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            row.DefaultCellStyle.ForeColor = Color.White;
            row.DefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
        }

        private void AgregarFila(DataGridView dgv, string parametro, object valor, string unidad, string obs)
        {
            string valorStr = valor is double d ? d.ToString("F2") : valor.ToString();
            dgv.Rows.Add(parametro, valorStr, unidad, obs);
        }

        private string ObtenerEstadoDensidad(double densidad)
        {
            if (densidad < 2.5)
                return "Excelente";
            else if (densidad < 3.5)
                return "Bueno";
            else if (densidad < 4.5)
                return "Aceptable";
            else
                return "Alto";
        }

        private string ObtenerEstadoFactorLlenado(double factor)
        {
            if (factor < 30)
                return "Bajo - Espacio disponible";
            else if (factor < 40)
                return "Óptimo";
            else if (factor < 50)
                return "Ajustado";
            else
                return "Excesivo - Revisar";
        }

        private string ObtenerClaseAislamiento(double tempHotspot)
        {
            if (tempHotspot < 105)
                return "Clase A (105°C)";
            else if (tempHotspot < 130)
                return "Clase B (130°C)";
            else if (tempHotspot < 155)
                return "Clase F (155°C)";
            else if (tempHotspot < 180)
                return "Clase H (180°C)";
            else
                return "Clase C (>180°C)";
        }

        private void ActualizarStatusBar(string mensaje, bool mostrarProgress)
        {
            toolStripStatusLabel1.Text = mensaje;
            toolStripProgressBar1.Visible = mostrarProgress;
            statusStrip1.Refresh();
        }

        #endregion
    }
}
