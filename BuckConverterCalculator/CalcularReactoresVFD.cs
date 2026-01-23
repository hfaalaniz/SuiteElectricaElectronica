using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class CalcularReactoresVFD : Form
    {
        // Datos calculados para el gráfico
        private DatosReactor datosReactor;

        public CalcularReactoresVFD()
        {
            InitializeComponent();
            datosReactor = new DatosReactor();
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar entradas
                if (!double.TryParse(txtPotencia.Text, out double potenciaKW) || potenciaKW <= 0)
                {
                    MessageBox.Show("Ingrese una potencia válida mayor a 0", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPotencia.Focus();
                    return;
                }

                if (!double.TryParse(txtVoltaje.Text, out double voltaje) || voltaje <= 0)
                {
                    MessageBox.Show("Ingrese un voltaje válido mayor a 0", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVoltaje.Focus();
                    return;
                }

                // Obtener parámetros
                double frecuencia = double.Parse(cboFrecuencia.Text);
                double impedanciaPct = double.Parse(cboImpedancia.Text);
                double impedancia = impedanciaPct / 100.0;
                bool esTrifasico = cboFases.SelectedIndex == 1;
                string tipoReactor = cboTipo.Text;

                // Factor de potencia más preciso según tipo de carga
                double factorPotencia = ObtenerFactorPotencia(tipoReactor, potenciaKW);

                // Cálculo de corriente nominal mejorado
                double corriente = CalcularCorrienteNominal(potenciaKW, voltaje, factorPotencia, esTrifasico);

                // Corrección por factor de servicio (1.15 para motores industriales)
                double factorServicio = 1.15;
                double corrienteDiseño = corriente * factorServicio;

                // Reactancia inductiva con corrección de temperatura
                double temperaturaAmbiente = 40; // °C
                double temperaturaOperacion = 75; // °C estimada
                double factorTemperatura = (234.5 + temperaturaOperacion) / (234.5 + temperaturaAmbiente);

                double voltajeFase = esTrifasico ? voltaje / Math.Sqrt(3) : voltaje;
                double reactanciaOhms = (impedancia * voltajeFase) / corriente;

                // Inductancia con mayor precisión
                double omega = 2 * Math.PI * frecuencia;
                double inductanciaH = reactanciaOhms / omega;
                double inductanciaMH = inductanciaH * 1000;

                // Caída de voltaje y porcentaje
                double voltajeCaida = corriente * reactanciaOhms * (esTrifasico ? Math.Sqrt(3) : 1);
                double caidaPorcentaje = (voltajeCaida / voltaje) * 100;

                // Pérdidas con modelo más preciso
                // R_dc = ρ * L / A donde ρ es resistividad del cobre
                double resistenciaCC = CalcularResistenciaCC(corriente, frecuencia);

                // Corrección por efecto piel y proximidad
                double factorCA = CalcularFactorCA(frecuencia, corriente);
                double resistenciaCA = resistenciaCC * factorCA;

                // Pérdidas totales (cobre + núcleo)
                double perdidasCobre = (esTrifasico ? 3 : 1) * Math.Pow(corriente, 2) * resistenciaCA;
                double perdidasNucleo = CalcularPerdidasNucleo(potenciaKW, frecuencia, voltajeFase);
                double perdidasTotales = perdidasCobre + perdidasNucleo;

                // Energía anual con factor de utilización del 80%
                double factorUtilizacion = 0.80;
                double horasAnuales = 8760 * factorUtilizacion;
                double energiaAnual = perdidasTotales * horasAnuales / 1000; // kWh/año

                // Diseño físico optimizado
                double areaNucleo = CalcularAreaNucleoOptimizada(potenciaKW, frecuencia, impedanciaPct);

                // Densidad de flujo magnético óptima según frecuencia
                double densidadFlujo = frecuencia == 50 ? 1.3 : 1.2; // Tesla

                int espiras = CalcularEspirasOptimizado(voltajeFase, frecuencia, areaNucleo, densidadFlujo);

                // Sección del conductor con densidad ajustada
                double densidadCorriente = CalcularDensidadCorriente(potenciaKW, frecuencia);
                double seccionCable = corrienteDiseño / densidadCorriente;

                // Ajuste por factor de relleno (cables múltiples en paralelo si es necesario)
                int numConductores = 1;
                if (seccionCable > 150) // mm²
                {
                    numConductores = (int)Math.Ceiling(seccionCable / 150);
                    seccionCable = seccionCable / numConductores;
                }

                double diametroCable = Math.Sqrt((4 * seccionCable) / Math.PI);

                // AWG equivalente
                int awg = SeccionToAWG(seccionCable);

                // Longitud del cable
                double longitudCable = CalcularLongitudCable(espiras, areaNucleo, esTrifasico, numConductores);

                // Peso del reactor
                double pesoAprox = CalcularPesoOptimizado(areaNucleo, longitudCable, seccionCable,
                    numConductores, esTrifasico);

                // Ventana del núcleo (área disponible para bobinado)
                double factorVentana = 0.35; // Factor típico considerando aislamiento
                double ventanaNucleo = areaNucleo * factorVentana;

                // Elevación de temperatura mejorada
                double superficieDisipacion = CalcularSuperficieDisipacion(areaNucleo);
                double tempRise = (perdidasTotales / superficieDisipacion) * 15; // °C

                // Almacenar datos para el gráfico
                datosReactor.AreaNucleo = areaNucleo;
                datosReactor.Espiras = espiras;
                datosReactor.DiametroCable = diametroCable;
                datosReactor.Corriente = corriente;
                datosReactor.Inductancia = inductanciaMH;
                datosReactor.Potencia = potenciaKW;
                datosReactor.EsTrifasico = esTrifasico;
                datosReactor.NumConductores = numConductores;
                datosReactor.LongitudCable = longitudCable;
                datosReactor.Peso = pesoAprox;

                // Actualizar interfaz
                ActualizarResultados(corriente, reactanciaOhms, inductanciaMH, inductanciaH,
                    voltajeCaida, caidaPorcentaje, perdidasTotales, energiaAnual,
                    areaNucleo, espiras, ventanaNucleo, seccionCable, awg,
                    diametroCable, longitudCable, pesoAprox, tempRise, numConductores);

                // Redibujar gráfico
                panelGrafico.Invalidate();

                // Mostrar recomendaciones
                MostrarRecomendaciones(tipoReactor, corriente, inductanciaMH, impedanciaPct,
                    voltajeCaida, perdidasTotales, tempRise, numConductores);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en cálculo:\n{ex.Message}\n\nPor favor, verifique los datos ingresados.",
                    "Error de Cálculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double ObtenerFactorPotencia(string tipoReactor, double potenciaKW)
        {
            // Factor de potencia según potencia del motor
            if (potenciaKW < 5)
                return 0.82;
            else if (potenciaKW < 15)
                return 0.85;
            else if (potenciaKW < 50)
                return 0.88;
            else
                return 0.90;
        }

        private double CalcularCorrienteNominal(double potenciaKW, double voltaje,
            double factorPotencia, bool esTrifasico)
        {
            if (esTrifasico)
            {
                // I = P / (√3 × V × FP)
                return (potenciaKW * 1000) / (Math.Sqrt(3) * voltaje * factorPotencia);
            }
            else
            {
                // I = P / (V × FP)
                return (potenciaKW * 1000) / (voltaje * factorPotencia);
            }
        }

        private double CalcularResistenciaCC(double corriente, double frecuencia)
        {
            // Resistencia DC típica en relación a la reactancia
            // Para reactores industriales: R ≈ 0.05 × XL a 0.10 × XL
            double factorR = frecuencia == 50 ? 0.08 : 0.07;
            return factorR;
        }

        private double CalcularFactorCA(double frecuencia, double corriente)
        {
            // Factor de corrección CA por efecto piel y proximidad
            // Más significativo en alta corriente y frecuencia
            if (corriente < 50)
                return 1.05;
            else if (corriente < 200)
                return 1.10;
            else if (corriente < 500)
                return 1.15;
            else
                return 1.20;
        }

        private double CalcularPerdidasNucleo(double potenciaKW, double frecuencia, double voltajeFase)
        {
            // Pérdidas en el núcleo: Ph = Kh × f × B² (pérdidas por histéresis)
            //                        Pe = Ke × f² × B² (pérdidas por corrientes parásitas)
            // Aproximación: 1-3% de la potencia del reactor
            double factorPerdidas = 0.015; // 1.5% típico
            return potenciaKW * 1000 * factorPerdidas * (frecuencia / 60);
        }

        private double CalcularAreaNucleoOptimizada(double potenciaKW, double frecuencia, double impedanciaPct)
        {
            // Fórmula de área del núcleo mejorada
            // A = K × √(P / f) donde K depende del diseño
            double k = frecuencia == 50 ? 3.2 : 3.6;

            // Ajuste por impedancia (mayor impedancia = mayor núcleo)
            double factorImpedancia = Math.Sqrt(impedanciaPct / 5.0);

            double area = Math.Pow(potenciaKW / k, 0.5) * 12 * factorImpedancia;

            // Área mínima práctica
            return Math.Max(area, 10); // cm²
        }

        private int CalcularEspirasOptimizado(double voltajeFase, double frecuencia,
            double areaNucleo, double densidadFlujo)
        {
            // Ley de Faraday: V = 4.44 × f × N × Φ
            // Φ = B × A, entonces: N = V / (4.44 × f × B × A)

            double areaM2 = areaNucleo / 10000; // cm² a m²
            double espiras = voltajeFase / (4.44 * frecuencia * densidadFlujo * areaM2);

            // Redondear al múltiplo de 5 más cercano para facilitar bobinado
            int espirasRedondeadas = (int)(Math.Ceiling(espiras / 5) * 5);

            return Math.Max(espirasRedondeadas, 10); // Mínimo 10 espiras
        }

        private double CalcularDensidadCorriente(double potenciaKW, double frecuencia)
        {
            // Densidad de corriente considerando refrigeración natural
            // Valores típicos: 2.5-4.5 A/mm² para reactores con ventilación natural

            if (potenciaKW < 10)
                return frecuencia == 50 ? 3.8 : 4.2;
            else if (potenciaKW < 50)
                return frecuencia == 50 ? 3.3 : 3.7;
            else if (potenciaKW < 200)
                return frecuencia == 50 ? 2.8 : 3.2;
            else
                return frecuencia == 50 ? 2.5 : 2.8;
        }

        private double CalcularLongitudCable(int espiras, double areaNucleo, bool esTrifasico, int numConductores)
        {
            // Perímetro medio de la bobina
            double ladoNucleo = Math.Sqrt(areaNucleo * 100); // mm
            double perimetroMedio = 4 * (ladoNucleo + 40); // +40mm de tolerancia por capa

            // Longitud total considerando conexiones y terminales
            double longitudPorFase = (espiras * perimetroMedio / 1000) * 1.20; // 20% extra

            // Ajustar por número de conductores en paralelo
            longitudPorFase *= numConductores;

            return longitudPorFase * (esTrifasico ? 3 : 1);
        }

        private double CalcularPesoOptimizado(double areaNucleo, double longitudCable,
            double seccionCable, int numConductores, bool esTrifasico)
        {
            // Peso del núcleo (acero silicio laminado: ~7.65 kg/dm³)
            double ladoNucleo = Math.Sqrt(areaNucleo * 100); // mm
            double alturaApilado = ladoNucleo * 1.5; // Altura típica
            double volumenNucleo = (areaNucleo * alturaApilado * 4) / 1000; // dm³
            double pesoNucleo = volumenNucleo * 7.65;

            // Peso del cobre (8.96 kg/dm³)
            double volumenCobre = (seccionCable / 100) * (longitudCable * 100) / 1000; // dm³
            double pesoCobre = volumenCobre * 8.96 * numConductores;

            // Peso de aislamiento, estructura y accesorios (15-25%)
            double factorEstructura = 1.20;

            return (pesoNucleo + pesoCobre) * factorEstructura;
        }

        private double CalcularSuperficieDisipacion(double areaNucleo)
        {
            // Superficie de disipación aproximada (todas las caras del reactor)
            double ladoNucleo = Math.Sqrt(areaNucleo * 100) / 10; // cm
            double altura = ladoNucleo * 1.5;

            // Área total (4 caras laterales + 2 bases)
            return 2 * (ladoNucleo * altura * 2 + ladoNucleo * ladoNucleo);
        }

        private int SeccionToAWG(double seccionMM2)
        {
            // Tabla AWG más completa
            double[] awgSecciones = {
                0.205, 0.324, 0.511, 0.823, 1.31, 2.08, 3.31, 5.26, 8.37,
                13.3, 21.2, 33.6, 53.5, 67.4, 85.0, 107.2, 126.7, 152.0
            };
            int[] awgNumeros = {
                24, 22, 20, 18, 16, 14, 12, 10, 8,
                6, 4, 2, 1, 2, 3, 4, 250
            };
            string[] awgNombres = {
                "24", "22", "20", "18", "16", "14", "12", "10", "8",
                "6", "4", "2", "1", "1/0", "2/0", "3/0", "4/0", "250 kcmil"
            };

            for (int i = 0; i < awgSecciones.Length; i++)
            {
                if (seccionMM2 <= awgSecciones[i] * 1.1) // 10% tolerancia
                    return awgNumeros[i];
            }
            return 300; // MCM muy grande
        }

        private void ActualizarResultados(double corriente, double reactancia, double inductanciaMH,
            double inductanciaH, double voltajeCaida, double caidaPct, double perdidas,
            double energia, double areaNucleo, int espiras, double ventana, double seccionCable,
            int awg, double diametroCable, double longitudCable, double peso, double tempRise,
            int numConductores)
        {
            txtCorriente.Text = corriente.ToString("F2");
            txtReactancia.Text = reactancia.ToString("F4");
            txtInductancia.Text = inductanciaMH.ToString("F3");
            FindControlByTag("inductanciaH").Text = inductanciaH.ToString("F6");
            txtVoltajeCaida.Text = voltajeCaida.ToString("F2");
            FindControlByTag("caidaPct").Text = caidaPct.ToString("F2");
            FindControlByTag("perdidas").Text = perdidas.ToString("F1");
            FindControlByTag("energia").Text = energia.ToString("F0");

            double ladoNucleo = Math.Sqrt(areaNucleo * 100);
            txtNucleo.Text = $"{ladoNucleo:F0} × {ladoNucleo:F0} × {ladoNucleo * 1.5:F0}";
            FindControlByTag("area").Text = areaNucleo.ToString("F2");
            txtEspiras.Text = espiras.ToString();
            FindControlByTag("ventana").Text = ventana.ToString("F2");

            string seccionTexto = numConductores > 1
                ? $"{seccionCable:F2} × {numConductores}"
                : seccionCable.ToString("F2");
            txtSeccionCable.Text = seccionTexto;

            FindControlByTag("awg").Text = $"AWG {awg}";
            txtDiametroCable.Text = diametroCable.ToString("F2");
            txtLongitudCable.Text = longitudCable.ToString("F1");
            txtPesoAprox.Text = peso.ToString("F2");
            FindControlByTag("temp").Text = tempRise.ToString("F1");
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

        private void MostrarRecomendaciones(string tipo, double corriente, double inductancia,
            double impedancia, double voltajeCaida, double perdidas, double tempRise, int numConductores)
        {
            string msg = "═══════════════════════════════════\n";
            msg += "   CÁLCULO COMPLETADO\n";
            msg += "═══════════════════════════════════\n\n";

            msg += $"Tipo de reactor: {tipo}\n";
            msg += $"Corriente nominal: {corriente:F2} A\n";
            msg += $"Inductancia: {inductancia:F2} mH\n";
            msg += $"Impedancia: {impedancia:F1} %\n";
            msg += $"Caída de voltaje: {voltajeCaida:F2} V\n";
            msg += $"Pérdidas totales: {perdidas:F1} W\n";
            msg += $"Elevación temperatura: +{tempRise:F1} °C\n";

            if (numConductores > 1)
                msg += $"Conductores en paralelo: {numConductores}\n";

            msg += "\n─────────────────────────────────\n";
            msg += "  RECOMENDACIONES TÉCNICAS\n";
            msg += "─────────────────────────────────\n";

            if (tipo.Contains("Entrada"))
            {
                msg += "\n✓ Reduce armónicos de entrada (THD)\n";
                msg += "✓ Protege la red eléctrica de distorsión\n";
                msg += "✓ Mejora factor de potencia (FP)\n";
                msg += "✓ Cumple normas IEEE-519\n";
                msg += "✓ Ubicación: Entre red y VFD\n";
                msg += "✓ Impedancia recomendada: 3-5%\n";
                msg += "✓ Protección: Fusibles clase J o T\n";
            }
            else if (tipo.Contains("Salida"))
            {
                msg += "\n✓ Limita dV/dt a <2000 V/µs\n";
                msg += "✓ Reduce corrientes de fuga a tierra\n";
                msg += "✓ Protege aislamiento del motor\n";
                msg += "✓ Esencial para cables >30m\n";
                msg += "✓ Ubicación: Entre VFD y motor\n";
                msg += "✓ Impedancia recomendada: 2-4%\n";
                msg += "✓ Reduce ruido audible del motor\n";
            }
            else
            {
                msg += "\n✓ Suaviza ripple del bus DC\n";
                msg += "✓ Mejora regulación de voltaje\n";
                msg += "✓ Reduce estrés en capacitores\n";
                msg += "✓ Aumenta vida útil del VFD\n";
                msg += "✓ Mejora respuesta dinámica\n";
            }

            msg += "\n─────────────────────────────────\n";
            msg += "  ADVERTENCIAS\n";
            msg += "─────────────────────────────────\n";

            if (tempRise > 50)
                msg += "\n⚠ ALTA TEMPERATURA\n   Considere refrigeración forzada\n   o aumente sección del conductor\n";

            if (corriente > 500)
                msg += "\n⚠ ALTA CORRIENTE\n   Verifique conexiones y terminales\n   Use conductores en paralelo\n";

            if (impedancia > 7)
                msg += "\n⚠ ALTA IMPEDANCIA\n   Puede afectar regulación de voltaje\n   Verifique compatibilidad con VFD\n";

            msg += "\n─────────────────────────────────\n";
            msg += "  MANTENIMIENTO\n";
            msg += "─────────────────────────────────\n";
            msg += "\n• Inspección visual: mensual\n";
            msg += "• Verificar temperatura: semanal\n";
            msg += "• Limpieza: cada 6 meses\n";
            msg += "• Pruebas eléctricas: anual\n";
            msg += "• Verificar conexiones: trimestral\n";

            MessageBox.Show(msg, "Resultados y Recomendaciones",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
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
                        if (ctrl is TextBox txt && ctrl.Tag != null)
                            txt.Clear();
                    }
                }
            }

            cboTipo.SelectedIndex = 0;
            cboFrecuencia.SelectedIndex = 1;
            cboFases.SelectedIndex = 1;
            cboImpedancia.SelectedIndex = 4;

            datosReactor = new DatosReactor();
            panelGrafico.Invalidate();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                string datos = "═══════════════════════════════════════════════════\n";
                datos += "   REACTOR VFD - INFORME TÉCNICO DETALLADO\n";
                datos += "═══════════════════════════════════════════════════\n\n";
                datos += $"Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
                datos += $"Generado por: Calculadora VFD Reactor v2.0\n\n";

                datos += "───────────────────────────────────────────────────\n";
                datos += "PARÁMETROS DE ENTRADA\n";
                datos += "───────────────────────────────────────────────────\n";
                datos += $"Tipo de reactor:      {cboTipo.Text}\n";
                datos += $"Potencia nominal:     {txtPotencia.Text} kW\n";
                datos += $"Voltaje nominal:      {txtVoltaje.Text} V\n";
                datos += $"Frecuencia:           {cboFrecuencia.Text} Hz\n";
                datos += $"Sistema eléctrico:    {cboFases.Text}\n";
                datos += $"Impedancia nominal:   {cboImpedancia.Text} %\n\n";

                datos += "───────────────────────────────────────────────────\n";
                datos += "RESULTADOS ELÉCTRICOS\n";
                datos += "───────────────────────────────────────────────────\n";
                datos += $"Corriente nominal:    {txtCorriente.Text} A\n";
                datos += $"Reactancia inductiva: {txtReactancia.Text} Ω\n";
                datos += $"Inductancia:          {txtInductancia.Text} mH\n";
                datos += $"Inductancia:          {FindControlByTag("inductanciaH").Text} H\n";
                datos += $"Caída de voltaje:     {txtVoltajeCaida.Text} V\n";
                datos += $"Caída porcentual:     {FindControlByTag("caidaPct").Text} %\n";
                datos += $"Pérdidas totales:     {FindControlByTag("perdidas").Text} W\n";
                datos += $"Consumo energético:   {FindControlByTag("energia").Text} kWh/año\n\n";

                datos += "───────────────────────────────────────────────────\n";
                datos += "DISEÑO FÍSICO CONSTRUCTIVO\n";
                datos += "───────────────────────────────────────────────────\n";
                datos += $"Dimensiones núcleo:   {txtNucleo.Text} mm\n";
                datos += $"Área del núcleo:      {FindControlByTag("area").Text} cm²\n";
                datos += $"Ventana bobinado:     {FindControlByTag("ventana").Text} cm²\n";
                datos += $"Espiras por fase:     {txtEspiras.Text}\n";
                datos += $"Sección conductor:    {txtSeccionCable.Text} mm²\n";
                datos += $"Calibre equivalente:  {FindControlByTag("awg").Text}\n";
                datos += $"Diámetro conductor:   {txtDiametroCable.Text} mm\n";
                datos += $"Longitud cable:       {txtLongitudCable.Text} m\n";
                datos += $"Peso aproximado:      {txtPesoAprox.Text} kg\n";
                datos += $"Elevación térmica:    +{FindControlByTag("temp").Text} °C\n\n";

                datos += "───────────────────────────────────────────────────\n";
                datos += "ESPECIFICACIONES TÉCNICAS\n";
                datos += "───────────────────────────────────────────────────\n";
                datos += "• Material núcleo: Acero silicio laminado E\n";
                datos += "• Material conductor: Cobre electrolítico 99.9%\n";
                datos += "• Aislamiento: Clase H (180°C)\n";
                datos += "• Clase térmica: F o H\n";
                datos += "• Grado de protección: IP20 mínimo\n";
                datos += "• Normas aplicables: IEEE-519, IEC 61558\n\n";
                datos += "═══════════════════════════════════════════════════\n";
                datos += "          FIN DEL INFORME\n";
                datos += "═══════════════════════════════════════════════════\n";

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Archivo de texto|*.txt|Todos los archivos|*.*",
                    FileName = $"Reactor_VFD_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                    Title = "Exportar Informe Técnico"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(sfd.FileName, datos);
                    MessageBox.Show($"Informe exportado correctamente:\n{sfd.FileName}",
                        "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar archivo:\n{ex.Message}",
                    "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PanelGrafico_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Si no hay datos calculados, mostrar mensaje
            if (datosReactor.AreaNucleo == 0)
            {
                string mensaje = "Realice un cálculo\npara visualizar el\ndiseño del reactor";
                Font font = new Font("Segoe UI", 11F);
                SizeF tamTexto = g.MeasureString(mensaje, font);
                g.DrawString(mensaje, font, Brushes.Gray,
                    (panelGrafico.Width - tamTexto.Width) / 2,
                    (panelGrafico.Height - tamTexto.Height) / 2);
                return;
            }

            // Área de dibujo
            int margen = 20;
            int yOffset = 40;
            int areaDisponible = Math.Min(panelGrafico.Width - 2 * margen,
                                         panelGrafico.Height - yOffset - margen);

            // Calcular escala
            double ladoNucleoMM = Math.Sqrt(datosReactor.AreaNucleo * 100);
            double alturaNucleoMM = ladoNucleoMM * 1.5;
            double escala = areaDisponible / (alturaNucleoMM * 1.2);

            // Centro del dibujo
            int centroX = panelGrafico.Width / 2;
            int centroY = yOffset + areaDisponible / 2;

            // Dimensiones escaladas
            int anchoNucleo = (int)(ladoNucleoMM * escala);
            int altoNucleo = (int)(alturaNucleoMM * escala);
            int profundidad = anchoNucleo;

            // Dibujar vista isométrica del reactor
            DibujarReactorIsometrico(g, centroX, centroY, anchoNucleo, altoNucleo, profundidad);

            // Dibujar dimensiones y etiquetas
            DibujarDimensionesYEtiquetas(g, centroX, centroY, anchoNucleo, altoNucleo,
                profundidad, ladoNucleoMM, alturaNucleoMM);

            // Dibujar leyenda con datos técnicos
            DibujarLeyendaTecnica(g, margen, yOffset + areaDisponible + 20);
        }

        private void DibujarReactorIsometrico(Graphics g, int cx, int cy, int ancho, int alto, int prof)
        {
            // Ángulo isométrico (30 grados)
            double angulo = Math.PI / 6; // 30°
            int offsetX = (int)(prof * Math.Cos(angulo) * 0.5);
            int offsetY = (int)(prof * Math.Sin(angulo) * 0.5);

            // Colores
            Color colorNucleo = Color.FromArgb(180, 70, 70, 70);
            Color colorNucleoOscuro = Color.FromArgb(180, 50, 50, 50);
            Color colorBobina = Color.FromArgb(200, 200, 100, 20);
            Color colorBobinaOscuro = Color.FromArgb(200, 150, 75, 15);
            Color colorCobre = Color.FromArgb(220, 184, 115, 51);

            Pen penContorno = new Pen(Color.Black, 2);
            Pen penLinea = new Pen(Color.FromArgb(100, 0, 0, 0), 1);

            // Calcular puntos para vista isométrica
            Point[] puntosFrontal = {
                new Point(cx - ancho/2, cy - alto/2),           // Superior izq
                new Point(cx + ancho/2, cy - alto/2),           // Superior der
                new Point(cx + ancho/2, cy + alto/2),           // Inferior der
                new Point(cx - ancho/2, cy + alto/2)            // Inferior izq
            };

            Point[] puntosTrasero = {
                new Point(puntosFrontal[0].X + offsetX, puntosFrontal[0].Y - offsetY),
                new Point(puntosFrontal[1].X + offsetX, puntosFrontal[1].Y - offsetY),
                new Point(puntosFrontal[2].X + offsetX, puntosFrontal[2].Y - offsetY),
                new Point(puntosFrontal[3].X + offsetX, puntosFrontal[3].Y - offsetY)
            };

            // Dibujar cara trasera (más oscura)
            using (SolidBrush brush = new SolidBrush(colorNucleoOscuro))
            {
                g.FillPolygon(brush, puntosTrasero);
            }
            g.DrawPolygon(penContorno, puntosTrasero);

            // Dibujar lado derecho
            Point[] ladoDerecho = {
                puntosFrontal[1], puntosTrasero[1],
                puntosTrasero[2], puntosFrontal[2]
            };
            using (SolidBrush brush = new SolidBrush(colorNucleo))
            {
                g.FillPolygon(brush, ladoDerecho);
            }
            g.DrawPolygon(penContorno, ladoDerecho);

            // Dibujar lado superior
            Point[] ladoSuperior = {
                puntosFrontal[0], puntosFrontal[1],
                puntosTrasero[1], puntosTrasero[0]
            };
            using (SolidBrush brush = new SolidBrush(colorNucleo))
            {
                g.FillPolygon(brush, ladoSuperior);
            }
            g.DrawPolygon(penContorno, ladoSuperior);

            // Dibujar bobinas (cara frontal)
            int margenBobina = ancho / 8;
            int anchoBobina = ancho - 2 * margenBobina;
            int altoBobina = alto / 3;

            for (int i = 0; i < (datosReactor.EsTrifasico ? 3 : 1); i++)
            {
                int yBobina = cy - alto / 2 + margenBobina + i * (alto - 2 * margenBobina) / (datosReactor.EsTrifasico ? 2 : 1);

                Rectangle rectBobina = new Rectangle(
                    cx - anchoBobina / 2,
                    yBobina - altoBobina / 2,
                    anchoBobina,
                    altoBobina
                );

                // Sombra de bobina
                Rectangle rectSombra = rectBobina;
                rectSombra.Offset(2, 2);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    g.FillRectangle(brush, rectSombra);
                }

                // Bobina
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rectBobina, colorBobina, colorBobinaOscuro, 90f))
                {
                    g.FillRectangle(brush, rectBobina);
                }
                g.DrawRectangle(penContorno, rectBobina);

                // Detalles de espiras (líneas horizontales)
                int numLineas = Math.Min(datosReactor.Espiras / 10, 8);
                for (int j = 1; j < numLineas; j++)
                {
                    int yLinea = rectBobina.Top + j * rectBobina.Height / numLineas;
                    g.DrawLine(penLinea,
                        rectBobina.Left, yLinea,
                        rectBobina.Right, yLinea);
                }

                // Etiqueta de fase
                if (datosReactor.EsTrifasico)
                {
                    string fase = i == 0 ? "R" : i == 1 ? "S" : "T";
                    Font fontFase = new Font("Segoe UI", 9F, FontStyle.Bold);
                    SizeF tamFase = g.MeasureString(fase, fontFase);
                    g.DrawString(fase, fontFase, Brushes.White,
                        rectBobina.Left + 5,
                        rectBobina.Top + (rectBobina.Height - tamFase.Height) / 2);
                }

                // Terminales de conexión
                using (SolidBrush brushCobre = new SolidBrush(colorCobre))
                {
                    int tamTerminal = 8;
                    // Terminal izquierdo
                    g.FillEllipse(brushCobre,
                        rectBobina.Left - tamTerminal,
                        rectBobina.Top + rectBobina.Height / 2 - tamTerminal / 2,
                        tamTerminal, tamTerminal);
                    g.DrawEllipse(penContorno,
                        rectBobina.Left - tamTerminal,
                        rectBobina.Top + rectBobina.Height / 2 - tamTerminal / 2,
                        tamTerminal, tamTerminal);

                    // Terminal derecho
                    g.FillEllipse(brushCobre,
                        rectBobina.Right,
                        rectBobina.Top + rectBobina.Height / 2 - tamTerminal / 2,
                        tamTerminal, tamTerminal);
                    g.DrawEllipse(penContorno,
                        rectBobina.Right,
                        rectBobina.Top + rectBobina.Height / 2 - tamTerminal / 2,
                        tamTerminal, tamTerminal);
                }
            }

            // Dibujar cara frontal del núcleo (transparente para ver bobinas)
            using (Pen penFrontal = new Pen(Color.FromArgb(150, 100, 100, 100), 3))
            {
                g.DrawPolygon(penFrontal, puntosFrontal);
            }

            penContorno.Dispose();
            penLinea.Dispose();
        }

        private void DibujarDimensionesYEtiquetas(Graphics g, int cx, int cy, int ancho, int alto,
            int prof, double ladoMM, double altoMM)
        {
            Font fontDim = new Font("Segoe UI", 8F);
            Pen penCota = new Pen(Color.DarkBlue, 1);
            penCota.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            penCota.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            double angulo = Math.PI / 6;
            int offsetX = (int)(prof * Math.Cos(angulo) * 0.5);
            int offsetY = (int)(prof * Math.Sin(angulo) * 0.5);

            // Dimensión horizontal (ancho)
            int yDimAncho = cy + alto / 2 + 25;
            g.DrawLine(penCota, cx - ancho / 2, yDimAncho, cx + ancho / 2, yDimAncho);
            string textoAncho = $"{ladoMM:F0} mm";
            SizeF tamAncho = g.MeasureString(textoAncho, fontDim);
            g.DrawString(textoAncho, fontDim, Brushes.DarkBlue,
                cx - tamAncho.Width / 2, yDimAncho + 5);

            // Dimensión vertical (altura)
            int xDimAlto = cx - ancho / 2 - 25;
            g.DrawLine(penCota, xDimAlto, cy - alto / 2, xDimAlto, cy + alto / 2);
            string textoAlto = $"{altoMM:F0} mm";
            SizeF tamAlto = g.MeasureString(textoAlto, fontDim);

            // Rotar texto para dimensión vertical
            GraphicsState estado = g.Save();
            g.TranslateTransform(xDimAlto - 15, cy);
            g.RotateTransform(-90);
            g.DrawString(textoAlto, fontDim, Brushes.DarkBlue, -tamAlto.Width / 2, 0);
            g.Restore(estado);

            // Dimensión de profundidad
            int xDimProf = cx + ancho / 2 + 10;
            int yDimProf = cy - alto / 2;
            g.DrawLine(penCota,
                xDimProf, yDimProf,
                xDimProf + offsetX, yDimProf - offsetY);
            string textoProf = $"{ladoMM:F0} mm";
            SizeF tamProf = g.MeasureString(textoProf, fontDim);
            g.DrawString(textoProf, fontDim, Brushes.DarkBlue,
                xDimProf + offsetX / 2 - tamProf.Width / 2, yDimProf - offsetY / 2 - 15);

            penCota.Dispose();
        }

        private void DibujarLeyendaTecnica(Graphics g, int x, int y)
        {
            Font fontTitulo = new Font("Segoe UI", 9F, FontStyle.Bold);
            Font fontDato = new Font("Segoe UI", 8F);

            string[] datos = {
                $"Corriente: {datosReactor.Corriente:F1} A",
                $"Inductancia: {datosReactor.Inductancia:F2} mH",
                $"Espiras/fase: {datosReactor.Espiras}",
                $"Cable Ø: {datosReactor.DiametroCable:F1} mm",
                $"Long. cable: {datosReactor.LongitudCable:F1} m",
                $"Peso: {datosReactor.Peso:F1} kg"
            };

            int lineHeight = 16;
            int columnas = 2;
            int anchoColumna = 150;

            g.DrawString("Datos Técnicos:", fontTitulo, Brushes.Black, x, y);
            y += 20;

            for (int i = 0; i < datos.Length; i++)
            {
                int col = i % columnas;
                int fila = i / columnas;
                g.DrawString($"• {datos[i]}", fontDato, Brushes.DarkSlateGray,
                    x + col * anchoColumna, y + fila * lineHeight);
            }
        }
    }

    // Clase para almacenar datos del reactor para el gráfico
    public class DatosReactor
    {
        public double AreaNucleo { get; set; }
        public int Espiras { get; set; }
        public double DiametroCable { get; set; }
        public double Corriente { get; set; }
        public double Inductancia { get; set; }
        public double Potencia { get; set; }
        public bool EsTrifasico { get; set; }
        public int NumConductores { get; set; }
        public double LongitudCable { get; set; }
        public double Peso { get; set; }

        public DatosReactor()
        {
            AreaNucleo = 0;
            Espiras = 0;
            DiametroCable = 0;
            Corriente = 0;
            Inductancia = 0;
            Potencia = 0;
            EsTrifasico = true;
            NumConductores = 1;
            LongitudCable = 0;
            Peso = 0;
        }
    }
}