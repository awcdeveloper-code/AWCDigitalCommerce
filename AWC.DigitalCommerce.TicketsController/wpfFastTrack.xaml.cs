using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfFastTrack : Window
    {
        #region GLOBAL VARIABLES
        // TRANSLATION VARIABLES
        private string lang = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strQtyEqualZero = string.Empty;
        public string strNoRemoveMeal = string.Empty;
        public string strNoRemoveItem = string.Empty;
        public string strRemoveItem = string.Empty;
        public string strAbortTicket = string.Empty;
        public string strPendingUpdate = string.Empty;
        public string strNoTicket = string.Empty;

        // WORK VARIABLES
        private string fullLogPath = string.Empty;
        private string fullLogFileName = string.Empty;

        private int totalPrice = 0;
        private int totApplyServiceFee = 0;

        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private clsTicket ticket = new clsTicket();
        private clsTicketsForDataGrid ticket4dg = new clsTicketsForDataGrid();
        private List<clsCustomerVIP> lstCustomers = new List<clsCustomerVIP>();
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        private List<clsTicketDetail> itemsDetail = new List<clsTicketDetail>();
        private List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        private List<clsItem> lstProducts = new List<clsItem>();
        private Dictionary<int, int> itemsIDList = new Dictionary<int, int>();
        #endregion

        public wpfFastTrack(string _lang)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            lang = _lang;

            InitializeComponent();

            DisplayUpdateSalesSummary(Settings.Default.DisplayUpdateSalesSummary);

            this.KeyDown += new KeyEventHandler(wpfFastTrack_KeyUp);

            Traductor.ApplyTranslation(this, lang);

            // GET TICKETS LIST
            lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
            lBox_Customers.ItemsSource = lstCustomers;

            // GET PRODUCTS LIST
            lstProducts = DB.ListBinding_tbl_Items(0);
            lBox_Products.ItemsSource = lstProducts;

            InitializeItemsDetailCache();

            PrintSummary.IsChecked = Settings.Default.AllowTicketSummary ? true : false;
            PrintClosedTicket.IsChecked = Settings.Default.PrintClosedTicket ? true : false;

            UpdateSalesSummary();
        }

        #region UTILITIES
        private void wpfFastTrack_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        if (UpdateTicket.IsEnabled)
                        {
                            if (wpfMessageBox.Show("Ticket Controller", strPendingUpdate, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Error, lang) == MessageBoxResult.No)
                                return;
                        }

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Fast-Track was closed.", Logger.Severity.INFORMATION);
                        this.Close();
                        break;
                    }
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
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void UpdateTicketDetailDataGrid(clsCustomerVIP custProf)
        {
            try
            {
                int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, custProf.ID);

                if (ticketNum == 0)
                {
                    wpfMessageBox.Show("Ticket Controller", strNoTicket, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    DB.DeleteTicketDetail("", false);
                    DB.UpdateCustomerStatus(custProf.ID, 0);

                    lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                    lBox_Customers.ItemsSource = lstCustomers;

                    return;
                }

                ticket = DB.GetTicket(DB.GetTicketNumber(Settings.Default.BusinessDate, custProf.ID));

                itemdg = Settings.Default.AllowTicketSummary ? DB.GetItemsByGUID(ticket.GUID, true) : DB.GetItemsByGUID(ticket.GUID, false);

                InitializeItemsDetailCache();
                
                StoreItemsInCache(itemdg);

                TicketDetail.Items.Clear();

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

                if (TicketDetail.Items.Count > 1)
                    SplitTicket.IsEnabled = true;

                Increase.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                PrintMeal.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
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
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
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
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }

        private List<clsTicketDetail> ExtractNewMealsOrder(Dictionary<int, int> tmpDict)
        {
            try
            {
                List<clsTicketDetail> tmp = new List<clsTicketDetail>();

                try
                {
                    foreach (clsTicketDetail rdi in TicketDetail.Items)
                    {
                        if (DB.IsMealItemType(rdi.ItemDesc))
                        {
                            if (tmpDict.ContainsKey(rdi.ItemID))
                            {
                                if (rdi.Qty > tmpDict[rdi.ItemID])
                                {
                                    clsTicketDetail newMealOrder = new clsTicketDetail();

                                    newMealOrder.ID = rdi.ID;
                                    newMealOrder.ItemID = rdi.ItemID;
                                    newMealOrder.GUID = rdi.GUID;
                                    newMealOrder.ItemDesc = rdi.ItemDesc;
                                    newMealOrder.Qty = rdi.Qty - tmpDict[rdi.ItemID];
                                    newMealOrder.UnitCost = rdi.UnitCost;
                                    newMealOrder.TotalCost = rdi.TotalCost;
                                    newMealOrder.UnitPrice = rdi.UnitPrice;
                                    newMealOrder.TotalPrice = rdi.TotalPrice;
                                    newMealOrder.Note = rdi.Note;

                                    tmp.Add(newMealOrder);
                                }
                            }
                        }
                    }
                }
                catch { }
                return tmp;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return null;
            }
        }

        private int TotalizeTicket(DataGrid dg)
        {
            try
            {
                totalPrice = 0;
                totApplyServiceFee = 0;

                foreach (clsTicketDetail rdi in TicketDetail.Items)
                {
                    totalPrice += rdi.TotalPrice;
                }

                ticket.TotalPrice = totalPrice;

                if (ApplyServiceFee.IsChecked == true)
                {
                    totApplyServiceFee = totalPrice * 10 / 100;
                    ticket.ServiceFee = totApplyServiceFee;

                    totalPrice += totApplyServiceFee;
                    ticket.TotalPrice = totalPrice;
                }

                lblTicketNumber.Content = "CUENTA: " + ticket.ID.ToString("000000");
                lblTotalPrice.Content = totalPrice.ToString("N0").PadLeft(7);
                lblServiceFee.Content = totApplyServiceFee.ToString("N0").PadLeft(7);

                return totalPrice;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return 0;
            }
        }

        private void UpdateSalesSummary()
        {
            try
            {
                clsTicket ticketSummary = DB.GetTicketsSummary(Settings.Default.BusinessDate);
                clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(Settings.Default.BusinessDate);

                ticketSummary.Cash += smlPay.Cash;
                ticketSummary.CreditCard += smlPay.CreditCard;
                ticketSummary.Transfer += smlPay.Transfer;

                int totalPrice = ticketSummary.Cash +
                                 ticketSummary.CreditCard +
                                 ticketSummary.Transfer +
                                 ticketSummary.Payments; // Outstanding (not Payments really)

                lblTotalReceivable.Content = ticketSummary.Payments.ToString("N0");
                lblTotalCash.Content = ticketSummary.Cash.ToString("N0");
                lblTotalCreditCard.Content = ticketSummary.CreditCard.ToString("N0");
                lblTotalTransfer.Content = ticketSummary.Transfer.ToString("N0");
                lblTotalServiceFee.Content = ticketSummary.ServiceFee.ToString("N0");
                lblTotalSale.Content = totalPrice.ToString("N0");
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void DisplayUpdateSalesSummary(bool display)
        {
            if (display)
            {
                lbl_TotalSalesRealTime.Visibility = Visibility.Visible;
                lbl_TotalReceivable.Visibility = Visibility.Visible;
                lbl_TotalCash.Visibility = Visibility.Visible;
                lbl_TotalCreditCard.Visibility = Visibility.Visible;
                lbl_TotalTransfer.Visibility = Visibility.Visible;
                lbl_TotalSale.Visibility = Visibility.Visible;   // Change to Hidden for NUBIA
                lbl_TotalServiceFee.Visibility = Visibility.Visible;

                lblTotalReceivable.Visibility = Visibility.Visible;
                lblTotalCash.Visibility = Visibility.Visible;
                lblTotalCreditCard.Visibility = Visibility.Visible;
                lblTotalTransfer.Visibility = Visibility.Visible;
                lblTotalSale.Visibility = Visibility.Visible;    // Change to Hidden for NUBIA
                lblTotalServiceFee.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_TotalSalesRealTime.Visibility = Visibility.Hidden;
                lbl_TotalReceivable.Visibility = Visibility.Hidden;
                lbl_TotalCash.Visibility = Visibility.Hidden;
                lbl_TotalCreditCard.Visibility = Visibility.Hidden;
                lbl_TotalTransfer.Visibility = Visibility.Hidden;
                lbl_TotalSale.Visibility = Visibility.Hidden;
                lbl_TotalServiceFee.Visibility = Visibility.Hidden;

                lblTotalReceivable.Visibility = Visibility.Hidden;
                lblTotalCash.Visibility = Visibility.Hidden;
                lblTotalCreditCard.Visibility = Visibility.Hidden;
                lblTotalTransfer.Visibility = Visibility.Hidden;
                lblTotalSale.Visibility = Visibility.Hidden;
                lblTotalServiceFee.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region TICKETS
        private void txtSearchCustomer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Topmost = false;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                this.Topmost = true;
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
                if (lBox_Customers.SelectedIndex == -1)
                    return;

                custProfile = lBox_Customers.SelectedItem as clsCustomerVIP;

                if (custProfile.ApplyServiceFee)
                    ApplyServiceFee.IsChecked = true;
                else
                    ApplyServiceFee.IsChecked = false;

                ApplyServiceFee.IsEnabled = true;

                UpdateTicketDetailDataGrid(custProfile);

                //if (Settings.Default.ShowCustFreqItemsPopUp)
                //{
                //    List<clsCustFreqItem> custFreqItemsList = DB.GetCustomerFrequentItems(custProfile.ID);

                //    // if the customer has history
                //    if (custFreqItemsList.Count > 0)
                //    {
                //        wpfCustFreqItems cfi = new wpfCustFreqItems(custFreqItemsList);
                //        cfi.ShowDialog();

                //        if (cfi.itemSelected)
                //            txtSearchProduct.Text = cfi.custFreqItem.ItemDescription;
                //    }
                //}

                txtSearchProduct.IsEnabled = true;
                lBox_Products.IsEnabled = true;

                PrintTicket.IsEnabled = true;
                AbortTicket.IsEnabled = true;
                SmallPayment.IsEnabled = true;
                PayTicket.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsTicketDetail rdi = TicketDetail.SelectedItem as clsTicketDetail;

            if (rdi == null) return;

            Increase.Visibility = Visibility.Visible;
            Decrease.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;

            if (DB.IsMealItemType(rdi.ItemDesc))
                PrintMeal.Visibility = Visibility.Visible;
            else
                PrintMeal.Visibility = Visibility.Hidden;
        }

        private void btn_Increase(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
                {
                    rdi.Qty++;
                    rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;

                    if (DB.IsMealItemType(rdi.ItemDesc))
                    {
                        this.Topmost = false;
                        wpfMealNote mn = new wpfMealNote(rdi.ItemDesc);
                        mn.ShowDialog();
                        this.Topmost = true;
                        rdi.Note = mn.mealNote;
                    }
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);
                lblTotalPrice.Content = ticket.TotalPrice;

                TicketDetail.Items.Refresh();

                lBox_Customers.IsEnabled = false;

                CloseFastTrack.IsEnabled = false;
                CreateTicket.IsEnabled = false;
                PrintTicket.IsEnabled = false;
                AbortTicket.IsEnabled = false;
                CancelUpdate.IsEnabled = true;
                UpdateTicket.IsEnabled = true;
                SmallPayment.IsEnabled = false;
                PayTicket.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Delete(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.AllowTicketSummary)
            {
                wpfMessageBox.Show("Ticket Controller", strNoRemoveItem, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                return;
            }

            if (wpfMessageBox.Show("Ticket Controller", strRemoveItem, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (clsTicketDetail row in TicketDetail.SelectedItems)
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

                    List<clsTicketDetail> removeItems = new List<clsTicketDetail>();

                    int total2Reduce = 0;

                    foreach (clsTicketDetail row in TicketDetail.SelectedItems)
                    {
                        total2Reduce += row.TotalPrice;
                        removeItems.Add(row);
                        DB.UpdateTicketDetailRemoved(row.ID);
                    }

                    DB.UpdateTicketTotalPrice(ticket.ID, total2Reduce);

                    UpdateTicketDetailDataGrid(custProfile);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                }
            }
        }

        private void btn_Decrease(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (clsTicketDetail rdi in TicketDetail.SelectedItems)
                {
                    if (rdi.Qty == 1)
                    {
                        wpfMessageBox.Show("Ticket Controller", strQtyEqualZero, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                        continue;
                    }
                    else
                    {
                        rdi.Qty--;
                        rdi.TotalPrice = rdi.UnitPrice * rdi.Qty;
                    }
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);
                lblTotalPrice.Content = ticket.TotalPrice;

                TicketDetail.Items.Refresh();

                CloseFastTrack.IsEnabled = false;
                CreateTicket.IsEnabled = false;
                PrintTicket.IsEnabled = false;
                AbortTicket.IsEnabled = false;
                CancelUpdate.IsEnabled = true;
                UpdateTicket.IsEnabled = true;
                SmallPayment.IsEnabled = false;
                PayTicket.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_PrintMeal(object sender, MouseButtonEventArgs e)
        {
            List<string> meal = new List<string>();

            clsTicketDetail row = TicketDetail.SelectedItem as clsTicketDetail;

            meal.Add(row.Qty.ToString() + "|" + row.ItemDesc + "|");

            Helper.PrintTicket(custProfile.CustomerID, meal, true);
        }
        #endregion

        #region PRODUCTS
        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string txtOrig = txtSearchProduct.Text;
                string upper = txtOrig.ToUpper();
                string lower = txtOrig.ToLower();

                var empFiltered = from prod in lstProducts
                                  let ename = prod.ItemDescription
                                  where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                                  select prod;

                lBox_Products.ItemsSource = empFiltered;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void txtSearchProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Topmost = false;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(4);
                alphaKey.ShowDialog();
                this.Topmost = true;
                txtSearchProduct.Text = alphaKey.alphaKeyed;
            }
        }

        private void lBox_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtProductQty.Text = "1";
            txtProductQty.IsEnabled = true;
            AddProduct.IsEnabled = true;
        }

        private void txtProductQty_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            this.Topmost = true;
            txtProductQty.Text = numKey.numKeyed;
        }

        private void btn_AddProduct(object sender, MouseButtonEventArgs e)
        {
            clsItem item = lBox_Products.SelectedItem as clsItem;

            clsTicketDetail newItem = new clsTicketDetail();

            newItem.ItemID = item.ID;
            newItem.ItemDesc = item.ItemDescription;
            newItem.Qty = Convert.ToInt32(txtProductQty.Text);
            newItem.UnitPrice = item.UnitPrice;
            newItem.TotalPrice = item.UnitPrice * newItem.Qty;

            if (DB.IsMealItemType(item.ItemDescription))
            {
                wpfMealNote mn = new wpfMealNote(item.ItemDescription);
                this.Topmost = false;
                mn.ShowDialog();
                this.Topmost = true;
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

                newMealsOrder.Add(newMealOrder);

                Helper.GetMealItemsFromTicket(custProfile.CustomerID, newMealsOrder);
            }

            TicketDetail.Items.Add(newItem);
            TicketDetail.Items.Refresh();

            totalPrice = TotalizeTicket(TicketDetail);
            ticket.TotalPrice = totalPrice;

            txtSearchProduct.Text = String.Empty;
            txtProductQty.IsEnabled = false;
            AddProduct.IsEnabled = false;

            lBox_Customers.IsEnabled = false;

            CloseFastTrack.IsEnabled = false;
            CreateTicket.IsEnabled = false;
            PrintTicket.IsEnabled = false;
            AbortTicket.IsEnabled = false;
            CancelUpdate.IsEnabled = true;
            UpdateTicket.IsEnabled = true;
            SmallPayment.IsEnabled = false;
            PayTicket.IsEnabled = false;
        }
        #endregion

        #region CHECKBOXES
        private void chkBox_PrintSummary_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.AllowTicketSummary == false)
                Settings.Default.AllowTicketSummary = true;
            else
                Settings.Default.AllowTicketSummary = false;

            Settings.Default.Save();

            if (custProfile.ID > 0)
                UpdateTicketDetailDataGrid(custProfile);
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
                totApplyServiceFee = totalPrice * 10 / 100;

                totalPrice += totApplyServiceFee;

                ticket.ServiceFee = totApplyServiceFee;
                ticket.TotalPrice = totalPrice;

                lblTotalPrice.Content = totalPrice.ToString("N0").PadLeft(7);
                lblServiceFee.Content = totApplyServiceFee.ToString("N0").PadLeft(7);
            }
            else
            {
                UpdateTicketDetailDataGrid(custProfile);

                totApplyServiceFee = 0;
                totalPrice = itemdg.Sum(x => x.TotalPrice);

                ticket.ServiceFee = totApplyServiceFee;
                ticket.TotalPrice = totalPrice;

                lblTotalPrice.Content = totalPrice.ToString("N0").PadLeft(7);
                lblServiceFee.Content = totApplyServiceFee.ToString("N0").PadLeft(7);
            }
        }
        #endregion

        #region BUTTONS
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_CreateTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Topmost = false;
                wpfNewTicket wpfNT = new wpfNewTicket(lang);
                wpfNT.ShowDialog();
                this.Topmost = true;

                if (wpfNT.newTicket)
                {
                    lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                    lBox_Customers.ItemsSource = lstCustomers;
                    UpdateSalesSummary();
                }
                return;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_PrintTicket(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));
        }

        private void btn_AbortTicket(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Ticket Controller", strAbortTicket, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                try
                {
                    DB.DeleteTicketDetail(ticket.GUID, false);

                    DB.CancelTicket(ticket.ID, Settings.Default.WhoOpen, 2);

                    DB.UpdateCustomerStatus(custProfile.ID, 0);

                    wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                    sw.ShowDialog();

                    TicketDetail.Items.Clear();

                    UpdateSalesSummary();

                    lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                    lBox_Customers.ItemsSource = lstCustomers;
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                }
            }
        }

        private void btn_CancelUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                TicketDetail.Items.Clear();

                using (StreamReader sr = new System.IO.StreamReader(fullLogFileName))
                {
                    while (!sr.EndOfStream)
                    {
                        string rec = sr.ReadLine();

                        clsTicketDetail rdi = new clsTicketDetail();

                        rdi.ID = Convert.ToInt32(rec.Split('|')[0]);
                        rdi.ItemID = Convert.ToInt32(rec.Split('|')[1]);
                        rdi.GUID = rec.Split('|')[2];
                        rdi.ItemDesc = rec.Split('|')[3];
                        rdi.Qty = Convert.ToInt32(rec.Split('|')[4]);
                        rdi.UnitCost = Convert.ToInt32(rec.Split('|')[5]);
                        rdi.TotalCost = Convert.ToInt32(rec.Split('|')[6]);
                        rdi.UnitPrice = Convert.ToInt32(rec.Split('|')[7]);
                        rdi.TotalPrice = Convert.ToInt32(rec.Split('|')[8]);

                        TicketDetail.Items.Add(rdi);
                    }
                }

                TicketDetail.Items.Refresh();

                totalPrice = TotalizeTicket(TicketDetail);

                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                PrintMeal.Visibility = Visibility.Hidden;

                lBox_Customers.IsEnabled = true;

                CloseFastTrack.IsEnabled = true;
                CreateTicket.IsEnabled = true;
                PrintTicket.IsEnabled = true;
                AbortTicket.IsEnabled = true;
                CancelUpdate.IsEnabled = false;
                UpdateTicket.IsEnabled = false;
                SmallPayment.IsEnabled = true;
                PayTicket.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_UpdateTicket(object sender, RoutedEventArgs e)
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

                    newMealsOrder = ExtractNewMealsOrder(itemsIDList);

                    if (newMealsOrder.Count > 0)
                        Helper.GetMealItemsFromTicket(Helper.Ticket.CustID, newMealsOrder);
                }

                wpfSplashWindow swnd = new wpfSplashWindow(1, lang);
                swnd.ShowDialog();

                UpdateSalesSummary();

                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                PrintMeal.Visibility = Visibility.Hidden;

                lBox_Customers.IsEnabled = true;

                CloseFastTrack.IsEnabled = true;
                CreateTicket.IsEnabled = true;
                PrintTicket.IsEnabled = true;
                AbortTicket.IsEnabled = true;
                CancelUpdate.IsEnabled = false;
                UpdateTicket.IsEnabled = false;
                SmallPayment.IsEnabled = true;
                PayTicket.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
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
                this.Topmost = false;
                wpfSpecialItems spec = new wpfSpecialItems();
                spec.ShowDialog();
                this.Topmost = true;

                if (spec.ItemID == 0) return;

                // select payment method
                this.Topmost = false;
                wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticket.ID, false, 0);
                payForm.ShowDialog();
                this.Topmost = true;

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
                    smlPay.CustomerID = custProfile.ID;
                    smlPay.TicketID = ticket.ID;
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

                smlPayment.GUID = ticket.GUID;
                smlPayment.ItemID = spec.ItemID;
                smlPayment.ItemDesc = spec.ItemDesc;
                smlPayment.Qty = 1;
                smlPayment.UnitPrice = paymentAmount * -1;
                smlPayment.UnitCost = 0;
                smlPayment.TotalPrice = smlPayment.UnitPrice;
                smlPayment.UnitCost = 0;
                smlPaymentList.Add(smlPayment);

                DB.InsertTicketDetail(smlPaymentList, ticket.GUID, Settings.Default.WhoOpen, true);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();

                UpdateTicketDetailDataGrid(custProfile);

                UpdateSalesSummary();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_PayTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Settings.Default.RequestPIN)
                {
                    this.Topmost = false;
                    wpfRequestPIN wpfPIN = new wpfRequestPIN();
                    wpfPIN.ShowDialog();
                    this.Topmost = true;

                    clsUser userProf = DB.CheckUserPIN(wpfPIN.numKeyed);

                    if (userProf.userActive == false)
                    {
                        wpfMessageBox.Show("Ticket Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                        return;
                    }

                    Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
                    Settings.Default.WhoOpenName = userProf.userName;
                    Settings.Default.Save();
                }

                this.Topmost = false;
                wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticket.ID, true, 0);
                payForm.ShowDialog();
                this.Topmost = true;

                if (payForm.payOK == false) return; // CANCEL

                // update inventory
                foreach (clsTicketDetail idg in TicketDetail.Items)
                {
                    clsItem item = new clsItem();
                    item.ID = idg.ItemID;
                    item.ItemSold = idg.Qty;
                    DB.UpdateItemInventory("SAL", item);
                }

                if (payForm.transfer > 0)
                {
                    // print voucher
                    ticket.Transfer = payForm.transfer;
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID), 1);
                }

                // update ticket
                DB.UpdateTicketStatus(ticket.ID, 0, ticket.TotalPrice, ticket.ServiceFee, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher, Settings.Default.WhoOpen, custProfile.CustomerID);

                // update customer status
                DB.UpdateCustomerStatus(custProfile.ID, 0);

                // print cancelled ticket
                if (Settings.Default.PrintClosedTicket)
                {
                    ticket.Status = false;
                    ticket.Cash = payForm.cash;
                    ticket.CreditCard = payForm.creditCard;
                    ticket.Transfer = payForm.transfer;
                    Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));
                }

                lblTicketNumber.Content = "ID:";
                lblServiceFee.Content = "0";
                lblTotalPrice.Content = "0";

                UpdateSalesSummary();

                TicketDetail.Items.Clear();

                lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                lBox_Customers.ItemsSource = lstCustomers;

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        
        private void btn_Express(object sender, RoutedEventArgs e)
        {
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Quick Order was called.", Logger.Severity.INFORMATION);
            this.Topmost = false;
            wpfQuickOrder quickOrder = new wpfQuickOrder();
            quickOrder.ShowDialog();
            this.Topmost = true;
        }

        private void btn_SplitTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Topmost = false;
                wpfSplitTicket splitTicket = new wpfSplitTicket(ticket, (bool)ApplyServiceFee.IsChecked);
                splitTicket.ShowDialog();
                this.Topmost = true;
                UpdateTicketDetailDataGrid(custProfile);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        #endregion
    }
}
