using System;

namespace BuckConverterCalculator.Core
{
    /// <summary>
    /// Motor de cálculo para diseño de Buck Converter
    /// Implementa todas las ecuaciones fundamentales
    /// </summary>
    public class BuckCalculator
    {
        private const double VREF_UC3843 = 2.5;  // Tensión de referencia UC3843 (V)
        private const double AMBIENT_TEMP = 25.0; // Temperatura ambiente (°C)

        public CalculationResults Calculate(DesignParameters param)
        {
            var results = new CalculationResults();

            // 1. PARÁMETROS FUNDAMENTALES
            CalculateFundamentals(param, results);

            // 2. INDUCTOR
            CalculateInductor(param, results);

            // 3. CAPACITOR DE SALIDA
            CalculateOutputCapacitor(param, results);

            // 4. DIVISOR DE REALIMENTACIÓN
            CalculateFeedbackDivider(param, results);

            // 5. CONFIGURACIÓN PWM (UC3843)
            CalculatePwmConfiguration(param, results);

            // 6. ANÁLISIS DE PÉRDIDAS
            CalculateLosses(param, results);

            // 7. ANÁLISIS TÉRMICO
            CalculateThermalAnalysis(param, results);

            return results;
        }

        private void CalculateFundamentals(DesignParameters param, CalculationResults results)
        {
            // Duty Cycle: D = Vout / Vin
            results.CalculatedDutyCycle = param.OutputVoltage / param.InputVoltageMax;

            // Potencia de salida
            results.PowerOutput = param.OutputVoltage * param.OutputCurrent;

            // Potencia de entrada (considerando eficiencia)
            double efficiency = param.EfficiencyPercent / 100.0;
            results.PowerInput = results.PowerOutput / efficiency;

            // Eficiencia real (se recalcula después del análisis de pérdidas)
            results.ActualEfficiency = param.EfficiencyPercent;
        }

        private void CalculateInductor(DesignParameters param, CalculationResults results)
        {
            // Ripple de corriente deseado
            double ripplePercent = param.RippleCurrentPercent / 100.0;
            results.RippleCurrent = param.Iout * ripplePercent;

            // Inductancia requerida: L = (Vin - Vout) * D / (ΔIL * f)
            results.Inductance = (param.InputVoltageMax - param.OutputVoltage) * results.DutyCycle /
                                (results.RippleCurrent * param.Frequency);

            // Valor comercial (redondear hacia arriba a valores estándar)
            results.InductanceCommercial = RoundToCommercialInductance(results.Inductance);

            // Recalcular ripple real con inductancia comercial
            results.RippleCurrent = (param.InputVoltageMax - param.OutputVoltage) * results.DutyCycle /
                                   (results.InductanceCommercial * param.Frequency);

            // Corriente pico: Ipk = Iout + ΔIL/2
            results.PeakInductorCurrent = param.Iout + (results.RippleCurrent / 2.0);

            // Corriente RMS en el inductor
            // Para forma de onda triangular: Irms² = Iavg² + (ΔI²/12)
            results.RmsInductorCurrent = Math.Sqrt(
                Math.Pow(param.Iout, 2) +
                Math.Pow(results.RippleCurrent, 2) / 12.0
            );
        }

        private void CalculateOutputCapacitor(DesignParameters param, CalculationResults results)
        {
            // Ripple de tensión deseado (convertir mV a V)
            double rippleVoltage = param.RippleVoltageMv / 1000.0;

            // Capacitancia requerida: C = ΔIL / (8 * f * ΔVout)
            results.OutputCapacitance = results.RippleCurrent /
                                       (8.0 * param.Frequency * rippleVoltage);

            // Valor comercial
            results.OutputCapacitanceCommercial = RoundToCommercialCapacitance(results.OutputCapacitance);

            // ESR máximo permitido: ESR_max = ΔVout / ΔIL
            results.MaxEsr = rippleVoltage / results.RippleCurrent;

            // Corriente RMS en el capacitor
            // Para Buck: Ic_rms ≈ ΔIL / √12
            results.RmsCapacitorCurrent = results.RippleCurrent / Math.Sqrt(12.0);
        }

        private void CalculateFeedbackDivider(DesignParameters param, CalculationResults results)
        {
            // Para UC3843, Vref = 2.5V
            // Vout = Vref * (1 + R1/R2)
            // Despejando: R1/R2 = (Vout/Vref) - 1

            double ratio = (param.OutputVoltage / VREF_UC3843) - 1.0;

            // Seleccionar R2 = 10kΩ (valor típico)
            results.FeedbackR2 = 10000;

            // Calcular R1
            results.FeedbackR1 = results.FeedbackR2 * ratio;

            // Redondear R1 a valor comercial
            results.FeedbackR1 = RoundToCommercialResistor(results.FeedbackR1);

            // Verificar Vout con valores reales
            results.VoutVerified = VREF_UC3843 * (1.0 + results.FeedbackR1 / results.FeedbackR2);
        }

        private void CalculatePwmConfiguration(DesignParameters param, CalculationResults results)
        {
            // Para UC3843: f = 1.72 / (Rt * Ct)
            // Seleccionar Ct estándar y calcular Rt

            // Capacitor típico para 100kHz: 2.2nF
            results.CtValue = 2.2e-9;

            // Calcular Rt: Rt = 1.72 / (f * Ct)
            results.RtValue = 1.72 / (param.Frequency * results.CtValue);

            // Redondear Rt a valor comercial
            results.RtValue = RoundToCommercialResistor(results.RtValue);

            // Frecuencia real con valores comerciales
            results.ActualFrequency = 1.72 / (results.RtValue * results.CtValue);
        }

