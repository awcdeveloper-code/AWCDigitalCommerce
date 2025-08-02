using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucNewTicket : UserControl
    {
        #region GLOBAL VARIABLES
        public delegate void OnTicketDetailDataEvent(object sender, clsTicketDetail data);
        public event OnTicketDetailDataEvent ClickTicketDetailData;

        public delegate void OnMainWindowDataEvent(object sender, int source);
        public event OnMainWindowDataEvent ClickMainWindowData;
        private wpfMainWindow mw;

        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private string customerID = string.Empty;
        private bool btn_CleanOrderClicked = false;
        private bool custFOC = false;
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private List<clsItem> lstBeer = new List<clsItem>();
        private List<clsItem> lstLiqour = new List<clsItem>();
        private List<clsItem> lstMeal = new List<clsItem>();
        #endregion

        #region MESSAGES
        private string lang = string.Empty;
        public string strCustomerNoExist = string.Empty;
        public string strCustomerAdded = string.Empty;
        public string strCustomerExist = string.Empty;
        public string strValueCannotBeZero = string.Empty;
        public string strCustomerIDNotFound = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strTickedAdded = string.Empty;
        public string strERRORsavingTck = string.Empty;
        public string strERRORsavingTckDet = string.Empty;
        #endregion

        public ucNewTicket(wpfMainWindow _mw, string _lang)
        {
            mw = _mw;
            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            ClickTicketDetailData += new ucNewTicket.OnTicketDetailDataEvent(Subscribe_Event);

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_VIP.ItemsSource = lstVIP;

            lBox_TablesSeats.DataContext = DB.DataBinding_tbl_CustomerID(2,0); ;    // Tables and Seats

            lstBeer = DB.ListBinding_tbl_Items(1);      // Beverages
            lBox_Beer.ItemsSource = lstBeer;

            lstLiqour = DB.ListBinding_tbl_Items(2);    // Liqours
            lBox_Liqour.ItemsSource = lstLiqour;

            lstMeal = DB.ListBinding_tbl_Items(3);      // Meals
            lBox_Meal.ItemsSource = lstMeal;

            if (Settings.Default.PrintOrder)
                PrintOrder.IsChecked = true;

            // disable
            txtSearchBeer.IsEnabled = false;
            lBox_Beer.IsEnabled = false;
            txtQtyBeer.IsEnabled = false;

            txtSearchLiqour.IsEnabled=false;
            lBox_Liqour.IsEnabled = false;
            txtQtyLiqour.IsEnabled= false;

            txtSearchMeal.IsEnabled = false;
            lBox_Meal.IsEnabled = false;
            txtQtyMeal.IsEnabled =false;

            SetUserAccessToResources();
        }

        #region UTILITIES
        private void Subscribe_Event(object sender, clsTicketDetail data)
        {
            //ucNewTicketDetail.ReceiveDataFromNewTicketDetail(data);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void AlphabeticValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void txtUnkowCust_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                txtUnkowCust.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtUnkowCust_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                if (txtUnkowCust.Text.Length > 0)
                {
                    if (!DB.CustomerIDExist(txtUnkowCust.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtUnkowCust.Text), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

                            //
                            // string custID, int type, int subType, int status, int serviceFee, int freeOfcharge, int creditLimit)
                            if (DB.InsertNewCustomer(txtUnkowCust.Text, 1, 0, 0, serviceFee, 0, Settings.Default.CreditLimitByDefault))
                            {
                                lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
                                lBox_VIP.ItemsSource = lstVIP;
                                CleanAll();
                            }
                        }
                        else
                        {
                            CleanAll();
                            return;
                        }
                    }
                    else
                    {
                        wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerExist, txtUnkowCust.Text.ToUpper()), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                        txtUnkowCust.Text = string.Empty;
                    }
                }
            }
        }
        private void CleanAll()
        {
            btn_CleanOrderClicked = true;

            itemsDetails.Clear();

            lBox_VIP.UnselectAll();
            lBox_VIP.IsEnabled = true;

            lBox_TablesSeats.UnselectAll();
            lBox_TablesSeats.IsEnabled = true;

            txtUnkowCust.Text = string.Empty;
            txtUnkowCust.IsEnabled = true;

            lBox_Beer.UnselectAll();
            lBox_Beer.IsEnabled = false;

            lBox_Liqour.UnselectAll();
            lBox_Liqour.IsEnabled=false;

            lBox_Meal.UnselectAll();
            lBox_Meal.IsEnabled=false;

            txtSearchVIP.Text = string.Empty;

            txtSearchBeer.Text = string.Empty;
            txtSearchBeer.IsEnabled=false;
            txtQtyBeer.Text = string.Empty;

            txtSearchLiqour.Text = string.Empty;
            txtSearchLiqour.IsEnabled = false;
            txtQtyLiqour.Text = string.Empty;

            txtSearchMeal.Text = string.Empty;
            txtSearchMeal.IsEnabled = false;
            txtQtyMeal.Text = string.Empty;

            txtQtyBeer.IsEnabled = false;
            AddBeer.IsEnabled = false;

            txtQtyLiqour.IsEnabled = false;
            AddLiqour.IsEnabled = false;

            txtQtyMeal.IsEnabled = false;
            AddMeal.IsEnabled = false;

            TakeOrder.IsEnabled = false;

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_VIP.ItemsSource = lstVIP;

            lBox_TablesSeats.DataContext = DB.DataBinding_tbl_CustomerID(2, 0);

            //mw.NewTicket.IsEnabled = true;

            btn_CleanOrderClicked = false;
        }
        private void SetUserAccessToResources()
        {
            NewCustomerGroupBox.IsEnabled = Helper.CheckUserAccessToResource("NewCustomer");
        }
        #endregion

        #region ENABLE ADD BUTTONS
        private void lBox_Beer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtQtyBeer.IsEnabled = true;
            txtQtyBeer.Text = "1";
            AddBeer.IsEnabled = true;
        }

        private void lBox_Liqour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtQtyLiqour.IsEnabled = true;
            txtQtyLiqour.Text= "1";
            AddLiqour.IsEnabled = true;
        }

        private void lBox_Meal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtQtyMeal.IsEnabled = true;
            txtQtyMeal.Text = "1";
            AddMeal.IsEnabled = true;
        }
        #endregion

        #region ADD ITEMS TO THE TICKET
        private void btn_AddBeer(object sender, RoutedEventArgs e)
        {
            if (txtQtyBeer.Text.Trim().Length == 0) return;

            int iQtyBeer = int.Parse(txtQtyBeer.Text.Trim(), NumberStyles.Integer);

            if (iQtyBeer == 0)
            {
                wpfMessageBox.Show("Ticket Controller", strValueCannotBeZero, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            if (ClickTicketDetailData != null)
            {
                clsItem tmp = (clsItem)lBox_Beer.SelectedItem;
                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = tmp.ItemDescription;
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.Qty = Convert.ToInt32(txtQtyBeer.Text.Trim());
                ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
                ntd.UnitCost = 0;
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;
                ntd.TotalCost = 0;

                // if the customer is Free Of Charge
                if (custFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                //mw.NewTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Beer.UnselectAll();
            txtQtyBeer.Text = string.Empty;
            txtQtyBeer.IsEnabled=false;
            txtSearchBeer.Text = string.Empty;
            AddBeer.IsEnabled = false;
        }
        private void btn_AddLiqour(object sender, RoutedEventArgs e)
        {
            if (txtQtyLiqour.Text.Trim().Length == 0) return;

            int iQtyLiquor = int.Parse(txtQtyLiqour.Text.Trim(), NumberStyles.Integer);

            if (iQtyLiquor == 0)
            {
                wpfMessageBox.Show("Ticket Controller", strValueCannotBeZero, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            if (ClickTicketDetailData != null)
            {
                clsItem tmp = (clsItem) lBox_Liqour.SelectedItem;
                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = tmp.ItemDescription;
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.Qty = Convert.ToInt32(txtQtyLiqour.Text.Trim());
                ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
                ntd.UnitCost = 0;
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;
                ntd.TotalCost = 0;

                // if the customer is Free Of Charge
                if (custFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                //mw.NewTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Liqour.UnselectAll();
            txtQtyLiqour.Text = string.Empty;
            txtQtyLiqour.IsEnabled=false;
            txtSearchLiqour.Text = string.Empty;
            AddLiqour.IsEnabled = false;
        }
        private void btn_AddMeal(object sender, RoutedEventArgs e)
        {
            if (txtQtyMeal.Text.Trim().Length == 0) return;

            int iQtyMeal = int.Parse(txtQtyMeal.Text.Trim(), NumberStyles.Integer);

            if (iQtyMeal == 0)
            {
                wpfMessageBox.Show("Ticket Controller", strValueCannotBeZero, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            if (ClickTicketDetailData != null)
            {
                clsItem tmp = (clsItem)lBox_Meal.SelectedItem;
                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = tmp.ItemDescription;
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.Qty = Convert.ToInt32(txtQtyMeal.Text.Trim());
                ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
                ntd.UnitCost = DB.GetUnitCostByItemDescription(ntd.ItemDesc);
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;
                ntd.TotalCost = ntd.UnitCost * ntd.Qty;

                wpfMealNote mn = new wpfMealNote(tmp.ItemDescription);
                mn.ShowDialog();
                ntd.Note = mn.mealNote;

                // if the customer is Free Of Charge
                if (custFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                //mw.NewTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Meal.UnselectAll();
            txtQtyMeal.Text = string.Empty;
            txtQtyMeal.IsEnabled=false;
            AddMeal.IsEnabled = false;
        }
        #endregion

        #region ADD TICKET TO TE SYSTEM        
        private void btn_TakeOrder(object sender, RoutedEventArgs e)
        {
            // Confirm Order
            this.Opacity = 0.5;
            wpfConfirmOrder confirmOrder = new wpfConfirmOrder(itemsDetails);
            confirmOrder.ShowDialog();
            this.Opacity = 1;

            if (!confirmOrder.confirmed) return;

            // check if the ticket already exist
            int ID = DB.GetIDByCustomerID(customerID);

            if (ID == 0)
            {
                wpfMessageBox.Show("Ticket Controller", strCustomerIDNotFound, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }
            else
            {
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

                // Create the ticket
                clsTicket ticket = new clsTicket();
                Guid guidID = Guid.NewGuid();

                // for capture old tickets (backlog)
                if (Settings.Default.OldTicketsDate.Length > 0)
                    ticket.TicketDate = Settings.Default.OldTicketsDate;
                else
                    ticket.TicketDate = Settings.Default.BusinessDate;

                ticket.GUID = guidID.ToString();
                ticket.CustID = ID;
                ticket.TotalPrice = 0;
                ticket.Status = true;

                //if (DB.InsertNewTicket(ticket, Settings.Default.WhoOpen))
                //{
                //    // create the details
                //    if (DB.InsertTicketDetail(itemsDetails, ticket.GUID, Settings.Default.WhoOpen, true))
                //    {
                //        if (Settings.Default.PrintOrder)
                //            Helper.PrintTicket(customerID, itemsDetails);

                //        Helper.GetMealItemsFromTicket(ID, itemsDetails);
                //    }
                //    else
                //        wpfMessageBox.Show("Ticket Controller", strERRORsavingTckDet, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                //}
                //else
                //    wpfMessageBox.Show("Ticket Controller", strERRORsavingTck, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);

                CleanAll();
            }
        }
        private int GetTotalPriceFromItemsList(List<clsTicketDetail> itemsDetails)
        {
            int totPrice = 0;

            foreach (clsTicketDetail itemDetail in itemsDetails)
                totPrice += itemDetail.TotalPrice;

            return totPrice;
        }
        private void btn_CleanOrder(object sender, RoutedEventArgs e)
        {
            CleanAll();
        }
        #endregion

        #region DISABLE CONTROLS
        private void lBox_VIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {
                clsCustomerVIP tmp = (clsCustomerVIP) lBox_VIP.SelectedItem;
                customerID = tmp.CustomerID;

                // get customer profile
                clsCustomerVIP custProf = DB.GetCustomerProfile(customerID);
                custFOC = custProf.CustomerFOC;

                // check customer birthday
                //if (custProf.BirthDay == DateTime.Now.ToString("MMdd"))
                //{
                //    wpfCustomerBirthDay wpfBirth = new wpfCustomerBirthDay(customerID);
                //    wpfBirth.ShowDialog();
                //}

                // check open tickets                
                List<clsTicketsForDataGrid> custOpenTcks = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, 1, custProf.ID);

                if (custOpenTcks.Count >= Settings.Default.MaxNumOpenTicketsPerCustomer)
                {
                    wpfCustomerOpenTickets frmCustOpen = new wpfCustomerOpenTickets(customerID, custOpenTcks);
                    frmCustOpen.ShowDialog();
                }

                // disable
                lBox_TablesSeats.IsEnabled = false;
                txtUnkowCust.IsEnabled = false;

                // enable
                txtSearchBeer.IsEnabled = true;
                lBox_Beer.IsEnabled = true;

                txtSearchLiqour.IsEnabled = true;
                lBox_Liqour.IsEnabled = true;

                txtSearchMeal.IsEnabled = true;
                lBox_Meal.IsEnabled = true;

                // show customer frequent items
                if (Settings.Default.ShowCustFreqItemsPopUp)
                {
                    List<clsCustFreqItem> custFreqItemsList = DB.GetCustomerFrequentItems(custProf.ID);

                    // if the customer has history
                    if (custFreqItemsList.Count > 0)
                    {
                        wpfCustFreqItems cfi = new wpfCustFreqItems(custFreqItemsList);
                        cfi.ShowDialog();

                        if (cfi.itemSelected)
                        {
                            switch (cfi.custFreqItem.ItemType)
                            {
                                case 1:
                                    txtSearchBeer.Text = cfi.custFreqItem.ItemDescription;
                                    break;
                                case 2:
                                    txtSearchLiqour.Text = cfi.custFreqItem.ItemDescription;
                                    break;
                                case 3:
                                    txtSearchMeal.Text = cfi.custFreqItem.ItemDescription;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void lBox_TablesSeats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {
                // get customr name
                DataRowView row = lBox_TablesSeats.SelectedItem as DataRowView;
                customerID = row["CustomerID"].ToString();

                // disable
                lBox_VIP.IsEnabled = false;
                
                txtUnkowCust.IsEnabled = false;

                // enable
                txtSearchBeer.IsEnabled = true;
                lBox_Beer.IsEnabled = true;

                txtSearchLiqour.IsEnabled = true;
                lBox_Liqour.IsEnabled = true;

                txtSearchMeal.IsEnabled = true;
                lBox_Meal.IsEnabled = true;
            }
        }

        private void txtUnkowCust_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {
                // disable
                lBox_VIP.IsEnabled = false;
                lBox_TablesSeats.IsEnabled = false;

                // enable
                txtSearchBeer.IsEnabled = true;
                lBox_Beer.IsEnabled = true;

                txtSearchLiqour.IsEnabled = true;
                lBox_Liqour.IsEnabled = true;

                txtSearchMeal.IsEnabled = true;
                lBox_Meal.IsEnabled = true;
            }
        }
        #endregion

        #region TEXTBOX GOTFOCUS
        private void txtSearchVIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                txtSearchVIP.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtSearchBeer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(1);
                alphaKey.ShowDialog();
                txtSearchBeer.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyBeer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtQtyBeer.Text = numKey.numKeyed;
            }
        }
        private void txtSearchLiqour_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(2);
                alphaKey.ShowDialog();
                txtSearchLiqour.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyLiqour_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtQtyLiqour.Text = numKey.numKeyed;
            }
        }
        private void txtSearchMeal_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(3);
                alphaKey.ShowDialog();
                txtSearchMeal.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyMeal_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtQtyMeal.Text = numKey.numKeyed;
            }
        }
        #endregion

        #region SEARCH INTO LISTBOX
        private void txtSearchVIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchVIP.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from vip in lstVIP
                              let ename = vip.CustomerID
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select vip;

            lBox_VIP.ItemsSource = empFiltered;
        }
        private void txtSearchBeer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchBeer.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from item in lstBeer
                              let ename = item.ItemDescription
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select item;

            lBox_Beer.ItemsSource = empFiltered;
        }
        private void txtSearchLiqour_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchLiqour.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from item in lstLiqour
                              let ename = item.ItemDescription
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select item;

            lBox_Liqour.ItemsSource = empFiltered;
        }
        private void txtSearchMeal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchMeal.Text;
            string upper = txtOrig.ToUpper();
            string lower = txtOrig.ToLower();

            var empFiltered = from item in lstMeal
                              let ename = item.ItemDescription
                              where ename.StartsWith(lower) || ename.StartsWith(upper) || ename.Contains(txtOrig)
                              select item;

            lBox_Meal.ItemsSource = empFiltered;
        }
        #endregion
        private void chkBox_PrintOrder(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintOrder == false)
                Settings.Default.PrintOrder = true;
            else
                Settings.Default.PrintOrder = false;

            Settings.Default.Save();
        }

    }
}