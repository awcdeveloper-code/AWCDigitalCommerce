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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfFixItemInventory.xaml
    /// </summary>
    public partial class wpfFixItemInventory : Window
    {
        public int itemSubtype = 0;
        public int itemParent = 0;
        public int itemParentUnit = 0;
        public int itemAvail = 0;
        public int itemSold = 0;
        public int itemDefective = 0;
        public int itemMinimum = 0;
        public int itemStock = 0;
        public bool bCancel = false;

        public wpfFixItemInventory(clsItem selectedItem)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            lblItemID.Content = selectedItem.ID.ToString();
            txtItemSubtype.Text = selectedItem.ItemSubType.ToString();
            txtItemParent.Text = selectedItem.ItemParent.ToString();
            txtItemParentUnit.Text = selectedItem.ItemParentUnit.ToString();
            txtItemAvailable.Text = selectedItem.ItemAvailable.ToString();
            txtItemSold.Text = selectedItem.ItemSold.ToString();
            txtItemDefective.Text = selectedItem.ItemDefective.ToString();
            txtItemMinimum.Text = selectedItem.ItemMinimum.ToString();
            txtItemStock.Text = selectedItem.ItemStock.ToString();
            txtItemSubtype.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            itemSubtype = txtItemSubtype.Text.Length > 0 ? Convert.ToInt32(txtItemSubtype.Text) : 0;
            itemParent = txtItemParent.Text.Length > 0 ? Convert.ToInt32(txtItemParent.Text) : 0;
            itemParentUnit = txtItemParentUnit.Text.Length > 0 ? Convert.ToInt32(txtItemParentUnit.Text) : 0;
            itemAvail = txtItemParent.Text.Length > 0 ? Convert.ToInt32(txtItemParent.Text) : 0;
            itemAvail = txtItemAvailable.Text.Length > 0 ? Convert.ToInt32(txtItemAvailable.Text) : 0;
            itemSold = txtItemSold.Text.Length > 0 ? Convert.ToInt32(txtItemSold.Text) : 0;
            itemDefective = txtItemDefective.Text.Length > 0 ? Convert.ToInt32(txtItemDefective.Text) : 0;
            itemMinimum = txtItemMinimum.Text.Length > 0 ? Convert.ToInt32(txtItemMinimum.Text) : 0;
            itemStock = txtItemStock.Text.Length > 0 ? Convert.ToInt32(txtItemStock.Text) : 0;
            this.Close();
        }

        private void btn_CNCL(object sender, RoutedEventArgs e)
        {
            bCancel = true;
            this.Close();
        }
    }
}
