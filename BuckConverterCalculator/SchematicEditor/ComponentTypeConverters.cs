using System;
using System.ComponentModel;
using System.Globalization;

namespace BuckConverterCalculator.SchematicEditor
{
    /// <summary>
    /// TypeConverter para Logic Gate Type - permite selección desde lista desplegable
    /// </summary>
    public class LogicGateTypeConverter : StringConverter
    {
        private static readonly string[] GateTypes = new[]
        {
            "AND",
            "OR",
            "NOT",
            "NAND",
            "NOR",
            "XOR",
            "XNOR"
        };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Solo permite valores de la lista
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GateTypes);
        }
    }

    /// <summary>
    /// TypeConverter para Flip-Flop Type
    /// </summary>
    public class FlipFlopTypeConverter : StringConverter
    {
        private static readonly string[] FFTypes = new[]
        {
            "D",
            "JK",
            "SR",
            "T"
        };

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
            return new StandardValuesCollection(FFTypes);
        }
    }

    /// <summary>
    /// TypeConverter para MOSFET Channel Type
    /// </summary>
    public class MOSFETChannelTypeConverter : StringConverter
    {
        private static readonly string[] ChannelTypes = new[]
        {
            "N-Channel",
            "P-Channel"
        };

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
            return new StandardValuesCollection(ChannelTypes);
        }
    }

    /// <summary>
    /// TypeConverter para BJT Transistor Type
    /// </summary>
    public class BJTTransistorTypeConverter : StringConverter
    {
        private static readonly string[] TransistorTypes = new[]
        {
            "NPN",
            "PNP"
        };

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
            return new StandardValuesCollection(TransistorTypes);
        }
    }

    /// <summary>
    /// TypeConverter para LED Color
    /// </summary>
    public class LEDColorConverter : StringConverter
    {
        private static readonly string[] LEDColors = new[]
        {
            "Red",
            "Green",
            "Blue",
            "Yellow",
            "White",
            "Orange",
            "Infrared",
            "UV"
        };

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
            return new StandardValuesCollection(LEDColors);
        }
    }

    /// <summary>
    /// TypeConverter para 7-Segment Common Type
    /// </summary>
    public class SevenSegmentCommonTypeConverter : StringConverter
    {
        private static readonly string[] CommonTypes = new[]
        {
            "Common Cathode",
            "Common Anode"
        };

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
            return new StandardValuesCollection(CommonTypes);
        }
    }
}