// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SfbLibrary.Components
{
    public class AnimatedLabel : Label
    {
        private Timer _timer;

        public new bool AutoSize { get; set; } = false;

        public AnimatedLabel() : base()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;

            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphic = e.Graphics;
            var brush = new SolidBrush(ForeColor);
            var pen = new Pen(brush, 1);
            base.OnPaint(e);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            
        }

        private void runAnimation()
        {
            if (this.Width < getTextWidth())
            {
                _timer.Enabled = true;
                _timer.Start();
            }
            else
            {
                _timer.Enabled = false;
                _timer.Stop();
            }
        }

        private int getTextWidth()
        {
            var measure = TextRenderer.MeasureText(Text, Font);
            return measure.Width;
        }

        private int getTextHeight()
        {
            var measure = TextRenderer.MeasureText(Text, Font);
            return measure.Height;
        }

        private Size getTextSize()
        {
            var measure = TextRenderer.MeasureText(Text, Font);
            return measure;
        }
    }
}
