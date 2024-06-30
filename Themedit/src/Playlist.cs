// Version: 1.0.0.64
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Themedit.src
{
    public class Playlist : IEnumerable<Track>
    {
        public string Name { get; private set; } = "Playlist";
        public string Description { get; private set; } = String.Empty;
        public IList<string> Tracks { get; private set; } = new List<string>();
        public IList<Track> TracksList { get; private set; } = new List<Track>();

        public Playlist() 
        { 
            
        }

        public void GetCurrentTrack(string name)
        {

        }

        public Playlist(Track[] tracks) : this()
        {
            foreach (var track in tracks)
            {
                TracksList.Add(track);
            }
        }

        public void AddTrack(string path) 
        {
            Tracks.Add(path);
        }

        public void RemoveTrack(string path)
        {
            Tracks.Remove(path);
        }

        public void ClearTracks()
        {
            Tracks.Clear();
        }

        public void NewPlaylist(string name, string description)
        {
            Name = name;
            Description = description;
            ClearTracks();
        }

        public void SavePlaylist(string file_path, string file_name)
        {
            var f = file_path + "/" + file_name + ".pls";
            SfbLibrary.IO.TempFile.CreateTempFile(Guid.NewGuid().ToString(), ".plstmp");
            var file = new FileInfo(f);
            if (!file.Exists)
            {
                try
                {
                    file.Create();
                    SavePlaylistInFile(file);
                }catch(Exception ex)
                {
                    MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var message = $"{file.Name} exist. Do you want to save instead of it?";
                var caption = "Save file";
                var button = MessageBoxButton.YesNoCancel;
                var icon = MessageBoxImage.Question;
                var result = MessageBox.Show(message, caption, button, icon, MessageBoxResult.Yes);

                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        break;
                    case MessageBoxResult.Yes:
                        file.Replace(file.FullName, file_name);
                        file.Create();
                        file.Delete();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void SavePlaylistInFile(FileInfo file)
        {
            try
            {
                using (var outputFile = file.AppendText())
                {
                    outputFile.WriteLine(this.Name);
                    outputFile.WriteLine(this.Description);
                    foreach (var track in TracksList)
                        outputFile.WriteLine(track.Id + "|" + track.Name + "|" + track.Description + "|" + track.Path + "|" + track.Time);
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public PlaybackPlaylistMethods GetPlaybackMethod()
        {
            return new PlaybackPlaylistMethods();
        }

        public void SetPlaybackMethod(PlaybackPlaylistMethods method)
        {

        }

        public long Count()
        {
            return Tracks.Count;
        }

        public double GetTime()
        {
            foreach (var track in Tracks)
            {

            }
            return 0; 
        }

        public IEnumerator<Track> GetEnumerator()
        {
            return TracksList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return TracksList.GetEnumerator();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// None            - no playback
    /// One             - play only one in endlessly
    /// Sequence        - play list sequence
    /// Random          - play list randomly
    /// All             - play all list endlessly
    /// </remarks>
    public enum PlaybackPlaylistMethods
    {
        None,                       // brak powtarzania
        One,                        // odtwarzanie tylko jednego utwóru
        Sequence,                   // odtwarzanie po kolei
        Random,                     // odtwarzanie losowe
        All,                        // odtwarzanie całej listy w nieskończoność
    }
}
