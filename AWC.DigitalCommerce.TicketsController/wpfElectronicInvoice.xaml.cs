using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for wpfElectronicInvoice.xaml
    /// </summary>
    public partial class wpfElectronicInvoice : Window
    {
        private int tck = 0;
        public bool bCancel = false;
        public string custName = string.Empty;
        public int custIDType = 0;
        public int custID = 0;
        public int custCountryCode = 0;
        public int custPhoneNumber = 0;
        public string custEmail = string.Empty;

        public wpfElectronicInvoice(int ticketNumber)
        {
            InitializeComponent();

            tck = ticketNumber;
            lblTicketNumber.Content = tck.ToString();

            cbox_IDType.Items.Add("CED FISICA");
            cbox_IDType.Items.Add("CED JURIDICA");
            cbox_IDType.Items.Add("NITE");
            cbox_IDType.Items.Add("DIMEX");

            txtBox_CustomerName.Focus();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            custName = txtBox_CustomerName.Text.ToUpper();
            custIDType = cbox_IDType.SelectedIndex + 1;
            custID = Convert.ToInt32(txtBox_CustomerID.Text);
            custCountryCode = Convert.ToInt32(txtBox_CountryCode.Text);
            custPhoneNumber = Convert.ToInt32(txtBox_PhoneNumber.Text);
            custEmail = txtBox_CustomerEmail.Text;
            this.Close();
        }

        private void txtBox_CustomerEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEmailValid(txtBox_CustomerEmail.Text))
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        public bool IsEmailValid(string address)
        {
            return Regex.IsMatch(address, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            bCancel = true;
            this.Close();
        }
    }
}
