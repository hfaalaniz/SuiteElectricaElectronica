using System;
using System.Collections.Generic;
using System.Linq;

namespace BuckConverterCalculator
{
    /// <summary>
    /// Datos de núcleos comerciales estándar
    /// </summary>
    public class NucleoComercial
    {
        public string Modelo { get; set; }
        public double AreaEfectiva { get; set; } // cm²
        public double AreaVentana { get; set; } // cm²
        public double LongitudMagnetica { get; set; } // cm
        public double PotenciaMin { get; set; } // VA
        public double PotenciaMax { get; set; } // VA
        public string Dimensiones { get; set; }

        public NucleoComercial(string modelo, double ae, double aw, double lm, double pMin, double pMax, string dim)
        {
            Modelo = modelo;
            AreaEfectiva = ae;
            AreaVentana = aw;
            LongitudMagnetica = lm;
            PotenciaMin = pMin;
            PotenciaMax = pMax;
            Dimensiones = dim;
        }

        public override string ToString()
        {
            return $"{Modelo} - Ae:{AreaEfectiva:F1} cm² - {PotenciaMin}-{PotenciaMax} VA";
        }
    }

    /// <summary>
    /// Calculadora de área de núcleo
    /// </summary>
    public static class CalculadoraNucleo
    {
        private static List<NucleoComercial> nucleosComerciales = new List<NucleoComercial>
        {
            new NucleoComercial("EI-48", 1.8, 1.3, 10.5, 30, 80, "48×16 mm"),
            new NucleoComercial("EI-57", 2.6, 2.0, 12.5, 60, 150, "57×19 mm"),
            new NucleoComercial("EI-66", 3.5, 3.0, 14.5, 120, 250, "66×22 mm"),
            new NucleoComercial("EI-76", 5.2, 4.5, 16.8, 200, 450, "76×25 mm"),
            new NucleoComercial("EI-84", 6.8, 6.0, 18.5, 350, 700, "84×28 mm"),
            new NucleoComercial("EI-96", 9.6, 8.5, 21.2, 550, 1200, "96×32 mm"),
            new NucleoComercial("EI-105", 12.5, 11.0, 23.2, 900, 1800, "105×35 mm"),
            new NucleoComercial("EI-120", 17.2, 15.0, 26.5, 1400, 3000, "120×40 mm"),
            new NucleoComercial("EI-133", 22.0, 19.0, 29.3, 2200, 4500, "133×44 mm"),
            new NucleoComercial("EI-150", 30.0, 26.0, 33.0, 3500, 7000, "150×50 mm"),
            new NucleoComercial("EI-162", 38.0, 33.0, 35.7, 5500, 9500, "162×54 mm"),
            new NucleoComercial("EI-180", 51.0, 45.0, 39.6, 7500, 14000, "180×60 mm"),
            new NucleoComercial("EI-200", 68.0, 60.0, 44.0, 12000, 20000, "200×66 mm")
        };

        /// <summary>
        /// Calcula el área mínima de núcleo según potencia
        /// Usa fórmula empírica validada: Ae = K × √(P_kVA) ajustada por frecuencia
        /// </summary>
        public static double CalcularAreaMinima(double potenciaVA, double frecuencia, double densidadFlujo = 1.5, double densidadCorriente = 3.5)
        {
            // Fórmula empírica tradicional para 60 Hz:
            // Ae (cm²) = 7.5 × √(P_kVA)
            // Ajuste por frecuencia: factor = 60/f
            // Ajuste por densidad de flujo: factor = 1.5/B

            double potenciaKVA = potenciaVA / 1000.0;
            double factorFrecuencia = 60.0 / frecuencia;
            double factorFlujo = 1.5 / densidadFlujo;

            // Constante base calibrada para 60Hz, B=1.5T
            double K = 7.5;

            double aeMinima = K * Math.Sqrt(potenciaKVA) * factorFrecuencia * factorFlujo;

            return aeMinima;
        }

