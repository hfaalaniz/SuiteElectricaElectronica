using System;
using System.Collections.Generic;
using System.Linq;

namespace BuckConverterCalculator.Simulation
{
    /// <summary>
    /// Simulador de formas de onda temporal para convertidor Buck
    /// </summary>
    public class WaveformSimulator
    {
        public CircuitParameters Circuit { get; set; }
        public SimulationSettings Settings { get; set; }

        private double currentInductor = 0;
        private double voltageCapacitor = 0;

        /// <summary>
        /// Ejecuta la simulación completa
        /// </summary>
        public SimulationResults RunSimulation()
        {
            if (Circuit == null || Settings == null)
                throw new InvalidOperationException("Circuit parameters and settings must be set");

            var results = new SimulationResults
            {
                Time = new List<double>(),
                InductorCurrent = new List<double>(),
                OutputVoltage = new List<double>(),
                SwitchVoltage = new List<double>(),
                DiodeVoltage = new List<double>(),
                SwitchCurrent = new List<double>(),
                DiodeCurrent = new List<double>()
            };

            // Reset estado inicial
            currentInductor = 0;
            voltageCapacitor = 0;

            double dt = 1.0 / (Settings.SamplesPerCycle * Circuit.SwitchingFrequency);
            int totalSamples = (int)(Settings.SimulationTime / dt);

            // Permitir estabilización
            int stabilizationSamples = (int)(Settings.StabilizationTime / dt);

            for (int i = -stabilizationSamples; i < totalSamples; i++)
            {
                double t = i * dt;

                // Determinar estado del switch
                double cycleTime = (t % (1.0 / Circuit.SwitchingFrequency));
                if (cycleTime < 0) cycleTime += 1.0 / Circuit.SwitchingFrequency;

                double dutyCycleTime = Circuit.DutyCycle / Circuit.SwitchingFrequency;
                bool switchOn = cycleTime < dutyCycleTime;

                // Integrar ecuaciones diferenciales
                IntegrateOneStep(dt, switchOn);

                // Solo guardar después de estabilización
                if (i >= 0)
                {
                    results.Time.Add(t);
                    results.InductorCurrent.Add(currentInductor);
                    results.OutputVoltage.Add(voltageCapacitor);

                    if (switchOn)
                    {
                        results.SwitchVoltage.Add(currentInductor * Circuit.SwitchRDSon);
                        results.SwitchCurrent.Add(currentInductor);
                        results.DiodeVoltage.Add(-Circuit.InputVoltage);
                        results.DiodeCurrent.Add(0);
                    }
                    else
                    {
                        results.SwitchVoltage.Add(Circuit.InputVoltage);
                        results.SwitchCurrent.Add(0);
                        results.DiodeVoltage.Add(Circuit.DiodeVF);
                        results.DiodeCurrent.Add(currentInductor);
                    }
                }
            }

            // Calcular métricas
            results.CalculateMetrics();

            return results;
        }

        /// <summary>
        /// Integra un paso de tiempo usando método Runge-Kutta 4
        /// </summary>
        private void IntegrateOneStep(double dt, bool switchOn)
        {
            if (Settings.UseRK4)
            {
                IntegrateRK4(dt, switchOn);
            }
            else
            {
                IntegrateEuler(dt, switchOn);
            }
        }

        /// <summary>
        /// Integración Euler (simple pero menos precisa)
        /// </summary>
        private void IntegrateEuler(double dt, bool switchOn)
        {
            var derivatives = CalculateDerivatives(currentInductor, voltageCapacitor, switchOn);

            currentInductor += derivatives.Item1 * dt;
            voltageCapacitor += derivatives.Item2 * dt;

            // Verificar DCM
            if (currentInductor < 0)
                currentInductor = 0;
        }

        /// <summary>
        /// Integración Runge-Kutta 4 (más precisa)
        /// </summary>
        private void IntegrateRK4(double dt, bool switchOn)
        {
            // k1
            var k1 = CalculateDerivatives(currentInductor, voltageCapacitor, switchOn);

            // k2
            double iL_k2 = currentInductor + 0.5 * dt * k1.Item1;
            double vC_k2 = voltageCapacitor + 0.5 * dt * k1.Item2;
            var k2 = CalculateDerivatives(iL_k2, vC_k2, switchOn);

            // k3
            double iL_k3 = currentInductor + 0.5 * dt * k2.Item1;
            double vC_k3 = voltageCapacitor + 0.5 * dt * k2.Item2;
            var k3 = CalculateDerivatives(iL_k3, vC_k3, switchOn);

            // k4
            double iL_k4 = currentInductor + dt * k3.Item1;
            double vC_k4 = voltageCapacitor + dt * k3.Item2;
            var k4 = CalculateDerivatives(iL_k4, vC_k4, switchOn);

            // Combinar
            currentInductor += (dt / 6.0) * (k1.Item1 + 2 * k2.Item1 + 2 * k3.Item1 + k4.Item1);
            voltageCapacitor += (dt / 6.0) * (k1.Item2 + 2 * k2.Item2 + 2 * k3.Item2 + k4.Item2);

            // Verificar DCM
            if (currentInductor < 0)
                currentInductor = 0;
        }

