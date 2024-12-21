using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;
using Microsoft.Win32;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucOldTickets.xaml
    /// </summary>
    public partial class ucOldTickets : UserControl
    {
        private bool CleanTicketClicked = false;
        private string customerID = string.Empty;

        #region MESSAGES
        private string lang = string.Empty;
        public string strTotalAmount = string.Empty;
        public string strPayAccount = string.Empty;
        public string sttNoOldTickets = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strMultiPayMethodNotAllowed = string.Empty;
        public string strAbortTicket = string.Empty;
        private int includeAllTickets = 1;
        #endregion

        public ucOldTickets(string _lang)
        {
            lang = _lang;

            InitializeComponent();
            Traductor.ApplyTranslation(this, lang);
            LoadOldTicketsDataGrid(includeAllTickets);
        }
        private void LoadOldTicketsDataGrid(int includeAllTickets)
        {
            try
            {
                CleanTicketClicked = true;

                Mouse.OverrideCursor = Cursors.Wait;

                //List<string> customerList = DB.GetCustomerListFromtDailyClosing();
                List<clsCustomerVIP> customerList = DB.GetCustomerListFromtDailyClosing2();
                lBox_CustomerID.ItemsSource = customerList;

                List<clsTicketsForDataGrid> totgral = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, includeAllTickets);
                OldOpenTickets.ItemsSource = totgral;

                lBox_CustomerID.SelectedIndex = -1;

                int totalPrice = totgral.Sum(x => x.TotalPrice);

                TotalOldTickets.Content = "TOTAL: " + string.Format("{0:C0}", totalPrice);

                InitializeButtons();

                Mouse.OverrideCursor = null;

                CleanTicketClicked = false;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void btn_PrintTicket(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            PrintTicket.IsEnabled = false;

            foreach (clsTicketsForDataGrid row in OldOpenTickets.SelectedItems)
            {
                row.Status = DB.GetTicketStatus(row.ID);

                clsTicket tck = DB.GetTicket(row.ID);

                if (ApplyServiceFee.IsChecked == true && tck.ApplyServiceFee == false)
                {
                    tck.ApplyServiceFee = true;
                    tck.ServiceFee = tck.TotalPrice * 10 / 100;
                    tck.TotalPrice += tck.ServiceFee;
                    DB.UpdateTicket(tck);
                }

                clsTicketsForDataGrid tmp = Helper.Convert2TicketsForDataGrid(tck, row.CustomerID);

                Helper.PrintTicket(tmp);
            }

            wpfSplashWindow sw = new wpfSplashWindow(2, "");
            sw.ShowDialog();

            PrintTicket.IsEnabled = true;
            Mouse.OverrideCursor = null;
        }
        private void btn_PayTicket(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Tickets Controller", strPayAccount, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                if (Settings.Default.RequestPIN)
                {
                    wpfRequestPIN wpfPIN = new wpfRequestPIN();
                    wpfPIN.ShowDialog();

                    clsUser userProf = DB.CheckUserPIN(wpfPIN.numKeyed);

                    if (userProf.userActive == false)
                    {
                        wpfMessageBox.Show("Tickets Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                        return;
                    }
                    Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
                    Settings.Default.Save();
                }

                bool firstTime = true;
                int payMethodSelection = 0;
                int cash = 0;
                int creditCard = 0;
                int transfer = 0;

                // for multiple cancellation
                if (OldOpenTickets.SelectedItems.Count > 1)
                {
                    wpfMessageBox.Show("Tickets Controller", strMultiPayMethodNotAllowed, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);

                    wpfOnePayMethodSelection opms = new wpfOnePayMethodSelection();
                    opms.ShowDialog();

                    if (opms.payMethod == 0) return;

                    payMethodSelection = opms.payMethod;
                    firstTime = false;
                }

                foreach (clsTicketsForDataGrid tck2pay in OldOpenTickets.SelectedItems)
                {
                    if (firstTime)
                    {
                        if (ApplyServiceFee.IsChecked == true && tck2pay.ApplyServiceFee == false)
                        {
                            tck2pay.ApplyServiceFee = true;
                            tck2pay.ServiceFee = tck2pay.TotalPrice * 10 / 100;
                            tck2pay.TotalPrice += tck2pay.ServiceFee;
                        }

                        // one-ticket cancellation
                        wpfPayMethod2 payForm = new wpfPayMethod2(lang, tck2pay.TotalPrice, tck2pay.ID, true, 0);
                        payForm.ShowDialog();

                        if (payForm.payOK == false) return; // CANCEL

                        payMethodSelection = 0;
                        cash = payForm.cash;
                        creditCard = payForm.creditCard;
                        transfer = payForm.transfer;
                        firstTime = false;
                    }

                    switch(payMethodSelection)
                    {
                        case 1:
                            cash = tck2pay.TotalPrice;
                            creditCard = 0;
                            transfer = 0;
                            break;
                        case 2:
                            cash = 0;
                            creditCard = tck2pay.TotalPrice;
                            transfer = 0;
                            break;
                        case 3:
                            cash = 0;
                            creditCard = 0;
                            transfer = tck2pay.TotalPrice;
                            break;
                    }

                    if (transfer > 0)
                    {
                        if (Settings.Default.PrintSINPETicket)
                        {
                            tck2pay.Transfer = transfer;
                            Helper.PrintTicket(tck2pay, 1);
                        }
                    }

                    // add ticket to tbl_TicketsOldCancelled
                    DB.InsertOldTicketCancelled(Settings.Default.BusinessDate, tck2pay.ID, tck2pay.TotalPrice);

                    // update ticket
                    DB.UpdateOldTicketStatus(tck2pay.ID, Settings.Default.BusinessDate, tck2pay.TotalPrice, cash, creditCard, transfer, Settings.Default.WhoOpen);

                    // delete ticket from tbl_DailyClosing table
                    DB.DeleteOldTicket(tck2pay.ID);

                    // print cancelled ticket
                    if (PrintClosedTicket.IsChecked == true)
                    {
                        tck2pay.Cash = cash;
                        tck2pay.CreditCard = creditCard;
                        tck2pay.Transfer = transfer;
                        tck2pay.Status = false;
                        Helper.PrintTicket(tck2pay);
                    }

                    wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                    sw.ShowDialog();
                }

                // update comboBox and dataGrid
                LoadOldTicketsDataGrid(includeAllTickets);
            }
        }
        private void btn_SmallPaymentTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                int cash = 0;
                int creditCard = 0;
                int transfer = 0;
                int paymentAmount = 0;

                clsTicketsForDataGrid tck2pay = OldOpenTickets.SelectedItem as clsTicketsForDataGrid;

                wpfPayMethod2 payForm = new wpfPayMethod2(lang, tck2pay.TotalPrice, tck2pay.ID, false, 0);
                payForm.ShowDialog();

                if (payForm.payOK == false) return; // CANCEL

                cash = payForm.cash;
                creditCard = payForm.creditCard;
                transfer = payForm.transfer;
                paymentAmount = cash + creditCard + transfer;

                if (paymentAmount > tck2pay.TotalPrice)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EL MONTO DEL ABONO NO PUEDE SER MAYOR AL TOTAL DE LA CUENTA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    return;
                }
                else
                if (paymentAmount == tck2pay.TotalPrice)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EL MONTO DEL ABONO NO PUEDE SER IGUAL AL TOTAL DE LA CUENTA. SI DESEA CANCELAR LA CUENTA, USE LA OPCIÓN 'PAGAR'.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                clsSmallPayment smlPay = new clsSmallPayment();
                smlPay.RandomRef = Helper.RandomString(6);
                smlPay.CustomerID = DB.GetCustomerID(customerID);
                smlPay.TicketID = tck2pay.ID;
                smlPay.CurTotalPrice = tck2pay.TotalPrice;
                smlPay.PaymentAmount = paymentAmount;
                smlPay.Cash = cash;
                smlPay.CreditCard = creditCard;
                smlPay.Transfer = transfer;
                smlPay.NewTotalPrice = tck2pay.TotalPrice - paymentAmount;
                smlPay.WhoClosed = Settings.Default.WhoOpen;

                // insert payment
                DB.InsertPayment(smlPay);

                // update ticket
                DB.UpdateOldTicketPayment(tck2pay.ID, paymentAmount, Settings.Default.WhoOpen);

                // print receipt
                Helper.PrintTicket(smlPay);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();

                // update comboBox and dataGrid
                LoadOldTicketsDataGrid(includeAllTickets);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void btn_AbortTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                clsUser userProf = new clsUser();

                this.Opacity = 0.5;
                wpfRequestPIN wpfPIN = new wpfRequestPIN();
                wpfPIN.ShowDialog();
                this.Opacity = 1;

                if (wpfPIN.numKeyed == "0")
                {
                    return;
                }

                userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

                if (!userProf.userPowerAdmin)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EL PIN INGRESADO NO TIENE PERMISO PARA ANULAR CUENTAS.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                if (wpfMessageBox.Show("Tickets Controller", strAbortTicket, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                {
                    wpfAbortReason ar = new wpfAbortReason();
                    ar.ShowDialog();

                    if (string.IsNullOrEmpty(ar.abortReason)) return;

                    foreach (clsTicketsForDataGrid row in OldOpenTickets.SelectedItems)
                    {
                        int custID = DB.GetIDByCustomerID(row.CustomerID);

                        int tckNum = Convert.ToInt32(row.ID);

                        DB.IncludeAbortReason(tckNum, ar.abortReason, Convert.ToInt32(userProf.userPIN));

                        DB.InsertNewTicketAborted(tckNum);

                        DB.CancelTicket(tckNum, Settings.Default.WhoOpen, 2);

                        DB.DeleteTicketDetail(DB.GetTicketGUID(tckNum), false);

                        DB.UpdateCustomerStatus(custID, 0);

                        DB.DeleteOldTicket(tckNum);
                    }

                    wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                    sw.ShowDialog();

                    // update comboBox and dataGrid
                    LoadOldTicketsDataGrid(includeAllTickets);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return;
            }
        }
        private void btn_AssignTicket(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfCustomerList custList = new wpfCustomerList();
            custList.ShowDialog();
            this.Opacity = 1;

            if (custList.customerID == 0) return;

            foreach (clsTicketsForDataGrid tck2reassign in OldOpenTickets.SelectedItems)
            {
                DB.ReassignCustomerID(tck2reassign.ID, custList.customerID);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();
            }

            // update comboBox and dataGrid
            LoadOldTicketsDataGrid(includeAllTickets);
        }
        private void lBox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CleanTicketClicked)
            {
                clsCustomerVIP custProfile = lBox_CustomerID.SelectedItem as clsCustomerVIP;

                // get items of the ticket
                List<clsTicketsForDataGrid> itemdg = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, includeAllTickets, DB.GetCustomerID(custProfile.CustomerID));

                OldOpenTickets.ItemsSource = itemdg;
                OldOpenTickets.Items.Refresh();

                // get total price
                int totalPrice = itemdg.Sum(x => x.TotalPrice);

                TotalOldTickets.Content = "TOTAL: " + string.Format("{0:C0}", totalPrice);

                OldOpenTickets.IsEnabled = true;
            }
        }
        private void lBox_CustomerID_GotFocus(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
        }
        private void OldOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int totalPrice = 0;

            PrintTicket.IsEnabled = true;
            AbortTicket.IsEnabled = true;
            SmallPaymentTicket.IsEnabled = true;
            AssignTicket.IsEnabled = true;
            PayTicket.IsEnabled = true;

            foreach (clsTicketsForDataGrid row in OldOpenTickets.SelectedItems)
            {
                totalPrice += row.TotalPrice;

                if (OldOpenTickets.SelectedItems.Count == 1)
                    ApplyServiceFee.IsChecked = row.ApplyServiceFee ? true : false;
            }

            TotalOldTickets.Content = "TOTAL: " + string.Format("{0:C0}", totalPrice);
        }
        private void OldOpenTickets_GotFocus(object sender, RoutedEventArgs e)
        {
            SetUserAccessToResources();
        }
        private void chkBox_PrintClosedTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintClosedTicket == false)
                Settings.Default.PrintClosedTicket = true;
            else
                Settings.Default.PrintClosedTicket = false;

            Settings.Default.Save();
        }
        private void chkBox_ApplyServiceFee(object sender, RoutedEventArgs e)
        {

        }
        private void InitializeButtons()
        {
            PrintTicket.IsEnabled = false;
            AbortTicket.IsEnabled = false;
            SmallPaymentTicket.IsEnabled = false;
            AssignTicket.IsEnabled = false;
            PayTicket.IsEnabled = false;
        }
        private void SetUserAccessToResources()
        {
            try
            {
                PrintTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucOldTickets_PrintTicket");
                AbortTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucOldTickets_AbortTicket");
                SmallPaymentTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucOldTickets_SmallPaymentTicket");
                PayTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucOldTickets_PayTicket");
                AssignTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucOldTickets_ReassignTicket");

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "SetUserAccessToResources2 validation PASSED successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return;
            }
        }
    }
}
