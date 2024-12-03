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
using System.Windows.Shapes;

namespace StatusMonitor
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public event EventHandler ReDrawRequest;

        SystemSettings systemSettings = new SystemSettings();
        List<DeviceSettings> ConfiguredDevices = new List<DeviceSettings>();

        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            systemSettings.LoadSettings();
            txtEmailFilter.Text = systemSettings.EMailFilter;
            ConfiguredDevices = systemSettings.LoadDeviceSettings();
        }

        public void UpdateDisplay()
        {
            spDevices.Children.Clear();
            foreach (DeviceSettings deviceSetting in ConfiguredDevices)
            {
                if (deviceSetting.Deleted == false)
                {
                    DeviceDetails uDetails = new DeviceDetails();
                    uDetails.parentWindow = this;
                    spDevices.Children.Add(uDetails);
                    uDetails.DeviceSettings = deviceSetting;
                }
            }
        }

        public void RetrieveSettings()
        {
            ConfiguredDevices.Clear();
            foreach (UserControl uDetails in spDevices.Children)
            {
                if(uDetails is DeviceDetails)
                {
                    if (((DeviceDetails)uDetails).DeviceSettings.Deleted == false)
                        ConfiguredDevices.Add(((DeviceDetails)uDetails).DeviceSettings);
                }
            }

            UpdateDisplay();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            systemSettings.EMailFilter = txtEmailFilter.Text;
            systemSettings.SaveSettings();
            systemSettings.SaveDeviceSettings(ConfiguredDevices);
            this.DialogResult = true;
            this.Close();
        }

        private void btnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            DeviceConfig deviceConfig = new DeviceConfig();
            if(deviceConfig.ShowDialog().Value)
            {
                ConfiguredDevices.Add(deviceConfig.deviceSettings);
                UpdateDisplay();
            }
        }

        private void btnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        protected virtual void OnRedrawRequest(EventArgs eventArgs)
        {
            UpdateDisplay();
        }
    }
}
