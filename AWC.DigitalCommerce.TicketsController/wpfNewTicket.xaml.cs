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

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfNewTicket : Window
    {
        #region GLOBAL VARIABLES
        public delegate void OnTicketDetailDataEvent(object sender, clsTicketDetail data);
        public event OnTicketDetailDataEvent ClickTicketDetailData;

        public delegate void OnMainWindowDataEvent(object sender, int source);

        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private bool btn_CleanOrderClicked = false;
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private List<clsCustomerVIP> lstTablesSeats = new List<clsCustomerVIP>();
        private List<clsItem> lstBeer = new List<clsItem>();
        private List<clsItem> lstLiqour = new List<clsItem>();
        private List<clsItem> lstMeal = new List<clsItem>();
        public bool newTicket = false;
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
        public string strCreditLimitExceeded = string.Empty;
        #endregion

        public wpfNewTicket(string _lang)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            ClickTicketDetailData += new wpfNewTicket.OnTicketDetailDataEvent(Subscribe_Event);

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_VIP.ItemsSource = lstVIP;

            lstTablesSeats = DB.ListBinding_tbl_CustomerID(2, 0);
            lBox_TablesSeats.ItemsSource = lstTablesSeats;

            lstBeer = DB.ListBinding_tbl_Items(1);      // Beverages
            lBox_Beer.ItemsSource = lstBeer;

            lstLiqour = DB.ListBinding_tbl_Items(2);    // Liqours
            lBox_Liqour.ItemsSource = lstLiqour;

            lstMeal = DB.ListBinding_tbl_Items(3);      // Meals
            lBox_Meal.ItemsSource = lstMeal;

            ApplyServiceFee.IsChecked = Settings.Default.ApplyServiceFee ? true : false;
            ApplyServiceFee.Visibility = Settings.Default.ApplyServiceFee ? Visibility.Visible : Visibility.Hidden;
            PrintOrder.IsChecked = Settings.Default.PrintOrder ? true : false;

            // disable
            txtSearchBeer.IsEnabled = false;
            lBox_Beer.IsEnabled = false;
            txtQtyBeer.IsEnabled = false;

            txtSearchLiqour.IsEnabled = false;
            lBox_Liqour.IsEnabled = false;
            txtQtyLiqour.IsEnabled = false;

            txtSearchMeal.IsEnabled = false;
            lBox_Meal.IsEnabled = false;
            txtQtyMeal.IsEnabled = false;

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
        private void txtNewTableSeat_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                if (txtNewTableSeat.Text.Length > 0)
                {
                    if (!DB.CustomerIDExist(txtNewTableSeat.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtNewTableSeat.Text.ToUpper()), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

                            if (DB.InsertNewCustomer(txtNewTableSeat.Text, 2, 1, 0, serviceFee, 0, 0))
                            {
                                lstTablesSeats = DB.ListBinding_tbl_CustomerID(2, 0);
                                lBox_TablesSeats.ItemsSource = lstTablesSeats;
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
        private void txtUnkowCust_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                if (txtUnkowCust.Text.Length > 0)
                {
                    txtUnkowCust.Text.ToUpper();

                    if (!DB.CustomerIDExist(txtUnkowCust.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtUnkowCust.Text.ToUpper()), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

                            if (DB.InsertNewCustomer(txtUnkowCust.Text, 1, 0, 0, serviceFee, 0, Settings.Default.CreditLimitByDefault))
                            {
                                string newVIP = txtUnkowCust.Text;
                                lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
                                lBox_VIP.ItemsSource = lstVIP;
                                CleanAll();

                                // select new VIP in the listbox
                                txtSearchVIP.Text = newVIP;
                                lBox_VIP.SelectedIndex = 0;
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
            lblWindowHeader.Content = "SELECCIONE A SU CLIENTE";

            VIPGroupBox.Visibility = Visibility.Visible;
            TablesSeatsGroupBox.Visibility = Visibility.Visible;
            NewTableGroupBox.Visibility = Visibility.Visible;

            btn_CleanOrderClicked = true;
            Close.IsEnabled = true;

            itemsDetails.Clear();

            lBox_VIP.UnselectAll();
            lBox_VIP.IsEnabled = true;

            lBox_TablesSeats.UnselectAll();
            lBox_TablesSeats.IsEnabled = true;

            txtNewTableSeat.Text = string.Empty;
            txtNewTableSeat.IsEnabled = true;

            txtUnkowCust.Text = string.Empty;
            txtUnkowCust.IsEnabled = true;

            lBox_Beer.UnselectAll();
            lBox_Beer.IsEnabled = false;

            lBox_Liqour.UnselectAll();
            lBox_Liqour.IsEnabled = false;

            lBox_Meal.UnselectAll();
            lBox_Meal.IsEnabled = false;

            txtSearchVIP.Text = string.Empty;

            txtSearchBeer.Text = string.Empty;
            txtSearchBeer.IsEnabled = false;
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
            FreqItems.IsEnabled = false;

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_VIP.ItemsSource = lstVIP;
            lBox_TablesSeats.DataContext = DB.DataBinding_tbl_CustomerID(2, 0);

            ApplyServiceFee.IsChecked = Settings.Default.ApplyServiceFee ? true : false;
            PrintOrder.IsChecked = Settings.Default.PrintOrder ? true : false;

            btn_CleanOrderClicked = false;

            this.Opacity = 1;
        }
        private void SetUserAccessToResources()
        {
            //NewCustomerGroupBox.IsEnabled = Helper.CheckUserAccessToResource("NewCustomer");
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
            txtQtyLiqour.Text = "1";
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
                ntd.ItemSubType = tmp.ItemSubType;
                ntd.Qty = Convert.ToInt32(txtQtyBeer.Text.Trim());
                ntd.UnitCost = tmp.UnitCost;
                ntd.TotalCost = ntd.UnitCost * ntd.Qty;
                ntd.UnitPrice = tmp.UnitPrice;
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;
                ntd.Bucket = false;

                if (DB.GetItemSubtype(tmp.ItemDescription) == 2)
                {
                    this.Opacity = 0.5;
                    wpfSelectBucketContent mn = new wpfSelectBucketContent(ntd.ItemID);
                    mn.ShowDialog();
                    this.Opacity = 1;
                    ntd.Note = mn.bucketContent;
                    ntd.Bucket = true;
                }

                // if the customer is Free Of Charge
                if (custProfile.CustomerFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Beer.UnselectAll();
            lBox_Beer.SelectionMode = SelectionMode.Single;
            //txtQtyBeer.Text = string.Empty;
            //txtQtyBeer.IsEnabled = false;
            //txtSearchBeer.Text = string.Empty;
            AddBeer.IsEnabled = false;
            Close.IsEnabled = false;
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
                clsItem tmp = (clsItem)lBox_Liqour.SelectedItem;
                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = tmp.ItemDescription;
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.ItemSubType = tmp.ItemSubType;
                ntd.Qty = Convert.ToInt32(txtQtyLiqour.Text.Trim());
                ntd.UnitCost = tmp.UnitCost;
                ntd.TotalCost = ntd.UnitCost * ntd.Qty;
                ntd.UnitPrice = tmp.UnitPrice;
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

                // if the customer is Free Of Charge
                if (custProfile.CustomerFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Liqour.UnselectAll();
            lBox_Liqour.SelectionMode = SelectionMode.Single;
            //txtQtyLiqour.Text = string.Empty;
            //txtQtyLiqour.IsEnabled = false;
            //txtSearchLiqour.Text = string.Empty;
            AddLiqour.IsEnabled = false;
            Close.IsEnabled = false;
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
                ntd.UnitCost = tmp.UnitCost;
                ntd.TotalCost = ntd.UnitCost * ntd.Qty;
                ntd.UnitPrice = tmp.UnitPrice;
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

                this.Opacity = 0.5;
                wpfMealNote mn = new wpfMealNote(tmp.ItemDescription);
                mn.ShowDialog();
                this.Opacity = 1;
                
                ntd.Note = mn.mealNote;

                // if the customer is Free Of Charge
                if (custProfile.CustomerFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.UnitCost = 0;
                    ntd.TotalPrice = 0;
                    ntd.TotalCost = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            lBox_Meal.UnselectAll();
            lBox_Meal.SelectionMode = SelectionMode.Single;
            //txtQtyMeal.Text = string.Empty;
            //txtQtyMeal.IsEnabled = false;
            //txtSearchMeal.Text = string.Empty;
            AddMeal.IsEnabled = false;
            Close.IsEnabled = false;
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
            ticket.CustID = custProfile.ID;
            ticket.TotalPrice = 0;
            ticket.Status = true;
            ticket.ApplyServiceFee = custProfile.ApplyServiceFee;
            ticket.CustomerAKA = custProfile.CustomerID;
            ticket.Shift = 0;

            int tnum = 0;
            tnum = DB.InsertNewTicket(ticket, Settings.Default.WhoOpen);

            if (tnum > 0)
            {
                // create the details
                if (DB.InsertTicketDetail(itemsDetails, ticket.GUID, Settings.Default.WhoOpen, true))
                {
                    DB.InsertNewOpenTicket(custProfile);

                    if (Settings.Default.PrintOrder)
                        Helper.PrintTicket(custProfile.CustomerID, itemsDetails);

                    Helper.GetMealItemsFromTicket(custProfile.ID, itemsDetails);
                }
                else
                {
                    wpfMessageBox.Show("Ticket Controller", strERRORsavingTckDet, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    return;
                }
            }
            else
            {
                wpfMessageBox.Show("Ticket Controller", strERRORsavingTck, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            newTicket = true;
            this.Close();
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
                custProfile = lBox_VIP.SelectedItem as clsCustomerVIP;

                lblWindowHeader.Content = $"ABRIENDO CUENTA PARA {custProfile.CustomerID}";

                List<clsTicketsForDataGrid> custOpenTcks = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, 1, custProfile.ID);

                if (custOpenTcks.Count >= Settings.Default.MaxNumOpenTicketsPerCustomer)
                {
                    this.Opacity = 0.5;
                    wpfCustomerOpenTickets frmCustOpen = new wpfCustomerOpenTickets(custProfile.CustomerID, custOpenTcks);
                    frmCustOpen.ShowDialog();
                    this.Opacity = 1;

                    if (Settings.Default.PrintMaxNumOpenTicketsPerCustomer || frmCustOpen.printTicketsList)
                        Helper.PrintTicket(custProfile.CustomerID, custOpenTcks, 1);

                    if (Settings.Default.SendAlert2AdminAboutVIP)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        SMTP.SendAlert2AdminAboutVIP(custProfile.CustomerID, custOpenTcks);
                        Mouse.OverrideCursor = null;
                    }
                }

                int totalDebt = custOpenTcks.Sum(x => x.TotalPrice);

                if (!Settings.Default.AllowNewTicketOverCreditLimit && totalDebt > custProfile.CreditLimit)
                {
                    Helper.PrintTicket(custProfile.CustomerID, custOpenTcks, 1);
                    wpfMessageBox.Show("Tickets Controller", "CLIENTE HA EXCEDIDO SU LÍMITE DE CRÉDITO Y NO SE LE PERMITE ABRIR UNA CUENTA NUEVA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                // disable
                TablesSeatsGroupBox.Visibility = Visibility.Hidden;
                NewTableGroupBox.Visibility = Visibility.Hidden;

                //lBox_TablesSeats.IsEnabled = false;
                //txtNewTableSeat.IsEnabled = false;
                //txtUnkowCust.IsEnabled = false;

                // enable
                txtSearchBeer.IsEnabled = true;
                lBox_Beer.IsEnabled = true;

                txtSearchLiqour.IsEnabled = true;
                lBox_Liqour.IsEnabled = true;

                txtSearchMeal.IsEnabled = true;
                lBox_Meal.IsEnabled = true;

                // enabled to create empty ticket
                //TakeOrder.IsEnabled = true;

                // show customer frequent items
                if (Settings.Default.ShowCustFreqItemsPopUp)
                {
                    List<clsCustFreqItem> custFreqItemsList = DB.GetCustomerFrequentItems(custProfile.ID);

                    // if the customer has history
                    if (custFreqItemsList.Count > 0)
                    {
                        this.Opacity = 0.5;
                        wpfCustFreqItems cfi = new wpfCustFreqItems(custFreqItemsList);
                        cfi.ShowDialog();
                        this.Opacity = 1;

                        if (cfi.itemSelected)
                        {
                            switch (cfi.custFreqItem.ItemType)
                            {
                                case 1:
                                    txtSearchBeer.Text = cfi.custFreqItem.ItemDescription;
                                    txtQtyBeer.IsEnabled = true;
                                    txtQtyBeer.Text = "1";
                                    AddBeer.IsEnabled = true;
                                    lBox_Beer.SelectionMode= SelectionMode.Extended;
                                    lBox_Beer.SelectAll();
                                    break;
                                case 2:
                                    txtSearchLiqour.Text = cfi.custFreqItem.ItemDescription;
                                    txtQtyLiqour.IsEnabled = true;
                                    txtQtyLiqour.Text = "1";
                                    AddBeer.IsEnabled = true;
                                    lBox_Liqour.SelectionMode = SelectionMode.Extended;
                                    lBox_Liqour.SelectAll();
                                    break;
                                case 3:
                                    txtSearchMeal.Text = cfi.custFreqItem.ItemDescription;
                                    txtQtyMeal.IsEnabled = true;
                                    txtQtyMeal.Text = "1";
                                    AddBeer.IsEnabled = true;
                                    lBox_Meal.SelectionMode = SelectionMode.Extended;
                                    lBox_Meal.SelectAll();
                                    break;
                            }
                            FreqItems.IsEnabled = true;
                        }
                    }
                }
            }
        }
        private void lBox_TablesSeats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {
                custProfile = lBox_TablesSeats.SelectedItem as clsCustomerVIP;

                if (Settings.Default.UseNickNames)
                {
                    wpfUseNickName unn = new wpfUseNickName(custProfile.CustomerID, true);
                    unn.ShowDialog();

                    if (string.IsNullOrEmpty(unn.nickName)) // cancel button was pressed
                    {
                        CleanAll();
                        return;
                    }
                    custProfile.CustomerID = unn.nickName;
                }

                lblWindowHeader.Content = $"ABRIENDO CUENTA PARA {custProfile.CustomerID}";

                // disable
                VIPGroupBox.Visibility = Visibility.Hidden;
                NewTableGroupBox.Visibility = Visibility.Hidden;

                //lBox_VIP.IsEnabled = false;
                //txtNewTableSeat.IsEnabled = false;
                //txtUnkowCust.IsEnabled = false;

                // enable
                txtSearchBeer.IsEnabled = true;
                lBox_Beer.IsEnabled = true;

                txtSearchLiqour.IsEnabled = true;
                lBox_Liqour.IsEnabled = true;

                txtSearchMeal.IsEnabled = true;
                lBox_Meal.IsEnabled = true;

                // enabled to create empty ticket
                //TakeOrder.IsEnabled = true;
            }
        }
        #endregion

        #region TEXTBOX GOTFOCUS
        private void txtSearchVIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchVIP.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtUnkowCust_GotFocus(object sender, RoutedEventArgs e)
        {
            VIPGroupBox.Visibility = Visibility.Hidden;
            TablesSeatsGroupBox.Visibility = Visibility.Hidden;

            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                this.Opacity = 1;

                txtUnkowCust.Text = alphaKey.alphaKeyed.ToUpper();

                if (txtUnkowCust.Text.Length > 0)
                {
                    if (!DB.CustomerIDExist(txtUnkowCust.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtUnkowCust.Text), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

                            if (DB.InsertNewCustomer(txtUnkowCust.Text, 1, 0, 0, serviceFee, 0, Settings.Default.CreditLimitByDefault))
                            {
                                string newVIP = txtUnkowCust.Text;
                                lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
                                lBox_VIP.ItemsSource = lstVIP;
                                CleanAll();

                                // select new VIP in the listbox
                                txtSearchVIP.Text = newVIP;
                                lBox_VIP.SelectedIndex = 0;
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
        private void txtNewTableSeat_GotFocus(object sender, RoutedEventArgs e)
        {
            VIPGroupBox.Visibility = Visibility.Hidden;
            TablesSeatsGroupBox.Visibility = Visibility.Hidden;

            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(0);
                alphaKey.ShowDialog();
                this.Opacity = 1;

                txtNewTableSeat.Text = alphaKey.alphaKeyed;

                if (txtNewTableSeat.Text.Length > 0)
                {
                    if (!DB.CustomerIDExist(txtNewTableSeat.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtNewTableSeat.Text.ToUpper()), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

                            if (DB.InsertNewCustomer(txtNewTableSeat.Text, 2, 1, 0, serviceFee, 0, 0))
                            {
                                lstTablesSeats = DB.ListBinding_tbl_CustomerID(2, 0);
                                lBox_TablesSeats.ItemsSource = lstTablesSeats;
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
        private void txtSearchBeer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(1);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchBeer.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyBeer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                this.Opacity = 1;
                txtQtyBeer.Text = numKey.numKeyed;
            }
        }
        private void txtSearchLiqour_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(2);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchLiqour.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyLiqour_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                this.Opacity = 1;
                txtQtyLiqour.Text = numKey.numKeyed;
            }
        }
        private void txtSearchMeal_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualAlphaKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfAlphaKeyboard alphaKey = new wpfAlphaKeyboard(3);
                alphaKey.ShowDialog();
                this.Opacity = 1;
                txtSearchMeal.Text = alphaKey.alphaKeyed;
            }
        }
        private void txtQtyMeal_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                this.Opacity = 0.5;
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                this.Opacity = 1;
                txtQtyMeal.Text = numKey.numKeyed;
            }
        }

        #endregion

        #region SEARCH INTO LISTBOX
        private void txtSearchVIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchVIP.Text.ToUpper();

            var empFiltered = from vip in lstVIP
                              let ename = vip.CustomerID
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
                              select vip;

            lBox_VIP.ItemsSource = empFiltered;
        }
        private void txtSearchBeer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchBeer.Text.ToUpper();

            var empFiltered = from item in lstBeer
                              let ename = item.ItemDescription
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
                              select item;

            lBox_Beer.ItemsSource = empFiltered;
        }
        private void txtSearchLiqour_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchLiqour.Text.ToUpper();

            var empFiltered = from item in lstLiqour
                              let ename = item.ItemDescription
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
                              select item;

            lBox_Liqour.ItemsSource = empFiltered;
        }
        private void txtSearchMeal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtOrig = txtSearchMeal.Text.ToUpper();

            var empFiltered = from item in lstMeal
                              let ename = item.ItemDescription
                              where ename.StartsWith(txtOrig) || ename.Contains(txtOrig) || ename.EndsWith(txtOrig)
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
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_FreqItems(object sender, RoutedEventArgs e)
        {
            List<clsCustFreqItem> custFreqItemsList = DB.GetCustomerFrequentItems(custProfile.ID);

            // if the customer has history
            if (custFreqItemsList.Count > 0)
            {
                this.Opacity = 0.5;
                wpfCustFreqItems cfi = new wpfCustFreqItems(custFreqItemsList);
                cfi.ShowDialog();
                this.Opacity = 1;

                if (cfi.itemSelected)
                {
                    switch (cfi.custFreqItem.ItemType)
                    {
                        case 1:
                            txtSearchBeer.Text = cfi.custFreqItem.ItemDescription;
                            txtQtyBeer.IsEnabled = true;
                            txtQtyBeer.Text = "1";
                            AddBeer.IsEnabled = true;
                            lBox_Beer.SelectionMode = SelectionMode.Extended;
                            lBox_Beer.SelectAll();
                            break;
                        case 2:
                            txtSearchLiqour.Text = cfi.custFreqItem.ItemDescription;
                            txtQtyLiqour.IsEnabled = true;
                            txtQtyLiqour.Text = "1";
                            AddBeer.IsEnabled = true;
                            lBox_Liqour.SelectionMode = SelectionMode.Extended;
                            lBox_Liqour.SelectAll();
                            break;
                        case 3:
                            txtSearchMeal.Text = cfi.custFreqItem.ItemDescription;
                            txtQtyMeal.IsEnabled = true;
                            txtQtyMeal.Text = "1";
                            AddBeer.IsEnabled = true;
                            lBox_Meal.SelectionMode = SelectionMode.Extended;
                            lBox_Meal.SelectAll();
                            break;
                    }
                    FreqItems.IsEnabled = true;
                }
            }
        }

        private void SelectVIPItem(string string2Select)
        {
            for (int i = 0; i < lBox_VIP.Items.Count; i++)
            {
                if (lBox_VIP.Items[i].ToString().Equals(string2Select, StringComparison.OrdinalIgnoreCase))
                {
                    lBox_VIP.SelectedIndex = i;
                    break;
                }
            }
        }
    }
}