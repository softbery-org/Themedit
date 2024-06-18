// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace SfbLibrary.Components
{
    public delegate void SelectedDelegate(object sender, PanelLabelEventArgs e);

    public class RotatedLabel : System.Windows.Forms.Label
    {
        private System.Windows.Forms.Label _label;
        private string[] _newText;
        private bool _selected = false;

        public event SelectedDelegate SelectPanelEvent;
        [Category("Text rotate")]
        public float RotateAngle { get; set; }
        [Category("Text rotate")]
        public string[] RotatedText { get => _newText; set => _newText = value; }

        /// <summary>
        /// 
        /// </summary>
        public RotatedLabel()
        {
            _label = new System.Windows.Forms.Label();
            _newText = new string[] { "" };
            
            this.RotateAngle = 0;
            this.SelectPanelEvent += OnSelected;
            this.Text = String.Empty;
            this.AutoSize = false;
            SelectPanelEvent += OnSelected;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush b = new SolidBrush(this.ForeColor);
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            int bigest = 0;

            if (this.RotatedText.Count() < 1)
            {
                bigest = 0;
            }
            else
            {
                bigest = 0;
                for (int i = 0; i < this.RotatedText.Count(); i++)
                {
                    if (this.RotatedText[i].Length > this.RotatedText[bigest].Length)
                    {
                        bigest = i;
                    }
                }
            }
            if (_selected)
            {

            }

            var s = e.Graphics.MeasureString(this.RotatedText[bigest], Font);

            var angle = ((RotateAngle % 360) + 360) % 360;
            var radians = Math.PI * angle / 180.0;

            int x = Math.Abs((int)Math.Ceiling(s.Height * this.RotatedText.Count() * Math.Sin(radians)));
            int y = Math.Abs((int)Math.Ceiling(s.Height * this.RotatedText.Count() * Math.Cos(radians)));
            Point rotatedHeight = new Point(x, y);

            x = Math.Abs((int)Math.Ceiling(s.Width * Math.Cos(radians)));
            y = Math.Abs((int)Math.Ceiling(s.Width * Math.Sin(radians)));
            Point rotatedWidth = new Point(x, y);

            e.Graphics.TranslateTransform((rotatedHeight.X + rotatedWidth.X) / 2, (rotatedWidth.Y + rotatedHeight.Y));
            e.Graphics.RotateTransform(this.RotateAngle);

            var txt = "";

            foreach (var line in this.RotatedText)
            {
                txt += line + Environment.NewLine;
            }

            e.Graphics.DrawString(txt, this.Font, b, 0f, 0f, format);
            this.Width = (rotatedHeight.X + rotatedWidth.X);
            this.Height = (rotatedWidth.Y + rotatedHeight.Y) * 2;
            Refresh();
            base.OnPaint(e);
        }

        public float GetRotareAngel()
        {
            var ra = (float)this.RotateAngle;
            return ra;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            if (SelectPanelEvent != null)
            {
                _selected = true;
            }
            base.OnMouseHover(e);
        }

        private void OnSelected(object sender, PanelLabelEventArgs e)
        {
            if (SelectPanelEvent != null)
            {
                _selected = true;
                e.PanelLabelName = this.Name;
                SelectPanelEvent(this, e);
            }
        }
    }
}
