// Version: 1.0.0.163
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

        public PlaylistWindow()
        {
            InitializeComponent();
            SelectOnListView += select;
        }

        private void select(Track track)
        {
            foreach (var item in _listView.Items)
            {
                var n = item.ToString();
                if (n == track.Path)
                {
                    _listView.SelectedItem = item; break;
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

        public void AddMedia(string path)
        {
            var t = new Track(path);
            _listView.Items.Add(path);
            IList<Track> list = new List<Track>();
            foreach (var track in _listView.Items)
            {
                var tr = new Track(track.ToString());
                list.Add(tr);
            }
            MainWindow.SetPlaylist(list);
            
        }

        public void AddMedia(string[] path)
        {
            var t = new List<Track>();
            foreach (var item in path)
            {
                var track = new Track(item);
                t.Add(track);
                _listView.Items.Add(track.Path);
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
                AddMedia(ofd.FileNames);
            }
            else
            {
                
            }
        }

        private void _playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem != null)
            {
                ((MainWindow)_window).Playlist.SetCurrent(_listView.SelectedItem.ToString());
                //MainWindow.PlayMedia(_listView.SelectedItem.ToString());
                MainWindow.OnOpeningMedia(this, _listView.SelectedItem.ToString());
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
    }
}