        /// <summary>
        /// Calcula el área recomendada de núcleo (con margen de seguridad)
        /// </summary>
        public static double CalcularAreaRecomendada(double potenciaVA, double frecuencia, double densidadFlujo = 1.5)
        {
            // El área mínima ya incluye los factores básicos
            // Añadimos un margen de seguridad del 100-120% para:
            // - Excelente disipación térmica
            // - Mayor vida útil (>20 años)
            // - Operación más silenciosa
            // - Baja temperatura (<80°C con ventilación forzada)
            double aeMinima = CalcularAreaMinima(potenciaVA, frecuencia, densidadFlujo, 3.5);
            return aeMinima * 2.2; // Margen de 120% para diseño conservador
        }

        /// <summary>
        /// Sugiere núcleos comerciales apropiados para la potencia
        /// </summary>
        public static List<NucleoComercial> SugerirNucleos(double potenciaVA)
        {
            return nucleosComerciales
                .Where(n => potenciaVA >= n.PotenciaMin && potenciaVA <= n.PotenciaMax)
                .ToList();
        }

        /// <summary>
        /// Obtiene el núcleo más cercano para una potencia dada
        /// </summary>
        public static NucleoComercial ObtenerNucleoOptimo(double potenciaVA)
        {
            var sugeridos = SugerirNucleos(potenciaVA);
            if (sugeridos.Any())
                return sugeridos.First();

            // Si no hay coincidencia exacta, buscar el más cercano por encima
            return nucleosComerciales
                .Where(n => potenciaVA <= n.PotenciaMax)
                .OrderBy(n => n.PotenciaMax)
                .FirstOrDefault() ?? nucleosComerciales.Last();
        }

        /// <summary>
        /// Obtiene todos los núcleos disponibles
        /// </summary>
        public static List<NucleoComercial> ObtenerTodosLosNucleos()
        {
            return new List<NucleoComercial>(nucleosComerciales);
        }

        /// <summary>
        /// Calcula el área de núcleo a partir de medidas físicas (EI laminado)
        /// </summary>
        public static double CalcularAreaDesdeMedidas(double anchoColumna, double espesorStack, double factorApilamiento = 0.95)
        {
            return anchoColumna * espesorStack * factorApilamiento;
        }

        /// <summary>
        /// Calcula el área de núcleo toroidal
        /// </summary>
        public static double CalcularAreaToroidal(double diametroExterno, double diametroInterno, double altura)
        {
            return altura * (diametroExterno - diametroInterno) / 2.0;
        }
    }

    /// <summary>
    /// Clase que encapsula toda la lógica de negocio para el cálculo de transformadores
    /// </summary>
    public class TransformadorCalculos
    {
        // Constantes físicas
        private const double RHO_COBRE_75C = 0.0214; // Ω·mm²/m
        private const double DENSIDAD_FE_SI = 7.65;  // g/cm³
        private const double PERDIDA_ESPECIFICA_M19 = 1.4; // W/kg @ 1.5T, 60Hz
        private const double TEMP_AMBIENTE = 25.0; // °C
        private const double AREA_AWG8 = 8.37; // mm²
        private const double DIAM_AWG8 = 3.26; // mm

        #region Propiedades de Entrada

        public double VoltagePrimario { get; set; }
        public int NumDevanadosPrimario { get; set; }
        public double VoltageSec1 { get; set; }
        public double CorrienteSec1 { get; set; }
        public double VoltageSec2 { get; set; }
        public double CorrienteSec2 { get; set; }
        public bool Sec2Habilitado { get; set; }
        public double AreaNucleo { get; set; } // cm²
        public double DensidadFlujo { get; set; } // T
        public double Frecuencia { get; set; } // Hz

        #endregion

        #region Propiedades de Resultados - Potencias

        public double PotenciaSec1 { get; private set; }
        public double PotenciaSec2 { get; private set; }
        public double PotenciaTotalSalida { get; private set; }
        public double PotenciaEntrada { get; private set; }
        public double PerdidasEstimadas { get; private set; }

        #endregion

        #region Propiedades de Resultados - Devanados

