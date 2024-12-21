using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfFrequentItems : Window
    {
        public string fip = string.Empty;
        public bool itemSelected = false;

        public wpfFrequentItems(List<string> prefixList)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            //this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            //this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
 
            lBox_FreqItems.ItemsSource = prefixList;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lBox_FreqItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fip = lBox_FreqItems.SelectedItem.ToString();
            itemSelected = true;
            this.Close();
        }
    }
}
