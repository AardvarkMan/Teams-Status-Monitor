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
    /// Interaction logic for UserDetails.xaml
    /// </summary>
    public partial class UserDetails : UserControl
    {
        MonitorUser _userData;

        public bool isOdd
        {
            get { return false; }
            set
            {
                if (value)
                    this.Background = new SolidColorBrush(Colors.LightGray);
            }
        }
        public MonitorUser UserData
        {
            set 
            {
                _userData = value;
                lblUserName.Content = value.Display;
                chkEnabled.IsChecked = value.Enabled;
                txtLEDStart.Text = value.LEDStart.ToString();
                txtLEDEnd.Text = value.LEDStart.ToString();
            }
            get
            {
                _userData.Enabled = chkEnabled.IsChecked ?? false;
                return _userData;
            }
        }
        public UserDetails()
        {
            InitializeComponent();
        }

        private void chkEnabled_Checked(object sender, RoutedEventArgs e)
        {
            _userData.Enabled = chkEnabled.IsChecked ?? false;
        }

        private void txtLEDStart_LostFocus(object sender, RoutedEventArgs e)
        {
            int tmpInt = 0;
            if (Int32.TryParse(txtLEDStart.Text, out tmpInt))
            {
                _userData.LEDStart = tmpInt;
            }
        }

        private void txtLEDEnd_LostFocus(object sender, RoutedEventArgs e)
        {
            int tmpInt = 0;
            if (Int32.TryParse(txtLEDEnd.Text, out tmpInt))
            {
                _userData.LEDEnd = tmpInt;
            }
        }
    }
}
