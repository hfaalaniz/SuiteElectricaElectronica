using BuckConverterCalculator;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class TransformerForm : Form
    {
        private TransformerCalculator calculator;
        private TransformerResult lastResult;

        public TransformerForm()
        {
            InitializeComponent();
            calculator = new TransformerCalculator();
            ConfigureEvents();
            CargarTiposLaminado();
        }

        private void ConfigureEvents()
        {
            btnCalcular.Click += BtnCalcular_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            btnExportarDiagrama.Click += BtnExportarDiagrama_Click;
            rbPrimarioMonofasico.CheckedChanged += (s, e) => UpdatePrimaryUI();
            rbPrimarioBifasico.CheckedChanged += (s, e) => UpdatePrimaryUI();
            rbPrimarioTrifasicoEstrella.CheckedChanged += (s, e) => UpdatePrimaryUI();
            rbPrimarioTrifasicoDelta.CheckedChanged += (s, e) => UpdatePrimaryUI();
            rbSecundarioMonofasico.CheckedChanged += (s, e) => UpdateSecondaryUI();
            rbSecundarioBifasico.CheckedChanged += (s, e) => UpdateSecondaryUI();
            rbSecundarioTrifasicoEstrella.CheckedChanged += (s, e) => UpdateSecondaryUI();
            rbSecundarioTrifasicoDelta.CheckedChanged += (s, e) => UpdateSecondaryUI();
        }

        private void CargarTiposLaminado()
        {
            cmbLaminado.Items.Add("M19 (29ga) - 0.35mm - Estándar");
            cmbLaminado.Items.Add("M15 (29ga) - 0.35mm - Bajo pérdidas");
            cmbLaminado.Items.Add("M6 (27ga) - 0.40mm - Muy bajo pérdidas");
            cmbLaminado.Items.Add("Amorfo - 0.025mm - Mínimas pérdidas");
            cmbLaminado.Items.Add("Grano Orientado - 0.30mm - Alta eficiencia");
            cmbLaminado.SelectedIndex = 0;
        }

        private void UpdatePrimaryUI()
        {
            if (rbPrimarioMonofasico.Checked)
                lblInfoPrimario.Text = "1φ - VL-N";
            else if (rbPrimarioBifasico.Checked)
                lblInfoPrimario.Text = "2φ - 90° - VL-L";
            else if (rbPrimarioTrifasicoEstrella.Checked)
                lblInfoPrimario.Text = "3φ-Y | Vf=VL/√3 | If=IL";
            else
                lblInfoPrimario.Text = "3φ-Δ | Vf=VL | If=IL/√3";
        }

        private void UpdateSecondaryUI()
        {
            if (rbSecundarioMonofasico.Checked)
                lblInfoSecundario.Text = "1φ - VL-N";
            else if (rbSecundarioBifasico.Checked)
                lblInfoSecundario.Text = "2φ - 90° - VL-L";
            else if (rbSecundarioTrifasicoEstrella.Checked)
                lblInfoSecundario.Text = "3φ-Y | Vf=VL/√3 | If=IL";
            else
                lblInfoSecundario.Text = "3φ-Δ | Vf=VL | If=IL/√3";
        }

        private TipoConexion GetTipoPrimario()
        {
            if (rbPrimarioMonofasico.Checked) return TipoConexion.Monofasico;
            if (rbPrimarioBifasico.Checked) return TipoConexion.Bifasico;
            if (rbPrimarioTrifasicoEstrella.Checked) return TipoConexion.TrifasicoEstrella;
            return TipoConexion.TrifasicoDelta;
        }

        private TipoConexion GetTipoSecundario()
        {
            if (rbSecundarioMonofasico.Checked) return TipoConexion.Monofasico;
            if (rbSecundarioBifasico.Checked) return TipoConexion.Bifasico;
            if (rbSecundarioTrifasicoEstrella.Checked) return TipoConexion.TrifasicoEstrella;
            return TipoConexion.TrifasicoDelta;
        }

        private TipoLaminado GetTipoLaminado()
        {
            return cmbLaminado.SelectedIndex switch
            {
                0 => TipoLaminado.M19_29Gauge,
                1 => TipoLaminado.M15_29Gauge,
                2 => TipoLaminado.M6_27Gauge,
                3 => TipoLaminado.Amorphous,
                4 => TipoLaminado.GrainOriented,
                _ => TipoLaminado.M19_29Gauge
            };
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                var config = new TransformerConfig
                {
                    TipoPrimario = GetTipoPrimario(),
                    TipoSecundario = GetTipoSecundario(),
                    VoltajePrimario = double.Parse(txtVoltajePrimario.Text),
                    VoltajeSecundario = double.Parse(txtVoltajeSecundario.Text),
                    CorrienteSecundaria = double.Parse(txtCorrienteSecundaria.Text),
                    Frecuencia = double.Parse(txtFrecuencia.Text),
                    Eficiencia = double.Parse(txtEficiencia.Text),
                    TipoLaminado = GetTipoLaminado(),
                    DensidadCorriente = double.Parse(txtDensidadCorriente.Text),
                    FactorLlenado = double.Parse(txtFactorLlenado.Text),
                    TempAmbiente = double.Parse(txtTempAmbiente.Text),
                    ElevacionTemp = double.Parse(txtElevacionTemp.Text)
                };

                lastResult = calculator.Calculate(config);
                MostrarResultados(lastResult);
                DibujarDiagramas(lastResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarResultados(TransformerResult r)
        {
            txtResultados.Clear();
            txtResultados.AppendText("╔════════════════════════════════════════════════════════════╗\r\n");
            txtResultados.AppendText("║     DISEÑO COMPLETO DE TRANSFORMADOR - ANÁLISIS TÉCNICO   ║\r\n");
            txtResultados.AppendText("╚════════════════════════════════════════════════════════════╝\r\n\r\n");

            txtResultados.AppendText($"Configuración: {r.TipoPrimario} → {r.TipoSecundario}\r\n");
            txtResultados.AppendText($"Laminado: {r.NombreLaminado}\r\n\r\n");

            txtResultados.AppendText("┌─ POTENCIAS Y EFICIENCIA ────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ Potencia Secundaria:              {r.PotenciaSecundaria,15:F2} VA\r\n");
            txtResultados.AppendText($"│ Potencia Primaria:                {r.PotenciaPrimaria,15:F2} VA\r\n");
            txtResultados.AppendText($"│ Pérdidas Núcleo:                  {r.PerdidasNucleo,15:F2} W\r\n");
            txtResultados.AppendText($"│ Pérdidas Cobre:                   {r.PerdidasCobre,15:F2} W\r\n");
            txtResultados.AppendText($"│ Pérdidas Totales:                 {r.PerdidasTotales,15:F2} W\r\n");
            txtResultados.AppendText($"│ Eficiencia Real:                  {r.EficienciaReal,15:F2} %\r\n");
            txtResultados.AppendText($"│ Regulación de Voltaje:            {r.RegulacionVoltaje,15:F2} %\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n\r\n");

            txtResultados.AppendText("┌─ PARÁMETROS ELÉCTRICOS ─────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ Voltaje Primario Fase:            {r.VoltajePrimarioFase,15:F2} V\r\n");
            txtResultados.AppendText($"│ Voltaje Secundario Fase:          {r.VoltajeSecundarioFase,15:F2} V\r\n");
            txtResultados.AppendText($"│ Corriente Primaria Línea:         {r.CorrientePrimaria,15:F4} A\r\n");
            txtResultados.AppendText($"│ Corriente Primaria Fase:          {r.CorrientePrimarioFase,15:F4} A\r\n");
            txtResultados.AppendText($"│ Corriente Secundaria Línea:       {r.CorrienteSecundaria,15:F4} A\r\n");
            txtResultados.AppendText($"│ Corriente Secundaria Fase:        {r.CorrienteSecundarioFase,15:F4} A\r\n");
            txtResultados.AppendText($"│ Relación de Transformación:       {r.RelacionTransformacion,15:F6}\r\n");
            txtResultados.AppendText($"│ Relación de Corrientes:           {r.RelacionCorrientes,15:F6}\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n\r\n");

            txtResultados.AppendText("┌─ NÚCLEO MAGNÉTICO ──────────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ Área Efectiva:                    {r.AreaNucleoEfectiva,15:F2} cm²\r\n");
            txtResultados.AppendText($"│ Área Bruta:                       {r.AreaNucleoBruta,15:F2} cm²\r\n");
            txtResultados.AppendText($"│ Inducción Magnética:              {r.InduccionReal,15:F2} T\r\n");
            txtResultados.AppendText($"│ Flujo Máximo:                     {r.FlujoMaximo,15:F6} Wb\r\n");
            txtResultados.AppendText($"│ Voltios/Espira:                   {r.VoltioPorEspira,15:F4} V\r\n");
            txtResultados.AppendText($"│ Ancho Núcleo:                     {r.AnchoNucleo,15:F2} cm\r\n");
            txtResultados.AppendText($"│ Alto Núcleo:                      {r.AltoNucleo,15:F2} cm\r\n");
            txtResultados.AppendText($"│ Peso Núcleo:                      {r.PesoNucleo,15:F2} g\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n\r\n");

            txtResultados.AppendText("┌─ DEVANADOS ─────────────────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ Espiras Primario (por fase):      {r.EspirasPrimario,15}\r\n");
            txtResultados.AppendText($"│ Espiras Secundario (por fase):    {r.EspirasSecundario,15}\r\n");
            txtResultados.AppendText($"│ Fases Primario:                   {r.FasesPrimario,15}\r\n");
            txtResultados.AppendText($"│ Fases Secundario:                 {r.FasesSecundario,15}\r\n");
            txtResultados.AppendText($"│ Longitud Media Espira:            {r.LongitudMediaEspira,15:F2} cm\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n\r\n");

            txtResultados.AppendText("┌─ CONDUCTORES ───────────────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ PRIMARIO:\r\n");
            txtResultados.AppendText($"│   Calibre AWG:                    {r.CalibrePrimario,15}\r\n");
            txtResultados.AppendText($"│   Área:                           {r.AreaConductorPrimario,15:F4} mm²\r\n");
            txtResultados.AppendText($"│   Diámetro:                       {r.DiametroConductorPrimario,15:F3} mm\r\n");
            txtResultados.AppendText($"│   Longitud Total:                 {r.LongitudCobrePrimario,15:F2} m\r\n");
            txtResultados.AppendText($"│   Peso:                           {r.PesoCobrePrimario,15:F2} g\r\n");
            txtResultados.AppendText($"│   Resistencia:                    {r.ResistenciaPrimario,15:F4} Ω\r\n");
            txtResultados.AppendText($"│\r\n");
            txtResultados.AppendText($"│ SECUNDARIO:\r\n");
            txtResultados.AppendText($"│   Calibre AWG:                    {r.CalibreSecundario,15}\r\n");
            txtResultados.AppendText($"│   Área:                           {r.AreaConductorSecundario,15:F4} mm²\r\n");
            txtResultados.AppendText($"│   Diámetro:                       {r.DiametroConductorSecundario,15:F3} mm\r\n");
            txtResultados.AppendText($"│   Longitud Total:                 {r.LongitudCobreSecundario,15:F2} m\r\n");
            txtResultados.AppendText($"│   Peso:                           {r.PesoCobreSecundario,15:F2} g\r\n");
            txtResultados.AppendText($"│   Resistencia:                    {r.ResistenciaSecundario,15:F4} Ω\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n\r\n");

            txtResultados.AppendText("┌─ TÉRMICA Y DIMENSIONES ─────────────────────────────────┐\r\n");
            txtResultados.AppendText($"│ Temperatura Devanados:            {r.TempDevanados,15:F1} °C\r\n");
            txtResultados.AppendText($"│ Ventana Requerida:                {r.AreaVentanaRequerida,15:F2} cm²\r\n");
            txtResultados.AppendText($"│ Ancho Ventana:                    {r.AnchoVentana,15:F2} cm\r\n");
            txtResultados.AppendText($"│ Alto Ventana:                     {r.AltoVentana,15:F2} cm\r\n");
            txtResultados.AppendText($"│ Producto Ap:                      {r.ProductoAp,15:F2} cm⁴\r\n");
            txtResultados.AppendText($"│ Peso Total:                       {r.PesoTotal,15:F2} g\r\n");
            txtResultados.AppendText("└─────────────────────────────────────────────────────────┘\r\n");
        }

        private void DibujarDiagramas(TransformerResult r)
        {
            var bmp = new Bitmap(picDiagramas.Width, picDiagramas.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Dibujar núcleo
                int xBase = 30, yBase = 30;
                float escala = Math.Min(200f / (float)r.AltoNucleo, 150f / (float)r.AnchoNucleo);

                int anchoPixels = (int)(r.AnchoNucleo * escala);
                int altoPixels = (int)(r.AltoNucleo * escala);

                using (var penNucleo = new Pen(Color.DarkSlateGray, 3))
                using (var brushTexto = new SolidBrush(Color.Black))
                using (var font = new Font("Arial", 8))
                {
                    // Núcleo EI simplificado
                    g.DrawRectangle(penNucleo, xBase, yBase, anchoPixels / 3, altoPixels);
                    g.DrawRectangle(penNucleo, xBase + anchoPixels * 2 / 3, yBase, anchoPixels / 3, altoPixels);
                    g.DrawLine(penNucleo, xBase, yBase, xBase + anchoPixels, yBase);
                    g.DrawLine(penNucleo, xBase, yBase + altoPixels, xBase + anchoPixels, yBase + altoPixels);

                    // Devanados
                    int ventanaX = xBase + anchoPixels / 3;
                    int ventanaY = yBase + altoPixels / 4;
                    int ventanaW = anchoPixels / 3;
                    int ventanaH = altoPixels / 2;

                    using (var penPrim = new Pen(Color.Blue, 2))
                    using (var penSec = new Pen(Color.Red, 2))
                    {
                        g.DrawEllipse(penPrim, ventanaX + 5, ventanaY + 10, ventanaW / 2 - 10, ventanaH - 20);
                        g.DrawEllipse(penSec, ventanaX + ventanaW / 2 + 5, ventanaY + 10, ventanaW / 2 - 10, ventanaH - 20);
                    }

                    g.DrawString($"Ae={r.AreaNucleoEfectiva:F1}cm²", font, brushTexto, xBase, yBase + altoPixels + 10);
                    g.DrawString($"{r.AnchoNucleo:F1}×{r.AltoNucleo:F1}cm", font, brushTexto, xBase, yBase + altoPixels + 25);
                    g.DrawString($"Primario\n{r.EspirasPrimario}N", font, Brushes.Blue, ventanaX + 5, ventanaY - 15);
                    g.DrawString($"Secundario\n{r.EspirasSecundario}N", font, Brushes.Red, ventanaX + ventanaW / 2 + 5, ventanaY - 15);
                }

                // Gráfico de pérdidas
                int xGraf = 280, yGraf = 30;
                DibujarGraficoPerdidas(g, xGraf, yGraf, r);

                // Curva de regulación
                DibujarCurvaRegulacion(g, xGraf, yGraf + 120, r);
            }

            picDiagramas.Image?.Dispose();
            picDiagramas.Image = bmp;
        }

        private void DibujarGraficoPerdidas(Graphics g, int x, int y, TransformerResult r)
        {
            using (var font = new Font("Arial", 8))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString("Distribución de Pérdidas", font, brush, x, y);

                double total = r.PerdidasTotales;
                int anchoTotal = 200;
                int altoBar = 20;

                int anchoNucleo = (int)(r.PerdidasNucleo / total * anchoTotal);
                int anchoCobre = (int)(r.PerdidasCobre / total * anchoTotal);

                g.FillRectangle(Brushes.Orange, x, y + 20, anchoNucleo, altoBar);
                g.FillRectangle(Brushes.Brown, x + anchoNucleo, y + 20, anchoCobre, altoBar);

                g.DrawString($"Núcleo: {r.PerdidasNucleo:F1}W", font, brush, x, y + 45);
                g.DrawString($"Cobre: {r.PerdidasCobre:F1}W", font, brush, x + 100, y + 45);
            }
        }

        private void DibujarCurvaRegulacion(Graphics g, int x, int y, TransformerResult r)
        {
            using (var font = new Font("Arial", 8))
            using (var pen = new Pen(Color.DarkGreen, 2))
            {
                g.DrawString("Regulación de Voltaje vs Carga", font, Brushes.Black, x, y);

                g.DrawLine(Pens.Black, x, y + 80, x + 200, y + 80);
                g.DrawLine(Pens.Black, x, y + 20, x, y + 80);

                for (int i = 0; i <= 10; i++)
                {
                    double carga = i / 10.0;
                    double caida = r.RegulacionVoltaje * carga;
                    int px = x + i * 20;
                    int py = y + 80 - (int)(caida * 2);

                    if (i > 0)
                        g.DrawLine(pen, x + (i - 1) * 20, y + 80 - (int)(r.RegulacionVoltaje * (i - 1) / 10.0 * 2), px, py);
                }

                g.DrawString("0%", font, Brushes.Black, x - 5, y + 82);
                g.DrawString("100%", font, Brushes.Black, x + 185, y + 82);
            }
        }

        private void BtnExportarDiagrama_Click(object sender, EventArgs e)
        {
            if (picDiagramas.Image == null)
            {
                MessageBox.Show("No hay diagrama para exportar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                sfd.FileName = "diagrama_transformador.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    picDiagramas.Image.Save(sfd.FileName);
                    MessageBox.Show("Diagrama exportado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtVoltajePrimario.Clear();
            txtVoltajeSecundario.Clear();
            txtCorrienteSecundaria.Clear();
            txtFrecuencia.Text = "60";
            txtEficiencia.Text = "95";
            txtDensidadCorriente.Text = "3.0";
            txtFactorLlenado.Text = "0.4";
            txtTempAmbiente.Text = "25";
            txtElevacionTemp.Text = "50";
            txtResultados.Clear();
            picDiagramas.Image?.Dispose();
            picDiagramas.Image = null;
            rbPrimarioMonofasico.Checked = true;
            rbSecundarioMonofasico.Checked = true;
            cmbLaminado.SelectedIndex = 0;
        }
    }
}