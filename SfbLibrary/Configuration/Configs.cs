// Copyright (c) 2024 Softbery by Paweł Tobis
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace SfbLibrary.Configuration
{
    public class Configs : IConfiguration
    {
        private string[] _args = null;
        private IHost _host;
        private HostApplicationBuilder _builder;
        private string _configPath;

        public readonly List<Config> Configurations = new List<Config>();
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Configs(ConfigFileType file_type = ConfigFileType.ini)
        {
            var typos = Enum.GetNames(typeof(ConfigFileType));
            var selected_type = Enum.GetName(typeof(ConfigFileType), file_type);
            _configPath = $"{AppDomain.CurrentDomain.BaseDirectory}config/config.{selected_type}";
            open(_configPath);
        }

        private async void open(string config_path)
        {
            _builder = Host.CreateApplicationBuilder(_args);
            _builder.Configuration.Sources.Clear();
            _builder.Configuration.AddEnvironmentVariables(prefix: "Video_");

            IHostEnvironment env = _builder.Environment;

            var file = config_path;
            var file_env = config_path.Insert((config_path.Length - 5), $"{env.EnvironmentName}");

            _builder.Configuration
                .AddIniFile(file, optional: true, reloadOnChange: true)
                .AddIniFile(file_env, true, true);

            _host = _builder.Build();
            await _host.RunAsync();
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
