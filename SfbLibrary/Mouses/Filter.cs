// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SfbLibrary.Mouses
{
    public class Filter : IMessageFilter
    {

        public delegate void dlgMouseMoved(object sender, System.Windows.Input.MouseEventArgs e);
        public delegate void dlgMouseNotMoveSeconds(object sender, EventArgs e);
        public delegate void dlgMouseHover();
        public event dlgMouseMoved MouseMoved;
        public event dlgMouseNotMoveSeconds MouseNotMove;
        public event dlgMouseHover MouseHover;

        public int Second { get; set; } = 3;

        private System.Drawing.Point _lastPoint;
        private System.Windows.Forms.Timer _tmr;
        private Control _ctrl;

        public Filter()
        {
            _tmr = new System.Windows.Forms.Timer();
            _tmr.Enabled = false;
            _tmr.Interval = (int)TimeSpan.FromSeconds(Second).TotalMilliseconds;
            _tmr.Tick += Tmr_Tick;
        }

        public Filter(Control ctrl)
        {
            _ctrl = ctrl;
            _tmr = new System.Windows.Forms.Timer();
            _tmr.Enabled = false;
            _tmr.Interval = (int)TimeSpan.FromSeconds(Second).TotalMilliseconds;
            _tmr.Tick += Tmr_Tick;
        }

        public bool isMoving = false;

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            var curPoint = System.Windows.Forms.Cursor.Position; //System.Windows.Forms.Cursor.Position;

            switch (m.Msg)
            {
                case MouseProc.WM_MOUSEMOVE:
                    if (!curPoint.Equals(_lastPoint))
                    {
                        _lastPoint = curPoint;
                        if (MouseMoved != null)
                        {
                            System.Windows.Input.MouseEventArgs e = new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice, 0);
                            MouseMoved.Invoke(this, e);
                            
                            isMoving = true;
                        }
                        _tmr.Stop();
                        _tmr.Start();
                    }
                    else
                    {
                        if (MouseNotMove!=null)
                        {
                            MouseNotMove.Invoke(this, EventArgs.Empty);
                        }
                    }
                    break;
                case MouseProc.WM_MOUSEHOVER:
                    if (curPoint.Equals(_lastPoint))
                    {
                        if (MouseHover != null)
                        {
                            //MouseEventArgs e = new MouseEventArgs(Control.MouseButtons, 0, _lastPosition.X, _lastPosition.Y, 0);
                            MouseHover();
                        }
                    }
                    break;
            }
            return false; // handle messages normally
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            _tmr.Stop();
            if (MouseNotMove != null)
            {
                MouseNotMove?.Invoke(sender, e);
            }
            _tmr.Stop();
        }

    }

    public static class MouseProc
    {
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_MOUSEHOVER = 0x02A1;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_NCMOUSELEAVE = 0x02A2; // nonclient area of a window mouse leave
        public const int WM_NCMOUSEMOVE = 0x00A0;
    }
}