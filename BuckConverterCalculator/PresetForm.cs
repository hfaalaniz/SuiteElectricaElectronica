using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    /// <summary>
    /// Formulario de configuraciones predefinidas
    /// </summary>
    public partial class PresetForm : Form
    {
        public PresetConfiguration SelectedPreset { get; private set; }

        private List<PresetConfiguration> presets;

        public PresetForm()
        {
            InitializeComponent();
            LoadPresets();
            PopulateList();
        }

        private void LoadPresets()
        {
            presets = new List<PresetConfiguration>
            {
                new PresetConfiguration
                {
                    Name = "90V → 13V @ 5A (Original)",
                    Description = "Configuración original del Buck Converter. Ideal para aplicaciones de alto voltaje con salida de 13V.",
                    Vin = 90,
                    Vout = 13,
                    Iout = 5,
                    Frequency = 100000
                },
                new PresetConfiguration
                {
                    Name = "48V → 12V @ 5A (Telecom)",
                    Description = "Configuración típica para equipos de telecomunicaciones. Entrada de 48V estándar.",
                    Vin = 48,
                    Vout = 12,
                    Iout = 5,
                    Frequency = 100000
                },
                new PresetConfiguration
                {
                    Name = "24V → 5V @ 3A (Industrial)",
                    Description = "Configuración industrial común. Conversión de 24V a 5V para electrónica digital.",
                    Vin = 24,
                    Vout = 5,
                    Iout = 3,
                    Frequency = 200000
                },
                new PresetConfiguration
                {
                    Name = "12V → 5V @ 2A (Automotriz)",
                    Description = "Configuración para aplicaciones automotrices. Batería 12V a 5V USB.",
                    Vin = 12,
                    Vout = 5,
                    Iout = 2,
                    Frequency = 250000
                },
                new PresetConfiguration
                {
                    Name = "48V → 12V @ 10A (Alta Corriente)",
                    Description = "Configuración de alta corriente para aplicaciones de potencia. 120W.",
                    Vin = 48,
                    Vout = 12,
                    Iout = 10,
                    Frequency = 100000
                },
                new PresetConfiguration
                {
                    Name = "36V → 9V @ 1A (Baja Potencia)",
                    Description = "Configuración de baja potencia para dispositivos portátiles.",
                    Vin = 36,
                    Vout = 9,
                    Iout = 1,
                    Frequency = 300000
                },
                new PresetConfiguration
                {
                    Name = "60V → 24V @ 4A (LED Driver)",
                    Description = "Configuración para driver de LEDs de alta potencia.",
                    Vin = 60,
                    Vout = 24,
                    Iout = 4,
                    Frequency = 150000
                },
                new PresetConfiguration
                {
                    Name = "100V → 15V @ 6A (Alta Tensión)",
                    Description = "Configuración de alta tensión de entrada. 90W de potencia.",
                    Vin = 100,
                    Vout = 15,
                    Iout = 6,
                    Frequency = 100000
                }
            };
        }

        private void PopulateList()
        {
            listBoxPresets.Items.Clear();
            foreach (var preset in presets)
            {
                listBoxPresets.Items.Add(preset.Name);
            }

            if (listBoxPresets.Items.Count > 0)
            {
                listBoxPresets.SelectedIndex = 0;
            }
        }

        private void listBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxPresets.SelectedIndex >= 0)
            {
                var preset = presets[listBoxPresets.SelectedIndex];
                textBoxDescription.Text = preset.Description;

                labelVin.Text = $"Vin:  {preset.Vin} V";
                labelVout.Text = $"Vout: {preset.Vout} V";
                labelIout.Text = $"Iout: {preset.Iout} A";
                labelFreq.Text = $"Freq: {preset.Frequency / 1000} kHz";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listBoxPresets.SelectedIndex >= 0)
            {
                SelectedPreset = presets[listBoxPresets.SelectedIndex];
            }
        }
    }

    /// <summary>
    /// Clase de configuración predefinida
    /// </summary>
    public class PresetConfiguration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Vin { get; set; }
        public double Vout { get; set; }
        public double Iout { get; set; }
        public double Frequency { get; set; }
    }
}
