// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SfbLibrary.Mouses
{
    public class WpfFilter : System.Windows.UIElement
    {
        public delegate void dlgMouseMoved(object sender, System.Windows.Input.MouseEventArgs e);
        public delegate void dlgMouseNotMoveSeconds(object sender, EventArgs e);
        public delegate void dlgMouseHover();

        public event dlgMouseMoved MouseMoved;
        public event dlgMouseNotMoveSeconds MouseNotMove;
        public event dlgMouseHover MouseHover;

        public int Second { get; set; } = 3;

        private System.Windows.Point _lastPoint;
        private System.Timers.Timer _timer;
        private UIElement _ctrl;

        public WpfFilter()
        {
            _timer = new System.Timers.Timer();
            _timer.Enabled = false;
            _timer.Interval = (int)TimeSpan.FromSeconds(Second).TotalMilliseconds;
            _timer.Elapsed += Tmr_Elapsed;

            UIElement uI = new UIElement();
            _ctrl = uI;
        }

        public WpfFilter(UIElement ctrl)
        {
            _ctrl = ctrl;
            _timer = new System.Timers.Timer();
            _timer.Enabled = false;
            _timer.Interval = (int)TimeSpan.FromSeconds(Second).TotalMilliseconds;
            _timer.Elapsed += Tmr_Elapsed;
        }

        public bool isMoving = false;

        private void Tmr_Elapsed(object sender, EventArgs e)
        {
            if (MouseNotMove!=null)
            {
                OnMouseNotMove(sender, e);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseNotMove(object sender, EventArgs e)
        {

        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var curPoint = Mouse.GetPosition(this);
            switch (msg)
            {
                // get phantom WM_MOUSEMOVE messages, when the mouse has NOT moved!
                case Mouses.MouseProc.WM_MOUSEMOVE:
                    if (!curPoint.Equals(_lastPoint))
                    {
                        _lastPoint = curPoint;

                        if (MouseMoveEvent != null)
                        {
                            //mediaControls.labelTitle.Content = "ZZZZZZZZZZZZZZZZZZZ";
                            MouseEventArgs e = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                            this.OnMouseMove(this, e);
                        }
                        _timer.Stop();
                        _timer.Start();
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
