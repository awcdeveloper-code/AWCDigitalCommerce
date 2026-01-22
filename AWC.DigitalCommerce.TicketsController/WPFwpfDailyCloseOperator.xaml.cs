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
    public partial class WPFwpfDailyCloseOperator : Window
    {
        private string businessDate;
        public WPFwpfDailyCloseOperator()
        {
            InitializeComponent();
        }

        private void calDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            lblCashSystem.Content = "0";
            lblCashOperator.Content = "0";
            lblCashDifference.Content = "0";

            lblCreditCardSystem.Content = "0";
            lblCreditCardOperator.Content = "0";
            lblCreditCardDifference.Content = "0";

            lblSINPESystem.Content = "0";
            lblSINPEOperator.Content = "0";
            lblSINPEDifference.Content = "0";

            lblVouchersSystem.Content = "0";
            lblVouchersOperator.Content = "0";
            lblVouchersDifference.Content = "0";

            businessDate = ((DateTime)calDate.SelectedDate).ToString("yyyyMMdd");
            Search.IsEnabled = true;
        }

        private void btn_Search(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