        public double EspirasPorVolt { get; private set; }
        public int EspirasPrimario { get; private set; }
        public int EspirasSecundario { get; private set; }
        public double CorrientePorDevanadoPrim { get; private set; }
        public double DensidadCorrientePrim { get; private set; }
        public double DensidadCorrienteSec { get; private set; }

        #endregion

        #region Propiedades de Resultados - Áreas y Ventana

        public double AreaCobrePrimario { get; private set; }
        public double AreaCobreSec1 { get; private set; }
        public double AreaCobreSec2 { get; private set; }
        public double AreaCobreTotal { get; private set; }
        public double FactorLlenado { get; private set; }

        #endregion

        #region Propiedades de Resultados - Pérdidas

        public double PerdidasCuPrimario { get; private set; }
        public double PerdidasCuSec1 { get; private set; }
        public double PerdidasCuSec2 { get; private set; }
        public double PerdidasCuTotal { get; private set; }
        public double PerdidasNucleo { get; private set; }
        public double PerdidasTotales { get; private set; }
        public double EficienciaReal { get; private set; }

        #endregion

        #region Propiedades de Resultados - Térmico

        public double SuperficieRadiante { get; private set; } // cm²
        public double TempOperacionNatural { get; private set; }
        public double TempOperacionForzada { get; private set; }
        public double TempHotspotNatural { get; private set; }
        public double TempHotspotForzada { get; private set; }

        #endregion

        /// <summary>
        /// Constructor con valores por defecto
        /// </summary>
        public TransformadorCalculos()
        {
            // Valores por defecto
            VoltagePrimario = 220;
            NumDevanadosPrimario = 2;
            VoltageSec1 = 110;
            CorrienteSec1 = 30;
            VoltageSec2 = 110;
            CorrienteSec2 = 30;
            Sec2Habilitado = true;
            AreaNucleo = 51.99;
            DensidadFlujo = 1.5;
            Frecuencia = 60;
        }

        /// <summary>
        /// Ejecuta todos los cálculos del transformador
        /// </summary>
        public void Calcular()
        {
            CalcularPotencias();
            CalcularEspiras();
            CalcularCorrientes();
            CalcularAreas();
            CalcularPerdidas();
            CalcularAnalisisTermico();
        }

        /// <summary>
        /// Calcula las potencias del transformador
        /// </summary>
        private void CalcularPotencias()
        {
            PotenciaSec1 = VoltageSec1 * CorrienteSec1;
            PotenciaSec2 = Sec2Habilitado ? VoltageSec2 * CorrienteSec2 : 0;
            PotenciaTotalSalida = PotenciaSec1 + PotenciaSec2;
            PotenciaEntrada = PotenciaTotalSalida / 0.95; // 5% pérdidas estimadas
            PerdidasEstimadas = PotenciaEntrada - PotenciaTotalSalida;
        }

        /// <summary>
        /// Calcula el número de espiras
        /// </summary>
        private void CalcularEspiras()
        {
            // N/V = 1 / (4.44 × f × B × Ae)
            double aeMetros = AreaNucleo * 1e-4; // cm² a m²
            EspirasPorVolt = 1.0 / (4.44 * Frecuencia * DensidadFlujo * aeMetros);

            EspirasPrimario = (int)Math.Round(VoltagePrimario * EspirasPorVolt);

            // Secundarios con 3% de compensación
            int espirasSecBase = (int)Math.Round(VoltageSec1 * EspirasPorVolt);
            EspirasSecundario = (int)Math.Round(espirasSecBase * 1.03);
        }

        /// <summary>
        /// Calcula las corrientes y densidades
        /// </summary>
        private void CalcularCorrientes()
        {
            // Corriente por devanado del primario
            CorrientePorDevanadoPrim = PotenciaEntrada / (VoltagePrimario * NumDevanadosPrimario);

            // Densidades de corriente
            DensidadCorrientePrim = CorrientePorDevanadoPrim / AREA_AWG8;
            DensidadCorrienteSec = CorrienteSec1 / AREA_AWG8;
        }

