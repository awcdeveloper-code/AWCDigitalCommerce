using AWC.DigitalCommerce.TicketsController.Properties;
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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSelectBucketContent.xaml
    /// </summary>
    public partial class wpfSelectBucketContent : Window
    {
        public bool bOK = true;
        private bool clear = false;
        private List<clsItem> lstProducts = new List<clsItem>();
        public string bucketContent = string.Empty;
        public wpfSelectBucketContent(int bucketID)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            lBox_Products.ItemsSource = DB.GetBucketItemsList(bucketID);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void lBox_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!clear)
            {
                txtQty.Text = "1";
                txtQty.IsEnabled = true;
                AddProduct.IsEnabled = true;
            }
            else
            {
                clear = false;
            }
        }

        private void txtQty_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            this.Opacity = 1;
            txtQty.Text = numKey.numKeyed;
        }

        private void btn_AddProduct(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtQty.Text))
                {
                    txtQty.Text = "1";
                    return;
                }

                clsItem item = lBox_Products.SelectedItem as clsItem;

                if (item == null)
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: ANTES DE AGREGAR DEBE DE SELECCIONAR EL PRODUCTO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
                    return;
                }

                clsTicketDetail newItem = new clsTicketDetail();

                newItem.ItemID = item.ID;
                newItem.ItemDesc = item.ItemDescription;
                newItem.Qty = Convert.ToInt32(txtQty.Text);
                newItem.UnitCost = item.UnitCost;
                newItem.TotalCost = item.UnitCost * newItem.Qty;
                newItem.UnitPrice = item.UnitPrice;
                newItem.TotalPrice = item.UnitPrice * newItem.Qty;

                ProductsSelected.Items.Add(newItem);
                ProductsSelected.Items.Refresh();

                txtQty.Text = string.Empty;
                txtQty.IsEnabled = false;

                AddProduct.IsEnabled = false;

                int qty = 0;

                foreach (clsTicketDetail itemDetail in ProductsSelected.Items)
                    qty += itemDetail.Qty;

                btnOK.IsEnabled = qty >= Settings.Default.NumberOfItemsPerBucket ? true: false;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void ProductsSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.IsEnabled = true;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            bOK = false;
            this.Close();
        }

        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            clsTicketDetail item = ProductsSelected.SelectedItem as clsTicketDetail;

            ProductsSelected.Items.Remove(item);
            ProductsSelected.Items.Refresh();

            if (ProductsSelected.Items.Count == 0)
            {
                btnDelete.IsEnabled = false;
                btnOK.IsEnabled = false;
            }

            int qty = 0;

            foreach (clsTicketDetail itemDetail in ProductsSelected.Items)
                qty += itemDetail.Qty;

            btnOK.IsEnabled = qty == Settings.Default.NumberOfItemsPerBucket ? true : false;

            clear = true;
            lBox_Products.SelectedIndex = -1;
        }
        
        private void btn_OK(object sender, RoutedEventArgs e)
        {
            bOK = true;
            bucketContent = string.Empty;

            foreach (clsTicketDetail item in ProductsSelected.Items)
            {
                bucketContent += item.ItemID + "," + item.Qty + "$";
            }

            bucketContent = bucketContent.Substring(0, bucketContent.Length - 1);

            this.Close();
        }
    }
}
