// Version: 1.0.0.118
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

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy ListWindow.xaml
    /// </summary>
    public partial class PlaylistWindow : Window
    {
        public src.Playlist Playlist 
        { 
            get => MediaControls.Playlist; 
        }

        private Window _window;

        public PlaylistWindow()
        {
            InitializeComponent();
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
            this.Playlist.AddTrack(path);
            _listView.Items.Add(path);
        }

        public void RemoveMedia(string path) 
        {
            this.Playlist.RemoveTrack(path);
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
                foreach (var file in ofd.FileNames)
                {
                    this.Playlist.AddTrack(file);
                    _listView.Items.Add(file);
                }
            }
            else
            {
                
            }
        }

        private void _playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem!=null)
                MainWindow.OnOpeningMedia(this, _listView.SelectedItem.ToString());
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
            Playlist.ClearTracks();
            _listView.Items.Clear();
        }
    }
}
