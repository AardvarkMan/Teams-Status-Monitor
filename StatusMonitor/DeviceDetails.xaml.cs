using Newtonsoft.Json.Linq;
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
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetails : UserControl
    {
        
        public Settings parentWindow
        {
            get;
            set;
        }

        DeviceSettings _deviceSettings;
        public DeviceSettings DeviceSettings
        {
            get
            {
                return _deviceSettings;
            }
            set
            {
                _deviceSettings = value;
                UpdateDisplay();
            }
        }

        public DeviceDetails()
        {
            InitializeComponent();
        }

        private void UpdateDisplay()
        {
            txtIPAddress.Content = _deviceSettings.TargetIPAddress;
            txtPort.Content = _deviceSettings.TargetPort;
            txtName.Content = _deviceSettings.DeviceName;
            txtLEDCount.Content = _deviceSettings.LEDCount;
        }


        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DeviceConfig deviceConfig = new DeviceConfig();
            deviceConfig.deviceSettings = _deviceSettings;
            if (deviceConfig.ShowDialog().Value)
            {
                this.DeviceSettings = deviceConfig.deviceSettings;
                UpdateDisplay();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _deviceSettings.Deleted = true;
            parentWindow.UpdateDisplay();
        }
    }
}
