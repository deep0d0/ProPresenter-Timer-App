using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ProPresenterTimerApp
{
    public class Clip
    {
        public Int64 id;
        public object name; //StringParameter {...}
        public object colorid; //ChoiceParameter {...}
        public object selected; //{...}
        public object connected; //	{...}
        public object target; //ChoiceParameter {...}
        public object triggerstyle; //ChoiceParameter {...}
        public object ignorecolumntrigger; //ChoiceParameter {...}
        public object faderstart; //ChoiceParameter {...}
        public object beatsnap; //ChoiceParameter {...}
        public object transporttype; //ChoiceParameter {...}
        public Transport transport; //  {...}
        public object dashboard; //ParameterCollection {...}
        public object audio; // AudioTrackClip {...}
        public object video; //VideoTrackClip {...}
        public object thumbnail; //{...}
    }

    public class Transport
    {
        public Position position;
        public object controls;
    }

    public class Position
    {
        public Int64 id;
        public string valuetype;
        public double min;
        public double max;
        public double @in;
        public double @out;
        public double value;
    }
}
