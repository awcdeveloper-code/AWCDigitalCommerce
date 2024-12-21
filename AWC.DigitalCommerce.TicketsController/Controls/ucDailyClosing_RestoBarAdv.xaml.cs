using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucDailyClosing_RestoBarAdv : UserControl
    {
        private string lang = string.Empty;
        private string workDay1 = string.Empty;
        private string workDay2 = string.Empty;
        private List<clsTicketsForDataGrid> itemsList;
        private List<clsSmallPayment> smPaymentsList;
        private clsDailyClosing dcReport = new clsDailyClosing();
        private List<clsTicketsForDataGrid> itemsListSorted = new List<clsTicketsForDataGrid>();
        public ucDailyClosing_RestoBarAdv(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            SendReportByEmail.IsEnabled = SMTP.CheckInternetConnection();
            PrintSummaryWithDetail.IsChecked = Settings.Default.PrintSummaryWithDetail;
        }

        private void SelectedDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            workDay1 = SelectedDay1.SelectedDate.ToString();
            workDay2 = SelectedDay2.SelectedDate.ToString();

            if (workDay1.Length == 0 || workDay2.Length == 0) return;

            string year1 = workDay1.Split('/')[2].Substring(0, 4);
            string month1 = workDay1.Split('/')[1].PadLeft(2, '0');
            string day1 = workDay1.Split('/')[0].PadLeft(2, '0');

            workDay1 = year1 + month1 + day1;

            string year2 = workDay2.Split('/')[2].Substring(0, 4);
            string month2 = workDay2.Split('/')[1].PadLeft(2, '0');
            string day2 = workDay2.Split('/')[0].PadLeft(2, '0');

            workDay2 = year2 + month2 + day2;

            if (Convert.ToInt32(workDay2) < Convert.ToInt32(workDay1))
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: LA FECHA FINAL NO PUEDE SER MENOR QUE LA FECHA INICIAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                return;
            }

            clsTicket ticketSummary = DB.GetTicketsSummary(workDay1, workDay2);
            clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(workDay1, workDay2);

            ticketSummary.Cash += smlPay.Cash;
            ticketSummary.CreditCard += smlPay.CreditCard;
            ticketSummary.Transfer += smlPay.Transfer;

            int totalPrice = ticketSummary.Cash +
                             ticketSummary.CreditCard +
                             ticketSummary.Transfer +
                             ticketSummary.Payments; // Outstanding (not Payments really)

            lbl_OutstandingAmount.Content = ticketSummary.Payments.ToString("N0");
            lbl_Cash.Content = ticketSummary.Cash.ToString("N0");
            lbl_CreditCard.Content = ticketSummary.CreditCard.ToString("N0");
            lbl_Transfer.Content = ticketSummary.Transfer.ToString("N0");
            lbl_ServiceFee.Content = ticketSummary.ServiceFee.ToString("N0");
            lbl_TotalSale.Content = totalPrice.ToString("N0");

            dcReport.AccountsReceivable = ticketSummary.Payments;
            dcReport.Cash = ticketSummary.Cash;
            dcReport.CreditCard = ticketSummary.CreditCard;
            dcReport.Transfer = ticketSummary.Transfer;
            dcReport.ServiceFee = ticketSummary.ServiceFee;
            dcReport.GrossSale = totalPrice;

            itemsList = DB.DataBinding_tbl_DailyClose(workDay1, workDay2);

            if (itemsList.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: LA FECHAS SELECCIONADAS NO CONTIENEN INFORMACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                return;
            }

            smPaymentsList = DB.GetSmallPayments(workDay1, workDay2);

            foreach (clsSmallPayment smlPayment in smPaymentsList)
            {
                clsTicketsForDataGrid item = new clsTicketsForDataGrid();

                item.ID = smlPayment.TicketID;
                item.CustomerID = "ABONO A LA CUENTA";
                item.TotalPrice = smlPayment.PaymentAmount;
                item.PayMethod = 1;
                item.PayMethodAlpha = "CANC";
                item.Status = false;
                item.StatusAlpha = "CANC";

                itemsList.Add(item);
            }

            itemsListSorted = Helper.SortTicketsForDataGrid(itemsList);

            TodayTickets.ItemsSource = itemsListSorted;

            Mouse.OverrideCursor = null;
        }
        private void btn_DailyClosePrint(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (SendReportByEmail.IsChecked == true)
            {
                SMTP.SendDailyReport(dcReport, workDay1, workDay2);
                SMTP.SendSalesSummary(workDay1, workDay2);
            }

            Mouse.OverrideCursor = null;
        }
        private void PrintSummaryWithDetail_Checked(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.PrintSummaryWithDetail == false)
                Settings.Default.PrintSummaryWithDetail = true;
            else
                Settings.Default.PrintSummaryWithDetail = false;

            Settings.Default.Save();
        }
    }
}
