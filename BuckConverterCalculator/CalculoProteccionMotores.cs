using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class CalculoProteccionMotores : Form
    {
        private CultureInfo culturaLocal = new CultureInfo("es-AR");

        public CalculoProteccionMotores()
        {
            InitializeComponent();
            txtEficiencia.Text = "90";
            txtFactorPotencia.Text = "0,85";
            cmbTipoInstalacion.SelectedIndex = 0;
            cmbTipoArranque.SelectedIndex = 0;
            numMotores.Value = 1;
            numMotores.Enabled = false;

            // Configurar cultura para usar coma como separador decimal
            culturaLocal.NumberFormat.NumberDecimalSeparator = ",";
            culturaLocal.NumberFormat.NumberGroupSeparator = ".";
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                double potencia = ParsearDecimal(txtPotencia.Text);
                double tension = ParsearDecimal(txtTension.Text);
                double eficiencia = ParsearDecimal(txtEficiencia.Text) / 100;
                double fp = ParsearDecimal(txtFactorPotencia.Text);
                int tipoInstalacion = cmbTipoInstalacion.SelectedIndex;
                int tipoArranque = cmbTipoArranque.SelectedIndex; // 0=Directo, 1=Y-Δ, 2=Soft-Starter, 3=VFD
                int numeroMotores = (int)numMotores.Value;

                // Verificar que múltiples motores solo se permita con Soft-Starter o VFD
                if (numeroMotores > 1 && tipoArranque < 2)
                {
                    MessageBox.Show("Múltiples motores solo está disponible para Arranque Suave y VFD.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cálculo de corriente por motor
                double corrienteNominalMotor = (potencia * 1000) / (Math.Sqrt(3) * tension * eficiencia * fp);

                // Corriente total depende del número de motores
                double corrienteNominalTotal = corrienteNominalMotor * numeroMotores;

                // Potencia total
                double potenciaTotal = potencia * numeroMotores;

                // Corriente de arranque según tipo
                double corrienteArranqueMotor;
                double corrienteArranqueTotal;
                double factorArmonicosCable = 1.0;
                double factorArmonicosTermico = 1.0;

                switch (tipoArranque)
                {
                    case 0: // Directo
                        corrienteArranqueMotor = corrienteNominalMotor * 7;
                        corrienteArranqueTotal = corrienteArranqueMotor;
                        break;
                    case 1: // Estrella-Triángulo
                        corrienteArranqueMotor = corrienteNominalMotor * 2.5;
                        corrienteArranqueTotal = corrienteArranqueMotor;
                        break;
                    case 2: // Soft-Starter
                        corrienteArranqueMotor = corrienteNominalMotor * 3.5; // Típico 3-4×In
                        // En múltiples motores, arrancan secuencial
                        corrienteArranqueTotal = corrienteArranqueMotor + (corrienteNominalMotor * (numeroMotores - 1));
                        break;
                    case 3: // VFD
                        corrienteArranqueMotor = corrienteNominalMotor * 1.5;
                        corrienteArranqueTotal = corrienteArranqueMotor * numeroMotores; // Todos simultáneos
                        factorArmonicosCable = 1.2;
                        factorArmonicosTermico = 1.15;
                        break;
                    default:
                        corrienteArranqueMotor = corrienteNominalMotor * 7;
                        corrienteArranqueTotal = corrienteArranqueMotor;
                        break;
                }

                // PROTECCIONES
                var guardamotor = SeleccionarGuardamotorPreciso(corrienteNominalMotor * factorArmonicosTermico);
                var termomagnetico = SeleccionarTermomagneticoPreciso(corrienteNominalTotal, corrienteArranqueTotal, tipoArranque);
                var cable = DimensionarCable(corrienteNominalTotal * factorArmonicosCable, tipoInstalacion);

                // REACTOR (para múltiples motores con VFD o Soft-Starter)
                Reactor reactor = null;
                if (numeroMotores > 1 && (tipoArranque == 2 || tipoArranque == 3))
                {
                    reactor = CalcularReactor(potenciaTotal, corrienteNominalTotal, tension, tipoArranque);
                }

                // CONTACTORES/DISPOSITIVOS
                Contactor contactor = null;
                ContactorEstrella contactorLinea = null, contactorEstrella = null, contactorTriangulo = null;
                Rele releTermico = null;
                SoftStarter softStarter = null;

                if (tipoArranque == 0) // Directo
                {
                    contactor = SeleccionarContactor(corrienteNominalMotor, potencia);
                }
                else if (tipoArranque == 1) // Y-Δ
                {
                    var contactoresYD = SeleccionarContactoresEstrellaTriangulo(corrienteNominalMotor, potencia, tension);
                    contactorLinea = contactoresYD.Item1;
                    contactorEstrella = contactoresYD.Item2;
                    contactorTriangulo = contactoresYD.Item3;
                    releTermico = contactoresYD.Item4;
                }
                else if (tipoArranque == 2) // Soft-Starter
                {
                    softStarter = SeleccionarSoftStarter(potenciaTotal, corrienteNominalTotal, tension, numeroMotores);
                }

                // Construir salida
                string tipoArranqueTexto = tipoArranque == 0 ? "ARRANQUE DIRECTO" :
                                          tipoArranque == 1 ? "ARRANQUE ESTRELLA-TRIÁNGULO" :
                                          tipoArranque == 2 ? "ARRANQUE SUAVE (SOFT-STARTER)" :
                                          "VARIADOR DE FRECUENCIA";

                txtResultados.Text = $"CÁLCULO DE PROTECCIONES MOTOR TRIFÁSICO\r\n" +
                    $"═══════════════════════════════════════════════\r\n\r\n" +
                    $"CONFIGURACIÓN:\r\n" +
                    $"  Tipo de arranque: {tipoArranqueTexto}\r\n" +
                    $"  Número de motores: {numeroMotores}\r\n" +
                    (numeroMotores > 1 ? $"  Configuración: Múltiples motores en paralelo\r\n" : "") +
                    $"\r\nDATOS POR MOTOR:\r\n" +
                    $"  Potencia unitaria: {FormatearDecimal(potencia)} kW\r\n" +
                    $"  Tensión: {FormatearDecimal(tension)} V\r\n" +
                    $"  Eficiencia: {FormatearDecimal(eficiencia * 100)}%\r\n" +
                    $"  Factor de potencia: {FormatearDecimal(fp)}\r\n" +
                    (numeroMotores > 1 ? $"\r\nDATOS TOTALES DEL SISTEMA:\r\n" +
                    $"  Potencia total: {FormatearDecimal(potenciaTotal)} kW\r\n" +
                    $"  Corriente total nominal: {FormatearDecimal(corrienteNominalTotal)} A\r\n" : "") +
                    $"\r\nCORRIENTES CALCULADAS:\r\n" +
                    $"  Corriente nominal por motor (In): {FormatearDecimal(corrienteNominalMotor)} A\r\n" +
                    (numeroMotores > 1 ? $"  Corriente nominal total: {FormatearDecimal(corrienteNominalTotal)} A\r\n" : "") +
                    $"  Corriente arranque por motor: {FormatearDecimal(corrienteArranqueMotor)} A " +
                    (tipoArranque == 0 ? "(~7×In directo)" :
                     tipoArranque == 1 ? "(~2,5×In en Y)" :
                     tipoArranque == 2 ? "(~3,5×In controlado)" :
                     "(~1,5×In limitado)") + "\r\n" +
                    (numeroMotores > 1 ? $"  Corriente arranque total: {FormatearDecimal(corrienteArranqueTotal)} A\r\n" : "") +
                    (tipoArranque == 1 ? $"  Corriente en estrella (Y): {FormatearDecimal(corrienteNominalMotor / Math.Sqrt(3))} A\r\n" : "") +
                    (tipoArranque == 1 ? $"  Corriente en triángulo (Δ): {FormatearDecimal(corrienteNominalMotor)} A\r\n" : "") +
                    $"  Corriente de diseño (Ib): {FormatearDecimal(corrienteNominalTotal)} A\r\n" +
                    $"  Corriente admisible cable (Iz): ≥ {FormatearDecimal(corrienteNominalTotal * 1.25 * factorArmonicosCable)} A\r\n" +
                    (tipoArranque == 3 ? $"  Factor armónicos (cable): {FormatearDecimal(factorArmonicosCable)}\r\n" : "") +
                    (tipoArranque == 3 ? $"  Factor armónicos (térmico): {FormatearDecimal(factorArmonicosTermico)}\r\n" : "");

                // GUARDAMOTOR
                txtResultados.Text +=
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"PROTECCIÓN TÉRMICA - GUARDAMOTOR\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    (numeroMotores > 1 ? $"  Cantidad requerida: {numeroMotores} (uno por motor)\r\n" : "") +
                    $"  Rango de ajuste: {FormatearDecimal(guardamotor.RangoMin)}-{FormatearDecimal(guardamotor.RangoMax)} A\r\n" +
                    $"  Ajuste recomendado: {FormatearDecimal(guardamotor.Ajuste)} A\r\n" +
                    $"  Clase de disparo: Clase 10A\r\n" +
                    $"  Código comercial: GV2ME{guardamotor.Codigo} / MS132-{FormatearDecimal(guardamotor.RangoMax)}\r\n" +
                    (tipoArranque == 1 ? $"  Nota: Para Y-Δ puede usarse relé térmico en su lugar\r\n" : "") +
                    (tipoArranque == 2 ? $"  Nota: Soft-Starter incluye protección térmica, guardamotor opcional\r\n" : "") +
                    (tipoArranque == 3 ? $"  Nota: VFD incluye protección térmica, guardamotor opcional\r\n" : "");

                // TERMOMAGNÉTICO
                txtResultados.Text +=
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"PROTECCIÓN CORTOCIRCUITO - TERMOMAGNÉTICO\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    $"  Corriente nominal: {termomagnetico.Nominal} A\r\n" +
                    $"  Curva característica: {termomagnetico.Curva}\r\n" +
                    $"  Capacidad de ruptura: {termomagnetico.PdC} kA\r\n" +
                    $"  Umbral magnético: {termomagnetico.UmbralMag} A ({termomagnetico.FactorMag}×In)\r\n" +
                    $"  Código comercial: {termomagnetico.Codigo}\r\n" +
                    $"  Norma: IEC 60898-2 / IEC 60947-2\r\n" +
                    (numeroMotores > 1 ? $"  Dimensionado para: {numeroMotores} motores en paralelo\r\n" : "");

                // CABLE
                txtResultados.Text +=
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"CONDUCTOR DE ALIMENTACIÓN\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    $"  Sección nominal: {FormatearDecimal(cable.Seccion)} mm²\r\n" +
                    $"  Capacidad de corriente (Iz): {cable.Capacidad} A\r\n" +
                    $"  Tipo instalación: {cable.TipoInstalacion}\r\n" +
                    $"  Material: Cobre\r\n" +
                    $"  Aislación: PVC 90°C / XLPE 90°C\r\n" +
                    $"  Caída de tensión estimada: {FormatearDecimal(cable.CaidaTension)}%\r\n" +
                    $"  Factor de corrección: {cable.FactorCorreccion}\r\n" +
                    (tipoArranque == 3 ? $"  Nota: Cable apantallado recomendado para VFD\r\n" : "") +
                    (numeroMotores > 1 ? $"  Cable común hasta derivación a motores individuales\r\n" : "");

                // REACTOR (si aplica)
                if (reactor != null)
                {
                    txtResultados.Text +=
                        $"\r\n══════════════════════════════════════════════\r\n" +
                        $"REACTOR DE SALIDA (MÚLTIPLES MOTORES)\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Función: {reactor.Funcion}\r\n" +
                        $"  Potencia nominal: {FormatearDecimal(reactor.Potencia)} kVA\r\n" +
                        $"  Corriente nominal: {FormatearDecimal(reactor.Corriente)} A\r\n" +
                        $"  Inductancia (por fase): {FormatearDecimal(reactor.Inductancia)} mH\r\n" +
                        $"  Caída de tensión: {FormatearDecimal(reactor.CaidaTension)}% @ 50Hz\r\n" +
                        $"  Impedancia: {FormatearDecimal(reactor.Impedancia)}%\r\n" +
                        $"  Código ejemplo: {reactor.Codigo}\r\n" +
                        $"  Instalación: {reactor.Ubicacion}\r\n\r\n" +
                        $"  BENEFICIOS:\r\n" +
                        $"  • Reduce dv/dt en cables a motores\r\n" +
                        $"  • Protege bobinados contra picos de tensión\r\n" +
                        $"  • Permite cables más largos (hasta 300m)\r\n" +
                        $"  • Reduce corrientes de modo común\r\n" +
                        $"  • Mejora compatibilidad EMC\r\n";
                }

                // DISPOSITIVO PRINCIPAL (Contactor, Y-Δ, Soft-Starter o VFD)
                txtResultados.Text += $"\r\n══════════════════════════════════════════════\r\n";

                if (tipoArranque == 0) // Directo
                {
                    txtResultados.Text +=
                        $"CONTACTOR\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Categoría: AC-3 (motores)\r\n" +
                        $"  Corriente nominal (AC-3): {contactor.Ie} A\r\n" +
                        $"  Potencia asignada 400V: {FormatearDecimal(contactor.Potencia)} kW\r\n" +
                        $"  Durabilidad eléctrica: ~1M operaciones\r\n" +
                        $"  Código comercial: {contactor.Codigo}\r\n";
                }
                else if (tipoArranque == 1) // Y-Δ
                {
                    txtResultados.Text +=
                        $"SISTEMA DE CONTACTORES ESTRELLA-TRIÁNGULO\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"CONTACTOR DE LÍNEA (KM1):\r\n" +
                        $"  Corriente nominal: {contactorLinea.Ie} A\r\n" +
                        $"  Potencia (Δ): {FormatearDecimal(contactorLinea.PotenciaTriangulo)} kW @ 400V\r\n" +
                        $"  Código: {contactorLinea.Codigo}\r\n\r\n" +
                        $"CONTACTOR ESTRELLA (KM2):\r\n" +
                        $"  Corriente nominal: {contactorEstrella.Ie} A\r\n" +
                        $"  Potencia (Y): {FormatearDecimal(contactorEstrella.PotenciaEstrella)} kW @ 400V\r\n" +
                        $"  Código: {contactorEstrella.Codigo}\r\n\r\n" +
                        $"CONTACTOR TRIÁNGULO (KM3):\r\n" +
                        $"  Corriente nominal: {contactorTriangulo.Ie} A\r\n" +
                        $"  Potencia (Δ): {FormatearDecimal(contactorTriangulo.PotenciaTriangulo)} kW @ 400V\r\n" +
                        $"  Código: {contactorTriangulo.Codigo}\r\n\r\n" +
                        $"RELÉ TÉRMICO:\r\n" +
                        $"  Rango: {FormatearDecimal(releTermico.RangoMin)}-{FormatearDecimal(releTermico.RangoMax)} A\r\n" +
                        $"  Ajuste: {FormatearDecimal(releTermico.Ajuste)} A\r\n" +
                        $"  Código: {releTermico.Codigo}\r\n\r\n" +
                        $"TEMPORIZADOR: RE7TA11MW (Schneider)\r\n" +
                        $"  Tiempo Y: 3-8 seg, Retardo Y→Δ: 50-100ms\r\n";
                }
                else if (tipoArranque == 2) // Soft-Starter
                {
                    txtResultados.Text +=
                        $"ARRANCADOR SUAVE (SOFT-STARTER)\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Potencia nominal: {FormatearDecimal(softStarter.Potencia)} kW\r\n" +
                        $"  Corriente nominal: {FormatearDecimal(softStarter.Corriente)} A\r\n" +
                        $"  Tensión: {FormatearDecimal(softStarter.Tension)} V\r\n" +
                        $"  Categoría: AC-53a (arranque suave)\r\n" +
                        $"  Código comercial: {softStarter.Codigo}\r\n\r\n" +
                        (numeroMotores > 1 ?
                        $"  CONFIGURACIÓN MÚLTIPLES MOTORES:\r\n" +
                        $"  • Número de motores: {numeroMotores}\r\n" +
                        $"  • Modo: Arranque secuencial programable\r\n" +
                        $"  • Bypass: Contactor de bypass incluido\r\n" +
                        $"  • Reactor salida: {(reactor != null ? "SÍ - Requerido" : "No requerido")}\r\n\r\n" : "") +
                        $"  PARÁMETROS CONFIGURABLES:\r\n" +
                        $"  • Tensión inicial: 30-50% Un\r\n" +
                        $"  • Rampa tensión: 3-20 segundos\r\n" +
                        $"  • Límite corriente: {FormatearDecimal(softStarter.LimiteCorriente)} A (ajustable 200-400%)\r\n" +
                        $"  • Kick-start: 0-50% (torque inicial)\r\n" +
                        $"  • Parada suave: 1-30 segundos\r\n\r\n" +
                        $"  PROTECCIONES INTEGRADAS:\r\n" +
                        $"  ✓ Sobrecarga térmica electrónica (Clase 10/20)\r\n" +
                        $"  ✓ Asimetría de fases (desbalance)\r\n" +
                        $"  ✓ Pérdida de fase\r\n" +
                        $"  ✓ Subvoltaje/Sobrevoltaje\r\n" +
                        $"  ✓ Sobretemperatura tiristores\r\n" +
                        $"  ✓ Tiempo de arranque excedido\r\n" +
                        $"  ✓ Cortocircuito (con fusibles externos)\r\n\r\n" +
                        $"  TECNOLOGÍA:\r\n" +
                        $"  • Control por tiristores SCR antiparalelo\r\n" +
                        $"  • Regulación por ángulo de disparo\r\n" +
                        $"  • Bypass con contactor integrado\r\n" +
                        $"  • Disipación: {FormatearDecimal(softStarter.Disipacion)} W @ In\r\n";
                }
                else if (tipoArranque == 3) // VFD
                {
                    txtResultados.Text +=
                        $"VARIADOR DE FRECUENCIA\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Potencia nominal: {ObtenerPotenciaVFD(potenciaTotal)} kW\r\n" +
                        $"  Corriente nominal: {ObtenerCorrienteVFD(potenciaTotal)} A\r\n" +
                        $"  Sobrecarga: 150% por 60s (típico)\r\n" +
                        $"  Código ejemplo: ATV320U{ObtenerCodigoVFD(potenciaTotal)} (Schneider)\r\n" +
                        $"                  PowerFlex 525-{ObtenerCodigoPF525(potenciaTotal)} (Rockwell)\r\n" +
                        (numeroMotores > 1 ?
                        $"\r\n  CONFIGURACIÓN MÚLTIPLES MOTORES:\r\n" +
                        $"  • Número de motores: {numeroMotores}\r\n" +
                        $"  • Potencia individual: {FormatearDecimal(potencia)} kW cada uno\r\n" +
                        $"  • Reactor salida: {(reactor != null ? "OBLIGATORIO" : "Recomendado")}\r\n" +
                        $"  • Control: V/f estándar (misma velocidad todos)\r\n" +
                        $"  • Limitación: Motores idénticos recomendado\r\n\r\n" : "\r\n") +
                        $"  Filtro EMI/RFI: Clase C2/C3 recomendado\r\n" +
                        $"  Protecciones integradas: Completas\r\n";
                }

                // VERIFICACIONES
                txtResultados.Text +=
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"VERIFICACIONES\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    VerificarCoordinacion(corrienteNominalTotal, guardamotor, termomagnetico, cable, tipoArranque, numeroMotores);

                // ESQUEMAS DE CONEXIÓN
                txtResultados.Text +=
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"ESQUEMA DE CONEXIÓN\r\n" +
                    $"══════════════════════════════════════════════\r\n";

                GenerarEsquemaConexion(tipoArranque, numeroMotores, termomagnetico, contactor,
                    contactorLinea, contactorEstrella, contactorTriangulo, guardamotor,
                    softStarter, reactor, potencia, corrienteNominalMotor, corrienteArranqueMotor,
                    corrienteNominalTotal, potenciaTotal, tension, releTermico);

                // NOTAS ADICIONALES
                if (numeroMotores > 1)
                {
                    txtResultados.Text +=
                        $"\r\n══════════════════════════════════════════════\r\n" +
                        $"NOTAS IMPORTANTES - MÚLTIPLES MOTORES\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"⚠ Motores deben ser idénticos (misma potencia y características)\r\n" +
                        $"⚠ Reactor de salida {(reactor != null ? "INCLUIDO" : "altamente recomendado")} para:\r\n" +
                        $"  • Reducir dv/dt y proteger bobinados\r\n" +
                        $"  • Permitir distancias mayores a motores\r\n" +
                        $"  • Balancear corrientes entre motores\r\n" +
                        (tipoArranque == 2 ?
                        $"⚠ Soft-Starter: Arranque secuencial recomendado\r\n" +
                        $"⚠ Ajustar retardos entre arranques (2-5 seg típico)\r\n" : "") +
                        (tipoArranque == 3 ?
                        $"⚠ VFD: Todos los motores giran a la misma velocidad\r\n" +
                        $"⚠ No apto si motores requieren velocidades independientes\r\n" +
                        $"⚠ Considerar un VFD por motor para control individual\r\n" : "") +
                        $"✓ Protección térmica individual por motor recomendada\r\n" +
                        $"✓ Cable desde dispositivo hasta cada motor debe ser balanceado\r\n";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el cálculo: {ex.Message}\r\n\r\n" +
                    "Verifique que todos los campos contengan valores numéricos válidos.\r\n" +
                    "Use coma (,) como separador decimal.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarEsquemaConexion(int tipoArranque, int numeroMotores, Termomagnetico tm,
            Contactor contactor, ContactorEstrella km1, ContactorEstrella km2, ContactorEstrella km3,
            Guardamotor gm, SoftStarter ss, Reactor reactor, double potencia, double inMotor,
            double iaMotor, double inTotal, double potTotal, double tension, Rele releTerm)
        {
            if (tipoArranque == 0) // Directo
            {
                txtResultados.Text += GenerarEsquemaDOL(tm, contactor, gm, potencia, inMotor, iaMotor);
            }
            else if (tipoArranque == 1) // Y-Δ
            {
                txtResultados.Text += GenerarEsquemaYD(tm, km1, km2, km3, releTerm, potencia, inMotor, iaMotor);
            }
            else if (tipoArranque == 2) // Soft-Starter
            {
                txtResultados.Text += GenerarEsquemaSoftStarter(tm, ss, gm, reactor, numeroMotores,
                    potencia, inMotor, iaMotor, potTotal, inTotal, tension);
            }
            else if (tipoArranque == 3) // VFD
            {
                txtResultados.Text += GenerarEsquemaVFD(tm, gm, reactor, numeroMotores, potencia,
                    inMotor, potTotal, inTotal, tension);
            }
        }

        private string GenerarEsquemaDOL(Termomagnetico tm, Contactor cont, Guardamotor gm,
            double pot, double inMot, double iaMot)
        {
            return $"ARRANQUE DIRECTO (DOL - Direct On Line)\r\n\r\n" +
                $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                $"                         |\r\n" +
                $"              ┌──────────┴──────────┐\r\n" +
                $"              │  TERMOMAGNÉTICO     │\r\n" +
                $"              │  {tm.Nominal}A Curva D        │\r\n" +
                $"              └──────────┬──────────┘\r\n" +
                $"                         |\r\n" +
                $"              ┌──────────┴──────────┐\r\n" +
                $"              │   CONTACTOR KM      │\r\n" +
                $"              │   {cont.Ie}A AC-3         │\r\n" +
                $"              └──────────┬──────────┘\r\n" +
                $"                         |\r\n" +
                $"              ┌──────────┴──────────┐\r\n" +
                $"              │   GUARDAMOTOR       │\r\n" +
                $"              │   {FormatearDecimal(gm.RangoMin)}-{FormatearDecimal(gm.RangoMax)}A         │\r\n" +
                $"              └──────────┬──────────┘\r\n" +
                $"                         |\r\n" +
                $"        ┌────────────────┼────────────────┐\r\n" +
                $"       U/L1             V/L2             W/L3\r\n" +
                $"        │                │                │\r\n" +
                $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                $"    │    MOTOR {FormatearDecimal(pot)} kW / In={FormatearDecimal(inMot)} A    │\r\n" +
                $"    │    Ia ≈ {FormatearDecimal(iaMot)} A (arranque)      │\r\n" +
                $"    └────────────────────────────────────────┘\r\n";
        }

        private string GenerarEsquemaYD(Termomagnetico tm, ContactorEstrella km1,
            ContactorEstrella km2, ContactorEstrella km3, Rele rt, double pot, double inMot, double iaMot)
        {
            return $"ESTRELLA-TRIÁNGULO (detallado en documento previo)\r\n" +
                $"KM1={km1.Codigo} KM2={km2.Codigo} KM3={km3.Codigo} RT={rt.Codigo}\r\n";
        }

        private string GenerarEsquemaSoftStarter(Termomagnetico tm, SoftStarter ss, Guardamotor gm,
            Reactor reactor, int numMot, double potMot, double inMot, double iaMot, double potTot,
            double inTot, double tension)
        {
            string esquema = $"ARRANQUE SUAVE (SOFT-STARTER)\r\n\r\n";

            if (numMot == 1)
            {
                esquema +=
                    $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                    $"                         |\r\n" +
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  TERMOMAGNÉTICO     │\r\n" +
                    $"              │  {tm.Nominal}A Curva C/D     │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" +
                    $"    ╔════════════════════╧════════════════════╗\r\n" +
                    $"    ║    SOFT-STARTER (ARRANCADOR SUAVE)     ║\r\n" +
                    $"    ║    {FormatearDecimal(ss.Potencia)} kW / {FormatearDecimal(ss.Corriente)} A                  ║\r\n" +
                    $"    ║    {ss.Codigo}              ║\r\n" +
                    $"    ║                                        ║\r\n" +
                    $"    ║  ┌──────────────────────────────────┐ ║\r\n" +
                    $"    ║  │  L1 ──[SCR]──┬──[SCR]── T1      │ ║\r\n" +
                    $"    ║  │  L2 ──[SCR]──┼──[SCR]── T2      │ ║\r\n" +
                    $"    ║  │  L3 ──[SCR]──┴──[SCR]── T3      │ ║\r\n" +
                    $"    ║  │                                  │ ║\r\n" +
                    $"    ║  │  CONTROL TIRISTORES:             │ ║\r\n" +
                    $"    ║  │  • Ángulo disparo variable       │ ║\r\n" +
                    $"    ║  │  • Rampa tensión: 30%→100%       │ ║\r\n" +
                    $"    ║  │  • Tiempo: 3-20 seg              │ ║\r\n" +
                    $"    ║  └──────────────────────────────────┘ ║\r\n" +
                    $"    ║         ↓ (Bypass interno)            ║\r\n" +
                    $"    ║  ┌──────────────────────────────────┐ ║\r\n" +
                    $"    ║  │  CONTACTOR BYPASS (opcional)     │ ║\r\n" +
                    $"    ║  │  Cierra tras finalizar arranque  │ ║\r\n" +
                    $"    ║  │  Reduce pérdidas en régimen      │ ║\r\n" +
                    $"    ║  └──────────────────────────────────┘ ║\r\n" +
                    $"    ╚════════════════════╤════════════════════╝\r\n" +
                    $"                         |\r\n" +
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │   GUARDAMOTOR (opc) │\r\n" +
                    $"              │   {FormatearDecimal(gm.RangoMin)}-{FormatearDecimal(gm.RangoMax)}A         │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" +
                    $"        ┌────────────────┼────────────────┐\r\n" +
                    $"       U/T1             V/T2             W/T3\r\n" +
                    $"        │                │                │\r\n" +
                    $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                    $"    │    MOTOR {FormatearDecimal(potMot)} kW / In={FormatearDecimal(inMot)} A    │\r\n" +
                    $"    │    Ia ≈ {FormatearDecimal(iaMot)} A (controlado)   │\r\n" +
                    $"    └────────────────────────────────────────┘\r\n\r\n" +
                    $"SECUENCIA DE ARRANQUE:\r\n" +
                    $"  1. START → Tensión inicial 30-50%\r\n" +
                    $"  2. Rampa suave 3-20 seg hasta 100%\r\n" +
                    $"  3. Corriente limitada ~{FormatearDecimal(iaMot)}A (3-4×In)\r\n" +
                    $"  4. Bypass cierra (opcional) → Tiristores apagan\r\n" +
                    $"  5. Régimen normal con corriente {FormatearDecimal(inMot)}A\r\n";
            }
            else // Múltiples motores
            {
                esquema +=
                    $"CONFIGURACIÓN MÚLTIPLES MOTORES ({numMot} motores)\r\n\r\n" +
                    $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                    $"                         |\r\n" +
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  TERMOMAGNÉTICO     │\r\n" +
                    $"              │  {tm.Nominal}A Curva C/D     │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" +
                    $"    ╔════════════════════╧════════════════════╗\r\n" +
                    $"    ║    SOFT-STARTER {FormatearDecimal(ss.Potencia)} kW / {FormatearDecimal(ss.Corriente)} A     ║\r\n" +
                    $"    ║    Arranque secuencial programable     ║\r\n" +
                    $"    ╚════════════════════╤════════════════════╝\r\n" +
                    $"                         |\r\n" +
                    (reactor != null ?
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  REACTOR SALIDA     │\r\n" +
                    $"              │  {FormatearDecimal(reactor.Inductancia)} mH / {FormatearDecimal(reactor.Potencia)} kVA    │\r\n" +
                    $"              │  (Protección motor) │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" : "") +
                    $"        ┌────────────────┼────────────────┬──────────────\r\n" +
                    $"        │                │                │\r\n" +
                    $"    [MOTOR 1]        [MOTOR 2]        [MOTOR {numMot}]\r\n" +
                    $"    {FormatearDecimal(potMot)} kW           {FormatearDecimal(potMot)} kW           {FormatearDecimal(potMot)} kW\r\n" +
                    $"    {FormatearDecimal(inMot)} A            {FormatearDecimal(inMot)} A            {FormatearDecimal(inMot)} A\r\n\r\n" +
                    $"POTENCIA TOTAL: {FormatearDecimal(potTot)} kW\r\n" +
                    $"CORRIENTE TOTAL: {FormatearDecimal(inTot)} A\r\n\r\n" +
                    $"ARRANQUE SECUENCIAL:\r\n" +
                    $"  • Motor 1: t=0s → Rampa suave\r\n" +
                    $"  • Motor 2: t=3s → Rampa suave (Motor 1 ya estable)\r\n" +
                    $"  • Motor {numMot}: t={3 * (numMot - 1)}s → Rampa suave\r\n" +
                    $"  • Evita pico simultáneo de corriente\r\n" +
                    $"  • Corriente pico total: ~{FormatearDecimal(iaMot + inMot * (numMot - 1))}A\r\n";
            }

            return esquema;
        }

        private string GenerarEsquemaVFD(Termomagnetico tm, Guardamotor gm, Reactor reactor,
            int numMot, double potMot, double inMot, double potTot, double inTot, double tension)
        {
            string esquema = $"VARIADOR DE FRECUENCIA (VFD)\r\n\r\n";

            if (numMot == 1)
            {
                esquema +=
                    $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                    $"                         |\r\n" +
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  TERMOMAGNÉTICO     │\r\n" +
                    $"              │  {tm.Nominal}A Curva C        │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" +
                    $"    ╔════════════════════╧════════════════════╗\r\n" +
                    $"    ║    VARIADOR DE FRECUENCIA (VFD)        ║\r\n" +
                    $"    ║    {ObtenerPotenciaVFD(potTot)} kW / {ObtenerCorrienteVFD(potTot)} A                       ║\r\n" +
                    $"    ║                                        ║\r\n" +
                    $"    ║  [Rectificador] → [DC Bus] → [Inversor PWM] ║\r\n" +
                    $"    ║  Control V/f - Frecuencia 0-50Hz       ║\r\n" +
                    $"    ╚════════════════════╤════════════════════╝\r\n" +
                    $"                         |\r\n" +
                    $"        ┌────────────────┼────────────────┐\r\n" +
                    $"       U/T1             V/T2             W/T3\r\n" +
                    $"        │ (Cable apantallado)            │\r\n" +
                    $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                    $"    │    MOTOR {FormatearDecimal(potMot)} kW / In={FormatearDecimal(inMot)} A    │\r\n" +
                    $"    │    Control velocidad 0-100%            │\r\n" +
                    $"    └────────────────────────────────────────┘\r\n";
            }
            else // Múltiples motores
            {
                esquema +=
                    $"CONFIGURACIÓN MÚLTIPLES MOTORES ({numMot} motores)\r\n\r\n" +
                    $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                    $"                         |\r\n" +
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  TERMOMAGNÉTICO     │\r\n" +
                    $"              │  {tm.Nominal}A Curva C        │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" +
                    $"    ╔════════════════════╧════════════════════╗\r\n" +
                    $"    ║    VFD {ObtenerPotenciaVFD(potTot)} kW / {ObtenerCorrienteVFD(potTot)} A              ║\r\n" +
                    $"    ║    Salida común para {numMot} motores          ║\r\n" +
                    $"    ╚════════════════════╤════════════════════╝\r\n" +
                    $"                         |\r\n" +
                    (reactor != null ?
                    $"              ┌──────────┴──────────┐\r\n" +
                    $"              │  REACTOR SALIDA     │\r\n" +
                    $"              │  {FormatearDecimal(reactor.Inductancia)} mH / {FormatearDecimal(reactor.Potencia)} kVA    │\r\n" +
                    $"              │  OBLIGATORIO        │\r\n" +
                    $"              └──────────┬──────────┘\r\n" +
                    $"                         |\r\n" : "") +
                    $"        ┌────────────────┼────────────────┬──────────────\r\n" +
                    $"        │                │                │\r\n" +
                    $"    ┌───┴───┐        ┌───┴───┐        ┌───┴───┐\r\n" +
                    $"    │ GM(1) │        │ GM(2) │   ...  │ GM({numMot}) │\r\n" +
                    $"    └───┬───┘        └───┬───┘        └───┬───┘\r\n" +
                    $"        │                │                │\r\n" +
                    $"    [MOTOR 1]        [MOTOR 2]        [MOTOR {numMot}]\r\n" +
                    $"    {FormatearDecimal(potMot)} kW           {FormatearDecimal(potMot)} kW           {FormatearDecimal(potMot)} kW\r\n" +
                    $"    {FormatearDecimal(inMot)} A            {FormatearDecimal(inMot)} A            {FormatearDecimal(inMot)} A\r\n\r\n" +
                    $"POTENCIA TOTAL: {FormatearDecimal(potTot)} kW\r\n" +
                    $"CORRIENTE TOTAL: {FormatearDecimal(inTot)} A\r\n\r\n" +
                    $"⚠ IMPORTANTE:\r\n" +
                    $"  • Todos los motores giran a la MISMA velocidad\r\n" +
                    $"  • Motores deben ser idénticos (mismo kW y rpm)\r\n" +
                    $"  • Reactor de salida OBLIGATORIO\r\n" +
                    $"  • Cables balanceados desde reactor a cada motor\r\n" +
                    $"  • Guardamotor individual por motor recomendado\r\n" +
                    $"  • Alternativa: 1 VFD por motor para control independiente\r\n";
            }

            return esquema;
        }

        // CONTINUACIÓN DE CalculoProteccionMotores_V3.cs - PARTE 2
        // Copiar este contenido al final del archivo anterior, antes del último }

        private double ParsearDecimal(string texto)
        {
            texto = texto.Replace(".", ",");
            return double.Parse(texto, culturaLocal);
        }

        private string FormatearDecimal(double valor)
        {
            return valor.ToString("0.##", culturaLocal);
        }

        // CLASES DE DATOS
        private class Guardamotor
        {
            public double RangoMin { get; set; }
            public double RangoMax { get; set; }
            public double Ajuste { get; set; }
            public string Codigo { get; set; }
        }

        private class Termomagnetico
        {
            public int Nominal { get; set; }
            public string Curva { get; set; }
            public int PdC { get; set; }
            public int UmbralMag { get; set; }
            public string FactorMag { get; set; }
            public string Codigo { get; set; }
        }

        private class Cable
        {
            public double Seccion { get; set; }
            public int Capacidad { get; set; }
            public string TipoInstalacion { get; set; }
            public double CaidaTension { get; set; }
            public string FactorCorreccion { get; set; }
        }

        private class Contactor
        {
            public int Ie { get; set; }
            public double Potencia { get; set; }
            public string Codigo { get; set; }
        }

        private class ContactorEstrella
        {
            public int Ie { get; set; }
            public double PotenciaEstrella { get; set; }
            public double PotenciaTriangulo { get; set; }
            public string Codigo { get; set; }
        }

        private class Rele
        {
            public double RangoMin { get; set; }
            public double RangoMax { get; set; }
            public double Ajuste { get; set; }
            public string Codigo { get; set; }
        }

        private class SoftStarter
        {
            public double Potencia { get; set; }
            public double Corriente { get; set; }
            public double Tension { get; set; }
            public double LimiteCorriente { get; set; }
            public double Disipacion { get; set; }
            public string Codigo { get; set; }
        }

        private class Reactor
        {
            public double Potencia { get; set; } // kVA
            public double Corriente { get; set; } // A
            public double Inductancia { get; set; } // mH
            public double CaidaTension { get; set; } // %
            public double Impedancia { get; set; } // %
            public string Codigo { get; set; }
            public string Funcion { get; set; }
            public string Ubicacion { get; set; }
        }

        // MÉTODOS DE SELECCIÓN
        private Guardamotor SeleccionarGuardamotorPreciso(double In)
        {
            var rangos = new[] {
                (0.1, 0.16, "01"), (0.16, 0.25, "02"), (0.25, 0.4, "03"),
                (0.4, 0.63, "04"), (0.63, 1.0, "05"), (1.0, 1.6, "06"),
                (1.6, 2.5, "07"), (2.5, 4.0, "08"), (4.0, 6.3, "10"),
                (6.3, 10.0, "14"), (9.0, 14.0, "16"), (13.0, 18.0, "20"),
                (17.0, 23.0, "21"), (23.0, 32.0, "22"), (30.0, 40.0, "25"),
                (37.0, 50.0, "32"), (48.0, 65.0, "35"), (63.0, 80.0, "38")
            };

            foreach (var (min, max, cod) in rangos)
            {
                if (In >= min && In <= max)
                {
                    return new Guardamotor
                    {
                        RangoMin = min,
                        RangoMax = max,
                        Ajuste = In,
                        Codigo = cod
                    };
                }
            }

            return new Guardamotor { RangoMin = 63, RangoMax = 80, Ajuste = In, Codigo = "38" };
        }

        private Termomagnetico SeleccionarTermomagneticoPreciso(double In, double Ia, int tipoArranque)
        {
            int[] nominales = { 1, 2, 3, 4, 6, 10, 13, 16, 20, 25, 32, 40, 50, 63, 80, 100, 125, 160, 200, 250 };
            double InRequerido = In * 1.25;
            int nominalSeleccionado = nominales[nominales.Length - 1];

            foreach (int nom in nominales)
            {
                if (nom >= InRequerido)
                {
                    nominalSeleccionado = nom;
                    break;
                }
            }

            // Soft-Starter y VFD pueden usar Curva C, el resto Curva D
            if (tipoArranque == 2 || tipoArranque == 3) // Soft-Starter o VFD
            {
                int umbralMag = nominalSeleccionado * 7;
                int pdc = nominalSeleccionado <= 25 ? 10 : (nominalSeleccionado <= 63 ? 15 : 25);

                return new Termomagnetico
                {
                    Nominal = nominalSeleccionado,
                    Curva = "C (arranque suave)",
                    PdC = pdc,
                    UmbralMag = umbralMag,
                    FactorMag = "5-10",
                    Codigo = $"iC60N-C{nominalSeleccionado} / C60N-C{nominalSeleccionado}"
                };
            }
            else // Directo o Y-Δ - Curva D
            {
                while (nominalSeleccionado * 12 <= Ia * 1.1)
                {
                    int indice = Array.IndexOf(nominales, nominalSeleccionado);
                    if (indice < nominales.Length - 1)
                    {
                        nominalSeleccionado = nominales[indice + 1];
                    }
                    else
                    {
                        break;
                    }
                }

                int umbralMag = nominalSeleccionado * 12;
                int pdc = nominalSeleccionado <= 25 ? 10 : (nominalSeleccionado <= 63 ? 15 : 25);

                return new Termomagnetico
                {
                    Nominal = nominalSeleccionado,
                    Curva = "D (motores)",
                    PdC = pdc,
                    UmbralMag = umbralMag,
                    FactorMag = "10-14",
                    Codigo = $"iC60N-D{nominalSeleccionado} / C60N-D{nominalSeleccionado}"
                };
            }
        }

        private Cable DimensionarCable(double In, int tipoInst)
        {
            var capacidades = new[] {
                (1.5, 17.5, 15.0, 13.5),
                (2.5, 24.0, 21.0, 18.0),
                (4.0, 32.0, 28.0, 24.0),
                (6.0, 41.0, 36.0, 31.0),
                (10.0, 57.0, 50.0, 42.0),
                (16.0, 76.0, 68.0, 56.0),
                (25.0, 101.0, 89.0, 73.0),
                (35.0, 125.0, 110.0, 89.0),
                (50.0, 151.0, 134.0, 108.0),
                (70.0, 192.0, 171.0, 136.0),
                (95.0, 232.0, 207.0, 164.0),
                (120.0, 269.0, 239.0, 188.0),
                (150.0, 309.0, 273.0, 213.0),
                (185.0, 353.0, 310.0, 240.0),
                (240.0, 415.0, 364.0, 281.0)
            };

            double Iz_req = In * 1.25;
            string[] metodos = { "A1 (conducto emp.)", "B1 (conducto sup.)", "C (aire libre)" };

            foreach (var (sec, a1, b1, c) in capacidades)
            {
                double[] caps = { a1, b1, c };
                if (caps[tipoInst] >= Iz_req)
                {
                    double longitud = 30;
                    double caida = (2 * longitud * In) / (56 * sec);

                    return new Cable
                    {
                        Seccion = sec,
                        Capacidad = (int)caps[tipoInst],
                        TipoInstalacion = metodos[tipoInst],
                        CaidaTension = caida,
                        FactorCorreccion = "1,0 (30°C)"
                    };
                }
            }

            return new Cable { Seccion = 240, Capacidad = 281, TipoInstalacion = metodos[tipoInst], CaidaTension = 1.5, FactorCorreccion = "1,0" };
        }

        private Contactor SeleccionarContactor(double In, double kW)
        {
            var contactores = new[] {
                (9, 4.0, "LC1D09"),
                (12, 5.5, "LC1D12"),
                (18, 7.5, "LC1D18"),
                (25, 11.0, "LC1D25"),
                (32, 15.0, "LC1D32"),
                (40, 18.5, "LC1D40"),
                (50, 22.0, "LC1D50"),
                (65, 30.0, "LC1D65"),
                (80, 37.0, "LC1D80"),
                (95, 45.0, "LC1D95"),
                (115, 55.0, "LC1D115"),
                (150, 75.0, "LC1D150"),
                (205, 110.0, "LC1D205"),
                (265, 132.0, "LC1D265")
            };

            foreach (var (ie, pot, cod) in contactores)
            {
                if (ie >= In * 1.1 && pot >= kW)
                {
                    return new Contactor { Ie = ie, Potencia = pot, Codigo = cod };
                }
            }

            return new Contactor { Ie = 265, Potencia = 132, Codigo = "LC1D265" };
        }

        private Tuple<ContactorEstrella, ContactorEstrella, ContactorEstrella, Rele> SeleccionarContactoresEstrellaTriangulo(double In, double kW, double tension)
        {
            var contactores = new[] {
                (9, 2.2, 4.0, "LC1D09"),
                (12, 3.0, 5.5, "LC1D12"),
                (18, 4.0, 7.5, "LC1D18"),
                (25, 5.5, 11.0, "LC1D25"),
                (32, 7.5, 15.0, "LC1D32"),
                (40, 11.0, 18.5, "LC1D40"),
                (50, 15.0, 22.0, "LC1D50"),
                (65, 18.5, 30.0, "LC1D65"),
                (80, 22.0, 37.0, "LC1D80"),
                (95, 25.0, 45.0, "LC1D95"),
                (115, 30.0, 55.0, "LC1D115"),
                (150, 37.0, 75.0, "LC1D150")
            };

            ContactorEstrella km1 = null, km2 = null, km3 = null;

            foreach (var (ie, potY, potD, cod) in contactores)
            {
                if (ie >= In * 1.1 && potD >= kW)
                {
                    km1 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    km3 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    break;
                }
            }

            double InEstrella = In / Math.Sqrt(3);
            foreach (var (ie, potY, potD, cod) in contactores)
            {
                if (ie >= InEstrella * 1.1 && potY >= kW / 3)
                {
                    km2 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    break;
                }
            }

            if (km2 == null) km2 = km1;

            var reles = new[] {
                (0.1, 0.16, "LRD01"), (0.16, 0.25, "LRD02"), (0.25, 0.4, "LRD03"),
                (0.4, 0.63, "LRD04"), (0.63, 1.0, "LRD05"), (1.0, 1.6, "LRD06"),
                (1.6, 2.5, "LRD07"), (2.5, 4.0, "LRD08"), (4.0, 6.0, "LRD10"),
                (5.5, 8.0, "LRD12"), (7.0, 10.0, "LRD14"), (9.0, 13.0, "LRD16"),
                (12.0, 18.0, "LRD21"), (16.0, 24.0, "LRD22"), (23.0, 32.0, "LRD32"),
                (30.0, 38.0, "LRD340"), (37.0, 50.0, "LRD350"), (48.0, 65.0, "LRD365")
            };

            Rele rele = null;
            foreach (var (min, max, cod) in reles)
            {
                if (In >= min && In <= max)
                {
                    rele = new Rele { RangoMin = min, RangoMax = max, Ajuste = In, Codigo = cod };
                    break;
                }
            }

            if (rele == null)
                rele = new Rele { RangoMin = 48, RangoMax = 65, Ajuste = In, Codigo = "LRD365" };

            return new Tuple<ContactorEstrella, ContactorEstrella, ContactorEstrella, Rele>(km1, km2, km3, rele);
        }

        private SoftStarter SeleccionarSoftStarter(double kW, double In, double tension, int numMotores)
        {
            // Soft-Starters comerciales (Schneider Altistart, ABB PSR)
            var softStarters = new[] {
                (4.0, 9.0, "ATS01N209Q / PSR9"),
                (5.5, 12.0, "ATS01N212Q / PSR12"),
                (7.5, 17.0, "ATS01N217Q / PSR16"),
                (11.0, 25.0, "ATS01N225Q / PSR25"),
                (15.0, 32.0, "ATS01N232Q / PSR30"),
                (18.5, 38.0, "ATS22C41Q / PSR37"),
                (22.0, 45.0, "ATS22C48Q / PSR45"),
                (30.0, 62.0, "ATS22C59Q / PSR60"),
                (37.0, 72.0, "ATS22D75Q / PSR72"),
                (45.0, 90.0, "ATS22D88Q / PSR85"),
                (55.0, 105.0, "ATS48C11Q / PSR105"),
                (75.0, 140.0, "ATS48C14Q / PSR140"),
                (90.0, 170.0, "ATS48C17Q / PSR170"),
                (110.0, 210.0, "ATS48C21Q / PSR210")
            };

            foreach (var (pot, corriente, codigo) in softStarters)
            {
                if (pot >= kW && corriente >= In)
                {
                    return new SoftStarter
                    {
                        Potencia = pot,
                        Corriente = corriente,
                        Tension = tension,
                        LimiteCorriente = corriente * 3.5, // 350% típico
                        Disipacion = pot * 10, // ~10W por kW típico
                        Codigo = codigo
                    };
                }
            }

            return new SoftStarter
            {
                Potencia = 110,
                Corriente = 210,
                Tension = tension,
                LimiteCorriente = 735,
                Disipacion = 1100,
                Codigo = "ATS48C21Q / PSR210"
            };
        }

        private Reactor CalcularReactor(double kW, double In, double tension, int tipoArranque)
        {
            // Cálculo de reactor de salida para VFD/Soft-Starter con múltiples motores
            // Impedancia típica: 2-5% para VFD, 3-5% para Soft-Starter
            double impedanciaPorcentaje = tipoArranque == 3 ? 3.0 : 4.0; // VFD vs Soft-Starter

            // Potencia aparente
            double potenciaKVA = kW * 1.2; // Factor típico

            // Inductancia por fase (mH)
            // L = (Z% × U²) / (100 × 2π × f × S)
            // Donde Z% es impedancia, U tensión fase-fase, f frecuencia, S potencia aparente
            double frecuencia = 50.0; // Hz
            double inductancia = (impedanciaPorcentaje * tension * tension) /
                                (100 * 2 * Math.PI * frecuencia * potenciaKVA * 1000);
            inductancia *= 1000; // Convertir a mH

            // Caída de tensión aproximada
            double caidaTension = impedanciaPorcentaje * 0.8; // Típicamente 80% de la impedancia

            string ubicacion = tipoArranque == 3 ?
                "Salida VFD, antes de derivación a motores" :
                "Salida Soft-Starter, antes de derivación a motores";

            string funcion = "Reduce dv/dt, protege bobinados, permite cables largos";

            // Código comercial ejemplo (Schneider / ABB)
            string codigo = $"VW3A4551 / NOCH-{(int)(potenciaKVA * 1.5)}";

            return new Reactor
            {
                Potencia = Math.Round(potenciaKVA, 1),
                Corriente = Math.Round(In, 1),
                Inductancia = Math.Round(inductancia, 2),
                CaidaTension = Math.Round(caidaTension, 1),
                Impedancia = impedanciaPorcentaje,
                Codigo = codigo,
                Funcion = funcion,
                Ubicacion = ubicacion
            };
        }

        private double ObtenerPotenciaVFD(double potenciaMotor)
        {
            double[] potenciasVFD = { 0.37, 0.55, 0.75, 1.1, 1.5, 2.2, 3.0, 4.0, 5.5, 7.5, 11.0, 15.0, 18.5, 22.0, 30.0, 37.0, 45.0, 55.0, 75.0, 90.0, 110.0, 132.0, 160.0, 200.0 };

            foreach (double p in potenciasVFD)
            {
                if (p >= potenciaMotor)
                    return p;
            }
            return 200.0;
        }

        private int ObtenerCorrienteVFD(double potencia)
        {
            var corrientesVFD = new[] {
                (0.37, 1.3), (0.55, 1.7), (0.75, 2.3), (1.1, 3.1), (1.5, 4.1),
                (2.2, 5.6), (3.0, 7.5), (4.0, 9.5), (5.5, 13.0), (7.5, 17.0),
                (11.0, 25.0), (15.0, 32.0), (18.5, 38.0), (22.0, 45.0), (30.0, 60.0),
                (37.0, 75.0), (45.0, 90.0), (55.0, 110.0), (75.0, 150.0),
                (90.0, 180.0), (110.0, 210.0), (132.0, 250.0), (160.0, 320.0), (200.0, 385.0)
            };

            double potVFD = ObtenerPotenciaVFD(potencia);
            foreach (var (p, i) in corrientesVFD)
            {
                if (Math.Abs(p - potVFD) < 0.01)
                    return (int)i;
            }
            return 385;
        }

        private string ObtenerCodigoVFD(double potencia)
        {
            double potVFD = ObtenerPotenciaVFD(potencia);
            var codigos = new[] {
                (0.37, "04M2"), (0.55, "055M2"), (0.75, "075M2"), (1.1, "11M2"), (1.5, "15M2"),
                (2.2, "22M2"), (3.0, "30M2"), (4.0, "40M2"), (5.5, "55M2"), (7.5, "75M2"),
                (11.0, "110M3"), (15.0, "150M3"), (18.5, "185M3"), (22.0, "220M3"), (30.0, "300M3"),
                (37.0, "370M3"), (45.0, "450M3"), (55.0, "550M3"), (75.0, "750M3"),
                (90.0, "900M3"), (110.0, "110N4"), (132.0, "132N4"), (160.0, "160N4"), (200.0, "200N4")
            };

            foreach (var (p, cod) in codigos)
            {
                if (Math.Abs(p - potVFD) < 0.01)
                    return cod;
            }
            return "200N4";
        }

        private string ObtenerCodigoPF525(double potencia)
        {
            double potVFD = ObtenerPotenciaVFD(potencia);
            int codigoPF = (int)(potVFD * 10);
            if (codigoPF < 10) codigoPF = (int)(potVFD * 100);
            return $"{codigoPF}";
        }

        private string VerificarCoordinacion(double In, Guardamotor gm, Termomagnetico tm,
            Cable cab, int tipoArranque, int numMotores)
        {
            string result = "";

            bool check1 = In <= tm.Nominal && tm.Nominal <= cab.Capacidad;
            result += $"  ✓ Ib ≤ In(TM) ≤ Iz: {FormatearDecimal(In)} ≤ {tm.Nominal} ≤ {cab.Capacidad} → {(check1 ? "OK" : "FALLA")}\r\n";

            bool check2 = tm.Nominal * 1.45 <= cab.Capacidad * 1.45;
            result += $"  ✓ I2 ≤ 1,45×Iz: {FormatearDecimal(tm.Nominal * 1.45)} ≤ {FormatearDecimal(cab.Capacidad * 1.45)} → {(check2 ? "OK" : "FALLA")}\r\n";

            bool check3 = gm.Ajuste >= In * 1.0 && gm.Ajuste <= In * 1.15;
            result += $"  ✓ Guardamotor 1,0-1,15×In: {FormatearDecimal(gm.Ajuste)} → {(check3 ? "OK" : "REVISAR")}\r\n";

            bool check4 = cab.CaidaTension <= 3.0;
            result += $"  ✓ Caída tensión ≤ 3%: {FormatearDecimal(cab.CaidaTension)}% → {(check4 ? "OK" : "REVISAR")}\r\n";

            if (numMotores > 1)
            {
                result += $"  ✓ Configuración {numMotores} motores en paralelo: OK\r\n";
            }

            switch (tipoArranque)
            {
                case 1:
                    result += $"  ⚠ Verificar enclavamiento mecánico KM2-KM3\r\n";
                    break;
                case 2:
                    result += $"  ✓ Soft-Starter con protecciones integradas: OK\r\n";
                    break;
                case 3:
                    result += $"  ✓ VFD con protecciones integradas: OK\r\n";
                    result += $"  ℹ Curva C apropiada para arranque suave\r\n";
                    break;
            }

            return result;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtPotencia.Clear();
            txtTension.Clear();
            txtEficiencia.Text = "90";
            txtFactorPotencia.Text = "0,85";
            cmbTipoInstalacion.SelectedIndex = 0;
            cmbTipoArranque.SelectedIndex = 0;
            numMotores.Value = 1;
            numMotores.Enabled = false;
            txtResultados.Clear();
            txtPotencia.Focus();
        }

        private void cmbTipoArranque_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Habilitar múltiples motores solo para Soft-Starter (2) y VFD (3)
            if (cmbTipoArranque.SelectedIndex >= 2)
            {
                numMotores.Enabled = true;
                numMotores.Minimum = 1;
                numMotores.Maximum = 10;
            }
            else
            {
                numMotores.Enabled = false;
                numMotores.Value = 1;
            }

            // Ajustar valores por defecto según tipo de arranque
            switch (cmbTipoArranque.SelectedIndex)
            {
                case 0: // Directo
                    txtEficiencia.Text = "90";
                    txtFactorPotencia.Text = "0,85";
                    break;
                case 1: // Y-Δ
                    txtEficiencia.Text = "91";
                    txtFactorPotencia.Text = "0,87";
                    break;
                case 2: // Soft-Starter
                    txtEficiencia.Text = "91";
                    txtFactorPotencia.Text = "0,88";
                    break;
                case 3: // VFD
                    txtEficiencia.Text = "92";
                    txtFactorPotencia.Text = "0,95";
                    break;
            }
        }
    }
}































/*using System;
using System.Globalization;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class CalculoProteccionMotores : Form
    {
        private CultureInfo culturaLocal = new CultureInfo("es-AR");

        public CalculoProteccionMotores()
        {
            InitializeComponent();
            txtEficiencia.Text = "90";
            txtFactorPotencia.Text = "0,85";
            cmbTipoInstalacion.SelectedIndex = 0;
            cmbTipoArranque.SelectedIndex = 0;

            // Configurar cultura para usar coma como separador decimal
            culturaLocal.NumberFormat.NumberDecimalSeparator = ",";
            culturaLocal.NumberFormat.NumberGroupSeparator = ".";
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                double potencia = ParsearDecimal(txtPotencia.Text);
                double tension = ParsearDecimal(txtTension.Text);
                double eficiencia = ParsearDecimal(txtEficiencia.Text) / 100;
                double fp = ParsearDecimal(txtFactorPotencia.Text);
                int tipoInstalacion = cmbTipoInstalacion.SelectedIndex;
                int tipoArranque = cmbTipoArranque.SelectedIndex; // 0=Directo, 1=Estrella-Triángulo, 2=VFD

                // Cálculo preciso de corriente nominal (IEC 60034)
                double corrienteNominal = (potencia * 1000) / (Math.Sqrt(3) * tension * eficiencia * fp);

                // Corriente de arranque según tipo de arranque
                double corrienteArranque;
                double factorArmonicosCable = 1.0;
                double factorArmonicosTermico = 1.0;

                switch (tipoArranque)
                {
                    case 0: // Arranque directo
                        corrienteArranque = corrienteNominal * 7;
                        break;
                    case 1: // Estrella-Triángulo
                        corrienteArranque = corrienteNominal * 2.5; // Reducida a ~1/3 del arranque directo
                        break;
                    case 2: // VFD
                        corrienteArranque = corrienteNominal * 1.5;
                        factorArmonicosCable = 1.2;
                        factorArmonicosTermico = 1.15;
                        break;
                    default:
                        corrienteArranque = corrienteNominal * 7;
                        break;
                }

                // GUARDAMOTOR
                var guardamotor = SeleccionarGuardamotorPreciso(corrienteNominal * factorArmonicosTermico);

                // TERMOMAGNÉTICO
                var termomagnetico = SeleccionarTermomagneticoPreciso(corrienteNominal, corrienteArranque, tipoArranque);

                // CABLE
                var cable = DimensionarCable(corrienteNominal * factorArmonicosCable, tipoInstalacion);

                // CONTACTOR(ES)
                Contactor contactor = null;
                ContactorEstrella contactorLinea = null;
                ContactorEstrella contactorEstrella = null;
                ContactorEstrella contactorTriangulo = null;
                Rele releTermico = null;

                if (tipoArranque == 0) // Directo
                {
                    contactor = SeleccionarContactor(corrienteNominal, potencia);
                }
                else if (tipoArranque == 1) // Estrella-Triángulo
                {
                    var contactoresYD = SeleccionarContactoresEstrellaTriangulo(corrienteNominal, potencia, tension);
                    contactorLinea = contactoresYD.Item1;
                    contactorEstrella = contactoresYD.Item2;
                    contactorTriangulo = contactoresYD.Item3;
                    releTermico = contactoresYD.Item4;
                }

                // Construir salida
                string tipoArranqueTexto = tipoArranque == 0 ? "ARRANQUE DIRECTO" :
                                          tipoArranque == 1 ? "ARRANQUE ESTRELLA-TRIÁNGULO" :
                                          "VARIADOR DE FRECUENCIA";

                txtResultados.Text = $"CÁLCULO DE PROTECCIONES MOTOR TRIFÁSICO\r\n" +
                    $"═══════════════════════════════════════════════\r\n\r\n" +
                    $"DATOS DEL MOTOR:\r\n" +
                    $"  Potencia: {FormatearDecimal(potencia)} kW\r\n" +
                    $"  Tensión: {FormatearDecimal(tension)} V\r\n" +
                    $"  Eficiencia: {FormatearDecimal(eficiencia * 100)}%\r\n" +
                    $"  Factor de potencia: {FormatearDecimal(fp)}\r\n" +
                    $"  Tipo de arranque: {tipoArranqueTexto}\r\n" +
                    (tipoArranque == 1 ? $"  Conexión motor: DOBLE ESTRELLA-TRIÁNGULO\r\n" : "") +
                    $"\r\nCORRIENTES CALCULADAS:\r\n" +
                    $"  Corriente nominal (In): {FormatearDecimal(corrienteNominal)} A\r\n" +
                    $"  Corriente de arranque: {FormatearDecimal(corrienteArranque)} A " +
                    (tipoArranque == 0 ? "(~7×In arranque directo)" :
                     tipoArranque == 1 ? "(~2,5×In en Y, ~33% del arranque directo)" :
                     "(limitada por VFD)") + "\r\n" +
                    (tipoArranque == 1 ? $"  Corriente en estrella (Y): {FormatearDecimal(corrienteNominal / Math.Sqrt(3))} A\r\n" : "") +
                    (tipoArranque == 1 ? $"  Corriente en triángulo (Δ): {FormatearDecimal(corrienteNominal)} A\r\n" : "") +
                    $"  Corriente de diseño (Ib): {FormatearDecimal(corrienteNominal)} A\r\n" +
                    $"  Corriente admisible cable (Iz): ≥ {FormatearDecimal(corrienteNominal * 1.25 * factorArmonicosCable)} A\r\n" +
                    (tipoArranque == 2 ? $"  Factor armónicos (cable): {FormatearDecimal(factorArmonicosCable)}\r\n" : "") +
                    (tipoArranque == 2 ? $"  Factor armónicos (térmico): {FormatearDecimal(factorArmonicosTermico)}\r\n" : "") +
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"PROTECCIÓN TÉRMICA - GUARDAMOTOR\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    $"  Rango de ajuste: {FormatearDecimal(guardamotor.RangoMin)}-{FormatearDecimal(guardamotor.RangoMax)} A\r\n" +
                    $"  Ajuste recomendado: {FormatearDecimal(guardamotor.Ajuste)} A\r\n" +
                    $"  Clase de disparo: Clase 10A\r\n" +
                    $"  Código comercial: GV2ME{guardamotor.Codigo} / MS132-{FormatearDecimal(guardamotor.RangoMax)}\r\n" +
                    (tipoArranque == 1 ? $"  Nota: Para Y-Δ puede usarse relé térmico en su lugar\r\n" : "") +
                    (tipoArranque == 2 ? $"  Nota: Ajustar considerando armónicos del VFD\r\n" : "") +
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"PROTECCIÓN CORTOCIRCUITO - TERMOMAGNÉTICO\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    $"  Corriente nominal: {termomagnetico.Nominal} A\r\n" +
                    $"  Curva característica: {termomagnetico.Curva}\r\n" +
                    $"  Capacidad de ruptura: {termomagnetico.PdC} kA\r\n" +
                    $"  Umbral magnético: {termomagnetico.UmbralMag} A ({termomagnetico.FactorMag}×In)\r\n" +
                    $"  Código comercial: {termomagnetico.Codigo}\r\n" +
                    $"  Norma: IEC 60898-2 / IEC 60947-2\r\n" +
                    (tipoArranque == 1 ? $"  Nota: Curva D adecuada para pico inicial en Y-Δ\r\n" : "") +
                    (tipoArranque == 2 ? $"  Nota: Curva C apropiada para arranque suave con VFD\r\n" : "") +
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"CONDUCTOR DE ALIMENTACIÓN\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    $"  Sección nominal: {FormatearDecimal(cable.Seccion)} mm²\r\n" +
                    $"  Capacidad de corriente (Iz): {cable.Capacidad} A\r\n" +
                    $"  Tipo instalación: {cable.TipoInstalacion}\r\n" +
                    $"  Material: Cobre\r\n" +
                    $"  Aislación: PVC 90°C / XLPE 90°C\r\n" +
                    $"  Caída de tensión estimada: {FormatearDecimal(cable.CaidaTension)}%\r\n" +
                    $"  Factor de corrección: {cable.FactorCorreccion}\r\n" +
                    (tipoArranque == 2 ? $"  Nota: Considerar cable apantallado para reducir EMI\r\n" : "") +
                    $"\r\n══════════════════════════════════════════════\r\n";

                // Sección de contactores según tipo de arranque
                if (tipoArranque == 0) // Directo
                {
                    txtResultados.Text +=
                        $"CONTACTOR\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Categoría: AC-3 (motores)\r\n" +
                        $"  Corriente nominal (AC-3): {contactor.Ie} A\r\n" +
                        $"  Potencia asignada 400V: {FormatearDecimal(contactor.Potencia)} kW\r\n" +
                        $"  Durabilidad eléctrica: ~1M operaciones\r\n" +
                        $"  Código comercial: {contactor.Codigo}\r\n" +
                        $"\r\n══════════════════════════════════════════════\r\n";
                }
                else if (tipoArranque == 1) // Estrella-Triángulo
                {
                    txtResultados.Text +=
                        $"SISTEMA DE CONTACTORES ESTRELLA-TRIÁNGULO\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"CONTACTOR DE LÍNEA (KM1):\r\n" +
                        $"  Categoría: AC-3\r\n" +
                        $"  Corriente nominal: {contactorLinea.Ie} A\r\n" +
                        $"  Potencia asignada (Δ): {FormatearDecimal(contactorLinea.PotenciaTriangulo)} kW @ 400V\r\n" +
                        $"  Código comercial: {contactorLinea.Codigo}\r\n" +
                        $"  Función: Alimentación principal, permanece cerrado\r\n\r\n" +
                        $"CONTACTOR ESTRELLA (KM2):\r\n" +
                        $"  Categoría: AC-3\r\n" +
                        $"  Corriente nominal: {contactorEstrella.Ie} A\r\n" +
                        $"  Potencia asignada (Y): {FormatearDecimal(contactorEstrella.PotenciaEstrella)} kW @ 400V\r\n" +
                        $"  Código comercial: {contactorEstrella.Codigo}\r\n" +
                        $"  Función: Cierra para arranque (Y), abre antes de Δ\r\n\r\n" +
                        $"CONTACTOR TRIÁNGULO (KM3):\r\n" +
                        $"  Categoría: AC-3\r\n" +
                        $"  Corriente nominal: {contactorTriangulo.Ie} A\r\n" +
                        $"  Potencia asignada (Δ): {FormatearDecimal(contactorTriangulo.PotenciaTriangulo)} kW @ 400V\r\n" +
                        $"  Código comercial: {contactorTriangulo.Codigo}\r\n" +
                        $"  Función: Cierra para régimen normal (Δ)\r\n\r\n" +
                        $"RELÉ TÉRMICO:\r\n" +
                        $"  Rango de ajuste: {FormatearDecimal(releTermico.RangoMin)}-{FormatearDecimal(releTermico.RangoMax)} A\r\n" +
                        $"  Ajuste recomendado: {FormatearDecimal(releTermico.Ajuste)} A (corriente Δ)\r\n" +
                        $"  Clase de disparo: Clase 10A\r\n" +
                        $"  Código comercial: {releTermico.Codigo}\r\n\r\n" +
                        $"TEMPORIZADOR:\r\n" +
                        $"  Tiempo arranque Y: 3-8 segundos (ajustable)\r\n" +
                        $"  Retardo Y→Δ: 50-100 ms (anti-cortocircuito)\r\n" +
                        $"  Código ejemplo: RE7TA11MW / RE11RMMW (Schneider)\r\n\r\n" +
                        $"ENCLAVAMIENTOS:\r\n" +
                        $"  KM2 y KM3: Enclavamiento eléctrico y mecánico obligatorio\r\n" +
                        $"  Previene cortocircuito entre fases en transición\r\n" +
                        $"\r\n══════════════════════════════════════════════\r\n";
                }
                else if (tipoArranque == 2) // VFD
                {
                    txtResultados.Text +=
                        $"VARIADOR DE FRECUENCIA\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"  Potencia nominal: {ObtenerPotenciaVFD(potencia)} kW\r\n" +
                        $"  Corriente nominal: {ObtenerCorrienteVFD(potencia)} A\r\n" +
                        $"  Sobrecarga: 150% por 60s (típico)\r\n" +
                        $"  Código ejemplo: ATV320U{ObtenerCodigoVFD(potencia)} (Schneider)\r\n" +
                        $"                  PowerFlex 525-{ObtenerCodigoPF525(potencia)} (Rockwell)\r\n" +
                        $"  Filtro EMI/RFI: Clase C2/C3 recomendado\r\n" +
                        $"  Protecciones integradas: Sobrecorriente, sobretensión,\r\n" +
                        $"                          subtensión, sobretemperatura\r\n" +
                        $"\r\n══════════════════════════════════════════════\r\n";
                }

                // Verificaciones
                txtResultados.Text +=
                    $"VERIFICACIONES\r\n" +
                    $"══════════════════════════════════════════════\r\n" +
                    VerificarCoordinacion(corrienteNominal, guardamotor, termomagnetico, cable, tipoArranque) +
                    $"\r\n══════════════════════════════════════════════\r\n" +
                    $"ESQUEMA DE CONEXIÓN\r\n" +
                    $"══════════════════════════════════════════════\r\n";

                // Esquemas según tipo de arranque
                if (tipoArranque == 0) // Directo
                {
                    txtResultados.Text +=
                        $"ARRANQUE DIRECTO (DOL - Direct On Line)\r\n\r\n" +
                        $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                        $"                         |\r\n" +
                        $"                         |\r\n" +
                        $"              ┌──────────┴──────────┐\r\n" +
                        $"              │  TERMOMAGNÉTICO     │\r\n" +
                        $"              │  {termomagnetico.Nominal}A Curva D        │\r\n" +
                        $"              │  IEC 60898-2        │\r\n" +
                        $"              └──────────┬──────────┘\r\n" +
                        $"                         |\r\n" +
                        $"              ┌──────────┴──────────┐\r\n" +
                        $"              │   CONTACTOR KM      │\r\n" +
                        $"              │   {contactor.Ie}A AC-3         │\r\n" +
                        $"              │   {contactor.Codigo}       │\r\n" +
                        $"              └──────────┬──────────┘\r\n" +
                        $"                         |\r\n" +
                        $"              ┌──────────┴──────────┐\r\n" +
                        $"              │   GUARDAMOTOR       │\r\n" +
                        $"              │   {FormatearDecimal(guardamotor.RangoMin)}-{FormatearDecimal(guardamotor.RangoMax)}A Ajuste:{FormatearDecimal(guardamotor.Ajuste)}A │\r\n" +
                        $"              │   GV2ME{guardamotor.Codigo}          │\r\n" +
                        $"              └──────────┬──────────┘\r\n" +
                        $"                         |\r\n" +
                        $"        ┌────────────────┼────────────────┐\r\n" +
                        $"        │                │                │\r\n" +
                        $"       U/L1             V/L2             W/L3\r\n" +
                        $"        │                │                │\r\n" +
                        $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                        $"    │         MOTOR TRIFÁSICO                │\r\n" +
                        $"    │         {FormatearDecimal(potencia)} kW / {FormatearDecimal(tension)} V               │\r\n" +
                        $"    │         In = {FormatearDecimal(corrienteNominal)} A                     │\r\n" +
                        $"    │         Ia ≈ {FormatearDecimal(corrienteArranque)} A (arranque)          │\r\n" +
                        $"    └────────────────────────────────────────┘\r\n" +
                        $"                         |\r\n" +
                        $"                        PE (Tierra)\r\n\r\n" +
                        $"FUNCIONAMIENTO:\r\n" +
                        $"  • Pulsador START → Energiza bobina KM\r\n" +
                        $"  • KM cierra → Motor arranca directamente a tensión plena\r\n" +
                        $"  • Corriente de arranque: ~{FormatearDecimal(corrienteArranque)}A (7×In)\r\n" +
                        $"  • Torque de arranque: 100% del nominal\r\n" +
                        $"  • Tiempo de arranque: 1-3 segundos (típico)\r\n" +
                        $"  • Pulsador STOP → Desenergiza KM → Motor se detiene\r\n\r\n" +
                        $"PROTECCIONES:\r\n" +
                        $"  ✓ Cortocircuito: Termomagnético {termomagnetico.Nominal}A\r\n" +
                        $"  ✓ Sobrecarga: Guardamotor {FormatearDecimal(guardamotor.RangoMin)}-{FormatearDecimal(guardamotor.RangoMax)}A\r\n" +
                        $"  ✓ Maniobra: Contactor {contactor.Ie}A\r\n";
                }
                else if (tipoArranque == 1) // Estrella-Triángulo
                {
                    txtResultados.Text +=
                        $"                    RED TRIFÁSICA\r\n" +
                        $"                         |\r\n" +
                        $"                Termomag. {termomagnetico.Nominal}A (Curva D)\r\n" +
                        $"                         |\r\n" +
                        $"              ┌──────────┴──────────┐\r\n" +
                        $"              │  KM1 ({contactorLinea.Ie}A) LÍNEA   │\r\n" +
                        $"              └──────────┬──────────┘\r\n" +
                        $"                         |\r\n" +
                        $"                   Relé Térmico\r\n" +
                        $"                  ({FormatearDecimal(releTermico.RangoMin)}-{FormatearDecimal(releTermico.RangoMax)}A)\r\n" +
                        $"                         |\r\n" +
                        $"        ┌────────────────┼────────────────┐\r\n" +
                        $"        │                │                │\r\n" +
                        $"       U1               V1               W1\r\n" +
                        $"        │                │                │\r\n" +
                        $"    ┌───┴───┐        ┌───┴───┐        ┌───┴───┐\r\n" +
                        $"    │  KM2  │────────│  KM2  │────────│  KM2  │  ESTRELLA\r\n" +
                        $"    │  (Y)  │   ┌────│  (Y)  │───┐    │  (Y)  │  ({contactorEstrella.Ie}A)\r\n" +
                        $"    └───┬───┘   │    └───────┘   │    └───────┘  Arranque\r\n" +
                        $"        │       │                │\r\n" +
                        $"       U2      V2               W2\r\n" +
                        $"        │       │                │\r\n" +
                        $"    ┌───┴───────┴───┐  ┌─────────────┐\r\n" +
                        $"    │  KM3 (Δ) {contactorTriangulo.Ie}A │──│  KM3 (Δ)    │  TRIÁNGULO\r\n" +
                        $"    └───┬───────────┘  └──────┬──────┘  Régimen\r\n" +
                        $"        │                     │\r\n" +
                        $"        └─────────┬───────────┘\r\n" +
                        $"                  |\r\n" +
                        $"             MOTOR {FormatearDecimal(potencia)}kW\r\n" +
                        $"          (Conexión Y-Δ doble)\r\n\r\n" +
                        $"SECUENCIA DE ARRANQUE:\r\n" +
                        $"  1. Cierra KM1 (línea) + KM2 (estrella)\r\n" +
                        $"  2. Motor arranca en Y (I≈{FormatearDecimal(corrienteArranque)}A, T≈33%)\r\n" +
                        $"  3. Espera 3-8 seg (temporizador)\r\n" +
                        $"  4. Abre KM2 (estrella)\r\n" +
                        $"  5. Retardo 50-100ms (anti-cortocircuito)\r\n" +
                        $"  6. Cierra KM3 (triángulo)\r\n" +
                        $"  7. Motor en régimen Δ (I={FormatearDecimal(corrienteNominal)}A, T=100%)\r\n";
                }
                else if (tipoArranque == 2) // VFD
                {
                    txtResultados.Text +=
                        $"ARRANQUE CON VARIADOR DE FRECUENCIA (VFD)\r\n\r\n" +
                        $"                RED TRIFÁSICA L1-L2-L3\r\n" +
                        $"                         |\r\n" +
                        $"              ┌──────────┴──────────┐\r\n" +
                        $"              │  TERMOMAGNÉTICO     │\r\n" +
                        $"              │  {termomagnetico.Nominal}A Curva C        │\r\n" +
                        $"              │  (Arranque suave)   │\r\n" +
                        $"              └──────────┬──────────┘\r\n" +
                        $"                         |\r\n" +
                        $"    ╔════════════════════╧════════════════════╗\r\n" +
                        $"    ║    VARIADOR DE FRECUENCIA (VFD)        ║\r\n" +
                        $"    ║    {ObtenerPotenciaVFD(potencia)} kW / {ObtenerCorrienteVFD(potencia)} A                       ║\r\n" +
                        $"    ║    {ObtenerCodigoVFD(potencia)} (Schneider ATV320)          ║\r\n" +
                        $"    ║                                        ║\r\n" +
                        $"    ║  ENTRADA (Rectificador):               ║\r\n" +
                        $"    ║  L1 ─┐                                 ║\r\n" +
                        $"    ║  L2 ─┤► DC BUS (Filtro + Capacitores) ║\r\n" +
                        $"    ║  L3 ─┘                                 ║\r\n" +
                        $"    ║       │                                ║\r\n" +
                        $"    ║  SALIDA (Inversor PWM):                ║\r\n" +
                        $"    ║       ├─► U  (Variable 0-{FormatearDecimal(tension)}V, 0-50Hz)  ║\r\n" +
                        $"    ║       ├─► V  (Frecuencia ajustable)   ║\r\n" +
                        $"    ║       └─► W  (Control V/f)            ║\r\n" +
                        $"    ║                                        ║\r\n" +
                        $"    ║  PROTECCIONES INTEGRADAS:              ║\r\n" +
                        $"    ║  • Sobrecorriente (150% × 60s)         ║\r\n" +
                        $"    ║  • Sobretensión DC bus                 ║\r\n" +
                        $"    ║  • Subtensión entrada                  ║\r\n" +
                        $"    ║  • Sobretemperatura IGBT               ║\r\n" +
                        $"    ║  • Pérdida de fase                     ║\r\n" +
                        $"    ║  • Cortocircuito salida                ║\r\n" +
                        $"    ╚════════════════════╤════════════════════╝\r\n" +
                        $"                         |\r\n" +
                        $"        ┌────────────────┼────────────────┐\r\n" +
                        $"        │                │                │\r\n" +
                        $"       U/T1             V/T2             W/T3\r\n" +
                        $"        │                │                │\r\n" +
                        (guardamotor != null ?
                        $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                        $"    │   GUARDAMOTOR (Opcional)               │\r\n" +
                        $"    │   {FormatearDecimal(guardamotor.RangoMin)}-{FormatearDecimal(guardamotor.RangoMax)}A                              │\r\n" +
                        $"    └───┬────────────────┬────────────────┬───┘\r\n" +
                        $"        │                │                │\r\n" : "") +
                        $"    ┌───┴────────────────┴────────────────┴───┐\r\n" +
                        $"    │         MOTOR TRIFÁSICO                │\r\n" +
                        $"    │         {FormatearDecimal(potencia)} kW / {FormatearDecimal(tension)} V               │\r\n" +
                        $"    │         In = {FormatearDecimal(corrienteNominal)} A                     │\r\n" +
                        $"    │         Control 0-100% velocidad       │\r\n" +
                        $"    └────────────────────────────────────────┘\r\n" +
                        $"                         |\r\n" +
                        $"                        PE (Tierra)\r\n\r\n" +
                        $"CABLEADO ESPECIAL:\r\n" +
                        $"  • Cable de potencia VFD→Motor: APANTALLADO recomendado\r\n" +
                        $"  • Pantalla conectada a tierra en ambos extremos\r\n" +
                        $"  • Separar cables de potencia y señal (min. 20cm)\r\n" +
                        $"  • Longitud máxima recomendada: 50m sin filtro, 100m con filtro\r\n\r\n" +
                        $"FILTRO EMI/RFI:\r\n" +
                        $"  • Clase C2: Entornos industriales (emisiones < norma)\r\n" +
                        $"  • Clase C3: Entornos residenciales (máxima atenuación)\r\n" +
                        $"  • Obligatorio según EN 61800-3 para instalaciones sensibles\r\n\r\n" +
                        $"FUNCIONAMIENTO:\r\n" +
                        $"  1. Rampa de aceleración programable (típico 5-20 seg)\r\n" +
                        $"  2. Arranque suave: corriente limitada ~{FormatearDecimal(corrienteArranque)}A (1,5×In)\r\n" +
                        $"  3. Control V/f: tensión proporcional a frecuencia\r\n" +
                        $"  4. Velocidad variable: 0-100% (0-50Hz típico)\r\n" +
                        $"  5. Rampa de desaceleración: frenado controlado\r\n" +
                        $"  6. Ahorro energético: reduce consumo en carga parcial\r\n\r\n" +
                        $"VENTAJAS VFD:\r\n" +
                        $"  ✓ Arranque suave (sin picos de corriente)\r\n" +
                        $"  ✓ Control preciso de velocidad\r\n" +
                        $"  ✓ Ahorro energético (hasta 30-50%)\r\n" +
                        $"  ✓ Protecciones integradas completas\r\n" +
                        $"  ✓ Factor de potencia mejorado (~0,95)\r\n" +
                        $"  ✓ Reduce desgaste mecánico\r\n\r\n" +
                        $"CONSIDERACIONES:\r\n" +
                        $"  ⚠ Armónicos en red (THDi típico 30-40%)\r\n" +
                        $"  ⚠ EMI/RFI requiere cable apantallado\r\n" +
                        $"  ⚠ Costo inicial mayor\r\n" +
                        $"  ⚠ Mantenimiento electrónico especializado\r\n\r\n" +
                        $"PARÁMETROS TÍPICOS A CONFIGURAR:\r\n" +
                        $"  • Corriente nominal motor: {FormatearDecimal(corrienteNominal)} A\r\n" +
                        $"  • Frecuencia nominal: 50 Hz\r\n" +
                        $"  • Tensión nominal: {FormatearDecimal(tension)} V\r\n" +
                        $"  • Rampa aceleración: 10 seg (ajustar según aplicación)\r\n" +
                        $"  • Rampa desaceleración: 15 seg (evitar sobretensión)\r\n" +
                        $"  • Frecuencia mínima: 5 Hz (evitar sobrecalentamiento)\r\n" +
                        $"  • Frecuencia máxima: 50 Hz (o según motor)\r\n";
                }

                // Notas adicionales para Y-Δ
                if (tipoArranque == 1)
                {
                    txtResultados.Text +=
                        $"\r\n══════════════════════════════════════════════\r\n" +
                        $"NOTAS IMPORTANTES ESTRELLA-TRIÁNGULO\r\n" +
                        $"══════════════════════════════════════════════\r\n" +
                        $"⚠ Motor debe tener 6 bornes accesibles (U1,V1,W1,U2,V2,W2)\r\n" +
                        $"⚠ Tensión de placa debe ser compatible con conexión Δ a la\r\n" +
                        $"  tensión de red (ej: 230/400V motor en red 400V)\r\n" +
                        $"⚠ Enclavamiento mecánico obligatorio entre KM2 y KM3\r\n" +
                        $"⚠ Torque de arranque reducido a ~33% (no apto para cargas pesadas)\r\n" +
                        $"⚠ Corriente de arranque reducida a ~33% del arranque directo\r\n" +
                        $"⚠ Temporizador debe ajustarse según inercia de la carga\r\n" +
                        $"✓ Ideal para: bombas, ventiladores, compresores sin carga inicial\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el cálculo: {ex.Message}\r\n\r\n" +
                    "Verifique que todos los campos contengan valores numéricos válidos.\r\n" +
                    "Use coma (,) como separador decimal.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double ParsearDecimal(string texto)
        {
            texto = texto.Replace(".", ",");
            return double.Parse(texto, culturaLocal);
        }

        private string FormatearDecimal(double valor)
        {
            return valor.ToString("0.##", culturaLocal);
        }

        private class Guardamotor
        {
            public double RangoMin { get; set; }
            public double RangoMax { get; set; }
            public double Ajuste { get; set; }
            public string Codigo { get; set; }
        }

        private class Termomagnetico
        {
            public int Nominal { get; set; }
            public string Curva { get; set; }
            public int PdC { get; set; }
            public int UmbralMag { get; set; }
            public string FactorMag { get; set; }
            public string Codigo { get; set; }
        }

        private class Cable
        {
            public double Seccion { get; set; }
            public int Capacidad { get; set; }
            public string TipoInstalacion { get; set; }
            public double CaidaTension { get; set; }
            public string FactorCorreccion { get; set; }
        }

        private class Contactor
        {
            public int Ie { get; set; }
            public double Potencia { get; set; }
            public string Codigo { get; set; }
        }

        private class ContactorEstrella
        {
            public int Ie { get; set; }
            public double PotenciaEstrella { get; set; }
            public double PotenciaTriangulo { get; set; }
            public string Codigo { get; set; }
        }

        private class Rele
        {
            public double RangoMin { get; set; }
            public double RangoMax { get; set; }
            public double Ajuste { get; set; }
            public string Codigo { get; set; }
        }

        private Guardamotor SeleccionarGuardamotorPreciso(double In)
        {
            var rangos = new[] {
                (0.1, 0.16, "01"), (0.16, 0.25, "02"), (0.25, 0.4, "03"),
                (0.4, 0.63, "04"), (0.63, 1.0, "05"), (1.0, 1.6, "06"),
                (1.6, 2.5, "07"), (2.5, 4.0, "08"), (4.0, 6.3, "10"),
                (6.3, 10.0, "14"), (9.0, 14.0, "16"), (13.0, 18.0, "20"),
                (17.0, 23.0, "21"), (23.0, 32.0, "22"), (30.0, 40.0, "25"),
                (37.0, 50.0, "32"), (48.0, 65.0, "35"), (63.0, 80.0, "38")
            };

            foreach (var (min, max, cod) in rangos)
            {
                if (In >= min && In <= max)
                {
                    return new Guardamotor
                    {
                        RangoMin = min,
                        RangoMax = max,
                        Ajuste = In,
                        Codigo = cod
                    };
                }
            }

            return new Guardamotor { RangoMin = 63, RangoMax = 80, Ajuste = In, Codigo = "38" };
        }

        private Termomagnetico SeleccionarTermomagneticoPreciso(double In, double Ia, int tipoArranque)
        {
            int[] nominales = { 1, 2, 3, 4, 6, 10, 13, 16, 20, 25, 32, 40, 50, 63, 80, 100, 125 };
            double InRequerido = In * 1.25;
            int nominalSeleccionado = nominales[nominales.Length - 1];

            foreach (int nom in nominales)
            {
                if (nom >= InRequerido)
                {
                    nominalSeleccionado = nom;
                    break;
                }
            }

            if (tipoArranque == 2) // VFD - Curva C
            {
                int umbralMag = nominalSeleccionado * 7;
                int pdc = nominalSeleccionado <= 25 ? 10 : (nominalSeleccionado <= 63 ? 15 : 25);

                return new Termomagnetico
                {
                    Nominal = nominalSeleccionado,
                    Curva = "C (VFD)",
                    PdC = pdc,
                    UmbralMag = umbralMag,
                    FactorMag = "5-10",
                    Codigo = $"iC60N-C{nominalSeleccionado} / C60N-C{nominalSeleccionado}"
                };
            }
            else // Arranque directo o Y-Δ - Curva D
            {
                while (nominalSeleccionado * 12 <= Ia * 1.1)
                {
                    int indice = Array.IndexOf(nominales, nominalSeleccionado);
                    if (indice < nominales.Length - 1)
                    {
                        nominalSeleccionado = nominales[indice + 1];
                    }
                    else
                    {
                        break;
                    }
                }

                int umbralMag = nominalSeleccionado * 12;
                int pdc = nominalSeleccionado <= 25 ? 10 : (nominalSeleccionado <= 63 ? 15 : 25);

                return new Termomagnetico
                {
                    Nominal = nominalSeleccionado,
                    Curva = "D (motores)",
                    PdC = pdc,
                    UmbralMag = umbralMag,
                    FactorMag = "10-14",
                    Codigo = $"iC60N-D{nominalSeleccionado} / C60N-D{nominalSeleccionado}"
                };
            }
        }

        private Cable DimensionarCable(double In, int tipoInst)
        {
            var capacidades = new[] {
                (1.5, 17.5, 15.0, 13.5),
                (2.5, 24.0, 21.0, 18.0),
                (4.0, 32.0, 28.0, 24.0),
                (6.0, 41.0, 36.0, 31.0),
                (10.0, 57.0, 50.0, 42.0),
                (16.0, 76.0, 68.0, 56.0),
                (25.0, 101.0, 89.0, 73.0),
                (35.0, 125.0, 110.0, 89.0),
                (50.0, 151.0, 134.0, 108.0),
                (70.0, 192.0, 171.0, 136.0),
                (95.0, 232.0, 207.0, 164.0),
                (120.0, 269.0, 239.0, 188.0),
                (150.0, 309.0, 273.0, 213.0),
                (185.0, 353.0, 310.0, 240.0)
            };

            double Iz_req = In * 1.25;
            string[] metodos = { "A1 (conducto emp.)", "B1 (conducto sup.)", "C (aire libre)" };

            foreach (var (sec, a1, b1, c) in capacidades)
            {
                double[] caps = { a1, b1, c };
                if (caps[tipoInst] >= Iz_req)
                {
                    double longitud = 30;
                    double caida = (2 * longitud * In) / (56 * sec);

                    return new Cable
                    {
                        Seccion = sec,
                        Capacidad = (int)caps[tipoInst],
                        TipoInstalacion = metodos[tipoInst],
                        CaidaTension = caida,
                        FactorCorreccion = "1,0 (30°C)"
                    };
                }
            }

            return new Cable { Seccion = 185, Capacidad = 240, TipoInstalacion = metodos[tipoInst], CaidaTension = 1.5, FactorCorreccion = "1,0" };
        }

        private Contactor SeleccionarContactor(double In, double kW)
        {
            var contactores = new[] {
                (9, 4.0, "LC1D09"),
                (12, 5.5, "LC1D12"),
                (18, 7.5, "LC1D18"),
                (25, 11.0, "LC1D25"),
                (32, 15.0, "LC1D32"),
                (40, 18.5, "LC1D40"),
                (50, 22.0, "LC1D50"),
                (65, 30.0, "LC1D65"),
                (80, 37.0, "LC1D80"),
                (95, 45.0, "LC1D95"),
                (115, 55.0, "LC1D115"),
                (150, 75.0, "LC1D150")
            };

            foreach (var (ie, pot, cod) in contactores)
            {
                if (ie >= In * 1.1 && pot >= kW)
                {
                    return new Contactor { Ie = ie, Potencia = pot, Codigo = cod };
                }
            }

            return new Contactor { Ie = 150, Potencia = 75, Codigo = "LC1D150" };
        }

        private Tuple<ContactorEstrella, ContactorEstrella, ContactorEstrella, Rele> SeleccionarContactoresEstrellaTriangulo(double In, double kW, double tension)
        {
            // Tabla de contactores para Y-Δ según IEC 60947-4-1
            // Formato: (Ie, PotY_230V, PotΔ_400V, Codigo)
            var contactores = new[] {
                (9, 2.2, 4.0, "LC1D09"),
                (12, 3.0, 5.5, "LC1D12"),
                (18, 4.0, 7.5, "LC1D18"),
                (25, 5.5, 11.0, "LC1D25"),
                (32, 7.5, 15.0, "LC1D32"),
                (40, 11.0, 18.5, "LC1D40"),
                (50, 15.0, 22.0, "LC1D50"),
                (65, 18.5, 30.0, "LC1D65"),
                (80, 22.0, 37.0, "LC1D80"),
                (95, 25.0, 45.0, "LC1D95"),
                (115, 30.0, 55.0, "LC1D115"),
                (150, 37.0, 75.0, "LC1D150")
            };

            ContactorEstrella km1 = null, km2 = null, km3 = null;

            // KM1 (línea) y KM3 (triángulo): dimensionados para corriente nominal
            foreach (var (ie, potY, potD, cod) in contactores)
            {
                if (ie >= In * 1.1 && potD >= kW)
                {
                    km1 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    km3 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    break;
                }
            }

            // KM2 (estrella): puede ser menor (In/√3), pero por economía se usa el mismo
            // En aplicaciones grandes se puede reducir a 60% del nominal
            double InEstrella = In / Math.Sqrt(3);
            foreach (var (ie, potY, potD, cod) in contactores)
            {
                if (ie >= InEstrella * 1.1 && potY >= kW / 3)
                {
                    km2 = new ContactorEstrella { Ie = ie, PotenciaEstrella = potY, PotenciaTriangulo = potD, Codigo = cod };
                    break;
                }
            }

            // Si no encontró, usar el mismo que KM1
            if (km2 == null) km2 = km1;

            // Relé térmico - Rangos estándar
            var reles = new[] {
                (0.1, 0.16, "LRD01"), (0.16, 0.25, "LRD02"), (0.25, 0.4, "LRD03"),
                (0.4, 0.63, "LRD04"), (0.63, 1.0, "LRD05"), (1.0, 1.6, "LRD06"),
                (1.6, 2.5, "LRD07"), (2.5, 4.0, "LRD08"), (4.0, 6.0, "LRD10"),
                (5.5, 8.0, "LRD12"), (7.0, 10.0, "LRD14"), (9.0, 13.0, "LRD16"),
                (12.0, 18.0, "LRD21"), (16.0, 24.0, "LRD22"), (23.0, 32.0, "LRD32"),
                (30.0, 38.0, "LRD340"), (37.0, 50.0, "LRD350"), (48.0, 65.0, "LRD365")
            };

            Rele rele = null;
            foreach (var (min, max, cod) in reles)
            {
                if (In >= min && In <= max)
                {
                    rele = new Rele { RangoMin = min, RangoMax = max, Ajuste = In, Codigo = cod };
                    break;
                }
            }

            if (rele == null)
                rele = new Rele { RangoMin = 48, RangoMax = 65, Ajuste = In, Codigo = "LRD365" };

            return new Tuple<ContactorEstrella, ContactorEstrella, ContactorEstrella, Rele>(km1, km2, km3, rele);
        }

        private double ObtenerPotenciaVFD(double potenciaMotor)
        {
            double[] potenciasVFD = { 0.37, 0.55, 0.75, 1.1, 1.5, 2.2, 3.0, 4.0, 5.5, 7.5, 11.0, 15.0, 18.5, 22.0, 30.0, 37.0, 45.0, 55.0, 75.0 };

            foreach (double p in potenciasVFD)
            {
                if (p >= potenciaMotor)
                    return p;
            }
            return 75.0;
        }

        private int ObtenerCorrienteVFD(double potencia)
        {
            var corrientesVFD = new[] {
                (0.37, 1.3), (0.55, 1.7), (0.75, 2.3), (1.1, 3.1), (1.5, 4.1),
                (2.2, 5.6), (3.0, 7.5), (4.0, 9.5), (5.5, 13.0), (7.5, 17.0),
                (11.0, 25.0), (15.0, 32.0), (18.5, 38.0), (22.0, 45.0), (30.0, 60.0),
                (37.0, 75.0), (45.0, 90.0), (55.0, 110.0), (75.0, 150.0)
            };

            double potVFD = ObtenerPotenciaVFD(potencia);
            foreach (var (p, i) in corrientesVFD)
            {
                if (Math.Abs(p - potVFD) < 0.01)
                    return (int)i;
            }
            return 150;
        }

        private string ObtenerCodigoVFD(double potencia)
        {
            double potVFD = ObtenerPotenciaVFD(potencia);
            var codigos = new[] {
                (0.37, "04M2"), (0.55, "055M2"), (0.75, "075M2"), (1.1, "11M2"), (1.5, "15M2"),
                (2.2, "22M2"), (3.0, "30M2"), (4.0, "40M2"), (5.5, "55M2"), (7.5, "75M2"),
                (11.0, "110M3"), (15.0, "150M3"), (18.5, "185M3"), (22.0, "220M3"), (30.0, "300M3"),
                (37.0, "370M3"), (45.0, "450M3"), (55.0, "550M3"), (75.0, "750M3")
            };

            foreach (var (p, cod) in codigos)
            {
                if (Math.Abs(p - potVFD) < 0.01)
                    return cod;
            }
            return "750M3";
        }

        private string ObtenerCodigoPF525(double potencia)
        {
            double potVFD = ObtenerPotenciaVFD(potencia);
            int codigoPF = (int)(potVFD * 10);
            if (codigoPF < 10) codigoPF = (int)(potVFD * 100);
            return $"{codigoPF}";
        }

        private string VerificarCoordinacion(double In, Guardamotor gm, Termomagnetico tm, Cable cab, int tipoArranque)
        {
            string result = "";

            bool check1 = In <= tm.Nominal && tm.Nominal <= cab.Capacidad;
            result += $"  ✓ Ib ≤ In(TM) ≤ Iz: {FormatearDecimal(In)} ≤ {tm.Nominal} ≤ {cab.Capacidad} → {(check1 ? "OK" : "FALLA")}\r\n";

            bool check2 = tm.Nominal * 1.45 <= cab.Capacidad * 1.45;
            result += $"  ✓ I2 ≤ 1,45×Iz: {FormatearDecimal(tm.Nominal * 1.45)} ≤ {FormatearDecimal(cab.Capacidad * 1.45)} → {(check2 ? "OK" : "FALLA")}\r\n";

            bool check3 = gm.Ajuste >= In * 1.0 && gm.Ajuste <= In * 1.15;
            result += $"  ✓ Guardamotor 1,0-1,15×In: {FormatearDecimal(gm.Ajuste)} → {(check3 ? "OK" : "REVISAR")}\r\n";

            bool check4 = cab.CaidaTension <= 3.0;
            result += $"  ✓ Caída tensión ≤ 3%: {FormatearDecimal(cab.CaidaTension)}% → {(check4 ? "OK" : "REVISAR")}\r\n";

            if (tipoArranque == 2)
            {
                result += $"  ✓ VFD con protecciones integradas: OK\r\n";
                result += $"  ℹ Curva C apropiada para arranque suave\r\n";
            }
            else if (tipoArranque == 1)
            {
                result += $"  ✓ Corriente arranque Y reducida a ~33%: OK\r\n";
                result += $"  ⚠ Verificar enclavamiento mecánico KM2-KM3\r\n";
            }
            else
            {
                result += $"  ✓ Curva D permite arranque directo: OK\r\n";
            }

            return result;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtPotencia.Clear();
            txtTension.Clear();
            txtEficiencia.Text = "90";
            txtFactorPotencia.Text = "0,85";
            cmbTipoInstalacion.SelectedIndex = 0;
            cmbTipoArranque.SelectedIndex = 0;
            txtResultados.Clear();
            txtPotencia.Focus();
        }

        private void cmbTipoArranque_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ajustar valores por defecto según tipo de arranque
            switch (cmbTipoArranque.SelectedIndex)
            {
                case 0: // Directo
                    txtEficiencia.Text = "90";
                    txtFactorPotencia.Text = "0,85";
                    break;
                case 1: // Y-Δ
                    txtEficiencia.Text = "91";
                    txtFactorPotencia.Text = "0,87";
                    break;
                case 2: // VFD
                    txtEficiencia.Text = "92";
                    txtFactorPotencia.Text = "0,95";
                    break;
            }
        }
    }
}*/