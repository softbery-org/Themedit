// Version: 1.0.0.273
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Themedit.src
{
    public static class TrackerList
    {
        public static IList<Track> Tracks { get; private set; }
        public static Track CurrentTrack { get; private set; }

        public static void Add(Track track)
        {
            Tracks.Add(track);
        }

        public static Track GetCurrentTrack(Track track)
        {
            int i = Tracks.IndexOf(track);
            if (i != -1)
            {
                CurrentTrack = Tracks[i];
                return Tracks[i];
            }
            else
            {
                return null;
            }
        }

        public static Track Next()
        {

            return Tracks[0];
        }

        public static void Play()
        {

        }
    }
}
