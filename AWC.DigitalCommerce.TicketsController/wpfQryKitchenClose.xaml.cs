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
    /// Interaction logic for wpfQryConsumption.xaml
    /// </summary>
    public partial class wpfQryKitchenClose : Window
    {
        private List<clsItemDetailForDatagrid> itemsList = new List<clsItemDetailForDatagrid>();
        private string workDay = string.Empty;

        public wpfQryKitchenClose()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            itemsList = DB.GetMealsItemsByDate(Settings.Default.BusinessDate);
            TicketDetail.ItemsSource = itemsList;

            if (itemsList.Count > 0)
                MealsSummary.IsEnabled = true;
        }

        private void btn_MealsSummary(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(itemsList, 1, null, null);
            this.Close();
        }

        private void SelectedDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            workDay = SelectedDay.SelectedDate.ToString();

            if (workDay.Length == 0) return;

            string year = workDay.Split('/')[2].Substring(0, 4);
            string month = workDay.Split('/')[1].PadLeft(2, '0');
            string day = workDay.Split('/')[0].PadLeft(2, '0');

            workDay = year + month + day;

            itemsList = DB.GetMealsItemsByDate(workDay);

            TicketDetail.ItemsSource = itemsList;

            if (itemsList.Count > 0)
                MealsSummary.IsEnabled = true;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
