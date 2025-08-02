using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucCloseTicket.xaml
    /// </summary>
    public partial class ucCloseTicket : UserControl
    {
        private bool CleanTicketClicked = false;
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        private string customerID = string.Empty;
        private clsTicketsForDataGrid ticket;
        private int ticketNum = 0;
        private string guidTck = string.Empty;
        private int totalPrice = 0;
        private int totApplyServiceFee = 0;

        #region MESSAGES
        private wpfMainWindow wpfMW;
        private string lang = string.Empty;
        public string strCustomerIDNotFound = string.Empty;
        public string strPrintTicket = string.Empty;
        public string strRemoveItem = string.Empty;
        public string strCloseTicket = string.Empty;
        public string strNoRemoveMeal = string.Empty;
        public string strTicketUpdated = string.Empty;
        #endregion

        public ucCloseTicket(wpfMainWindow _wpfMW, string _lang)
        {
            wpfMW = _wpfMW;
            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            if (Settings.Default.AllowTicketSummary)
                PrintSummary.IsChecked = true;

            if (Settings.Default.PrintClosedTicket)
                PrintClosedTicket.IsChecked = true;

            lstVIP = DB.ListBinding_tbl_CustomerID(3, 1);
            lBox_CustomerID.ItemsSource = lstVIP;
        }
        private void CleanAll()
        {
            CleanTicketClicked = true;

            TicketNumber.Content = "000000";
            lBox_CustomerID.UnselectAll();
            TicketDetail.ItemsSource = string.Empty;
            TotalTicket.Content = string.Empty;
            ApplyServiceFee.IsChecked = false;
            SetUserAccessToResources();
            
            CleanTicketClicked = false;
        }
        private void SetUserAccessToResources()
        {
            RemoveItem.IsEnabled = Helper.CheckUserAccessToResource("RemoveItem");
            SplitTicket.IsEnabled = Helper.CheckUserAccessToResource("SplitTicket");
            SmallPayment.IsEnabled = Helper.CheckUserAccessToResource("ToApply");
            CancelTicket.IsEnabled = Helper.CheckUserAccessToResource("CancelTicket");
        }
        private void lBox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CleanTicketClicked)
            {
                clsCustomerVIP tmp = (clsCustomerVIP)lBox_CustomerID.SelectedItem;
                customerID = tmp.CustomerID;

                if (tmp.ApplyServiceFee)
                    ApplyServiceFee.IsChecked = true;
                else
                    ApplyServiceFee.IsChecked = false;

                ApplyServiceFee.IsEnabled = true;

                // fill datagrid with ticket details
                int ID = DB.GetIDByCustomerID(customerID);

                if (ID == 0)
                {
                    MessageBox.Show(string.Format(strCustomerIDNotFound, customerID), "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    // get ticket number
                    ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, ID);
                    TicketNumber.Content = ticketNum.ToString("000000");

                    // get GUID of the ticket
                    guidTck = DB.GetTicketGUID(ticketNum);

                    // get items of the ticket
                    if (Settings.Default.AllowTicketSummary)
                        itemdg = DB.GetItemsByGUID(guidTck, true);
                    else
                        itemdg = DB.GetItemsByGUID(guidTck, false);

                    TicketDetail.ItemsSource = itemdg;

                    TicketDetail.Items.Refresh();

                    if (TicketDetail.Items.Count == 0)
                    {
                        RemoveItem.IsEnabled = false;
                        SplitTicket.IsEnabled = false;
                    }
                    else
                    {
                        RemoveItem.IsEnabled = true;
                        SplitTicket.IsEnabled = true;
                    }

                    // get total price
                    totalPrice = itemdg.Sum(x => x.TotalPrice);
                    totApplyServiceFee = 0;

                    if (ApplyServiceFee.IsChecked == true)
                    {
                        Helper.AddServiceFee(totalPrice, itemdg, this);
                        totApplyServiceFee = totalPrice * 10 / 100;
                        totalPrice += totApplyServiceFee;
                    }

                    TotalTicket.Content = totalPrice.ToString("N0").PadLeft(7);

                    // for print purposes
                    ticket = new clsTicketsForDataGrid();
                    ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
                    ticket.ID = ticketNum;
                    ticket.CustomerID = customerID;
                    ticket.ServiceFee = totApplyServiceFee;
                    ticket.TotalPrice = totalPrice;
                    ticket.Status = DB.GetTicketStatus(ticketNum);
                }

                Clean.IsEnabled = true;
                CloseTicket.IsEnabled = true;
                PrintMeal.IsEnabled = false;
                SmallPayment.IsEnabled = true;

                // get Splited status
                if (!DB.GetTicketSplitedStatus(ticketNum) && TicketDetail.Items.Count > 1)
                    SplitTicket.IsEnabled = false;
                else
                    SplitTicket.IsEnabled = true;

                SetUserAccessToResources();
            }
        }
        private void btn_CleanTicket(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(ticket);
        }
        private void btn_CloseTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.RequestPIN)
            {
                wpfRequestPIN wpfPIN = new wpfRequestPIN();
                wpfPIN.ShowDialog();

                clsUser userProf = DB.CheckUserPIN(wpfPIN.numKeyed);

                if (userProf.userActive == false)
                {
                    MessageBox.Show(wpfMW.strPINdoNotExist, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
                Settings.Default.WhoOpenName = userProf.userName;
                Settings.Default.Save();
            }

            wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticket.ID, true,0);
            payForm.ShowDialog();
            if (payForm.payOK == false) return; // CANCEL

            // update inventory
            foreach (clsItemDetailForDatagrid itemdg in TicketDetail.Items)
            {
                clsItem item = new clsItem();
                item.ID = itemdg.ItemID;
                item.ItemSold = itemdg.Qty;
                DB.UpdateItemInventory("SAL", item);
            }

            if (payForm.transfer > 0)
            {
                // print voucher
                ticket.Transfer = payForm.transfer;
                Helper.PrintTicket(ticket, 1);
            }

            // update ticket
            DB.UpdateTicketStatus(Convert.ToInt32(TicketNumber.Content), 0, ticket.TotalPrice, ticket.ServiceFee, payForm.cash, payForm.creditCard, payForm.transfer, payForm.transfer, Settings.Default.WhoOpen, customerID);

            // update customer status
            DB.UpdateCustomerStatus(DB.GetIDByCustomerID(customerID), 0);

            // print cancelled ticket
            if (Settings.Default.PrintClosedTicket)
            {
                ticket.Cash = payForm.cash;
                ticket.CreditCard = payForm.creditCard;
                ticket.Transfer = payForm.transfer;
                ticket.Status = false;
                Helper.PrintTicket(ticket);
            }

            CleanTicketClicked = true;
            lstVIP = DB.ListBinding_tbl_CustomerID(3, 1);
            lBox_CustomerID.ItemsSource = lstVIP;
            CleanTicketClicked = false;

            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();

            CleanAll();
        }
        private void btn_SplitTicket(object sender, RoutedEventArgs e)
        {
            //wpfSplitTicket wpfSplitTck = new wpfSplitTicket();
            //wpfSplitTck.ShowDialog();
        }
        private void btn_RemoveItem(object sender, RoutedEventArgs e)
        {
            if (TicketDetail.SelectedItems.Count == 0) return;

            foreach (clsItemDetailForDatagrid row in TicketDetail.SelectedItems)
            {
                if (!Settings.Default.CanDeleteItemsFromTicket)
                {
                    if (DB.IsMealItemType(row.ItemDesc))
                    {
                        wpfMessageBox.Show("Tickets Controller", strNoRemoveMeal, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                        return;
                    }
                }
            }

            List<clsItemDetailForDatagrid> removeItems = new List<clsItemDetailForDatagrid>();

            int total2Reduce = 0;

            foreach (clsItemDetailForDatagrid row in TicketDetail.SelectedItems)
            {
                total2Reduce += row.TotalPrice;
                removeItems.Add(row);
                DB.UpdateTicketDetailRemoved(row.ID);
            }

            DB.UpdateTicketTotalPrice(ticketNum, total2Reduce);

            // get GUID of the ticket
            guidTck = DB.GetTicketGUID(ticketNum);

            // get items of the ticket
            List<clsItemDetailForDatagrid> itemdg = DB.GetItemsByGUID(guidTck, false);

            TicketDetail.ItemsSource = itemdg;
            TicketDetail.Items.Refresh();

            // enable or disable buttons
            if (TicketDetail.Items.Count == 1)
            {
                RemoveItem.IsEnabled = false;
                SplitTicket.IsEnabled = false;
            }
            else
            {
                RemoveItem.IsEnabled = true;
                SplitTicket.IsEnabled = true;
            }

            // get new total price
            int totalPrice = 0;
            foreach (clsItemDetailForDatagrid idg in itemdg)
                totalPrice += idg.TotalPrice;

            TotalTicket.Content = string.Format("{0:C0}", totalPrice);

            // for print purposes
            ticket = new clsTicketsForDataGrid();
            ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);
            ticket.ID = ticketNum;
            ticket.CustomerID = customerID;
            ticket.TotalPrice = totalPrice;
        }
        private void chkBox_PrintSummary_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.AllowTicketSummary == false)
                Settings.Default.AllowTicketSummary = true;
            else
                Settings.Default.AllowTicketSummary = false;

            Settings.Default.Save();
        }
        private void chkBox_PrintClosedTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintClosedTicket == false)
                Settings.Default.PrintClosedTicket = true;
            else
                Settings.Default.PrintClosedTicket = false;

            Settings.Default.Save();
        }
        private void ApplyServiceFee_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyServiceFee.IsChecked == true)
            {
                Helper.AddServiceFee(totalPrice, itemdg, this);

                totApplyServiceFee = totalPrice * 10 / 100;
                
                totalPrice += totApplyServiceFee;

                ticket.ServiceFee = totApplyServiceFee;

                ticket.TotalPrice = totalPrice;

                TotalTicket.Content = totalPrice.ToString("N0").PadLeft(7);
            }
            else
            {
                if (Settings.Default.AllowTicketSummary)
                    itemdg = DB.GetItemsByGUID(guidTck, true);
                else
                    itemdg = DB.GetItemsByGUID(guidTck, false);

                TicketDetail.ItemsSource = itemdg;
                
                TicketDetail.Items.Refresh();

                totalPrice = itemdg.Sum(x => x.TotalPrice);

                ticket.ServiceFee = 0;

                ticket.TotalPrice = totalPrice;

                TotalTicket.Content = totalPrice.ToString("N0").PadLeft(7);
            }
        }
        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsItemDetailForDatagrid row = TicketDetail.SelectedItem as clsItemDetailForDatagrid;

            if (row != null)
                PrintMeal.IsEnabled = DB.IsMealItemType(row.ItemDesc) ? true : false;
        }
        private void btn_PrintMeal(object sender, RoutedEventArgs e)
        {
            List<string> meal = new List<string>();

            clsItemDetailForDatagrid row = TicketDetail.SelectedItem as clsItemDetailForDatagrid;

            meal.Add(row.Qty.ToString() + "|" + row.ItemDesc + "|");

            Helper.PrintTicket(customerID, meal, true);
        }
        private void btn_SmallPayment(object sender, RoutedEventArgs e)
        {
            try
            {
                int cash = 0;
                int creditCard = 0;
                int transfer = 0;
                int paymentAmount = 0;

                // select concept
                wpfSpecialItems spec = new wpfSpecialItems();
                spec.ShowDialog();
                if (spec.ItemID == 0) return;

                // select payment method
                wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticketNum, false, 0);
                payForm.ShowDialog();

                if (payForm.payOK == false) return; // CANCEL

                cash = payForm.cash;
                creditCard = payForm.creditCard;
                transfer = payForm.transfer;
                paymentAmount = cash + creditCard + transfer;

                // prepare Payment record
                if (spec.ItemDesc.Contains("PAGO"))
                {
                    clsSmallPayment smlPay = new clsSmallPayment();

                    smlPay.RandomRef = Helper.RandomString(6);
                    smlPay.CustomerID = DB.GetCustomerID(customerID);
                    smlPay.TicketID = ticketNum;
                    smlPay.CurTotalPrice = ticket.TotalPrice;
                    smlPay.PaymentAmount = paymentAmount;
                    smlPay.Cash = cash;
                    smlPay.CreditCard = creditCard;
                    smlPay.Transfer = transfer;
                    smlPay.NewTotalPrice = ticket.TotalPrice - paymentAmount;
                    smlPay.WhoClosed = Settings.Default.WhoOpen;

                    DB.InsertPayment(smlPay);
                    Helper.PrintTicket(smlPay);
                }
                else
                {
                    // do nothing for discount or credit
                }

                // prepare Item record
                List<clsTicketDetail> smlPaymentList = new List<clsTicketDetail>();
                clsTicketDetail smlPayment = new clsTicketDetail();
                smlPayment.GUID = guidTck;
                smlPayment.ItemID = spec.ItemID;
                smlPayment.ItemDesc = spec.ItemDesc;
                smlPayment.Qty = 1;
                smlPayment.UnitPrice = paymentAmount * -1;
                smlPayment.UnitCost = 0;
                smlPayment.TotalPrice = smlPayment.UnitPrice;
                smlPayment.UnitCost = 0;
                smlPaymentList.Add(smlPayment);
                DB.InsertTicketDetail(smlPaymentList, guidTck, Settings.Default.WhoOpen, true);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();
                CleanAll();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void btn_CancelTicket(object sender, RoutedEventArgs e)
        {
            int custID = DB.GetIDByCustomerID(customerID);

            int tckNum = Convert.ToInt32(TicketNumber.Content);

            DB.DeleteTicketDetail(DB.GetTicketGUID(tckNum), false);

            DB.CancelTicket(tckNum, Settings.Default.WhoOpen, 2);

            DB.UpdateCustomerStatus(custID, 0);

            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();

            CleanTicketClicked = true;
            lstVIP = DB.ListBinding_tbl_CustomerID(3, 1);
            lBox_CustomerID.ItemsSource = lstVIP;
            CleanTicketClicked = false;

            CleanAll();
        }
    }
}