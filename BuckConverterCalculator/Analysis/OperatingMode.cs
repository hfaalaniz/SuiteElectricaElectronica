using System;

namespace BuckConverterCalculator.Analysis
{
    /// <summary>
    /// Modos de operación del convertidor
    /// </summary>
    public enum OperatingMode
    {
        /// <summary>
        /// Continuous Conduction Mode - Corriente del inductor nunca llega a cero
        /// </summary>
        CCM,

        /// <summary>
        /// Discontinuous Conduction Mode - Corriente del inductor llega a cero
        /// </summary>
        DCM,

        /// <summary>
        /// Boundary Conduction Mode - Corriente del inductor justo toca cero
        /// </summary>
        BCM
    }

    /// <summary>
    /// Parámetros calculados en modo DCM
    /// </summary>
    [Serializable]
    public class DCMParameters
    {
        /// <summary>
        /// Duty cycle D1 (tiempo ON del switch)
        /// </summary>
        public double DutyCycle { get; set; }

        /// <summary>
        /// Duty cycle D2 (tiempo de descarga del inductor)
        /// </summary>
        public double DischargeDuty { get; set; }

        /// <summary>
        /// Tiempo muerto D3 (inductor en corriente cero)
        /// </summary>
        public double DeadTime { get; set; }

        /// <summary>
        /// Corriente pico del inductor (A)
        /// </summary>
        public double PeakInductorCurrent { get; set; }

        /// <summary>
        /// Ripple de voltaje de salida (V)
        /// </summary>
        public double OutputRipple { get; set; }

        /// <summary>
        /// Modo de operación detectado
        /// </summary>
        public OperatingMode Mode { get; set; }

        /// <summary>
        /// Corriente promedio del inductor (A)
        /// </summary>
        public double AverageInductorCurrent { get; set; }

        /// <summary>
        /// Corriente RMS del inductor (A)
        /// </summary>
        public double RMSInductorCurrent { get; set; }

        // AGREGAR ESTA PROPIEDAD SI NO EXISTE:
        public double RMSCurrent
        {
            get { return RMSInductorCurrent; }
            set { RMSInductorCurrent = value; }
        }

        public override string ToString()
        {
            return $"Mode: {Mode}, D1={DutyCycle:F3}, D2={DischargeDuty:F3}, D3={DeadTime:F3}, Ipeak={PeakInductorCurrent:F3}A";
        }
    }

    /*
    */

    /// <summary>
    /// Recomendación de inductor para evitar o permitir DCM
    /// </summary>
    [Serializable]
    public class InductorRecommendation
    {
        /// <summary>
        /// Inductancia mínima para evitar DCM (H)
        /// </summary>
        public double MinimumInductance { get; set; }

        /// <summary>
        /// Inductancia recomendada con margen (H)
        /// </summary>
        public double RecommendedInductance { get; set; }

        /// <summary>
        /// Corriente de saturación requerida (A)
        /// </summary>
        public double SaturationCurrent { get; set; }

        /// <summary>
        /// Corriente RMS para cálculo de pérdidas (A)
        /// </summary>
        public double RMSCurrent { get; set; }

        /// <summary>
        /// DCR máximo recomendado (Ω)
        /// </summary>
        public double MaxDCR { get; set; }

        /// <summary>
        /// Razón por la que se recomienda este valor
        /// </summary>
        public string Reasoning { get; set; }

        public override string ToString()
        {
            return $"L_min={MinimumInductance * 1e6:F1}µH, L_rec={RecommendedInductance * 1e6:F1}µH, Isat={SaturationCurrent:F2}A";
        }
    }

    /// <summary>
    /// Condiciones de operación del convertidor
    /// </summary>
    [Serializable]
    public class OperatingConditions
    {
        public double InputVoltageMin { get; set; }
        public double InputVoltageMax { get; set; }
        public double OutputVoltage { get; set; }
        public double LoadCurrentMin { get; set; }
        public double LoadCurrentMax { get; set; }
        public double SwitchingFrequency { get; set; }
        public double AmbientTemperature { get; set; } = 25; // °C

        public bool IsValid()
        {
            return InputVoltageMin > 0 &&
                   InputVoltageMax >= InputVoltageMin &&
                   OutputVoltage > 0 &&
                   LoadCurrentMax >= LoadCurrentMin &&
                   LoadCurrentMin >= 0 &&
                   SwitchingFrequency > 0;
        }
    }

    
}