// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Taskbar
{
    /// <summary>
    /// Windows taskbar visibility changer.
    /// </summary>
    public class WindowsTaskbar
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        /// <summary>
        /// Taskbar visibility status.
        /// </summary>
        private static bool _visibility = true;

        /// <summary>
        /// Handle
        /// </summary>
        protected static int Handle
        {
            get
            {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        private WindowsTaskbar()
        {
            // hide ctor
        }

        /// <summary>
        /// Show Windows taskbar
        /// </summary>
        public static void Show()
        {
            ShowWindow(Handle, SW_SHOW);
            _visibility = true;
        }

        /// <summary>
        /// Hide Windows taskbar
        /// </summary>
        public static void Hide()
        {
            ShowWindow(Handle, SW_HIDE);
            _visibility = false;
        }

        public static bool GetState()
        {
            return _visibility;
        }
    }
}
