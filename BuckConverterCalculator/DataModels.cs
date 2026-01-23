using System;

namespace BuckConverterCalculator.Core
{
    /// <summary>
    /// Parámetros de diseño del convertidor Buck
    /// </summary>
    public class DesignParameters
    {
        public double Vin { get; set; }                    // Tensión de entrada (V)
        public double Vout { get; set; }                   // Tensión de salida (V)
        public double Iout { get; set; }                   // Corriente de salida (A)
        public double Frequency { get; set; }              // Frecuencia de switching (Hz)
        public double RippleCurrentPercent { get; set; }   // Ripple de corriente (% de Iout)
        public double RippleVoltageMv { get; set; }        // Ripple de tensión (mV)
        public double EfficiencyPercent { get; set; }      // Eficiencia estimada (%)
    }

    /// <summary>
    /// Resultados de los cálculos del Buck Converter
    /// </summary>
    public class CalculationResults
    {
        // Parámetros fundamentales
        public double DutyCycle { get; set; }
        public double PowerOutput { get; set; }
        public double PowerInput { get; set; }
        public double ActualEfficiency { get; set; }

        // Inductor
        public double Inductance { get; set; }              // Inductancia calculada (H)
        public double InductanceCommercial { get; set; }    // Valor comercial (H)
        public double RippleCurrent { get; set; }           // ΔIL (A)
        public double PeakInductorCurrent { get; set; }     // Corriente pico (A)
        public double RmsInductorCurrent { get; set; }      // Corriente RMS (A)

        // Capacitor de salida
        public double OutputCapacitance { get; set; }           // Capacitancia calculada (F)
        public double OutputCapacitanceCommercial { get; set; } // Valor comercial (F)
        public double MaxEsr { get; set; }                      // ESR máximo (Ω)
        public double RmsCapacitorCurrent { get; set; }         // Corriente RMS (A)

        // Divisor de realimentación
        public double FeedbackR1 { get; set; }              // Resistor superior (Ω)
        public double FeedbackR2 { get; set; }              // Resistor inferior (Ω)
        public double VoutVerified { get; set; }            // Vout verificado (V)

        // Configuración PWM UC3843
        public double RtValue { get; set; }                 // Resistor timing (Ω)
        public double CtValue { get; set; }                 // Capacitor timing (F)
        public double ActualFrequency { get; set; }         // Frecuencia real (Hz)

        // Pérdidas
        public double MosfetConductionLoss { get; set; }    // Pérdidas MOSFET conducción (W)
        public double MosfetSwitchingLoss { get; set; }     // Pérdidas MOSFET switching (W)
        public double MosfetTotalLoss { get; set; }         // Pérdidas MOSFET total (W)
        public double DiodeConductionLoss { get; set; }     // Pérdidas diodo (W)
        public double InductorCoreLoss { get; set; }        // Pérdidas inductor (W)
        public double TotalLosses { get; set; }             // Pérdidas totales (W)

        // Análisis térmico
        public double MosfetJunctionTemp { get; set; }      // Temperatura juntura MOSFET (°C)
        public double DiodeJunctionTemp { get; set; }       // Temperatura juntura diodo (°C)
        public double InductorTemp { get; set; }            // Temperatura inductor (°C)
    }
}