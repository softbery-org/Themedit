// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SfbLibrary.Keybords
{
    public class Filter : IMessageFilter
    {
        public delegate void dlgKeyPressed(object sender, KeyPressEventArgs e);
        public delegate void dlgKeyUped(object sender, System.Windows.Forms.KeyEventArgs e);
        public delegate void dlgExceptionShow(object sender, System.Windows.Forms.MouseEventArgs e);
        public delegate void dlgMouseMoveTest(object sender, System.Windows.Forms.MouseEventArgs e);

        public event dlgKeyPressed KeyPressed;
        public event EventHandler Disposed;
        public event dlgExceptionShow ExceptionShow;
        public event dlgMouseMoveTest MouseMoveTest;

        private const int WM_KEYDOWN = 0x0100;
        private BackgroundWorker _backgroundWorker;
        private CancellationToken _cancellationToken = new CancellationToken(false);

        public string VALUE { get; private set; }

        /*public event EventHandler<MouseButtons> MouseButtons;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseLeave;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseNotMove;*/

        //private Timer _keyEventTimer;
        public System.Drawing.Point StartPoint { get; private set; }
        public System.Drawing.Point CurrentPoint { get; private set; }
        public int DeltaDelta { get; private set; }
        public bool IsDown { get; private set; }
        public bool IsCanceled { get; private set; }

        public Filter()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerAsync();

            /*_keyEventTimer = new Timer();
            _keyEventTimer.Interval = 100; // in ms
            _keyEventTimer.Tick += _keyEventTimer_Tick;
            _keyEventTimer.Start();*/
        }

        void Event_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            StartPoint = e.Location;
            CurrentPoint = StartPoint;
            DeltaDelta = e.Delta;
            IsDown = true;
            IsCanceled = false;
            //MouseMoveTest?.Invoke(sender, e);
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private async void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var mea = new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, 0, 0, 0);

            await Task.Factory.StartNew(async (resultTask) =>
            {
                try
                {
                    var curr = Control.MousePosition; 
                    mea = new System.Windows.Forms.MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0);
                    MouseMoveTest?.Invoke(sender, mea);
                    //ExceptionShow += Filter_ExceptionShow;
                    //MouseMoveTest += Filter_MouseMoveTest;
                    await Task.Delay(10);
                }
                catch (Exception ex)
                {
                    _cancellationToken.Register(() =>
                    {
                        MessageBox.Show(ex.Message);
                        _backgroundWorker.CancelAsync();
                    });
                }
            }, _cancellationToken);
        }

        private void Filter_MouseMoveTest(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseMoveTest?.Invoke(sender, e);
            MessageBox.Show(VALUE);
        }

        private void Filter_ExceptionShow(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            VALUE = $"{sender} | {e.X} | {e.Y}";
        }

        private void _keyEventTimer_Tick(object sender, EventArgs e)
        {

        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            

            switch (m.Msg)
            {
                case WM_KEYDOWN: // on key down
                    if (KeyPressed != null)
                    {
                        var k = new KeyPressEventArgs((char)m.WParam);
                        KeyPressed.Invoke(this, k);
                    }
                    break;
            }
            return false; // handle messages normally
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
