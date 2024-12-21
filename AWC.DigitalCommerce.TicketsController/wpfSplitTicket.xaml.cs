using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSplitTicket.xaml
    /// </summary>
    public partial class wpfSplitTicket : Window
    {
        private bool applyServiceFee = false;
        private int totalPrice = 0;
        private int totalServiceFee= 0;
        private clsTicket ticket = new clsTicket();
        private List<clsItemDetailForDatagrid> ItemDetailForDatagrid = new List<clsItemDetailForDatagrid>();
        private List<clsTicketDetail> ticketDetailTarget = new List<clsTicketDetail>();

        public wpfSplitTicket(clsTicket _ticket, bool _applyServiceFee)
        {
            this.Topmost = true;

            ticket = _ticket;
            applyServiceFee = _applyServiceFee;

            InitializeComponent();

            lblTicketNumber.Content = ticket.ID.ToString("000000");
            lblCustomerName.Content = DB.GetCustomerIDByID(ticket.CustID);

            LoadSourceDataGrid();
        }

        private void LoadSourceDataGrid()
        {
            try
            {
                totalServiceFee = 0;
                lblServiceFee.Content = totalServiceFee.ToString("N0");

                totalPrice = 0;
                lblTotalPrice.Content = totalPrice.ToString("N0");

                TicketDetailSource.Items.Clear();
                TicketDetailTarget.Items.Clear();

                ItemDetailForDatagrid = DB.GetItemsByGUID(ticket.GUID, false);

                foreach (clsItemDetailForDatagrid data in ItemDetailForDatagrid)
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

                TicketDetailSource.Items.Refresh();

                Cancel.IsEnabled = false;
                Print.IsEnabled = false;
                Pay.IsEnabled = false;

                AddOne2Bottom.Visibility = Visibility.Hidden;
                AddOne2Top.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void TicketDetailSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TicketDetailSource.Items.Count > 1)
                AddOne2Bottom.Visibility = Visibility.Visible;
        }

        private void TicketDetailTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddOne2Top.Visibility = Visibility.Visible;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Add2Bottom(object sender, MouseButtonEventArgs e)
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

                foreach(clsTicketDetail item in items2Move)
                {
                    TicketDetailTarget.Items.Add(item);
                    TicketDetailSource.Items.Remove(item);

                    totalPrice += item.TotalPrice;
                    lblTotalPrice.Content = totalPrice.ToString("N0");

                    if (applyServiceFee)
                    {
                        totalServiceFee = totalPrice * 10 / 100;
                        lblServiceFee.Content = totalServiceFee.ToString("N0");
                    }
                }

                Cancel.IsEnabled = true;
                Print.IsEnabled = true;
                Pay.IsEnabled = true;

                AddOne2Bottom.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Add2Top(object sender, MouseButtonEventArgs e)
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

                    totalPrice += item.TotalPrice;
                    lblTotalPrice.Content = totalPrice.ToString("N0");

                    if (applyServiceFee)
                    {
                        totalServiceFee = totalPrice * 10 / 100;
                        lblServiceFee.Content = totalServiceFee.ToString("N0");
                    }
                }

                //clsTicketDetail rdi = TicketDetailTarget.SelectedItem as clsTicketDetail;

                //TicketDetailSource.Items.Add(rdi);
                //TicketDetailTarget.Items.Remove(rdi);

                //totalPrice -= rdi.TotalPrice;
                //lblTotalPrice.Content = totalPrice.ToString("N0");

                //if (applyServiceFee)
                //{
                //    totalServiceFee = totalPrice * 10 / 100;
                //    lblServiceFee.Content = totalServiceFee.ToString("N0");
                //}

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
                    Pay.IsEnabled = false;
                }

                AddOne2Top.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            LoadSourceDataGrid();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Print.IsEnabled = false;

            ticketDetailTarget.Clear();

            foreach (clsTicketDetail itemDetail in TicketDetailTarget.Items)
                ticketDetailTarget.Add(itemDetail);

            ticket.TotalPrice = ticketDetailTarget.Sum(x => x.TotalPrice);
            ticket.ServiceFee = totalServiceFee;
            ticket.TicketDate = DB.ConverTicketDate(Settings.Default.BusinessDate);

            Helper.PrintTicket(ticket, ticketDetailTarget, "");

            wpfSplashWindow sw = new wpfSplashWindow(2, "");
            sw.ShowDialog();

            Print.IsEnabled = true;
            Mouse.OverrideCursor = null;
        }

        private void btn_Pay(object sender, RoutedEventArgs e)
        {
            try
            {
                totalPrice += totalServiceFee;
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
                    // print voucher
                    ticket.Transfer = payForm.transfer;                    
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, DB.GetCustomerIDByID(Settings.Default.SplitTicketCustID)), 1);
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

                Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(newTicket, DB.GetCustomerIDByID(Settings.Default.SplitTicketCustID)));

                wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
                sw.ShowDialog();

                LoadSourceDataGrid();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
