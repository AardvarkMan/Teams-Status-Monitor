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
    /// Interaction logic for GroupSelector.xaml
    /// </summary>
    public partial class GroupSelector : Window
    {
        public GroupSelector()
        {
            InitializeComponent();
        }

        public void Groups(SortedList<string,string> Groups)
        {
            lbGroups.ItemsSource = Groups.Values.ToArray();
        }
    }
}
