// Version: 1.0.0.447
using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SfbLibrary.Languages;
using System.IO;
using System.Windows.Threading;
using Themedit.src;
using System.Threading;
using SfbLibrary.Configuration;
using System.Diagnostics;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Configuration plugin
        /// </summary>
        public static Dictionary<string, string> Config = new Dictionary<string, string>();

        /// <summary>
        /// Language plugin
        /// </summary>
        public static IList<ILanguage> Languages;
        public static Ini IniConfiguration { get => _ini; }

        private static Mutex _mutex = null;
        private static Ini _ini;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set config
            GetConfig();

            // Language
            Translation.GetLanguages();
            Translation.SetLanguage(Config["Application:Language"]);

            // Application multiple run
            const string appName = "Themedit";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (Config["Settings:ApplicationMultipleRun"] == "deny" && !isRunning(appName))
            {
                //app is already running! Exiting the application
                MessageBox.Show("Multiple launches were not allowed in the application settings.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }

            // Set the application theme to Dark.Green
            ThemeManager.Current.ChangeTheme(this, "Dark.Orange");
            //ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            // Plugins path
            AppResourceCreator.PluginsPath();
        }

        public bool isRunning(string app_name) 
        {
            if (Process.GetProcessesByName(app_name).Length > 0 && Process.GetProcessesByName(app_name).Length < 2)
                return true;
            else
                return false;
        }
        

        protected override void OnExit(ExitEventArgs e)
        {
            _ini.Close();
            base.OnExit(e);
            Application.Current.Shutdown();
        }

        private void GetConfig()
        {
            _ini = new SfbLibrary.Configuration.Ini();
            Config = _ini.GetSettings();
        }

        public static void WriteSettings()
        {
            _ini.Write("Media:Audio","Volume", "175");
        }
    }
}
