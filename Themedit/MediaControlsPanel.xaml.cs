// Version: 1.0.0.39
// Copyright (c) 2024 Softbery by Pawe� Tobis

// Version: 1.0.0.79
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy MediaControls.xaml
    /// </summary>
    public partial class MediaControls : UserControl
    {
        private MediaElement _player;

        public MediaControls()
        {
            InitializeComponent();
            _player = new MediaElement();
        }

        public MediaControls(MediaElement media)
        {
            InitializeComponent();
            _player = media;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //_player.Play();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void progressBarVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*Mouse.GetPosition(this).X;
            e = new MouseButtonEventArgs(e.Device.)*/
        }

        private void progressBarVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
