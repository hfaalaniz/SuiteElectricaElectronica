using BuckConverterCalculator;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class CalculadoraNucleoForm : Form
    {
        public double AreaSeleccionada { get; private set; }
        public bool AplicoValor { get; private set; }

        public CalculadoraNucleoForm()
        {
            InitializeComponent();
        }

        private void CalculadoraNucleoForm_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridViews();
            CargarTodosLosNucleos();
        }

        private void ConfigurarDataGridViews()
        {
            // Configurar grilla de núcleos sugeridos
            dgvNucleosSugeridos.DefaultCellStyle.Font = new Font("Consolas", 9F);
            dgvNucleosSugeridos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvNucleosSugeridos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvNucleosSugeridos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNucleosSugeridos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvNucleosSugeridos.EnableHeadersVisualStyles = false;

            // Configurar grilla de núcleos disponibles
            dgvNucleosDisponibles.DefaultCellStyle.Font = new Font("Consolas", 9F);
            dgvNucleosDisponibles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvNucleosDisponibles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgvNucleosDisponibles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNucleosDisponibles.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvNucleosDisponibles.EnableHeadersVisualStyles = false;
        }

        private void CargarTodosLosNucleos()
        {
            dgvNucleosDisponibles.Rows.Clear();
            var nucleos = CalculadoraNucleo.ObtenerTodosLosNucleos();

            foreach (var nucleo in nucleos)
            {
                dgvNucleosDisponibles.Rows.Add(
                    nucleo.Modelo,
                    nucleo.AreaEfectiva.ToString("F1"),
                    nucleo.AreaVentana.ToString("F1"),
                    nucleo.LongitudMagnetica.ToString("F1"),
                    $"{nucleo.PotenciaMin:F0} - {nucleo.PotenciaMax:F0}",
                    nucleo.Dimensiones
                );
            }
        }

        private void btnCalcularPorPotencia_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar entrada
                if (!double.TryParse(txtPotencia.Text, out double potencia) || potencia <= 0)
                {
                    MessageBox.Show("Ingrese una potencia válida mayor a 0.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtFrecuencia.Text, out double frecuencia) || frecuencia <= 0)
                {
                    MessageBox.Show("Ingrese una frecuencia válida.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtDensidadFlujo.Text, out double densidadFlujo) || densidadFlujo <= 0)
                {
                    MessageBox.Show("Ingrese una densidad de flujo válida.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calcular áreas
                double areaMinima = CalculadoraNucleo.CalcularAreaMinima(potencia, frecuencia, densidadFlujo);
                double areaRecomendada = CalculadoraNucleo.CalcularAreaRecomendada(potencia, frecuencia, densidadFlujo);
                var nucleoOptimo = CalculadoraNucleo.ObtenerNucleoOptimo(potencia);

                // Mostrar resultados
                lblAreaMinima.Text = $"{areaMinima:F2} cm²";
                lblAreaRecomendada.Text = $"{areaRecomendada:F2} cm²";
                lblNucleoSugerido.Text = nucleoOptimo != null ? $"{nucleoOptimo.Modelo} ({nucleoOptimo.AreaEfectiva:F1} cm²)" : "No disponible";

                // Cargar núcleos sugeridos
                dgvNucleosSugeridos.Rows.Clear();
                var nucleosSugeridos = CalculadoraNucleo.SugerirNucleos(potencia);

                if (nucleosSugeridos.Count == 0)
                {
                    // Si no hay coincidencia exacta, mostrar los 3 más cercanos
                    var todosNucleos = CalculadoraNucleo.ObtenerTodosLosNucleos();
                    foreach (var nucleo in todosNucleos)
                    {
                        if (nucleo.AreaEfectiva >= areaMinima)
                        {
                            dgvNucleosSugeridos.Rows.Add(
                                nucleo.Modelo,
                                nucleo.AreaEfectiva.ToString("F1"),
                                nucleo.AreaVentana.ToString("F1"),
                                $"{nucleo.PotenciaMin:F0} - {nucleo.PotenciaMax:F0}",
                                nucleo.Dimensiones
                            );

                            if (dgvNucleosSugeridos.Rows.Count >= 3)
                                break;
                        }
                    }
                }
                else
                {
                    foreach (var nucleo in nucleosSugeridos)
                    {
                        int rowIndex = dgvNucleosSugeridos.Rows.Add(
                            nucleo.Modelo,
                            nucleo.AreaEfectiva.ToString("F1"),
                            nucleo.AreaVentana.ToString("F1"),
                            $"{nucleo.PotenciaMin:F0} - {nucleo.PotenciaMax:F0}",
                            nucleo.Dimensiones
                        );

                        // Resaltar el óptimo
                        if (nucleo.Modelo == nucleoOptimo?.Modelo)
                        {
                            dgvNucleosSugeridos.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(144, 238, 144);
                            dgvNucleosSugeridos.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgvNucleosSugeridos.Font, FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalcularMedidas_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar entrada
                if (!double.TryParse(txtAnchoColumna.Text, out double ancho) || ancho <= 0)
                {
                    MessageBox.Show("Ingrese un ancho válido mayor a 0.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtEspesorStack.Text, out double espesor) || espesor <= 0)
                {
                    MessageBox.Show("Ingrese un espesor válido mayor a 0.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtFactorApilamiento.Text, out double factor) || factor <= 0 || factor > 1)
                {
                    MessageBox.Show("Ingrese un factor de apilamiento válido entre 0 y 1.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calcular área
                double area = CalculadoraNucleo.CalcularAreaDesdeMedidas(ancho, espesor, factor);

                // Mostrar resultado
                lblResultadoMedidas.Text = $"{area:F2} cm²";

                // Actualizar área seleccionada
                AreaSeleccionada = area;
                lblAreaSeleccionada.Text = $"Área calculada: {area:F2} cm²";
                btnAplicar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvNucleosSugeridos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNucleosSugeridos.SelectedRows.Count > 0)
            {
                var row = dgvNucleosSugeridos.SelectedRows[0];
                if (double.TryParse(row.Cells[1].Value?.ToString(), out double area))
                {
                    AreaSeleccionada = area;
                    lblAreaSeleccionada.Text = $"Área seleccionada: {area:F2} cm² - Modelo: {row.Cells[0].Value}";
                    btnAplicar.Enabled = true;
                }
            }
        }

        private void dgvNucleosDisponibles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNucleosDisponibles.SelectedRows.Count > 0)
            {
                var row = dgvNucleosDisponibles.SelectedRows[0];
                if (double.TryParse(row.Cells[1].Value?.ToString(), out double area))
                {
                    AreaSeleccionada = area;
                    lblAreaSeleccionada.Text = $"Área seleccionada: {area:F2} cm² - Modelo: {row.Cells[0].Value}";
                    btnAplicar.Enabled = true;
                }
            }
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            AplicoValor = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            AplicoValor = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
