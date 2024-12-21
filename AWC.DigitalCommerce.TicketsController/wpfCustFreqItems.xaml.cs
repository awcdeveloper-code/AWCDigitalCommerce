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
    public partial class wpfCustFreqItems : Window
    {
        public clsCustFreqItem custFreqItem = null;
        public bool itemSelected = false;

        public wpfCustFreqItems(List<clsCustFreqItem> custFreqItemsList)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            lBox_FreqItems.ItemsSource = custFreqItemsList;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lBox_FreqItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            custFreqItem = (clsCustFreqItem)lBox_FreqItems.SelectedItem;
            itemSelected = true;
            this.Close();
        }
    }
}
