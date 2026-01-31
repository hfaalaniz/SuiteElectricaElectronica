using System;
using System.Text;

namespace BuckConverterCalculator
{
    public class TransformerCalculatore
    {
        // Inputs
        public int TipoEntrada { get; set; }
        public double VoltajeEntrada { get; set; }
        public double Frecuencia { get; set; }
        public int NumSalidas { get; set; }
        public double VoltajeSalida1 { get; set; }
        public double CorrienteSalida1 { get; set; }
        public double VoltajeSalida2 { get; set; }
        public double CorrienteSalida2 { get; set; }
        public double DensidadFlujo { get; set; }
        public double DensidadCorriente { get; set; }
        public int TipoNucleo { get; set; }
        public int MaterialNucleo { get; set; }
        public double Eficiencia { get; set; }
        public double FactorLlenado { get; set; }
        public double TempAmbiente { get; set; }

        // Constantes físicas
        private const double PI = Math.PI;
        private const double RESISTIVIDAD_COBRE = 1.724e-8; // Ohm·m a 20°C
        private const double COEF_TEMP_COBRE = 0.00393; // /°C

        public string Calcular()
        {
            var sb = new StringBuilder();

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("              CÁLCULO DE TRANSFORMADOR DE POTENCIA v2.0");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            // 1. CÁLCULO DE POTENCIAS
            double potSalida1 = VoltajeSalida1 * CorrienteSalida1;
            double potSalida2 = NumSalidas == 2 ? VoltajeSalida2 * CorrienteSalida2 : 0;
            double potTotalSalida = potSalida1 + potSalida2;
            double potEntrada = potTotalSalida / Eficiencia;
            double perdidasEstimadas = potEntrada - potTotalSalida;

            sb.AppendLine("POTENCIAS:");
            sb.AppendLine($"  Salida 1:        {potSalida1:F2} VA ({potSalida1 / 1000:F3} kVA)");
            if (NumSalidas == 2)
                sb.AppendLine($"  Salida 2:        {potSalida2:F2} VA ({potSalida2 / 1000:F3} kVA)");
            sb.AppendLine($"  Total Salida:    {potTotalSalida:F2} VA ({potTotalSalida / 1000:F3} kVA)");
            sb.AppendLine($"  Entrada Req.:    {potEntrada:F2} VA ({potEntrada / 1000:F3} kVA)");
            sb.AppendLine($"  Pérdidas Est.:   {perdidasEstimadas:F2} W ({(perdidasEstimadas / potEntrada * 100):F2}%)");

            // 2. DIMENSIONAMIENTO DEL NÚCLEO CORREGIDO
            double Kf = 4.44; // Constante de forma
            double factorTopo = ObtenerFactorTopologia(TipoNucleo);

            // Producto de áreas basado en potencia aparente total (fórmula corregida)
            // Ap = (Pt × 10^4) / (Kf × f × Bmax × J × Ku × Kf_topo)
            // Resultado en cm⁴
            double Ap = (potEntrada * 10000.0) / (Kf * Frecuencia * DensidadFlujo * DensidadCorriente * FactorLlenado * factorTopo);

            // Dimensiones del núcleo
            double relacionVentana = ObtenerRelacionVentana(TipoNucleo);
            double Ae = Math.Sqrt(Ap / relacionVentana);  // cm²
            double Aw = Ap / Ae;  // cm²

            // Dimensiones físicas
            double anchoNucleo = Math.Sqrt(Ae);  // cm
            double altoVentana = Math.Sqrt(Aw);  // cm
            double anchoVentana = Aw / altoVentana;  // cm

            // Longitud magnética media (corregida por tipo de núcleo)
            double longitudMagnetica = ObtenerLongitudMagnetica(TipoNucleo, anchoNucleo, altoVentana);
            double volumenNucleo = Ae * longitudMagnetica;

            sb.AppendLine("DIMENSIONES DEL NÚCLEO:");
            sb.AppendLine($"  Tipo:            {ObtenerNombreTipoNucleo(TipoNucleo)}");
            sb.AppendLine($"  Material:        {ObtenerNombreMaterial(MaterialNucleo)}");
            sb.AppendLine($"  Área Núcleo Ae:  {Ae:F2} cm²");
            sb.AppendLine($"  Área Ventana Aw: {Aw:F2} cm²");
            sb.AppendLine($"  Producto Áreas:  {Ap:F2} cm⁴");
            sb.AppendLine($"  Dimensión Núcleo: {anchoNucleo:F2} × {anchoNucleo:F2} cm");
            sb.AppendLine($"  Ventana:         {anchoVentana:F2} × {altoVentana:F2} cm");
            sb.AppendLine($"  Long. Magnética: {longitudMagnetica:F2} cm");
            sb.AppendLine($"  Volumen Núcleo:  {volumenNucleo:F2} cm³");

            // 3. CÁLCULO DE ESPIRAS
            double espirasVolt = 1.0 / (Kf * Frecuencia * DensidadFlujo * Ae * 1e-4); // Ae en m²

            sb.AppendLine("DEVANADOS:");
            sb.AppendLine($"  Espiras/Volt:    {espirasVolt:F4} N/V");
            sb.AppendLine($"  Frecuencia:      {Frecuencia:F0} Hz");
            sb.AppendLine($"  Densidad Flujo:  {DensidadFlujo:F3} T");
            sb.AppendLine($"  Área Núcleo:     {Ae:F2} cm² ({Ae * 1e-4:F6} m²)");
            sb.AppendLine($"  Fórmula:         N/V = 1/(4.44×f×B×Ae)");

            // 4. DEVANADO PRIMARIO
            int numDevanadosPrim = TipoEntrada == 0 ? 1 : 2; // Monofásico: 1, Trifásico: 2
            double espirasPrim = Math.Ceiling(VoltajeEntrada * espirasVolt);
            double corrientePrimTotal = potEntrada / VoltajeEntrada;
            double corrientePrimPorDev = corrientePrimTotal / numDevanadosPrim;

            double seccionPrim = corrientePrimPorDev / DensidadCorriente;
            var awgPrim = SeleccionarAWG(seccionPrim);
            double densidadRealPrim = corrientePrimPorDev / awgPrim.Area;
            double areaCuPrim = espirasPrim * awgPrim.Area * numDevanadosPrim;
            double longitudPrim = CalcularLongitudBobinado(espirasPrim, anchoNucleo, altoVentana);
            double resistenciaPrim20 = (RESISTIVIDAD_COBRE * longitudPrim / 100.0) / (awgPrim.Area * 1e-6);

            sb.AppendLine("PRIMARIO:");
            sb.AppendLine($"  Configuración:   {numDevanadosPrim} devanado(s) {(TipoEntrada == 2 ? "+ Neutro" : "")}");
            sb.AppendLine($"  Espiras/dev:     {espirasPrim:F0}");
            sb.AppendLine($"  Corriente Total: {corrientePrimTotal:F3} A");
            sb.AppendLine($"  Corriente/dev:   {corrientePrimPorDev:F3} A");
            sb.AppendLine($"  Sección Calc.:   {seccionPrim:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgPrim.Gauge} ({awgPrim.Area:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {awgPrim.Diameter:F3} mm");
            sb.AppendLine($"  Densidad Real:   {densidadRealPrim:F2} A/mm²");
            sb.AppendLine($"  Long. Bobinado:  {longitudPrim:F2} cm");
            sb.AppendLine($"  Resistencia 20°C: {resistenciaPrim20:F6} Ω");
            sb.AppendLine($"  Área Cobre:      {areaCuPrim:F2} mm²");

            // 5. DEVANADOS SECUNDARIOS
            double espirasSecCrudo1 = VoltajeSalida1 * espirasVolt;
            int espirasSecCompensado1 = (int)Math.Ceiling(espirasSecCrudo1 * 1.03); // +3% compensación caída

            double seccionSec1 = CorrienteSalida1 / DensidadCorriente;
            var awgSec1 = SeleccionarAWG(seccionSec1);
            double densidadRealSec1 = CorrienteSalida1 / awgSec1.Area;
            double areaCuSec1 = espirasSecCompensado1 * awgSec1.Area;
            double longitudSec1 = CalcularLongitudBobinado(espirasSecCompensado1, anchoNucleo, altoVentana);
            double resistenciaSec1_20 = (RESISTIVIDAD_COBRE * longitudSec1 / 100.0) / (awgSec1.Area * 1e-6);

            sb.AppendLine("SECUNDARIO 1:");
            sb.AppendLine($"  Voltaje:         {VoltajeSalida1:F1} V");
            sb.AppendLine($"  Corriente:       {CorrienteSalida1:F2} A");
            sb.AppendLine($"  Espiras Calc.:   {espirasSecCrudo1:F2}");
            sb.AppendLine($"  Espiras Comp.:   {espirasSecCompensado1} (+3% compensación)");
            sb.AppendLine($"  Sección Calc.:   {seccionSec1:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgSec1.Gauge} ({awgSec1.Area:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {awgSec1.Diameter:F3} mm");
            sb.AppendLine($"  Densidad Real:   {densidadRealSec1:F2} A/mm²");
            sb.AppendLine($"  Long. Bobinado:  {longitudSec1:F2} cm");
            sb.AppendLine($"  Resistencia 20°C: {resistenciaSec1_20:F6} Ω");
            sb.AppendLine($"  Área Cobre:      {areaCuSec1:F2} mm²");

            double areaCuSec2 = 0;
            double longitudSec2 = 0;
            double resistenciaSec2_20 = 0;
            AWGInfo awgSec2 = new AWGInfo();
            int espirasSecCompensado2 = 0;

            if (NumSalidas == 2)
            {
                double espirasSecCrudo2 = VoltajeSalida2 * espirasVolt;
                espirasSecCompensado2 = (int)Math.Ceiling(espirasSecCrudo2 * 1.03);

                double seccionSec2 = CorrienteSalida2 / DensidadCorriente;
                awgSec2 = SeleccionarAWG(seccionSec2);
                double densidadRealSec2 = CorrienteSalida2 / awgSec2.Area;
                areaCuSec2 = espirasSecCompensado2 * awgSec2.Area;
                longitudSec2 = CalcularLongitudBobinado(espirasSecCompensado2, anchoNucleo, altoVentana);
                resistenciaSec2_20 = (RESISTIVIDAD_COBRE * longitudSec2 / 100.0) / (awgSec2.Area * 1e-6);

                sb.AppendLine("SECUNDARIO 2:");
                sb.AppendLine($"  Voltaje:         {VoltajeSalida2:F1} V");
                sb.AppendLine($"  Corriente:       {CorrienteSalida2:F2} A");
                sb.AppendLine($"  Espiras Calc.:   {espirasSecCrudo2:F2}");
                sb.AppendLine($"  Espiras Comp.:   {espirasSecCompensado2} (+3% compensación)");
                sb.AppendLine($"  Sección Calc.:   {seccionSec2:F3} mm²");
                sb.AppendLine($"  Calibre:         AWG {awgSec2.Gauge} ({awgSec2.Area:F3} mm²)");
                sb.AppendLine($"  Diámetro:        {awgSec2.Diameter:F3} mm");
                sb.AppendLine($"  Densidad Real:   {densidadRealSec2:F2} A/mm²");
                sb.AppendLine($"  Long. Bobinado:  {longitudSec2:F2} cm");
                sb.AppendLine($"  Resistencia 20°C: {resistenciaSec2_20:F6} Ω");
                sb.AppendLine($"  Área Cobre:      {areaCuSec2:F2} mm²");
            }

            // 6. VERIFICACIÓN DE VENTANA
            double totalCobre = areaCuPrim + areaCuSec1 + areaCuSec2;
            double ventanaDisp = Aw * 100; // cm² a mm²
            double factorLlenadoReal = (totalCobre / ventanaDisp) * 100;

            sb.AppendLine("VERIFICACIÓN DE VENTANA:");
            sb.AppendLine($"  Área Cobre Prim: {areaCuPrim:F2} mm²");
            sb.AppendLine($"  Área Cobre Sec1: {areaCuSec1:F2} mm²");
            if (NumSalidas == 2)
                sb.AppendLine($"  Área Cobre Sec2: {areaCuSec2:F2} mm²");
            sb.AppendLine($"  Total Cobre:     {totalCobre:F2} mm²");
            sb.AppendLine($"  Ventana Disp.:   {ventanaDisp:F2} mm²");
            sb.AppendLine($"  Factor Llenado:  {factorLlenadoReal:F1}% (objetivo: {FactorLlenado * 100:F0}%)");

            string estadoVentana = factorLlenadoReal < FactorLlenado * 100 * 1.2 ? "✓ ADECUADA" : "⚠ SOBREDIMENSIONADA";
            if (factorLlenadoReal > FactorLlenado * 100 * 1.5)
                estadoVentana = "✗ INSUFICIENTE - Rediseñar";
            sb.AppendLine($"  Estado:          {estadoVentana}");

            // 7. PROTECCIONES
            double fusiblePrim = corrientePrimTotal * 1.25;
            double fusibleSec1 = CorrienteSalida1 * 1.25;
            double fusibleSec2 = CorrienteSalida2 * 1.25;

            sb.AppendLine("PROTECCIONES RECOMENDADAS (125%):");
            sb.AppendLine($"  Primario:        {fusiblePrim:F0} A");
            sb.AppendLine($"  Secundario 1:    {fusibleSec1:F0} A");
            if (NumSalidas == 2)
                sb.AppendLine($"  Secundario 2:    {fusibleSec2:F0} A");

            // 8. ANÁLISIS DE PÉRDIDAS CORREGIDO
            // Temperatura de operación estimada (iterativa simplificada)
            double tempOperacion = TempAmbiente + 40; // Estimación inicial conservadora

            // Resistencias corregidas por temperatura
            double factorTemp = 1 + COEF_TEMP_COBRE * (tempOperacion - 20);
            double resistenciaPrim = resistenciaPrim20 * factorTemp * numDevanadosPrim;
            double resistenciaSec1 = resistenciaSec1_20 * factorTemp;
            double resistenciaSec2 = resistenciaSec2_20 * factorTemp;

            // Pérdidas en cobre
            double perdCuPrim = Math.Pow(corrientePrimPorDev, 2) * resistenciaPrim;
            double perdCuSec1 = Math.Pow(CorrienteSalida1, 2) * resistenciaSec1;
            double perdCuSec2 = NumSalidas == 2 ? Math.Pow(CorrienteSalida2, 2) * resistenciaSec2 : 0;
            double perdCuTotal = perdCuPrim + perdCuSec1 + perdCuSec2;

            // Pérdidas en el núcleo (mejoradas)
            double perdNucleo = CalcularPerdidasNucleo(MaterialNucleo, volumenNucleo, Frecuencia, DensidadFlujo);

            // Pérdidas totales y eficiencia real
            double perdTotal = perdCuTotal + perdNucleo;
            double eficienciaReal = (potTotalSalida / (potTotalSalida + perdTotal)) * 100;

            // Recalcular temperatura con pérdidas reales
            double superficieRadiacion = CalcularSuperficieRadiacion(TipoNucleo, anchoNucleo, altoVentana, anchoVentana);
            double elevacionTemp = CalcularElevacionTemperatura(perdTotal, superficieRadiacion);
            tempOperacion = TempAmbiente + elevacionTemp;

            sb.AppendLine("ANÁLISIS DE PÉRDIDAS:");
            sb.AppendLine($"  Pérd. Cu Prim:   {perdCuPrim:F2} W");
            sb.AppendLine($"  Pérd. Cu Sec1:   {perdCuSec1:F2} W");
            if (NumSalidas == 2)
                sb.AppendLine($"  Pérd. Cu Sec2:   {perdCuSec2:F2} W");
            sb.AppendLine($"  Total Cobre:     {perdCuTotal:F2} W");
            sb.AppendLine($"  Pérdida Núcleo:  {perdNucleo:F2} W");
            sb.AppendLine($"  Pérdida Total:   {perdTotal:F2} W ({(perdTotal / potEntrada * 100):F2}%)");
            sb.AppendLine($"  Eficiencia Real: {eficienciaReal:F2}%");

            // 9. ANÁLISIS TÉRMICO CORREGIDO
            sb.AppendLine("ANÁLISIS TÉRMICO:");
            sb.AppendLine($"  Temp. Ambiente:  {TempAmbiente:F0} °C");
            sb.AppendLine($"  Elev. Temp Est:  {elevacionTemp:F1} °C");
            sb.AppendLine($"  Temp. Operación: {tempOperacion:F1} °C");
            sb.AppendLine($"  Superficie Rad.: {superficieRadiacion:F0} cm²");

            string ventilacion = tempOperacion < 60 ? "Natural suficiente" :
                                tempOperacion < 80 ? "Natural mejorada recomendada" :
                                tempOperacion < 100 ? "Forzada recomendada" :
                                "Forzada + disipadores OBLIGATORIOS";

            string claseAislamiento = tempOperacion < 105 ? "A (105°C)" :
                                     tempOperacion < 130 ? "B (130°C)" :
                                     tempOperacion < 155 ? "F (155°C)" :
                                     tempOperacion < 180 ? "H (180°C)" :
                                     "C (>180°C) - Materiales especiales";

            sb.AppendLine($"  Ventilación:     {ventilacion}");
            sb.AppendLine($"  Clase Aislam.:   {claseAislamiento}");

            if (tempOperacion > 100)
                sb.AppendLine($"  ⚠ ADVERTENCIA: Temperatura elevada - Revisar diseño");

            // 10. REGULACIÓN DE VOLTAJE
            double regulacionSec1 = (resistenciaSec1 * CorrienteSalida1 / VoltajeSalida1) * 100;
            double regulacionSec2 = NumSalidas == 2 ? (resistenciaSec2 * CorrienteSalida2 / VoltajeSalida2) * 100 : 0;

            sb.AppendLine("REGULACIÓN DE VOLTAJE:");
            sb.AppendLine($"  Secundario 1:    {regulacionSec1:F2}% (@ carga nominal)");
            if (NumSalidas == 2)
                sb.AppendLine($"  Secundario 2:    {regulacionSec2:F2}% (@ carga nominal)");

            // 11. RECOMENDACIONES
            int numCapasPrim = (int)Math.Ceiling(espirasPrim * awgPrim.Diameter / (anchoVentana * 10));

            sb.AppendLine("RECOMENDACIONES DE CONSTRUCCIÓN:");
            sb.AppendLine($"  • Número de capas primario: {numCapasPrim}");
            sb.AppendLine($"  • Aislamiento entre capas: Papel kraft 0.08mm o barniz");
            sb.AppendLine($"  • Aislamiento prim-sec: {(VoltajeEntrada > 240 ? "5" : "3")} capas papel kraft");
            sb.AppendLine($"  • Material aislante: {(tempOperacion > 130 ? "Kapton, Nomex" : "Papel kraft, barniz")}");

            if (tempOperacion > 80)
                sb.AppendLine($"  • ⚠ Implementar ventilación forzada");
            if (factorLlenadoReal < 25)
                sb.AppendLine($"  • ℹ Núcleo sobredimensionado - Considerar tamaño menor");

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine($"Cálculo realizado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        private double ObtenerFactorTopologia(int tipo)
        {
            // Factor de forma que afecta el producto de áreas
            return tipo switch
            {
                0 => 4.0,  // E-I Laminado
                1 => 5.5,  // Toroidal (más eficiente)
                2 => 4.8,  // C-Core
                3 => 3.5,  // UI Laminado
                _ => 4.0
            };
        }

        private double ObtenerRelacionVentana(int tipo)
        {
            // Relación Aw/Ae típica para cada geometría
            return tipo switch
            {
                0 => 1.0,  // E-I (ventana cuadrada aprox)
                1 => 2.0,  // Toroidal (ventana alargada)
                2 => 1.2,  // C-Core
                3 => 0.8,  // UI (ventana más compacta)
                _ => 1.0
            };
        }

        private double ObtenerLongitudMagnetica(int tipo, double ancho, double alto)
        {
            // Longitud del camino magnético medio en cm
            return tipo switch
            {
                0 => 2.0 * (2.0 * ancho + alto),     // E-I: 2*(2*núcleo + ventana)
                1 => PI * (ancho + alto / 2.0),      // Toroidal: circunferencia media
                2 => 2.0 * (1.5 * ancho + alto),     // C-Core
                3 => 2.0 * (2.5 * ancho + alto),     // UI
                _ => 2.0 * (2.0 * ancho + alto)
            };
        }

        private double CalcularLongitudBobinado(double espiras, double anchoNucleo, double altoVentana)
        {
            // Longitud media de una espira (perímetro aproximado del bobinado)
            // Incluye margen para aislamientos y capas
            double diametroMedioBobina = Math.Sqrt(Math.Pow(anchoNucleo + 2.0, 2) + Math.Pow(altoVentana + 2.0, 2));
            double perimetroMedio = PI * diametroMedioBobina;
            return espiras * perimetroMedio;
        }

        private double CalcularPerdidasNucleo(int material, double volumen, double frecuencia, double densidad)
        {
            // Pérdidas específicas en W/kg para diferentes materiales
            // Fórmula de Steinmetz: P = Kh*f*B^α + Ke*f²*B²
            double densidadMaterial = material switch
            {
                0 => 7.65,  // M19
                1 => 7.70,  // M27
                2 => 7.65,  // Grano orientado
                3 => 4.80,  // Ferrita
                _ => 7.65
            };

            double masa = (volumen / 1000.0) * densidadMaterial; // kg

            double perdidaEspecifica = material switch
            {
                0 => 1.8 * (frecuencia / 60.0) * Math.Pow(densidad / 1.5, 2.0),  // M19
                1 => 2.2 * (frecuencia / 60.0) * Math.Pow(densidad / 1.4, 2.0),  // M27
                2 => 1.3 * (frecuencia / 60.0) * Math.Pow(densidad / 1.7, 2.0),  // Grano orientado
                3 => 0.15 * Math.Pow(frecuencia / 100.0, 1.3) * Math.Pow(densidad / 0.35, 2.5),  // Ferrita
                _ => 2.0 * (frecuencia / 60.0) * Math.Pow(densidad, 2.0)
            };

            return perdidaEspecifica * masa;
        }

        private double CalcularSuperficieRadiacion(int tipo, double ancho, double alto, double anchoVent)
        {
            // Superficie total aproximada del transformador en cm²
            // Incluye todas las caras expuestas al aire
            double profundidad = ancho; // Asumimos profundidad similar al ancho

            return tipo switch
            {
                0 => 2.0 * (ancho * alto + ancho * profundidad + alto * profundidad) +
                     2.0 * anchoVent * alto,  // E-I: caras + ventanas
                1 => 2.0 * PI * Math.Pow((ancho + alto) / 2.0, 2) +
                     PI * (ancho + alto) * profundidad,  // Toroidal
                2 => 2.0 * (ancho * alto + ancho * profundidad + alto * profundidad) * 1.1,  // C-Core
                3 => 2.0 * (ancho * alto + ancho * profundidad + alto * profundidad) * 1.15,  // UI
                _ => 2.0 * (ancho * alto + ancho * profundidad + alto * profundidad)
            };
        }

        private double CalcularElevacionTemperatura(double potenciaDis, double superficie)
        {
            // Modelo térmico mejorado: ΔT = P / (h × A)
            // h = coeficiente de transferencia térmica combinado (W/cm²·°C)

            // Convección natural + radiación
            double hConveccion = 0.00015; // W/(cm²·°C) para convección natural
            double hRadiacion = 0.00010;  // W/(cm²·°C) para radiación térmica
            double hTotal = hConveccion + hRadiacion;

            // Temperatura elevación básica
            double deltaT = potenciaDis / (superficie * hTotal);

            // Límite de seguridad (no puede exceder valores físicos razonables)
            if (deltaT > 150.0) deltaT = 150.0; // Límite de diseño

            return deltaT;
        }

        private string ObtenerNombreMaterial(int material)
        {
            return material switch
            {
                0 => "Acero Silicio M19",
                1 => "Acero Silicio M27",
                2 => "Acero Silicio Grano Orientado",
                3 => "Ferrita",
                _ => "Desconocido"
            };
        }

        private AWGInfo SeleccionarAWG(double seccionRequerida)
        {
            // Tabla AWG completa
            var awgTable = new[]
            {
                new AWGInfo { Gauge = 0, Diameter = 8.252, Area = 53.48 },
                new AWGInfo { Gauge = 1, Diameter = 7.348, Area = 42.41 },
                new AWGInfo { Gauge = 2, Diameter = 6.544, Area = 33.63 },
                new AWGInfo { Gauge = 3, Diameter = 5.827, Area = 26.67 },
                new AWGInfo { Gauge = 4, Diameter = 5.189, Area = 21.15 },
                new AWGInfo { Gauge = 5, Diameter = 4.621, Area = 16.77 },
                new AWGInfo { Gauge = 6, Diameter = 4.115, Area = 13.30 },
                new AWGInfo { Gauge = 7, Diameter = 3.665, Area = 10.55 },
                new AWGInfo { Gauge = 8, Diameter = 3.264, Area = 8.366 },
                new AWGInfo { Gauge = 9, Diameter = 2.906, Area = 6.634 },
                new AWGInfo { Gauge = 10, Diameter = 2.588, Area = 5.261 },
                new AWGInfo { Gauge = 11, Diameter = 2.305, Area = 4.172 },
                new AWGInfo { Gauge = 12, Diameter = 2.053, Area = 3.309 },
                new AWGInfo { Gauge = 13, Diameter = 1.828, Area = 2.624 },
                new AWGInfo { Gauge = 14, Diameter = 1.628, Area = 2.081 },
                new AWGInfo { Gauge = 15, Diameter = 1.450, Area = 1.650 },
                new AWGInfo { Gauge = 16, Diameter = 1.291, Area = 1.309 },
                new AWGInfo { Gauge = 17, Diameter = 1.150, Area = 1.038 },
                new AWGInfo { Gauge = 18, Diameter = 1.024, Area = 0.823 },
                new AWGInfo { Gauge = 19, Diameter = 0.912, Area = 0.653 },
                new AWGInfo { Gauge = 20, Diameter = 0.812, Area = 0.518 },
                new AWGInfo { Gauge = 21, Diameter = 0.723, Area = 0.410 },
                new AWGInfo { Gauge = 22, Diameter = 0.644, Area = 0.326 },
                new AWGInfo { Gauge = 23, Diameter = 0.573, Area = 0.258 },
                new AWGInfo { Gauge = 24, Diameter = 0.511, Area = 0.205 },
                new AWGInfo { Gauge = 25, Diameter = 0.455, Area = 0.162 },
                new AWGInfo { Gauge = 26, Diameter = 0.405, Area = 0.129 },
                new AWGInfo { Gauge = 27, Diameter = 0.361, Area = 0.102 },
                new AWGInfo { Gauge = 28, Diameter = 0.321, Area = 0.081 },
                new AWGInfo { Gauge = 29, Diameter = 0.286, Area = 0.064 },
                new AWGInfo { Gauge = 30, Diameter = 0.255, Area = 0.051 }
            };

            // Buscar el calibre más cercano por encima de la sección requerida
            foreach (var awg in awgTable)
            {
                if (awg.Area >= seccionRequerida)
                    return awg;
            }

            // Si ninguno es suficiente, retornar el más grueso
            return awgTable[0];
        }

        private string ObtenerNombreTipoNucleo(int tipo)
        {
            return tipo switch
            {
                0 => "E-I Laminado",
                1 => "Toroidal",
                2 => "C-Core",
                3 => "UI Laminado",
                _ => "Desconocido"
            };
        }

    }

    public class AWGInfo
    {
        public int Gauge { get; set; }
        public double Diameter { get; set; }  // mm
        public double Area { get; set; }      // mm²
    }
}




















/*using System;
using System.Collections.Generic;
using System.Text;

namespace BuckConverterCalculator
{
    public class TransformerCalculatore
    {
        #region Propiedades de Entrada

        public int TipoEntrada { get; set; }
        public double VoltajeEntrada { get; set; }
        public double Frecuencia { get; set; }
        public int NumSalidas { get; set; }
        public double VoltajeSalida1 { get; set; }
        public double CorrienteSalida1 { get; set; }
        public double VoltajeSalida2 { get; set; }
        public double CorrienteSalida2 { get; set; }
        public double DensidadFlujo { get; set; }
        public double DensidadCorriente { get; set; }
        public int TipoNucleo { get; set; }
        public int MaterialNucleo { get; set; }
        public double Eficiencia { get; set; }
        public double FactorLlenado { get; set; }
        public double TempAmbiente { get; set; }

        #endregion

        #region Constantes Físicas

        private const double RESISTIVIDAD_COBRE_20C = 1.7241e-8; // Ohm·m a 20°C
        private const double COEF_TEMP_COBRE = 0.00393; // 1/°C
        private const double PERMEABILIDAD_VACIO = 4 * Math.PI * 1e-7; // H/m

        #endregion

        #region Método Principal de Cálculo

        public string Calcular()
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("              CÁLCULO DE TRANSFORMADOR DE POTENCIA v2.0");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            // PASO 1: Cálculo de potencias
            DebugLogger.Log("CALC", "--- PASO 1: Cálculo de Potencias ---");
            double potenciaSalida1 = VoltajeSalida1 * CorrienteSalida1;
            double potenciaSalida2 = NumSalidas == 2 ? VoltajeSalida2 * CorrienteSalida2 : 0;
            double potenciaTotal = potenciaSalida1 + potenciaSalida2;
            double potenciaEntrada = potenciaTotal / Eficiencia;
            double perdidaEstimada = potenciaEntrada - potenciaTotal;

            DebugLogger.LogCalculation("Potencia Salida 1", potenciaSalida1, "VA");
            DebugLogger.LogCalculation("Potencia Salida 2", potenciaSalida2, "VA");
            DebugLogger.LogCalculation("Potencia Total Salida", potenciaTotal, "VA");
            DebugLogger.LogCalculation("Potencia Entrada", potenciaEntrada, "VA");
            DebugLogger.LogCalculation("Pérdidas Estimadas", perdidaEstimada, "W");

            sb.AppendLine("POTENCIAS:");
            sb.AppendLine($"  Salida 1:        {potenciaSalida1:F2} VA ({potenciaSalida1 / 1000:F3} kVA)");
            if (NumSalidas == 2)
                sb.AppendLine($"  Salida 2:        {potenciaSalida2:F2} VA ({potenciaSalida2 / 1000:F3} kVA)");
            sb.AppendLine($"  Total Salida:    {potenciaTotal:F2} VA ({potenciaTotal / 1000:F3} kVA)");
            sb.AppendLine($"  Entrada Req.:    {potenciaEntrada:F2} VA ({potenciaEntrada / 1000:F3} kVA)");
            sb.AppendLine($"  Pérdidas Est.:   {perdidaEstimada:F2} W ({(perdidaEstimada / potenciaTotal * 100):F2}%)");
            sb.AppendLine();

            // PASO 2: Cálculo del área del núcleo
            DebugLogger.Log("CALC", "--- PASO 2: Dimensionamiento del Núcleo ---");

            double kNucleo = ObtenerConstanteNucleo();
            double areaNucleo = kNucleo * Math.Sqrt(potenciaTotal);
            double areaVentana = areaNucleo * ObtenerRelacionVentana();
            double productoAreas = areaNucleo * areaVentana;

            DebugLogger.LogCalculation("Constante K núcleo", kNucleo);
            DebugLogger.LogCalculation("Área Núcleo (Ae)", areaNucleo, "cm²");
            DebugLogger.LogCalculation("Área Ventana (Aw)", areaVentana, "cm²");
            DebugLogger.LogCalculation("Producto Áreas (Ap)", productoAreas, "cm⁴");

            double ladoNucleo = Math.Sqrt(areaNucleo);
            double longitudMagnetica = CalcularLongitudMagnetica(areaNucleo);
            double volumenNucleo = areaNucleo * longitudMagnetica / 10;

            DebugLogger.LogCalculation("Lado Núcleo Estimado", ladoNucleo, "cm");
            DebugLogger.LogCalculation("Longitud Magnética", longitudMagnetica, "cm");
            DebugLogger.LogCalculation("Volumen Núcleo", volumenNucleo, "cm³");

            sb.AppendLine("DIMENSIONES DEL NÚCLEO:");
            sb.AppendLine($"  Tipo:            {ObtenerNombreNucleo()}");
            sb.AppendLine($"  Material:        {ObtenerNombreMaterial()}");
            sb.AppendLine($"  Área Núcleo Ae:  {areaNucleo:F2} cm²");
            sb.AppendLine($"  Área Ventana Aw: {areaVentana:F2} cm²");
            sb.AppendLine($"  Producto Áreas:  {productoAreas:F2} cm⁴");
            sb.AppendLine($"  Dimensión Aprox: {ladoNucleo:F2} × {ladoNucleo:F2} cm");
            sb.AppendLine($"  Long. Magnética: {longitudMagnetica:F2} cm");
            sb.AppendLine($"  Volumen Aprox:   {volumenNucleo:F2} cm³");
            sb.AppendLine();

            // PASO 3: Cálculo de espiras por voltio
            DebugLogger.Log("CALC", "--- PASO 3: Cálculo de Espiras/Voltio ---");

            double aeMetrosCuadrados = areaNucleo * 1e-4;
            double espirasVolt = 1.0 / (4.44 * Frecuencia * DensidadFlujo * aeMetrosCuadrados);

            DebugLogger.LogCalculation("Área Núcleo", areaNucleo, "cm²");
            DebugLogger.LogCalculation("Área Núcleo", aeMetrosCuadrados, "m²");
            DebugLogger.LogCalculation("Frecuencia", Frecuencia, "Hz");
            DebugLogger.LogCalculation("Densidad Flujo", DensidadFlujo, "T");
            DebugLogger.Log("CALC", "Fórmula: N/V = 1 / (4.44 × {0} × {1} × {2})",
                Frecuencia, DensidadFlujo, aeMetrosCuadrados);
            DebugLogger.LogCalculation("Espiras/Voltio", espirasVolt, "N/V");

            double voltajePrueba = 100;
            double espirasPrueba = voltajePrueba * espirasVolt;
            double voltajeRecalculado = espirasPrueba / espirasVolt;
            DebugLogger.Log("CALC", "Prueba: {0}V × {1:F4}N/V = {2:F0} espiras → {3:F2}V",
                voltajePrueba, espirasVolt, espirasPrueba, voltajeRecalculado);

            sb.AppendLine("DEVANADOS:");
            sb.AppendLine($"  Espiras/Volt:    {espirasVolt:F4} N/V");
            sb.AppendLine($"  Frecuencia:      {Frecuencia:F0} Hz");
            sb.AppendLine($"  Densidad Flujo:  {DensidadFlujo:F3} T");
            sb.AppendLine($"  Área Núcleo:     {areaNucleo:F2} cm² ({aeMetrosCuadrados:F6} m²)");
            sb.AppendLine($"  Fórmula:         N/V = 1/(4.44×f×B×Ae)");
            sb.AppendLine();

            // PASO 4: Cálculo del primario
            DebugLogger.Log("CALC", "--- PASO 4: Devanado Primario ---");

            double espirasPrimario = Math.Ceiling(VoltajeEntrada * espirasVolt);

            if (espirasPrimario < 10 || espirasPrimario > 50000)
            {
                DebugLogger.LogError($"ERROR CRÍTICO: Espiras primario fuera de rango razonable: {espirasPrimario}");
                DebugLogger.Log("CALC", "Valores: V={0}, N/V={1}, Ae={2}cm²={3}m², f={4}, B={5}",
                    VoltajeEntrada, espirasVolt, areaNucleo, aeMetrosCuadrados, Frecuencia, DensidadFlujo);
            }

            double voltajeVerificacion = espirasPrimario / espirasVolt;
            DebugLogger.Log("CALC", "PRIMARIO: {0}V × {1:F4}N/V = {2:F0} espiras",
                VoltajeEntrada, espirasVolt, espirasPrimario);
            DebugLogger.Log("CALC", "Verificación: {0:F0} espiras / {1:F4} N/V = {2:F2} V",
                espirasPrimario, espirasVolt, voltajeVerificacion);

            double corrientePrimario1 = potenciaSalida1 / (VoltajeEntrada * Eficiencia);
            double corrientePrimario2 = NumSalidas == 2 ? potenciaSalida2 / (VoltajeEntrada * Eficiencia) : 0;
            double corrientePrimarioTotal = corrientePrimario1 + corrientePrimario2;

            double seccionPrimarioCalculada = corrientePrimarioTotal / DensidadCorriente;
            int awgPrimario = CalcularAWG(seccionPrimarioCalculada);
            double seccionPrimarioReal = ObtenerSeccionAWG(awgPrimario);
            double diametroPrimario = ObtenerDiametroAWG(awgPrimario);
            double areaCobrePrimario = espirasPrimario * seccionPrimarioReal;

            DebugLogger.LogCalculation("Espiras Primario", espirasPrimario);
            DebugLogger.LogCalculation("Corriente Primario", corrientePrimarioTotal, "A");
            DebugLogger.LogCalculation("Sección Calculada", seccionPrimarioCalculada, "mm²");
            DebugLogger.LogCalculation("AWG Seleccionado", awgPrimario);
            DebugLogger.LogCalculation("Sección Real AWG", seccionPrimarioReal, "mm²");
            DebugLogger.LogCalculation("Área Cobre Primario", areaCobrePrimario, "mm²");

            sb.AppendLine("PRIMARIO:");
            if (TipoEntrada == 2)
            {
                sb.AppendLine($"  Configuración:   {(NumSalidas == 2 ? "2 devanados independientes" : "1 devanado")}");
                sb.AppendLine($"  Espiras/dev:     {espirasPrimario:F0}");
                sb.AppendLine($"  Corriente/dev:   {(NumSalidas == 2 ? corrientePrimario1 : corrientePrimarioTotal):F3} A");
            }
            else
            {
                sb.AppendLine($"  Espiras:         {espirasPrimario:F0}");
                sb.AppendLine($"  Corriente:       {corrientePrimarioTotal:F3} A");
            }
            sb.AppendLine($"  Sección Calc.:   {seccionPrimarioCalculada:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgPrimario} ({seccionPrimarioReal:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {diametroPrimario:F3} mm");
            sb.AppendLine($"  Densidad Real:   {(corrientePrimarioTotal / seccionPrimarioReal):F2} A/mm²");
            sb.AppendLine($"  Área Cobre:      {areaCobrePrimario:F2} mm²");
            sb.AppendLine();

            // PASO 5: Cálculo del secundario 1
            DebugLogger.Log("CALC", "--- PASO 5: Devanado Secundario 1 ---");

            double espirasSecundario1 = Math.Ceiling(VoltajeSalida1 * espirasVolt * 1.03);
            double seccionSecundario1Calculada = CorrienteSalida1 / DensidadCorriente;
            int awgSecundario1 = CalcularAWG(seccionSecundario1Calculada);
            double seccionSecundario1Real = ObtenerSeccionAWG(awgSecundario1);
            double diametroSecundario1 = ObtenerDiametroAWG(awgSecundario1);
            double areaCobreSecundario1 = espirasSecundario1 * seccionSecundario1Real;

            DebugLogger.LogCalculation("Espiras Secundario 1", espirasSecundario1);
            DebugLogger.LogCalculation("Corriente Secundario 1", CorrienteSalida1, "A");
            DebugLogger.LogCalculation("Sección Calculada S1", seccionSecundario1Calculada, "mm²");
            DebugLogger.LogCalculation("AWG S1", awgSecundario1);
            DebugLogger.LogCalculation("Área Cobre S1", areaCobreSecundario1, "mm²");

            sb.AppendLine("SECUNDARIO 1:");
            sb.AppendLine($"  Voltaje:         {VoltajeSalida1:F1} V");
            sb.AppendLine($"  Corriente:       {CorrienteSalida1:F2} A");
            sb.AppendLine($"  Espiras:         {espirasSecundario1:F0} (+3% compensación)");
            sb.AppendLine($"  Sección Calc.:   {seccionSecundario1Calculada:F3} mm²");
            sb.AppendLine($"  Calibre:         AWG {awgSecundario1} ({seccionSecundario1Real:F3} mm²)");
            sb.AppendLine($"  Diámetro:        {diametroSecundario1:F3} mm");
            sb.AppendLine($"  Densidad Real:   {(CorrienteSalida1 / seccionSecundario1Real):F2} A/mm²");
            sb.AppendLine($"  Área Cobre:      {areaCobreSecundario1:F2} mm²");
            sb.AppendLine();

            // PASO 6: Cálculo del secundario 2
            double espirasSecundario2 = 0;
            double seccionSecundario2Real = 0;
            double areaCobreSecundario2 = 0;
            int awgSecundario2 = 0;
            double diametroSecundario2 = 0;

            if (NumSalidas == 2)
            {
                DebugLogger.Log("CALC", "--- PASO 6: Devanado Secundario 2 ---");

                espirasSecundario2 = Math.Ceiling(VoltajeSalida2 * espirasVolt * 1.03);
                double seccionSecundario2Calculada = CorrienteSalida2 / DensidadCorriente;
                awgSecundario2 = CalcularAWG(seccionSecundario2Calculada);
                seccionSecundario2Real = ObtenerSeccionAWG(awgSecundario2);
                diametroSecundario2 = ObtenerDiametroAWG(awgSecundario2);
                areaCobreSecundario2 = espirasSecundario2 * seccionSecundario2Real;

                DebugLogger.LogCalculation("Espiras Secundario 2", espirasSecundario2);
                DebugLogger.LogCalculation("Corriente Secundario 2", CorrienteSalida2, "A");
                DebugLogger.LogCalculation("Sección Calculada S2", seccionSecundario2Calculada, "mm²");
                DebugLogger.LogCalculation("Área Cobre S2", areaCobreSecundario2, "mm²");

                sb.AppendLine("SECUNDARIO 2:");
                sb.AppendLine($"  Voltaje:         {VoltajeSalida2:F1} V");
                sb.AppendLine($"  Corriente:       {CorrienteSalida2:F2} A");
                sb.AppendLine($"  Espiras:         {espirasSecundario2:F0} (+3% compensación)");
                sb.AppendLine($"  Sección Calc.:   {seccionSecundario2Calculada:F3} mm²");
                sb.AppendLine($"  Calibre:         AWG {awgSecundario2} ({seccionSecundario2Real:F3} mm²)");
                sb.AppendLine($"  Diámetro:        {diametroSecundario2:F3} mm");
                sb.AppendLine($"  Densidad Real:   {(CorrienteSalida2 / seccionSecundario2Real):F2} A/mm²");
                sb.AppendLine($"  Área Cobre:      {areaCobreSecundario2:F2} mm²");
                sb.AppendLine();
            }

            // PASO 7: Verificación del factor de llenado
            DebugLogger.Log("CALC", "--- PASO 7: Verificación Factor de Llenado ---");

            double areaTotalCobre = areaCobrePrimario + areaCobreSecundario1 + areaCobreSecundario2;
            double areaVentanaMM2 = areaVentana * 100;
            double factorLlenadoReal = (areaTotalCobre / areaVentanaMM2) * 100;
            bool ventanaAdecuada = factorLlenadoReal <= (FactorLlenado * 100);

            DebugLogger.LogCalculation("Área Total Cobre", areaTotalCobre, "mm²");
            DebugLogger.LogCalculation("Área Ventana", areaVentanaMM2, "mm²");
            DebugLogger.LogCalculation("Factor Llenado Real", factorLlenadoReal, "%");
            DebugLogger.Log("CALC", "Ventana {0}", ventanaAdecuada ? "ADECUADA" : "INSUFICIENTE");

            sb.AppendLine("VERIFICACIÓN DE VENTANA:");
            sb.AppendLine($"  Área Cobre Prim: {areaCobrePrimario:F2} mm²");
            sb.AppendLine($"  Área Cobre Sec1: {areaCobreSecundario1:F2} mm²");
            if (NumSalidas == 2)
                sb.AppendLine($"  Área Cobre Sec2: {areaCobreSecundario2:F2} mm²");
            sb.AppendLine($"  Total Cobre:     {areaTotalCobre:F2} mm²");
            sb.AppendLine($"  Ventana Disp.:   {areaVentanaMM2:F2} mm²");
            sb.AppendLine($"  Factor Llenado:  {factorLlenadoReal:F1}% (objetivo: {FactorLlenado * 100:F0}%)");
            sb.AppendLine($"  Estado:          {(ventanaAdecuada ? "✓ ADECUADA" : "✗ INSUFICIENTE - Aumentar núcleo")}");
            sb.AppendLine();

            // PASO 8: Protecciones
            DebugLogger.Log("CALC", "--- PASO 8: Dimensionamiento de Protecciones ---");

            double fusiblePrimario = (NumSalidas == 2 ? corrientePrimario1 : corrientePrimarioTotal) * 1.25;
            double fusibleSec1 = CorrienteSalida1 * 1.25;
            double fusibleSec2 = NumSalidas == 2 ? CorrienteSalida2 * 1.25 : 0;

            DebugLogger.LogCalculation("Fusible Primario", fusiblePrimario, "A");
            DebugLogger.LogCalculation("Fusible Sec 1", fusibleSec1, "A");
            if (NumSalidas == 2)
                DebugLogger.LogCalculation("Fusible Sec 2", fusibleSec2, "A");

            sb.AppendLine("PROTECCIONES RECOMENDADAS (125%):");
            sb.AppendLine($"  Primario:        {Math.Ceiling(fusiblePrimario):F0} A");
            sb.AppendLine($"  Secundario 1:    {Math.Ceiling(fusibleSec1):F0} A");
            if (NumSalidas == 2)
                sb.AppendLine($"  Secundario 2:    {Math.Ceiling(fusibleSec2):F0} A");
            sb.AppendLine();

            // PASO 9: Análisis de pérdidas
            DebugLogger.Log("CALC", "--- PASO 9: Análisis de Pérdidas y Térmica ---");

            double longMediaPrimario = CalcularLongitudMediaBobinado(areaNucleo, 1);
            double longMediaSecundario1 = CalcularLongitudMediaBobinado(areaNucleo, 2);
            double longMediaSecundario2 = NumSalidas == 2 ? CalcularLongitudMediaBobinado(areaNucleo, 3) : 0;

            DebugLogger.LogCalculation("Long. Media Primario", longMediaPrimario, "cm");
            DebugLogger.LogCalculation("Long. Media Sec1", longMediaSecundario1, "cm");

            double tempCobre = TempAmbiente + 40;
            double resistividadCu = RESISTIVIDAD_COBRE_20C * (1 + COEF_TEMP_COBRE * (tempCobre - 20));

            DebugLogger.LogCalculation("Temp. Cobre Estimada", tempCobre, "°C");
            DebugLogger.LogCalculation("Resistividad Cu", resistividadCu * 1e8, "Ω·cm");

            double perdidaCobrePrimario = CalcularPerdidaCobre(espirasPrimario, corrientePrimarioTotal,
                seccionPrimarioReal, longMediaPrimario, resistividadCu);
            double perdidaCobreSecundario1 = CalcularPerdidaCobre(espirasSecundario1, CorrienteSalida1,
                seccionSecundario1Real, longMediaSecundario1, resistividadCu);
            double perdidaCobreSecundario2 = NumSalidas == 2 ?
                CalcularPerdidaCobre(espirasSecundario2, CorrienteSalida2, seccionSecundario2Real,
                longMediaSecundario2, resistividadCu) : 0;

            double perdidaCobreTotal = perdidaCobrePrimario + perdidaCobreSecundario1 + perdidaCobreSecundario2;

            DebugLogger.LogCalculation("Pérdida Cu Primario", perdidaCobrePrimario, "W");
            DebugLogger.LogCalculation("Pérdida Cu Sec1", perdidaCobreSecundario1, "W");
            if (NumSalidas == 2)
                DebugLogger.LogCalculation("Pérdida Cu Sec2", perdidaCobreSecundario2, "W");
            DebugLogger.LogCalculation("Pérdida Cu Total", perdidaCobreTotal, "W");

            double perdidaNucleo = CalcularPerdidaNucleo(volumenNucleo, DensidadFlujo, Frecuencia);

            DebugLogger.LogCalculation("Pérdida Núcleo", perdidaNucleo, "W");

            double perdidaTotal = perdidaCobreTotal + perdidaNucleo;
            double eficienciaReal = (potenciaTotal / (potenciaTotal + perdidaTotal)) * 100;

            DebugLogger.LogCalculation("Pérdida Total", perdidaTotal, "W");
            DebugLogger.LogCalculation("Eficiencia Real", eficienciaReal, "%");

            double superficieRadiacion = CalcularSuperficieRadiacion(areaNucleo);
            double elevacionTemperatura = CalcularElevacionTemperatura(perdidaTotal, superficieRadiacion);
            double tempMaximaOperacion = TempAmbiente + elevacionTemperatura;

            DebugLogger.LogCalculation("Superficie Radiación", superficieRadiacion, "cm²");
            DebugLogger.LogCalculation("Elevación Temperatura", elevacionTemperatura, "°C");
            DebugLogger.LogCalculation("Temp. Máx. Operación", tempMaximaOperacion, "°C");

            sb.AppendLine("ANÁLISIS DE PÉRDIDAS:");
            sb.AppendLine($"  Pérd. Cu Prim:   {perdidaCobrePrimario:F2} W");
            sb.AppendLine($"  Pérd. Cu Sec1:   {perdidaCobreSecundario1:F2} W");
            if (NumSalidas == 2)
                sb.AppendLine($"  Pérd. Cu Sec2:   {perdidaCobreSecundario2:F2} W");
            sb.AppendLine($"  Total Cobre:     {perdidaCobreTotal:F2} W");
            sb.AppendLine($"  Pérdida Núcleo:  {perdidaNucleo:F2} W");
            sb.AppendLine($"  Pérdida Total:   {perdidaTotal:F2} W ({(perdidaTotal / potenciaTotal * 100):F2}%)");
            sb.AppendLine($"  Eficiencia Real: {eficienciaReal:F2}%");
            sb.AppendLine();

            sb.AppendLine("ANÁLISIS TÉRMICO:");
            sb.AppendLine($"  Temp. Ambiente:  {TempAmbiente:F0} °C");
            sb.AppendLine($"  Elev. Temp Est:  {elevacionTemperatura:F1} °C");
            sb.AppendLine($"  Temp. Operación: {tempMaximaOperacion:F1} °C");
            sb.AppendLine($"  Superficie Rad.: {superficieRadiacion:F0} cm²");
            sb.AppendLine($"  Ventilación:     {ObtenerTipoVentilacion(tempMaximaOperacion)}");
            sb.AppendLine($"  Clase Aislam.:   {ObtenerClaseAislamiento(tempMaximaOperacion)}");
            sb.AppendLine();

            // PASO 10: Regulación
            DebugLogger.Log("CALC", "--- PASO 10: Regulación de Voltaje ---");

            double regulacionSec1 = CalcularRegulacion(espirasPrimario, espirasSecundario1,
                seccionPrimarioReal, seccionSecundario1Real, longMediaPrimario,
                longMediaSecundario1, corrientePrimarioTotal, CorrienteSalida1, resistividadCu);

            DebugLogger.LogCalculation("Regulación Sec1", regulacionSec1, "%");

            sb.AppendLine("REGULACIÓN DE VOLTAJE:");
            sb.AppendLine($"  Secundario 1:    {regulacionSec1:F2}% (@ carga nominal)");
            if (NumSalidas == 2)
            {
                double regulacionSec2 = CalcularRegulacion(espirasPrimario, espirasSecundario2,
                    seccionPrimarioReal, seccionSecundario2Real, longMediaPrimario,
                    longMediaSecundario2, corrientePrimarioTotal, CorrienteSalida2, resistividadCu);
                sb.AppendLine($"  Secundario 2:    {regulacionSec2:F2}% (@ carga nominal)");
                DebugLogger.LogCalculation("Regulación Sec2", regulacionSec2, "%");
            }
            sb.AppendLine();

            // PASO 11: Recomendaciones
            DebugLogger.Log("CALC", "--- PASO 11: Recomendaciones ---");

            sb.AppendLine("RECOMENDACIONES DE CONSTRUCCIÓN:");
            sb.AppendLine($"  • Número de capas primario: {Math.Ceiling(espirasPrimario / (areaVentana * 10 / diametroPrimario)):F0}");
            sb.AppendLine($"  • Aislamiento entre capas: {ObtenerAislamientoRecomendado(VoltajeEntrada)}");
            sb.AppendLine($"  • Aislamiento prim-sec: {ObtenerAislamientoPrimSec(VoltajeEntrada, VoltajeSalida1)}");
            sb.AppendLine($"  • Material aislante: {ObtenerMaterialAislante(tempMaximaOperacion)}");
            if (!ventanaAdecuada)
                sb.AppendLine($"  • ⚠ ADVERTENCIA: Aumentar núcleo ~{Math.Ceiling((factorLlenadoReal / (FactorLlenado * 100) - 1) * 100)}% o reducir densidad de corriente");
            if (tempMaximaOperacion > 100)
                sb.AppendLine($"  • ⚠ ADVERTENCIA: Temperatura elevada - Considerar ventilación forzada");
            sb.AppendLine();

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine($"Cálculo realizado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        #endregion

        #region Métodos de Cálculo de Núcleo

        private double ObtenerConstanteNucleo()
        {
            double kBase = TipoNucleo switch
            {
                0 => 0.64,  // E-I Laminado
                1 => 0.58,  // Toroidal
                2 => 0.60,  // C-Core
                3 => 0.62,  // UI Laminado
                _ => 0.64
            };

            double factorFrecuencia = Math.Sqrt(60.0 / Frecuencia);
            return kBase * factorFrecuencia;
        }

        private double CalcularLongitudMagnetica(double areaCm2)
        {
            double lado = Math.Sqrt(areaCm2);

            return TipoNucleo switch
            {
                0 => lado * 3.6,    // E-I
                1 => lado * 2.8,    // Toroidal
                2 => lado * 3.2,    // C-Core
                3 => lado * 3.5,    // UI
                _ => lado * 3.6
            };
        }

        private double CalcularLongitudMediaBobinado(double areaCm2, int capa)
        {
            double radioInterior = Math.Sqrt(areaCm2 / Math.PI);
            double incrementoPorCapa = 0.3;
            double radioEfectivo = radioInterior + (capa * incrementoPorCapa);

            return 2 * Math.PI * radioEfectivo;
        }

        private double ObtenerRelacionVentana()
        {
            return TipoNucleo switch
            {
                0 => 1.8,   // E-I
                1 => 1.2,   // Toroidal
                2 => 1.5,   // C-Core
                3 => 1.7,   // UI
                _ => 1.8
            };
        }

        private string ObtenerNombreNucleo()
        {
            return TipoNucleo switch
            {
                0 => "E-I Laminado",
                1 => "Toroidal",
                2 => "C-Core",
                3 => "UI Laminado",
                _ => "Desconocido"
            };
        }

        private string ObtenerNombreMaterial()
        {
            return MaterialNucleo switch
            {
                0 => "Acero Silicio M19 (0.014\")",
                1 => "Acero Silicio M27 (0.011\")",
                2 => "Acero Silicio Grano Orientado",
                3 => "Ferrita MnZn",
                _ => "Desconocido"
            };
        }

        #endregion

        #region Métodos de Cálculo AWG

        private int CalcularAWG(double seccionMM2)
        {
            var awgTable = new (int awg, double mm2)[]
            {
                (0, 53.49), (1, 42.41), (2, 33.62), (3, 26.67), (4, 21.15),
                (5, 16.77), (6, 13.30), (7, 10.55), (8, 8.37), (9, 6.63),
                (10, 5.26), (11, 4.17), (12, 3.31), (13, 2.62), (14, 2.08),
                (15, 1.65), (16, 1.31), (17, 1.04), (18, 0.823), (19, 0.653),
                (20, 0.518), (21, 0.410), (22, 0.326), (23, 0.258), (24, 0.205),
                (25, 0.162), (26, 0.129), (27, 0.102), (28, 0.081), (29, 0.0642),
                (30, 0.0509)
            };

            for (int i = 0; i < awgTable.Length; i++)
            {
                if (seccionMM2 >= awgTable[i].mm2)
                {
                    DebugLogger.Log("AWG", "Sección {0:F3} mm² -> AWG {1} ({2:F3} mm²)",
                        seccionMM2, awgTable[i].awg, awgTable[i].mm2);
                    return awgTable[i].awg;
                }
            }

            DebugLogger.Log("AWG", "Sección {0:F3} mm² -> AWG 30 (mínimo)", seccionMM2);
            return 30;
        }

        private double ObtenerSeccionAWG(int awg)
        {
            var secciones = new Dictionary<int, double>
            {
                {0, 53.49}, {1, 42.41}, {2, 33.62}, {3, 26.67}, {4, 21.15},
                {5, 16.77}, {6, 13.30}, {7, 10.55}, {8, 8.37}, {9, 6.63},
                {10, 5.26}, {11, 4.17}, {12, 3.31}, {13, 2.62}, {14, 2.08},
                {15, 1.65}, {16, 1.31}, {17, 1.04}, {18, 0.823}, {19, 0.653},
                {20, 0.518}, {21, 0.410}, {22, 0.326}, {23, 0.258}, {24, 0.205},
                {25, 0.162}, {26, 0.129}, {27, 0.102}, {28, 0.081}, {29, 0.0642},
                {30, 0.0509}
            };

            return secciones.ContainsKey(awg) ? secciones[awg] : 1.0;
        }

        private double ObtenerDiametroAWG(int awg)
        {
            var diametros = new Dictionary<int, double>
            {
                {0, 8.25}, {1, 7.35}, {2, 6.54}, {3, 5.83}, {4, 5.19},
                {5, 4.62}, {6, 4.11}, {7, 3.67}, {8, 3.26}, {9, 2.91},
                {10, 2.59}, {11, 2.30}, {12, 2.05}, {13, 1.83}, {14, 1.63},
                {15, 1.45}, {16, 1.29}, {17, 1.15}, {18, 1.02}, {19, 0.912},
                {20, 0.812}, {21, 0.723}, {22, 0.644}, {23, 0.573}, {24, 0.511},
                {25, 0.455}, {26, 0.405}, {27, 0.361}, {28, 0.321}, {29, 0.286},
                {30, 0.255}
            };

            return diametros.ContainsKey(awg) ? diametros[awg] : 1.0;
        }

        #endregion

        #region Métodos de Cálculo de Pérdidas

        private double CalcularPerdidaCobre(double espiras, double corriente, double seccionMM2,
            double longitudMediaCm, double resistividad)
        {
            double longitudTotalM = (espiras * longitudMediaCm) / 100.0;
            double seccionM2 = seccionMM2 * 1e-6;
            double resistencia = resistividad * longitudTotalM / seccionM2;
            double perdida = corriente * corriente * resistencia;

            DebugLogger.Log("PERDIDA_CU", "N={0}, I={1:F2}A, S={2:F3}mm², Lm={3:F2}cm, Lt={4:F2}m, R={5:F4}Ω, P={6:F2}W",
                espiras, corriente, seccionMM2, longitudMediaCm, longitudTotalM, resistencia, perdida);

            return perdida;
        }

        private double CalcularPerdidaNucleo(double volumenCm3, double densidadFlujotesla, double frecuenciaHz)
        {
            double kh = 0, ke = 0, espesor = 0, exponente = 1.6;

            switch (MaterialNucleo)
            {
                case 0: // M19
                    kh = 0.0045; ke = 0.0008; espesor = 0.014 * 2.54; exponente = 1.7;
                    break;
                case 1: // M27
                    kh = 0.0055; ke = 0.0010; espesor = 0.011 * 2.54; exponente = 1.7;
                    break;
                case 2: // Grano orientado
                    kh = 0.0030; ke = 0.0005; espesor = 0.012 * 2.54; exponente = 1.6;
                    break;
                case 3: // Ferrita
                    kh = 0.010; ke = 0.0003; espesor = 0; exponente = 2.0;
                    break;
            }

            double perdidaHisteresis = kh * frecuenciaHz * Math.Pow(densidadFlujotesla, exponente) * volumenCm3;
            double perdidaParasitas = ke * frecuenciaHz * frecuenciaHz * densidadFlujotesla * densidadFlujotesla
                * espesor * espesor * volumenCm3;

            double perdidaTotal = perdidaHisteresis + perdidaParasitas;

            DebugLogger.Log("PERDIDA_FE", "Vol={0:F2}cm³, B={1:F3}T, f={2}Hz, Ph={3:F2}W, Pe={4:F2}W, Total={5:F2}W",
                volumenCm3, densidadFlujotesla, frecuenciaHz, perdidaHisteresis, perdidaParasitas, perdidaTotal);

            return perdidaTotal;
        }

        #endregion

        #region Métodos de Análisis Térmico

        private double CalcularSuperficieRadiacion(double areaNucleoCm2)
        {
            double lado = Math.Sqrt(areaNucleoCm2);

            double superficie = TipoNucleo switch
            {
                0 => lado * lado * 6 * 1.2,  // E-I
                1 => Math.PI * lado * lado * 3,  // Toroidal
                2 => lado * lado * 5 * 1.3,  // C-Core
                3 => lado * lado * 6 * 1.15, // UI
                _ => lado * lado * 6
            };

            return superficie;
        }

        private double CalcularElevacionTemperatura(double perdidaW, double superficieCm2)
        {
            double coeficienteConveccion = 0.0008;
            double deltaT = perdidaW / (coeficienteConveccion * superficieCm2);

            DebugLogger.Log("TERMICA", "P={0:F2}W, A={1:F0}cm², h={2:F6}, ΔT={3:F1}°C",
                perdidaW, superficieCm2, coeficienteConveccion, deltaT);

            return deltaT;
        }

        private double CalcularRegulacion(double espirasPrim, double espirasSecundario,
            double seccionPrim, double seccionSec, double longMediaPrim, double longMediaSec,
            double corrientePrim, double corrienteSec, double resistividad)
        {
            double resistenciaPrim = resistividad * (espirasPrim * longMediaPrim / 100) / (seccionPrim * 1e-6);
            double resistenciaSec = resistividad * (espirasSecundario * longMediaSec / 100) / (seccionSec * 1e-6);

            double relacionTransformacion = espirasSecundario / espirasPrim;
            double resistenciaEquivalente = resistenciaPrim * relacionTransformacion * relacionTransformacion + resistenciaSec;

            double caidaVoltaje = corrienteSec * resistenciaEquivalente;
            double voltajeSecundario = espirasSecundario * (100.0 / espirasPrim) * 10;

            double regulacion = (caidaVoltaje / voltajeSecundario) * 100;

            DebugLogger.Log("REGULACION", "Req={0:F4}Ω, ΔV={1:F2}V, Reg={2:F2}%",
                resistenciaEquivalente, caidaVoltaje, regulacion);

            return regulacion;
        }

        #endregion

        #region Métodos de Recomendaciones

        private string ObtenerClaseAislamiento(double tempOperacion)
        {
            if (tempOperacion < 105) return "A (105°C) - Papel, algodón";
            if (tempOperacion < 130) return "B (130°C) - Mica, fibra vidrio";
            if (tempOperacion < 155) return "F (155°C) - Mica, asbesto, fibra vidrio";
            if (tempOperacion < 180) return "H (180°C) - Silicona, mica";
            return "C (>180°C) - Cerámica, mica, cuarzo";
        }

        private string ObtenerTipoVentilacion(double tempOperacion)
        {
            if (tempOperacion < 60) return "Natural (sin ventilador)";
            if (tempOperacion < 80) return "Natural mejorada (aletas)";
            if (tempOperacion < 100) return "Forzada (ventilador)";
            return "Forzada + Refrigeración activa";
        }

        private string ObtenerAislamientoRecomendado(double voltaje)
        {
            if (voltaje < 250) return "Papel kraft 0.05mm o barniz aislante";
            if (voltaje < 600) return "Papel kraft 0.08mm + barniz";
            if (voltaje < 1000) return "Presspahn 0.3mm";
            return "Presspahn 0.5mm + múltiples capas";
        }

        private string ObtenerAislamientoPrimSec(double voltajePrim, double voltajeSec)
        {
            double voltajeMax = Math.Max(voltajePrim, voltajeSec);

            if (voltajeMax < 250) return "3 capas papel kraft + cinta aislante";
            if (voltajeMax < 600) return "Presspahn 0.5mm + papel kraft";
            if (voltajeMax < 1000) return "Presspahn 1.0mm + cinta Nomex";
            return "Presspahn 1.5mm + múltiples barreras + barniz";
        }

        private string ObtenerMaterialAislante(double temperatura)
        {
            if (temperatura < 105) return "Barniz acrílico, papel kraft";
            if (temperatura < 130) return "Barniz poliéster, papel Nomex";
            if (temperatura < 155) return "Barniz poliamida-imida, fibra vidrio";
            return "Barniz silicona, mica, Kapton";
        }

        #endregion
    }
}*/