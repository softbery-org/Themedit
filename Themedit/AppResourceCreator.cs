// Version: 1.0.0.87
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Themedit
{
    internal static class AppResourceCreator
    {
        private static string _pluginsPath = "plugins/";

        public static void PluginsPath()
        {
            var dir = new DirectoryInfo(_pluginsPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }
    }
}
