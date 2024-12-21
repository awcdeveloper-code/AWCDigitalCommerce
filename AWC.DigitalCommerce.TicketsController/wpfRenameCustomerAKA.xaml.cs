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
    /// Interaction logic for wpfRenameCustomerAKA.xaml
    /// </summary>
    public partial class wpfRenameCustomerAKA : Window
    {
        public string newCustAKA = string.Empty;
        public wpfRenameCustomerAKA(string custAKA)
        {
            InitializeComponent();
            lblCustomerAKA.Content = custAKA;
            txtNewCustomerAKA.Focus();
        }

        private void txtNewCustomerAKA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNewCustomerAKA.Text.Length > 0)
            {
                btnOK.IsEnabled = true;
            }
            else
            {
                btnOK.IsEnabled = false;
            }
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            newCustAKA = string.Empty;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            newCustAKA = txtNewCustomerAKA.Text.ToUpper();
            this.Close();
        }
    }
}