        /// <summary>
        /// Calcula las áreas de cobre
        /// </summary>
        private void CalcularAreas()
        {
            AreaCobrePrimario = NumDevanadosPrimario * EspirasPrimario * AREA_AWG8;
            AreaCobreSec1 = EspirasSecundario * AREA_AWG8;
            AreaCobreSec2 = Sec2Habilitado ? EspirasSecundario * AREA_AWG8 : 0;
            AreaCobreTotal = AreaCobrePrimario + AreaCobreSec1 + AreaCobreSec2;

            // Calcular área de ventana (aproximación)
            double areaVentana = AreaNucleo * 1.8; // Factor típico para núcleos E-I
            FactorLlenado = (AreaCobreTotal / (areaVentana * 100)) * 100; // Convertir cm² a mm²
        }

        /// <summary>
        /// Calcula las pérdidas en cobre y núcleo
        /// </summary>
        private void CalcularPerdidas()
        {
            // Estimación de longitud media de espira
            double nucleoAncho = Math.Sqrt(AreaNucleo); // Aproximación
            double lmt = CalcularLongitudMediaEspira(nucleoAncho);

            // Resistencias
            double longitudPrimDev = EspirasPrimario * lmt / 100.0; // a metros
            double longitudSec = EspirasSecundario * lmt / 100.0; // a metros

            double rPrimDev = (RHO_COBRE_75C * longitudPrimDev / AREA_AWG8) * 1000; // mΩ
            double rSec = (RHO_COBRE_75C * longitudSec / AREA_AWG8) * 1000; // mΩ

            // Pérdidas en cobre
            PerdidasCuPrimario = NumDevanadosPrimario * Math.Pow(CorrientePorDevanadoPrim, 2) * rPrimDev / 1000.0;
            PerdidasCuSec1 = Math.Pow(CorrienteSec1, 2) * rSec / 1000.0;
            PerdidasCuSec2 = Sec2Habilitado ? Math.Pow(CorrienteSec2, 2) * rSec / 1000.0 : 0;
            PerdidasCuTotal = PerdidasCuPrimario + PerdidasCuSec1 + PerdidasCuSec2;

            // Pérdidas en núcleo
            double volumenNucleo = CalcularVolumenNucleo(AreaNucleo);
            double masaNucleo = (volumenNucleo * DENSIDAD_FE_SI) / 1000.0; // kg
            PerdidasNucleo = masaNucleo * PERDIDA_ESPECIFICA_M19;

            // Totales
            PerdidasTotales = PerdidasCuTotal + PerdidasNucleo;
            EficienciaReal = (PotenciaTotalSalida / (PotenciaTotalSalida + PerdidasTotales)) * 100;
        }

        /// <summary>
        /// Realiza el análisis térmico
        /// </summary>
        private void CalcularAnalisisTermico()
        {
            // Calcular superficie radiante
            double nucleoAncho = Math.Sqrt(AreaNucleo);

            // Dimensiones más realistas considerando:
            // - Espesor de bobinas (2-3 cm por lado)
            // - Espaciado para ventilación
            // - Estructura de montaje
            double anchoTotal = nucleoAncho + 6; // cm (núcleo + bobinas + espacio)
            double altoTotal = nucleoAncho + 6;  // cm
            double profundidad = nucleoAncho + 4; // cm (considerando stack de láminas)

            // Superficies que radian calor (sin contar la base)
            double supFrontal = anchoTotal * altoTotal;
            double supTrasera = anchoTotal * altoTotal;
            double supLateral1 = altoTotal * profundidad;
            double supLateral2 = altoTotal * profundidad;
            double supSuperior = anchoTotal * profundidad;

            SuperficieRadiante = supFrontal + supTrasera + supLateral1 + supLateral2 + supSuperior; // cm²

            double superficieM2 = SuperficieRadiante / 10000.0;

            // Coeficientes de transferencia de calor
            double hNatural = 12.0; // W/(m²·K) típico
            double hForzada = 40.0; // W/(m²·K) típico

            // Temperaturas
            double deltaTNatural = PerdidasTotales / (hNatural * superficieM2);
            double deltaTForzada = PerdidasTotales / (hForzada * superficieM2);

            TempOperacionNatural = TEMP_AMBIENTE + deltaTNatural;
            TempOperacionForzada = TEMP_AMBIENTE + deltaTForzada;

            // Hotspot (punto caliente) típicamente +20°C
            TempHotspotNatural = TempOperacionNatural + 20;
            TempHotspotForzada = TempOperacionForzada + 20;
        }

