using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// <summary>
    /// Interaction logic for wpfNewTicketStep1.xaml
    /// </summary>
    public partial class wpfNewTicketStep1 : Window
    {
        private clsCustomerVIP custProfile = new clsCustomerVIP();
        private bool btn_CleanOrderClicked = false;
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private List<clsCustomerVIP> lstTablesSeats = new List<clsCustomerVIP>();
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
        public wpfNewTicketStep1(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            lstVIP = DB.ListBinding_tbl_CustomerID(1, 0);
            lBox_VIP.ItemsSource = lstVIP;

            lstTablesSeats = DB.ListBinding_tbl_CustomerID(2, 0);
            lBox_TablesSeats.ItemsSource = lstTablesSeats;

        }

        private void CleanAll()
        {
            btn_CleanOrderClicked = true;

            lBox_VIP.UnselectAll();
            lBox_TablesSeats.UnselectAll();

            txtNewTableSeat.Text = string.Empty;
            txtUnkowCust.Text = string.Empty;
            CustomerSelected.IsEnabled = false;

            btn_CleanOrderClicked = false;
        }

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

        private void lBox_VIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {
                btn_CleanOrderClicked = true;
                lBox_TablesSeats.UnselectAll();
                btn_CleanOrderClicked = false;

                CustomerSelected.IsEnabled = true;

                custProfile = lBox_VIP.SelectedItem as clsCustomerVIP;

                List<clsTicketsForDataGrid> custOpenTcks = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, 1, custProfile.ID);

                if (custOpenTcks.Count >= Settings.Default.MaxNumOpenTicketsPerCustomer)
                {
                    this.Opacity = 0.5;
                    wpfCustomerOpenTickets frmCustOpen = new wpfCustomerOpenTickets(custProfile.CustomerID, custOpenTcks);
                    frmCustOpen.ShowDialog();
                    this.Opacity = 1;

                    if (Settings.Default.PrintMaxNumOpenTicketsPerCustomer || frmCustOpen.printTicketsList)
                        Helper.PrintTicket(custProfile.CustomerID, custOpenTcks, 1);
                }

                int totalDebt = custOpenTcks.Sum(x => x.TotalPrice);

                if (!Settings.Default.AllowNewTicketOverCreditLimit && totalDebt > custProfile.CreditLimit)
                {
                    Helper.PrintTicket(custProfile.CustomerID, custOpenTcks, 1);
                    wpfMessageBox.Show("Tickets Controller", "CLIENTE HA EXCEDIDO SU LÍMITE DE CRÉDITO Y NO SE LE PERMITE ABRIR UNA CUENTA NUEVA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
                    return;
                }
            }
        }

        private void lBox_TablesSeats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!btn_CleanOrderClicked)
            {

                btn_CleanOrderClicked = true;
                lBox_VIP.UnselectAll();
                btn_CleanOrderClicked = false;

                CustomerSelected.IsEnabled= true;

                custProfile = lBox_TablesSeats.SelectedItem as clsCustomerVIP;

                if (Settings.Default.UseNickNames)
                {
                    wpfUseNickName unn = new wpfUseNickName(custProfile.CustomerID, true);
                    unn.ShowDialog();

                    if (string.IsNullOrEmpty(unn.nickName)) // cancel button was pressed
                    {
                        return;
                    }
                    custProfile.CustomerID = unn.nickName;
                }
            }
        }

        private void txtNewTableSeat_GotFocus(object sender, RoutedEventArgs e)
        {

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
                            if (DB.InsertNewCustomer(txtNewTableSeat.Text, 2, 1, 0, 1, 0, 0))
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

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            CleanAll();
        }

        private void btn_CustomerSelected(object sender, RoutedEventArgs e)
        {

        }

        private void txtUnkowCust_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtUnkowCust_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                if (txtUnkowCust.Text.Length > 0)
                {
                    if (!DB.CustomerIDExist(txtUnkowCust.Text))
                    {
                        if (wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerNoExist, txtUnkowCust.Text.ToUpper()), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                        {
                            int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;

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
    }
}
