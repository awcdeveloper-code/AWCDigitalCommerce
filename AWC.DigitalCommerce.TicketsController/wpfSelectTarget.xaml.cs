using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSelectTarget.xaml
    /// </summary>
    public partial class wpfSelectTarget : Window
    {
        private int _action = 0;
        private clsCustomerVIP _custProfile;
        private List<clsCustomerVIP> _lstCustomers;
        private List<clsCustomerVIP> _candidates = new List<clsCustomerVIP>();
        private string customerName = string.Empty;
        private int _ticketNumber = 0;

        public wpfSelectTarget(int action, int ticketNumber, clsCustomerVIP custProfile, List<clsCustomerVIP> lstCustomers)
        {
            _action = action;
            _ticketNumber = ticketNumber;
            _custProfile = custProfile;
            _lstCustomers = lstCustomers;
            InitializeComponent();

            if (_action == 0)
            {
                // reassign
                cbox_CustomerID.ItemsSource = DB.ListBinding_tbl_CustomerID(3, 0);
            }
            else
            {
                // inherit
                Action.Content = "HEREDAR CUENTA A:";
                _candidates = lstCustomers.FindAll(x => x.ID != custProfile.ID);
                cbox_CustomerID.ItemsSource = _candidates;
            }
        }

        private void cbox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOK.IsEnabled = true;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            clsCustomerVIP custSelected = cbox_CustomerID.SelectedItem as clsCustomerVIP;
            customerName = custSelected.CustomerID;

            if (_action == 0)
            {
                int oldCustomerID = _custProfile.ID;

                if (oldCustomerID == 0)
                {
                    string err = "ERROR: NO SE ENCONTRÓ EL ID DEL CLIENTE A SER REASIGNADO, LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                clsTicket otck = DB.GetTicket(_ticketNumber);

                clsCustomerVIP newCustProf = DB.GetCustomerProfile(customerName);

                if (newCustProf == null)
                {
                    string err = "ERROR: NO SE ENCONTRÓ EL CLIENTE A SER ASIGNADO (clsCustomerVIP), LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                DB.InserttTicketReassigned(_ticketNumber, otck.CustomerAKA, customerName);
                DB.UpdateTicketCustomerID(_ticketNumber, newCustProf.ID, customerName, newCustProf.ApplyServiceFee);
                DB.UpdateCustomerStatus(oldCustomerID, 0);
                DB.UpdateCustomerStatus(newCustProf.ID, 1);
                DB.ReassignOpenTicket(oldCustomerID, customerName);
            }
            else
            {
                int fromCustomerID = _custProfile.ID;

                int fromTicketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, fromCustomerID);

                if (fromTicketNum == 0)
                {
                    string err = "ERROR: NO SE ENCONTRÓ EL ID DE LA CUENTA A SER HEREDADA, LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                clsTicket fromTicket = DB.GetTicket(fromTicketNum);

                if (fromTicket == null)
                {
                    string err = "ERROR: NO SE ENCONTRÓ LA CUENTA A SER HEREDADA (clsTicket), LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                int ToCustomerID = custSelected.ID;

                if (ToCustomerID == 0)
                {
                    string err = "ERROR: NO SE ENCONTRÓ EL ID DEL CLIENTE QUE HEREDARÁ LA CUENTA, LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                int ToTicketNum = DB.GetTicketNumber(Settings.Default.BusinessDate, ToCustomerID);

                if (ToTicketNum == 0)
                {
                    string err = "ERROR: NO SE ENCONTRÓ EL ID DE LA CUENTA QUE HEREDARÁ, LA TRANSACCIÓN SERÁ CANCELADA.";
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                clsTicket ToTicket = DB.GetTicket(ToTicketNum);

                if (ToTicket == null)
                {
                    string err = "ERROR: NO SE ENCONTRÓ LA CUENTA QUE HEREDARÁ (clsTicket), LA TRANSACCIÓN SERÁ CANCELADA."; ;
                    wpfMessageBox.Show("Ticket Controller", err, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, err, Logger.Severity.ERROR);
                    this.Close();
                    return;
                }

                DB.InserttTicketInherited(fromTicketNum, fromTicket.CustomerAKA, ToTicket.CustomerAKA, fromTicket.GUID);
                DB.InsertTicketInheritedDetail(fromTicket.GUID);

                DB.UpdateTicketDetailGUID(fromTicket.GUID, ToTicket.GUID, 0);
                DB.CancelTicket(fromTicketNum, Settings.Default.WhoOpen, 3);
                DB.UpdateCustomerStatus(fromCustomerID, 0);
                DB.DeleteOpenTickets(fromCustomerID);
            }

            wpfSplashWindow sw = new wpfSplashWindow(1, "");
            sw.ShowDialog();
            this.Close();
        }
    }
}
