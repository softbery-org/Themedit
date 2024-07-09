// Version: 1.0.0.447
// Copyright (c) 2024 Softbery by Pawe� Tobis
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Themedit.src;
using Themedit.Subtitles;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MediaElementStatus _status;
        private DispatcherTimer _timer;
        private DispatcherTimer _mouseNotMoveTimer;
        private double _volumeLevel = 1;
        private bool _fullscreen = false;
        private SubtitlesManager _subtitlesManager = null;
        private Dictionary<TimeSpan, SrtSub> _subtitles = null;
        private bool _subtitlesShow = true;
        private bool _mediaElementEnd;
        private bool _isMediaElementSelected = true;
        private static Uri _currentMedia = null;
        private static Uri _currentVideo = null;

        public MediaPlaylist Playlist { get; private set; }

        /// <summary>
        /// Url to this application repository.
        /// </summary>
        public Uri GitHubApplicationRepository = new Uri("https://github.com/softbery-org/Themedit");
        /// <summary>
        /// Change label timer color delegate
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Label timer events arguments</param>
        public delegate void dlgChangeLabelTimerColor(object sender, LabelTimerEventArgs e);
        /// <summary>
        /// Change label timer color event
        /// </summary>
        public event dlgChangeLabelTimerColor ChangeLabelTimerColorEvent;
        public event EventHandler BeyondTextBoxMouseClick;
        /// <summary>
        /// Pictogram action
        /// </summary>
        public enum PictogramAction
        {
            Show,
            Hide
        }

        public bool Fullscreen
        {
            get => _fullscreen;
            set => _fullscreen = value;
        }

        public double WaitTime { get; set; } = 5;

        public static Action<object, string> OnOpeningMedia;
        public static Action<string> PlayMedia;
        public static Action<IList<Track>> SetPlaylist;
        public static Action<Track> RemoveFromPlaylist;
        public static Action<Track> SetCurrent;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // init fullscreen
            this.Loaded += MetroWindow_OnLoaded;

            // Events
            mediaControlsButtonEvents();
            mediaControlsProgressBarEvents();
            keyboardEvents();
            mouseEvents();

            PictogramViewer(PictogramAction.Hide);

            labelTimerColorSet(Brushes.Red);

            Playlist = new MediaPlaylist();

            _textBoxUrl.Visibility = Visibility.Hidden;
            _lblSubtitles.Content = String.Empty;
            _mediaControls.MWindow = this;
            _mediaElement.MediaEnded += _mediaElement_MediaEnded;
            _mediaElement.MediaOpened += _mediaElement_MediaOpened;

            OnOpeningMedia += open;
            PlayMedia += play;
            SetPlaylist += set;
            RemoveFromPlaylist += remove;
            SetCurrent += set;
        }

        private void _mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaControls.ThumbnailMediaSet(_mediaElement.Source);
        }

        public static Uri GetCurrentMedia()
        {
            return _currentMedia;
        }

        private void thumbnailPlay(Track track)
        {

        }

        private void remove(Track track)
        {
            try
            {
                Playlist.Remove(track);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[{Translation.Current.ExceptionsMessage.Msg0003.ToString().Replace("Translation.Current.ExceptionsMessage.", "")}]: {Translation.Current.ExceptionsMessage.Msg0003}. {ex.Message}");
            }
            
        }

        private void set(Track track)
        {
            if (Playlist.Tracks.Contains(track))
            {
                try
                {
                    Playlist.SetCurrent(track.Path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[{Translation.Current.ExceptionsMessage.Msg0002.ToString().Replace("Translation.Current.ExceptionsMessage.", "")}]: {Translation.Current.ExceptionsMessage.Msg0002}. {track.Name}");
                }
                
            }
        }

        private void set(IList<Track> tracks)
        {
            Playlist.Tracks.Clear();
            foreach (var track in tracks)
            {
                try
                {
                    Playlist.Tracks.Add(track);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"[{Translation.Current.ExceptionsMessage.Msg0004.ToString().Replace("Translation.Current.ExceptionsMessage.", "")}]: {Translation.Current.ExceptionsMessage.Msg0004}. {ex.Message}");
                }
                
            }
        }

        private void open(object sender, string path = "")
        {
            if (path != null)
            {
                if (_mediaElement != null)
                {
                    if (sender != null)
                    {
                        if (sender.GetType() == typeof(PlaylistWindow))
                        {
                            var obj = sender as PlaylistWindow;
                            if (obj._listView.SelectedItem!=null)
                                richTextBoxDrawContent(obj._listView.SelectedItem.ToString());
                        }
                    }

                    Playlist.SetCurrent(path);
                    _mediaElement.Source = new Uri(path);
                    _mediaElement.Stop();
                    _mediaElement.Play();
                    _status = MediaElementStatus.Playing;
                    _mediaControls.labelTitle.Content = path;
                    _videoPath = path;
                    _timer.Stop();
                    _timer.Start();
                    _lblTask.Content = Playlist.Current.Name;
                    DelayAsync(10000);
                }
            }
        }

        private void play(string track_path)
        {
            if (_mediaElement != null)
            {
                foreach (var track in Playlist.Tracks)
                {
                    if (track.Name==track_path)
                    {
                        Playlist.SetCurrent(Playlist.Tracks.IndexOf(track));
                        _timer.Stop();
                        PlaylistWindow.SelectOnListView(track);
                        _mediaElement.Source = new Uri(track.Path);
                        _mediaElement.Play();
                        _status = MediaElementStatus.Playing;
                        _timer.Start();
                    }
                }
            }
        }

        private void play(Track track)
        {
            if (_mediaElement != null)
            {
                if (Playlist.Tracks.Contains(track))
                {
                    Playlist.SetCurrent(Playlist.Tracks.IndexOf(track));
                    _timer.Stop();
                    _mediaElement.Source = new Uri(Playlist.Current.Path);
                    PlaylistWindow.SelectOnListView(track);
                    _mediaElement.Play();
                    _status = MediaElementStatus.Playing;
                    _mediaControls.labelTitle.Content = track.Path;
                    _videoPath = track.Path;
                    _timer.Start();
                }
            }
        }

        private void play(int id)
        {
            if (_mediaElement != null)
            {
                if (Playlist.Tracks[id]!=null)
                {
                    var track = Playlist.Tracks[id];
                    Playlist.SetCurrent(Playlist.Tracks.IndexOf(track));
                    _timer.Stop();
                    PlaylistWindow.SelectOnListView(track);
                    _mediaElement.Source = new Uri(Playlist.Current.Path);
                    _mediaElement.Play();
                    _status = MediaElementStatus.Playing;
                    _mediaControls.labelTitle.Content = Playlist.Current.Path;
                    _videoPath = Playlist.Current.Path;
                    _timer.Start();
                }
            }
            else
            {
                MessageBox.Show(Translation.Current.ExceptionsMessage.Msg0000);
            }
        }

        private void play()
        {
            if (_mediaElement != null)
            {
                if (_status==MediaElementStatus.Playing)
                {
                    _mediaElement.Pause();
                    _timer.Stop();
                    _status = MediaElementStatus.Paused;
                }
                else if (_status==MediaElementStatus.Paused)
                {
                    _mediaElement.Play();
                    _timer.Start();
                    _status = MediaElementStatus.Playing;
                }
                else if (_status == MediaElementStatus.Stoped)
                {
                    _mediaElement.Play();
                    _timer.Start();
                    _status = MediaElementStatus.Playing;
                }
            }
            else
            {
                MessageBox.Show(Translation.Current.ExceptionsMessage.Msg0000);
            }
        }

        private void OnMouseClickBeyondTextBox(object sender, EventArgs e)
        {
            if (BeyondTextBoxMouseClick != null)
            {

            }
        }

        private void _bgWorkerCheckingTaskbarVisibility_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {

            }
        }

        private void mediaElementEvents()
        {

        }

        private void mediaControlsButtonEvents()
        {
            _mediaControls.btnPlay.Click += BtnPlay_Click;
            _mediaControls.btnNext.Click += BtnNext_Click;
            _mediaControls.btnOpen.Click += BtnOpen_Click;
            _mediaControls.btnStop.Click += BtnStop_Click;
            _mediaControls.btnPause.Click += BtnPause_Click;
            _mediaControls.btnClose.Click += BtnClose_Click;
            _mediaControls.btnMute.Click += BtnVolumeMute_Click;
            _mediaControls.btnVolumeUp.Click += BtnVolumeUp_Click;
            _mediaControls.btnVolumeDown.Click += BtnVolumeDown_Click;
            _mediaControls.btnFullscreen.Click += BtnFullscreen_Click;
            _mediaControls.btnUrl.Click += BtnUrl_Click;
            _mediaControls.btnSubtilesOnOff.Click += BtnSubtilesOnOff_Click;
            _mediaControls.btnSubtiles.Click += BtnOpenSubtiles_Click;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void mediaControlsProgressBarEvents()
        {
            _mediaControls.progressBarVideo.ValueChanged += ProgressBarVideo_ValueChanged;
            _mediaControls.progressBarVideo.MouseDown += ProgressBarVideo_MouseDown;
            _mediaControls.progressBarVideo.MouseEnter += ProgressBarVideo_MouseEnter;
            _mediaControls.progressBarVideo.MouseLeave += ProgressBarVideo_MouseLeave;
            _mediaControls.progressBarVolume.MouseDown += ProgressBarVolume_MouseDown;
            _mediaControls.MouseEnter += _mediaControlPanel_MouseEnter;
        }

        private void ProgressBarVideo_MouseLeave(object sender, MouseEventArgs e)
        {
            _mediaControls._canvasThumbnail.Visibility = Visibility.Hidden;
        }

        private bool _thumbnailVisibility = true;

        private double _thumbnailX = -200;

        private void ProgressBarVideo_MouseEnter(object sender, MouseEventArgs e)
        {
            /*_mediaControls._thumbnailMediaElement.LoadedBehavior = MediaState.Manual;
            _mediaControls._canvasThumbnail.Visibility = Visibility.Visible;

            var over_position = e.GetPosition(_mediaControls.progressBarVideo).X;
            var width = _mediaControls.progressBarVideo.ActualWidth;
            var X = (over_position / width) * _mediaControls.progressBarVideo.Maximum;

            //MessageBox.Show(e.GetPosition(_mediaControls.progressBarVideo).X.ToString()+ Mouse.GetPosition(this).X);
            _mediaControls._canvasThumbnail.Margin = new Thickness(0 + over_position, -5, 0, 0);
            _mediaControls._thumbnailMediaElement.Source = new Uri(_videoPath);
            _mediaControls._thumbnailMediaElement.Position = TimeSpan.Parse("00:25:41"); //TimeSpan.FromSeconds((_mediaControls._thumbnailMediaElement.NaturalDuration.TimeSpan.TotalSeconds * X) / _mediaControls.progressBarVideo.Maximum); _mediaControls._thumbnailMediaElement.Position = TimeSpan.FromSeconds((_mediaControls._thumbnailMediaElement.NaturalDuration.TimeSpan.TotalSeconds * X) / _mediaControls.progressBarVideo.Maximum);

            if (_mediaControls._thumbnailMediaElement.NaturalDuration.HasTimeSpan)
            {
                _mediaControls._thumbnailMediaElement.Play();
                _mediaControls._thumbnailMediaElement.Pause();
            }*/
            /*//progressBarVideo.Value = X;
            if (_mediaControls._thumbnailMediaElement.NaturalDuration.HasTimeSpan)
            {
                // Video jump to time
                var thumbnail = (_mediaControls._thumbnailMediaElement.NaturalDuration.TimeSpan.TotalSeconds * X) / _mediaControls.progressBarVideo.Maximum;


                _mediaControls._thumbnailMediaElement.Position = TimeSpan.FromSeconds(thumbnail);
                _mediaControls._canvasThumbnail.Margin = new Thickness(-200 + e.GetPosition(_mediaControls.progressBarVideo).X, -6, 0, 0);
            }*/
        }

        private void keyboardEvents()
        {
            this.KeyDown += _mediaElement_KeyDown;
        }

        private void mouseEvents()
        {
            
        }

        private void moveVideoForward(object sender, EventArgs e)
        {
            _mediaElement.Position += TimeSpan.FromSeconds(10);

        }

        private void moveVideoRewers(object sender, EventArgs e)
        {
            _mediaElement.Position -= TimeSpan.FromSeconds(10);
        }

        private void ProgressBarVolume_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var click_position = e.GetPosition(_mediaControls.progressBarVolume).X;
            var width = _mediaControls.progressBarVolume.ActualWidth;
            var result = (click_position / width) * _mediaControls.progressBarVolume.Maximum;

            _mediaControls.progressBarVolume.Value = (double)result;
            _mediaElement.Volume = _mediaControls.progressBarVolume.Value;
            _mediaControls.lblVolumeLevel.Content = (double)_mediaElement.Volume*100;
        }

        private void ProgressBarVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {
                var click_position = e.GetPosition(_mediaControls.progressBarVideo).X;
                var width = _mediaControls.progressBarVideo.ActualWidth;
                var result = (click_position / width) * _mediaControls.progressBarVideo.Maximum;

                _mediaControls.progressBarVideo.Value = result;

                // Video jump to time
                var jump_to_sec = (_mediaElement.NaturalDuration.TimeSpan.TotalSeconds * result) / _mediaControls.progressBarVideo.Maximum;
                _mediaElement.Position = TimeSpan.FromSeconds(jump_to_sec);
            }
            else
            {
                MessageBox.Show(Translation.Current.ExceptionsMessage.Msg0001);
            }
        }

        private void ProgressBarVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string r = e.NewValue + " " + e.OldValue + "\n";
        }

        private void richTextBoxDrawContent(string text)
        {
            FlowDocument flow_doc = new FlowDocument();
            Run run = new Run($"{text}");
            //Bold myBold = new Bold(new Run("edit me!"));

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(run);
            //paragraph.Inlines.AddTrack(myBold);

            flow_doc.Blocks.Add(paragraph);
            _mediaControls.logRichTextBox.Document = flow_doc;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            // Check is time span exist
            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {
                // Control label time
                _mediaControls.labelTime.Content = $"{_mediaElement.Position.Hours:00}:{_mediaElement.Position.Minutes:00}:{_mediaElement.Position.Seconds:00}/{_mediaElement.NaturalDuration.TimeSpan}";
                // Label - time left to end media
                _lblTimer.Content = $"-{GetTimeToVideoEnd().Hours:00}:{GetTimeToVideoEnd().Minutes:00}:{GetTimeToVideoEnd().Seconds:00}";
                // ProgressBar
                _mediaControls.progressBarVideo.Value = (_mediaElement.Position.TotalSeconds * _mediaControls.progressBarVideo.Maximum) / _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;

                if (_subtitles != null && _subtitlesShow == true)
                {
                    // Draw subtitles if showing and exist
                    DrawSubtitles(_mediaElement.Position);
                }            
            }
        }

        private void DrawSubtitles(TimeSpan time)
        {
            var task = Task.Run(() =>
            {
                this.Invoke((() =>
                {
                    bool show = false;
                    int run = -1;
                    foreach (var item in _subtitlesManager.GetSubtitlesAsTimeSpan())
                    {
                        if (time >= item.Value.StartTime && time <= item.Value.EndTime)
                        {
                            show = true;
                            run = 1;
                        }
                        if (time >= item.Value.EndTime)
                        {
                            show = false;
                            run = 0;
                        }

                        if (show && run == 1)
                        {
                            foreach (var line in item.Value.Text)
                            {
                                _lblSubtitles.Content += line + Environment.NewLine;
                            }
                        }
                        else if (!show && run == 0)
                        {
                            _lblSubtitles.Content = String.Empty;
                        }
                        run = -1;
                    }
                }));
            });
        }

        private void mouseNotMoveTimer_Tick(object sender, EventArgs e)
        {
            HideControls();
        }

        private TimeSpan GetTimeToVideoEnd()
        {
            return _mediaElement.NaturalDuration.TimeSpan - _mediaElement.Position;
        }

        private void OnLabelTimerColorChange(object sender, LabelTimerEventArgs e)
        {
            if (ChangeLabelTimerColorEvent != null)
            {
                var uc = sender as Label;

                e = new LabelTimerEventArgs(_mediaControls.labelTime);
                uc.Foreground = e.Color;
                ChangeLabelTimerColorEvent?.Invoke(_mediaControls.labelTime, e);
            }
        }

        private void labelTimerColorSet(Brush brush)
        {
            _lblTimer.Foreground = brush;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            _mediaControls.labelTitle.Content = _status.ToString();
            Application.Current.Shutdown();
        }

        private async void DelayAsync(int ms)
        {
            await Task.Delay(ms);
            _lblTask.Content = String.Empty;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            play();

            _mediaControls.labelTitle.Content = _status.ToString();
            richTextBoxDrawContent(_status.ToString());
            DelayAsync(5000);
            _mediaControls.labelTitle.Content = _videoPath;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _mediaElement.Close();
            _status = MediaElementStatus.Closing;
            _mediaControls.labelTitle.Content = _status.ToString();
            base.OnClosing(e);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _mediaElement.Stop();
            _mediaControls.labelTitle.Content = _status.ToString();
            _status = MediaElementStatus.Stoped;
            _timer.Stop();
        }

        private void BtnOpenSubtiles_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Subtitles files(*.txt;*.srt)|*.txt;*.srt|" +
                         "All files (*.*)|*.*";
            if (ofd.ShowDialog() != null)
            {
                try
                {
                    _subtitlesManager = new Subtitles.SubtitlesManager($"{ofd.FileName}");
                    _subtitles = _subtitlesManager.GetSubtitlesAsTimeSpan();
                    richTextBoxDrawContent("Ustawiono napisy.");
                }
                catch (Exception ex)
                {
                    richTextBoxDrawContent(ex.Message);
                }
            }
        }

        private void BtnSubtilesOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (_subtitlesShow)
            {
                _subtitlesShow = false;
                _mediaControls.labelSubtilesOnOff.Foreground = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)); // Red color
                _mediaControls.labelSubtilesOnOff.Content = "OFF";
                _lblSubtitles.Content = String.Empty;
                _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.Subtitles;
                PictogramViewerAsync(PictogramAction.Show, 5000);
            }
            else
            {
                _subtitlesShow = true;
                _mediaControls.labelSubtilesOnOff.Foreground = new SolidColorBrush(Color.FromArgb(100, 31, 255, 0)); // Green color
                _mediaControls.labelSubtilesOnOff.Content = "ON";
            }
        }

        private string _videoPath;

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            _mediaControls.labelTitle.Content = _status.ToString();
            var ofd = new OpenFileDialog();
            ofd.Filter = "Video files(*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg;)|*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg|" +
                             "Audio files(*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac;)|*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac|" +
                             "All files (*.*)|*.*";
            if (ofd.ShowDialog() != null)
            {
                try
                {
                    if (_mediaElement != null)
                        _mediaElement.Stop();

                    _mediaElement.Source = new Uri(ofd.FileName);

                    if (SearchSubtitlesFiles(ofd.FileName) != null)
                    {
                        _subtitlesManager = new Subtitles.SubtitlesManager($"{SearchSubtitlesFiles(ofd.FileName)}");
                        _subtitles = _subtitlesManager.GetSubtitlesAsTimeSpan();
                    }

                    _mediaElement.Play();
                    _status = MediaElementStatus.Playing;
                    _mediaControls.labelTitle.Content = ofd.FileName;
                    _videoPath = ofd.FileName;

                    _volumeLevel = 1.0;
                    _mediaElement.Volume = _volumeLevel;
                    _mediaControls.lblVolumeLevel.Content = _volumeLevel.ToString();

                    if (_mediaElement.NaturalDuration.HasTimeSpan)
                    {
                        if (File.Exists($"{ofd.FileName}.tmp"))
                        {
                            var s = File.ReadAllText($"{ofd.FileName}.tmp");
                        }
                    }
                    _timer.Stop();
                    _timer.Start();
                }
                catch (Exception ex)
                {
                    richTextBoxDrawContent(ex.Message);
                }
            }
            else
            {
                richTextBoxDrawContent("Someting gose wrong with opening file.");
            }
        }

        private string SearchSubtitlesFiles(string video_file_path)
        {
            var extension = new FileInfo(video_file_path).Extension;
            var srt = video_file_path.Replace(extension, ".srt");
            var txt = video_file_path.Replace(extension, ".txt");

            if (File.Exists(srt))
            {
                return srt;
            }

            if (File.Exists(txt))
            {
                return txt;
            }

            return null;

        }

        private void BtnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            FullscreenMode();
        }

        private void FullscreenMode()
        {
            if (_fullscreen)
            {
                SfbLibrary.Taskbar.WindowsTaskbar.Show();
                this.ShowInTaskbar = true;
                this.ShowMaxRestoreButton = true;
                this.IgnoreTaskbarOnMaximize = false;
                this.ShowMinButton = true;
                this.ShowCloseButton = true;
                this.ShowTitleBar = true;
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;

                _mediaControls.Visibility = Visibility.Visible;
                _fullscreen = false;
            }
            else
            {
                SfbLibrary.Taskbar.WindowsTaskbar.Hide();
                this.ShowTitleBar = false;
                this.ShowInTaskbar = false;
                this.ShowMaxRestoreButton = false;
                this.IgnoreTaskbarOnMaximize = true;
                this.ShowMinButton = false;
                this.ShowCloseButton = false;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.ResizeMode = ResizeMode.NoResize;
                _fullscreen = true;
            }
        }

        private void MainWindow_PlayMedia(string obj)
        {
            if (File.Exists(obj))
            {
                try
                {

                _mediaElement.Source = new Uri(obj);
                _mediaElement.Play();
                _status = MediaElementStatus.Playing;
                _mediaControls.labelTime.Content = $"{_mediaElement.Position.Hours}:{_mediaElement.Position.Minutes}:{_mediaElement.Position.Seconds}";
                _timer.Start();

                }catch(Exception ex)
                {
                    richTextBoxDrawContent(ex.Message);
                }
            }
            else
            {
                richTextBoxDrawContent(obj + " not exist!");
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            play();
        }

        private void BtnVolumeUp_Click(object sender, RoutedEventArgs e)
        {
            _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.VolumeUp;
            _mediaElement.Volume+=0.05;
            if (_mediaElement.Volume >= 1.0)
            {
                _mediaElement.Volume = 1.0;
            }

            _mediaControls.lblVolumeLevel.Content = _mediaElement.Volume * 100;
            PictogramViewer(PictogramAction.Hide);
        }

        private void BtnVolumeDown_Click(object sender, RoutedEventArgs e)
        {
            _mediaElement.Volume-=0.05;
            if (_mediaElement.Volume <= 0)
            {
                _mediaElement.Volume = 0;
                _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.VolumeDown;
                PictogramViewer(PictogramAction.Show);
            }
            else
                PictogramViewer(PictogramAction.Hide);

            _mediaControls.lblVolumeLevel.Content = _mediaElement.Volume * 100;
        }

        private void BtnVolumeMute_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaElement.IsMuted || _mediaElement.Volume == 0)
            {
                _mediaElement.Volume = _volumeLevel;
                PictogramViewer(PictogramAction.Hide);
            }
            else
            {
                _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.VolumeMute;
                PictogramViewer(PictogramAction.Show);
                _volumeLevel = (double)_mediaElement.Volume;
                _mediaElement.Volume = 0;
            }

            _mediaControls.lblVolumeLevel.Content = Math.Round(_mediaElement.Volume * 100, 0);
        }

        private void BtnUrl_Click(object sender, RoutedEventArgs e)
        {
            _textBoxUrl.Visibility = Visibility.Visible;
            _textBoxUrl.Focus();
        }

        private void PictogramViewer(PictogramAction action)
        {
            if (action == PictogramAction.Hide)
            {
                _pictogramCanvas.Visibility = Visibility.Hidden;
            }
            else
            {
                _pictogramCanvas.Visibility = Visibility.Visible;
            }
        }

        private async Task PictogramViewerAsync(PictogramAction action, int delay_time)
        {
            await Task.Run((async () =>
            {
                if (action == PictogramAction.Hide)
                {
                    await Task.Delay(delay_time);
                    _pictogramCanvas.Visibility = Visibility.Hidden;
                }
                else
                {
                    _pictogramCanvas.Visibility = Visibility.Visible;
                    await Task.Delay(delay_time);
                    _pictogramCanvas.Visibility = Visibility.Hidden;
                }
            }));
        }

        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {
            // Launch the GitHub site with Themedit Player repository
            Process.Start(GitHubApplicationRepository.ToString());
        }

        private void HelpTopBarButton(object sender, RoutedEventArgs e)
        {
            // for help information
        }

        private SfbLibrary.Hooks.HookManager _hookManager;

        private void MetroWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += _timer_Tick;

            _mouseNotMoveTimer = new DispatcherTimer();
            _mouseNotMoveTimer.Interval = TimeSpan.FromSeconds(WaitTime);
            _mouseNotMoveTimer.Tick += mouseNotMoveTimer_Tick;

            _hookManager = new SfbLibrary.Hooks.HookManager();
            _hookManager.Devices.Add(new SfbLibrary.Hooks.Mouse());
            var devices = _hookManager.Devices[0] as SfbLibrary.Hooks.Mouse;
            devices.Install();
            devices.MouseMove += Hook_mouse_MouseMove;
            devices.MouseHover += Hook_mouse_MouseHover;

            Loaded -= MetroWindow_OnLoaded;
        }

        // 

        private void _mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_mediaElement != null)
            {
                if (Playlist.GetNext() != null)
                {
                    Playlist.SetCurrent(Playlist.GetNext().Path);
                    PlaylistWindow.SelectOnListView(Playlist.Current);
                    _mediaElement.Source = new Uri(Playlist.Current.Path);
                    _mediaElement.Play();
                    _mediaControls.labelTitle.Content = Playlist.Current.Path;
                    _status = MediaElementStatus.Playing;
                    Playlist.SetCurrent(Playlist.Current.Path);
                    _videoPath = Playlist.Current.Path;
                    _timer.Stop();
                    _timer.Start();
                    _lblTask.Content = Playlist.Current.Name;
                    DelayAsync(10000); 
                }
            }
        }

        private void Hook_mouse_MouseHover(SfbLibrary.Hooks.Mouse.MSLLHOOKSTRUCT mouseStruct)
        {
            _lblSubtitles.Content = mouseStruct.dwExtraInfo.ToString();
        }

        private void Hook_mouse_MouseMove(SfbLibrary.Hooks.Mouse.MSLLHOOKSTRUCT mouseStruct)
        {
            _mouseNotMoveTimer.Stop();

            ShowControls();
            //_lblTask.Content = mouseStruct.pt.x + ":" + mouseStruct.pt.y;

            _mouseNotMoveTimer.Start();
        }

        private void ShowControls()
        {
            this.Cursor = Cursors.Arrow;
            //_mediaControls.Visibility = Visibility.Visible;
            _lblTask.Visibility = Visibility.Visible;

            var task = Task.Run((() =>
            {
                this.BeginInvoke(() =>
                {
                    Storyboard myStoryboard = (Storyboard)(this.FindResource("fadeIn"));
                    if (_mediaControls.Opacity < 1.0)
                    {
                        myStoryboard.Begin();
                    }

                });
            }));
        }

        private void HideControls()
        {
            if (_lblTask.IsVisible && _mediaControls.IsVisible)
            {
                this.Cursor = Cursors.None;
                _lblTask.Visibility = Visibility.Hidden;

                Storyboard myStoryboard = (Storyboard)(this.FindResource("fadeOut"));
                myStoryboard.Begin();
            }
        }

        private void textBoxUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBoxUrl.Visibility = Visibility.Visible;
        }

        private void textBoxUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            _textBoxUrl.Visibility = Visibility.Hidden;
        }

        private void metroWindow_Closing(object sender, CancelEventArgs e)
        {
            SfbLibrary.Taskbar.WindowsTaskbar.Show();

        }

        private int _escHowManyPress = 0;

        private async void _mediaElement_KeyDown(object sender, KeyEventArgs e)
        {
            richTextBoxDrawContent(sender.GetType().Name);

            if (e.Key == Key.Escape)
            {
                _escHowManyPress++;
            }
            else
            {
                _escHowManyPress = 0;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    break;
                case Key.Tab:
                    ShowControls();
                    break;
                case Key.Space:
                        if (_status == MediaElementStatus.Paused)
                    {
                        _mediaElement.Play();
                        _status = MediaElementStatus.Playing;
                        _timer.Start();
                    }
                        else if (_status == MediaElementStatus.Playing)
                    {
                        _mediaElement.Pause();
                        _status = MediaElementStatus.Paused;
                        _timer.Stop();
                    }

                    _mediaControls.labelTitle.Content = $"Key pressed - SPACE - video is {_status} ";
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(1000);
                        _mediaControls.labelTitle.Content += $".";
                    }
                    
                    _mediaControls.labelTitle.Content = _videoPath;
                    break;
                case Key.Left:
                    if (_mediaElement.Position != TimeSpan.Zero)
                        if (e.IsToggled)
                        {
                            _mediaElement.Position -= TimeSpan.FromSeconds(10);
                            _mediaControls.labelTitle.Content = "Key left - Move video position -10sec - ";
                        }
                        else
                            _mediaElement.Position -= TimeSpan.FromSeconds(5);
                    break;
                case Key.Right:
                    
                        if (_mediaElement.Position != TimeSpan.Zero)
                            if (e.IsToggled)
                                _mediaElement.Position += TimeSpan.FromSeconds(10);
                            else
                                _mediaElement.Position += TimeSpan.FromSeconds(5);
                    break;
                case Key.N:
                    var next = Playlist.GetNext();
                    play(next);
                    break;
                case Key.S:
                    BtnSubtilesOnOff_Click(sender, e); // Subtitles on / off
                    break;
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Escape:
                    if (_escHowManyPress==1)
                    {
                        ClearFocus(sender);
                    }
                    if (_escHowManyPress==2)
                    {
                        _fullscreen = true;
                        FullscreenMode();
                        _escHowManyPress = 0;
                    }
                    break;
                case Key.F11:
                    FullscreenMode();
                    break;
            }
        }

        private object MovingObject;
        private Thickness _movingObjectPosition;

        private void _mediaControls_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control)
            {
                MovingObject = sender;
                richTextBoxDrawContent(e.GetPosition(_mediaControls as FrameworkElement).X + ":" + e.GetPosition(_mediaControls as FrameworkElement).Y.ToString());
                _movingObjectPosition = new Thickness(e.GetPosition(_mediaControls as FrameworkElement).X, e.GetPosition(_mediaControls as FrameworkElement).Y, 0, 0);
            }
        }

        private void _mediaControls_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MovingObject = null;
        }

        private void _mediaControls_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender.GetType() == typeof(MediaControls))
            {
                if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    _mediaControls.Margin = new Thickness(
                        e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - _movingObjectPosition.Left,
                        e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - _movingObjectPosition.Top,
                        0,
                        0);
                }
            }
        }

        private bool _mediaElementSelected = false;

        private void _mediaElement_GotFocus(object sender, RoutedEventArgs e)
        {
            _mediaElementSelected = true;
        }

        private void _mediaElement_LostFocus(object sender, RoutedEventArgs e)
        {
            _mediaElementSelected = false;
        }

        private void _mediaControls_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowControls();
        }

        private void _mediaControlPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private LayoutWindow _themeEditorWindow;

        private void OpenPlayerThemeEditor(object sender, RoutedEventArgs e)
        {
            if (_themeEditorWindow == null)
            {
                _themeEditorWindow = new LayoutWindow(); //new ThemeEditorWindow();
                _themeEditorWindow.Show();
            }
        }

        private void _mediaElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMediaElementSelected = true;
            richTextBoxDrawContent(sender.GetType().Name + ": " + _isMediaElementSelected.ToString());

            if (_status == MediaElementStatus.Playing)
            {
                _mediaElement.Pause();
                _status = MediaElementStatus.Paused;
                _timer.Stop();
            }
            else if (_status == MediaElementStatus.Paused)
            {
                _mediaElement.Play();
                _status = MediaElementStatus.Playing;
                _timer.Start();
            }

            ClearFocus(sender);
        }
        public void ClearFocus(object sender)
        {
            FrameworkElement parent = (FrameworkElement)_mediaControls.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(_mediaControls);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }

        public void SetupMediaElementMouseHoverEffect(MediaElement mediaElement)
        {
            mediaElement.MouseEnter += (sender, e) =>
            {
                if (sender is MediaElement)
                {
                    MediaElement element = (MediaElement)sender;
                    element.Focus();
                }
            };

            mediaElement.MouseLeave += (sender, e) =>
            {
                if (sender is MediaElement)
                {
                    MediaElement element = (MediaElement)sender;
                }
            };
        }

        private void _mediaControlPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMediaElementSelected = false;
        }

        private void _mediaControlPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            _mediaControls.Opacity = 1;
        }

        private void _mediaControlPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void _metroWindow_Closed(object sender, EventArgs e)
        {

        }
    }
    public enum ScreenOptions
    {
        FullscreenOn,
        FullscreenOff,
        Maximize,
        Minimize,
    }

    enum MediaElementStatus
    {
        Playing,
        Stoped,
        Paused,
        Closing,
        Opening,
        Buffored,
        Bufforing,
        Downloaded,
        Downloading,
    }

    public class LabelTimerEventArgs : EventArgs
    {
        private object _sender;
        public Brush Color { get; set; }
        public object LabelTimer { get { return _sender; } }

        public LabelTimerEventArgs()
        { 
            
        }

        public LabelTimerEventArgs(object sender, Brush brush = null)
        {
            if (brush == null)
            {
                brush = Brushes.Red;
                Color = brush;
            }
            else
            {
                Color = brush;
            }
            _sender = sender;
        }
    }
}
