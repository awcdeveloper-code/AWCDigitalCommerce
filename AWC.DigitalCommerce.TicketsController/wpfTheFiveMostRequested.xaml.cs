using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for wpfTheFiveMostRequested.xaml
    /// </summary>
    public partial class wpfTheFiveMostRequested : Window
    {
        public wpfTheFiveMostRequested()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            UpdateCharts();
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Refresh(object sender, RoutedEventArgs e)
        {
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            List<clsItemType> BeveragesList = DB.DataBinding_tbl_TicketsDetail(Settings.Default.BusinessDate, 1, Settings.Default.NumberOfMostRequestedItems);
            Beverages.ItemsSource = BeveragesList;

            List<clsItemType> LiquoursList = DB.DataBinding_tbl_TicketsDetail(Settings.Default.BusinessDate, 2, Settings.Default.NumberOfMostRequestedItems);
            Liquors.ItemsSource = LiquoursList;

            List<clsItemType> MealsList = DB.DataBinding_tbl_TicketsDetail(Settings.Default.BusinessDate, 3, Settings.Default.NumberOfMostRequestedItems);
            Meals.ItemsSource = MealsList;
        }
    }
}
