// Version: 1.0.0.114
// Copyright (c) 2024 Softbery by Paweł Tobis
using SfbLibrary.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Themedit
{
    internal static class Translation
    {
        public static ILanguage Controls { get; private set; }
        public static IList<ILanguage> Languages { get; private set; } = new List<ILanguage>();

        private static bool checkVersion(ILanguage language, PlayerInfo player_info)
        {
            if (language.Version == player_info.Version)
            {
                return true;
            }
            return false;
        }

        public static void ChangeLanguage(string language)
        {
            foreach (var item in Languages)
            {
                if (item.Name == language && checkVersion(item, new PlayerInfo()))
                {
                    Controls = item.UseLanguage() as ILanguage;
                }
            }
        }

        public static void ChangeLanguage(this object obj, string language = null)
        {
            if (language != null)
            {
                ChangeLanguage(language);
            }
            var control = obj as Control;
            
            control.InvalidateVisual();
        }

        public static ILanguage GetCurrentLanguage()
        {
            if (Controls != null)
                return Controls;
            else
                return null;
        }

        public static void SetLanguage(string lang)
        {
            var languages = Language.LoadLanguages();

            foreach (var language in languages)
            {
                if (language.Name == lang)
                {
                    Controls = language as ILanguage;
                }
            }
        }
    }
}
