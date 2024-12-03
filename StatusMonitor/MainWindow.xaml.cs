using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace StatusMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MicrosoftGraphInterface graph = new MicrosoftGraphInterface();
        System.Windows.Threading.DispatcherTimer updateTimer;
        List<DeviceSettings> devices = new List<DeviceSettings>();
        SystemSettings settings = new SystemSettings();
        List<MonitorUser> Users;

        UDPInterface udpInterface;

        public MainWindow()
        {
            udpInterface = new UDPInterface();
            updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 15);
            updateTimer.Tick += UpdateTimer_Tick;

            InitializeComponent();
            LoadSettings();
            StartGraph();

            txtStatus.Content = "Ready";
        }

        private void LoadSettings()
        {
            settings.LoadSettings();
            devices = settings.LoadDeviceSettings();
        }

        private async void StartGraph()
        {
            if(string.IsNullOrEmpty(settings.EMailFilter)) { DisplaySettings(); }

            if(await graph.StartConnection())
            {
                SortedList<string, string> People = await graph.GetUsers(settings.EMailFilter);
                Users = MonitorUser.FromList(People);

                if(string.IsNullOrEmpty(settings.UserSelection)) { ShowUserSelectionDialog(); }
                else { settings.LoadUserSelection(Users); }

                PopulateUsers();
            }
        }

        private void ShowUserSelectionDialog()
        {
            UserSelector userSelector = new UserSelector(Users);
            if (userSelector.ShowDialog().Value)
            {
                settings.SaveUserSelection(Users);
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            getPresence();            
        }

        public async void getPresence()
        {
            string UDPMessage = string.Empty;
            bool PopulatingLights = true;
            int CurrentLED = 0;

            txtStatus.Content = "Retrieving Teams Status.";
            //Retrieve User Data Once
            foreach (MonitorUser user in Users.Where(l => l.Enabled == true).OrderBy(i => i.LEDStart))
            {
                try
                {
                    Microsoft.Graph.Presence P = await graph.GetPresence(user.Id);
                    user.Status = P.Availability;
                }
                catch { };
            }

            PopulateUsers();

            //Loop through each connected device
            foreach (DeviceSettings device in devices)
            {
                txtStatus.Content = "Updating Device " + device.DeviceName;
                UDPMessage = string.Empty;
                CurrentLED = 0;
                PopulatingLights = true;

                string[] LEDStatus = new string[device.LEDCount];
                for (int i = 0; i < device.LEDCount; i++) { LEDStatus[i] = "O"; }

                //Loop through the LED Handling
                while (PopulatingLights)
                {
                    foreach (MonitorUser user in Users.Where(l => l.Enabled == true).OrderBy(i => i.LEDStart))
                    {
                        try
                        {
                            for (int i = user.LEDStart; i <= user.LEDEnd; i++)
                            {
                                if (CurrentLED < device.LEDCount)
                                {
                                    LEDStatus[CurrentLED] = EvaluateStatus(user.Status);
                                    CurrentLED++;
                                }
                            }
                        }
                        catch { };
                    }

                    if ((CurrentLED < device.LEDCount) && device.FillString) { PopulatingLights = true; }
                    else { PopulatingLights = false; }
                }

                for (int i = device.RotationOffset; i < device.LEDCount; i++)
                {
                    UDPMessage = UDPMessage + LEDStatus[i];
                }
                for (int i = 0; i < device.RotationOffset; i++)
                {
                    UDPMessage = UDPMessage + LEDStatus[i];
                }

                if (device.RotateLights)
                {
                    device.RotationOffset++;
                    if (device.RotationOffset >= device.LEDCount)
                    {
                        device.RotationOffset = 0;
                    }
                }

                udpInterface.SendPacket(device.TargetIPAddress, device.TargetPort, UDPMessage);
            }

            txtStatus.Content = "Waiting";
        }

        private string EvaluateStatus(string AvailabilityText)
        {
            string Status = "O";

            if (AvailabilityText.ToLower() == "busy" || AvailabilityText.ToLower() == "donotdisturb")
            {
                Status = "B";
            }
            else if(AvailabilityText.ToLower() == "available")
            {
                Status = "A";
            }
            else if (AvailabilityText.ToLower() == "away")
            {
                Status = "Y";
            }

            return Status;
        }
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (updateTimer.IsEnabled)
            {
                updateTimer.Stop();
                btnStart.Content = "Start";
                txtStatus.Content = "Ready";
            }
            else
            {
                getPresence();
                updateTimer.Start();
                btnStart.Content = "Stop";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            graph.SingOut();
        }

        private void mnuSettings_Click(object sender, RoutedEventArgs e)
        {
            DisplaySettings();
        }

        private void DisplaySettings()
        {
            Settings settingsPage = new Settings();
            settingsPage.ShowDialog();
            LoadSettings();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnuUserSelect_Click(object sender, RoutedEventArgs e)
        {
            ShowUserSelectionDialog();
        }

        private void PopulateUsers()
        {
            bool Odd = false;
            spUsers.Children.Clear();
            
            foreach (MonitorUser user in Users)
            {
                if (user.Enabled)
                {
                    UserStatus uDetails = new UserStatus();
                    uDetails.isOdd = Odd;
                    Odd = !Odd;
                    spUsers.Children.Add(uDetails);
                    uDetails.UserData = user;
                }
            }
        }
    }
}
