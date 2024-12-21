using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucTicketsMaintenance : UserControl
    {
        #region GLOBAL VARIABLES
        private string lang = string.Empty;
        private string customerName = string.Empty;
        private bool cleanAll = false;
        private clsCustomerVIP CustProf = new clsCustomerVIP();
        private List<clsCustomerVIP> lstVIP = new List<clsCustomerVIP>();
        private clsTicket tckOrig = new clsTicket();
        private clsTicketModified tckMod = new clsTicketModified();

        public string strCancelTicket = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strTicketCancelled = string.Empty;
        public string strReassignTicket = string.Empty;
        public string strTicketReassigned = string.Empty;
        public string strInheritTicket = string.Empty;
        public string strTicketInherited = string.Empty;
        #endregion

        public ucTicketsMaintenance(string _lang)
        {
            lang = _lang;

            InitializeComponent();
            
            Traductor.ApplyTranslation(this, lang);

            LoadInfo();
        }
        private void LoadInfo()
        {
            lstVIP = DB.ListBinding_tbl_CustomerID(3, 1);

            lBox_CustomerID.ItemsSource = lstVIP;

            cbox_CustomerID.DataContext = DB.DataBinding_tbl_CustomerID(3, 0);

            cbox_CustomerID2.DataContext = DB.DataBinding_tbl_CustomerID(3, 1);

        }      
        private void CleanAll()
        {
            cleanAll = true;
            lBox_CustomerID.UnselectAll();

            grpBox_MoveTicket.IsEnabled = false;
            cbox_CustomerID.SelectedIndex = -1;
            btnReassignTicket.IsEnabled = false;

            grpBox_InheritTicket.IsEnabled = false;
            cbox_CustomerID2.SelectedIndex = -1;
            btnInheritTicket.IsEnabled = false;

            grpBox_CancelTicket.IsEnabled = false;
            lblTicket2Cancel.Content = string.Empty;
            btnCancelTicket.IsEnabled = false;

            LoadInfo();

            cleanAll = false;
        }
        private void lBox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grpBox_MoveTicket.IsEnabled = true;
            grpBox_InheritTicket.IsEnabled = true;
            grpBox_CancelTicket.IsEnabled = true;

            if (!cleanAll)
            {
                CustProf = lBox_CustomerID.SelectedItem as clsCustomerVIP;

                cbox_CustomerID2.DataContext = DB.DataBinding_tbl_CustomerID(4, 1, CustProf.CustomerID);

                lblTicket2Cancel.Content = CustProf.CustomerID;

                btnCancelTicket.IsEnabled = true;
            }
        }
        private void cbox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnReassignTicket.IsEnabled = true;
            grpBox_InheritTicket.IsEnabled = false;
            grpBox_CancelTicket.IsEnabled = false;
        }
        private void cbox_CustomerID2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnInheritTicket.IsEnabled = true;
            grpBox_MoveTicket.IsEnabled = false;
            grpBox_CancelTicket.IsEnabled = false;
        }
        private void btn_ReassignTicket(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Tickets Controller", strReassignTicket, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                DataRowView row = cbox_CustomerID.SelectedItem as DataRowView;
                customerName = row["CustomerID"].ToString();

                int oldCustomerID = DB.GetIDByCustomerID(CustProf.CustomerID);
                int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, oldCustomerID);

                clsCustomerVIP newCustProf = DB.GetCustomerProfile(customerName);

                DB.UpdateTicketCustomerID(ticketNum, newCustProf.ID, customerName, newCustProf.ApplyServiceFee);
                DB.UpdateCustomerStatus(oldCustomerID, 0);
                DB.UpdateCustomerStatus(newCustProf.ID, 1);
                DB.ReassignOpenTicket(oldCustomerID, customerName);

                CleanAll();
            }
        }
        private void btn_InheritTicket(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Tickets Controller", strInheritTicket, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                DataRowView row = cbox_CustomerID2.SelectedItem as DataRowView;
                customerName = row["CustomerID"].ToString();

                int fromCustomerID = DB.GetIDByCustomerID(CustProf.CustomerID);
                int fromTicketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, fromCustomerID);
                clsTicket fromTicket = DB.GetTicket(fromTicketNum);

                int ToCustomerID = DB.GetIDByCustomerID(customerName);
                int ToTicketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, ToCustomerID);
                clsTicket ToTicket = DB.GetTicket(ToTicketNum);

                DB.UpdateTicketDetailGUID(fromTicket.GUID, ToTicket.GUID, 0);
                DB.CancelTicket(fromTicketNum, Settings.Default.WhoOpen, 2);
                DB.UpdateCustomerStatus(fromCustomerID, 0);
                DB.DeleteOpenTickets(fromCustomerID);

                CleanAll();
            }
        }
        private void btn_CancelTicket(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Tickets Controller", strCancelTicket, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                int ID = DB.GetIDByCustomerID(lblTicket2Cancel.Content.ToString());

                int ticketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, ID);

                DB.InsertNewTicketAborted(ticketNum);

                clsTicket tck = DB.GetTicket(ticketNum);

                DB.DeleteTicketDetail(tck.GUID, false);

                DB.DeleteOpenTickets(ID);

                DB.CancelTicket(ticketNum, Settings.Default.WhoOpen, 2);

                DB.UpdateCustomerStatus(DB.GetIDByCustomerID(lblTicket2Cancel.Content.ToString()), 0);

                CleanAll();
            }
        }
        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            CleanAll();
        }

        #region FIX TICKET
        private void CleanFixTicket()
        {
            txtTicketNum.Text = string.Empty;
            txtTicketDate.Text = string.Empty;
            txtTicketCustomerID.Text = string.Empty;
            txtTicketTotal.Text = string.Empty;
            txtTicketPayments.Text = string.Empty;
            txtTicketServiceFee.Text = string.Empty;
            txtTicketCash.Text = string.Empty;
            txtTicketCreditCard.Text = string.Empty;
            txtTicketTransfer.Text = string.Empty;
            txtTicketPayMethod.Text = string.Empty;
            txtTicketStatus.Text = string.Empty;
            txtTicketSplited.Text = string.Empty;

            txtTicketNum.IsEnabled = true;
            txtTicketDate.IsEnabled = true;
            txtTicketCustomerID.IsEnabled = true;
            txtTicketTotal.IsEnabled = true;
            txtTicketPayments.IsEnabled = true;
            txtTicketServiceFee.IsEnabled = true;
            txtTicketCash.IsEnabled = true;
            txtTicketCreditCard.IsEnabled = true;
            txtTicketTransfer.IsEnabled = true;
            txtTicketPayMethod.IsEnabled = true;
            txtTicketStatus.IsEnabled = true;
            txtTicketSplited.IsEnabled = true;

            TicketUpdate.IsEnabled = false;

            txtTicketNum.Focus();
        }
        private void btn_TicketCancel(object sender, RoutedEventArgs e)
        {
            CleanFixTicket();
        }
        private void txtTicketNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTicketNum.Text.Length == 0) return;

            int ticketNum = Convert.ToInt32(txtTicketNum.Text);

            tckOrig = DB.GetTicket(ticketNum);

            if (tckOrig.ID == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: Número de cuenta NO EXISTE. Por favor, verifique e intente de nuevo.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            txtTicketDate.Text = tckOrig.TicketDate;
            txtTicketCustomerID.Text = tckOrig.CustID.ToString();
            txtTicketTotal.Text = tckOrig.TotalPrice.ToString();
            txtTicketPayments.Text = tckOrig.Payments.ToString();
            txtTicketServiceFee.Text = tckOrig.ServiceFee.ToString();
            txtTicketCash.Text = tckOrig.Cash.ToString();
            txtTicketCreditCard.Text = tckOrig.CreditCard.ToString();
            txtTicketTransfer.Text = tckOrig.Transfer.ToString();
            txtTicketPayMethod.Text = tckOrig.PayMethod.ToString();
            txtTicketStatus.Text = tckOrig.Status.ToString();
            txtTicketSplited.Text = tckOrig.Splited.ToString();

            TicketUpdate.IsEnabled = true;
            txtTicketDate.Focus();
        }
        private void btn_TicketUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                clsTicket ticket = new clsTicket();

                ticket.ID = Convert.ToInt32(txtTicketNum.Text);
                ticket.TicketDate = txtTicketDate.Text;
                ticket.CustID = Convert.ToInt32(txtTicketCustomerID.Text);
                ticket.TotalPrice = Convert.ToInt32(txtTicketTotal.Text);
                ticket.Payments = Convert.ToInt32(txtTicketPayments.Text);
                ticket.ServiceFee = Convert.ToInt32(txtTicketServiceFee.Text);
                ticket.Cash = Convert.ToInt32(txtTicketCash.Text);
                ticket.CreditCard = Convert.ToInt32(txtTicketCreditCard.Text);
                ticket.Transfer = Convert.ToInt32(txtTicketTransfer.Text);
                ticket.PayMethod = Convert.ToInt32(txtTicketPayMethod.Text);
                ticket.Status = txtTicketStatus.Text == "True" ? true : false;
                ticket.Splited = txtTicketSplited.Text == "True" ? true : false;

                if (DB.UpdateTicket(ticket))
                {
                    tckMod = new clsTicketModified();

                    tckMod.ID = ticket.ID;
                    tckMod.oriTicketDate = tckOrig.TicketDate;
                    tckMod.oriCustID = tckOrig.CustID;
                    tckMod.oriTotalPrice = tckOrig.TotalPrice;
                    tckMod.oriPayments = tckOrig.Payments;
                    tckMod.oriServiceFee = tckOrig.ServiceFee;
                    tckMod.oriCash = tckOrig.Cash;
                    tckMod.oriCreditCard = tckOrig.CreditCard;
                    tckMod.oriTransfer = tckOrig.Transfer;
                    tckMod.oriCreateAt = tckOrig.CreateAt.ToString();

                    tckMod.modTicketDate = ticket.TicketDate;
                    tckMod.modCustID = ticket.CustID;
                    tckMod.modTotalPrice = ticket.TotalPrice;
                    tckMod.modPayments = ticket.Payments;
                    tckMod.modServiceFee = ticket.ServiceFee;
                    tckMod.modCash = ticket.Cash;
                    tckMod.modCreditCard = ticket.CreditCard;
                    tckMod.modTransfer = ticket.Transfer;

                    DB.InsertTicketModified(tckMod);
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: La cuenta fue ACTUALIZADA exitosamente.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                }
            }
            catch  (Exception ex)
            {
                wpfMessageBox.Show("Tickets Controller", ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
            finally
            {
                CleanFixTicket();
            }
        }
        #endregion
    }
}
