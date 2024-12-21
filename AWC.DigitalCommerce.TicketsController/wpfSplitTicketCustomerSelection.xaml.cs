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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSplitTicketCustomerSelection.xaml
    /// </summary>
    public partial class wpfSplitTicketCustomerSelection : Window
    {
        private List<clsCustomerVIP> lstCustomers = new List<clsCustomerVIP>();
        public clsCustomerVIP custProfile = new clsCustomerVIP();
        public bool isOK = false;

        public wpfSplitTicketCustomerSelection()
        {
            InitializeComponent();
            lstCustomers = DB.ListBinding_tbl_CustomerID(5, 0);
            lBox_Customer.ItemsSource = lstCustomers;
        }

        private void txtSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchCustomer.Text.ToUpper();

            var empFiltered = from vip in lstCustomers
                              let ename = vip.CustomerID
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
                              select vip;

            lBox_Customer.ItemsSource = empFiltered;

        }

        private void lBox_Customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            custProfile = lBox_Customer.SelectedItem as clsCustomerVIP;
            btnOK.IsEnabled = true;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            isOK = true;
            this.Close();
        }
    }
}
