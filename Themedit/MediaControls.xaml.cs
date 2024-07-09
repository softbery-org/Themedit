// Version: 1.0.0.451
// Copyright (c) 2024 Softbery by Pawe� Tobis

// Version: 1.0.0.79
using SfbLibrary.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
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
using Themedit.src;
using ThemeditLanguage;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy MediaControls.xaml
    /// </summary>
    public partial class MediaControls : UserControl
    {
        public delegate void dlgChangeLanguageHandler();
        public delegate void dlgPlaylistHandle(ref string track);
        public delegate void dlgMediaElementSeekHandler(object sender, RoutedPropertyChangedEventArgs<double> args);

        public static Action OnLanguageChange;
        public static Action<Uri> ThumbnailMediaSet;

        private PlaylistWindow _playlist;
        private MediaPlaylist _mediaPlaylist;
        private IList<IPlugin> _plugins;
        private IList<IPlugin> _runnedPlugins = new List<IPlugin>();
        private FileSystemWatcher _fsw;
        private Window _window;
        private VideoDrawing _videoDrawing;

        public MediaPlaylist Playlist { get => _mediaPlaylist; }

        public Window MWindow
        {
            get
            {
                if (_window == null)
                    _window = new PlaylistWindow();
                return _window;
            }
            set
            {
                _window = value;
                _playlist = new PlaylistWindow(_window);
                _window.Closed += _window_Closed;
            }
        }

        private void translate()
        {
            var language = Translation.Current;

            this.Resources["labelSubtitles_Txt.Content"] = language.MediaControls.labelSubtitles_Txt;
            this.Resources["labelSubtilesOnOff.Content"] = language.MediaControls.labelSubtilesOnOff;
            this.Resources["btnClose.ToolTip"] = language.MediaControls.btnClose;
            this.Resources["logRichTextBox.Text}"] = language.MediaControls.logRichTextBox;
            this.Resources["btnPrevious.ToolTip"] = language.MediaControls.btnPrevious;
            this.Resources["btnPlay.ToolTip"] = language.MediaControls.btnPlay;
            this.Resources["btnStop.ToolTip"] = language.MediaControls.btnStop;
            this.Resources["btnPause.ToolTip"] = language.MediaControls.btnPause;
            this.Resources["btnRewind.ToolTip"] = language.MediaControls.btnRewind;
            this.Resources["btnForward.ToolTip"] = language.MediaControls.btnForward;
            this.Resources["btnFullscreen.ToolTip"] = language.MediaControls.btnFullscreen;
            this.Resources["btnSubtitles.ToolTip"] = language.MediaControls.btnSubtitles;
            this.Resources["btnVolumeUp.ToolTip"] = language.MediaControls.btnVolumeUp;
            this.Resources["btnVolumeDown.ToolTip"] = language.MediaControls.btnVolumeDown;
            this.Resources["btnMute.ToolTip"] = language.MediaControls.btnMute;
            this.Resources["btnSettings.ToolTip"] = language.MediaControls.btnSettings;
            this.Resources["btnOpen.ToolTip"] = language.MediaControls.btnOpen;
            this.Resources["btnPlaylist.ToolTip"] = language.MediaControls.btnPlaylist;
            this.Resources["btnUrl.ToolTip"] = language.MediaControls.btnUrl;
            this.Resources["btnSubtitlesOnOff.ToolTip"] = language.MediaControls.btnSubtitlesOnOff;
            this.Resources["btnPlaylistNext.ToolTip"] = language.MediaControls.btnPlaylistNext;
            this.Resources["btnPlaylistRepeater.ToolTip}"] = language.MediaControls.btnPlaylistRepeater;
        }

        public MediaControls()
        {
            InitializeComponent();

            _plugins = Plugin.LoadPlugins();

            foreach (var plugin in _plugins)
            {
                _comboboxPlugins.Items.Add(plugin.PluginName);
            }

            _thumbnailMediaElement.LoadedBehavior = MediaState.Manual;
            _canvasThumbnail.Visibility = Visibility.Hidden;

            OnLanguageChange += LanguageChange;
            ThumbnailMediaSet += ThumbnailMediaUriSet;

            translate();
        }

        private void ThumbnailMediaUriSet(Uri uri)
        {
            if (uri != null)
            {
                _thumbnailMediaElement.Source = uri;
                _thumbnailMediaElement.Play();
            }
        }

        private void LanguageChange()
        {
            translate();            
        }

        public MediaControls(MediaPlaylist playlist) : this()
        {
            _mediaPlaylist = playlist;
        }

        private void _window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

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

        }

        private void progressBarVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private SettingsWindow _settingsWindow;

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Show();
                _settingsWindow.Activate();
            }
            else
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Show();
            }
        }

        private void btnPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_playlist != null)
            {
                _playlist.Show();
                _playlist.Activate();
            }
            else
            {
                _playlist = new PlaylistWindow();
                _playlist.Show();
            }

        }

        private void _comboboxPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_plugins != null)
            {
                foreach (var plugin in _plugins)
                {
                    if (plugin.PluginName == _comboboxPlugins.SelectedItem.ToString())
                    {
                        var r = plugin.RunPlugin() as IPlugin;
                        r.Show();
                        if (!_runnedPlugins.Contains(r))
                            _runnedPlugins.Add(r);
                    }
                }
            }
        }

        private void progressBarVolume_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var click_position = e.GetPosition(progressBarVolume).X;
                var width = progressBarVolume.ActualWidth;
                var result = (click_position / width) * progressBarVolume.Maximum;

                progressBarVolume.Value = result;
                var w = _window as MainWindow;
                w._mediaElement.Volume = result;
                lblVolumeLevel.Content = Math.Round(result * 100, 0);
            }
        }

        private void progressBarVideo_MouseMove(object sender, MouseEventArgs e)
        {
            var click_position = e.GetPosition(progressBarVideo).X;
            var width = progressBarVideo.ActualWidth;
            var result = (click_position / width) * progressBarVideo.Maximum;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var w = _window as MainWindow;

                if (w._mediaElement.NaturalDuration.HasTimeSpan)
                {
                    progressBarVideo.Value = result;

                    // Video jump to time
                    var jump_to_sec = (w._mediaElement.NaturalDuration.TimeSpan.TotalSeconds * result) / progressBarVideo.Maximum;
                    w._mediaElement.Position = TimeSpan.FromSeconds(jump_to_sec);
                }
            }
            if (_thumbnailMediaElement!=null && _thumbnailMediaElement.NaturalDuration.HasTimeSpan)
            {
                var time = TimeSpan.FromSeconds((_thumbnailMediaElement.NaturalDuration.TimeSpan.TotalSeconds * result) / progressBarVideo.Maximum);
                _thumbnailTimer.Text = $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}";
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnPlaylistRepeater_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void _comboboxPlugins_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var plugin in _plugins)
            {
                if (plugin.PluginName == _comboboxPlugins.SelectedItem.ToString())
                {
                    var r = plugin.RunPlugin() as IPlugin;
                    r.Show();
                    if (!_runnedPlugins.Contains(r))
                        _runnedPlugins.Add(r);
                }
            }
        }

        private void progressBarVideo_MouseEnter(object sender, MouseEventArgs e)
        {
            _canvasThumbnail.Visibility = Visibility.Visible;

            var over_position = e.GetPosition(progressBarVideo).X;
            var width = progressBarVideo.ActualWidth;
            var X = (over_position / width) * progressBarVideo.Maximum;

            //MessageBox.Show(e.GetPosition(_mediaControls.progressBarVideo).X.ToString()+ Mouse.GetPosition(this).X);
            _canvasThumbnail.Margin = new Thickness(0 + over_position, -5, 0, 0);

            if (_thumbnailMediaElement.NaturalDuration.HasTimeSpan)
            {
                var ts = TimeSpan.FromSeconds((_thumbnailMediaElement.NaturalDuration.TimeSpan.TotalSeconds * X) / progressBarVideo.Maximum);
                _thumbnailMediaElement.Position = ts;
                _thumbnailMediaElement.Play();
            }
        }

        private void progressBarVideo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_thumbnailMediaElement!=null)
            {
                _canvasThumbnail.Visibility = Visibility.Hidden;
                _thumbnailMediaElement.Pause();
            }
        }
    }

    public class PlaylistEventArgs : RoutedEventArgs
    {
        public IList<Track> Tracks { get; set; }
        public Track CurrentTrack { get; set; }
        public event Action<string> PlayTrack;
        public event Action<string> NextTrack;
        public event Action<string> PreviouseTrack;
        public event Action<string> GetCurrentTrack;

        public PlaylistEventArgs(Playlist playlist)
        {
            Tracks = playlist.TracksList;
            PlayTrack += PlaylistEventArgs_PlayTrack;
        }

        private void PlaylistEventArgs_PlayTrack(string obj)
        {
            
        }
    }
}
