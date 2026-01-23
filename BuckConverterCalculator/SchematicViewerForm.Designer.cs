namespace BuckConverterCalculator
{
    partial class SchematicViewerForm
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
            canvasPanel = new Panel();
            schemaPictureBox = new PictureBox();
            btnExportImage = new Button();
            btnClose = new Button();
            lblTitle = new Label();
            cmbZoom = new ComboBox();
            lblZoom = new Label();
            hScrollBar = new HScrollBar();
            vScrollBar = new VScrollBar();
            canvasPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)schemaPictureBox).BeginInit();
            SuspendLayout();
            // 
            // canvasPanel
            // 
            canvasPanel.AutoScroll = true;
            canvasPanel.BackColor = Color.White;
            canvasPanel.BorderStyle = BorderStyle.FixedSingle;
            canvasPanel.Controls.Add(schemaPictureBox);
            canvasPanel.Location = new Point(12, 95);
            canvasPanel.Name = "canvasPanel";
            canvasPanel.Size = new Size(1352, 600);
            canvasPanel.TabIndex = 1;
            // 
            // schemaPictureBox
            // 
            schemaPictureBox.Location = new Point(0, 0);
            schemaPictureBox.Name = "schemaPictureBox";
            schemaPictureBox.Size = new Size(228, 143);
            schemaPictureBox.TabIndex = 0;
            schemaPictureBox.TabStop = false;
            schemaPictureBox.MouseDown += schemaPictureBox_MouseDown;
            schemaPictureBox.MouseMove += schemaPictureBox_MouseMove;
            schemaPictureBox.MouseUp += schemaPictureBox_MouseUp;
            // 
            // btnExportImage
            // 
            btnExportImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportImage.Location = new Point(342, 54);
            btnExportImage.Name = "btnExportImage";
            btnExportImage.Size = new Size(133, 29);
            btnExportImage.TabIndex = 4;
            btnExportImage.Text = "💾 Exportar PNG";
            btnExportImage.UseVisualStyleBackColor = true;
            btnExportImage.Click += btnExportImage_Click;
            // 
            // btnClose
            // 
            btnClose.Font = new Font("Segoe UI", 9F);
            btnClose.Location = new Point(196, 58);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(118, 23);
            btnClose.TabIndex = 5;
            btnClose.Text = "✖ Cerrar";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.FromArgb(0, 120, 215);
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1400, 50);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Esquemático Buck Converter - Vista Profesional";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.Font = new Font("Segoe UI", 9F);
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "25%", "50%", "75%", "100%", "125%", "150%", "200%", "300%", "400%" });
            cmbZoom.Location = new Point(70, 60);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(100, 23);
            cmbZoom.TabIndex = 3;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // lblZoom
            // 
            lblZoom.AutoSize = true;
            lblZoom.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblZoom.Location = new Point(12, 60);
            lblZoom.Name = "lblZoom";
            lblZoom.Size = new Size(53, 19);
            lblZoom.TabIndex = 2;
            lblZoom.Text = "Zoom:";
            // 
            // hScrollBar
            // 
            hScrollBar.Location = new Point(20, 20);
            hScrollBar.Name = "hScrollBar";
            hScrollBar.Size = new Size(500, 17);
            hScrollBar.TabIndex = 0;
            // 
            // vScrollBar
            // 
            vScrollBar.Location = new Point(0, 0);
            vScrollBar.Name = "vScrollBar";
            vScrollBar.Size = new Size(17, 80);
            vScrollBar.TabIndex = 0;
            // 
            // SchematicViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1370, 749);
            Controls.Add(btnClose);
            Controls.Add(btnExportImage);
            Controls.Add(cmbZoom);
            Controls.Add(lblZoom);
            Controls.Add(canvasPanel);
            Controls.Add(lblTitle);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "SchematicViewerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Visualizador de Esquemático - Buck Converter";
            canvasPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)schemaPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();


        }

        private Panel canvasPanel;
        private PictureBox schemaPictureBox;
        private Button btnExportImage;
        private Button btnClose;
        private Label lblTitle;
        private ComboBox cmbZoom;


        #endregion
    }
}