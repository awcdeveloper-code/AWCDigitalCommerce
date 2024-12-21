using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ucUpdateTicket.xaml
    /// </summary>
    public partial class ucUpdateTicket : UserControl
    {
        #region GLOBAL VARIABLES
        private wpfMainWindow mw;
        public delegate void OnTicketDataEvent(object sender, clsTicket data);
        public event OnTicketDataEvent ClickTicketData;

        public delegate void OnTicketDetailDataEvent(object sender, clsTicketDetail data);
        public event OnTicketDetailDataEvent ClickTicketDetailData;

        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        private string customerID = string.Empty;
        private bool btn_CleanOrderClicked = false;
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private List<clsItem> lstBeer = new List<clsItem>();
        private List<clsItem> lstLiqour = new List<clsItem>();
        private List<clsItem> lstMeal = new List<clsItem>();
        private bool custFOC = false;
        private TabItem ticketDetail;
        #endregion

        //  MESSAGES
        private string lang = string.Empty;
        public string strValueCannotBeZero = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strCustomerIDNotFound = string.Empty;
        public string strTickedUpdated = string.Empty;
        public string strERRORsavingTck = string.Empty;

        public ucUpdateTicket(wpfMainWindow _mw, string _lang, TabItem _ticketDetail)
        {
            mw = _mw;
            lang = _lang;
            ticketDetail = _ticketDetail;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            ticketDetail.IsEnabled = false;

            ClickTicketData += new ucUpdateTicket.OnTicketDataEvent(Subscribe_TicketEvent);
            ClickTicketDetailData += new ucUpdateTicket.OnTicketDetailDataEvent(Subscribe_TicketDetailEvent);

            lstVIP = DB.ListBinding_tbl_CustomerID(3, 1);
            lBox_CustomerID.ItemsSource = lstVIP;

            lstBeer = DB.ListBinding_tbl_Items(1);      // beverages
            lBox_Beer.ItemsSource = lstBeer;

            lstLiqour = DB.ListBinding_tbl_Items(2);    // liqours
            lBox_Liqour.ItemsSource = lstLiqour;

            lstMeal = DB.ListBinding_tbl_Items(3);      // meals
            lBox_Meal.ItemsSource = lstMeal;

            if (Settings.Default.PrintOrder)
                PrintOrder.IsChecked = true;

            CleanAll(false);
        }

        #region UTILITIES
        private void Subscribe_TicketEvent(object sender, clsTicket data)
        {
            ucNewTicketDetail.ReceiveDataFromNewTicket(data);
        }
        private void Subscribe_TicketDetailEvent(object sender, clsTicketDetail data)
        {
            ucNewTicketDetail.ReceiveDataFromNewTicketDetail(data);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void CleanAll(bool cleanDG)
        {
            btn_CleanOrderClicked = true;

            if (cleanDG)
                ucNewTicketDetail.CleanTicketDetailDataGrid();

            itemsDetails.Clear();

            lBox_Beer.IsEnabled = false;
            lBox_Beer.UnselectAll();

            lBox_Liqour.IsEnabled = false;
            lBox_Liqour.UnselectAll();

            lBox_Meal.IsEnabled = false;
            lBox_Meal.UnselectAll();

            txtSearchBeer.IsEnabled = false;
            txtSearchBeer.Text = string.Empty;
            txtQtyBeer.Text = string.Empty;

            txtSearchLiqour.IsEnabled = false;
            txtSearchLiqour.Text = string.Empty;
            txtQtyLiqour.Text = string.Empty;

            txtSearchMeal.IsEnabled = false;
            txtSearchMeal.Text = string.Empty;
            txtQtyMeal.Text = string.Empty;

            txtQtyBeer.IsEnabled = false;
            AddBeer.IsEnabled = false;

            txtQtyLiqour.IsEnabled = false;
            AddLiqour.IsEnabled = false;

            txtQtyMeal.IsEnabled = false;
            AddMeal.IsEnabled = false;

            CleanOrder.IsEnabled = false;
            TakeOrder.IsEnabled = false;

            btn_CleanOrderClicked = false;

            ticketDetail.IsEnabled = false;

            mw.UpdateTicket.IsEnabled = true;
        }
        #endregion

        #region TEXTCHANGED
        private void lBox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsCustomerVIP tmp = (clsCustomerVIP)lBox_CustomerID.SelectedItem;
            customerID = tmp.CustomerID;

            // get customer profile
            clsCustomerVIP custProf = DB.GetCustomerProfile(customerID);
            custFOC = custProf.CustomerFOC;

            // update TicketDetail datagrid
            ucNewTicketDetail.CleanTicketDetailDataGrid();
            UpdateTicketDetailDataGrid(e, custProf);

            ticketDetail.IsEnabled = true;

            txtSearchBeer.IsEnabled = true;
            lBox_Beer.IsEnabled = true;

            txtSearchLiqour.IsEnabled = true;
            lBox_Liqour.IsEnabled = true;

            lBox_Meal.IsEnabled = true;
            txtSearchMeal.IsEnabled = true;

            CleanOrder.IsEnabled = true;
            TakeOrder.IsEnabled = true;

            // show customer frequent items
            if (Settings.Default.ShowCustFreqItemsPopUp && custProf.Type == 1)
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
        private void UpdateTicketDetailDataGrid(SelectionChangedEventArgs e, clsCustomerVIP custProf)
        {
            try
            {
                int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, DB.GetIDByCustomerID(custProf.CustomerID));
                string GUID = DB.GetTicketGUID(ticketNum);
                clsTicket ticket = DB.GetTicket(ticketNum);

                Helper.ShareTicketAndCustomerID(ticket, custProf.CustomerID);

                if (Settings.Default.AllowTicketSummary)
                    itemdg = DB.GetItemsByGUID(GUID, true);
                else
                    itemdg = DB.GetItemsByGUID(GUID, false);

                foreach (clsItemDetailForDatagrid itemDetail in itemdg)
                {
                    clsTicketDetail ntd = new clsTicketDetail();

                    ntd.ItemID = itemDetail.ItemID;
                    ntd.ItemDesc = itemDetail.ItemDesc;
                    ntd.GUID = GUID;
                    ntd.Qty = itemDetail.Qty;
                    ntd.UnitCost = itemDetail.UnitCost;
                    ntd.TotalCost = ntd.UnitCost * ntd.Qty;
                    ntd.UnitPrice = itemDetail.UnitPrice;
                    ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

                    ClickTicketDetailData(e, ntd);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
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
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

                // if the customer is Free Of Charge
                if (custFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.TotalPrice = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                mw.UpdateTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();

            lBox_Beer.UnselectAll();
            txtQtyBeer.Text = string.Empty;
            txtQtyBeer.IsEnabled = false;
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
                clsItem tmp = (clsItem)lBox_Liqour.SelectedItem;
                clsTicketDetail ntd = new clsTicketDetail();

                ntd.ItemDesc = tmp.ItemDescription;
                ntd.ItemID = DB.GetIDByItemDescription(ntd.ItemDesc);
                ntd.Qty = Convert.ToInt32(txtQtyLiqour.Text.Trim());
                ntd.UnitPrice = DB.GetUnitPriceByItemDescription(ntd.ItemDesc);
                ntd.TotalPrice = ntd.UnitPrice * ntd.Qty;

                // if the customer is Free Of Charge
                if (custFOC)
                {
                    ntd.UnitPrice = 0;
                    ntd.TotalPrice = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                mw.UpdateTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();

            lBox_Liqour.UnselectAll();
            txtQtyLiqour.Text = string.Empty;
            txtQtyLiqour.IsEnabled = false;
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
                    ntd.TotalPrice = 0;
                }

                // update the ticket
                itemsDetails.Add(ntd);
                TakeOrder.IsEnabled = true;
                mw.UpdateTicket.IsEnabled = false;

                // update the datagrid
                ClickTicketDetailData(e, ntd);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();

            lBox_Meal.UnselectAll();
            txtQtyMeal.Text = string.Empty;
            txtQtyMeal.IsEnabled = false;
            AddMeal.IsEnabled = false;
        }
        #endregion

        #region ADD TICKET TO TE SYSTEM        
        private void btn_TakeOrder(object sender, RoutedEventArgs e)
        {
            // Confirm Order
            wpfConfirmOrder confirmOrder = new wpfConfirmOrder(itemsDetails);
            confirmOrder.ShowDialog();

            if (!confirmOrder.confirmed) return;

            if (Settings.Default.RequestPIN)
            {
                wpfRequestPIN wpfPIN = new wpfRequestPIN();
                wpfPIN.ShowDialog();

                clsUser userProf = DB.CheckUserPIN(wpfPIN.numKeyed);

                if (userProf.userActive == false)
                {
                    wpfMessageBox.Show("Ticket Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    CleanAll(true);
                    return;
                }
                Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
                Settings.Default.Save();
            }

            // check if the ticket already exist
            int ID = DB.GetIDByCustomerID(customerID);

            if (ID == 0)
            {
                wpfMessageBox.Show("Ticket Controller", strCustomerIDNotFound, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }
            else
            {
                // create the details
                string guidTck = DB.GetTicketGUID(Settings.Default.BusinessDate, ID, 1);

                if (DB.InsertTicketDetail(itemsDetails, guidTck, Settings.Default.WhoOpen, true))
                {
                    if (Settings.Default.PrintOrder)
                        Helper.PrintTicket(customerID, itemsDetails);

                    Helper.GetMealItemsFromTicket(ID, itemsDetails);
                }
                else
                    wpfMessageBox.Show("Ticket Controller", strERRORsavingTck, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);

                CleanAll(true);
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
            CleanAll(true);
        }
        #endregion

        #region TEXTBOX GOTFOCUS
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
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQtyBeer.Text = numKey.numKeyed;
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
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQtyLiqour.Text = numKey.numKeyed;
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
            wpfNumericKeyboard numKey = new wpfNumericKeyboard();
            numKey.ShowDialog();
            txtQtyMeal.Text = numKey.numKeyed;
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
