using System;

namespace BuckConverterCalculator.Analysis
{
    /// <summary>
    /// Analizador de modo de conducción discontinua (DCM) para convertidor Buck
    /// </summary>
    public class DCMAnalyzer
    {
        public double InputVoltage { get; set; }
        public double OutputVoltage { get; set; }
        public double OutputCurrent { get; set; }
        public double SwitchingFrequency { get; set; }
        public double Inductance { get; set; }
        public double Capacitance { get; set; } = 10e-6; // Default 10µF

        /// <summary>
        /// Detecta el modo de operación (CCM o DCM)
        /// </summary>
        public OperatingMode DetectMode()
        {
            double criticalCurrent = CalculateBoundaryCurrent();

            if (Math.Abs(OutputCurrent - criticalCurrent) < 0.001)
                return OperatingMode.BCM;
            else if (OutputCurrent < criticalCurrent)
                return OperatingMode.DCM;
            else
                return OperatingMode.CCM;
        }

        /// <summary>
        /// Calcula la corriente crítica que marca el límite entre CCM y DCM
        /// </summary>
        /// <returns>Corriente crítica en amperios</returns>
        public double CalculateBoundaryCurrent()
        {
            // Icrit = (Vo * (Vin - Vo)) / (2 * L * fs * Vin)
            if (Inductance == 0 || SwitchingFrequency == 0 || InputVoltage == 0)
                return 0;

            double numerator = OutputVoltage * (InputVoltage - OutputVoltage);
            double denominator = 2 * Inductance * SwitchingFrequency * InputVoltage;

            return numerator / denominator;
        }

        /// <summary>
        /// Calcula los parámetros completos de operación en DCM
        /// </summary>
        public DCMParameters CalculateDCMParameters()
        {
            var mode = DetectMode();

            if (mode == OperatingMode.CCM)
            {
                return CalculateCCMParameters();
            }

            // Duty cycle ideal
            double dutyCycle = OutputVoltage / InputVoltage;
            double d1 = dutyCycle;

            // Calcular D2 (tiempo de descarga del inductor en DCM)
            // D2 = D1 * (Vin - Vo) / Vo
            double d2 = d1 * (InputVoltage - OutputVoltage) / OutputVoltage;

            // D3 = tiempo muerto (inductor en corriente cero)
            double d3 = 1.0 - d1 - d2;

            // Asegurar que D3 >= 0 (si es negativo, estamos en CCM)
            if (d3 < 0)
            {
                return CalculateCCMParameters();
            }

            // Corriente pico del inductor en DCM
            // Ipeak = 2 * Io / (D1 + D2)
            double peakCurrent = (2 * OutputCurrent) / (d1 + d2);

            // Corriente promedio y RMS
            double avgCurrent = OutputCurrent;
            double rmsCurrent = peakCurrent / Math.Sqrt(3); // Aproximación para forma triangular

            // Ripple de voltaje de salida
            // ΔVo = Ipeak / (8 * C * fs)
            double rippleVoltage = 0;
            if (Capacitance > 0)
            {
                rippleVoltage = peakCurrent / (8 * Capacitance * SwitchingFrequency);
            }

            return new DCMParameters
            {
                DutyCycle = d1,
                DischargeDuty = d2,
                DeadTime = d3,
                PeakInductorCurrent = peakCurrent,
                OutputRipple = rippleVoltage,
                Mode = OperatingMode.DCM,
                AverageInductorCurrent = avgCurrent,
                RMSInductorCurrent = rmsCurrent
            };
        }

        /// <summary>
        /// Calcula parámetros en modo CCM para comparación
        /// </summary>
        private DCMParameters CalculateCCMParameters()
        {
            double dutyCycle = OutputVoltage / InputVoltage;

            // Ripple de corriente en CCM
            // ΔIL = (Vin - Vo) * D / (L * fs)
            double currentRipple = (InputVoltage - OutputVoltage) * dutyCycle /
                                  (Inductance * SwitchingFrequency);

            double peakCurrent = OutputCurrent + currentRipple / 2;
            double avgCurrent = OutputCurrent;

            // Corriente RMS en CCM
            // IRMS = sqrt(Iavg^2 + (ΔIL^2)/12)
            double rmsCurrent = Math.Sqrt(Math.Pow(avgCurrent, 2) + Math.Pow(currentRipple, 2) / 12);

            // Ripple de voltaje
            double rippleVoltage = 0;
            if (Capacitance > 0)
            {
                rippleVoltage = currentRipple / (8 * Capacitance * SwitchingFrequency);
            }

            return new DCMParameters
            {
                DutyCycle = dutyCycle,
                DischargeDuty = 1 - dutyCycle,
                DeadTime = 0,
                PeakInductorCurrent = peakCurrent,
                OutputRipple = rippleVoltage,
                Mode = OperatingMode.CCM,
                AverageInductorCurrent = avgCurrent,
                RMSInductorCurrent = rmsCurrent
            };
        }

