
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace CleanProPresenterApp
{
    public partial class SettingsWindow : Window
    {
        public string IpAddress { get; private set; } = "";
        public int Port { get; private set; }

        
        public SettingsWindow() { InitializeComponent(); }
public SettingsWindow(string currentIp, int currentPort)
        {
            InitializeComponent();
            IpInput.Text = currentIp;
            PortInput.Text = currentPort.ToString();
        }

        private void OnCancel(object? sender, RoutedEventArgs e) => this.Close();

        private void OnSave(object? sender, RoutedEventArgs e)
        {
            IpAddress = IpInput.Text ?? "";
            if (int.TryParse(PortInput.Text, out int parsedPort))
            {
                Port = parsedPort;
                this.Close(true);
            }
            else
            {
                var dlg = new Window { Title = "Invalid Port" };
                dlg.ShowDialog(this);
            }
        }
    }
}
