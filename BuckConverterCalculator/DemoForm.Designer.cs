namespace BuckConverterCalculator
{
    partial class DemoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new Label();
            tempBar1 = new TemperatureProgressBar();
            slider1 = new TrackBar();
            label2 = new Label();
            tempBar2 = new TemperatureProgressBar();
            numericTemp = new NumericUpDown();
            btnPreset1 = new Button();
            btnPreset2 = new Button();
            btnPreset3 = new Button();
            btnPreset4 = new Button();
            label3 = new Label();
            tempBar3 = new TemperatureProgressBar();
            tempBar4 = new TemperatureProgressBar();
            tempBar5 = new TemperatureProgressBar();
            label4 = new Label();
            tempBar6 = new TemperatureProgressBar();
            btnStart = new Button();
            btnStop = new Button();
            btnReset = new Button();
            animationTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)slider1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericTemp).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.Location = new Point(20, 20);
            label1.Name = "label1";
            label1.Size = new Size(291, 19);
            label1.TabIndex = 0;
            label1.Text = "1. Horizontal (0-250°C, BorderRadius=12):";
            // 
            // tempBar1
            // 
            tempBar1.BorderRadius = 10;
            tempBar1.Location = new Point(20, 45);
            tempBar1.Margin = new Padding(4, 3, 4, 3);
            tempBar1.Name = "tempBar1";
            tempBar1.Size = new Size(868, 40);
            tempBar1.TabIndex = 1;
            tempBar1.Temperature = 75F;
            // 
            // slider1
            // 
            slider1.Location = new Point(20, 90);
            slider1.Maximum = 2500;
            slider1.Name = "slider1";
            slider1.Size = new Size(868, 45);
            slider1.TabIndex = 2;
            slider1.TickFrequency = 250;
            slider1.Value = 750;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(20, 122);
            label2.Name = "label2";
            label2.Size = new Size(307, 19);
            label2.TabIndex = 3;
            label2.Text = "2. Horizontal (-20 a 100°C, BorderRadius=8):";
            // 
            // tempBar2
            // 
            tempBar2.BorderRadius = 5;
            tempBar2.Location = new Point(20, 147);
            tempBar2.Margin = new Padding(4, 3, 4, 3);
            tempBar2.MaxTemperature = 100F;
            tempBar2.MinTemperature = -20F;
            tempBar2.Name = "tempBar2";
            tempBar2.Size = new Size(868, 40);
            tempBar2.TabIndex = 4;
            tempBar2.Temperature = 20F;
            // 
            // numericTemp
            // 
            numericTemp.DecimalPlaces = 1;
            numericTemp.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            numericTemp.Location = new Point(28, 223);
            numericTemp.Minimum = new decimal(new int[] { 20, 0, 0, int.MinValue });
            numericTemp.Name = "numericTemp";
            numericTemp.Size = new Size(150, 23);
            numericTemp.TabIndex = 5;
            numericTemp.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // btnPreset1
            // 
            btnPreset1.Location = new Point(188, 223);
            btnPreset1.Name = "btnPreset1";
            btnPreset1.Size = new Size(70, 28);
            btnPreset1.TabIndex = 6;
            btnPreset1.Text = "-10°C";
            btnPreset1.UseVisualStyleBackColor = true;
            // 
            // btnPreset2
            // 
            btnPreset2.Location = new Point(263, 223);
            btnPreset2.Name = "btnPreset2";
            btnPreset2.Size = new Size(70, 28);
            btnPreset2.TabIndex = 7;
            btnPreset2.Text = "0°C";
            btnPreset2.UseVisualStyleBackColor = true;
            // 
            // btnPreset3
            // 
            btnPreset3.Location = new Point(338, 223);
            btnPreset3.Name = "btnPreset3";
            btnPreset3.Size = new Size(70, 28);
            btnPreset3.TabIndex = 8;
            btnPreset3.Text = "25°C";
            btnPreset3.UseVisualStyleBackColor = true;
            // 
            // btnPreset4
            // 
            btnPreset4.Location = new Point(413, 223);
            btnPreset4.Name = "btnPreset4";
            btnPreset4.Size = new Size(70, 28);
            btnPreset4.TabIndex = 9;
            btnPreset4.Text = "80°C";
            btnPreset4.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.Location = new Point(28, 273);
            label3.Name = "label3";
            label3.Size = new Size(287, 19);
            label3.TabIndex = 10;
            label3.Text = "3. Verticales con diferentes BorderRadius:";
            // 
            // tempBar3
            // 
            tempBar3.BorderRadius = 6;
            tempBar3.Location = new Point(38, 298);
            tempBar3.Margin = new Padding(4, 3, 4, 3);
            tempBar3.Name = "tempBar3";
            tempBar3.Orientation = TemperatureOrientation.Vertical;
            tempBar3.Size = new Size(60, 327);
            tempBar3.TabIndex = 11;
            tempBar3.Temperature = 35F;
            // 
            // tempBar4
            // 
            tempBar4.BorderRadius = 10;
            tempBar4.Location = new Point(138, 298);
            tempBar4.Margin = new Padding(4, 3, 4, 3);
            tempBar4.Name = "tempBar4";
            tempBar4.Orientation = TemperatureOrientation.Vertical;
            tempBar4.Size = new Size(60, 327);
            tempBar4.TabIndex = 12;
            tempBar4.Temperature = 120F;
            // 
            // tempBar5
            // 
            tempBar5.BorderRadius = 10;
            tempBar5.Location = new Point(251, 298);
            tempBar5.Margin = new Padding(4, 3, 4, 3);
            tempBar5.Name = "tempBar5";
            tempBar5.Orientation = TemperatureOrientation.Vertical;
            tempBar5.Size = new Size(60, 327);
            tempBar5.TabIndex = 13;
            tempBar5.Temperature = 200F;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.Location = new Point(458, 273);
            label4.Name = "label4";
            label4.Size = new Size(166, 19);
            label4.TabIndex = 14;
            label4.Text = "4. Animación (0-500°C):";
            // 
            // tempBar6
            // 
            tempBar6.BorderRadius = 10;
            tempBar6.EnableMouseControl = false;
            tempBar6.Location = new Point(469, 298);
            tempBar6.Margin = new Padding(4, 3, 4, 3);
            tempBar6.MaxTemperature = 300F;
            tempBar6.Name = "tempBar6";
            tempBar6.Size = new Size(418, 51);
            tempBar6.TabIndex = 15;
            tempBar6.Temperature = 0F;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(542, 366);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(90, 32);
            btnStart.TabIndex = 16;
            btnStart.Text = "▶ Iniciar";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(642, 366);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(90, 32);
            btnStop.TabIndex = 17;
            btnStop.Text = "⏸ Detener";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(742, 366);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(90, 32);
            btnReset.TabIndex = 18;
            btnReset.Text = "⏹ Reiniciar";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // animationTimer
            // 
            animationTimer.Interval = 30;
            // 
            // DemoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 252);
            ClientSize = new Size(900, 635);
            Controls.Add(btnReset);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(tempBar6);
            Controls.Add(label4);
            Controls.Add(tempBar5);
            Controls.Add(tempBar4);
            Controls.Add(tempBar3);
            Controls.Add(label3);
            Controls.Add(btnPreset4);
            Controls.Add(btnPreset3);
            Controls.Add(btnPreset2);
            Controls.Add(btnPreset1);
            Controls.Add(numericTemp);
            Controls.Add(tempBar2);
            Controls.Add(label2);
            Controls.Add(slider1);
            Controls.Add(tempBar1);
            Controls.Add(label1);
            Name = "DemoForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Demo - Temperature Progress Bar";
            ((System.ComponentModel.ISupportInitialize)slider1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericTemp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private BuckConverterCalculator.TemperatureProgressBar tempBar1;
        private System.Windows.Forms.TrackBar slider1;
        private System.Windows.Forms.Label label2;
        private BuckConverterCalculator.TemperatureProgressBar tempBar2;
        private System.Windows.Forms.NumericUpDown numericTemp;
        private System.Windows.Forms.Button btnPreset1;
        private System.Windows.Forms.Button btnPreset2;
        private System.Windows.Forms.Button btnPreset3;
        private System.Windows.Forms.Button btnPreset4;
        private System.Windows.Forms.Label label3;
        private BuckConverterCalculator.TemperatureProgressBar tempBar3;
        private BuckConverterCalculator.TemperatureProgressBar tempBar4;
        private BuckConverterCalculator.TemperatureProgressBar tempBar5;
        private System.Windows.Forms.Label label4;
        private BuckConverterCalculator.TemperatureProgressBar tempBar6;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Timer animationTimer;
    }
}
