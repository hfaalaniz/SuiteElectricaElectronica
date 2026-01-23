using System;

namespace BuckConverterCalculator.Components
{
    /// <summary>
    /// Modelo de componente MOSFET
    /// </summary>
    public class MosfetComponent
    {
        public string PartNumber { get; set; }
        public double Vds { get; set; }          // Tensión drain-source máxima (V)
        public double Id { get; set; }           // Corriente drain continua (A)
        public double RdsOn { get; set; }        // Resistencia on-state (Ω)
        public string Package { get; set; }
        public string Supplier { get; set; }
        public bool RequiresHeatsink { get; set; }
    }

    /// <summary>
    /// Modelo de componente Diodo
    /// </summary>
    public class DiodeComponent
    {
        public string PartNumber { get; set; }
        public double Vr { get; set; }           // Tensión reverse máxima (V)
        public double If { get; set; }           // Corriente forward máxima (A)
        public double Vf { get; set; }           // Caída forward (V)
        public double IfTest { get; set; }       // Corriente de test para Vf (A)
        public string Package { get; set; }
        public string Supplier { get; set; }
        public bool RequiresHeatsink { get; set; }
    }

    /// <summary>
    /// Modelo de componente Inductor
    /// </summary>
    public class InductorComponent
    {
        public string PartNumber { get; set; }
        public double Inductance { get; set; }      // Inductancia (H)
        public double SaturationCurrent { get; set; } // Corriente de saturación (A)
        public double RmsCurrent { get; set; }        // Corriente RMS máxima (A)
        public double Dcr { get; set; }              // Resistencia DC (Ω)
        public string Package { get; set; }
        public string Supplier { get; set; }
    }

    /// <summary>
    /// Modelo de componente Capacitor
    /// </summary>
    public class CapacitorComponent
    {
        public string PartNumber { get; set; }
        public double Capacitance { get; set; }    // Capacitancia (F)
        public double Voltage { get; set; }        // Tensión máxima (V)
        public string Type { get; set; }           // Tipo: Electrolytic, Ceramic, etc.
        public double Esr { get; set; }            // ESR (Ω)
        public double RippleCurrent { get; set; }  // Corriente ripple máxima (A)
        public string Package { get; set; }
        public string Supplier { get; set; }
    }

    /// <summary>
    /// Modelo de controlador PWM
    /// </summary>
    public class PwmControllerComponent
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public double Vcc { get; set; }            // Tensión alimentación (V)
        public double Vref { get; set; }           // Tensión referencia (V)
        public double MaxFrequency { get; set; }   // Frecuencia máxima (Hz)
        public string Package { get; set; }
        public string Supplier { get; set; }
    }

    /// <summary>
    /// Conjunto completo de componentes seleccionados
    /// </summary>
    public class SelectedComponents
    {
        public MosfetComponent Mosfet { get; set; }
        public DiodeComponent Diode { get; set; }
        public InductorComponent Inductor { get; set; }
        public CapacitorComponent InputCapacitor { get; set; }
        public CapacitorComponent OutputCapacitor { get; set; }
        public int OutputCapacitorCount { get; set; }
        public PwmControllerComponent PwmController { get; set; }
    }
}