using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
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
using System.Windows.Threading;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfBartenderOrdersMonitor.xaml
    /// </summary>
    public partial class wpfBartenderOrdersMonitor : Window
    {
        private DispatcherTimer bartenderOrder = new DispatcherTimer();
        
        public wpfBartenderOrdersMonitor()
        {
            InitializeComponent();

            bartenderOrder.Tick += new EventHandler(bartenderOrder_Tick);
            bartenderOrder.Interval = new TimeSpan(0, 0, Settings.Default.BartenderOrderTickInSeconds);
            bartenderOrder.Start();
        }
        
        private void bartenderOrder_Tick(object sender, EventArgs eArgs)
        {
            bartenderOrder.Stop();
            RetrieveOrders();
        }

        private void RetrieveOrders()
        {
            try
            {
                // first, get ticket to be printed
                clsPrintTicketRemotely ticketSource = DB.GetTicketToPrintRemotely();

                if (ticketSource.GUID.Length > 0)
                {
                    clsTicketsForDataGrid ticketTarget = Helper.LoadFromXMLString(ticketSource.TicketForDataGrid);

                    xPrinterTicket xPrintTck = new xPrinterTicket(ticketTarget);
                    xPrintTck.print();

                    DB.DeleteTicketPrintedRemotely(ticketSource.GUID);
                }

                // second, get beverages to display
                List<clsBartenderOrder> ordersList = DB.GetBartenderOrdersList();

                if (ordersList.Count > 0)
                {
                    bool firstTicket = true;

                    foreach(clsBartenderOrder order in ordersList)
                    {
                        if (order.GUID.Length > 0)
                        {
                            string[] bList = order.BeveragesList.ToString().Split('^');

                            if (firstTicket)
                            {
                                firstTicket = false;
                                lblLeftOrder.Content = order.CustomerID.Split('|')[0];
                                clsUser userProf = Helper.CheckUserProfile(order.CustomerID.Split('|')[1]);
                                lblLeftWaitrest.Content = userProf.userName;

                                int idx = 0;

                                foreach (string b in bList)
                                {
                                    idx++;

                                    if (string.IsNullOrEmpty(b.Split('|')[0]))
                                    {
                                        continue;
                                    }

                                    bool extraDesc = b.Split('|')[2].Length > 0;

                                    switch (idx)
                                    {
                                        case 1:
                                            lblLeftTicketItemNum1.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc1.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc2.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 2:
                                            lblLeftTicketItemNum2.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc2.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc3.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 3:
                                            lblLeftTicketItemNum3.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc3.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc4.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 4:
                                            lblLeftTicketItemNum4.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc4.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc5.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 5:
                                            lblLeftTicketItemNum5.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc5.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc6.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 6:
                                            lblLeftTicketItemNum6.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc6.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc7.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 7:
                                            lblLeftTicketItemNum7.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc7.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc8.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 8:
                                            lblLeftTicketItemNum8.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc8.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc9.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 9:
                                            lblLeftTicketItemNum9.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc9.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblLeftTicketItemDesc10.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 10:
                                            lblLeftTicketItemNum10.Content = b.Split('|')[0];
                                            lblLeftTicketItemDesc10.Content = b.Split('|')[1];
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                firstTicket = true;
                                lblRightOrder.Content = order.CustomerID.Split('|')[0];
                                clsUser userProf = Helper.CheckUserProfile(order.CustomerID.Split('|')[1]);
                                lblRightWaitrest.Content = userProf.userName;

                                int idx = 0;

                                foreach (string b in bList)
                                {
                                    idx++;


                                    if (string.IsNullOrEmpty(b.Split('|')[0]))
                                    {
                                        continue;
                                    }

                                    bool extraDesc = b.Split('|')[2].Length > 0;

                                    switch (idx)
                                    {
                                        case 1:
                                            lblRightTicketItemNum1.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc1.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc2.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 2:
                                            lblRightTicketItemNum2.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc2.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc3.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 3:
                                            lblRightTicketItemNum3.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc3.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc4.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 4:
                                            lblRightTicketItemNum4.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc4.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc5.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 5:
                                            lblRightTicketItemNum5.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc5.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc6.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 6:
                                            lblRightTicketItemNum6.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc6.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc7.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 7:
                                            lblRightTicketItemNum7.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc7.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc8.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 8:
                                            lblRightTicketItemNum8.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc8.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc9.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 9:
                                            lblRightTicketItemNum9.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc9.Content = b.Split('|')[1];
                                            if (extraDesc)
                                            {
                                                lblRightTicketItemDesc10.Content = b.Split('|')[2];
                                                idx++;
                                            }
                                            break;
                                        case 10:
                                            lblRightTicketItemNum10.Content = b.Split('|')[0];
                                            lblRightTicketItemDesc10.Content = b.Split('|')[1];
                                            break;
                                    }
                                }
                            }

                            DB.DeleteBartenderOrder(order.GUID);
                        }
                        Next.IsEnabled = true;
                    }
                }
                else
                {
                    Next.IsEnabled = false;
                    bartenderOrder.Start();
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"bartenderOrder_Tick ERROR: {ex.Message}", Logger.Severity.ERROR);
            }
        }

        private void CleanMonitor()
        {
            // HEADER
            lblLeftOrder.Content = string.Empty;
            lblRightOrder.Content = string.Empty;
            lblLeftWaitrest.Content = string.Empty;
            lblRightWaitrest.Content = string.Empty;

            // LEFT TICKET
            lblLeftTicketItemNum1.Content = string.Empty;
            lblLeftTicketItemDesc1.Content = string.Empty;
            lblLeftTicketItemNum2.Content = string.Empty;
            lblLeftTicketItemDesc2.Content = string.Empty;
            lblLeftTicketItemNum3.Content = string.Empty;
            lblLeftTicketItemDesc3.Content = string.Empty;
            lblLeftTicketItemNum4.Content = string.Empty;
            lblLeftTicketItemDesc4.Content = string.Empty;
            lblLeftTicketItemNum5.Content = string.Empty;
            lblLeftTicketItemDesc5.Content = string.Empty;
            lblLeftTicketItemNum6.Content = string.Empty;
            lblLeftTicketItemDesc6.Content = string.Empty;
            lblLeftTicketItemNum7.Content = string.Empty;
            lblLeftTicketItemDesc7.Content = string.Empty;
            lblLeftTicketItemNum8.Content = string.Empty;
            lblLeftTicketItemDesc8.Content = string.Empty;
            lblLeftTicketItemNum9.Content = string.Empty;
            lblLeftTicketItemDesc9.Content = string.Empty;
            lblLeftTicketItemNum10.Content = string.Empty;
            lblLeftTicketItemDesc10.Content = string.Empty;

            //RIGHT TICKET
            lblRightTicketItemNum1.Content = string.Empty;
            lblRightTicketItemDesc1.Content = string.Empty;
            lblRightTicketItemNum2.Content = string.Empty;
            lblRightTicketItemDesc2.Content = string.Empty;
            lblRightTicketItemNum3.Content = string.Empty;
            lblRightTicketItemDesc3.Content = string.Empty;
            lblRightTicketItemNum4.Content = string.Empty;
            lblRightTicketItemDesc4.Content = string.Empty;
            lblRightTicketItemNum5.Content = string.Empty;
            lblRightTicketItemDesc5.Content = string.Empty;
            lblRightTicketItemNum6.Content = string.Empty;
            lblRightTicketItemDesc6.Content = string.Empty;
            lblRightTicketItemNum7.Content = string.Empty;
            lblRightTicketItemDesc7.Content = string.Empty;
            lblRightTicketItemNum8.Content = string.Empty;
            lblRightTicketItemDesc8.Content = string.Empty;
            lblRightTicketItemNum9.Content = string.Empty;
            lblRightTicketItemDesc9.Content = string.Empty;
            lblRightTicketItemNum10.Content = string.Empty;
            lblRightTicketItemDesc10.Content = string.Empty;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void btn_Next(object sender, RoutedEventArgs e)
        {
            CleanMonitor();
            Next.IsEnabled = false;
            bartenderOrder.Start();
        }

        private void Window_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bartenderOrder.Stop();
        }
    }
}
