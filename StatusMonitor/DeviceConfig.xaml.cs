using Microsoft.Graph;
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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StatusMonitor
{
    /// <summary>
    /// Interaction logic for DeviceConfig.xaml
    /// </summary>
    public partial class DeviceConfig : Window
    {
        public string DeviceName
        {
            get
            {
                return txtName.Text;
            }
            set
            {
                txtName.Text = value;
            }
        }

        public string IPAddress
        {
            get
            {
                return txtIPAddress.Text;
            }
            set
            {
                txtIPAddress.Text = value;
            }
        }

        public int Port
        {
            get
            {
                return Int32.Parse(txtPort.Text);
            }
            set
            {
                txtPort.Text = value.ToString();
            }
        }

        public int LEDCount
        {
            get
            {
                return Int32.Parse(txtLEDCount.Text);
            }
            set
            {
                txtLEDCount.Text = value.ToString();
            }
        }

        public Boolean FillString
        {
            get
            {
                if (chkFillString.IsChecked == true) return true;
                return false;
            }
            set
            {
                chkFillString.IsChecked = value;
            }
        }

        public Boolean RotateString
        {
            get
            {
                if(chkRotateLights.IsChecked == true) return true;
                else return false;
            }
            set
            {
                chkRotateLights.IsChecked = value;
            }
        }

        private DeviceSettings _deviceSettings;
        public DeviceSettings deviceSettings
        {
            get
            {
                _deviceSettings.DeviceName = this.DeviceName;
                _deviceSettings.TargetIPAddress = this.IPAddress;
                _deviceSettings.TargetPort = this.Port;
                _deviceSettings.LEDCount = this.LEDCount;
                _deviceSettings.FillString = this.FillString;
                _deviceSettings.RotateLights = this.RotateString;
                return _deviceSettings;
            }

            set
            {
                _deviceSettings = value;
                UpdateDisplay();
            }
        }

        public DeviceConfig()
        {
            InitializeComponent();
            _deviceSettings = new DeviceSettings();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {

            this.DeviceName = _deviceSettings.DeviceName;
            this.IPAddress = _deviceSettings.TargetIPAddress;
            this.Port = _deviceSettings.TargetPort;
            this.LEDCount = _deviceSettings.LEDCount;
            this.FillString = _deviceSettings.FillString;
            this.RotateString = _deviceSettings.RotateLights;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
