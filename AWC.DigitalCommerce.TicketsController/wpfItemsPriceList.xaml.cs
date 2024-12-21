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
    /// Interaction logic for wpfItemsPriceList.xaml
    /// </summary>
    public partial class wpfItemsPriceList : Window
    {
        List<clsItem> itemsPriceList = new List<clsItem>();
        public wpfItemsPriceList()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            itemsPriceList = DB.ListBinding_tbl_Items(0);
            ItemsPriceList.ItemsSource = itemsPriceList;

            PrintItemsList.IsEnabled = ItemsPriceList.Items.Count > 0;
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_PrintItemsPriceList(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(itemsPriceList);
        }
    }
}
