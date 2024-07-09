// Version: 1.0.0.369
// Copyright (c) 2024 Softbery by Paweł Tobis
using ThemeditLanguage;
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
        public static ILanguage Current { get; private set; }
        public static IList<ILanguage> Languages { get; private set; } = new List<ILanguage>();

        private static bool checkVersion(ILanguage language, PlayerInfo player_info)
        {
            if (language.Version == player_info.Version)
            {
                return true;
            }
            return false;
        }

        public static void ChangeLanguage(string language_code)
        {
            foreach (var item in Languages)
            {
                if (item.Code == language_code && checkVersion(item, new PlayerInfo()))
                {
                    Current = item.UseLanguage() as ILanguage;
                }
            }
        }

        public static void ChangeLanguage(ILanguage language)
        {
            
            foreach (var item in Languages)
            {
                if (item == language && checkVersion(item, new PlayerInfo()))
                {
                    Current = item.UseLanguage() as ILanguage;
                }
            }
        }

        public static void ChangeLanguage(this object obj, string language_code)
        {
            if (language_code != null)
            {
                ChangeLanguage(language_code);
            }
        }

        public static IList<ILanguage> GetLanguages()
        {
            return Languages;
        }

        public static ILanguage GetCurrentLanguage()
        {
            if (Current != null)
                return Current;
            else
                return null;
        }

        public static void SetLanguage(string language_code)
        {
            var languages = Language.LoadLanguages();
            Languages = languages;

            foreach (var lang in languages)
            {
                if (lang.Code == language_code)
                {
                    Current = lang as ILanguage;
                }
            }
        }
    }
}