        /// <summary>
        /// Calcula la longitud media de espira
        /// </summary>
        private double CalcularLongitudMediaEspira(double nucleoAncho)
        {
            // LMT aproximada para núcleo E-I
            double anchoBobi = nucleoAncho + 1.5;
            double altoBobi = nucleoAncho + 1.5;
            return 2 * (anchoBobi + altoBobi) + 15; // cm
        }

        /// <summary>
        /// Calcula el volumen aproximado del núcleo
        /// </summary>
        private double CalcularVolumenNucleo(double areaNucleo)
        {
            // Aproximación para núcleo E-I
            double lado = Math.Sqrt(areaNucleo);
            return areaNucleo * lado * 0.36; // cm³ (factor de forma típico)
        }

        /// <summary>
        /// Obtiene información de escenarios térmicos
        /// </summary>
        public EscenarioTermico[] ObtenerEscenariosTermicos()
        {
            double superficieM2 = SuperficieRadiante / 10000.0;

            var escenarios = new[]
            {
                new EscenarioTermico("Natural - Pobre", 8, TEMP_AMBIENTE, PerdidasTotales, superficieM2),
                new EscenarioTermico("Natural - Típica", 12, TEMP_AMBIENTE, PerdidasTotales, superficieM2),
                new EscenarioTermico("Natural - Óptima", 15, TEMP_AMBIENTE, PerdidasTotales, superficieM2),
                new EscenarioTermico("Forzada - Débil", 25, TEMP_AMBIENTE, PerdidasTotales, superficieM2),
                new EscenarioTermico("Forzada - Típica ★", 40, TEMP_AMBIENTE, PerdidasTotales, superficieM2),
                new EscenarioTermico("Forzada - Potente", 60, TEMP_AMBIENTE, PerdidasTotales, superficieM2)
            };

            return escenarios;
        }

