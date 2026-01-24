using System;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// Parámetros de diseño del convertidor Buck
    /// </summary>
    [Serializable]
    public class DesignParameters
    {
        // Parámetros de entrada
        public double InputVoltageMin { get; set; }
        public double InputVoltageMax { get; set; }
        public double OutputVoltage { get; set; }
        public double OutputCurrent { get; set; }
        public double SwitchingFrequency { get; set; }

        // Ripple targets
        public double MaxOutputRipple { get; set; }
        public double MaxInductorCurrentRipple { get; set; }

        // Eficiencia
        public double TargetEfficiency { get; set; }

        public DesignParameters()
        {
            // Valores por defecto
            InputVoltageMin = 80;
            InputVoltageMax = 100;
            OutputVoltage = 13;
            OutputCurrent = 5;
            SwitchingFrequency = 100000; // 100 kHz
            MaxOutputRipple = 0.05; // 5%
            MaxInductorCurrentRipple = 0.30; // 30%
            TargetEfficiency = 0.90; // 90%
        }

        public override string ToString()
        {
            return $"Vin: {InputVoltageMin}-{InputVoltageMax}V, Vout: {OutputVoltage}V @ {OutputCurrent}A, Fsw: {SwitchingFrequency / 1000}kHz";
        }
    }

    /// <summary>
    /// Resultados de cálculo del convertidor
    /// </summary>
    [Serializable]
    public class CalculationResults
    {
        // Componentes calculados
        public double CalculatedInductance { get; set; }
        public double CalculatedCapacitance { get; set; }
        public double CalculatedDutyCycle { get; set; }

        // Componentes seleccionados (valores estándar)
        public double SelectedInductance { get; set; }
        public string InductorPartNumber { get; set; }

        public double SelectedCapacitance { get; set; }
        public string CapacitorPartNumber { get; set; }

        public string MOSFETPartNumber { get; set; }
        public double MOSFETRDSon { get; set; }

        public string DiodePartNumber { get; set; }
        public double DiodeVF { get; set; }

        // Resultados de rendimiento
        public double PeakInductorCurrent { get; set; }
        public double RMSInductorCurrent { get; set; }
        public double ActualOutputRipple { get; set; }
        public double EstimatedEfficiency { get; set; }

        // Pérdidas
        public double SwitchingLosses { get; set; }
        public double ConductionLosses { get; set; }
        public double DiodeLosses { get; set; }
        public double InductorLosses { get; set; }
        public double TotalLosses { get; set; }

        // Modo de operación
        public string OperatingMode { get; set; }

        public CalculationResults()
        {
            OperatingMode = "Unknown";
        }

        public override string ToString()
        {
            return $"L={SelectedInductance * 1e6:F1}µH, C={SelectedCapacitance * 1e6:F1}µF, D={CalculatedDutyCycle:F3}, η={EstimatedEfficiency:P2}";
        }
    }
}