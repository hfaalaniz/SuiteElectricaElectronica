using System;
using System.Collections.Generic;
using System.Linq;
using BuckConverterCalculator.Core;

namespace BuckConverterCalculator.Components
{
    /// <summary>
    /// Selector automático de componentes basado en base de datos integrada
    /// </summary>
    public class ComponentSelector
    {
        private List<MosfetComponent> mosfetDatabase;
        private List<DiodeComponent> diodeDatabase;
        private List<InductorComponent> inductorDatabase;
        private List<CapacitorComponent> capacitorDatabase;
        private List<PwmControllerComponent> pwmDatabase;

        public ComponentSelector()
        {
            InitializeDatabases();
        }

        private void InitializeDatabases()
        {
            InitializeMosfets();
            InitializeDiodes();
            InitializeInductors();
            InitializeCapacitors();
            InitializePwmControllers();
        }

        private void InitializeMosfets()
        {
            mosfetDatabase = new List<MosfetComponent>
            {
                new MosfetComponent
                {
                    PartNumber = "IRFB4115PBF",
                    Vds = 150,
                    Id = 104,
                    RdsOn = 0.0087,
                    Package = "TO-220AB",
                    Supplier = "Infineon",
                    RequiresHeatsink = true
                },
                new MosfetComponent
                {
                    PartNumber = "IRFB4110PBF",
                    Vds = 100,
                    Id = 120,
                    RdsOn = 0.0039,
                    Package = "TO-220AB",
                    Supplier = "Infineon",
                    RequiresHeatsink = true
                },
                new MosfetComponent
                {
                    PartNumber = "IPP052N08N5",
                    Vds = 80,
                    Id = 80,
                    RdsOn = 0.0052,
                    Package = "TO-220",
                    Supplier = "Infineon",
                    RequiresHeatsink = true
                },
                new MosfetComponent
                {
                    PartNumber = "STP75NF75",
                    Vds = 75,
                    Id = 80,
                    RdsOn = 0.0075,
                    Package = "TO-220",
                    Supplier = "STMicroelectronics",
                    RequiresHeatsink = true
                },
                new MosfetComponent
                {
                    PartNumber = "IRF540N",
                    Vds = 100,
                    Id = 33,
                    RdsOn = 0.044,
                    Package = "TO-220AB",
                    Supplier = "Infineon",
                    RequiresHeatsink = false
                },
                new MosfetComponent
                {
                    PartNumber = "IRFZ44N",
                    Vds = 55,
                    Id = 49,
                    RdsOn = 0.0175,
                    Package = "TO-220AB",
                    Supplier = "Infineon",
                    RequiresHeatsink = false
                }
            };
        }

        private void InitializeDiodes()
        {
            diodeDatabase = new List<DiodeComponent>
            {
                new DiodeComponent
                {
                    PartNumber = "MBR20200CT",
                    Vr = 200,
                    If = 20,
                    Vf = 0.85,
                    IfTest = 10,
                    Package = "TO-220AB",
                    Supplier = "ON Semiconductor",
                    RequiresHeatsink = true
                },
                new DiodeComponent
                {
                    PartNumber = "MBR10100CT",
                    Vr = 100,
                    If = 10,
                    Vf = 0.75,
                    IfTest = 5,
                    Package = "TO-220AB",
                    Supplier = "ON Semiconductor",
                    RequiresHeatsink = false
                },
                new DiodeComponent
                {
                    PartNumber = "STPS30L60CT",
                    Vr = 60,
                    If = 30,
                    Vf = 0.65,
                    IfTest = 15,
                    Package = "TO-220AB",
                    Supplier = "STMicroelectronics",
                    RequiresHeatsink = true
                },
                new DiodeComponent
                {
                    PartNumber = "MBRF20200CT",
                    Vr = 200,
                    If = 20,
                    Vf = 0.80,
                    IfTest = 10,
                    Package = "TO-220AB",
                    Supplier = "ON Semiconductor",
                    RequiresHeatsink = true
                },
                new DiodeComponent
                {
                    PartNumber = "VS-30CPH06PBF",
                    Vr = 600,
                    If = 30,
                    Vf = 1.8,
                    IfTest = 15,
                    Package = "TO-247AC",
                    Supplier = "Vishay",
                    RequiresHeatsink = true
                }
            };
        }

