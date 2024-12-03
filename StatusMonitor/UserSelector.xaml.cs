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
    /// Interaction logic for UserSelector.xaml
    /// </summary>
    public partial class UserSelector : Window
    {
        public List<MonitorUser> Users 
        { 
            get; 
            set;
        }

        public UserSelector(List<MonitorUser> users)
        {
            InitializeComponent();
            Users = users;
            UpdateDisplay(users);
        }

        private void UpdateDisplay(List<MonitorUser> users)
        {
            spUsers.Children.Clear();
            bool Odd = false;

            foreach(MonitorUser user in users)
            {
                UserDetails uDetails = new UserDetails();
                uDetails.isOdd = Odd;
                spUsers.Children.Add(uDetails);
                uDetails.UserData = user;
                Odd = !Odd;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void mnuSetCount_Click(object sender, RoutedEventArgs e)
        {
            int Count = 0;
            foreach (MonitorUser user in Users)
            {
                if (user.Enabled)
                {
                    user.LEDStart = Count;
                    user.LEDEnd = Count;
                    Count++;
                }
            }

            UpdateDisplay(Users);
        }
    }
}
