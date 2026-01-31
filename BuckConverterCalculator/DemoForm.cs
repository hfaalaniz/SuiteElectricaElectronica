using System;
using System.Drawing;
using System.Windows.Forms;


namespace BuckConverterCalculator
{
    public partial class DemoForm : Form
    {
        private bool isAnimating = false;
        private float animationDirection = 1f;

        public DemoForm()
        {
            InitializeComponent();
            ConfigureEventHandlers();
        }

        private void ConfigureEventHandlers()
        {
            // TrackBar 1 sincronizado con tempBar1
            slider1.ValueChanged += (s, e) => tempBar1.Temperature = slider1.Value / 10f;
            tempBar1.TemperatureChanged += (s, e) => slider1.Value = (int)(tempBar1.Temperature * 10);

            // NumericUpDown sincronizado con tempBar2
            numericTemp.ValueChanged += (s, e) => tempBar2.Temperature = (float)numericTemp.Value;
            tempBar2.TemperatureChanged += (s, e) => numericTemp.Value = (decimal)tempBar2.Temperature;

            // Botones de preset
            btnPreset1.Click += (s, e) => numericTemp.Value = -10;
            btnPreset2.Click += (s, e) => numericTemp.Value = 0;
            btnPreset3.Click += (s, e) => numericTemp.Value = 25;
            btnPreset4.Click += (s, e) => numericTemp.Value = 80;

            // Botones de animación
            btnStart.Click += (s, e) => StartAnimation();
            btnStop.Click += (s, e) => StopAnimation();
            btnReset.Click += (s, e) => ResetAnimation();

            // Timer
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void StartAnimation()
        {
            isAnimating = true;
            animationTimer.Start();
        }

        private void StopAnimation()
        {
            isAnimating = false;
            animationTimer.Stop();
        }

        private void ResetAnimation()
        {
            StopAnimation();
            tempBar6.Temperature = tempBar6.MinTemperature;
            animationDirection = 1f;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            float newTemp = tempBar6.Temperature + (5f * animationDirection);
            
            if (newTemp >= tempBar6.MaxTemperature)
            {
                newTemp = tempBar6.MaxTemperature;
                animationDirection = -1f;
            }
            else if (newTemp <= tempBar6.MinTemperature)
            {
                newTemp = tempBar6.MinTemperature;
                animationDirection = 1f;
            }
            
            tempBar6.Temperature = newTemp;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
