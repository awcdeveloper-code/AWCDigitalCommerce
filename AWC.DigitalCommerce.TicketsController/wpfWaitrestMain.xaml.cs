using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Security.Policy;
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
    public partial class wpfWaitrestMain : Window
    {
        // WORK VARIABLES
        private string fullLogPath = string.Empty;
        private string fullLogFileName = string.Empty;
        private string origGUID = string.Empty;
        private string targGUID = string.Empty;

        private int totalPrice = 0;
        private bool applyFeeService = false;
        private clsUser userProf = new clsUser();
        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private clsTicket ticket = new clsTicket();
        private List<clsCustomerVIP> lstCustomers = new List<clsCustomerVIP>();
        private List<clsItem> lstProducts = new List<clsItem>();
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        private List<clsTicketDetail> itemsDetail = new List<clsTicketDetail>();
        private List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        private List<clsTicketDetail> newBeveragesOrder = new List<clsTicketDetail>();
        private Dictionary<int, int> itemsIDList = new Dictionary<int, int>();

        public wpfWaitrestMain()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(this_KeyDown);

            lstProducts = DB.ListBinding_tbl_Items(0);

            OpenTicket.Focus();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F12:
                    this.WindowState = (this.WindowState == WindowState.Minimized) ? WindowState.Maximized : WindowState.Minimized;
                    break;
                case Key.Escape:
                    lblTicketNumber.Content = "CUENTA:";
                    lblTotalAmount.Content = "TOTAL: 0.00 ";

                    TicketDetail.Items.Clear();
                    
                    lBox_Customers.SelectedIndex = -1;
                    lBox_Customers.IsEnabled = true;
                    lBox_Customers.Focus();
                    break;
                case Key.System:
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Tickets Controller Waitrest stopped by request of the user.", Logger.Severity.INFORMATION);
                    this.Close();
                    break;
            }
        }

        private void wpfWaitrestMain_ContentRendered(object sender, EventArgs e)
        {
            //check if dailyClosing must be done

            clsCustomerVIP awcDC = DB.GetCustomerProfile(Settings.Default.DBMasterKey);

            string bussinessDay = DB.ConverTicketDate(awcDC.LastPayment);
            string today = DB.ConverTicketDate(DateTime.Now.ToString("yyyyMMdd"));

            if (today != bussinessDay)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: No puede abrir este módulo debido a que no se ha realizado el cierre contable del día anterior.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, null);
                App.Current.Shutdown();
                return;
            }

            this.Opacity = 0.5;
            wpfRequestPIN wpfPIN = new wpfRequestPIN();
            wpfPIN.ShowDialog();
            this.Opacity = 1;

            if (wpfPIN.numKeyed == "0")
            {
                App.Current.Shutdown();
                return;
            }

            userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

            if (userProf.userActive == false)
            {
                wpfMessageBox.Show("Tickets Controller", "ERROR: El PIN ingresado es inválido, su acceso no está autorizado.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, null);
                App.Current.Shutdown();
                return;
            }

            Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
            Settings.Default.BusinessDate = DateTime.Now.ToString("yyyyMMdd");
            Settings.Default.Save();

            RefreshOpenTicketsListBox();

            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Tickets Controller Waitrest initialized successfully.", Logger.Severity.INFORMATION);
        }

        private void txtSearchCustomer_GotFocus(object sender, RoutedEventArgs e)
        {
            TicketDetail.Items.Clear();

            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchCustomer.Text = alphaKey.alphaKeyed;
            }
        }

        private void txtSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchCustomer.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from cust in lstCustomers
                              let ename = cust.CustomerID
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select cust;

            lBox_Customers.ItemsSource = empFiltered;
        }

        private void lBox_Customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;

                if (lBox_Customers.SelectedIndex == -1)
                    return;

                custProfile = lBox_Customers.SelectedItem as clsCustomerVIP;

                UpdateTicketDetailDataGrid(custProfile);

                PayTicket.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsTicketDetail rdi = TicketDetail.SelectedItem as clsTicketDetail;

            if (rdi == null) return;

            Increase.Visibility = Visibility.Visible;
            Decrease.Visibility = Visibility.Visible;
        }

        private void btn_OpenTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Opacity = 0.5;
                wpfNewTicket wpfNT = new wpfNewTicket("-sp");
                wpfNT.ShowDialog();
                this.Opacity = 1;

                if (wpfNT.newTicket)
                {
                    RefreshOpenTicketsListBox();
                }

                lBox_Customers.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btn_AddProducts(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfSelectProducts prodsel = new wpfSelectProducts(custProfile, lstProducts);
            prodsel.ShowDialog();
            this.Opacity = 1;

            if (!prodsel.bOK) return;

            foreach (clsTicketDetail item in prodsel.SelectedProducts)
            {
                TicketDetail.Items.Add(item);
                totalPrice = TotalizeTicket(TicketDetail);
                ticket.TotalPrice = totalPrice;
            }

            newMealsOrder.Clear();
            newBeveragesOrder.Clear();

            if (prodsel.newMealsOrder.Count > 0)
            {
                foreach (clsTicketDetail newMeal in prodsel.newMealsOrder)
                {
                    newMealsOrder.Add(newMeal);
                }
            }

            if (prodsel.newBeveragesOrder.Count > 0)
            {
                foreach (clsTicketDetail newBeverage in prodsel.newBeveragesOrder)
                {
                    newBeveragesOrder.Add(newBeverage);
                }
            }

            TicketDetail.Items.Refresh();

            UpdateTicket();
        }

        private void btn_Increase(object sender, MouseButtonEventArgs e)
        {
            Increase.IsEnabled = false;

            try
            {
                foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
                {
                    rdi.Qty++;
                    rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;

                    if (DB.IsMealItemType(rdi.ItemDesc))
                    {
                        wpfMealNote mn = new wpfMealNote(rdi.ItemDesc);
                        mn.ShowDialog();
                        rdi.Note = mn.mealNote;

                        clsTicketDetail newMealOrder = new clsTicketDetail();

                        newMealOrder.Qty = 1;
                        newMealOrder.ItemID = rdi.ItemID;
                        newMealOrder.ItemDesc = rdi.ItemDesc;
                        newMealOrder.Note = rdi.Note;

                        newMealsOrder.Add(newMealOrder);
                    }
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);

                TicketDetail.Items.Refresh();

                UpdateTicket();

                Increase.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btn_Decrease(object sender, MouseButtonEventArgs e)
        {
            Decrease.IsEnabled = false;

            try
            {
                foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
                {
                    if (rdi.Qty == 1)
                    {
                        continue;
                    }
                    else
                    {
                        rdi.Qty--;
                        rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;
                    }

                    if (DB.IsMealItemType(rdi.ItemDesc))
                    {
                        try
                        {
                            var itemToRemove = newMealsOrder.Single(r => r.ItemID == rdi.ItemID);
                            newMealsOrder.Remove(itemToRemove);
                        }
                        catch { }
                    }
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);

                TicketDetail.Items.Refresh();

                UpdateTicket();

                Decrease.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btn_PayTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Opacity = 0.5;

                wpfPayMethod2 payForm = new wpfPayMethod2("-sp", ticket.TotalPrice, ticket.ID, true, 0);
                payForm.ShowDialog();

                if (payForm.printTicket)
                {
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));
                    wpfSplashWindow ptck = new wpfSplashWindow(1, "");
                    ptck.ShowDialog();
                    this.Opacity = 1;
                    return;
                }

                if (!payForm.payOK)
                {
                    this.Opacity = 1;
                    return;
                }

                // update inventory
                foreach (clsTicketDetail idg in TicketDetail.Items)
                {
                    if (idg.ItemID > 100000)
                    {
                        // old ticket added to the current ticket
                        DB.UpdateTicketStatus(idg.ItemID - 100000, 0, idg.TotalPrice, 0, idg.TotalPrice, 0, 0, 0, Settings.Default.WhoOpen, custProfile.CustomerID);
                    }

                    clsItem item = new clsItem();
                    item.ID = idg.ItemID;
                    item.ItemSold = idg.Qty;
                    DB.UpdateItemInventory("SAL", item);
                }

                // update ticket
                DB.UpdateTicketStatus(ticket.ID, 0, ticket.TotalPrice, ticket.ServiceFee, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher,
                                      Settings.Default.WhoOpen, custProfile.CustomerID);

                // update customer status
                DB.UpdateCustomerStatus(custProfile.ID, 0);

                // delete Customer from OpenTickets
                DB.DeleteOpenTickets(custProfile.ID);

                if (payForm.transfer > 0)
                {
                    // print voucher
                    ticket.Transfer = payForm.transfer;
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID), 1);
                }

                if (payForm.equalParts > 1)
                {
                    int eachPerson = ticket.TotalPrice / payForm.equalParts;

                    // generates FoodService voucher
                    for (int i = 0; i < payForm.equalParts; i++)
                    {
                        clsTicketsForDataGrid tck4dg = new clsTicketsForDataGrid();

                        tck4dg.TicketDate = Settings.Default.BusinessDate;
                        tck4dg.ID = ticket.ID;
                        tck4dg.CustomerID = ticket.CustomerAKA;
                        tck4dg.TotalPrice = eachPerson;

                        Helper.PrintTicket(tck4dg, 2);
                    }
                }

                // genrate XML for backup
                if (Settings.Default.GenerateXMLforTicket)
                {
                    Helper.GenerateXMLforTicket(ticket.ID);
                }

                // print cancelled ticket
                if (Settings.Default.PrintClosedTicket)
                {
                    ticket.Status = false;
                    ticket.Cash = payForm.cash;
                    ticket.CreditCard = payForm.creditCard;
                    ticket.Transfer = payForm.transfer;
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));
                }

                wpfSplashWindow sw = new wpfSplashWindow(1, "-sp");
                sw.ShowDialog();
                this.Opacity = 1;

                RefreshOpenTicketsListBox();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                this.Opacity = 1;
            }
        }

        private void RefreshOpenTicketsListBox()
        {
            lblTicketNumber.Content = "CUENTA:";

            TicketDetail.Items.Clear();

            if (Settings.Default.UseNickNames)
            {
                lstCustomers = DB.ListBinding_tbl_OpenTickets();
            }
            else
            {
                lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
            }

            lBox_Customers.ItemsSource = lstCustomers;
        }

        private void UpdateTicketDetailDataGrid(clsCustomerVIP custProf)
        {
            try
            {
                int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, custProf.ID);

                if (ticketNum == 0)
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: CLIENTE SELECCIONADO HA SIDO MODIFICADO EN OTRA TRANSACCIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    DB.DeleteTicketDetail("", false);
                    DB.UpdateCustomerStatus(custProf.ID, 0);

                    lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                    lBox_Customers.ItemsSource = lstCustomers;

                    return;
                }

                ticket = DB.GetTicket(DB.GetTicketNumber(Settings.Default.BusinessDate, custProf.ID));

                if (ticket.ApplyServiceFee)
                {
                    FoodService.Visibility = Visibility.Visible;
                    applyFeeService = true;
                }
                else
                {
                    FoodService.Visibility = Visibility.Hidden;
                }

                targGUID = ticket.GUID;

                itemdg = Settings.Default.AllowTicketSummary ? DB.GetItemsByGUID(ticket.GUID, true) : DB.GetItemsByGUID(ticket.GUID, false);

                InitializeItemsDetailCache();

                StoreItemsInCache(itemdg);

                TicketDetail.Items.Clear();

                // ticket created empty
                // ready to receive items from another ticket
                // create dummy row
                if (itemdg.Count == 0 && origGUID != targGUID)
                {
                    clsTicketDetail rdi = new clsTicketDetail();
                    TicketDetail.Items.Add(rdi);
                }
                else
                {
                    foreach (clsItemDetailForDatagrid data in itemdg)
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

                        TicketDetail.Items.Add(rdi);
                    }

                    totalPrice = TotalizeTicket(TicketDetail);

                    TicketDetail.Items.Refresh();
                }

                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                AddProducts.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void InitializeItemsDetailCache()
        {
            try
            {
                fullLogPath = System.IO.Path.Combine(Settings.Default.SerilogRootPath, "WorkArea");

                if (!Directory.Exists(fullLogPath))
                    Directory.CreateDirectory(fullLogPath);

                fullLogFileName = System.IO.Path.Combine(fullLogPath, "TicketDetail.tmp");

                if (File.Exists(fullLogFileName))
                    File.Delete(fullLogFileName);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void StoreItemsInCache(List<clsItemDetailForDatagrid> itemdg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fullLogFileName, true))
                {
                    foreach (clsItemDetailForDatagrid data in itemdg)
                    {
                        sw.WriteLine(data.ID + "|" +
                                     data.ItemID + "|" +
                                     ticket.GUID + "|" +
                                     data.ItemDesc + "|" +
                                     data.Qty + "|" +
                                     data.UnitCost + "|" +
                                     data.TotalCost + "|" +
                                     data.UnitPrice + "|" +
                                     data.TotalPrice);
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private Dictionary<int, int> LoadCacheInMemory()
        {
            try
            {
                Dictionary<int, int> tmpTemsList = new Dictionary<int, int>();

                using (StreamReader sr = new StreamReader(fullLogFileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string rec = sr.ReadLine();

                        if (Settings.Default.AllowTicketSummary)
                            // ItemID
                            tmpTemsList.Add(Convert.ToInt32(rec.Split('|')[1]), Convert.ToInt32(rec.Split('|')[4]));
                        else
                            // ID
                            tmpTemsList.Add(Convert.ToInt32(rec.Split('|')[0]), Convert.ToInt32(rec.Split('|')[4]));
                    }
                }
                return tmpTemsList;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }

        private int TotalizeTicket(DataGrid dg)
        {
            try
            {
                string cash = string.Empty;

                totalPrice = 0;

                foreach (clsTicketDetail rdi in TicketDetail.Items)
                {
                    if (rdi.ItemDesc.Contains("EFECTIVO"))
                    {
                        cash = "*";
                        continue;
                    }

                    totalPrice += rdi.TotalPrice;
                }

                if (applyFeeService)
                {
                    ticket.ServiceFee = totalPrice * 10 / 100;
                    totalPrice += ticket.ServiceFee;
                    ticket.TotalPrice = totalPrice;
                }

                ticket.TotalPrice = totalPrice;

                lblTicketNumber.Content = "CUENTA: " + ticket.ID.ToString("000000");
                lblTotalAmount.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7) + cash + " ";

                return totalPrice;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }

        private void UpdateTicket()
        {
            try
            {
                itemsIDList = LoadCacheInMemory();

                InitializeItemsDetailCache();

                using (StreamWriter sw = new StreamWriter(fullLogFileName, true))
                {
                    itemsDetail.Clear();

                    try
                    {
                        foreach (clsTicketDetail rdi in TicketDetail.Items)
                        {
                            sw.WriteLine(rdi.ID + "|" +
                                         rdi.ItemID + "|" +
                                         rdi.GUID + "|" +
                                         rdi.ItemDesc + "|" +
                                         rdi.Qty + "|" +
                                         rdi.UnitCost + "|" +
                                         rdi.TotalCost + "|" +
                                         rdi.UnitPrice + "|" +
                                         rdi.TotalPrice);

                            itemsDetail.Add(rdi);
                            sw.Flush();
                        }
                    }
                    catch { }
                }

                DB.DeleteTicketDetail(ticket.GUID, true);

                if (DB.InsertTicketDetail(itemsDetail, ticket.GUID, Settings.Default.WhoOpen, true))
                {
                    if (Settings.Default.PrintOrder)
                        Helper.PrintTicket(custProfile.CustomerID, itemsDetail);

                    if (newMealsOrder.Count > 0)
                    {
                        Helper.GetMealItemsFromTicket(custProfile.CustomerID, newMealsOrder);
                        newMealsOrder.Clear();
                    }

                    if (newBeveragesOrder.Count > 0)
                    {
                        Helper.GetBeverageItemsFromTicket(custProfile.CustomerID, newBeveragesOrder);
                        newBeveragesOrder.Clear();
                    }
                }

                lBox_Customers.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
