using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for wpfQuickOrder.xaml
    /// </summary>
    public partial class wpfQuickOrder : Window
    {
        private wpfMainWindow mw;
        private List<clsItem> lstProducts = new List<clsItem>();
        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();
        private int totalPrice = 0;
        private bool calledFromMainWindow = true;

        public wpfQuickOrder(wpfMainWindow _mw)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            mw = _mw;

            mw.QuickOrder.IsEnabled = false;

            InitializeComponent();

            this.KeyDown += new KeyEventHandler(wpfQuickOrder_KeyUp);

            lstProducts = DB.ListBinding_tbl_Items(0);  // Retrieve ALL
            lBox_Products.ItemsSource = lstProducts;

            Payment.Focus();
        }

        public wpfQuickOrder()
        {
            this.Topmost = true;

            calledFromMainWindow = false;

            InitializeComponent();

            if (Settings.Default.PrintClosedTicket)
                PrintClosedTicket.IsChecked = true;

            this.KeyDown += new KeyEventHandler(wpfQuickOrder_KeyUp);

            lstProducts = DB.ListBinding_tbl_Items(0);  // Retrieve ALL
            lBox_Products.ItemsSource = lstProducts;

            Payment.Focus();
        }
        private void wpfQuickOrder_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Quick Order was cancelleded.", Logger.Severity.INFORMATION);
                    
                    if (calledFromMainWindow) mw.QuickOrder.IsEnabled = true;

                    this.Close();
                    break;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchProduct.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from item in lstProducts
                              let ename = item.ItemDescription
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select item;

            lBox_Products.ItemsSource = empFiltered;
        }
        private void txtSearchProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(4);
                alphaKey.ShowDialog();
                txtSearchProduct.Text = alphaKey.alphaKeyed;
            }
        }
        private void lBox_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtQtyProduct.IsEnabled = true;
            txtQtyProduct.Text = "1";
            AddProduct.IsEnabled = true;
        }
        private void txtQtyProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQtyProduct.Text = numKey.numKeyed;
        }
        private void btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (txtQtyProduct.Text.Trim().Length == 0) return;

            int iQtyProduct = int.Parse(txtQtyProduct.Text.Trim(), NumberStyles.Integer);

            if (iQtyProduct == 0)
            {
                wpfMessageBox.Show("Ticket Controller", "CANTIDAD NO PUEDE SER CERO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            clsItem tmp = (clsItem)lBox_Products.SelectedItem;

            clsTicketDetail ntd = new clsTicketDetail();

            ntd.ItemDesc = tmp.ItemDescription;
            ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
            ntd.Qty = Convert.ToInt32(txtQtyProduct.Text.Trim());
            ntd.UnitCost = 0;
            ntd.TotalCost = 0;
            ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
            ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

            totalPrice += ntd.TotalPrice;

            // update the ticket
            itemsDetails.Add(ntd);
            TicketDetail.Items.Add(ntd);
            TicketDetail.Items.Refresh();
            Payment.IsEnabled = true;

            if (DB.IsMealItemType(tmp.ItemDescription))
            {
                List<clsTicketDetail> itemMeal = new List<clsTicketDetail>();
                itemMeal.Add(ntd);
                Helper.GetMealItemsFromTicket(Settings.Default.QuickOrderCustID, itemMeal);
            }

            lBox_Products.UnselectAll();
            txtQtyProduct.Text = string.Empty;
            txtQtyProduct.IsEnabled = false;
            txtSearchProduct.Text = string.Empty;
            AddProduct.IsEnabled = false;

            if (calledFromMainWindow) mw.QuickOrder.IsEnabled = false;
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            if (calledFromMainWindow) mw.QuickOrder.IsEnabled = true;
            this.Close();
        }
        private void btn_Payment(object sender, RoutedEventArgs e)
        {
            wpfPayMethod2 payForm = new wpfPayMethod2("-sp", totalPrice, 9999, true, 0);

            payForm.ShowDialog();

            if (payForm.payOK == false) return; // CANCEL

            foreach (clsTicketDetail itemdg in TicketDetail.Items)
            {
                clsItem item = new clsItem();
                item.ID = itemdg.ItemID;
                item.ItemSold = itemdg.Qty;
                DB.UpdateItemInventory("SAL", item);
            }

            Guid guidID = Guid.NewGuid();

            int ticketNumber = DB.CreateNextTicket(guidID.ToString(), Settings.Default.QuickOrderCustID);

            DB.UpdateTicketStatus(ticketNumber, 0, 0, 0, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher, Settings.Default.WhoOpen, ticket.CustomerID);

            DB.InsertTicketDetail(itemsDetails, guidID.ToString(), Settings.Default.WhoOpen, true);

            // print cancelled ticket
            if (Settings.Default.PrintClosedTicket)
            {
                ticket.ID = ticketNumber;
                ticket.CustomerID = DB.GetCustomerIDByID(Settings.Default.QuickOrderCustID);
                ticket.Cash = payForm.cash;
                ticket.CreditCard = payForm.creditCard;
                ticket.Transfer = payForm.transfer;
                ticket.Status = false;
                Helper.PrintTicket(ticket);
            }
            wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
            sw.ShowDialog();

            if (calledFromMainWindow) mw.QuickOrder.IsEnabled = true;

            this.Close();
        }
        private void chkBox_PrintClosedTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintClosedTicket == false)
                Settings.Default.PrintClosedTicket = true;
            else
                Settings.Default.PrintClosedTicket = false;

            Settings.Default.Save();
        }
    }
}
