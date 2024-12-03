using System;
using System.Collections.Generic;
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

namespace StatusMonitor
{
    /// <summary>
    /// Interaction logic for UserStatus.xaml
    /// </summary>
    public partial class UserStatus : UserControl
    {
        MonitorUser _userData;
        public MonitorUser UserData
        {
            set
            {
                _userData = value;
                lblUserName.Content = value.Display;
                lblStatus.Content = value.Status;
                EvaluateStatus(value.Status);
            }
        }

        public bool isOdd
        {
            get { return false; }
            set
            {
                if (value)
                    this.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void EvaluateStatus(string p)
        {
            if (p.ToLower() == "busy" || p.ToLower() == "donotdisturb" || p.ToLower() == "busyidle")
            {
                this.lblStatus.Background = new SolidColorBrush(Colors.Red);
            }
            else if (p.ToLower() == "available")
            {
                this.lblStatus.Background = new SolidColorBrush(Colors.Green);
            }
            else if (p.ToLower() == "away")
            {
                this.lblStatus.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                this.lblStatus.Background = new SolidColorBrush(Colors.White);
            }
        }

        public string Id
        {
            get { return _userData.Id; }
        }

        public UserStatus()
        {
            InitializeComponent();
        }
    }
}