        private void InitializeInductors()
        {
            inductorDatabase = new List<InductorComponent>
            {
                new InductorComponent
                {
                    PartNumber = "MSS1278-124MEC",
                    Inductance = 120e-6,
                    SaturationCurrent = 13,
                    RmsCurrent = 10,
                    Dcr = 0.020,
                    Package = "Radial D12.0mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "DO3316P-103HC",
                    Inductance = 10e-6,
                    SaturationCurrent = 28,
                    RmsCurrent = 22,
                    Dcr = 0.0028,
                    Package = "SMD 33x33mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "MSS1260-223ML",
                    Inductance = 22e-6,
                    SaturationCurrent = 18,
                    RmsCurrent = 14,
                    Dcr = 0.0055,
                    Package = "Radial D12.6mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "SER2915H-472KL",
                    Inductance = 4.7e-6,
                    SaturationCurrent = 35,
                    RmsCurrent = 28,
                    Dcr = 0.0015,
                    Package = "SMD 29x15mm",
                    Supplier = "Bourns"
                },
                new InductorComponent
                {
                    PartNumber = "MSS1278-473MEC",
                    Inductance = 47e-6,
                    SaturationCurrent = 15,
                    RmsCurrent = 12,
                    Dcr = 0.012,
                    Package = "Radial D12.0mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "DO5040H-103ML",
                    Inductance = 10e-6,
                    SaturationCurrent = 45,
                    RmsCurrent = 36,
                    Dcr = 0.0012,
                    Package = "SMD 50x40mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "SER2915H-333KL",
                    Inductance = 33e-6,
                    SaturationCurrent = 20,
                    RmsCurrent = 16,
                    Dcr = 0.0065,
                    Package = "SMD 29x15mm",
                    Supplier = "Bourns"
                },
                new InductorComponent
                {
                    PartNumber = "MSS1278-683MEC",
                    Inductance = 68e-6,
                    SaturationCurrent = 11,
                    RmsCurrent = 9,
                    Dcr = 0.025,
                    Package = "Radial D12.0mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "MSS1260-153ML",
                    Inductance = 15e-6,
                    SaturationCurrent = 23,
                    RmsCurrent = 18,
                    Dcr = 0.0040,
                    Package = "Radial D12.6mm",
                    Supplier = "Coilcraft"
                },
                new InductorComponent
                {
                    PartNumber = "MSS1278-333MEC",
                    Inductance = 33e-6,
                    SaturationCurrent = 18,
                    RmsCurrent = 14,
                    Dcr = 0.0085,
                    Package = "Radial D12.0mm",
                    Supplier = "Coilcraft"
                }
            };
        }

