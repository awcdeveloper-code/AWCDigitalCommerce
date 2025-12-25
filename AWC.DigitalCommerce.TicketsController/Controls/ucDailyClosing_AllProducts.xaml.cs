using AWC.DigitalCommerce.TicketsController.Properties;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucDailyClosing_AllProducts : UserControl
    {
        private string lang = string.Empty;

        private List<clsItemDetailForDatagrid> itemsList;
        private string workDay = string.Empty;
        public ucDailyClosing_AllProducts(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            itemsList = DB.GetItemsByDate(Settings.Default.BusinessDate, Settings.Default.BusinessDate, 4);
            TicketDetail.ItemsSource = itemsList;
            int totalPrice = itemsList.Sum(x => x.TotalPrice);
            lblTotalSale.Content = totalPrice.ToString("N0").PadLeft(7);

            if (itemsList.Count > 0)
                AllProductsSummary.IsEnabled = true;
        }
        private void btn_MealsSummary(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(itemsList, 1, null, null);
        }

        private void btn_AllProductsSummary(object sender, RoutedEventArgs e)
        {

        }
    }
}