        private void CalculateLosses(DesignParameters param, CalculationResults results)
        {
            // Pérdidas en MOSFET - Conducción
            // Pcond = Iout² * Rds(on) * D
            double rdsOn = 0.009; // 9mΩ típico para IRFB4115
            results.MosfetConductionLoss = Math.Pow(param.Iout, 2) * rdsOn * results.DutyCycle;

            // Pérdidas en MOSFET - Switching
            // Psw = 0.5 * Vin * Iout * (tr + tf) * f
            double transitionTime = 45e-9; // 45ns típico (tr + tf)
            results.MosfetSwitchingLoss = 0.5 * param.InputVoltageMax * param.Iout * transitionTime * param.Frequency;

            // Pérdidas totales MOSFET
            results.MosfetTotalLoss = results.MosfetConductionLoss + results.MosfetSwitchingLoss;

            // Pérdidas en Diodo
            // Pcond = Vf * Iout * (1 - D)
            double vf = 0.85; // 850mV típico para Schottky a corriente nominal
            results.DiodeConductionLoss = vf * param.Iout * (1.0 - results.DutyCycle);

            // Pérdidas en Inductor
            // Pcond = Irms² * DCR
            double dcr = 0.020; // 20mΩ típico
            results.InductorCoreLoss = Math.Pow(results.RmsInductorCurrent, 2) * dcr;

            // Pérdidas adicionales (driver, control, etc) - estimado 5% de Pout
            double miscLosses = results.PowerOutput * 0.05;

            // Pérdidas totales
            results.TotalLosses = results.MosfetTotalLoss +
                                 results.DiodeConductionLoss +
                                 results.InductorCoreLoss +
                                 miscLosses;

            // Recalcular eficiencia real
            results.ActualEfficiency = (results.PowerOutput /
                                       (results.PowerOutput + results.TotalLosses)) * 100.0;
        }

        private void CalculateThermalAnalysis(DesignParameters param, CalculationResults results)
        {
            // MOSFET con disipador
            // θJA típico con disipador: 15°C/W
            double thetaJA_mosfet = 15.0;
            results.MosfetJunctionTemp = AMBIENT_TEMP +
                                        (results.MosfetTotalLoss * thetaJA_mosfet);

            // Diodo con disipador
            // θJA típico con disipador: 12°C/W
            double thetaJA_diode = 12.0;
            results.DiodeJunctionTemp = AMBIENT_TEMP +
                                       (results.DiodeConductionLoss * thetaJA_diode);

            // Inductor
            // θJA típico: 40°C/W
            double thetaJA_inductor = 40.0;
            results.InductorTemp = AMBIENT_TEMP +
                                  (results.InductorCoreLoss * thetaJA_inductor);
        }

        // FUNCIONES AUXILIARES PARA VALORES COMERCIALES

        private double RoundToCommercialInductance(double value)
        {
            // Valores comerciales de inductancia en µH: 10, 15, 22, 33, 47, 68, 100, 120, 150, 220...
            double[] standardValues = { 10, 15, 22, 33, 47, 68, 100, 120, 150, 220, 330, 470, 680, 1000 };
            double valueUh = value * 1e6; // Convertir a µH

            // Encontrar múltiplo de 10 apropiado
            double multiplier = 1.0;
            while (valueUh > 1000)
            {
                valueUh /= 10.0;
                multiplier *= 10.0;
            }
            while (valueUh < 10)
            {
                valueUh *= 10.0;
                multiplier /= 10.0;
            }

            // Encontrar valor estándar más cercano (hacia arriba)
            double selected = standardValues[standardValues.Length - 1];
            foreach (var std in standardValues)
            {
                if (std >= valueUh)
                {
                    selected = std;
                    break;
                }
            }

            return (selected * multiplier) * 1e-6; // Convertir de vuelta a H
        }

        private double RoundToCommercialCapacitance(double value)
        {
            // Valores comerciales E12: 10, 12, 15, 18, 22, 27, 33, 39, 47, 56, 68, 82
            double[] standardValues = { 10, 12, 15, 18, 22, 27, 33, 39, 47, 56, 68, 82, 100 };
            double valueUf = value * 1e6; // Convertir a µF

            // Encontrar múltiplo de 10 apropiado
            double multiplier = 1.0;
            while (valueUf > 100)
            {
                valueUf /= 10.0;
                multiplier *= 10.0;
            }
            while (valueUf < 10)
            {
                valueUf *= 10.0;
                multiplier /= 10.0;
            }

            // Encontrar valor estándar más cercano (hacia arriba)
            double selected = standardValues[standardValues.Length - 1];
            foreach (var std in standardValues)
            {
                if (std >= valueUf)
                {
                    selected = std;
                    break;
                }
            }

            return (selected * multiplier) * 1e-6; // Convertir de vuelta a F
        }

        private double RoundToCommercialResistor(double value)
        {
            // Serie E24 (valores 1% tolerancia)
            double[] e24 = { 1.0, 1.1, 1.2, 1.3, 1.5, 1.6, 1.8, 2.0, 2.2, 2.4, 2.7, 3.0,
                           3.3, 3.6, 3.9, 4.3, 4.7, 5.1, 5.6, 6.2, 6.8, 7.5, 8.2, 9.1 };

            // Encontrar múltiplo de 10 apropiado
            double multiplier = 1.0;
            double normalized = value;

            while (normalized >= 10.0)
            {
                normalized /= 10.0;
                multiplier *= 10.0;
            }
            while (normalized < 1.0)
            {
                normalized *= 10.0;
                multiplier /= 10.0;
            }

            // Encontrar valor E24 más cercano
            double closest = e24[0];
            double minDiff = Math.Abs(normalized - e24[0]);

            foreach (var e in e24)
            {
                double diff = Math.Abs(normalized - e);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = e;
                }
            }

            return closest * multiplier;
        }
    }
}