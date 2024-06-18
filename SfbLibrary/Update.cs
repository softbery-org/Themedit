// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Diagnostics;
using System.Windows.Forms;

namespace SfbLibrary
{
    public class Update
    {
        public Apk Apks { get => _apk; }
        public const string UPDATE_URL = "http://softbery.com.pl/zlecenia.json";
        //public const string INFO_URL = "http://softbery.com.pl/zlecenia-info.json";
        public string Info { get => _info; }

        private string _info;
        private string[] _args;
        private string _currentVersion = "1.0.0.195";
        private static Apk _apk = null;
        private Apk _updateApk = null;
        private BackgroundWorker _bgUpdate = null;

        public Update(bool auto_update, string[] args)
        {
            var split = _currentVersion.Split('.');
            var version = new Version();
            _args = args;
            _apk = new Apk();
            _updateApk = new Apk();

            if (auto_update)
            {
                _bgUpdate = new BackgroundWorker();
                _bgUpdate.DoWork += _bgUpdate_DoWork;
                _bgUpdate.RunWorkerAsync();
            }

            version.Build = Int32.Parse(split[0]);
            version.Major = Int32.Parse(split[1]);
            version.Minor = Int32.Parse(split[2]);
            version.Revision = Int32.Parse(split[3]);

            _apk.Guid = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>().Value.ToString();
            _apk.Version = version;
        }

        private async void _bgUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                await Task.Delay(50000);
                var dialog_result = MessageBox.Show("Chcesz aktualizować program?", "Aktualizacja", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (dialog_result == DialogResult.OK)
                {
                    Run();
                }
                else
                {
                    MessageBox.Show($"Wersja: {_apk.Version} Free");
                }
            }
        }

        private string getUpdateJson()
        {
            using (var httpClient = new HttpClient())
            {
                var json = httpClient.GetStringAsync(UPDATE_URL).Result;

                return json;
            }
        }

        private string getInfoJson()
        {
            using (var httpClient = new HttpClient())
            {
                var json = httpClient.GetStringAsync(UPDATE_URL).Result;

                return json;
            }
        }

        public string GetInfo()
        {
            _info = JsonConvert.DeserializeObject(getInfoJson()).ToString();
            return _info;
        }

        public bool CanBeUpdate()
        {
            _updateApk = JsonConvert.DeserializeObject<Apk>(getUpdateJson());

            _apk = _updateApk;

            if (_updateApk.Version.Build > _apk.Version.Build)
            {
                return true;
            }
            else if (_updateApk.Version.Major > _apk.Version.Major)
            {
                return true;
            }
            else if (_updateApk.Version.Minor > _apk.Version.Minor)
            {
                return true;
            }
            else if (_updateApk.Version.Revision > _apk.Version.Revision)
            {
                return true;
            }

            return false;
        }


        public string Download()
        {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(_updateApk.DownloadUrl);

                if (!System.IO.Directory.Exists($"update/"))
                {
                    System.IO.Directory.CreateDirectory($"update/");
                }

                //webClient.DownloadFileAsync(uri, $"{Program.AppConfig.Configuration["Update:path"]}{_updateApk.Name}");
                webClient.DownloadFileTaskAsync(uri, $"update/{_updateApk.Name}").Start();

                return $"update/{_updateApk.Name}";
            }
        }

        private void copyDir(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);
            try
            {
                if (!dir.Exists)
                {
                    var log = new Log($"Nie znaleziono pliku Zrodlowego: {dir.FullName}");
                    Logger.Write(log);
                }
            }
            catch (Exception ex)
            {
                var log = new Log($"Nie znaleziono pliku Zrodlowego: {dir.FullName}", ex);
                Logger.Write(log);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    copyDir(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public void Run()
        {
            if (CanBeUpdate())
            {
                var update_app_path = Download();
                //Logger.Write(new Log("Pobrano aktualizacje oprogramowania."));

                Decompress(update_app_path);
                //Logger.Write(new Log("Rozpakowano aktualizacje."));

                copyDir($"update/{_updateApk.Name}", AppDomain.CurrentDomain.BaseDirectory, true);

                //Logger.Write(new Log("Przekopiowano pliki aplikacji."));
                //Logger.Write(new Log("Restart aplikacji."));
                Restart();
            }
        }
        public static void Restart()
        {
            // Restart application
            var process = Process.GetCurrentProcess();
            process.Close();
            Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}Zlecenia.exe");
        }

        public void Decompress(string file)
        {
            try
            {
                using (Stream stream = File.OpenRead($"{file}"))
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            //Console.WriteLine(reader.Entry.Key);
                            Logger.Write(new Log($"Rozpakowuje: {reader.Entry.Key}"));
                            //file = file.Replace(".zip", "");
                            reader.WriteEntryToDirectory($"{file}/", new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(new Log("Problem z dekompresj�.", ex));
            }
        }

        public class Apk
        {
            public string Guid { get; set; }
            public string Name { get; set; }
            public string DownloadUrl { get; set; }
            public Version Version { get; set; }
        }

        public class Version
        {
            public int Build { get; set; }
            public int Major { get; set; }
            public int Minor { get; set; }
            public int Revision { get; set; }
        }
    }
}
