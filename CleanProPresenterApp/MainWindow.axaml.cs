
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace CleanProPresenterApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer? _timer;
        private ProPresenterService _service;
        private string _ipAddress;
        private int _port;

        public MainWindow()
        {
            InitializeComponent();
            (_ipAddress, _port) = SettingsManager.Load();
            _service = new ProPresenterService(_ipAddress, _port);

            SettingsButton.Click += async (_, _) => await OpenSettingsAsync();
            this.Opened += OnOpened;
        }

        private void OnOpened(object? sender, EventArgs e)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _timer.Tick += (_, _) => RunTimerTick();
            _timer.Start();
        }

        private void RunTimerTick()
        {
            _ = TimerTickAsync();
        }

        private async Task TimerTickAsync()
        {
            double audioSeconds = 0;
            try
            {
                Label3.Text = $"Connected to: {_ipAddress}:{_port}";
                Label4.Text = $"Updated: {DateTime.Now:HH:mm:ss.fff}";

                string video = "00:00:00";
                string audioRaw = "0";
                TimeSpan audioTime = TimeSpan.Zero;

                bool videoValid = false;
                bool audioValid = false;

                try
                {
                    video = await _service.GetVideoCountDownAsync();
                    videoValid = TimeSpan.TryParse(video, out var videoTs) && videoTs.TotalSeconds > 0;
                }
                catch
                {
                    video = "00:00:00";
                }

                try
                {
                    audioRaw = await _service.GetAudioTimeRemainingAsync();
                    // Skipped TimeSpan.TryParse (expecting raw double)
                    {
                        
                    }
                    if (double.TryParse(audioRaw, out var parsedSeconds))
                    {
                        audioSeconds = parsedSeconds;
                        audioTime = TimeSpan.FromSeconds(audioSeconds);
                        audioValid = audioSeconds > 0;
                    }
                }
                catch
                {
                    audioRaw = "0";
                }

                string videoStatus = videoValid ? "🟢" : "🔴";
                string audioStatus = audioValid ? "🟢" : "🔴";

                Label1.Text = $"{videoStatus} Video: {video}";
                
                var audioH = (int)(audioSeconds / 3600);
                var audioM = (int)((audioSeconds % 3600) / 60);
                var audioS = (int)(audioSeconds % 60);
                var audioMs = (int)((audioSeconds - Math.Floor(audioSeconds)) * 1000);
                Label2.Text = $"{audioStatus} Audio: {audioH:D2}:{audioM:D2}:{audioS:D2}.{audioMs:D3}";

            }
            catch (Exception ex)
            {
                Label1.Text = "🔴 Video: Error";
                Label2.Text = "🔴 Audio: Error";
                Label3.Text = $"Timer Failure: {ex.Message}";
            }
        }

        private async Task OpenSettingsAsync()
        {
            var settingsWindow = new SettingsWindow(_ipAddress, _port);
            var result = await settingsWindow.ShowDialog<bool?>(this);
            if (result == true)
            {
                _ipAddress = settingsWindow.IpAddress ?? "192.168.1.112";
                _port = settingsWindow.Port;
                SettingsManager.Save(_ipAddress, _port);
                _service = new ProPresenterService(_ipAddress, _port);
            }
        }
    }
}