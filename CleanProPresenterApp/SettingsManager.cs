
using System.IO;
using Newtonsoft.Json;

namespace CleanProPresenterApp
{
    public static class SettingsManager
    {
        private const string FileName = "settings.json";

        public static (string ip, int port) Load()
        {
            if (File.Exists(FileName))
            {
                var json = File.ReadAllText(FileName);
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                return (settings?.Ip ?? "192.168.1.112", settings?.Port ?? 1982);
            }

            return ("192.168.1.112", 1982);
        }

        public static void Save(string ip, int port)
        {
            var settings = new Settings { Ip = ip, Port = port };
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        private class Settings
        {
            public string Ip { get; set; } = "";
            public int Port { get; set; }
        }
    }
}
