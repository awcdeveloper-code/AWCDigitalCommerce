using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfTicketsAborted.xaml
    /// </summary>
    public partial class wpfTicketsAborted : Window
    {
        private string startDate = string.Empty;
        private string finishDate = string.Empty;

        private List<clsTicket> ticketsList = new List<clsTicket>();
        private List<clsItemDetailForDatagrid> ticketDetails = new List<clsItemDetailForDatagrid>();
        public wpfTicketsAborted()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
        }

        private void TicketsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TicketsList.SelectedIndex == -1)
                return;

            clsTicket tck = TicketsList.SelectedItem as clsTicket;

            clsUser checkPIN = DB.CheckUserPIN(tck.WhoOpened.ToString());
            WhoRequest.Content = checkPIN.userName;
            checkPIN = DB.CheckUserPIN(tck.WhoClosed.ToString());
            WhoApproved.Content = checkPIN.userName;
            TransactionDateTime.Content = tck.CloseAt.ToString("dd/MM/yyyy HH:mm:ss");

            AbortReason.Content = "RAZÓN: " + tck.AbortReason;

            ticketDetails = Settings.Default.AllowTicketSummary ? DB.GetItemsAbortedByGUID(tck.GUID, true) : DB.GetItemsAbortedByGUID(tck.GUID, false);

            TicketDetails.ItemsSource = ticketDetails;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = StartDate.SelectedDate.ToString();

            if (startDate.Length == 0) return;

            string year = startDate.Split('/')[2].Substring(0, 4);
            string month = startDate.Split('/')[1].PadLeft(2, '0');
            string day = startDate.Split('/')[0].PadLeft(2, '0');

            startDate = year + month + day;
        }

        private void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            finishDate = FinishDate.SelectedDate.ToString();

            if (finishDate.Length == 0) return;

            string year = finishDate.Split('/')[2].Substring(0, 4);
            string month = finishDate.Split('/')[1].PadLeft(2, '0');
            string day = finishDate.Split('/')[0].PadLeft(2, '0');

            finishDate = year + month + day;

            if (Convert.ToInt32(startDate) > Convert.ToInt32(finishDate))
            {
                wpfMessageBox.Show("Tickets Controller", "ERROR: FECHA INICIAL NO PUEDE SER MAYOR QUE LA FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            ticketsList = DB.ListBinding_tbl_TicketsAborted(startDate, finishDate);

            if (ticketsList.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: El rango de fechas seleccionado NO contiene información", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                return;
            }

            TicketsList.ItemsSource = ticketsList;
        }
    }
}
