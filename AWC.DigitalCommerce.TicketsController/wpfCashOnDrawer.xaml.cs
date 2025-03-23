using AWC.DigitalCommerce.TicketsController.Properties;
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
    /// Interaction logic for wpfCashOnDrawer.xaml
    /// </summary>
    public partial class wpfCashOnDrawer : Window
    {
        public int Cash = 0;
        public int CashWithdrawal = 0;
        public wpfCashOnDrawer(int _cash)
        {
            Cash = _cash;
            InitializeComponent();
            txtCash.Text = _cash.ToString();
            txtCashWithdrawal.Text = "0";
            txtCashOnDrawer.Text = "0";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_OK_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCashOnDrawer.Text = (Convert.ToInt32(txtCash.Text) - Convert.ToInt32(txtCashWithdrawal.Text)).ToString();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            CashWithdrawal = Convert.ToInt32(txtCashWithdrawal.Text);
            DB.InsertCashOnDrawer(Cash, CashWithdrawal);

            Settings.Default.CashRegisterOpening = Convert.ToInt32(txtCashOnDrawer.Text);
            Settings.Default.Save();
            Helper.ShowToastNotification($"Efectivo en caja actualizado");
            this.Close();
        }
    }
}