        /// <summary>
        /// Genera el reporte completo en formato texto
        /// </summary>
        public string GenerarReporteCompleto()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine(" CÁLCULO DE TRANSFORMADOR DE POTENCIA v2.1");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine("POTENCIAS:");
            sb.AppendLine($"  Salida 1:        {PotenciaSec1:F2} VA  ({PotenciaSec1 / 1000:F3} kVA)");
            sb.AppendLine($"  Salida 2:        {PotenciaSec2:F2} VA  ({PotenciaSec2 / 1000:F3} kVA)");
            sb.AppendLine($"  Total Salida:    {PotenciaTotalSalida:F2} VA  ({PotenciaTotalSalida / 1000:F3} kVA)");
            sb.AppendLine($"  Entrada Req.:    {PotenciaEntrada:F2} VA  ({PotenciaEntrada / 1000:F3} kVA)");
            sb.AppendLine($"  Pérdidas Est.:   {PerdidasEstimadas:F2} W   ({PerdidasEstimadas / PotenciaEntrada * 100:F2}%)");
            sb.AppendLine();
            sb.AppendLine("DEVANADOS:");
            sb.AppendLine($"  Espiras/Volt:    {EspirasPorVolt:F4} N/V");
            sb.AppendLine($"  Frecuencia:      {Frecuencia} Hz");
            sb.AppendLine($"  Densidad Flujo:  {DensidadFlujo:F3} T");
            sb.AppendLine($"  Área Núcleo:     {AreaNucleo:F2} cm²");
            sb.AppendLine();
            sb.AppendLine("PRIMARIO:");
            sb.AppendLine($"  Configuración:   {NumDevanadosPrimario} devanados independientes");
            sb.AppendLine($"  Espiras/dev:     {EspirasPrimario}");
            sb.AppendLine($"  Corriente/dev:   {CorrientePorDevanadoPrim:F3} A");
            sb.AppendLine($"  Calibre:         AWG 8 ({AREA_AWG8} mm²)");
            sb.AppendLine($"  Densidad Real:   {DensidadCorrientePrim:F2} A/mm²");
            sb.AppendLine($"  Área Cobre:      {AreaCobrePrimario:F2} mm²");
            sb.AppendLine();
            sb.AppendLine("SECUNDARIOS:");
            sb.AppendLine($"  Espiras:         {EspirasSecundario} (+3% compensación)");
            sb.AppendLine($"  Calibre:         AWG 8 ({AREA_AWG8} mm²)");
            sb.AppendLine($"  Densidad Real:   {DensidadCorrienteSec:F2} A/mm²");
            sb.AppendLine();
            sb.AppendLine("VERIFICACIÓN DE VENTANA:");
            sb.AppendLine($"  Total Cobre:     {AreaCobreTotal:F2} mm²");
            sb.AppendLine($"  Factor Llenado:  {FactorLlenado:F1}%");
            sb.AppendLine();
            sb.AppendLine("ANÁLISIS DE PÉRDIDAS:");
            sb.AppendLine($"  Pérd. Cu Prim:   {PerdidasCuPrimario:F2} W");
            sb.AppendLine($"  Pérd. Cu Sec1:   {PerdidasCuSec1:F2} W");
            if (Sec2Habilitado)
                sb.AppendLine($"  Pérd. Cu Sec2:   {PerdidasCuSec2:F2} W");
            sb.AppendLine($"  Total Cobre:     {PerdidasCuTotal:F2} W");
            sb.AppendLine($"  Pérdida Núcleo:  {PerdidasNucleo:F2} W");
            sb.AppendLine($"  Pérdida Total:   {PerdidasTotales:F2} W ({PerdidasTotales / PotenciaEntrada * 100:F2}%)");
            sb.AppendLine($"  Eficiencia Real: {EficienciaReal:F2}%");
            sb.AppendLine();
            sb.AppendLine("ANÁLISIS TÉRMICO:");
            sb.AppendLine($"  Superficie Rad.: {SuperficieRadiante:F0} cm²");
            sb.AppendLine($"  Temp. Amb.:      {TEMP_AMBIENTE} °C");
            sb.AppendLine();
            sb.AppendLine("  VENTILACIÓN NATURAL:");
            sb.AppendLine($"    Temp. Operación: {TempOperacionNatural:F1} °C");
            sb.AppendLine($"    Temp. Hotspot:   {TempHotspotNatural:F1} °C");
            sb.AppendLine();
            sb.AppendLine("  VENTILACIÓN FORZADA (recomendada):");
            sb.AppendLine($"    Temp. Operación: {TempOperacionForzada:F1} °C");
            sb.AppendLine($"    Temp. Hotspot:   {TempHotspotForzada:F1} °C");
            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Representa un escenario de análisis térmico
    /// </summary>
    public class EscenarioTermico
    {
        public string Nombre { get; set; }
        public double Coeficiente { get; set; } // W/(m²·K)
        public double DeltaT { get; set; } // °C
        public double TempOperacion { get; set; } // °C
        public double TempHotspot { get; set; } // °C
        public string Estado { get; set; }

        public EscenarioTermico(string nombre, double h, double tAmb, double perdidas, double superficieM2)
        {
            Nombre = nombre;
            Coeficiente = h;
            DeltaT = perdidas / (h * superficieM2);
            TempOperacion = tAmb + DeltaT;
            TempHotspot = TempOperacion + 20; // +20°C típico para hotspot

            // Determinar estado
            if (TempHotspot < 105)
                Estado = "Excelente";
            else if (TempHotspot < 130)
                Estado = "Bueno";
            else if (TempHotspot < 155)
                Estado = "Aceptable";
            else if (TempHotspot < 180)
                Estado = "Límite";
            else
                Estado = "Crítico";
        }
    }
}
