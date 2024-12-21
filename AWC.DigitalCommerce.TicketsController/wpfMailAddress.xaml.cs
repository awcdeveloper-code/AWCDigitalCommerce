using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// Interaction logic for wpfMailAddress.xaml
    /// </summary>
    public partial class wpfMailAddress : Window
    {
        public bool bCancel = false;
        public bool restVoucher = false;
        public string mailAddress = string.Empty;
        public wpfMailAddress(string _mailAddress, bool voucherActive = true)
        {
            InitializeComponent();

            if (!voucherActive)
            {
                chkBox_RestVoucher.Visibility = Visibility.Hidden;
            }

            txtMailAddress.Text = _mailAddress;
            txtMailAddress.Focus();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            bCancel = true;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            restVoucher = chkBox_RestVoucher.IsChecked.Value;
            mailAddress = txtMailAddress.Text;
            this.Close();
        }

        private void txtMailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEmailValid(txtMailAddress.Text))
                btnOK.IsEnabled = true;
            else
                btnOK.IsEnabled = false;
        }

        public bool IsEmailValid(string address)
        {
            return Regex.IsMatch(address, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
    }
}
