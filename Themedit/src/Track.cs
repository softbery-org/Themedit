// Version: 1.0.0.3
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Themedit.src
{
    public class Track
    {
        private int _id = 0;
        private string _name;
        private MediaInfo _mediaInfo;

        /// <summary>
        /// Track id
        /// </summary>
        public int Id { get => _id; }

        /// <summary>
        /// Track name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Track description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Track file path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Track time
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public Track(string path)
        {
            var file = new FileInfo(path);
            var dir = new DirectoryInfo(path);
            if (file.Exists)
            {
                Name = file.Name;
                Description = "";
                Path = file.FullName;
                _id++;
            }
        }
        public TimeSpan? GetTime()
        {
            _mediaInfo = new MediaInfo(Path);
            _mediaInfo.GetInfo(MediaInfoStreamKind.Video, 0, null);
            return null;
        }
    }
}