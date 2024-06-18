// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace SfbLibrary.Configuration
{
    /// <summary>
    /// Ini class
    /// </summary>
    /// <example>
    ///         [Section:Header]
    ///         key1=value1
    ///         key2 = " value2 "
    ///         ; comment
    ///         # comment
    ///         / comment
    /// </example>
    public class Ini
    {
        public const string DEFAULT_CONFIG_PATH = "config.ini";
        public Dictionary<string, string> Settings { get => _settings; }

        private Dictionary<string, string> _settings = new Dictionary<string, string>();
        private string[] _args;
        private IHost _host;
        private HostApplicationBuilder _builder = new HostApplicationBuilder();

        public Ini()
        {
            _args = new string[] { };
        }

        public Ini(string[] args)
        {
            int i = 0;
            foreach (var arg in args)
            {
                var s = "CommandLine:" + arg;
                _args[i++] = s;
            }
        }

        public async Task Open(string config_file = "config/config.ini")
        {
            _builder = Host.CreateApplicationBuilder(_args);
            _builder.Configuration.Sources.Clear();
            _builder.Configuration.AddEnvironmentVariables(prefix: "");

            IHostEnvironment env = _builder.Environment;

            var file = config_file;
            var file_env = config_file.Insert((config_file.Length - 5), $"{env.EnvironmentName}");

            _builder.Configuration
                .AddIniFile(file, optional: true, reloadOnChange: true)
                .AddIniFile(file_env, true, true);

            _host = _builder.Build();
            await _host.RunAsync();
        }

        public async Task Write(string section, string key, string value)
        {
            await Task.Run(() => {
                try
                {
                    _settings[$"{section}:{key}"] = value;
                }
                catch (Exception)
                {
                    
                }
                });
        }

        public async Task Write(string key, string value)
        {
            await Task.Run(() =>
            {
                _settings[key] = value;
            });
        }

        public async void Close()
        {
            await _host.StopAsync();
        }

        ~Ini()
        {
            Close();
        }

        private void setSettings()
        {
            foreach (var settings in _builder.Configuration.AsEnumerable().Where(t => t.Value is not null))// _applicationBuilder.Configuration.AsEnumerable().Where(t=>t.Value is not null))
            {
                
                if (_settings.Keys.Contains(settings.Key))
                {
                    _settings[settings.Key] = settings.Value;
                }
                else
                    _settings.Add(settings.Key, settings.Value);
            }
        }

        public Dictionary<string, string> GetSettings()
        {
            foreach (var settings in _builder.Configuration.AsEnumerable().Where(t => t.Value is not null))// _applicationBuilder.Configuration.AsEnumerable().Where(t=>t.Value is not null))
            {
                if (_settings.Keys.Contains(settings.Key))
                {
                    _settings[settings.Key] = settings.Value;
                }
                else
                    _settings.Add(settings.Key, settings.Value);
            }
            return _settings;
        }

        public void DefaultConfigFile(string path)
        {
            var contents =
                "SecretKey = \"Secret key value\"\r\n\r\n" +
                "[TransientFaultHandlingOptions]\r\nEnabled = True\r\n" +
                "AutoRetryDelay = \"00:00:07\"\r\n\r\n" +
                "[Logging: LogLevel]\r\n" +
                "Default = Information\r\n" +
                "Microsoft = Warning";

            File.WriteAllText(path + $"/config.ini", contents, Encoding.UTF8);
        }
    }
}
