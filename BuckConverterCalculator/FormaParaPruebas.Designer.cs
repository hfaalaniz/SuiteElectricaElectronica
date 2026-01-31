namespace BuckConverterCalculator
{
    partial class FormaParaPruebas
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
            button1 = new Button();
            checkedListBox1 = new CheckedListBox();
            dateTimePicker1 = new DateTimePicker();
            dataGridView1 = new DataGridView();
            temperatureProgressBar1 = new TemperatureProgressBar();
            temperatureProgressBar2 = new TemperatureProgressBar();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(9, 249);
            button1.Name = "button1";
            button1.Size = new Size(347, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(9, 79);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(120, 94);
            checkedListBox1.TabIndex = 1;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(9, 196);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(347, 23);
            dateTimePicker1.TabIndex = 2;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(135, 79);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(221, 94);
            dataGridView1.TabIndex = 3;
            // 
            // temperatureProgressBar1
            // 
            temperatureProgressBar1.BorderRadius = 5;
            temperatureProgressBar1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            temperatureProgressBar1.Location = new Point(9, 10);
            temperatureProgressBar1.MaxTemperature = 500F;
            temperatureProgressBar1.MinTemperature = -50F;
            temperatureProgressBar1.Name = "temperatureProgressBar1";
            temperatureProgressBar1.Size = new Size(347, 39);
            temperatureProgressBar1.TabIndex = 4;
            temperatureProgressBar1.Temperature = -30F;
            // 
            // temperatureProgressBar2
            // 
            temperatureProgressBar2.BorderRadius = 5;
            temperatureProgressBar2.Location = new Point(372, 10);
            temperatureProgressBar2.Margin = new Padding(4, 3, 4, 3);
            temperatureProgressBar2.Name = "temperatureProgressBar2";
            temperatureProgressBar2.Orientation = TemperatureOrientation.Vertical;
            temperatureProgressBar2.Size = new Size(64, 262);
            temperatureProgressBar2.TabIndex = 5;
            temperatureProgressBar2.Temperature = 0F;
            // 
            // FormaParaPruebas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(521, 387);
            Controls.Add(temperatureProgressBar2);
            Controls.Add(temperatureProgressBar1);
            Controls.Add(dataGridView1);
            Controls.Add(dateTimePicker1);
            Controls.Add(checkedListBox1);
            Controls.Add(button1);
            Name = "FormaParaPruebas";
            Text = "FormaParaPruebas";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private CheckedListBox checkedListBox1;
        private DateTimePicker dateTimePicker1;
        private DataGridView dataGridView1;
        private TemperatureProgressBar temperatureProgressBar1;
        private TemperatureProgressBar temperatureProgressBar2;
    }
}