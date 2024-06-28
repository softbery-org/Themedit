// Version: 1.0.0.257
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

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Dictionary<string, string> Config = new Dictionary<string, string>();
        public static IList<ILanguage> Languages;

        private SfbLibrary.Configuration.Ini _ini;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the application theme to Dark.Green
            ThemeManager.Current.ChangeTheme(this, "Dark.Orange");
            //ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            // Set config
            GetConfig();
            // Set language
            GetLanguages();
            // Plugins path
            AppResourceCreator.PluginsPath();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _ini.Close();
            base.OnExit(e);
            Application.Current.Shutdown();
        }

        private void GetLanguages()
        {
            var langs = Language.LoadLanguages();
            Languages = langs;
        }

        private void GetConfig()
        {
            _ini = new SfbLibrary.Configuration.Ini();
            Config = _ini.GetSettings();
        }
    }
}
