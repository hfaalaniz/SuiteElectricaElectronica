using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public class VFD : Form
    {
        private ComboBox cboTipo, cboFrecuencia, cboFases, cboImpedancia;
        private TextBox txtPotencia, txtVoltaje;
        private TextBox txtCorriente, txtInductancia, txtVoltajeCaida, txtReactancia;
        private TextBox txtNucleo, txtEspiras, txtSeccionCable, txtDiametroCable;
        private TextBox txtPesoAprox, txtLongitudCable;
        private Button btnCalcular, btnLimpiar, btnExportar;
        private Label lblResultados;
        private Panel panelResultados, panelDiseno;

        public VFD()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Calculadora de Reactores de Línea VFD - Avanzada";
            Size = new Size(750, 820);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 9F);

            int y = 15;
            int leftMargin = 20;

            // Panel de entrada
            var panelEntrada = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(715, 260),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            Controls.Add(panelEntrada);

            var lblTitulo = new Label
            {
                Text = "PARÁMETROS DE ENTRADA",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelEntrada.Controls.Add(lblTitulo);

            y = 35;

            // Tipo de reactor
            AddLabelToPanel(panelEntrada, "Tipo de Reactor:", leftMargin, y);
            cboTipo = new ComboBox { Location = new Point(180, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTipo.Items.AddRange(new[] { "Entrada (AC Line)", "Salida (Load)", "DC Bus" });
            cboTipo.SelectedIndex = 0;
            panelEntrada.Controls.Add(cboTipo);

            // Frecuencia
            y += 35;
            AddLabelToPanel(panelEntrada, "Frecuencia (Hz):", leftMargin, y);
            cboFrecuencia = new ComboBox { Location = new Point(180, y), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFrecuencia.Items.AddRange(new[] { "50", "60" });
            cboFrecuencia.SelectedIndex = 1;
            panelEntrada.Controls.Add(cboFrecuencia);

            // Fases
            AddLabelToPanel(panelEntrada, "Sistema:", 320, y);
            cboFases = new ComboBox { Location = new Point(420, y), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFases.Items.AddRange(new[] { "Monofásico", "Trifásico" });
            cboFases.SelectedIndex = 1;
            panelEntrada.Controls.Add(cboFases);

            // Potencia
            y += 35;
            AddLabelToPanel(panelEntrada, "Potencia Motor (kW):", leftMargin, y);
            txtPotencia = new TextBox { Location = new Point(180, y), Width = 100 };
            panelEntrada.Controls.Add(txtPotencia);

            // Voltaje
            AddLabelToPanel(panelEntrada, "Voltaje (V):", 320, y);
            txtVoltaje = new TextBox { Location = new Point(420, y), Width = 100 };
            txtVoltaje.Text = "380";
            panelEntrada.Controls.Add(txtVoltaje);

            // Impedancia
            y += 35;
            AddLabelToPanel(panelEntrada, "Impedancia (%):", leftMargin, y);
            cboImpedancia = new ComboBox { Location = new Point(180, y), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cboImpedancia.Items.AddRange(new[] { "1", "2", "3", "4", "5", "7", "10" });
            cboImpedancia.SelectedIndex = 4; // 5%
            panelEntrada.Controls.Add(cboImpedancia);
            AddLabelToPanel(panelEntrada, "(3% salida, 5% entrada típico)", 290, y);

            // Botones
            y += 45;
            btnCalcular = new Button
            {
                Text = "Calcular",
                Location = new Point(180, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCalcular.FlatAppearance.BorderSize = 0;
            btnCalcular.Click += BtnCalcular_Click;
            panelEntrada.Controls.Add(btnCalcular);

            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(290, y),
                Width = 100,
                Height = 35,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            panelEntrada.Controls.Add(btnLimpiar);

            btnExportar = new Button
            {
                Text = "Exportar",
                Location = new Point(400, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += BtnExportar_Click;
            panelEntrada.Controls.Add(btnExportar);

            // Panel de resultados eléctricos
            y = 285;
            panelResultados = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(715, 220),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            Controls.Add(panelResultados);

            lblResultados = new Label
            {
                Text = "RESULTADOS ELÉCTRICOS",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelResultados.Controls.Add(lblResultados);

            y = 35;
            AddLabelToPanel(panelResultados, "Corriente Nominal (A):", leftMargin, y);
            txtCorriente = CreateReadOnlyTextBox(180, y, 100);
            panelResultados.Controls.Add(txtCorriente);

            AddLabelToPanel(panelResultados, "Reactancia (Ω):", 320, y);
            txtReactancia = CreateReadOnlyTextBox(450, y, 100);
            panelResultados.Controls.Add(txtReactancia);

            y += 35;
            AddLabelToPanel(panelResultados, "Inductancia (mH):", leftMargin, y);
            txtInductancia = CreateReadOnlyTextBox(180, y, 100);
            panelResultados.Controls.Add(txtInductancia);

            AddLabelToPanel(panelResultados, "Inductancia (H):", 320, y);
            var txtInductanciaH = CreateReadOnlyTextBox(450, y, 100);
            txtInductanciaH.Tag = "inductanciaH";
            panelResultados.Controls.Add(txtInductanciaH);

            y += 35;
            AddLabelToPanel(panelResultados, "Caída Voltaje (V):", leftMargin, y);
            txtVoltajeCaida = CreateReadOnlyTextBox(180, y, 100);
            panelResultados.Controls.Add(txtVoltajeCaida);

            AddLabelToPanel(panelResultados, "Caída (%):", 320, y);
            var txtCaidaPorcentaje = CreateReadOnlyTextBox(450, y, 100);
            txtCaidaPorcentaje.Tag = "caidaPct";
            panelResultados.Controls.Add(txtCaidaPorcentaje);

            y += 35;
            AddLabelToPanel(panelResultados, "Pérdidas (W):", leftMargin, y);
            var txtPerdidas = CreateReadOnlyTextBox(180, y, 100);
            txtPerdidas.Tag = "perdidas";
            panelResultados.Controls.Add(txtPerdidas);

            AddLabelToPanel(panelResultados, "Energía (kWh/año):", 320, y);
            var txtEnergia = CreateReadOnlyTextBox(450, y, 100);
            txtEnergia.Tag = "energia";
            panelResultados.Controls.Add(txtEnergia);

            // Panel de diseño físico
            y = 515;
            panelDiseno = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(715, 260),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            Controls.Add(panelDiseno);

            var lblDiseno = new Label
            {
                Text = "DISEÑO FÍSICO APROXIMADO",
                Location = new Point(10, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelDiseno.Controls.Add(lblDiseno);

            y = 35;
            AddLabelToPanel(panelDiseno, "Núcleo (mm):", leftMargin, y);
            txtNucleo = CreateReadOnlyTextBox(180, y, 150);
            panelDiseno.Controls.Add(txtNucleo);

            AddLabelToPanel(panelDiseno, "Área (cm²):", 370, y);
            var txtArea = CreateReadOnlyTextBox(480, y, 100);
            txtArea.Tag = "area";
            panelDiseno.Controls.Add(txtArea);

            y += 35;
            AddLabelToPanel(panelDiseno, "Espiras por fase:", leftMargin, y);
            txtEspiras = CreateReadOnlyTextBox(180, y, 100);
            panelDiseno.Controls.Add(txtEspiras);

            AddLabelToPanel(panelDiseno, "Ventana (cm²):", 320, y);
            var txtVentana = CreateReadOnlyTextBox(480, y, 100);
            txtVentana.Tag = "ventana";
            panelDiseno.Controls.Add(txtVentana);

            y += 35;
            AddLabelToPanel(panelDiseno, "Sección cable (mm²):", leftMargin, y);
            txtSeccionCable = CreateReadOnlyTextBox(180, y, 100);
            panelDiseno.Controls.Add(txtSeccionCable);

            AddLabelToPanel(panelDiseno, "AWG equiv.:", 320, y);
            var txtAWG = CreateReadOnlyTextBox(480, y, 100);
            txtAWG.Tag = "awg";
            panelDiseno.Controls.Add(txtAWG);

            y += 35;
            AddLabelToPanel(panelDiseno, "Diámetro cable (mm):", leftMargin, y);
            txtDiametroCable = CreateReadOnlyTextBox(180, y, 100);
            panelDiseno.Controls.Add(txtDiametroCable);

            AddLabelToPanel(panelDiseno, "Long. cable (m):", 320, y);
            txtLongitudCable = CreateReadOnlyTextBox(480, y, 100);
            panelDiseno.Controls.Add(txtLongitudCable);

            y += 35;
            AddLabelToPanel(panelDiseno, "Peso aprox. (kg):", leftMargin, y);
            txtPesoAprox = CreateReadOnlyTextBox(180, y, 100);
            panelDiseno.Controls.Add(txtPesoAprox);

            AddLabelToPanel(panelDiseno, "Temp. rise (°C):", 320, y);
            var txtTemp = CreateReadOnlyTextBox(480, y, 100);
            txtTemp.Tag = "temp";
            panelDiseno.Controls.Add(txtTemp);
        }

        private void AddLabelToPanel(Panel panel, string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSize = true
            };
            panel.Controls.Add(lbl);
        }

        private TextBox CreateReadOnlyTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(txtPotencia.Text, out double potenciaKW) || potenciaKW <= 0)
                {
                    MessageBox.Show("Ingrese potencia válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtVoltaje.Text, out double voltaje) || voltaje <= 0)
                {
                    MessageBox.Show("Ingrese voltaje válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double frecuencia = double.Parse(cboFrecuencia.Text);
                double impedanciaPct = double.Parse(cboImpedancia.Text);
                double impedancia = impedanciaPct / 100.0;
                bool esTrifasico = cboFases.SelectedIndex == 1;

                // Calcular corriente nominal con factor de potencia más preciso
                double factorPotencia = 0.87; // FP típico motores industriales
                double corriente;
                if (esTrifasico)
                {
                    corriente = (potenciaKW * 1000) / (Math.Sqrt(3) * voltaje * factorPotencia);
                }
                else
                {
                    corriente = (potenciaKW * 1000) / (voltaje * factorPotencia);
                }

                // Reactancia inductiva más precisa
                double voltajeFase = esTrifasico ? voltaje / Math.Sqrt(3) : voltaje;
                double reactanciaOhms = (impedancia * voltajeFase) / corriente;

                // Inductancia precisa
                double inductanciaH = reactanciaOhms / (2 * Math.PI * frecuencia);
                double inductanciaMH = inductanciaH * 1000;

                // Caída de voltaje
                double voltajeCaida = voltaje * impedancia;
                double caidaPorcentaje = impedancia * 100;

                // Pérdidas (aproximadas con resistencia típica R = 0.1 * XL)
                double resistenciaOhms = reactanciaOhms * 0.1;
                double perdidas = (esTrifasico ? 3 : 1) * Math.Pow(corriente, 2) * resistenciaOhms;
                double energiaAnual = perdidas * 8760 / 1000; // kWh/año (24/7 operación)

                // Diseño físico más preciso
                double areaNucleo = CalcularAreaNucleo(potenciaKW, frecuencia);
                int espiras = CalcularEspiras(voltajeFase, frecuencia, areaNucleo);

                // Sección conductor con densidad ajustada por frecuencia
                double densidadCorriente = frecuencia == 50 ? 3.5 : 4.0; // A/mm²
                double seccionCable = corriente / densidadCorriente;
                double diametroCable = Math.Sqrt((4 * seccionCable) / Math.PI);

                // AWG equivalente
                int awg = SeccionToAWG(seccionCable);

                // Longitud y peso del cable
                double longitudCable = CalcularLongitudCable(espiras, areaNucleo, esTrifasico);
                double pesoAprox = CalcularPeso(areaNucleo, longitudCable, seccionCable);

                // Ventana del núcleo
                double ventanaNucleo = areaNucleo * 0.4; // Factor de ventana típico 0.4

                // Elevación de temperatura estimada
                double tempRise = (perdidas / (pesoAprox * 0.5)) * 20; // °C aprox

                // Mostrar resultados
                txtCorriente.Text = corriente.ToString("F2");
                txtReactancia.Text = reactanciaOhms.ToString("F3");
                txtInductancia.Text = inductanciaMH.ToString("F2");
                FindControlByTag("inductanciaH").Text = inductanciaH.ToString("F6");
                txtVoltajeCaida.Text = voltajeCaida.ToString("F2");
                FindControlByTag("caidaPct").Text = caidaPorcentaje.ToString("F1");
                FindControlByTag("perdidas").Text = perdidas.ToString("F1");
                FindControlByTag("energia").Text = energiaAnual.ToString("F0");

                txtNucleo.Text = $"{Math.Sqrt(areaNucleo * 100):F0} x {Math.Sqrt(areaNucleo * 100):F0}";
                FindControlByTag("area").Text = areaNucleo.ToString("F2");
                txtEspiras.Text = espiras.ToString();
                FindControlByTag("ventana").Text = ventanaNucleo.ToString("F2");
                txtSeccionCable.Text = seccionCable.ToString("F2");
                FindControlByTag("awg").Text = $"AWG {awg}";
                txtDiametroCable.Text = diametroCable.ToString("F2");
                txtLongitudCable.Text = longitudCable.ToString("F1");
                txtPesoAprox.Text = pesoAprox.ToString("F2");
                FindControlByTag("temp").Text = tempRise.ToString("F1");

                MostrarRecomendaciones(cboTipo.Text, corriente, inductanciaMH, impedanciaPct,
                    voltajeCaida, perdidas, tempRise);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en cálculo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox FindControlByTag(string tag)
        {
            foreach (Control panel in Controls)
            {
                if (panel is Panel)
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is TextBox && ctrl.Tag?.ToString() == tag)
                            return (TextBox)ctrl;
                    }
                }
            }
            return null;
        }

        private double CalcularAreaNucleo(double potenciaKW, double frecuencia)
        {
            // Fórmula mejorada considerando frecuencia
            double k = frecuencia == 50 ? 2.5 : 2.8;
            return Math.Pow(potenciaKW / k, 0.5) * 10;
        }

        private int CalcularEspiras(double voltajeFase, double frecuencia, double areaNucleo)
        {
            // N = V / (4.44 * f * A * B)
            double B = 1.2; // Tesla (densidad flujo optimizada)
            double areaM2 = areaNucleo / 10000; // cm² a m²
            double espiras = voltajeFase / (4.44 * frecuencia * areaM2 * B);
            return (int)Math.Ceiling(espiras);
        }

        private double CalcularLongitudCable(int espiras, double areaNucleo, bool trifasico)
        {
            double perimetro = 4 * Math.Sqrt(areaNucleo * 100) / 1000; // metros
            double longitudPorFase = espiras * perimetro * 1.15; // 15% extra por conexiones
            return longitudPorFase * (trifasico ? 3 : 1);
        }

        private double CalcularPeso(double areaNucleo, double longitudCable, double seccionCable)
        {
            // Peso núcleo (hierro silicio ~7.65 kg/dm³)
            double volumenNucleo = areaNucleo * Math.Sqrt(areaNucleo * 100) * 4 / 1000; // dm³
            double pesoNucleo = volumenNucleo * 7.65;

            // Peso cable (cobre ~8.9 kg/dm³)
            double volumenCable = (seccionCable / 100) * longitudCable / 10; // dm³
            double pesoCable = volumenCable * 8.9;

            return pesoNucleo + pesoCable;
        }

        private int SeccionToAWG(double seccionMM2)
        {
            double[] awgSecciones = { 0.205, 0.324, 0.511, 0.823, 1.31, 2.08, 3.31, 5.26, 8.37,
                13.3, 21.2, 33.6, 53.5, 85.0, 107 };
            int[] awgNumeros = { 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4, 2, 1, 0, 2 };

            for (int i = 0; i < awgSecciones.Length; i++)
            {
                if (seccionMM2 <= awgSecciones[i])
                    return awgNumeros[i];
            }
            return 4; // Muy grande
        }

        private void MostrarRecomendaciones(string tipo, double corriente, double inductancia,
            double impedancia, double voltajeCaida, double perdidas, double tempRise)
        {
            string msg = $"=== CÁLCULO COMPLETADO ===\n\n";
            msg += $"Tipo: {tipo}\n";
            msg += $"Corriente: {corriente:F2} A\n";
            msg += $"Inductancia: {inductancia:F2} mH\n";
            msg += $"Impedancia: {impedancia:F1}%\n";
            msg += $"Caída voltaje: {voltajeCaida:F2} V\n";
            msg += $"Pérdidas: {perdidas:F1} W\n";
            msg += $"Temp. estimada: +{tempRise:F1}°C\n\n";

            msg += "--- RECOMENDACIONES ---\n";
            if (tipo.Contains("Entrada"))
            {
                msg += "✓ Reduce armónicos entrada (THD)\n";
                msg += "✓ Protege red eléctrica\n";
                msg += "✓ Mejora FP y cumple normas\n";
                msg += "✓ Ubicación: Entre red y VFD\n";
                msg += "✓ Impedancia óptima: 3-5%";
            }
            else if (tipo.Contains("Salida"))
            {
                msg += "✓ Limita dV/dt (<2000 V/µs)\n";
                msg += "✓ Reduce corrientes fuga\n";
                msg += "✓ Protege cables largos >30m\n";
                msg += "✓ Ubicación: Entre VFD y motor\n";
                msg += "✓ Impedancia óptima: 2-4%";
            }
            else
            {
                msg += "✓ Suaviza ripple bus DC\n";
                msg += "✓ Mejora regulación\n";
                msg += "✓ Reduce stress capacitores\n";
                msg += "✓ Mayor vida útil VFD";
            }

            if (tempRise > 50)
                msg += "\n\n⚠ ADVERTENCIA: Temp. alta, considere refrigeración";

            MessageBox.Show(msg, "Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                string datos = "=== REACTOR VFD - DATOS CALCULADOS ===\n\n";
                datos += $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n";
                datos += "PARÁMETROS:\n";
                datos += $"Tipo: {cboTipo.Text}\n";
                datos += $"Potencia: {txtPotencia.Text} kW\n";
                datos += $"Voltaje: {txtVoltaje.Text} V\n";
                datos += $"Frecuencia: {cboFrecuencia.Text} Hz\n";
                datos += $"Sistema: {cboFases.Text}\n";
                datos += $"Impedancia: {cboImpedancia.Text}%\n\n";
                datos += "RESULTADOS ELÉCTRICOS:\n";
                datos += $"Corriente: {txtCorriente.Text} A\n";
                datos += $"Reactancia: {txtReactancia.Text} Ω\n";
                datos += $"Inductancia: {txtInductancia.Text} mH\n";
                datos += $"Caída voltaje: {txtVoltajeCaida.Text} V\n\n";
                datos += "DISEÑO FÍSICO:\n";
                datos += $"Núcleo: {txtNucleo.Text} mm\n";
                datos += $"Espiras: {txtEspiras.Text}\n";
                datos += $"Sección cable: {txtSeccionCable.Text} mm²\n";
                datos += $"Diámetro: {txtDiametroCable.Text} mm\n";
                datos += $"Longitud: {txtLongitudCable.Text} m\n";
                datos += $"Peso: {txtPesoAprox.Text} kg\n";

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Archivo texto|*.txt",
                    FileName = $"Reactor_{DateTime.Now:yyyyMMdd_HHmm}.txt"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(sfd.FileName, datos);
                    MessageBox.Show("Datos exportados correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtPotencia.Clear();
            txtVoltaje.Text = "380";
            txtCorriente.Clear();
            txtReactancia.Clear();
            txtInductancia.Clear();
            txtVoltajeCaida.Clear();
            txtNucleo.Clear();
            txtEspiras.Clear();
            txtSeccionCable.Clear();
            txtDiametroCable.Clear();
            txtLongitudCable.Clear();
            txtPesoAprox.Clear();

            foreach (Control panel in Controls)
            {
                if (panel is Panel)
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is TextBox && ctrl.Tag != null)
                            ctrl.Text = "";
                    }
                }
            }

            cboTipo.SelectedIndex = 0;
            cboFrecuencia.SelectedIndex = 1;
            cboFases.SelectedIndex = 1;
            cboImpedancia.SelectedIndex = 4;
        }

    }
}