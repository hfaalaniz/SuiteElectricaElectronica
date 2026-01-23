using BuckConverterCalculator.Models;
using BuckConverterCalculator.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace BuckConverterCalculator.Controls
{
    public partial class SymbolToolbox : UserControl
    {
        private class SymbolButton : Button
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public SymbolType SymbolType { get; set; }

            public SymbolButton(SymbolType type, string text)
            {
                this.SymbolType = type;
                this.Text = text;
                this.Size = new Size(100, 80);
                this.Margin = new Padding(5);
                this.MouseDown += SymbolButton_MouseDown;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                var symbol = new ElectricalSymbol
                {
                    Type = this.SymbolType,
                    X = 20,
                    Y = 10,
                    Width = 60,
                    Height = 50
                };
                symbol.InitializeConnectionPoints();

                e.Graphics.Clear(this.BackColor);
                SymbolRenderer.DrawSymbol(e.Graphics, symbol);
            }

            private void SymbolButton_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.DoDragDrop(this.SymbolType, DragDropEffects.Copy);
                }
            }
        }

        public SymbolToolbox()
        {
            //InitializeComponent();
            InitializeToolbox();
        }

        private void InitializeToolbox()
        {
            this.AutoScroll = true;

            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            var label = new Label
            {
                Text = "Símbolos Eléctricos",
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(5, 5, 5, 10)
            };
            flowPanel.Controls.Add(label);

            foreach (SymbolType type in Enum.GetValues(typeof(SymbolType)))
            {
                flowPanel.Controls.Add(new SymbolButton(type, type.ToString()));
            }

            this.Controls.Add(flowPanel);
        }
    }
}