        /// <summary>
        /// Optimiza el valor del inductor para operar en un modo específico
        /// </summary>
        /// <param name="targetMode">Modo deseado (CCM o DCM)</param>
        /// <param name="maxRipplePercent">Ripple máximo permitido en porcentaje</param>
        public InductorRecommendation OptimizeInductor(OperatingMode targetMode, double maxRipplePercent = 10)
        {
            var recommendation = new InductorRecommendation();

            // Calcular L mínimo para evitar DCM
            double Lmin = (OutputVoltage * (InputVoltage - OutputVoltage)) /
                         (2 * OutputCurrent * SwitchingFrequency * InputVoltage);

            recommendation.MinimumInductance = Lmin;

            if (targetMode == OperatingMode.CCM)
            {
                // Para CCM, usar L con 50% de margen sobre Lmin
                recommendation.RecommendedInductance = Lmin * 1.5;
                recommendation.Reasoning = "Inductance 50% above critical value to ensure CCM operation";
            }
            else if (targetMode == OperatingMode.DCM)
            {
                // Para DCM intencional, usar L menor que Lmin
                recommendation.RecommendedInductance = Lmin * 0.5;
                recommendation.Reasoning = "Inductance below critical value for intentional DCM operation";
            }
            else // BCM
            {
                recommendation.RecommendedInductance = Lmin;
                recommendation.Reasoning = "Inductance at critical value for boundary mode operation";
            }

            // Calcular corriente de saturación requerida
            // Usar los parámetros con la inductancia recomendada
            this.Inductance = recommendation.RecommendedInductance;
            var parameters = CalculateDCMParameters();

            recommendation.SaturationCurrent = parameters.PeakInductorCurrent * 1.3; // 30% margin
            recommendation.RMSCurrent = parameters.RMSInductorCurrent;

            // DCR máximo (causar máximo 1% de pérdida de eficiencia)
            // Analizar codigo comentado si se desea implementar esta característica
            /*if (parameters.RMSCurrent > 0)
            {
                recommendation.MaxDCR = (OutputVoltage * 0.01) / Math.Pow(parameters.RMSCurrent, 2);
            }*/

            return recommendation;
        }

        /// <summary>
        /// Calcula la eficiencia estimada en el modo actual
        /// </summary>
        public double CalculateEfficiency(double switchRDSon, double diodeVF, double inductorESR)
        {
            var parameters = CalculateDCMParameters();

            // Pérdidas en el switch
            double switchLosses = Math.Pow(parameters.RMSInductorCurrent, 2) * switchRDSon * parameters.DutyCycle;

            // Pérdidas en el diodo
            double diodeLosses = parameters.AverageInductorCurrent * diodeVF * (1 - parameters.DutyCycle);

            // Pérdidas en el inductor (ESR)
            double inductorLosses = Math.Pow(parameters.RMSInductorCurrent, 2) * inductorESR;

            // Potencia de salida
            double outputPower = OutputVoltage * OutputCurrent;

            // Potencia de entrada = salida + pérdidas
            double totalLosses = switchLosses + diodeLosses + inductorLosses;
            double inputPower = outputPower + totalLosses;

            // Eficiencia
            double efficiency = (outputPower / inputPower) * 100;

            return efficiency;
        }

        /// <summary>
        /// Valida que los parámetros del convertidor sean razonables
        /// </summary>
        public bool ValidateParameters(out string errorMessage)
        {
            errorMessage = "";

            if (InputVoltage <= 0)
            {
                errorMessage = "Input voltage must be positive";
                return false;
            }

            if (OutputVoltage <= 0)
            {
                errorMessage = "Output voltage must be positive";
                return false;
            }

            if (OutputVoltage >= InputVoltage)
            {
                errorMessage = "Output voltage must be less than input voltage for Buck converter";
                return false;
            }

            if (OutputCurrent < 0)
            {
                errorMessage = "Output current cannot be negative";
                return false;
            }

            if (SwitchingFrequency <= 0)
            {
                errorMessage = "Switching frequency must be positive";
                return false;
            }

            if (Inductance <= 0)
            {
                errorMessage = "Inductance must be positive";
                return false;
            }

            return true;
        }
    }
}