// Version: 1.0.0.275
// Copyright (c) 2024 Softbery by Paweł Tobis
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Themedit.src
{
    class AppHelp
    {
        public static string ProductName { get; } = (Assembly.GetEntryAssembly()?.GetCustomAttributes(
            typeof(AssemblyProductAttribute)).First() as AssemblyProductAttribute)?.Product ?? "";

        static string _ExecutablePath = "";

        public static string ExecutablePath
        {
            get
            {
                if (_ExecutablePath == "")
                {
                    StringBuilder sb = new StringBuilder(500);
                    TaskDialogNative.GetModuleFileName(IntPtr.Zero, sb, sb.Capacity);
                    _ExecutablePath = sb.ToString();
                }
                return _ExecutablePath;
            }
        }

        public static bool IsDarkTheme
        {
            get
            {
                object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);

                if (value is null)
                    value = 1;

                return (int)value == 0;
            }
        }
    }
}
