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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucVouchers.xaml
    /// </summary>
    public partial class ucVouchers : UserControl
    {
        public ucVouchers()
        {
            InitializeComponent();

            clsUser userProfile = DB.CheckUserPIN(Settings.Default.WhoOpen.ToString());
            txtIssuedBy.Text = userProfile.userName;
            txtVoucherAmount.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_AddVoucher(object sender, RoutedEventArgs e)
        {
            clsVoucher voucher = DB.InsertVoucher(Convert.ToInt32(txtVoucherAmount.Text));
            Helper.PrintVoucher(voucher);
            Helper.ShowToastNotification($"Voucher {voucher.ID} emitido");
            txtVoucherAmount.Text = string.Empty;
            txtVoucherAmount.Focus();
        }
    }
}
