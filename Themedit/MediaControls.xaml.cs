// Version: 1.0.0.365
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


        public static Action OnLanguageChange;
        private PlaylistWindow _playlist;
        private MediaPlaylist _mediaPlaylist;
        private IList<IPlugin> _plugins;
        private IList<IPlugin> _runnedPlugins = new List<IPlugin>();
        private FileSystemWatcher _fsw;
        private Window _window;

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

            /*_fsw = new FileSystemWatcher("plugins/", "*.dll");
            _fsw.Changed += _fsw_Changed;
            _fsw.Deleted += _fsw_Deleted;
            _fsw.Renamed += _fsw_Renamed;
            _fsw.Created += _fsw_Created;*/
            _plugins = Plugin.LoadPlugins();

            foreach (var plugin in _plugins)
            {
                _comboboxPlugins.Items.Add(plugin.PluginName);
            }
            OnLanguageChange += LanguageChange;
            translate();
        }

        private void LanguageChange()
        {
            translate();            
        }

        public MediaControls(MediaPlaylist playlist) : this()
        {
            _mediaPlaylist = playlist;
        }

        /*private void _fsw_Created(object sender, FileSystemEventArgs e)
        {
            _plugins = Plugin.LoadPlugins();

            foreach (var plugin in _plugins)
            {
                _comboboxPlugins.Items.AddTrack(plugin.PluginName);
            }
        }

        private void _fsw_Renamed(object sender, RenamedEventArgs e)
        {
            _plugins = Plugin.LoadPlugins();

            foreach (var plugin in _plugins)
            {
                _comboboxPlugins.Items.AddTrack(plugin.PluginName);
            }
        }

        private void _fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            _plugins = Plugin.LoadPlugins();

            foreach (var plugin in _plugins)
            {
                _comboboxPlugins.Items.AddTrack(plugin.PluginName);
            }
        }

        private void _fsw_Changed(object sender, FileSystemEventArgs e)
        {
            var s = sender as IPlugin;
            var w = s as Window;
            w.InvalidateVisual();
        }*/

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

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var w = _window as MainWindow;

                if (w._mediaElement.NaturalDuration.HasTimeSpan)
                {
                    var click_position = e.GetPosition(progressBarVideo).X;
                    var width = progressBarVideo.ActualWidth;
                    var result = (click_position / width) * progressBarVideo.Maximum;

                    progressBarVideo.Value = result;

                    // Video jump to time
                    var jump_to_sec = (w._mediaElement.NaturalDuration.TimeSpan.TotalSeconds * result) / progressBarVideo.Maximum;
                    w._mediaElement.Position = TimeSpan.FromSeconds(jump_to_sec);
                }
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
