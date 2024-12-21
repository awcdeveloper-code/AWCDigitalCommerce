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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfCustomerList.xaml
    /// </summary>
    public partial class wpfCustomerList : Window
    {
        public int customerID = 0;
        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();

        public wpfCustomerList()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_CustomerID.ItemsSource = lstVIP;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            customerID = 0;
            this.Close();
        }

        private void btn_Assign(object sender, RoutedEventArgs e)
        {
            customerID = custProfile.ID;
            this.Close();
        }

        private void lBox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            custProfile = lBox_CustomerID.SelectedItem as clsCustomerVIP;
            customerID = custProfile.ID;
            Reassign.IsEnabled = true;
        }
    }
}
