// Version: 1.0.0.187
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Themedit.src
{
    public class MediaPlaylist : IEnumerable<Track>
    {
        private IList<Track> _tracks;

        public Track Current { get; private set; }
        public IList<Track> Tracks { get { return _tracks; } }

        public Track this[int id]
        {
            get => _tracks[id];
            set => _tracks[id] = value;
        }

        public Track this[Track track]
        {
            get
            {
                var i = _tracks.IndexOf(track);
                return _tracks[i];
            }
            set
            {
                _tracks[_tracks.IndexOf(track)] = value;
            }
        }

        public MediaPlaylist()
        {
            _tracks = new List<Track>();
        }

        public void Add(Track track)
        {
            _tracks.Add(track);
        }

        public void Add(Track[] tracks)
        {
            foreach (var track in tracks)
            {
                _tracks.Add(track);
            }
        }

        public void Insert(int i, Track track)
        {
            
        }

        public void Remove(Track track)
        {
            _tracks.RemoveAt(_tracks.IndexOf(track));
        }

        public void Remove(Track[] tracks)
        {
            foreach (var track in tracks)
            {
                if (_tracks.Contains(track))
                {
                    _tracks.RemoveAt(_tracks.IndexOf(track));
                }
            }
        }

        public Track GetTrack(int index)
        {
            return _tracks[index];
        }

        public Track GetNext()
        {
            foreach (var track in _tracks)
            {
                if (track == Current)
                {
                    var i = _tracks.IndexOf(Current);
                    if (_tracks.Last() == _tracks[i])
                    {
                        return _tracks.First();
                    }
                    if (_tracks[i+1]!=null)
                    {
                        return _tracks[i+1];
                    }
                    
                }
            }
            return null;
        }

        public void SetCurrent(string track_path) 
        {
            foreach (var track in _tracks)
            {
                if (track.Path == track_path)
                {
                    Current = track;
                    break;
                }
            }
        }

        public void SetCurrent(int id)
        {
            foreach (var track in _tracks)
            {
                if (_tracks.IndexOf(track) == id)
                {
                    Current = track;
                    break;
                }
            }
        }

        public IEnumerator<Track> GetEnumerator()
        {
            return _tracks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tracks.GetEnumerator();
        }
    }
}
