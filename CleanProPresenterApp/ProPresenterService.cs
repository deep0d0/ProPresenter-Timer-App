using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CleanProPresenterApp
{
    public class ProPresenterService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ProPresenterService(string ip = "192.168.1.112", int port = 1982)
        {
            _baseUrl = $"http://{ip}:{port}";
            _httpClient = new HttpClient();
        }

        public async Task<string> GetVideoCountDownAsync()
        {
            string uri = $"{_baseUrl}/v1/timer/video_countdown";
            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode) return "00:00:00";

                using var stream = await response.Content.ReadAsStreamAsync();
                using var sr = new StreamReader(stream);
                string result = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<string>(result) ?? "00:00:00";
            }
            catch { return "00:00:00"; }
        }

        public async Task<string> GetAudioTimeRemainingAsync()
        {

            try
            {
                // Get total duration
                string durationUri = $"{_baseUrl}/v1/transport/audio/current";
                var durationRes = await _httpClient.GetAsync(durationUri);
                string durationString = "00:00:00";

                if (durationRes.IsSuccessStatusCode)
                {
                    using var sr = new StreamReader(await durationRes.Content.ReadAsStreamAsync());
                    string json = await sr.ReadToEndAsync();
                    var result = JsonConvert.DeserializeObject<AudioClass>(json);
                    durationString = result?.duration ?? "00:00:00";
                }

                // Get current time
                string timeUri = $"{_baseUrl}/v1/transport/audio/time";
                var timeRes = await _httpClient.GetAsync(timeUri);
                if (!timeRes.IsSuccessStatusCode) return "0";

                using var srTime = new StreamReader(await timeRes.Content.ReadAsStreamAsync());
                string rawTime = await srTime.ReadToEndAsync();
                double currentSeconds = JsonConvert.DeserializeObject<double>(rawTime);

                TimeSpan total = TimeSpan.Zero;
                if (double.TryParse(durationString, out var totalSeconds))
                {
                    total = TimeSpan.FromSeconds(totalSeconds);
                    var remaining = total - TimeSpan.FromSeconds(currentSeconds);
                    return remaining.TotalSeconds > 0 ? remaining.TotalSeconds.ToString("F6") : "0";
                }
            }
            catch (Exception ex)
            {
            }

            return "0";
        }

        public class AudioClass
        {
            public string duration { get; set; } = "00:00:00";
        }
    }
}