        /// <summary>
        /// Calcula las derivadas diL/dt y dvC/dt
        /// </summary>
        private Tuple<double, double> CalculateDerivatives(double iL, double vC, bool switchOn)
        {
            double diL_dt, dvC_dt;

            if (switchOn)
            {
                // Switch ON: Vin -> L -> C -> Load
                // diL/dt = (Vin - vC - iL*rL - iL*rDS) / L
                // dvC/dt = (iL - vC/R - vC/(rC)) / C

                if (Settings.IncludeParasitics)
                {
                    double vL = Circuit.InputVoltage - vC -
                               iL * Circuit.InductorESR -
                               iL * Circuit.SwitchRDSon;
                    diL_dt = vL / Circuit.Inductance;

                    double iC = iL - vC / Circuit.LoadResistance;
                    dvC_dt = iC / Circuit.Capacitance -
                            (vC / (Circuit.CapacitorESR * Circuit.Capacitance));
                }
                else
                {
                    diL_dt = (Circuit.InputVoltage - vC) / Circuit.Inductance;
                    dvC_dt = (iL - vC / Circuit.LoadResistance) / Circuit.Capacitance;
                }
            }
            else
            {
                // Switch OFF: L -> Diode -> C -> Load
                // diL/dt = (-vC - iL*rL - VF) / L
                // dvC/dt = (iL - vC/R) / C

                if (iL < 1e-6) // DCM - corriente cero
                {
                    diL_dt = 0;
                    dvC_dt = -vC / (Circuit.LoadResistance * Circuit.Capacitance);
                }
                else
                {
                    if (Settings.IncludeParasitics)
                    {
                        double vL = -vC - iL * Circuit.InductorESR - Circuit.DiodeVF;
                        diL_dt = vL / Circuit.Inductance;

                        double iC = iL - vC / Circuit.LoadResistance;
                        dvC_dt = iC / Circuit.Capacitance;
                    }
                    else
                    {
                        diL_dt = (-vC - Circuit.DiodeVF) / Circuit.Inductance;
                        dvC_dt = (iL - vC / Circuit.LoadResistance) / Circuit.Capacitance;
                    }
                }
            }

            return Tuple.Create(diL_dt, dvC_dt);
        }
    }

    /// <summary>
    /// Parámetros del circuito
    /// </summary>
    [Serializable]
    public class CircuitParameters
    {
        public double InputVoltage { get; set; }
        public double DutyCycle { get; set; }
        public double SwitchingFrequency { get; set; }
        public double Inductance { get; set; }
        public double InductorESR { get; set; } = 0.02;
        public double Capacitance { get; set; }
        public double CapacitorESR { get; set; } = 0.005;
        public double LoadResistance { get; set; }
        public double SwitchRDSon { get; set; } = 0.05;
        public double DiodeVF { get; set; } = 0.5;
    }

    /// <summary>
    /// Configuración de la simulación
    /// </summary>
    [Serializable]
    public class SimulationSettings
    {
        public double SimulationTime { get; set; } = 0.001; // 1ms
        public double StabilizationTime { get; set; } = 0.0005; // 0.5ms
        public int SamplesPerCycle { get; set; } = 100;
        public bool IncludeParasitics { get; set; } = true;
        public bool UseRK4 { get; set; } = true; // true=RK4, false=Euler
    }

    /// <summary>
    /// Resultados de la simulación
    /// </summary>
    [Serializable]
    public class SimulationResults
    {
        public List<double> Time { get; set; }
        public List<double> InductorCurrent { get; set; }
        public List<double> OutputVoltage { get; set; }
        public List<double> SwitchVoltage { get; set; }
        public List<double> DiodeVoltage { get; set; }
        public List<double> SwitchCurrent { get; set; }
        public List<double> DiodeCurrent { get; set; }

        // Métricas calculadas
        public double AverageOutputVoltage { get; private set; }
        public double OutputRipple { get; private set; }
        public double PeakInductorCurrent { get; private set; }
        public double AverageInductorCurrent { get; private set; }
        public double InductorCurrentRipple { get; private set; }
        public double RMSInductorCurrent { get; private set; }

        public void CalculateMetrics()
        {
            if (OutputVoltage == null || OutputVoltage.Count == 0) return;

            // Promedios
            AverageOutputVoltage = OutputVoltage.Average();
            AverageInductorCurrent = InductorCurrent.Average();

            // Picos y ripple
            PeakInductorCurrent = InductorCurrent.Max();
            double minVout = OutputVoltage.Min();
            double maxVout = OutputVoltage.Max();
            OutputRipple = maxVout - minVout;

            double minIL = InductorCurrent.Min();
            double maxIL = InductorCurrent.Max();
            InductorCurrentRipple = maxIL - minIL;

            // RMS
            double sumSquares = 0;
            foreach (var i in InductorCurrent)
            {
                sumSquares += i * i;
            }
            RMSInductorCurrent = Math.Sqrt(sumSquares / InductorCurrent.Count);
        }

        /// <summary>
        /// Exporta resultados a CSV
        /// </summary>
        public void ExportToCSV(string filename)
        {
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("Time(s),InductorCurrent(A),OutputVoltage(V),SwitchVoltage(V),DiodeVoltage(V),SwitchCurrent(A),DiodeCurrent(A)");

                for (int i = 0; i < Time.Count; i++)
                {
                    writer.WriteLine($"{Time[i]:E6},{InductorCurrent[i]:F6},{OutputVoltage[i]:F6},{SwitchVoltage[i]:F6},{DiodeVoltage[i]:F6},{SwitchCurrent[i]:F6},{DiodeCurrent[i]:F6}");
                }
            }
        }

        /// <summary>
        /// Obtiene un resumen de los resultados
        /// </summary>
        public string GetSummary()
        {
            return $"Simulation Summary:\n" +
                   $"  Average Output Voltage: {AverageOutputVoltage:F3} V\n" +
                   $"  Output Ripple: {OutputRipple * 1000:F2} mV\n" +
                   $"  Peak Inductor Current: {PeakInductorCurrent:F3} A\n" +
                   $"  Average Inductor Current: {AverageInductorCurrent:F3} A\n" +
                   $"  RMS Inductor Current: {RMSInductorCurrent:F3} A\n" +
                   $"  Inductor Current Ripple: {InductorCurrentRipple:F3} A";
        }
    }
}