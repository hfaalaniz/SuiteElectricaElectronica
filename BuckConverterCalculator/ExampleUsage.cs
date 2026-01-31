using System;
using System.Windows.Forms;


namespace BuckConverterCalculator
{
    public partial class ExampleUsage : Form
    {
        private TemperatureProgressBar tempBar;
        private TrackBar slider;
        private System.Windows.Forms.Timer animationTimer;

        public ExampleUsage()
        {
           // InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Configurar UserControl
            tempBar = new TemperatureProgressBar
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(400, 60),
                Temperature = 0
            };

            // Slider para control manual
            slider = new TrackBar
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(400, 45),
                Minimum = 0,
                Maximum = 2500, // 250.0 * 10 para decimales
                TickFrequency = 100,
                Value = 0
            };
            slider.ValueChanged += (s, e) => 
            {
                tempBar.Temperature = slider.Value / 10f;
            };

            // Timer para animación
            animationTimer = new System.Windows.Forms.Timer { Interval = 50 };
            animationTimer.Tick += AnimationTimer_Tick;

            Button btnAnimate = new Button
            {
                Text = "Animar",
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAnimate.Click += (s, e) => animationTimer.Start();

            Button btnStop = new Button
            {
                Text = "Detener",
                Location = new System.Drawing.Point(130, 160),
                Size = new System.Drawing.Size(100, 30)
            };
            btnStop.Click += (s, e) => animationTimer.Stop();

            Controls.Add(tempBar);
            Controls.Add(slider);
            Controls.Add(btnAnimate);
            Controls.Add(btnStop);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float newTemp = tempBar.Temperature + 2.5f;
            if (newTemp > 250) newTemp = 0;
            
            tempBar.Temperature = newTemp;
            slider.Value = (int)(newTemp * 10);
        }
    }
}
