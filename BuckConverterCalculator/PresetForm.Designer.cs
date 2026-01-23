namespace BuckConverterCalculator
{
    partial class PresetForm
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
            this.listBoxPresets = new System.Windows.Forms.ListBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.labelVin = new System.Windows.Forms.Label();
            this.labelVout = new System.Windows.Forms.Label();
            this.labelIout = new System.Windows.Forms.Label();
            this.labelFreq = new System.Windows.Forms.Label();
            this.groupBoxDetails.SuspendLayout();
            this.SuspendLayout();

            // listBoxPresets
            this.listBoxPresets.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBoxPresets.FormattingEnabled = true;
            this.listBoxPresets.ItemHeight = 15;
            this.listBoxPresets.Location = new System.Drawing.Point(12, 12);
            this.listBoxPresets.Name = "listBoxPresets";
            this.listBoxPresets.Size = new System.Drawing.Size(300, 214);
            this.listBoxPresets.TabIndex = 0;
            this.listBoxPresets.SelectedIndexChanged += new System.EventHandler(this.listBoxPresets_SelectedIndexChanged);

            // labelDescription
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelDescription.Location = new System.Drawing.Point(12, 240);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(74, 15);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "Descripción:";

            // textBoxDescription
            this.textBoxDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxDescription.Location = new System.Drawing.Point(12, 260);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(300, 60);
            this.textBoxDescription.TabIndex = 2;

            // groupBoxDetails
            this.groupBoxDetails.Controls.Add(this.labelFreq);
            this.groupBoxDetails.Controls.Add(this.labelIout);
            this.groupBoxDetails.Controls.Add(this.labelVout);
            this.groupBoxDetails.Controls.Add(this.labelVin);
            this.groupBoxDetails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxDetails.Location = new System.Drawing.Point(330, 12);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(200, 150);
            this.groupBoxDetails.TabIndex = 3;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Parámetros";

            // labelVin
            this.labelVin.AutoSize = true;
            this.labelVin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelVin.Location = new System.Drawing.Point(15, 30);
            this.labelVin.Name = "labelVin";
            this.labelVin.Size = new System.Drawing.Size(28, 15);
            this.labelVin.TabIndex = 0;
            this.labelVin.Text = "Vin: ";

            // labelVout
            this.labelVout.AutoSize = true;
            this.labelVout.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelVout.Location = new System.Drawing.Point(15, 55);
            this.labelVout.Name = "labelVout";
            this.labelVout.Size = new System.Drawing.Size(36, 15);
            this.labelVout.TabIndex = 1;
            this.labelVout.Text = "Vout:";

            // labelIout
            this.labelIout.AutoSize = true;
            this.labelIout.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelIout.Location = new System.Drawing.Point(15, 80);
            this.labelIout.Name = "labelIout";
            this.labelIout.Size = new System.Drawing.Size(32, 15);
            this.labelIout.TabIndex = 2;
            this.labelIout.Text = "Iout:";

            // labelFreq
            this.labelFreq.AutoSize = true;
            this.labelFreq.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFreq.Location = new System.Drawing.Point(15, 105);
            this.labelFreq.Name = "labelFreq";
            this.labelFreq.Size = new System.Drawing.Size(33, 15);
            this.labelFreq.TabIndex = 3;
            this.labelFreq.Text = "Freq:";

            // btnOK
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOK.Location = new System.Drawing.Point(330, 280);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 40);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Aceptar";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(440, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;

            // PresetForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 336);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.listBoxPresets);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PresetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuraciones Predefinidas";
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ListBox listBoxPresets;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.Label labelVin;
        private System.Windows.Forms.Label labelVout;
        private System.Windows.Forms.Label labelIout;
        private System.Windows.Forms.Label labelFreq;
        #endregion
    }
}