        private void InitializeCapacitors()
        {
            capacitorDatabase = new List<CapacitorComponent>
            {
                // Capacitores de entrada (Electrolíticos)
                new CapacitorComponent
                {
                    PartNumber = "UVR2C470MPD",
                    Capacitance = 47e-6,
                    Voltage = 160,
                    Type = "Electrolytic Aluminum",
                    Esr = 0.150,
                    RippleCurrent = 2.2,
                    Package = "Radial D10.0mm P5.00mm",
                    Supplier = "Nichicon"
                },
                new CapacitorComponent
                {
                    PartNumber = "EEU-FR1C102",
                    Capacitance = 1000e-6,
                    Voltage = 16,
                    Type = "Electrolytic Aluminum",
                    Esr = 0.050,
                    RippleCurrent = 4.8,
                    Package = "Radial D10.0mm",
                    Supplier = "Panasonic"
                },
                new CapacitorComponent
                {
                    PartNumber = "UVR2C100MPD",
                    Capacitance = 10e-6,
                    Voltage = 160,
                    Type = "Electrolytic Aluminum",
                    Esr = 0.500,
                    RippleCurrent = 0.8,
                    Package = "Radial D8.0mm",
                    Supplier = "Nichicon"
                },
                // Capacitores de salida (Cerámicos X7R)
                new CapacitorComponent
                {
                    PartNumber = "C1210X7R1E107M",
                    Capacitance = 100e-6,
                    Voltage = 25,
                    Type = "Ceramic X7R",
                    Esr = 0.005,
                    RippleCurrent = 3.5,
                    Package = "1210 (3225 Metric)",
                    Supplier = "TDK"
                },
                new CapacitorComponent
                {
                    PartNumber = "GRM32ER71E226KE15",
                    Capacitance = 22e-6,
                    Voltage = 25,
                    Type = "Ceramic X7R",
                    Esr = 0.008,
                    RippleCurrent = 2.8,
                    Package = "1210 (3225 Metric)",
                    Supplier = "Murata"
                },
                new CapacitorComponent
                {
                    PartNumber = "C1210X7R1C476M",
                    Capacitance = 47e-6,
                    Voltage = 16,
                    Type = "Ceramic X7R",
                    Esr = 0.006,
                    RippleCurrent = 3.0,
                    Package = "1210 (3225 Metric)",
                    Supplier = "TDK"
                },
                new CapacitorComponent
                {
                    PartNumber = "GRM32ER71C226KE15",
                    Capacitance = 22e-6,
                    Voltage = 16,
                    Type = "Ceramic X7R",
                    Esr = 0.007,
                    RippleCurrent = 2.9,
                    Package = "1210 (3225 Metric)",
                    Supplier = "Murata"
                }
            };
        }

        private void InitializePwmControllers()
        {
            pwmDatabase = new List<PwmControllerComponent>
            {
                new PwmControllerComponent
                {
                    PartNumber = "UC3843AN",
                    Description = "Current Mode PWM Controller",
                    Vcc = 15,
                    Vref = 2.5,
                    MaxFrequency = 500000,
                    Package = "DIP-8",
                    Supplier = "Texas Instruments"
                },
                new PwmControllerComponent
                {
                    PartNumber = "UC3842AN",
                    Description = "Current Mode PWM Controller",
                    Vcc = 15,
                    Vref = 2.5,
                    MaxFrequency = 500000,
                    Package = "DIP-8",
                    Supplier = "Texas Instruments"
                },
                new PwmControllerComponent
                {
                    PartNumber = "TL494CN",
                    Description = "PWM Controller",
                    Vcc = 15,
                    Vref = 5.0,
                    MaxFrequency = 300000,
                    Package = "DIP-16",
                    Supplier = "Texas Instruments"
                },
                new PwmControllerComponent
                {
                    PartNumber = "SG3525AN",
                    Description = "PWM Controller",
                    Vcc = 15,
                    Vref = 5.1,
                    MaxFrequency = 400000,
                    Package = "DIP-16",
                    Supplier = "STMicroelectronics"
                }
            };
        }

        public SelectedComponents SelectComponents(CalculationResults results, DesignParameters parameters)
        {
            var selected = new SelectedComponents();

            // Seleccionar MOSFET
            selected.Mosfet = SelectMosfet(parameters, results);

            // Seleccionar Diodo
            selected.Diode = SelectDiode(parameters, results);

            // Seleccionar Inductor
            selected.Inductor = SelectInductor(results);

            // Seleccionar Capacitor de entrada
            selected.InputCapacitor = SelectInputCapacitor(parameters);

            // Seleccionar Capacitor(es) de salida
            SelectOutputCapacitors(results, parameters, selected);

            // Seleccionar Controlador PWM
            selected.PwmController = SelectPwmController(parameters);

            return selected;
        }

