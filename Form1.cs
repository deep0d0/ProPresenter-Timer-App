using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProPresenterTimerApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            int height = this.Height / 4;

            this.label3.Location = new Point(0, 0);
            this.label3.Width = this.Width;
            this.label3.Height = height;

            this.label1.Location = new Point(0, height);
            this.label1.Width = this.Width;
            this.label1.Height = height;

            this.label4.Location = new Point(0, height * 2);
            this.label4.Width = this.Width;
            this.label4.Height = height;

            this.label2.Location = new Point(0, height * 3);
            this.label2.Width = this.Width;
            this.label2.Height = height;

            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label2.TextAlign = ContentAlignment.MiddleCenter;
            this.label3.TextAlign = ContentAlignment.MiddleCenter;
            this.label4.TextAlign = ContentAlignment.MiddleCenter;

            this.label1.Text = GetVideoCountDownCenterScreen();
            this.label2.Text = GetVideoCountDownSideScreen();
            this.timer1.Enabled = true;

            //< !--ProPresenter / Resolume-- >
            //< add key = "MainScreenUses" value = "Resolume" />
            MainScreenUses = ConfigurationManager.AppSettings["MainScreenUses"];

            if (MainScreenUses != null && MainScreenUses == "ProPresenter") 
                MainScreenURL = $"{ConfigurationManager.AppSettings["MainScreenIPAddress"]}/v1/timer/video_countdown";
            else if (MainScreenUses != null && MainScreenUses == "Resolume")
                MainScreenURL = $"{ConfigurationManager.AppSettings["MainScreenIPAddress"]}/api/v1/composition/clips/selected";

            //< !--ProPresenter / Resolume-- >
            //< add key = "MainScreenUses" value = "Resolume" />
            SideScreenUses = ConfigurationManager.AppSettings["SideScreenUses"];

            if (SideScreenUses != null && SideScreenUses == "ProPresenter")
                SideScreenURL = $"{ConfigurationManager.AppSettings["SideScreenIPAddress"]}/v1/timer/video_countdown";
            else if (SideScreenUses != null && SideScreenUses == "Resolume")
                SideScreenURL = $"{ConfigurationManager.AppSettings["SideScreenIPAddress"]}/api/v1/composition/clips/selected";
            
        }

        private string GetAudioTimeRemains()
        {
            string totalAudioTime = GetAudioCountDown();
            string currentAudioTime = GetAudioCountDown(true);
            double timeRemains = double.Parse(totalAudioTime) - double.Parse(currentAudioTime);
            TimeSpan timeRemainsSpan = TimeSpan.FromSeconds(timeRemains);
            return timeRemainsSpan.ToString(@"hh\:mm\:ss");
        }

        private string GetVideoCountDownCenterScreen()
        {
            string CountDown = "00:00:00";
            string timeOut = $"{ConfigurationManager.AppSettings["TimeOut"]}";
            try
            {
                if(MainScreenURL == null) 
                {
                    return CountDown;
                }
                
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(MainScreenURL);
                req.Timeout = int.Parse(timeOut);
                req.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string result = sr.ReadToEnd();
                    if(MainScreenUses != null && MainScreenUses == "ProPresenter")
                        CountDown = JsonConvert.DeserializeObject<string>(result);
                    else if (MainScreenUses != null && MainScreenUses == "Resolume")
                    { 
                        var output = JsonConvert.DeserializeObject<Clip>(result);
                        double totalAudioTime = output.transport.position.max; //Total Time in MilliSeconds
                        double currentAudioTime = output.transport.position.value; //Current Time in MilliSeconds
                        double timeRemains = totalAudioTime - currentAudioTime;
                        TimeSpan timeRemainsSpan = TimeSpan.FromMilliseconds(timeRemains);
                        CountDown = timeRemainsSpan.ToString(@"hh\:mm\:ss");
                    }
                }
            }
            catch (Exception) 
            {
            }

            return CountDown;
        }

        private string GetVideoCountDownSideScreen()
        {
            string CountDown = "00:00:00";
            string timeOut = $"{ConfigurationManager.AppSettings["TimeOut"]}";
            try
            {
                if (SideScreenURL == null)
                {
                    return CountDown;
                }

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(SideScreenURL);
                req.Timeout = int.Parse(timeOut);
                req.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string result = sr.ReadToEnd();
                    if (SideScreenUses != null && SideScreenUses == "ProPresenter")
                        CountDown = JsonConvert.DeserializeObject<string>(result);
                    else if (SideScreenUses != null && SideScreenUses == "Resolume")
                    {
                        var output = JsonConvert.DeserializeObject<Clip>(result);
                        double totalAudioTime = output.transport.position.max; //Total Time in MilliSeconds
                        double currentAudioTime = output.transport.position.value; //Current Time in MilliSeconds
                        double timeRemains = totalAudioTime - currentAudioTime;
                        TimeSpan timeRemainsSpan = TimeSpan.FromMilliseconds(timeRemains);
                        CountDown = timeRemainsSpan.ToString(@"hh\:mm\:ss");
                    }
                }
            }
            catch (Exception)
            {
            }

            return CountDown;
        }

        private string GetAudioCountDown(bool isCurrentTime = false)
        {
            string CountDown = "00:00:00";
            string uri;
            if (isCurrentTime) //Get Current Audio Time
                uri = $"{ConfigurationManager.AppSettings["ProPresenterIPAddress"]}/v1/transport/audio/time";
            else //Get Total Audio Time
                uri = $"{ConfigurationManager.AppSettings["ProPresenterIPAddress"]}/v1/transport/audio/current";

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();

                if (isCurrentTime)
                    CountDown = JsonConvert.DeserializeObject<string>(result);
                else
                {
                    AudioClass audioClass = JsonConvert.DeserializeObject<AudioClass>(result);
                    CountDown = audioClass.duration;
                }
            }

            return CountDown;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label1.Text = GetVideoCountDownCenterScreen();
            this.label2.Text = GetVideoCountDownSideScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            int height = this.Height / 4;

            this.label3.Location = new Point(0, 0);
            this.label3.Width = this.Width;
            this.label3.Height = height;

            this.label1.Location = new Point(0, height);
            this.label1.Width = this.Width;
            this.label1.Height = height;

            this.label4.Location = new Point(0, height * 2);
            this.label4.Width = this.Width;
            this.label4.Height = height;

            this.label2.Location = new Point(0, height * 3);
            this.label2.Width = this.Width;
            this.label2.Height = height;

            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label2.TextAlign = ContentAlignment.MiddleCenter;
            this.label3.TextAlign = ContentAlignment.MiddleCenter;
            this.label4.TextAlign = ContentAlignment.MiddleCenter;


        }
    }

    public class AudioClass
    {
        public bool is_playing { get; set; }
        public Dictionary<string, string> uuid { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public bool audio_only { get; set; }
        public string duration { get; set; }
    }
}
