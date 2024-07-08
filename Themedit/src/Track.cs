// Version: 1.0.0.193
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Themedit.src
{
    public class Track
    {
        private int _id = 0;
        private string _name;

        /// <summary>
        /// Track id
        /// </summary>
        public int Id { get => _id; }

        /// <summary>
        /// Track name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Track description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Track file path
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// Track time
        /// </summary>
        public TimeSpan Time { get; private set; }

        MediaElement _mediaElement = new MediaElement();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public Track(string path)
        {
            _mediaElement.MediaOpened += _mediaElement_SourceUpdated;
            var file = new FileInfo(path);
            var dir = new DirectoryInfo(path);
            if (file.Exists)
            {
                Name = file.Name;
                Description = "";
                Path = file.FullName;
                _mediaElement.Source = new Uri(Path);
                _mediaElement.LoadedBehavior = MediaState.Manual;
                _mediaElement.ScrubbingEnabled = true;
                _mediaElement.Play();
                _mediaElement.Stop();
                if (_mediaElement.NaturalDuration.HasTimeSpan)
                {
                    Time = _mediaElement.NaturalDuration.TimeSpan;
                }
                //_mediaElement.Position = TimeSpan.Zero;
                //time = GetTime();
                _id++;
            }
        }
        
        public TimeSpan GetTime()
        {
            
                try
                {
                    _mediaElement.LoadedBehavior = MediaState.Manual;
                    _mediaElement.Play();
                    _mediaElement.Stop();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {

                return _mediaElement.NaturalDuration.TimeSpan;
            }
            else
            {
                return TimeSpan.Parse("00:01:00");
            }
        }

        private void _mediaElement_SourceUpdated(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_mediaElement.NaturalDuration != null)
            {
                if (_mediaElement.NaturalDuration.HasTimeSpan)
                {
                    Time = _mediaElement.NaturalDuration.TimeSpan;
                }
            }
        }
    }
}
