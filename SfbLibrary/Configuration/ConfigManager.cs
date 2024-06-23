// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Configuration
{
    public class ConfigManager
    {
        public Dictionary<string,string> Configs { get; private set; }

        public ConfigManager() 
        {
            GetConfigs();
        }

        private async void GetConfigs()
        {
            Ini ini = new Ini();
            await ini.Open();
            var configs = ini.GetSettings();
            Configs = configs;
        }
    }
}
