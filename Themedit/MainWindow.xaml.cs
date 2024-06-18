// Version: 1.0.0.5
// Copyright (c) 2024 Softbery by Pawe� Tobis
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xaml;
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
        private int _volumeLevel = 50;
        private bool _fullscreen = false;
        private Subtitles.SubtitlesManager _subtitlesManager = null;
        private Dictionary<TimeSpan, SrtSub> _subtitles =  null;
        private bool _subtitlesShow = true;
        private bool _mediaElementEnd;

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

            PictogramViewer(PictogramAction.Hide);

            labelTimerColorSet(Brushes.Red);

            _textBoxUrl.Visibility = Visibility.Hidden;
            _lblSubtitles.Content = String.Empty;
        }

        private void OnMouseClickBeyondTextBox(object sender, EventArgs e)
        {
            if (BeyondTextBoxMouseClick!=null)
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
            _mediaControlPanel.btnPlay.Click += BtnPlay_Click;
            _mediaControlPanel.btnOpen.Click += BtnOpen_Click;
            _mediaControlPanel.btnStop.Click += BtnStop_Click;
            _mediaControlPanel.btnPause.Click += BtnPause_Click;
            _mediaControlPanel.btnClose.Click += BtnClose_Click;
            _mediaControlPanel.btnMute.Click += BtnVolumeMute_Click;
            _mediaControlPanel.btnVolumeUp.Click += BtnVolumeUp_Click;
            _mediaControlPanel.btnVolumeDown.Click += BtnVolumeDown_Click;
            _mediaControlPanel.btnFullscreen.Click += BtnFullscreen_Click;
            _mediaControlPanel.btnUrl.Click += BtnUrl_Click;
            _mediaControlPanel.btnSubtilesOnOff.Click += BtnSubtilesOnOff_Click;
            _mediaControlPanel.btnSubtiles.Click += BtnSubtiles_Click;
        }

        private void mediaControlsProgressBarEvents()
        {
            _mediaControlPanel.progressBarVideo.ValueChanged += ProgressBarVideo_ValueChanged;
            _mediaControlPanel.progressBarVideo.MouseDown += ProgressBarVideo_MouseDown;
            _mediaControlPanel.progressBarVolume.MouseDown += ProgressBarVolume_MouseDown;
        }

        private void keyboardEvents()
        {
            this.KeyDown += _mediaElement_KeyDown;
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
            var click_position = e.GetPosition(_mediaControlPanel.progressBarVolume).X;
            var width = _mediaControlPanel.progressBarVolume.ActualWidth;
            var result = (click_position / width) * _mediaControlPanel.progressBarVolume.Maximum;

            _mediaControlPanel.progressBarVolume.Value = (int)result;
            _mediaElement.Volume = _mediaControlPanel.progressBarVolume.Value;
            _mediaControlPanel.lblVolumeLevel.Content = (int)_mediaElement.Volume;
        }

        private void ProgressBarVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {
                var click_position = e.GetPosition(_mediaControlPanel.progressBarVideo).X;
                var width = _mediaControlPanel.progressBarVideo.ActualWidth;
                var result = (click_position / width) * _mediaControlPanel.progressBarVideo.Maximum;

                _mediaControlPanel.progressBarVideo.Value = result;

                var jump_to_sec = (_mediaElement.NaturalDuration.TimeSpan.TotalSeconds * result) / _mediaControlPanel.progressBarVideo.Maximum;
                _mediaElement.Position = TimeSpan.FromSeconds(jump_to_sec);
            }
        }

        private void ProgressBarVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string r = e.NewValue + " " + e.OldValue + "\n";
        }

        private void richTextBoxDrawContent(string text)
        {
            // Create a FlowDocument to contain content for the RichTextBox.
            FlowDocument myFlowDoc = new FlowDocument();

            // Create a Run of plain text and some bold text.
            Run myRun = new Run($"{text}");
            //Bold myBold = new Bold(new Run("edit me!"));

            // Create a paragraph and add the Run and Bold to it.
            Paragraph myParagraph = new Paragraph();
            myParagraph.Inlines.Add(myRun);
            //myParagraph.Inlines.Add(myBold);

            // Add the paragraph to the FlowDocument.
            myFlowDoc.Blocks.Add(myParagraph);

            // Add initial content to the RichTextBox.
            _mediaControlPanel.logRichTextBox.Document = myFlowDoc;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {
                // Control label time
                _mediaControlPanel.labelTime.Content = $"{_mediaElement.Position.Hours:00}:{_mediaElement.Position.Minutes:00}:{_mediaElement.Position.Seconds:00}/{_mediaElement.NaturalDuration.TimeSpan}";
                // Label - time left to end media
                _lblTimer.Content = $"-{GetTimeToVideoEnd().Hours:00}:{GetTimeToVideoEnd().Minutes:00}:{GetTimeToVideoEnd().Seconds:00}";
                // ProgressBar
                _mediaControlPanel.progressBarVideo.Value = (_mediaElement.Position.TotalSeconds * _mediaControlPanel.progressBarVideo.Maximum) / _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;

                if (_subtitles != null && _subtitlesShow == true)
                {
                    DrawSubtitles(_mediaElement.Position);
                }
            }
        }

        public void DrawSubtitles(TimeSpan time)
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
                            //_lblSubtitles.Content = String.Empty;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Time left to media end</returns>
        public TimeSpan GetTimeToVideoEnd()
        {
            return _mediaElement.NaturalDuration.TimeSpan - _mediaElement.Position;
        }

        private void OnLabelTimerColorChange(object sender, LabelTimerEventArgs e)
        {
            if (ChangeLabelTimerColorEvent != null)
            {
                var uc = sender as Label;

                e = new LabelTimerEventArgs(_mediaControlPanel.labelTime);
                uc.Foreground = e.Color;
                //uc.Background = e.Color;
                ChangeLabelTimerColorEvent?.Invoke(_mediaControlPanel.labelTime, e);
            }
        }

        private void labelTimerColorSet(Brush brush)
        {
            _lblTimer.Foreground = brush;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            _mediaControlPanel.labelTitle.Content = _status.ToString();
            Application.Current.Shutdown();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _mediaElement.Pause();
            _status = MediaElementStatus.Paused;
            _mediaControlPanel.labelTitle.Content = _status.ToString();
            _timer.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _mediaElement.Close();
            _status = MediaElementStatus.Closing;
            _mediaControlPanel.labelTitle.Content = _status.ToString();
            base.OnClosing(e);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _mediaElement.Stop();
            _mediaControlPanel.labelTitle.Content = _status.ToString();
            _status = MediaElementStatus.Stoped;
            _timer.Stop();
        }

        private void BtnSubtiles_Click(object sender, RoutedEventArgs e)
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
                _mediaControlPanel.labelSubtilesOnOff.Foreground = new SolidColorBrush(Color.FromArgb(100,255,0,0)); // Red color
                _mediaControlPanel.labelSubtilesOnOff.Content = "OFF";
                _lblSubtitles.Content = String.Empty;
                _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.Subtitles;
                PictogramViewerAsync(PictogramAction.Show, 5000);
            }
            else
            {
                _subtitlesShow = true;
                _mediaControlPanel.labelSubtilesOnOff.Foreground = new SolidColorBrush(Color.FromArgb(100,31,255,0)); // Green color
                _mediaControlPanel.labelSubtilesOnOff.Content = "ON";
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            _mediaControlPanel.labelTitle.Content = _status.ToString();
            var ofd = new OpenFileDialog();
            ofd.Filter = "Video files(*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg;)|*.avi;*.mp4;*.mkv;*.wmv;*.mpg;*.mpeg|" +
                             "Audio files(*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac;)|*.mp3;*.wma;*.wav;*.midi;*.ogg;*.flac|" +
                             "All files (*.*)|*.*";
            if (ofd.ShowDialog() != null)
            {
                try { 

                    if (_mediaElement != null)
                        _mediaElement.Stop();

                    _mediaElement.Source = new Uri(ofd.FileName);

                    if (SearchSubtitlesFiles(ofd.FileName) != null)
                    {
                        _subtitlesManager = new Subtitles.SubtitlesManager($"{SearchSubtitlesFiles(ofd.FileName)}");
                        _subtitles = _subtitlesManager.GetSubtitlesAsTimeSpan();
                    }

                    _mediaElement.Play();
                    _mediaControlPanel.labelTitle.Content = ofd.FileName;

                    if (_mediaElement.NaturalDuration.HasTimeSpan)
                    {
                        if (File.Exists($"{ofd.FileName}.tmp"))
                        {
                            var s = File.ReadAllText($"{ofd.FileName}.tmp");
                        }

                        _mediaControlPanel.lblVolumeLevel.Content = _volumeLevel.ToString();
                        _mediaElement.Volume = _volumeLevel;
                    }
                    _timer.Stop();
                    _timer.Start();
                }catch(Exception ex)
                {
                    richTextBoxDrawContent(ex.Message);
                }
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

        private void FullscreenMode(bool on_off=false)
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

                _mediaControlPanel.Visibility = Visibility.Visible;
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

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaElement != null)
            {
                if (_mediaElementEnd==true)
                {
                    _mediaElement.Stop();
                    _mediaElementEnd = false;
                }

                _mediaElement.Play();
                _status = MediaElementStatus.Playing;
                _mediaControlPanel.labelTime.Content = $"{_mediaElement.Position.Hours}:{_mediaElement.Position.Minutes}:{_mediaElement.Position.Seconds}";
                _timer.Start();
            }
        }

        private void BtnVolumeUp_Click(object sender, RoutedEventArgs e)
        {
            _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.VolumeUp;
            _mediaElement.Volume++;
            if (_mediaElement.Volume >= 100)
            {
                _mediaElement.Volume = 100;
            }

            _mediaControlPanel.lblVolumeLevel.Content = _mediaElement.Volume.ToString();
            PictogramViewer(PictogramAction.Hide);
        }

        private void BtnVolumeDown_Click(object sender, RoutedEventArgs e)
        {
            _mediaElement.Volume--;
            if (_mediaElement.Volume <= 0)
            {
                _mediaElement.Volume = 0;
                _icoPictogram.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.VolumeDown;
                PictogramViewer(PictogramAction.Show);
            }
            else
                PictogramViewer(PictogramAction.Hide);

            _mediaControlPanel.lblVolumeLevel.Content = _mediaElement.Volume.ToString();
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
                _volumeLevel = (int)_mediaElement.Volume;
                _mediaElement.Volume = 0;
            }

            _mediaControlPanel.lblVolumeLevel.Content = _mediaElement.Volume.ToString();
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
            await Task.Run((async() =>
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

            _mediaElement.MediaEnded += _mediaElement_MediaEnded;

            Loaded -= MetroWindow_OnLoaded;
        }

        private void _mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_mediaElement != null)
            {
                _mediaElementEnd = true;
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
            _lblTask.Content = mouseStruct.pt.x + ":" + mouseStruct.pt.y;

            _mouseNotMoveTimer.Start();
        }

        private async void ShowControls()
        {
            this.Cursor = Cursors.Arrow;
            //_mediaControlPanel.Visibility = Visibility.Visible;
            _lblTask.Visibility = Visibility.Visible;
            
            var task = Task.Run((() =>
            {
                this.BeginInvoke(() =>
                {
                    Storyboard myStoryboard = (Storyboard)(this.FindResource("fadeIn"));
                    if (_mediaControlPanel.Opacity < 1.0)
                    {
                        myStoryboard.Begin();
                    }
                    
                });
            }));
            /*
            if (task.IsCompleted)
            {
                _mediaControlPanel.Opacity = 1.0;
            }
            else
            {
                task.Wait();
            }*/

            /*if (_mediaControlPanel.Opacity != 1.0)
            {
                Storyboard myStoryboard = (Storyboard)(this.FindResource("fadeIn"));
                myStoryboard.Begin();
            }
            else
            {
                _mediaControlPanel.Opacity = 1.0;
            }*/

        }

        private void HideControls()
        {
            if (_lblTask.IsVisible && _mediaControlPanel.IsVisible)
            {
                this.Cursor = Cursors.None;
                //_mediaControlPanel.Visibility = Visibility.Hidden;
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

        private void _mediaElement_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    break;
                case Key.Tab:
                    ShowControls();
                    break;
                case Key.Space:
                    if (_status == MediaElementStatus.Paused)
                        _mediaElement.Play();
                    else if (_status == MediaElementStatus.Playing)
                        _mediaElement.Pause();
                    break;
                case Key.Left:
                    if (_mediaElement.Position != TimeSpan.Zero)
                        if (e.IsToggled)
                            _mediaElement.Position -= TimeSpan.FromSeconds(10);
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
                case Key.S:
                    BtnSubtilesOnOff_Click(sender, e); // Subtitles on / off
                    break;
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Escape:
                    FullscreenMode(true);
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
                richTextBoxDrawContent(e.GetPosition(_mediaControlPanel as FrameworkElement).X + ":" + e.GetPosition(_mediaControlPanel as FrameworkElement).Y.ToString());
                _movingObjectPosition = new Thickness(e.GetPosition(_mediaControlPanel as FrameworkElement).X, e.GetPosition(_mediaControlPanel as FrameworkElement).Y, 0, 0);
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
                    _mediaControlPanel.Margin = new Thickness(
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
            //Storyboard myStoryboard = (Storyboard)(this.FindResource("MoveMe"));
            //Canvas.SetTop(_mediaControlPanel, 694);
            //myStoryboard.Begin();
        }

        private ThemeEditorWindow _themeEditorWindow;

        private void OpenPlayerThemeEditor(object sender, RoutedEventArgs e)
        {
            if (_themeEditorWindow == null)
            {
                _themeEditorWindow = new ThemeEditorWindow();
                _themeEditorWindow.Show();
            }
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

    public class VideoMovingEventArgs : EventArgs
    {
        public TimeSpan Time { get; private set; }
        public MoveDirection Direction { get; private set; }

        public VideoMovingEventArgs()
        {

        }
    }

    public enum MoveDirection 
    {
        Backward,
        Forwarding,
        Current
    }
}