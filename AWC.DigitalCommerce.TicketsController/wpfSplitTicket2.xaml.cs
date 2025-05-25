using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSplitTicket2.xaml
    /// </summary>
    public partial class wpfSplitTicket2 : Window
    {
        #region GLOBAL VARIABLES
        private bool applyServiceFee = false;
        private bool applyIVAFee = false;
        private int subTotalPrice = 0;
        private int totalServiceFee = 0;
        private int totalIVAFee = 0;
        private int totalPrice = 0;
        private clsTicket ticket = new clsTicket();
        private List<clsTicketProform> proformsList = new List<clsTicketProform>();
        private List<clsItemDetailForDatagrid> ItemDetailForDatagrid = new List<clsItemDetailForDatagrid>();
        private List<clsTicketDetail> ticketDetailTarget = new List<clsTicketDetail>();
        private string customerAKA = string.Empty;
        #endregion

        public wpfSplitTicket2(clsTicket _ticket, bool _applyServiceFee, bool _applyIVAFee)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            ticket = _ticket;

            applyServiceFee = _applyServiceFee;
            applyIVAFee = _applyIVAFee;

            InitializeComponent();

            lblTicketNumber.Content = ticket.ID.ToString("000000");

            PrintClosedTicket.IsChecked = Settings.Default.PrintClosedTicket;

            LoadSourceDataGrid();
            CheckTicketProforms(ticket.ID, string.Empty);
        }

        private void LoadSourceDataGrid()
        {
            try
            {
                subTotalPrice = 0;
                lblSubTotalPrice.Content = subTotalPrice.ToString("N0");

                totalServiceFee = 0;
                lblServiceFee.Content = totalServiceFee.ToString("N0");

                totalPrice = 0;
                lblTotalPrice.Content = totalPrice.ToString("N0");

                TicketDetailSource.Items.Clear();
                TicketDetailTarget.Items.Clear();

                ItemDetailForDatagrid = DB.GetItemsByGUID(ticket.GUID, false);

                foreach (clsItemDetailForDatagrid data in ItemDetailForDatagrid)
                {
                    clsTicketProform ticketProformItem = DB.GetTicketProformByTicketDetailID(data.ID);

                    if (ticketProformItem.TicketDetailID == 0)
                    {
                        clsTicketDetail rdi = new clsTicketDetail();

                        rdi.ID = data.ID;
                        rdi.ItemID = data.ItemID;
                        rdi.GUID = ticket.GUID;
                        rdi.ItemDesc = data.ItemDesc;
                        rdi.Qty = data.Qty;
                        rdi.UnitCost = data.UnitCost;
                        rdi.TotalCost = data.TotalCost;
                        rdi.UnitPrice = data.UnitPrice;
                        rdi.TotalPrice = data.TotalPrice;

                        TicketDetailSource.Items.Add(rdi);
                    }
                }

                TicketDetailSource.Items.Refresh();

                Cancel.IsEnabled = false;
                Print.IsEnabled = false;
                NewTicket.IsEnabled = false;
                Pay.IsEnabled = false;

                AddOne2Right.Visibility = Visibility.Hidden;
                AddOne2Left.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void LoadTargetDataGridWithProform(List<clsTicketProform> proformsList)
        {
            try
            {
                TicketDetailSource.Items.Clear();

                foreach (clsTicketProform data in proformsList)
                {
                    customerAKA = data.CustomerAKA;

                    clsTicketDetail rdi = new clsTicketDetail();

                    rdi = DB.GetSingleTickeDetail(data.TicketDetailID);
                    subTotalPrice += rdi.TotalPrice;
                    lblSubTotalPrice.Content = subTotalPrice.ToString("N0");
                    TicketDetailTarget.Items.Add(rdi);

                    clsTicketDetail tdg = new clsTicketDetail();

                    tdg.ID = rdi.ID;
                    tdg.ItemID = rdi.ItemID;
                    tdg.GUID = rdi.GUID;
                    tdg.ItemDesc = rdi.ItemDesc;
                    tdg.Qty = rdi.Qty;
                    tdg.UnitCost = rdi.UnitCost;
                    tdg.TotalCost = rdi.TotalCost;
                    tdg.UnitPrice = rdi.UnitPrice;
                    tdg.TotalPrice = rdi.TotalPrice;
                    TicketDetailSource.Items.Add(rdi);
                }

                TicketDetailSource.Items.Refresh();

                totalServiceFee = 0;

                if (applyServiceFee)
                {
                    totalServiceFee = subTotalPrice * 10 / 100;
                    lblServiceFee.Content = totalServiceFee.ToString("N0");
                }

                totalIVAFee = 0;

                if (applyIVAFee)
                {
                    totalIVAFee = subTotalPrice * 13 / 100;
                    lblIVAFee.Content = totalIVAFee.ToString("N0");
                }

                totalPrice = subTotalPrice + totalServiceFee + totalIVAFee;
                lblTotalPrice.Content = totalPrice.ToString("N0");

                TicketDetailTarget.Items.Refresh();

                Cancel.IsEnabled = true;
                Print.IsEnabled = true;
                NewTicket.IsEnabled = true;
                Pay.IsEnabled = true;

                AddOne2Right.Visibility = Visibility.Hidden;
                AddOne2Left.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void CheckTicketProforms(int tn, string custAKA)
        {
            proformsList = DB.CheckTicketProforms(tn, custAKA);
            Proforms.IsEnabled = proformsList.Count > 0 ? true : false; 
        }

        private void TicketDetailSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TicketDetailSource.Items.Count > 1)
                AddOne2Right.Visibility = Visibility.Visible;
        }

        private void TicketDetailTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddOne2Left.Visibility = Visibility.Visible;
        }

        private void btn_Add2Right(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<clsTicketDetail> items2Move = new List<clsTicketDetail>();

                foreach (clsTicketDetail item in TicketDetailSource.SelectedItems)
                {
                    items2Move.Add(item);
                }

                if (TicketDetailSource.Items.Count == items2Move.Count)
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: NO SE PERMITE SELECCIONAR TODOS LOS ÍTEMES.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                    TicketDetailSource.UnselectAll();
                    return;
                }

                foreach (clsTicketDetail item in items2Move)
                {
                    TicketDetailTarget.Items.Add(item);
                    TicketDetailSource.Items.Remove(item);

                    subTotalPrice += item.TotalPrice;
                    lblSubTotalPrice.Content = subTotalPrice.ToString("N0");
                }

                totalServiceFee = 0;

                if (applyServiceFee)
                {
                    totalServiceFee = subTotalPrice * 10 / 100;
                    lblServiceFee.Content = totalServiceFee.ToString("N0");
                }

                totalIVAFee = 0;

                if (applyIVAFee)
                {
                    totalIVAFee = subTotalPrice * 13 / 100;
                    lblIVAFee.Content = totalIVAFee.ToString("N0");
                }

                totalPrice = subTotalPrice + totalServiceFee + totalIVAFee;
                lblTotalPrice.Content = totalPrice.ToString("N0");

                Cancel.IsEnabled = true;
                Print.IsEnabled = true;
                NewTicket.IsEnabled = true;
                Pay.IsEnabled = true;

                AddOne2Right.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Add2Left(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<clsTicketDetail> items2Move = new List<clsTicketDetail>();

                foreach (clsTicketDetail item in TicketDetailTarget.SelectedItems)
                {
                    items2Move.Add(item);
                }

                foreach (clsTicketDetail item in items2Move)
                {
                    TicketDetailSource.Items.Add(item);
                    TicketDetailTarget.Items.Remove(item);

                    subTotalPrice -= item.TotalPrice;
                    lblSubTotalPrice.Content = subTotalPrice.ToString("N0");
                }

                totalServiceFee = 0;

                if (applyServiceFee)
                {
                    totalServiceFee = subTotalPrice * 10 / 100;
                    lblServiceFee.Content = totalServiceFee.ToString("N0");
                }

                totalPrice = subTotalPrice + totalServiceFee;
                lblTotalPrice.Content = totalPrice.ToString("N0");

                if (TicketDetailTarget.Items.Count > 0)
                {
                    Print.IsEnabled = true;
                    Cancel.IsEnabled = true;
                    Pay.IsEnabled = true;
                }
                else
                {
                    Print.IsEnabled = false;
                    Cancel.IsEnabled = false;
                    NewTicket.IsEnabled = false;
                    Pay.IsEnabled = false;
                }

                AddOne2Left.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            LoadSourceDataGrid();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            wpfUseNickName unn = new wpfUseNickName(string.Empty, false);

            if (wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: DESEA GUARDAR LA PRE-FACTURA (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, null) == MessageBoxResult.Yes)
            {
                this.Opacity = 0.5;
                unn.ShowDialog();
                this.Opacity = 1;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            Print.IsEnabled = false;

            ticketDetailTarget.Clear();

            foreach (clsTicketDetail itemDetail in TicketDetailTarget.Items)
            {
                ticketDetailTarget.Add(itemDetail);

                if (unn.nickName.Length > 0)
                {
                    DB.InsertTicketProform(ticket.ID, itemDetail.ID, unn.nickName, itemDetail.ItemID, itemDetail.Qty);
                }
            }

            ticket.TotalPrice = totalPrice;
            ticket.ServiceFee = totalServiceFee;
            ticket.IVAFee = totalIVAFee;
            ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);

            Helper.PrintTicket(ticket, ticketDetailTarget, unn.nickName);

            wpfSplashWindow sw = new wpfSplashWindow(2, "");
            sw.ShowDialog();

            CheckTicketProforms(ticket.ID, string.Empty);
            Print.IsEnabled = true;
            Mouse.OverrideCursor = null;
        }

        private void btn_Pay(object sender, RoutedEventArgs e)
        {
            try
            {
                wpfPayMethod2 payForm = new wpfPayMethod2("-sp", totalPrice, ticket.ID, true, 0);
                payForm.ShowDialog();

                if (payForm.payOK == false) return; // CANCEL

                foreach (clsTicketDetail itemdg in TicketDetailTarget.Items)
                {
                    clsItem item = new clsItem();

                    item.ID = itemdg.ItemID;
                    item.ItemSold = itemdg.Qty;

                    DB.UpdateItemInventory("SAL", item);
                }

                if (payForm.transfer > 0)
                {
                    if (Settings.Default.PrintSINPETicket)
                    {
                        // print voucher
                        ticket.Transfer = payForm.transfer;
                        Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, DB.GetCustomerIDByID(Settings.Default.SplitTicketCustID)), 1);
                    }
                }

                Guid guidID = Guid.NewGuid();

                int ticketNumber = DB.CreateNextTicket(guidID.ToString(), Settings.Default.SplitTicketCustID);

                DB.UpdateTicketStatus(ticketNumber, 0, totalPrice, totalServiceFee, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher,
                                      Settings.Default.WhoOpen, DB.GetCustomerIDByID(Settings.Default.SplitTicketCustID));

                ticketDetailTarget.Clear();

                foreach (clsTicketDetail itemDetail in TicketDetailTarget.Items)
                {
                    ticketDetailTarget.Add(itemDetail);
                    DB.DeleteSplitTicketDetail(itemDetail, true);
                }

                DB.InsertTicketDetail(ticketDetailTarget, guidID.ToString(), Settings.Default.WhoOpen, false);

                clsTicket newTicket = DB.GetTicket(ticketNumber);

                if (PrintClosedTicket.IsChecked == true)
                {
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(newTicket, DB.GetCustomerIDByID(Settings.Default.SplitTicketCustID)));
                }

                if (customerAKA.Length > 0)
                {
                    DB.DeleteTicketProform(ticket.ID, customerAKA);
                    customerAKA = string.Empty;
                }

                wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
                sw.ShowDialog();

                LoadSourceDataGrid();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_NewTicket(object sender, RoutedEventArgs e)
        {
            Opacity = 0.5;
            wpfSplitTicketCustomerSelection selecCust = new wpfSplitTicketCustomerSelection();
            selecCust.ShowDialog();
            Opacity = 1;

            if (selecCust.isOK)
            {
                Guid guidID = Guid.NewGuid();

                int ticketNumber = DB.CreateNextTicket(guidID.ToString(), selecCust.custProfile.ID);

                DB.UpdateTicketStatusForSplitTicket(ticketNumber, totalPrice, totalServiceFee, DB.GetCustomerIDByID(selecCust.custProfile.ID));

                ticketDetailTarget.Clear();

                foreach (clsTicketDetail itemDetail in TicketDetailTarget.Items)
                {
                    ticketDetailTarget.Add(itemDetail);
                    DB.DeleteSplitTicketDetail(itemDetail, true);
                }

                DB.InsertTicketDetail(ticketDetailTarget, guidID.ToString(), Settings.Default.WhoOpen, false);
                DB.InsertNewOpenTicket(selecCust.custProfile);
                DB.UpdateCustomerStatus(selecCust.custProfile.ID, 1);

                wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
                sw.ShowDialog();

                LoadSourceDataGrid();
            }
        }

        private void btn_Proforms(object sender, RoutedEventArgs e)
        {
            List<string> customersAKA = DB.GetTicketProformsCustomerAKAList(ticket.ID);
            wpfSelectCustomer custAKA = new wpfSelectCustomer(ticket.ID, customersAKA);
            custAKA.ShowDialog();

            if (custAKA.customerAKA.Length > 0)
            {
                CheckTicketProforms(ticket.ID, custAKA.customerAKA);
                LoadTargetDataGridWithProform(proformsList);
                DB.DeleteTicketProform(ticket.ID, custAKA.customerAKA);
            }
            else
            {
                CheckTicketProforms(ticket.ID, string.Empty);
            }
        }

        private void chkBox_PrintClosedTicket(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintClosedTicket == false)
                Settings.Default.PrintClosedTicket = true;
            else
                Settings.Default.PrintClosedTicket = false;

            Settings.Default.Save();
        }

        private void btn_PrintAsIs(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                int pai_subTotalPrice = 0;

                foreach (clsTicketDetail item in TicketDetailSource.Items)
                {
                    pai_subTotalPrice += item.TotalPrice;
                }

                int pai_totalServiceFee = 0;

                if (applyServiceFee)
                {
                    pai_totalServiceFee = pai_subTotalPrice * 10 / 100;
                }

                int pai_totalIVAFee = 0;

                if (applyIVAFee)
                {
                    pai_totalIVAFee = pai_subTotalPrice * 13 / 100;
                }

                int pai_totalPrice = pai_subTotalPrice + pai_totalServiceFee + pai_totalIVAFee;

                wpfSplashWindow sw = new wpfSplashWindow(2, "");
                sw.ShowDialog();

                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
