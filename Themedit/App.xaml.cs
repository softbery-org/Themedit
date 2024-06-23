// Version: 1.0.0.145
using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SfbLibrary.Languages;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Dictionary<string, string> Config = new Dictionary<string, string>();
        public static IList<ILanguage> LanguagesList;

        SfbLibrary.Configuration.ConfigManager _configManager;

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
        }

        private void GetLanguages()
        {
            var langs = Language.LoadLanguages();
            LanguagesList = langs;
        }

        ///         [Section: Header]
        ///         key1=value1
        ///         key2 = " value2 "
        ///         ; comment
        ///         # comment
        ///         / comment
        private void GetConfig()
        {
            _configManager = new SfbLibrary.Configuration.ConfigManager();
            Config = _configManager.Configs;

        }
    }

    public enum LanguagesTypes
    {
        Polski,
        English
    }
}
