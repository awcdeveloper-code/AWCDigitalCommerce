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
    /// <summary>
    /// Interaction logic for wpfSpecialItems.xaml
    /// </summary>
    public partial class wpfSpecialItems : Window
    {
        public int ItemID = 0;
        public string ItemDesc = string.Empty;
        private List<clsItem> lstSpecialItems = new List<clsItem>();

        public wpfSpecialItems()
        {
            this.Topmost = true;

            InitializeComponent();

            lstSpecialItems = DB.ListBinding_tbl_Items(9);      // Special Items
            cbox_SpecialItems.ItemsSource = lstSpecialItems;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            ItemID = 0;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (cbox_SpecialItems.SelectedIndex == -1) return;

            clsItem itemSelected = cbox_SpecialItems.SelectedItem as clsItem;
            ItemID = itemSelected.ID;
            ItemDesc = itemSelected.ItemDescription;
            this.Close();
        }
    }
}
