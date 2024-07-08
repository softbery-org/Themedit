// Version: 1.0.0.371
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Themedit
{
    public class LabelTimer : Label
    {
        /// <summary>
        /// Delegate for timer value changes.
        /// </summary>
        /// <param name="sender">timer object</param>
        /// <param name="e">timer arguments</param>
        public delegate void dlgTimerValueChenge(object sender, TimerEventArgs e);
        
        /// <summary>
        /// Color timer
        /// </summary>
        public Color TimerColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
        
        /// <summary>
        /// Current timer time
        /// </summary>
        public TimeSpan TimerValue { get; private set; } = new TimeSpan(0, 0, 0);
        
        /// <summary>
        /// Event during time change.
        /// </summary>
        public event dlgTimerValueChenge OnTimerValueChange;

        /// <summary>
        /// Specifies the type of data displayed.
        /// </summary>
        /// <example>
        /// Left - how long its left to end media. Example: -00:25:00
        /// Current - current time,                Example:  00:35:00
        /// </example>
        public enum TimerLabelViewType
        {
            Left,
            Current
        }

        public TimerLabelViewType LabelViewType { get; private set; } = TimerLabelViewType.Left;

        public LabelTimer()
        {
            DefaultSettingsOfLabel();
        }

        private void DefaultSettingsOfLabel()
        {
            Effect = new DropShadowEffect();
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Content = TimerValue;
        }

        private void onTimerValueChange(object sender, TimerEventArgs e)
        {
            if (OnTimerValueChange!=null)
            {
                Content = e.Time;
                OnTimerValueChange?.Invoke(this, e);
            }
        }
    }


    public class TimerEventArgs : EventArgs
    {
        public TimeSpan Time { get; private set; } = TimeSpan.Zero;

        public TimerEventArgs(TimeSpan time)
        {
            Time = time;
        }
    }
}
