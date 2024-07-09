// Version: 1.0.0.270
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
        public int Id { get => _id; set => _id = value; }

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

        MediaElement _mediaElement = new MediaElement();
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
                ReadDurationAsync(path);
                _id++;
            }
        }

        public async Task<TimeSpan> ReadDurationAsync(string path)
        {
            var time = TimeSpan.Zero;

            try 
            {
                var probe = new NReco.VideoInfo.FFProbe();

                await Task.Run(() =>
                {
                    var info = probe.GetMediaInfo(path);
                    time = TimeSpan.Parse($"{info.Duration.Hours}:{info.Duration.Minutes}:{info.Duration.Seconds}");
                });

                return time;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

                return TimeSpan.Zero;
            }
        }

        public void SetTime(TimeSpan time) 
        {
            Time = time;
        }

        private async Task<string> GetTimeAsync()
        {
            try
            {
                return ReadAsync(this).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "00:00:00";
            }
        }

        private async Task<string> ReadAsync(Track track)
        {
            string s = "";

            try
            {
                await Task.Run(() =>
                {
                    var media = new MediaInfoLib.MediaInfo();
                    media.Open(track.Path);
                    var split_line = media.Inform().Split('\n');

                    for (int i = 0; i < split_line.Length; i++)
                    {
                        var split = split_line[i].Split(':');
                        if (split[0].Contains("Duration"))
                        {
                            s = split[1];
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return s;
        }
    }
}
