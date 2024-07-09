// Version: 1.0.0.85
// Copyright (c) 2024 Softbery by Paweł Tobis
using Microsoft.VisualStudio.Threading;
using SfbLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NReco.VideoInfo;

namespace Themedit.src
{
    public class MediaInformation
    {
        private Track _track;

        public MediaInformation(Track track)
        {
            _track = track;
            FFProbe probe = new FFProbe();
            var info = probe.GetMediaInfo(track.Path);
            MessageBox.Show(info.Duration + " " + info.Streams[0].CodecName);
        }

        public async Task<string> ReadAsync()
        {
            string s = "";

            try
            {
                await Task.Run(() =>
                {
                    var media = new MediaInfoLib.MediaInfo();
                    media.Open(_track.Path);
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
                    media.Close();
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
