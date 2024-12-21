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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfSelectProducts : Window
    {
        public bool bOK = true;
        private bool clear = false;
        private clsCustomerVIP custProf = new clsCustomerVIP();
        private List<clsItem> lstProducts = new List<clsItem>();
        private bool bInventory = false;
        public List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        public List<clsTicketDetail> newBeveragesOrder = new List<clsTicketDetail>();
        public List<clsTicketDetail> SelectedProducts = new List<clsTicketDetail>();

        public wpfSelectProducts(clsCustomerVIP _custProf, List<clsItem> _lstProducts)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            custProf = _custProf;
            lstProducts = _lstProducts;
            lBox_Products.ItemsSource = lstProducts;
        }

        public wpfSelectProducts(List<clsItem> _lstProducts)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            lstProducts = _lstProducts;
            lBox_Products.ItemsSource = lstProducts;
            bInventory = true;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchProduct.Text.ToUpper();

            var empFiltered = from prod in lstProducts
                              let desc = prod.ItemDescription
                              where desc.StartsWith(txtOrig) || desc.Contains(txtOrig) || desc.EndsWith(txtOrig)
                              select prod;

            lBox_Products.ItemsSource = empFiltered;
        }

        private void txtSearchProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(4);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchProduct.Text = alphaKey.alphaKeyed;
            }
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
                Guid guidID = Guid.NewGuid();

                newItem.ItemID = item.ID;
                newItem.ItemType = item.ItemType;
                newItem.ItemSubType = item.ItemSubType;
                newItem.GUID = guidID.ToString();
                newItem.ItemDesc = item.ItemDescription;
                newItem.Qty = Convert.ToInt32(txtQty.Text);
                newItem.UnitCost = item.UnitCost;
                newItem.TotalCost = item.UnitCost * newItem.Qty;
                newItem.UnitPrice = item.UnitPrice;
                newItem.TotalPrice = item.UnitPrice * newItem.Qty;

                if (!bInventory)
                {
                    if (custProf.CustomerFOC)
                    {
                        newItem.UnitPrice = 0;
                        newItem.UnitCost = 0;
                        newItem.TotalPrice = 0;
                        newItem.TotalCost = 0;
                    }
                }

                if (DB.IsMealItemType(item.ItemDescription))
                {
                    this.Opacity = 0.5;
                    wpfMealNote mn = new wpfMealNote(item.ItemDescription);
                    mn.ShowDialog();
                    this.Opacity = 1;

                    newItem.Note = mn.mealNote;

                    clsTicketDetail newMealOrder = new clsTicketDetail();

                    newMealOrder.ID = newItem.ID;
                    newMealOrder.ItemID = newItem.ItemID;
                    newMealOrder.GUID = newItem.GUID;
                    newMealOrder.ItemDesc = newItem.ItemDesc;
                    newMealOrder.Qty = newItem.Qty;
                    newMealOrder.UnitCost = newItem.UnitCost;
                    newMealOrder.TotalCost = newItem.TotalCost;
                    newMealOrder.UnitPrice = newItem.UnitPrice;
                    newMealOrder.TotalPrice = newItem.TotalPrice;
                    newMealOrder.Note = newItem.Note;
                    newMealOrder.Bucket = false;

                    newMealsOrder.Add(newMealOrder);
                }
                else
                {
                    clsTicketDetail newBeverageOrder = new clsTicketDetail();

                    newBeverageOrder.ID = newItem.ID;
                    newBeverageOrder.ItemID = newItem.ItemID;
                    newBeverageOrder.ItemSubType = newItem.ItemSubType;
                    newBeverageOrder.GUID = newItem.GUID;
                    newBeverageOrder.ItemDesc = newItem.ItemDesc;
                    newBeverageOrder.Qty = newItem.Qty;
                    newBeverageOrder.UnitCost = newItem.UnitCost;
                    newBeverageOrder.TotalCost = newItem.TotalCost;
                    newBeverageOrder.UnitPrice = newItem.UnitPrice;
                    newBeverageOrder.TotalPrice = newItem.TotalPrice;
                    newBeverageOrder.Note = newItem.Note;
                    newBeverageOrder.Bucket = false;

                    // get bucket content
                    if (DB.GetItemSubtype(item.ItemDescription) == 2)
                    {
                        this.Opacity = 0.5;
                        wpfSelectBucketContent mn = new wpfSelectBucketContent(item.ID);
                        mn.ShowDialog();
                        this.Opacity = 1;
                        newBeverageOrder.Note = mn.bucketContent;
                        newBeverageOrder.Bucket = true;
                    }
                    newBeveragesOrder.Add(newBeverageOrder);
                }

                ProductsSelected.Items.Add(newItem);
                ProductsSelected.Items.Refresh();

                txtSearchProduct.Text = string.Empty;
                txtQty.Text = string.Empty;
                txtQty.IsEnabled = false;

                AddProduct.IsEnabled = false;
                btnOK.IsEnabled = true;
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
            try
            {
                clsTicketDetail item = ProductsSelected.SelectedItem as clsTicketDetail;

                if (DB.IsMealItemType(item.ItemDesc))
                {
                    clsTicketDetail meal = newMealsOrder.SingleOrDefault(x => x.GUID == item.GUID);

                    if (meal != null)
                    {
                        newMealsOrder.Remove(meal);
                    }
                }
                else
                {
                    clsTicketDetail beverage = newBeveragesOrder.SingleOrDefault(x => x.GUID == item.GUID);

                    if (beverage != null)
                    {
                        newBeveragesOrder.Remove(beverage);
                    }
                }

                ProductsSelected.Items.Remove(item);
                ProductsSelected.Items.Refresh();

                if (ProductsSelected.Items.Count == 0)
                {
                    btnDelete.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }

                clear = true;
                lBox_Products.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                wpfMessageBox.Show("Ticket Controller", $"ERROR: {ex.Message}", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, null);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            bOK = true;

            foreach (clsTicketDetail item in ProductsSelected.Items)
            {
                SelectedProducts.Add(item);
            }

            this.Close();
        }
    }
}
