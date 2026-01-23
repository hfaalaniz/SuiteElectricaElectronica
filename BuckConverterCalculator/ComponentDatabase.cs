using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BuckConverterDesign
{
    /// <summary>
    /// Base de datos de componentes con especificaciones reales
    /// </summary>
    public static class ComponentDatabase
    {
        // ==================== DIODOS ====================
        public static Dictionary<string, DiodeSpec> Diodes = new Dictionary<string, DiodeSpec>
        {
            // Diodos Schottky (para Buck Converters)
            { "MBRS360", new DiodeSpec {
                PartNumber = "MBRS360",
                DiodeType = "Schottky",
                VoltageRating = "60V",
                CurrentRating = "3A",
                VF = "0.5V",
                Package = "DO-214AA"
            }},
            { "1N5822", new DiodeSpec {
                PartNumber = "1N5822",
                DiodeType = "Schottky",
                VoltageRating = "40V",
                CurrentRating = "3A",
                VF = "0.525V",
                Package = "DO-201AD"
            }},
            { "MBR20100CT", new DiodeSpec {
                PartNumber = "MBR20100CT",
                DiodeType = "Schottky",
                VoltageRating = "100V",
                CurrentRating = "10A",
                VF = "0.85V",
                Package = "TO-220AB"
            }},
            { "SS34", new DiodeSpec {
                PartNumber = "SS34",
                DiodeType = "Schottky",
                VoltageRating = "40V",
                CurrentRating = "3A",
                VF = "0.5V",
                Package = "DO-214AC"
            }},
            
            // Diodos rápidos
            { "1N4148", new DiodeSpec {
                PartNumber = "1N4148",
                DiodeType = "Fast",
                VoltageRating = "100V",
                CurrentRating = "200mA",
                VF = "0.7V",
                Package = "DO-35"
            }},
            { "UF4007", new DiodeSpec {
                PartNumber = "UF4007",
                DiodeType = "Ultrafast",
                VoltageRating = "1000V",
                CurrentRating = "1A",
                VF = "1.7V",
                Package = "DO-41"
            }},
            { "STTH3006", new DiodeSpec {
                PartNumber = "STTH3006",
                DiodeType = "Ultrafast",
                VoltageRating = "600V",
                CurrentRating = "30A",
                VF = "1.4V",
                Package = "TO-247"
            }}
        };

        // ==================== MOSFETs ====================
        public static Dictionary<string, MosfetSpec> Mosfets = new Dictionary<string, MosfetSpec>
        {
            // N-Channel MOSFETs (para Buck Converters)
            { "IRF540N", new MosfetSpec {
                PartNumber = "IRF540N",
                ChannelType = "N-Channel",
                VDS = "100V",
                ID = "33A",
                RDSon = "44mΩ",
                VGS = "±20V",
                QG = "72nC",
                Package = "TO-220AB"
            }},
            { "IRFZ44N", new MosfetSpec {
                PartNumber = "IRFZ44N",
                ChannelType = "N-Channel",
                VDS = "55V",
                ID = "49A",
                RDSon = "17.5mΩ",
                VGS = "±20V",
                QG = "63nC",
                Package = "TO-220AB"
            }},
            { "FDP047AN08A0", new MosfetSpec {
                PartNumber = "FDP047AN08A0",
                ChannelType = "N-Channel",
                VDS = "80V",
                ID = "120A",
                RDSon = "4.7mΩ",
                VGS = "±20V",
                QG = "85nC",
                Package = "TO-220"
            }},
            { "IPP075N15N3", new MosfetSpec {
                PartNumber = "IPP075N15N3",
                ChannelType = "N-Channel",
                VDS = "150V",
                ID = "46A",
                RDSon = "7.5mΩ",
                VGS = "±20V",
                QG = "130nC",
                Package = "TO-220"
            }},
            
            // P-Channel MOSFETs
            { "IRF9540N", new MosfetSpec {
                PartNumber = "IRF9540N",
                ChannelType = "P-Channel",
                VDS = "-100V",
                ID = "-23A",
                RDSon = "117mΩ",
                VGS = "±20V",
                QG = "72nC",
                Package = "TO-220AB"
            }},
            { "FQP47P06", new MosfetSpec {
                PartNumber = "FQP47P06",
                ChannelType = "P-Channel",
                VDS = "-60V",
                ID = "-47A",
                RDSon = "21mΩ",
                VGS = "±25V",
                QG = "120nC",
                Package = "TO-220"
            }}
        };

        // ==================== BJTs ====================
        public static Dictionary<string, BJTSpec> BJTs = new Dictionary<string, BJTSpec>
        {
            // NPN BJTs
            { "2N2222A", new BJTSpec {
                PartNumber = "2N2222A",
                TransistorType = "NPN",
                VCE = "40V",
                IC = "800mA",
                PowerRating = "500mW",
                HFE = "100-300",
                Package = "TO-18"
            }},
            { "2N3904", new BJTSpec {
                PartNumber = "2N3904",
                TransistorType = "NPN",
                VCE = "40V",
                IC = "200mA",
                PowerRating = "625mW",
                HFE = "100-300",
                Package = "TO-92"
            }},
            { "BC547", new BJTSpec {
                PartNumber = "BC547",
                TransistorType = "NPN",
                VCE = "45V",
                IC = "100mA",
                PowerRating = "500mW",
                HFE = "110-800",
                Package = "TO-92"
            }},
            { "TIP41C", new BJTSpec {
                PartNumber = "TIP41C",
                TransistorType = "NPN",
                VCE = "100V",
                IC = "6A",
                PowerRating = "65W",
                HFE = "15-75",
                Package = "TO-220"
            }},
            
            // PNP BJTs
            { "2N2907A", new BJTSpec {
                PartNumber = "2N2907A",
                TransistorType = "PNP",
                VCE = "-40V",
                IC = "-600mA",
                PowerRating = "400mW",
                HFE = "100-300",
                Package = "TO-18"
            }},
            { "2N3906", new BJTSpec {
                PartNumber = "2N3906",
                TransistorType = "PNP",
                VCE = "-40V",
                IC = "-200mA",
                PowerRating = "625mW",
                HFE = "100-300",
                Package = "TO-92"
            }},
            { "BC557", new BJTSpec {
                PartNumber = "BC557",
                TransistorType = "PNP",
                VCE = "-45V",
                IC = "-100mA",
                PowerRating = "500mW",
                HFE = "110-800",
                Package = "TO-92"
            }},
            { "TIP42C", new BJTSpec {
                PartNumber = "TIP42C",
                TransistorType = "PNP",
                VCE = "-100V",
                IC = "-6A",
                PowerRating = "65W",
                HFE = "15-75",
                Package = "TO-220"
            }}
        };

        // ==================== ICs ====================
        public static Dictionary<string, ICSpec> ICs = new Dictionary<string, ICSpec>
        {
            // PWM Controllers
            { "TL494", new ICSpec {
                PartNumber = "TL494",
                Function = "PWM Controller",
                SupplyVoltage = "7-40V",
                PinCount = 16,
                Features = "Dual PWM, 200kHz max",
                Package = "DIP-16"
            }},
            { "UC3842", new ICSpec {
                PartNumber = "UC3842",
                Function = "PWM Controller",
                SupplyVoltage = "10-30V",
                PinCount = 8,
                Features = "Current Mode, 500kHz",
                Package = "DIP-8"
            }},
            { "LM2596", new ICSpec {
                PartNumber = "LM2596",
                Function = "Buck Regulator",
                SupplyVoltage = "4.5-40V",
                PinCount = 5,
                Features = "3A, 150kHz, Integrated",
                Package = "TO-220"
            }},
            { "MC34063", new ICSpec {
                PartNumber = "MC34063",
                Function = "Buck/Boost Regulator",
                SupplyVoltage = "3-40V",
                PinCount = 8,
                Features = "1.5A, 100kHz",
                Package = "DIP-8"
            }},
            
            // Linear Regulators
            { "LM7805", new ICSpec {
                PartNumber = "LM7805",
                Function = "Linear Regulator",
                SupplyVoltage = "7-35V",
                PinCount = 3,
                Features = "5V, 1A",
                Package = "TO-220"
            }},
            { "LM317", new ICSpec {
                PartNumber = "LM317",
                Function = "Adjustable Regulator",
                SupplyVoltage = "3-40V",
                PinCount = 3,
                Features = "1.25-37V, 1.5A",
                Package = "TO-220"
            }},
            
            // Op-Amps
            { "LM358", new ICSpec {
                PartNumber = "LM358",
                Function = "Dual Op-Amp",
                SupplyVoltage = "3-32V",
                PinCount = 8,
                Features = "Dual, Low Power",
                Package = "DIP-8"
            }},
            { "TL072", new ICSpec {
                PartNumber = "TL072",
                Function = "Dual Op-Amp",
                SupplyVoltage = "±18V",
                PinCount = 8,
                Features = "Low Noise, JFET",
                Package = "DIP-8"
            }}
        };

        // Métodos helper
        public static string[] GetDiodePartNumbers()
        {
            return Diodes.Keys.OrderBy(k => k).ToArray();
        }

        public static string[] GetMosfetPartNumbers()
        {
            return Mosfets.Keys.OrderBy(k => k).ToArray();
        }

        public static string[] GetBJTPartNumbers()
        {
            return BJTs.Keys.OrderBy(k => k).ToArray();
        }

        public static string[] GetICPartNumbers()
        {
            return ICs.Keys.OrderBy(k => k).ToArray();
        }
    }

    // ==================== ESPECIFICACIONES ====================

    [Serializable]
    public class DiodeSpec
    {
        public string PartNumber { get; set; }
        public string DiodeType { get; set; }
        public string VoltageRating { get; set; }
        public string CurrentRating { get; set; }
        public string VF { get; set; }
        public string Package { get; set; }
    }

    [Serializable]
    public class MosfetSpec
    {
        public string PartNumber { get; set; }
        public string ChannelType { get; set; }
        public string VDS { get; set; }
        public string ID { get; set; }
        public string RDSon { get; set; }
        public string VGS { get; set; }
        public string QG { get; set; }
        public string Package { get; set; }
    }

    [Serializable]
    public class BJTSpec
    {
        public string PartNumber { get; set; }
        public string TransistorType { get; set; }
        public string VCE { get; set; }
        public string IC { get; set; }
        public string PowerRating { get; set; }
        public string HFE { get; set; }
        public string Package { get; set; }
    }

    [Serializable]
    public class ICSpec
    {
        public string PartNumber { get; set; }
        public string Function { get; set; }
        public string SupplyVoltage { get; set; }
        public int PinCount { get; set; }
        public string Features { get; set; }
        public string Package { get; set; }
    }

    // ==================== TYPE CONVERTERS ====================

    /// <summary>
    /// TypeConverter para mostrar dropdown de part numbers de Diodos
    /// </summary>
    public class DiodePartNumberConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ComponentDatabase.GetDiodePartNumbers());
        }
    }

    /// <summary>
    /// TypeConverter para mostrar dropdown de part numbers de MOSFETs
    /// </summary>
    public class MosfetPartNumberConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ComponentDatabase.GetMosfetPartNumbers());
        }
    }

    /// <summary>
    /// TypeConverter para mostrar dropdown de part numbers de BJTs
    /// </summary>
    public class BJTPartNumberConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ComponentDatabase.GetBJTPartNumbers());
        }
    }

    /// <summary>
    /// TypeConverter para mostrar dropdown de part numbers de ICs
    /// </summary>
    public class ICPartNumberConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ComponentDatabase.GetICPartNumbers());
        }
    }
}