using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;
using Newtonsoft.Json;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucTickets : UserControl
    {
        #region GLOBAL VARIABLES
        // TRANSLATION VARIABLES
        private wpfMainWindow2 mw;
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

        private int debits = 0;
        private int credits = 0;
        private int totalPrice = 0;
        private int totApplyServiceFee = 0;
        private int totIVAFee = 0;
        private int totalPriceWithoutTaxes = 0;

        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private clsTicket ticket = new clsTicket();
        private clsTicketsForDataGrid ticket4dg = new clsTicketsForDataGrid();
        private List<clsCustomerVIP> lstCustomers = new List<clsCustomerVIP>();
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        private List<clsTicketDetail> itemsDetail = new List<clsTicketDetail>();
        private List<clsTicketDetail> newMealsOrder = new List<clsTicketDetail>();
        private List<clsTicketDetail> newBeveragesOrder = new List<clsTicketDetail>();
        private List<clsItem> lstProducts = new List<clsItem>();
        private Dictionary<int, int> itemsIDList = new Dictionary<int, int>();
        private bool bTrigger = false;

        private List<clsTicketDetail> ticketDetailInMemory = new List<clsTicketDetail>();
        private string origGUID = string.Empty;
        private string targGUID = string.Empty;
        #endregion

        public ucTickets(wpfMainWindow2 _mw, string _lang)
        {
            try
            {
                mw = _mw;
                mw.ucTicketsShared = this;
                lang = _lang;

                InitializeComponent();

                //Traductor.ApplyTranslation(this, lang);

                // GET TICKETS LIST
                RefreshOpenTicketsListBox();

                // GET PRODUCTS LIST
                lstProducts = DB.ListBinding_tbl_Items(6);
                //lBox_Products.ItemsSource = lstProducts;

                InitializeItemsDetailCache();

                lblShiftNumber.Content = $"TURNO ACTIVO: {Settings.Default.Shift}";
                PrintSummary.IsChecked = Settings.Default.AllowTicketSummary ? true : false;
                PrintClosedTicket.IsChecked = Settings.Default.PrintClosedTicket ? true : false;

                ServiceFee.Visibility = Settings.Default.ApplyServiceFee ? Visibility.Visible : Visibility.Hidden;
                lblServiceFee.Visibility = Settings.Default.ApplyServiceFee ? Visibility.Visible : Visibility.Hidden;
                ApplyServiceFee.Visibility = Settings.Default.ApplyServiceFee ? Visibility.Visible : Visibility.Hidden;

                lblIVA.Visibility = Settings.Default.ATVApplyFee ? Visibility.Visible : Visibility.Hidden;
                lblIVAFee.Visibility = Settings.Default.ATVApplyFee ? Visibility.Visible : Visibility.Hidden;
                ApplyIVAFee.Visibility = Settings.Default.ATVApplyFee ? Visibility.Visible : Visibility.Hidden;

                InitializeButtonsState(0);

                this.KeyUp += new KeyEventHandler(ucTicket_KeyUp);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);

            }
        }

        #region UTILITIES

        private void ucTicket_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Add:
                    btn_Increase(sender, null);
                    break;
                case Key.Delete:
                    btn_Delete(sender, null);
                    break;
                case Key.Subtract:
                    btn_Decrease(sender, null);
                    break;
            }
        }

        private void InitializeButtonsState(int state)
        {
            SetUserAccessToResources();

            Loyalty.IsEnabled = false;

            switch (state)
            {
                case 0:
                    CreateTicket.IsEnabled = true;
                    PrintTicket.IsEnabled = false;
                    SplitTicket.IsEnabled = false;
                    AbortTicket.IsEnabled = false;
                    CancelUpdate.IsEnabled = false;
                    UpdateTicket.IsEnabled = false;
                    SmallPayment.IsEnabled = false;
                    PayTicket.IsEnabled = false;
                    ReasignTicket.IsEnabled = false;
                    InheritTicket.IsEnabled = false;
                    AddOldTicket.IsEnabled = false;
                    Loyalty.IsEnabled = false;
                    break;
                case 1:
                    CreateTicket.IsEnabled = true;
                    PrintTicket.IsEnabled = true;
                    SplitTicket.IsEnabled = true;
                    AbortTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_AbortTicket");
                    CancelUpdate.IsEnabled = false;
                    UpdateTicket.IsEnabled = false;
                    SmallPayment.IsEnabled = true;

                    if (Settings.Default.WorkStationType.ToUpper().Contains("MASTER"))
                    {
                        PayTicket.IsEnabled = true;
                    }
                    else
                    {
                        PayTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_PayTicket");
                    }

                    if (custProfile.Type == 1)
                    {
                        Loyalty.IsEnabled = true;
                    }

                    ReasignTicket.IsEnabled = true;
                    InheritTicket.IsEnabled = true;
                    AddOldTicket.IsEnabled = true;
                    break;
                case 2:
                    CreateTicket.IsEnabled = false;
                    PrintTicket.IsEnabled = false;
                    SplitTicket.IsEnabled = false;
                    AbortTicket.IsEnabled = false;
                    CancelUpdate.IsEnabled = true;
                    UpdateTicket.IsEnabled = true;
                    SmallPayment.IsEnabled = false;
                    PayTicket.IsEnabled = false;
                    ReasignTicket.IsEnabled = false;
                    InheritTicket.IsEnabled = false;
                    AddOldTicket.IsEnabled = false;
                    Loyalty.IsEnabled = false;
                    break;
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
                    wpfMessageBox.Show("Ticket Controller",
                                       "ALERTA: NO SE ENCONTRÓ UNA CUENTA ACTIVA DE ESTE CLIENTE CON LA FECHA CONTABLE ACTUAL.",
                                       MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);

                    DB.DeleteTicketDetail("", false);
                    DB.UpdateCustomerStatus(custProf.ID, 0);

                    lstCustomers = DB.ListBinding_tbl_CustomerID(3, 1);
                    lBox_Customers.ItemsSource = lstCustomers;

                    return;
                }

                ticket = DB.GetTicket(DB.GetTicketNumber(Settings.Default.BusinessDate, custProf.ID));

                if (ticket.IVAFee > 0)
                {
                    ApplyIVAFee.IsChecked = true;
                }

                targGUID = ticket.GUID;

                itemdg = Settings.Default.AllowTicketSummary ? DB.GetItemsByGUID(ticket.GUID, true) : DB.GetItemsByGUID(ticket.GUID, false);

                InitializeItemsDetailCache();

                StoreItemsInCache(itemdg);

                TicketDetail.Items.Clear();

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
                        rdi.Note = data.Note;
                        rdi.ImagePath = data.ImagePath;

                        TicketDetail.Items.Add(rdi);
                    }

                    totalPrice = TotalizeTicket(TicketDetail);

                    TicketDetail.Items.Refresh();

                    if (TicketDetail.Items.Count > 1)
                        SplitTicket.IsEnabled = true;
                }

                InitializeButtonsState(1);

                Increase.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                PrintMeal.Visibility = Visibility.Hidden;

                ProductSelecion.IsEnabled = true;
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
                                     data.TotalPrice + "|" +
                                     data.Note);
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
                string cash = string.Empty;

                debits = 0;
                credits = 0;
                totalPrice = 0;
                totIVAFee = 0;
                totApplyServiceFee = 0;
                totalPriceWithoutTaxes = 0;
                ticket.CashLoan = 0;

                foreach (clsTicketDetail rdi in TicketDetail.Items)
                {
                    if (string.IsNullOrEmpty(rdi.ItemDesc))
                    {
                        rdi.ItemDesc = "DESCRIPCIÓN NO DISPONIBLE";
                    }
                    else if (rdi.ItemDesc.Contains("EFECTIVO"))
                    {
                        cash = "*";
                        ticket.CashLoan += rdi.TotalPrice;
                        continue;
                    }

                    if (rdi.TotalPrice < 0)
                    {
                        credits += rdi.TotalPrice;
                    }
                    else
                    {
                        debits += rdi.TotalPrice;
                    }
                }

                totalPrice = debits + credits;
                ticket.TotalPrice = totalPrice;
                totalPriceWithoutTaxes = totalPrice;

                if (ticket.ApplyServiceFee)
                {
                    ApplyServiceFee.IsChecked = true;
                    totApplyServiceFee = debits * 10 / 100;
                    ticket.ServiceFee = totApplyServiceFee;

                    totalPrice = debits + totApplyServiceFee + credits;
                    ticket.TotalPrice = totalPrice;
                }
                else
                {
                    ApplyServiceFee.IsChecked = false;
                }

                if (Settings.Default.ATVApplyFee)
                {
                    if (ApplyIVAFee.IsChecked == true)
                    {
                        totIVAFee = totalPriceWithoutTaxes * 13 / 100;
                        ticket.IVAFee = totIVAFee;
                        ticket.TotalPrice = totalPrice + totIVAFee;
                    }
                }

                lblTicketNumber.Content = ticket.ID.ToString("000000");
                lblServiceFee.Content = totApplyServiceFee.ToString("N0");
                lblIVAFee.Content = totIVAFee.ToString("N0");
                lblTotalPrice.Content = ticket.TotalPrice.ToString("N0").PadLeft(7) + cash;

                if (ticket.TotalPrice >= Settings.Default.SendAlertIfTicketIsHigherThan)
                {
                    wpfMessageBox.Show("Ticket Controller", "ALERTA: EL MONTO DE ESTA CUENTA SUPERA EL MONTO PROMEDIO, FAVOR DE REVISAR QUE ESTÉ CORRECTA..", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                }

                return totalPrice;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return 0;
            }
        }

        private void SetUserAccessToResources()
        {
            try
            {
                CreateTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_CreateTicket");
                PrintTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_PrintTicket");

                if (custProfile.Type > 1)
                {
                    AddOldTicket.IsEnabled = false;
                }
                else
                {
                    AddOldTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_AddOldTicket");
                }

                SplitTicket.IsEnabled = true;
                AbortTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_AbortTicket");
                CancelUpdate.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_CancelUpdate");
                UpdateTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_UpdateTicket");
                SmallPayment.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_SmallPayment");
                PayTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTickets_PayTicket");

                CancelUpdate.IsEnabled = false;
                UpdateTicket.IsEnabled = false;

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "SetUserAccessToResources2 validation PASSED successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return;
            }
        }

        private void RefreshOpenTicketsListBox()
        {
            lblTicketNumber.Content = "0";
            lblServiceFee.Content = "0";
            lblIVAFee.Content = "0";
            lblTotalPrice.Content = "0";

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

        #endregion

        #region TICKETS
        private void txtSearchCustomer_GotFocus(object sender, RoutedEventArgs e)
        {
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
            string txtOrig = txtSearchCustomer.Text.ToUpper();

            var empFiltered = from cust in lstCustomers
                              let ename = cust.CustomerID
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
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

                InitializeProduct(true);

                newMealsOrder.Clear();

                if (custProfile.Type > 1)
                {
                    Loyalty.IsEnabled = false;
                    AddOldTicket.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void lBox_Customers_GotFocus(object sender, RoutedEventArgs e)
        {
            Increase.Visibility = Visibility.Hidden;
            Decrease.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            PrintMeal.Visibility = Visibility.Hidden;
        }

        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsTicketDetail rdi = TicketDetail.SelectedItem as clsTicketDetail;

            if (rdi == null) return;

            clsItem item = DB.GetItem(rdi.ItemID);

            // disable right buttons if the item is a bucket
            if (item.ItemSubType == 2)
            {
                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                return;
            }

            Increase.Visibility = Helper.CheckUserAccessToResource3("ucTickets_btnAdd");
            Decrease.Visibility = Helper.CheckUserAccessToResource3("ucTickets_btnDel");
            Delete.Visibility = Helper.CheckUserAccessToResource3("ucTickets_btnSub");

            if (DB.IsMealItemType(rdi.ItemDesc))
                PrintMeal.Visibility = Helper.CheckUserAccessToResource3("ucTickets_btnPrn"); ;
        }
        
        private void TicketDetail_GotFocus(object sender, RoutedEventArgs e)
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
                        this.Opacity = 0.5;
                        wpfMealNote mn = new wpfMealNote(rdi.ItemDesc);
                        mn.ShowDialog();
                        this.Opacity = 1;
                        rdi.Note = mn.mealNote;

                        clsTicketDetail newMealOrder = new clsTicketDetail();

                        newMealOrder.Qty = 1;
                        newMealOrder.ItemID = rdi.ItemID;
                        newMealOrder.ItemDesc = rdi.ItemDesc;
                        newMealOrder.Note = rdi.Note;

                        newMealsOrder.Add(newMealOrder);
                    }
                    else
                    {
                        clsTicketDetail newBeverageOrder = new clsTicketDetail();

                        newBeverageOrder.Qty = 1;
                        newBeverageOrder.ItemID = rdi.ItemID;
                        newBeverageOrder.ItemDesc = rdi.ItemDesc;
                        newBeverageOrder.Note = rdi.Note;
                        newBeverageOrder.Bucket = false;

                        if (DB.GetItemSubtype(rdi.ItemDesc) == 2)
                        {
                            this.Opacity = 0.5;
                            wpfSelectBucketContent mn = new wpfSelectBucketContent(newBeverageOrder.ItemID);
                            mn.ShowDialog();
                            this.Opacity = 1;
                            newBeverageOrder.Note = mn.bucketContent;
                            newBeverageOrder.Bucket = true;
                        }
                        newBeveragesOrder.Add(newBeverageOrder);
                    }

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ITEM {rdi.ItemID} INCREASED BY 1.", Logger.Severity.INFORMATION);
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);
                lblTotalPrice.Content = ticket.TotalPrice;

                TicketDetail.Items.Refresh();

                lBox_Customers.IsEnabled = false;

                mw.transInProgress = true;

                InitializeButtonsState(2);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Delete(object sender, MouseButtonEventArgs e)
        {
            try
            {
                clsUser userProf = new clsUser();

                clsTicketDetail item = TicketDetail.SelectedItem as clsTicketDetail;

                bool isMealItem = DB.IsMealItemType(item.ItemDesc);

                if (!Settings.Default.CanDeleteItemsFromTicket && isMealItem)
                {
                    wpfMessageBox.Show("Tickets Controller", strNoRemoveMeal, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

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
                    wpfMessageBox.Show("Tickets Controller", "PIN INGRESADO NO TIENE PERMISO PARA ELIMINAR PRODUCTOS", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    return;
                }

                if (isMealItem)
                {
                    try
                    {
                        var itemToRemove = newMealsOrder.Single(r => r.ItemID == item.ItemID);
                        newMealsOrder.Remove(itemToRemove);
                    }
                    catch { }
                }

                DB.InsertItemDeleted(ticket.ID, item.ItemDesc, item.Qty, userProf.userPIN);

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ITEM {item.ItemID} DELETED.", Logger.Severity.INFORMATION);

                TicketDetail.Items.Remove(item);

                ticket.TotalPrice = TotalizeTicket(TicketDetail);

                lblTotalPrice.Content = ticket.TotalPrice;

                TicketDetail.Items.Refresh();

                lBox_Customers.IsEnabled = false;

                mw.transInProgress = true;

                InitializeButtonsState(2);

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
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

                    if (DB.IsMealItemType(rdi.ItemDesc))
                    {
                        try
                        {
                            var itemToRemove = newMealsOrder.First(r => r.ItemID == rdi.ItemID);
                            newMealsOrder.Remove(itemToRemove);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            var itemToRemove = newBeveragesOrder.First(r => r.ItemID == rdi.ItemID);
                            newBeveragesOrder.Remove(itemToRemove);
                        }
                        catch { }
                    }
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ITEM {rdi.ItemID} DECREASED BY 1.", Logger.Severity.INFORMATION);
                }

                ticket.TotalPrice = TotalizeTicket(TicketDetail);
                lblTotalPrice.Content = ticket.TotalPrice;

                TicketDetail.Items.Refresh();

                mw.transInProgress = true;

                InitializeButtonsState(2);
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
        private void InitializeProduct(bool action)
        {
            //newMealsOrder.Clear();
            //lBox_Products.SelectedIndex = -1;

            //if (action)
            //{
            //    txtSearchProduct.IsEnabled = true;
            //    lBox_Products.IsEnabled = true;
            //    lBox_Products.SelectedIndex = -1;
            //}
            //else
            //{
            //    lBox_Products.SelectedIndex = -1;
            //    txtProductQty.Text = string.Empty;
            //    txtProductQty.IsEnabled = false;
            //    AddProduct.IsEnabled = false;
            //}
        }

        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            //try
            //{
            //    string txtOrig = txtSearchProduct.Text;
            //    string upper = txtOrig.ToUpper();
            //    string lower = txtOrig.ToLower();

            //    var empFiltered = from prod in lstProducts
            //                      let ename = prod.ItemDescription
            //                      where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
            //                      select prod;
            //    bTrigger = true;
            //    lBox_Products.ItemsSource = empFiltered;
            //    bTrigger = false;
            //}
            //catch (Exception ex)
            //{
            //    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            //}
        }

        private void txtSearchProduct_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (Settings.Default.VirtualAlphaKeyboardActive)
            //{
            //    this.Opacity = 0.5;
            //    wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard();
            //    alphaKey.ShowDialog();
            //    this.Opacity = 1;
            //    txtSearchProduct.Text = alphaKey.alphaKeyed;
            //}
        }

        private void lBox_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (!bTrigger)
            //{
            //    txtProductQty.Text = "1";
            //    txtProductQty.IsEnabled = true;
            //    AddProduct.IsEnabled = true;
            //}
        }

        private void txtProductQty_GotFocus(object sender, RoutedEventArgs e)
        {
            //this.Opacity = 0.5;
            //wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            //numKey.ShowDialog();
            //this.Opacity = 1;
            //txtProductQty.Text = numKey.numKeyed;
        }

        private void btn_AddProduct(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    if (string.IsNullOrEmpty(txtProductQty.Text))
            //    {
            //        txtProductQty.Text = "1";
            //        return;
            //    }

            //    clsItem item = lBox_Products.SelectedItem as clsItem;

            //    if (item == null)
            //    {
            //        wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: ANTES DE AGREGAR DEBE DE SELECCIONAR EL PRODUCTO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
            //        return;
            //    }

            //    clsTicketDetail newItem = new clsTicketDetail();

            //    newItem.ItemID = item.ID;
            //    newItem.ItemDesc = item.ItemDescription;
            //    newItem.Qty = Convert.ToInt32(txtProductQty.Text);
            //    newItem.UnitCost = item.UnitCost;
            //    newItem.TotalCost = item.UnitCost * newItem.Qty;
            //    newItem.UnitPrice = item.UnitPrice;
            //    newItem.TotalPrice = item.UnitPrice * newItem.Qty;

            //    if (custProfile.CustomerFOC)
            //    {
            //        newItem.UnitPrice = 0;
            //        newItem.UnitCost = 0;
            //        newItem.TotalPrice = 0;
            //        newItem.TotalCost = 0;
            //    }

            //    if (DB.IsMealItemType(item.ItemDescription))
            //    {
            //        wpfMealNote mn = new wpfMealNote(item.ItemDescription);
            //        mn.ShowDialog();
            //        newItem.Note = mn.mealNote;

            //        clsTicketDetail newMealOrder = new clsTicketDetail();

            //        newMealOrder.ID = newItem.ID;
            //        newMealOrder.ItemID = newItem.ItemID;
            //        newMealOrder.GUID = newItem.GUID;
            //        newMealOrder.ItemDesc = newItem.ItemDesc;
            //        newMealOrder.Qty = newItem.Qty;
            //        newMealOrder.UnitCost = newItem.UnitCost;
            //        newMealOrder.TotalCost = newItem.TotalCost;
            //        newMealOrder.UnitPrice = newItem.UnitPrice;
            //        newMealOrder.TotalPrice = newItem.TotalPrice;
            //        newMealOrder.Note = newItem.Note;

            //        newMealsOrder.Add(newMealOrder);
            //    }

            //    TicketDetail.Items.Add(newItem);
            //    TicketDetail.Items.Refresh();

            //    totalPrice = TotalizeTicket(TicketDetail);
            //    ticket.TotalPrice = totalPrice;

            //    txtSearchProduct.Text = string.Empty;
            //    txtProductQty.Text = string.Empty;
            //    txtProductQty.IsEnabled = false;
            //    AddProduct.IsEnabled = false;

            //    lBox_Customers.IsEnabled = false;

            //    mw.transInProgress = true;

            //    InitializeButtonsState(2);
            //}
            //catch (Exception ex)
            //{
            //    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            //}
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

        private void PrintSummary_Checked(object sender, RoutedEventArgs e)
        {

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
                debits = 0;
                credits = 0;
                totalPrice = 0;
                totApplyServiceFee = 0;
                totIVAFee = 0;
                totalPriceWithoutTaxes = 0;

                foreach (clsTicketDetail rdi in TicketDetail.Items)
                {
                    if (rdi.TotalPrice < 0)
                    {
                        credits += rdi.TotalPrice;
                    }
                    else
                    {
                        debits += rdi.TotalPrice;
                    }
                }

                totalPriceWithoutTaxes = debits + credits;
                totApplyServiceFee = debits * 10 / 100;
                ticket.ServiceFee = totApplyServiceFee;

                if (Settings.Default.ATVApplyFee && ApplyIVAFee.IsChecked == true )
                {
                    totIVAFee = totalPriceWithoutTaxes * 13 / 100;
                }

                totalPrice = debits + totApplyServiceFee + credits;
                ticket.TotalPrice = totalPrice + totIVAFee;

                DB.UpdateFeeServiceToTicket(ticket.ID, true, totApplyServiceFee, totIVAFee);
            }
            else
            {
                UpdateTicketDetailDataGrid(custProfile);

                totApplyServiceFee = 0;
                totalPrice = itemdg.Sum(x => x.TotalPrice);

                ticket.ServiceFee = totApplyServiceFee;
                ticket.TotalPrice = totalPrice;

                if (Settings.Default.ATVApplyFee && ApplyIVAFee.IsChecked == true)
                {
                    totIVAFee = totalPriceWithoutTaxes * 13 / 100;
                    ticket.TotalPrice = totalPrice + totIVAFee;
                }

                DB.UpdateFeeServiceToTicket(ticket.ID, false, totApplyServiceFee, totIVAFee);
            }

            lblTicketNumber.Content = ticket.ID.ToString("000000");
            lblServiceFee.Content = totApplyServiceFee.ToString("N0");
            lblIVAFee.Content = totIVAFee.ToString("N0");
            lblTotalPrice.Content = ticket.TotalPrice.ToString("N0").PadLeft(7);
        }

        private void ApplyIVAFee_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyIVAFee.IsChecked == true)
            {
                if (ApplyServiceFee.IsChecked == true)
                {
                    debits = 0;
                    credits = 0;
                    totalPrice = 0;
                    totApplyServiceFee = 0;
                    totIVAFee = 0;
                    totalPriceWithoutTaxes = 0;

                    foreach (clsTicketDetail rdi in TicketDetail.Items)
                    {
                        if (rdi.TotalPrice < 0)
                        {
                            credits += rdi.TotalPrice;
                        }
                        else
                        {
                            debits += rdi.TotalPrice;
                        }
                    }

                    totalPriceWithoutTaxes = debits + credits;
                    totApplyServiceFee = debits * 10 / 100;
                    ticket.ServiceFee = totApplyServiceFee;

                    totIVAFee = totalPriceWithoutTaxes * 13 / 100;

                    totalPrice = debits + totApplyServiceFee + credits;
                    ticket.TotalPrice = totalPrice + totIVAFee;

                    DB.UpdateFeeServiceToTicket(ticket.ID, true, totApplyServiceFee, totIVAFee);
                }
                else
                {
                    UpdateTicketDetailDataGrid(custProfile);

                    totApplyServiceFee = 0;
                    totalPrice = itemdg.Sum(x => x.TotalPrice);

                    ticket.ServiceFee = totApplyServiceFee;
                    ticket.TotalPrice = totalPrice;

                    totIVAFee = totalPriceWithoutTaxes * 13 / 100;
                    ticket.TotalPrice = totalPrice + totIVAFee;

                    DB.UpdateFeeServiceToTicket(ticket.ID, false, totApplyServiceFee, totIVAFee);
                }
            }
            else
            {
                if (ApplyServiceFee.IsChecked == true)
                {
                    debits = 0;
                    credits = 0;
                    totalPrice = 0;
                    totApplyServiceFee = 0;
                    totIVAFee = 0;
                    totalPriceWithoutTaxes = 0;

                    foreach (clsTicketDetail rdi in TicketDetail.Items)
                    {
                        if (rdi.TotalPrice < 0)
                        {
                            credits += rdi.TotalPrice;
                        }
                        else
                        {
                            debits += rdi.TotalPrice;
                        }
                    }

                    totalPriceWithoutTaxes = debits + credits;
                    totApplyServiceFee = debits * 10 / 100;
                    ticket.ServiceFee = totApplyServiceFee;

                    totalPrice = debits + totApplyServiceFee + credits;
                    ticket.TotalPrice = totalPrice;

                    DB.UpdateFeeServiceToTicket(ticket.ID, true, totApplyServiceFee, totIVAFee);
                }
                else
                {
                    UpdateTicketDetailDataGrid(custProfile);

                    totalPrice = itemdg.Sum(x => x.TotalPrice);
                    ticket.TotalPrice = totalPrice;

                    DB.UpdateFeeServiceToTicket(ticket.ID, false, totApplyServiceFee, totIVAFee);
                }
                totIVAFee = 0;
            }

            lblTicketNumber.Content = ticket.ID.ToString("000000");
            lblServiceFee.Content = totApplyServiceFee.ToString("N0");
            lblIVAFee.Content = totIVAFee.ToString("N0");
            lblTotalPrice.Content = ticket.TotalPrice.ToString("N0").PadLeft(7);
        }
        #endregion

        #region BUTTONS
        private void btn_CreateTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                mw.Opacity = 0.5;

                //wpfNewTicketStep1 nts1 = new wpfNewTicketStep1(lang);
                //nts1.ShowDialog();

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"CREATE TICKET...", Logger.Severity.INFORMATION);

                wpfNewTicket wpfNT = new wpfNewTicket(lang);
                wpfNT.ShowDialog();
                mw.Opacity = 1;

                if (wpfNT.newTicket)
                {
                    RefreshOpenTicketsListBox();
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
            Mouse.OverrideCursor = Cursors.Wait;
            PrintTicket.IsEnabled = false;

            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"PRINT TICKET {ticket.ID}", Logger.Severity.INFORMATION);

            Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));

            wpfSplashWindow sw = new wpfSplashWindow(1, "");
            sw.ShowDialog();

            PrintTicket.IsEnabled = true;            
            Mouse.OverrideCursor = null;
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

                wpfAbortReason ar = new wpfAbortReason();
                ar.ShowDialog();

                if (string.IsNullOrEmpty(ar.abortReason)) return;

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"ABORT TICKET {ticket.ID}", Logger.Severity.INFORMATION);

                DB.IncludeAbortReason(ticket.ID, ar.abortReason, Convert.ToInt32(userProf.userPIN));

                DB.InsertNewTicketAborted(ticket.ID);

                DB.CancelTicket(ticket.ID, Settings.Default.WhoOpen, 2);

                DB.DeleteTicketDetail(ticket.GUID, false);

                DB.DeleteOpenTickets(custProfile.ID);

                DB.UpdateCustomerStatus(custProfile.ID, 0);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();

                TicketDetail.Items.Clear();

                RefreshOpenTicketsListBox();

                InitializeButtonsState(0);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_CancelUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"CANCEL CHANGES TO TICKET {ticket.ID}", Logger.Severity.INFORMATION);

                TicketDetail.Items.Clear();
                newMealsOrder.Clear();

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

                InitializeProduct(false);

                mw.transInProgress = false;
                mw.transInProgressTries = 0;

                InitializeButtonsState(1);
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
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"SAVE CHANGES TO TICKET {ticket.ID}", Logger.Severity.INFORMATION);

                itemsIDList = LoadCacheInMemory();

                InitializeItemsDetailCache();

                using (StreamWriter sw = new StreamWriter(fullLogFileName, true))
                {
                    itemsDetail.Clear();

                    try
                    {
                        foreach (clsTicketDetail rdi in TicketDetail.Items)
                        {
                            // if the customer is Free Of Charge
                            if (custProfile.CustomerFOC)
                            {
                                rdi.UnitPrice = 0;
                                rdi.UnitCost = 0;
                                rdi.TotalPrice = 0;
                                rdi.TotalCost = 0;
                            }

                            sw.WriteLine(rdi.ID + "|" +
                                         rdi.ItemID + "|" +
                                         rdi.GUID + "|" +
                                         rdi.ItemDesc + "|" +
                                         rdi.Qty + "|" +
                                         rdi.UnitCost + "|" +
                                         rdi.TotalCost + "|" +
                                         rdi.UnitPrice + "|" +
                                         rdi.TotalPrice + "|" +
                                         rdi.Note);

                            itemsDetail.Add(rdi);
                            sw.Flush();
                        }
                    }
                    catch { }
                }

                DB.DeleteTicketDetail(ticket.GUID, true);

                DB.InsertTicketDetail(itemsDetail, ticket.GUID, Settings.Default.WhoOpen, true);

                if (Settings.Default.PrintOrder)
                    Helper.PrintTicket(custProfile.CustomerID, itemsDetail);

                if (newMealsOrder.Count > 0)
                {
                    Helper.GetMealItemsFromTicket(custProfile.CustomerID, newMealsOrder);
                }

                if (newBeveragesOrder.Count > 0)
                {
                    Helper.GetBeverageItemsFromTicket(ticket.ID.ToString() + "^" + custProfile.CustomerID, newBeveragesOrder);
                }

                newMealsOrder.Clear();
                newBeveragesOrder.Clear();

                wpfSplashWindow swnd = new wpfSplashWindow(1, lang);
                swnd.ShowDialog();

                Increase.Visibility = Visibility.Hidden;
                Decrease.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                PrintMeal.Visibility = Visibility.Hidden;

                lBox_Customers.IsEnabled = true;

                InitializeProduct(false);

                mw.transInProgress = false;
                mw.transInProgressTries = 0;

                InitializeButtonsState(1);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void UpdateBeforeDeleteItem()
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
                }
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
                mw.Opacity = 0.5;
                wpfSpecialItems spec = new wpfSpecialItems();
                spec.ShowDialog();

                if (spec.ItemID == 0)
                {
                    mw.Opacity = 1;
                    return;
                }

                if (spec.ItemDesc.Contains("EFECTIVO"))
                {
                    mw.Opacity = 0.5;
                    wpfEnterAmount enterAmount = new wpfEnterAmount();
                    enterAmount.ShowDialog();
                    mw.Opacity = 1;

                    if (enterAmount.amount == 0) return;

                    cash = enterAmount.amount;
                }
                else
                {
                    // select payment method
                    wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticket.ID, false, 0);
                    payForm.ShowDialog();

                    if (payForm.payOK == false)
                    {
                        mw.Opacity = 1;
                        return;
                    }
                    cash = payForm.cash;
                    creditCard = payForm.creditCard;
                    transfer = payForm.transfer;
                }

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

                // prepare Item record
                List<clsTicketDetail> smlPaymentList = new List<clsTicketDetail>();
                clsTicketDetail smlPayment = new clsTicketDetail();

                smlPayment.GUID = ticket.GUID;
                smlPayment.ItemID = spec.ItemID;
                smlPayment.ItemDesc = spec.ItemDesc;
                smlPayment.Qty = 1;

                if (spec.ItemDesc.Contains("EFECTIVO"))
                    smlPayment.UnitPrice = paymentAmount;
                else
                    smlPayment.UnitPrice = paymentAmount * -1;

                smlPayment.UnitCost = 0;
                smlPayment.TotalPrice = smlPayment.UnitPrice;
                smlPaymentList.Add(smlPayment);

                DB.InsertTicketDetail(smlPaymentList, ticket.GUID, Settings.Default.WhoOpen, true);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();
                mw.Opacity = 1;

                UpdateTicketDetailDataGrid(custProfile);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                mw.Opacity = 1;
            }
        }

        private void btn_PayTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                mw.Opacity = 0.3;

                if (Settings.Default.RequestPIN)
                {
                    wpfRequestPIN wpfPIN = new wpfRequestPIN();
                    wpfPIN.ShowDialog();

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

                wpfPayMethod2 payForm = new wpfPayMethod2(lang, ticket.TotalPrice, ticket.ID, true, ticket.CashLoan);
                payForm.ShowDialog();

                if (payForm.payOK == false)
                {
                    mw.Opacity = 1;
                    return;
                }

                // update inventory
                foreach (clsTicketDetail idg in TicketDetail.Items)
                {
                    if (idg.ItemID > 100000)
                    {
                        // old ticket added to the current ticket
                        DB.UpdateTicketStatus(idg.ItemID - 100000, 0, idg.TotalPrice, 0, idg.TotalPrice, 0, 0, 0, Settings.Default.WhoOpen, custProfile.CustomerID);

                        // add ticket to tbl_TicketsOldCancelled
                        DB.InsertOldTicketCancelled(Settings.Default.BusinessDate, idg.ItemID - 100000, idg.TotalPrice);
                    }

                    clsItem item = new clsItem();
                    item.ID = idg.ItemID;
                    item.ItemSubType = idg.ItemSubType;
                    item.ItemSold = idg.Qty;

                    DB.UpdateItemInventory("SAL", item);

                    if (DB.IsMealItemType(idg.ItemDesc))
                    {
                        Helper.ApplySaleToInvenytory(idg.ItemID, idg.Qty);
                    }

                    clsPromoConfig promo = DB.GetPromotion(idg.ItemID);

                    if (promo.ID > 0)
                    {
                        clsItem ni = new clsItem();
                        ni.ID = promo.ItemID;
                        ni.ItemSold = promo.PromoQty * idg.Qty;
                        DB.UpdateItemInventory("SAL", ni);
                    }
                }

                // check if the ticket have buckets
                List<clsBucketsDetail> haveBuckets = DB.GetBucketsByTicketNumber(ticket.ID);

                if (haveBuckets.Count > 0)
                {
                    foreach (clsBucketsDetail bi in haveBuckets)
                    {
                        clsItem item = new clsItem();
                        item.ID = bi.ItemID;
                        item.ItemSold = bi.Qty;
                        DB.UpdateItemInventory("SAL", item);
                    }
                    DB.DeleteBucketDetailByTicketNumber(ticket.ID);
                }

                // update ticket
                DB.UpdateTicketStatus(ticket.ID, 0, ticket.TotalPrice, ticket.ServiceFee, payForm.cash, payForm.creditCard, payForm.transfer, payForm.voucher,
                                      Settings.Default.WhoOpen, custProfile.CustomerID);

                // update customer status
                DB.UpdateCustomerStatus(custProfile.ID, 0);

                // update loyalty points for VIP only
                if (custProfile.Type == 1)
                {
                    DB.UpdateCustomerLoyaltyPoints(custProfile.ID, totalPrice);
                }

                // delete Customer from OpenTickets
                DB.DeleteOpenTickets(custProfile.ID);

                if (payForm.transfer > 0)
                {
                    if (Settings.Default.PrintSINPETicket)
                    {
                        ticket.Transfer = payForm.transfer;
                        Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID), 1);
                    }
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
                if (Settings.Default.PrintClosedTicket || Settings.Default.PrintClosedTicketQuestion)
                {
                    if (wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: DESEA IMPRIMIR LA FACTURA (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                    {
                        ticket.Status = false;
                        ticket.Cash = payForm.cash;
                        ticket.CreditCard = payForm.creditCard;
                        ticket.Transfer = payForm.transfer;
                        ticket.Voucher = payForm.voucher;
                        Helper.PrintTicket(Helper.Convert2TicketsForDataGrid(ticket, custProfile.CustomerID));
                    }
                    else
                    {
                        if (Settings.Default.UseCashDrawer)
                        {
                            xPrinterOpenCashbox xpCash = new xPrinterOpenCashbox();
                            xpCash.print();
                            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Open Cash Drawer request by user {Settings.Default.WhoOpen}", Logger.Severity.WARNING);
                        }
                    }
                }

                if (Settings.Default.ATVApplyFee)
                {
                    if (payForm.send2IRSforElectronicTicket || payForm.send2IRSforElectronicInvoice)
                    {
                        ElectronicDoc ATV = new ElectronicDoc();
                        ATV.DocElectronico = new DocElectronico();

                        // header
                        ATV.DocElectronico.Token = Settings.Default.ATVToken;
                        ATV.DocElectronico.CodigoActividad = Settings.Default.ATVActivityCode;
                        ATV.DocElectronico.Cliente = Settings.Default.ATVClientCode;

                        if (payForm.send2IRSforElectronicInvoice)
                        {
                            mw.Opacity = 0.5;
                            wpfElectronicInvoice einv = new wpfElectronicInvoice(ticket.ID);
                            einv.ShowDialog();
                            mw.Opacity = 1;

                            if (einv.bCancel)
                            {
                                InitializeButtonsState(0);

                                RefreshOpenTicketsListBox();

                                mw.transInProgress = false;
                                mw.transInProgressTries = 0;

                                return;
                            }

                            ATVQuery atvqry = new ATVQuery();

                            atvqry.TicketID = ticket.ID;
                            atvqry.CustomerName = einv.custName;
                            atvqry.SSN_Type = einv.custIDType;
                            atvqry.SSN = einv.custID;
                            atvqry.CountryCode = einv.custCountryCode;
                            atvqry.PhoneNumber = einv.custPhoneNumber;
                            atvqry.eMailAddress = einv.custEmail;

                            DB.InsertATVTicket(atvqry);

                            // receptor info
                            ATV.DocElectronico.Receptor = new WhoReceive();
                            ATV.DocElectronico.Receptor.Nombre = einv.custName;

                            ATV.DocElectronico.Receptor.Identificacion = new SSN();
                            ATV.DocElectronico.Receptor.Identificacion.Tipo = einv.custIDType;
                            ATV.DocElectronico.Receptor.Identificacion.Numero = einv.custID;

                            ATV.DocElectronico.Receptor.Telefono = new PhoneNumber();
                            ATV.DocElectronico.Receptor.Telefono.CodigoPais = einv.custCountryCode;
                            ATV.DocElectronico.Receptor.Telefono.NumTelefono = einv.custPhoneNumber;
                            ATV.DocElectronico.Receptor.CorreoElectronico = einv.custEmail;
                        }

                        // ticket header
                        ATV.DocElectronico.CondicionVenta = 1;

                        if (payForm.cash > 0 && payForm.creditCard == 0 && payForm.transfer == 0)
                        {
                            ATV.DocElectronico.MedioPago = "01";
                        }
                        else if (payForm.cash == 0 && payForm.creditCard > 0 && payForm.transfer == 0)
                        {
                            ATV.DocElectronico.MedioPago = "02";
                        }
                        else if (payForm.cash == 0 && payForm.creditCard == 0 && payForm.transfer > 0)
                        {
                            ATV.DocElectronico.MedioPago = "04";
                        }
                        else
                        {
                            if (payForm.cash > 0 && payForm.creditCard > 0 && payForm.transfer == 0)
                            {
                                ATV.DocElectronico.MedioPago = "01,02";
                            }
                            else if (payForm.cash > 0 && payForm.creditCard == 0 && payForm.transfer > 0)
                            {
                                ATV.DocElectronico.MedioPago = "01,04";
                            }
                            else if (payForm.cash > 0 && payForm.creditCard > 0 && payForm.transfer > 0)
                            {
                                ATV.DocElectronico.MedioPago = "01,02,04";
                            }
                            else if (payForm.cash == 0 && payForm.creditCard > 0 && payForm.transfer > 0)
                            {
                                ATV.DocElectronico.MedioPago = "02,04";
                            }
                            else
                            {
                                ATV.DocElectronico.MedioPago = "01";
                            }
                        }

                        // ticket detail
                        LineDetail lineDetail = new LineDetail();
                        lineDetail.NumeroLinea = 1;
                        lineDetail.Codigo = 6331000000000;

                        lineDetail.CodigoComercial = new ComercialCode();
                        lineDetail.CodigoComercial.Tipo = 1;
                        lineDetail.CodigoComercial.Codigo = 4;

                        lineDetail.Cantidad = 1;
                        lineDetail.UnidadMedida = "Unid";
                        lineDetail.Detalle = "SERVICIO DE RESTAURANTE";
                        lineDetail.PrecioUnitario = ticket.TotalPrice;

                        lineDetail.Descuento = new Discount();
                        lineDetail.Descuento.MontoDescuento = 0;
                        lineDetail.Descuento.NaturalezaDescuento = "SIN DESCUENTO";

                        lineDetail.SubTotal = ticket.TotalPrice;

                        lineDetail.Impuesto = new Tax();
                        lineDetail.Impuesto.Codigo = 1;
                        lineDetail.Impuesto.CodigoTarifa = 8;
                        lineDetail.Impuesto.Tarifa = 13;
                        lineDetail.Impuesto.Monto = lineDetail.SubTotal * 13 / 100;

                        lineDetail.MontoTotalLinea = lineDetail.SubTotal + lineDetail.Impuesto.Monto;

                        // ticket summary
                        ATV.DocElectronico.DetalleServicio = new ServiceDetail();
                        ATV.DocElectronico.DetalleServicio.LineaDetalle = new List<LineDetail>();
                        ATV.DocElectronico.DetalleServicio.LineaDetalle.Add(lineDetail);

                        ATV.DocElectronico.OtrosCargos = new OtherCharges();
                        ATV.DocElectronico.OtrosCargos.TipoDocumento = 6;
                        ATV.DocElectronico.OtrosCargos.Detalle = "Impuesto de Servicio 10%";
                        ATV.DocElectronico.OtrosCargos.MontoCargo = 0;

                        ATV.DocElectronico.ResumenFactura = new TicketSummary();
                        ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda = new CurrencyTypeCode();
                        ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.CodigoMoneda = "CRC";
                        ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.TipoCambio = 1;

                        // Serializing JSON
                        string jsonOutput = JsonConvert.SerializeObject(ATV);
                        JSON.ATVSendWebServiceCall(ticket.ID, jsonOutput);
                    }
                }

                InitializeButtonsState(0);

                wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                sw.ShowDialog();
                mw.Opacity = 1;

                RefreshOpenTicketsListBox();

                mw.transInProgress = false;
                mw.transInProgressTries = 0;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                mw.Opacity = 1;
            }
        }

        private void btn_SplitTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                mw.Opacity = 0.5;
                wpfSplitTicket2 splitTicket = new wpfSplitTicket2(ticket, (bool)ApplyServiceFee.IsChecked, (bool)ApplyIVAFee.IsChecked);
                splitTicket.ShowDialog();
                UpdateTicketDetailDataGrid(custProfile);
                RefreshOpenTicketsListBox();
                mw.Opacity = 1;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                mw.Opacity = 1;
            }
        }

        private void btn_ReasignTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Opacity = 0.5;
                wpfSelectTarget selTar = new wpfSelectTarget(0, ticket.ID, custProfile, lstCustomers);
                selTar.ShowDialog();
                this.Opacity = 1;

                InitializeButtonsState(0);

                RefreshOpenTicketsListBox();

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                mw.Opacity = 1;
            }
        }

        private void btn_InheritTicket(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfSelectTarget selTar = new wpfSelectTarget(1, ticket.ID, custProfile, lstCustomers);
            selTar.ShowDialog();
            this.Opacity = 1;

            RefreshOpenTicketsListBox();
        }

        private void btn_AddOldTicket(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfCustomerOpenTickets2 custOpenTcks = new wpfCustomerOpenTickets2(custProfile.ID, custProfile.CustomerID);
            custOpenTcks.ShowDialog();
            mw.Opacity = 1;

            if (custOpenTcks.tcks2Add.Count > 0)
            {
                List<clsTicketDetail> opentcks2Add = new List<clsTicketDetail>();

                foreach (KeyValuePair<int, int> item in custOpenTcks.tcks2Add)
                {
                    clsTicketDetail openTck2Add = new clsTicketDetail();

                    openTck2Add.GUID = ticket.GUID;
                    openTck2Add.ItemID = 100000 + item.Key;
                    openTck2Add.ItemDesc = "CUENTA " + item.Key.ToString("000000");
                    openTck2Add.Qty = 1;
                    openTck2Add.UnitPrice = item.Value;
                    openTck2Add.UnitCost = 0;
                    openTck2Add.TotalPrice = item.Value;

                    opentcks2Add.Add(openTck2Add);
                }
                DB.InsertTicketDetail(opentcks2Add, ticket.GUID, Settings.Default.WhoOpen, true);
                UpdateTicketDetailDataGrid(custProfile);
            }
        }

        private void btn_ProductSelecion(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfSelectProducts prodsel = new wpfSelectProducts(custProfile, lstProducts);
            prodsel.ShowDialog();
            this.Opacity = 1;

            if (!prodsel.bOK) return;

            foreach (clsTicketDetail item in prodsel.SelectedProducts)
            {
                if (custProfile.CustomerFOC)
                {
                    item.UnitPrice = 0;
                    item.UnitCost = 0;
                    item.TotalPrice = 0;
                    item.TotalCost = 0;
                }

                switch (item.ItemType)
                {
                    case 1:
                        item.ImagePath = @"C:\AWC.DigitalCommerce\Images\beer.png";
                        break;
                    case 2:
                        item.ImagePath = @"C:\AWC.DigitalCommerce\Images\liquors.ico";
                        break;
                    case 3:
                        item.ImagePath = @"C:\AWC.DigitalCommerce\Images\kitchen.ico";
                        break;
                    case 9:
                        item.ImagePath = @"C:\AWC.DigitalCommerce\Images\otherTrans.png";
                        break;
                }

                TicketDetail.Items.Add(item);
                totalPrice = TotalizeTicket(TicketDetail);
                ticket.TotalPrice = totalPrice;
            }

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
            lBox_Customers.IsEnabled = false;
            mw.transInProgress = true;
            InitializeButtonsState(2);
        }

        private void btn_Loyalty(object sender, RoutedEventArgs e)
        {
            Helper.InDevelopment();
        }

        #endregion

        #region MOUSE EVENTS
        private void lBox_Customers_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Help;
        }

        private void lBox_Products_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Help;
        }

        private void TicketDetail_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Help;
        }

        private void lBox_Products_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void lBox_Customers_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void TicketDetail_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
        #endregion

        #region CONTEXTMENU
        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TicketDetail.SelectedItems.Count > 0 && PrintSummary.IsChecked == false;
            e.Handled = true;
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            origGUID = string.Empty;
            ticketDetailInMemory.Clear();

            foreach (clsTicketDetail item2Move in TicketDetail.SelectedItems)
            {
                if (item2Move.ID == 9999)
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: PAGAR PODER MOVER PRODUCTOS ENTRE CUENTAS, PRIMERO DEBE DE DESACTIVAR LA CASILLA 'AGRUPAR PRODUCTOS'.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                if (string.IsNullOrEmpty(origGUID))
                    origGUID = item2Move.GUID;

                ticketDetailInMemory.Add(item2Move);
            }
            CopyPaste.Visibility = Visibility.Visible;
        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ticketDetailInMemory.Count > 0 && origGUID != targGUID;
            e.Handled = true;
        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (clsTicketDetail item2Paste in ticketDetailInMemory)
            {
                item2Paste.GUID = targGUID;
                DB.UpdateTicketDetailGUID(origGUID, targGUID, item2Paste.ID);
            }

            CopyPaste.Visibility = Visibility.Hidden;

            origGUID = string.Empty;
            ticketDetailInMemory.Clear();

            InitializeButtonsState(0);

            mw.Opacity = 0.5;
            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();
            mw.Opacity = 1;

            RefreshOpenTicketsListBox();

            mw.transInProgress = false;
            mw.transInProgressTries = 0;
        }
        
        private void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ticketDetailInMemory.Count > 0;
            e.Handled = true;
        }

        private void Undo(object sender, ExecutedRoutedEventArgs e)
        {
            origGUID = string.Empty;
            ticketDetailInMemory.Clear();
            CopyPaste.Visibility = Visibility.Hidden;
        }

        private void CanReplace(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = custProfile.Type == 2;
            e.Handled = true;
        }

        private void Replace(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                wpfRenameCustomerAKA wpfRenCustAKA = new wpfRenameCustomerAKA(custProfile.CustomerID);
                wpfRenCustAKA.ShowDialog();

                if (wpfRenCustAKA.newCustAKA.Length > 0)
                {
                    DB.RenameCustomerAKA(ticket.ID, custProfile.ID, wpfRenCustAKA.newCustAKA);
                    RefreshOpenTicketsListBox();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TicketDetail.SelectedItems.Count == 1 && PrintSummary.IsChecked == false;
            e.Handled = true;
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                this.Opacity = 0.5;
                wpfEnterAmount wpfea = new wpfEnterAmount();
                wpfea.ShowDialog();
                this.Opacity = 1;

                if (wpfea.amount == 0) return;

                clsTicketDetail rdi = TicketDetail.SelectedItem as clsTicketDetail;

                DB.ChangeItemPriceInTicket(rdi, wpfea.amount);

                UpdateTicketDetailDataGrid(custProfile);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void CanPrint(object sender, CanExecuteRoutedEventArgs e)
        {
            clsTicketDetail rdi = TicketDetail.SelectedItem as clsTicketDetail;

            if (rdi == null) return;

            clsItem item = DB.GetItem(rdi.ItemID);

            e.CanExecute = TicketDetail.SelectedItems.Count == 1 && item.ItemSubType == 2;
            e.Handled = true;
        }

        private void Print(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                wpfBucketContent bc = new wpfBucketContent(ticket.ID);
                bc.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        #endregion
    }
}
