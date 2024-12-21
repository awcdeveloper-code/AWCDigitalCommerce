using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfQryDailyClose.xaml
    /// </summary>
    public partial class wpfQryDailyClose : Window
    {
        private string workDay = string.Empty;
        private List<clsTicketsForDataGrid> itemsList;
        private List<clsSmallPayment> smPaymentsList;
        public wpfQryDailyClose()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();
        }
        private void SelectedDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            workDay = SelectedDay.SelectedDate.ToString();

            if (workDay.Length == 0) return;

            string year = workDay.Split('/')[2].Substring(0, 4);
            string month = workDay.Split('/')[1].PadLeft(2, '0');
            string day = workDay.Split('/')[0].PadLeft(2, '0');

            workDay = year + month + day;

            clsTicket ticketSummary = DB.GetTicketsSummary(workDay);
            clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(workDay);

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

            itemsList = DB.DataBinding_tbl_DailyClose(workDay);

            if (itemsList.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: La fecha seleccionada NO contiene información", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                this.Close();
            }

            smPaymentsList = DB.GetSmallPayments(workDay);

            foreach(clsSmallPayment smlPayment in smPaymentsList)
            {
                clsTicketsForDataGrid item = new clsTicketsForDataGrid();

                item.ID = smlPayment.TicketID;
                item.CustomerID = "ABONO A LA CUENTA";
                item.TotalPrice = smlPayment.PaymentAmount;
                item.PayMethod = 1;
                item.Status = false;

                itemsList.Add(item);
            }

            List<clsTicketsForDataGrid> itemsListSorted = Helper.SortTicketsForDataGrid(itemsList);

            TodayTickets.ItemsSource = itemsListSorted;
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_DailyClosePrint(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(workDay, itemsList, 0);

            if (Settings.Default.ReportsRepository)
                ReportsRepository.DailyClosing(workDay, itemsList);

            this.Close();
        }
    }
}
