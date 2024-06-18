// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Languages
{
    public static class Language
    {
        private static IList<ILanguage> _langs = new List<ILanguage>();
        private static string _playerVersion;
        private static string _path = "languages/";

        /// <summary>
        /// File path
        /// </summary>
        public static string Path { get => _path; }

        /// <summary>
        /// Version of library
        /// </summary>
        public static string LibraryVersion => "1.0.0.173";

        /// <summary>
        /// Check player version
        /// </summary>
        /// <param name="language">Languge</param>
        /// <param name="player_version">Player version</param>
        /// <returns></returns>
        public static bool CheckPlayerVersion(ILanguage language, string player_version)
        {
            _playerVersion = player_version;
            if (language.Version == player_version)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Load folder with languages dll files
        /// </summary>
        /// <param name="dir_path">Path to languages; default is `<application>/languages/`</param>
        /// <returns>List of ILanguages files</returns>
        public static IList<ILanguage> LoadLanguages(string dir_path = null)
        {
            if (dir_path != null)
            {
                _path = dir_path;
            }

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);

            }

            var files = Directory.GetFiles(_path, "*.dll");

            if (files != null)
            {
                foreach (var file in files)
                {
                    var assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetExportedTypes();

                    foreach (Type type in types)
                    {
                        if (type.GetInterfaces().Contains(typeof(ILanguage)))
                        {
                            var obj = Activator.CreateInstance(type);
                            _langs.Add(obj as ILanguage);
                        }
                    }
                }
            }
            return _langs;
        }

        /// <summary>
        /// Create instance from ILanguage object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>Language instance</returns>
        public static object UseLanguage<T>(this T obj)
        {
            if (_langs.Contains((ILanguage)obj))
                return Activator.CreateInstance(obj.GetType());
            else
                return null;
        }
    }
}
