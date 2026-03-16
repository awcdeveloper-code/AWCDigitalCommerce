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
    public partial class ucDailyClosing_Liquors : UserControl
    {
        private string lang = string.Empty;
        private List<clsItemDetailForDatagrid> itemsList = new List<clsItemDetailForDatagrid>();
        public ucDailyClosing_Liquors(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            itemsList = DB.GetLiquorsItemsByDate(Settings.Default.BusinessDate);
            TicketDetail.ItemsSource = itemsList;
            int totalPrice = itemsList.Sum(x => x.TotalCost);
            lblTotalSale.Content = totalPrice.ToString("N0").PadLeft(7);

            if (itemsList.Count > 0)
                MealsSummary.IsEnabled = true;
        }
        private void btn_MealsSummary(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(itemsList, 3, null, null);
        }
    }
}