        private MosfetComponent SelectMosfet(DesignParameters parameters, CalculationResults results)
        {
            // Criterios: Vds > 1.5 × Vin, Id > 1.5 × Ipeak
            double minVds = parameters.Vin * 1.5;
            double minId = results.PeakInductorCurrent * 1.5;

            var candidates = mosfetDatabase
                .Where(m => m.Vds >= minVds && m.Id >= minId)
                .OrderBy(m => m.RdsOn)
                .ToList();

            return candidates.FirstOrDefault() ?? mosfetDatabase.First();
        }

        private DiodeComponent SelectDiode(DesignParameters parameters, CalculationResults results)
        {
            // Criterios: Vr > 1.5 × Vin, If > 1.5 × Ipeak
            double minVr = parameters.Vin * 1.5;
            double minIf = results.PeakInductorCurrent * 1.5;

            var candidates = diodeDatabase
                .Where(d => d.Vr >= minVr && d.If >= minIf)
                .OrderBy(d => d.Vf)
                .ToList();

            return candidates.FirstOrDefault() ?? diodeDatabase.First();
        }

        private InductorComponent SelectInductor(CalculationResults results)
        {
            // Buscar inductancia comercial más cercana con corriente adecuada
            double targetL = results.InductanceCommercial;
            double minIsat = results.PeakInductorCurrent * 1.3;
            double minIrms = results.RmsInductorCurrent * 1.2;

            var candidates = inductorDatabase
                .Where(i => Math.Abs(i.Inductance - targetL) / targetL < 0.2 &&
                           i.SaturationCurrent >= minIsat &&
                           i.RmsCurrent >= minIrms)
                .OrderBy(i => Math.Abs(i.Inductance - targetL))
                .ToList();

            return candidates.FirstOrDefault() ??
                   inductorDatabase.OrderBy(i => Math.Abs(i.Inductance - targetL)).First();
        }

        private CapacitorComponent SelectInputCapacitor(DesignParameters parameters)
        {
            // Seleccionar capacitor electrolítico con tensión adecuada
            double minVoltage = parameters.Vin * 1.2;

            var candidates = capacitorDatabase
                .Where(c => c.Type.Contains("Electrolytic") && c.Voltage >= minVoltage)
                .OrderByDescending(c => c.Capacitance)
                .ToList();

            return candidates.FirstOrDefault() ??
                   capacitorDatabase.First(c => c.Type.Contains("Electrolytic"));
        }

        private void SelectOutputCapacitors(CalculationResults results, DesignParameters parameters,
                                           SelectedComponents selected)
        {
            // Seleccionar capacitores cerámicos para salida
            double targetCap = results.OutputCapacitanceCommercial;
            double minVoltage = parameters.Vout * 1.5;

            var ceramicCaps = capacitorDatabase
                .Where(c => c.Type.Contains("Ceramic") && c.Voltage >= minVoltage)
                .OrderByDescending(c => c.Capacitance)
                .ToList();

            if (ceramicCaps.Any())
            {
                selected.OutputCapacitor = ceramicCaps.First();

                // Calcular cuántos en paralelo se necesitan
                selected.OutputCapacitorCount = (int)Math.Ceiling(targetCap / selected.OutputCapacitor.Capacitance);

                // Mínimo 2 capacitores
                if (selected.OutputCapacitorCount < 2)
                    selected.OutputCapacitorCount = 2;
            }
            else
            {
                // Fallback a primer capacitor disponible
                selected.OutputCapacitor = capacitorDatabase.First();
                selected.OutputCapacitorCount = 2;
            }
        }

        private PwmControllerComponent SelectPwmController(DesignParameters parameters)
        {
            // Seleccionar UC3843 si frecuencia es compatible
            var uc3843 = pwmDatabase.FirstOrDefault(p => p.PartNumber == "UC3843AN");

            if (uc3843 != null && parameters.Frequency <= uc3843.MaxFrequency)
            {
                return uc3843;
            }

            // Buscar alternativa compatible
            var candidates = pwmDatabase
                .Where(p => p.MaxFrequency >= parameters.Frequency)
                .OrderByDescending(p => p.MaxFrequency)
                .ToList();

            return candidates.FirstOrDefault() ?? pwmDatabase.First();
        }
    }
}