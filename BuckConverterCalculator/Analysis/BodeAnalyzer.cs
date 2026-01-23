using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace BuckConverterCalculator.Analysis
{
    /// <summary>
    /// Analizador de diagrama de Bode para estabilidad del lazo de control
    /// </summary>
    public class BodeAnalyzer
    {
        public double Inductance { get; set; }
        public double Capacitance { get; set; }
        public double ESR { get; set; }
        public double LoadResistance { get; set; }
        public CompensationNetwork Compensation { get; set; }
        public double InputVoltage { get; set; } = 90;
        public double OutputVoltage { get; set; } = 13;

        /// <summary>
        /// Genera el diagrama de Bode completo
        /// </summary>
        public BodePlot GenerateBodePlot(double startFreq, double endFreq, int points)
        {
            var frequencies = LogSpace(startFreq, endFreq, points);
            var magnitudes = new List<double>();
            var phases = new List<double>();

            foreach (var freq in frequencies)
            {
                var response = EvaluateTransferFunction(freq);
                magnitudes.Add(20 * Math.Log10(response.Magnitude));
                phases.Add(response.Phase * 180 / Math.PI);
            }

            var plot = new BodePlot
            {
                Frequencies = frequencies,
                MagnitudeDB = magnitudes,
                PhaseDegrees = phases
            };

            // Calcular márgenes
            plot.PhaseMargin = CalculatePhaseMargin(frequencies, magnitudes, phases);
            plot.GainMargin = CalculateGainMargin(frequencies, magnitudes, phases);
            plot.CrossoverFrequency = FindCrossoverFrequency(frequencies, magnitudes);
            plot.PhaseAt180 = FindPhaseAt180Frequency(frequencies, phases);

            return plot;
        }

        /// <summary>
        /// Evalúa la función de transferencia en una frecuencia específica
        /// </summary>
        private Complex EvaluateTransferFunction(double frequency)
        {
            double omega = 2 * Math.PI * frequency;
            Complex s = new Complex(0, omega);

            // Power stage transfer function del Buck converter
            // Gvd(s) = Vin / (L*C*s^2 + (L/R + ESR*C)*s + (1 + ESR/R))

            double L = Inductance;
            double C = Capacitance;
            double R = LoadResistance;
            double rc = ESR;

            // Numerator: Vin
            Complex numerator = InputVoltage;

            // Denominator: L*C*s^2 + (L/R + ESR*C)*s + (1 + ESR/R)
            Complex s2 = Complex.Pow(s, 2);
            Complex denominator = L * C * s2 +
                                 (L / R + rc * C) * s +
                                 (1 + rc / R);

            Complex powerStage = numerator / denominator;

            // Si hay red de compensación, multiplicar
            if (Compensation != null)
            {
                Complex compensation = EvaluateCompensation(s);
                return powerStage * compensation;
            }

            return powerStage;
        }

        /// <summary>
        /// Evalúa la red de compensación
        /// </summary>
        private Complex EvaluateCompensation(Complex s)
        {
            if (Compensation == null) return Complex.One;

            double wz = 2 * Math.PI * Compensation.ZeroFrequency;
            double wp = 2 * Math.PI * Compensation.PoleFrequency;
            double K = Compensation.Gain;

            if (Compensation.Type == CompensationType.TypeI)
            {
                // Type I: Simple integrator
                // H(s) = K/s
                return K / s;
            }
            else if (Compensation.Type == CompensationType.TypeII)
            {
                // Type II: Pole at origin + zero + pole
                // H(s) = K * (1 + s/wz) / (s * (1 + s/wp))

                Complex numerator = K * (1 + s / wz);
                Complex denominator = s * (1 + s / wp);

                return numerator / denominator;
            }
            else if (Compensation.Type == CompensationType.TypeIII)
            {
                // Type III: Pole at origin + 2 zeros + 2 poles
                double wz2 = 2 * Math.PI * Compensation.SecondZeroFrequency;
                double wp2 = 2 * Math.PI * Compensation.SecondPoleFrequency;

                Complex numerator = K * (1 + s / wz) * (1 + s / wz2);
                Complex denominator = s * (1 + s / wp) * (1 + s / wp2);

                return numerator / denominator;
            }

            return Complex.One;
        }

        /// <summary>
        /// Calcula el margen de fase
        /// </summary>
        private double CalculatePhaseMargin(List<double> freqs, List<double> mags, List<double> phases)
        {
            // Encontrar frecuencia donde magnitud = 0 dB
            int crossoverIndex = -1;
            for (int i = 0; i < mags.Count - 1; i++)
            {
                if (mags[i] >= 0 && mags[i + 1] < 0)
                {
                    crossoverIndex = i;
                    break;
                }
            }

            if (crossoverIndex == -1) return double.NaN;

            // Interpolar para obtener fase exacta en crossover
            double ratio = -mags[crossoverIndex] / (mags[crossoverIndex + 1] - mags[crossoverIndex]);
            double phaseAtCrossover = phases[crossoverIndex] + ratio * (phases[crossoverIndex + 1] - phases[crossoverIndex]);

            // Phase margin = 180° + phase at crossover
            return 180 + phaseAtCrossover;
        }

        /// <summary>
        /// Calcula el margen de ganancia
        /// </summary>
        private double CalculateGainMargin(List<double> freqs, List<double> mags, List<double> phases)
        {
            // Encontrar frecuencia donde phase = -180°
            int phaseIndex = -1;
            for (int i = 0; i < phases.Count - 1; i++)
            {
                if (phases[i] > -180 && phases[i + 1] <= -180)
                {
                    phaseIndex = i;
                    break;
                }
            }

            if (phaseIndex == -1) return double.NaN;

            // Interpolar magnitud en -180°
            double ratio = (-180 - phases[phaseIndex]) / (phases[phaseIndex + 1] - phases[phaseIndex]);
            double magAt180 = mags[phaseIndex] + ratio * (mags[phaseIndex + 1] - mags[phaseIndex]);

            // Gain margin = -magnitude at phase crossover
            return -magAt180;
        }

        /// <summary>
        /// Encuentra la frecuencia de cruce por cero (0 dB)
        /// </summary>
        private double FindCrossoverFrequency(List<double> freqs, List<double> mags)
        {
            for (int i = 0; i < mags.Count - 1; i++)
            {
                if (mags[i] >= 0 && mags[i + 1] < 0)
                {
                    // Interpolación lineal
                    double ratio = -mags[i] / (mags[i + 1] - mags[i]);
                    return freqs[i] + ratio * (freqs[i + 1] - freqs[i]);
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Encuentra la frecuencia donde la fase cruza -180°
        /// </summary>
        private double FindPhaseAt180Frequency(List<double> freqs, List<double> phases)
        {
            for (int i = 0; i < phases.Count - 1; i++)
            {
                if (phases[i] > -180 && phases[i + 1] <= -180)
                {
                    double ratio = (-180 - phases[i]) / (phases[i + 1] - phases[i]);
                    return freqs[i] + ratio * (freqs[i + 1] - freqs[i]);
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Genera puntos logarítmicamente espaciados
        /// </summary>
        private List<double> LogSpace(double start, double end, int points)
        {
            var result = new List<double>();
            double logStart = Math.Log10(start);
            double logEnd = Math.Log10(end);
            double step = (logEnd - logStart) / (points - 1);

            for (int i = 0; i < points; i++)
            {
                result.Add(Math.Pow(10, logStart + i * step));
            }

            return result;
        }

        /// <summary>
        /// Diseña automáticamente una red de compensación Type II
        /// </summary>
        public CompensationNetwork DesignTypeIICompensation(double targetCrossoverFreq, double targetPhaseMargin)
        {
            // Evaluar ganancia del power stage en la frecuencia de cruce deseada
            var powerStageResponse = EvaluatePowerStageOnly(targetCrossoverFreq);
            double powerStageGain = 20 * Math.Log10(powerStageResponse.Magnitude);

            // Ganancia del compensador = -ganancia del power stage
            double compensatorGain = Math.Pow(10, -powerStageGain / 20);

            // Colocar zero en frecuencia natural del filtro LC
            double wn = 1 / Math.Sqrt(Inductance * Capacitance);
            double zeroFreq = wn / (2 * Math.PI);

            // Colocar polo para obtener el margen de fase deseado
            // Típicamente 5-10 veces la frecuencia de cruce
            double poleFreq = targetCrossoverFreq * 10;

            return new CompensationNetwork
            {
                Type = CompensationType.TypeII,
                Gain = compensatorGain,
                ZeroFrequency = zeroFreq,
                PoleFrequency = poleFreq
            };
        }

        /// <summary>
        /// Evalúa solo el power stage sin compensación
        /// </summary>
        private Complex EvaluatePowerStageOnly(double frequency)
        {
            double omega = 2 * Math.PI * frequency;
            Complex s = new Complex(0, omega);

            double L = Inductance;
            double C = Capacitance;
            double R = LoadResistance;
            double rc = ESR;

            Complex numerator = InputVoltage;
            Complex s2 = Complex.Pow(s, 2);
            Complex denominator = L * C * s2 + (L / R + rc * C) * s + (1 + rc / R);

            return numerator / denominator;
        }
    }

    /// <summary>
    /// Red de compensación
    /// </summary>
    [Serializable]
    public class CompensationNetwork
    {
        public CompensationType Type { get; set; }
        public double Gain { get; set; }
        public double ZeroFrequency { get; set; }
        public double PoleFrequency { get; set; }
        public double SecondZeroFrequency { get; set; }
        public double SecondPoleFrequency { get; set; }
    }

    /// <summary>
    /// Tipo de compensación
    /// </summary>
    public enum CompensationType
    {
        TypeI,   // Integrador simple
        TypeII,  // Polo en origen + cero + polo
        TypeIII  // Polo en origen + 2 ceros + 2 polos
    }

    /// <summary>
    /// Resultado del diagrama de Bode
    /// </summary>
    [Serializable]
    public class BodePlot
    {
        public List<double> Frequencies { get; set; }
        public List<double> MagnitudeDB { get; set; }
        public List<double> PhaseDegrees { get; set; }
        public double PhaseMargin { get; set; }
        public double GainMargin { get; set; }
        public double CrossoverFrequency { get; set; }
        public double PhaseAt180 { get; set; }

        public bool IsStable()
        {
            return PhaseMargin > 0 && GainMargin > 0;
        }

        public string GetStabilityReport()
        {
            if (!IsStable())
                return "UNSTABLE - System will oscillate";

            if (PhaseMargin < 30)
                return "MARGINAL - Phase margin too low, risk of ringing";

            if (PhaseMargin < 45)
                return "ACCEPTABLE - Adequate phase margin";

            return "GOOD - Good phase margin";
        }
    }
}