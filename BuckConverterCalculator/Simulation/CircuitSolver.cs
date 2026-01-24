using System;

namespace BuckConverterCalculator.Simulation
{
    /// <summary>
    /// Solver numérico para ecuaciones diferenciales del circuito
    /// </summary>
    public class CircuitSolver
    {
        public CircuitParameters Circuit { get; set; }
        public SolverMethod Method { get; set; } = SolverMethod.RK4;

        private double currentInductor = 0;
        private double voltageCapacitor = 0;

        /// <summary>
        /// Inicializa el estado del circuito
        /// </summary>
        public void Initialize(double initialInductorCurrent = 0, double initialCapacitorVoltage = 0)
        {
            currentInductor = initialInductorCurrent;
            voltageCapacitor = initialCapacitorVoltage;
        }

        /// <summary>
        /// Avanza un paso de tiempo
        /// </summary>
        public CircuitState Step(double dt, bool switchOn)
        {
            switch (Method)
            {
                case SolverMethod.Euler:
                    return StepEuler(dt, switchOn);
                case SolverMethod.RK4:
                    return StepRK4(dt, switchOn);
                case SolverMethod.RK2:
                    return StepRK2(dt, switchOn);
                default:
                    return StepRK4(dt, switchOn);
            }
        }

        /// <summary>
        /// Método de Euler (primer orden)
        /// </summary>
        private CircuitState StepEuler(double dt, bool switchOn)
        {
            var derivatives = CalculateDerivatives(currentInductor, voltageCapacitor, switchOn);

            currentInductor += derivatives.Item1 * dt;
            voltageCapacitor += derivatives.Item2 * dt;

            // Verificar DCM (corriente no puede ser negativa)
            if (currentInductor < 0)
                currentInductor = 0;

            return new CircuitState
            {
                InductorCurrent = currentInductor,
                CapacitorVoltage = voltageCapacitor,
                SwitchState = switchOn
            };
        }

        /// <summary>
        /// Método Runge-Kutta 2do orden (Heun)
        /// </summary>
        private CircuitState StepRK2(double dt, bool switchOn)
        {
            // k1
            var k1 = CalculateDerivatives(currentInductor, voltageCapacitor, switchOn);

            // k2
            double iL_temp = currentInductor + dt * k1.Item1;
            double vC_temp = voltageCapacitor + dt * k1.Item2;
            var k2 = CalculateDerivatives(iL_temp, vC_temp, switchOn);

            // Combine
            currentInductor += (dt / 2.0) * (k1.Item1 + k2.Item1);
            voltageCapacitor += (dt / 2.0) * (k1.Item2 + k2.Item2);

            if (currentInductor < 0)
                currentInductor = 0;

            return new CircuitState
            {
                InductorCurrent = currentInductor,
                CapacitorVoltage = voltageCapacitor,
                SwitchState = switchOn
            };
        }

        /// <summary>
        /// Método Runge-Kutta 4to orden (más preciso)
        /// </summary>
        private CircuitState StepRK4(double dt, bool switchOn)
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

            // Combinar resultados
            currentInductor += (dt / 6.0) * (k1.Item1 + 2 * k2.Item1 + 2 * k3.Item1 + k4.Item1);
            voltageCapacitor += (dt / 6.0) * (k1.Item2 + 2 * k2.Item2 + 2 * k3.Item2 + k4.Item2);

            // Verificar DCM
            if (currentInductor < 0)
                currentInductor = 0;

            return new CircuitState
            {
                InductorCurrent = currentInductor,
                CapacitorVoltage = voltageCapacitor,
                SwitchState = switchOn
            };
        }

        /// <summary>
        /// Calcula las derivadas diL/dt y dvC/dt según las ecuaciones del Buck converter
        /// </summary>
        private Tuple<double, double> CalculateDerivatives(double iL, double vC, bool switchOn)
        {
            double diL_dt, dvC_dt;

            if (switchOn)
            {
                // Switch ON: Vin -> L -> C -> Load
                // Ecuación del inductor: L * diL/dt = Vin - vC - iL*rL - iL*rDS
                // Ecuación del capacitor: C * dvC/dt = iL - vC/R

                double vL = Circuit.InputVoltage - vC -
                           iL * Circuit.InductorESR -
                           iL * Circuit.SwitchRDSon;

                diL_dt = vL / Circuit.Inductance;

                double iC = iL - vC / Circuit.LoadResistance;
                dvC_dt = iC / Circuit.Capacitance;
            }
            else
            {
                // Switch OFF: L -> Diode -> C -> Load (freewheeling)
                // Ecuación del inductor: L * diL/dt = -vC - iL*rL - VF
                // Ecuación del capacitor: C * dvC/dt = iL - vC/R

                if (iL < 1e-6) // DCM - corriente prácticamente cero
                {
                    diL_dt = 0;
                    dvC_dt = -vC / (Circuit.LoadResistance * Circuit.Capacitance);
                }
                else
                {
                    double vL = -vC - iL * Circuit.InductorESR - Circuit.DiodeVF;
                    diL_dt = vL / Circuit.Inductance;

                    double iC = iL - vC / Circuit.LoadResistance;
                    dvC_dt = iC / Circuit.Capacitance;
                }
            }

            return Tuple.Create(diL_dt, dvC_dt);
        }

        /// <summary>
        /// Obtiene el estado actual del circuito
        /// </summary>
        public CircuitState GetCurrentState(bool switchOn)
        {
            return new CircuitState
            {
                InductorCurrent = currentInductor,
                CapacitorVoltage = voltageCapacitor,
                SwitchState = switchOn
            };
        }
    }

    /// <summary>
    /// Métodos de solución disponibles
    /// </summary>
    public enum SolverMethod
    {
        /// <summary>
        /// Euler - Primer orden (rápido pero menos preciso)
        /// </summary>
        Euler,

        /// <summary>
        /// Runge-Kutta 2do orden (balance velocidad/precisión)
        /// </summary>
        RK2,

        /// <summary>
        /// Runge-Kutta 4to orden (más preciso)
        /// </summary>
        RK4
    }

    /// <summary>
    /// Estado instantáneo del circuito
    /// </summary>
    public class CircuitState
    {
        public double InductorCurrent { get; set; }
        public double CapacitorVoltage { get; set; }
        public bool SwitchState { get; set; }
        public double Time { get; set; }

        /// <summary>
        /// Voltaje del switch (VDS)
        /// </summary>
        public double SwitchVoltage(CircuitParameters circuit)
        {
            if (SwitchState)
                return InductorCurrent * circuit.SwitchRDSon;
            else
                return circuit.InputVoltage;
        }

        /// <summary>
        /// Voltaje del diodo
        /// </summary>
        public double DiodeVoltage(CircuitParameters circuit)
        {
            if (SwitchState)
                return -circuit.InputVoltage; // Reverse biased
            else
                return circuit.DiodeVF; // Forward biased
        }

        /// <summary>
        /// Corriente del switch
        /// </summary>
        public double SwitchCurrent()
        {
            return SwitchState ? InductorCurrent : 0;
        }

        /// <summary>
        /// Corriente del diodo
        /// </summary>
        public double DiodeCurrent()
        {
            return !SwitchState ? InductorCurrent : 0;
        }
    }
}