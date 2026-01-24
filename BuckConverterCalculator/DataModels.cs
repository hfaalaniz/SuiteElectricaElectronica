using System;

namespace BuckConverterCalculator.Core
{
    /// <summary>
    /// Parámetros de diseño del convertidor Buck
    /// </summary>
    public class DesignParameters
    {
        // ESTAS son las propiedades correctas:
        public double InputVoltageMin { get; set; }
        public double InputVoltageMax { get; set; }
        public double OutputVoltage { get; set; }
        public double OutputCurrent { get; set; }
        public double SwitchingFrequency { get; set; }
        public double MaxOutputRipple { get; set; }
        public double MaxInductorCurrentRipple { get; set; }
        public double TargetEfficiency { get; set; }

        // ============================================
        // ALIASES - AGREGAR ESTAS
        // ============================================

        public double Vin
        {
            get { return InputVoltageMax; }
            set { InputVoltageMax = value; }
        }

        public double Vout
        {
            get { return OutputVoltage; }
            set { OutputVoltage = value; }
        }

        public double Iout
        {
            get { return OutputCurrent; }
            set { OutputCurrent = value; }
        }

        public double Frequency
        {
            get { return SwitchingFrequency; }
            set { SwitchingFrequency = value; }
        }

        public double RippleCurrentPercent
        {
            get { return MaxInductorCurrentRipple * 100; }
            set { MaxInductorCurrentRipple = value / 100; }
        }

        public double RippleVoltageMv
        {
            get { return MaxOutputRipple * 1000; }
            set { MaxOutputRipple = value / 1000; }
        }

        public double EfficiencyPercent
        {
            get { return TargetEfficiency * 100; }
            set { TargetEfficiency = value / 100; }
        }
    }

    /// <summary>
    /// Resultados de los cálculos del Buck Converter
    /// </summary>
    public class CalculationResults
    {
        // ESTAS son las propiedades correctas:
        public double CalculatedInductance { get; set; }
        public double CalculatedCapacitance { get; set; }
        public double CalculatedDutyCycle { get; set; }

        public double SelectedInductance { get; set; }  // NO "InductanceCommercial"
        public string InductorPartNumber { get; set; }

        public double SelectedCapacitance { get; set; }
        public string CapacitorPartNumber { get; set; }

        public string MOSFETPartNumber { get; set; }
        public double MOSFETRDSon { get; set; }

        public string DiodePartNumber { get; set; }
        public double DiodeVF { get; set; }

        public double PeakInductorCurrent { get; set; }
        public double RMSInductorCurrent { get; set; }
        public double ActualOutputRipple { get; set; }
        public double EstimatedEfficiency { get; set; }

        public double SwitchingLosses { get; set; }
        public double ConductionLosses { get; set; }
        public double DiodeLosses { get; set; }
        public double InductorLosses { get; set; }
        public double TotalLosses { get; set; }

        public string OperatingMode { get; set; }

        // Aliases comunes
        public double DutyCycle
        {
            get { return CalculatedDutyCycle; }
            set { CalculatedDutyCycle = value; }
        }

        public double Inductance
        {
            get { return CalculatedInductance; }
            set { CalculatedInductance = value; }
        }

        public double InductanceCommercial
        {
            get { return SelectedInductance; }
            set { SelectedInductance = value; }
        }

        public double OutputCapacitance
        {
            get { return CalculatedCapacitance; }
            set { CalculatedCapacitance = value; }
        }

        public double OutputCapacitanceCommercial
        {
            get { return SelectedCapacitance; }
            set { SelectedCapacitance = value; }
        }

        // Corrientes
        public double RippleCurrent { get; set; }
        public double RmsInductorCurrent
        {
            get { return RMSInductorCurrent; }
            set { RMSInductorCurrent = value; }
        }
        public double RmsCapacitorCurrent { get; set; }

        // Potencias
        public double PowerOutput { get; set; }
        public double PowerInput { get; set; }
        public double ActualEfficiency
        {
            get { return EstimatedEfficiency; }
            set { EstimatedEfficiency = value; }
        }

        // ESR y características del capacitor
        public double MaxEsr { get; set; }

        // Feedback resistors
        public double FeedbackR1 { get; set; }
        public double FeedbackR2 { get; set; }
        public double VoutVerified { get; set; }

        // Timing components
        public double RtValue { get; set; }
        public double CtValue { get; set; }
        public double ActualFrequency { get; set; }

        // Pérdidas detalladas del MOSFET
        public double MosfetConductionLoss { get; set; }
        public double MosfetSwitchingLoss { get; set; }
        public double MosfetTotalLoss { get; set; }
        public double MosfetJunctionTemp { get; set; }

        // Pérdidas del diodo
        public double DiodeConductionLoss
        {
            get { return DiodeLosses; }
            set { DiodeLosses = value; }
        }
        public double DiodeJunctionTemp { get; set; }

        // Pérdidas del inductor
        public double InductorCoreLoss
        {
            get { return InductorLosses; }
            set { InductorLosses = value; }
        }
        public double InductorTemp { get; set; }
    }
}