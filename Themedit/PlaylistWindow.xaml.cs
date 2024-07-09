// Version: 1.0.0.391
// Copyright (c) 2024 Softbery by Paweł Tobis
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xaml;
using Themedit.src;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy ListWindow.xaml
    /// </summary>
    public partial class PlaylistWindow : Window
    {
        private Window _window;

        public static Action<Track> SelectOnListView;
        public static Action<Track[]> LoadUserSetting;
        public static Action OnLanguageChange;

        public PlaylistWindow()
        {
            InitializeComponent();
            SelectOnListView += select;
            LoadUserSetting += load;

            OnLanguageChange += LanguageChange;
            translate();
        }

        private void LanguageChange()
        {
            translate();
        }

        private void translate()
        {
            var language = Translation.Current;

            this.Resources["Id.Header"] = language.Playlist.id;
            this.Resources["Name.Header"] = language.Playlist.name;
            this.Resources["Path.Header"] = language.Playlist.path;
            this.Resources["Time.Header"] = language.Playlist.time;
            this.Resources["btnAddMedia.Content"] = language.Playlist.btnAddMedia;
            this.Resources["btnRemMedia.Content"] = language.Playlist.btnRemMedia;
            this.Resources["btnClrList.Content"] = language.Playlist.btnClrList;
            this.Resources["btnClose.Content"] = language.Playlist.btnClose;
        }

        private async void load(Track[] tracks)
        {
            _listView.Items.Clear();
            /*for (int i = 0; i < tracks.Length; i++)
            {
                var time = await new MediaInformation(tracks[i]).ReadAsync();
                tracks[i].SetTime(time);
            }*/

            _listView.ItemsSource = tracks.ToList();
        }

        private void select(Track track)
        {
            foreach (var item in _listView.Items)
            {
                var n = item.ToString();
                if (n == track.Path)
                {
                    _listView.SelectedItem = item; 
                    break;
                }
            }
        }

        public PlaylistWindow(Window window) : this()
        {
            _window = window;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public async Task AddMediaAsync(string path)
        {
            var t = new Track(path);
            _listView.Items.Add(t);
            IList<Track> list = new List<Track>();
            foreach (Track track in _listView.Items)
            {
                track.SetTime(await track.ReadDurationAsync(track.Path));
                list.Add(track);
            }
            MainWindow.SetPlaylist(list);
            
        }

        public async Task AddMediaAsync(string[] path)
        {
            var t = new List<Track>();
            foreach (var item in path)
            {
                var track = new Track(item);
                track.SetTime(await track.ReadDurationAsync(item));
                t.Add(track);
                _listView.Items.Add(track);

            }
            MainWindow.SetPlaylist(t);
        }

        public void RemoveMedia(string path) 
        {
            foreach (var track in ((MainWindow)_window).Playlist)
            {
                if (track.Path == path)
                {
                    MainWindow.RemoveFromPlaylist(track);
                    break;
                }
            }
            
            _listView.Items.Remove(path);
        }

        private void BtnAddMedia_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Video files(*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg;)|*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg|" +
                         "Audio files(*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac;)|*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac|" +
                         "All files (*.*)|*.*";
            if (ofd.ShowDialog() != null)
            {
                try 
                { 
                    AddMediaAsync(ofd.FileNames);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"[{Translation.Current.ExceptionsMessage.Msg0006.Replace("Translation.Current.ExceptionsMessage.","")}]: {Translation.Current.ExceptionsMessage.Msg0006} \n\nException:\n{ex.Message}");
                }
            }
            else
            {
                
            }
        }

        private void _playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem != null)
            {
                ((MainWindow)_window).Playlist.SetCurrent((_listView.SelectedItem as Track).Path);
                MainWindow.OnOpeningMedia(this, (_listView.SelectedItem as Track).Path);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnClrList_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)_window).Playlist.Tracks.Clear();
            _listView.Items.Clear();
        }

        private async void btnRemMedia_Click(object sender, RoutedEventArgs e)
        {
            var result = new MediaInfoLib.MediaInfo();
            result.Open((_listView.SelectedItem as Track).Path);
            
            var t = result.Inform();

            MessageBox.Show(t);
            result.Close();
        }
    }
}
