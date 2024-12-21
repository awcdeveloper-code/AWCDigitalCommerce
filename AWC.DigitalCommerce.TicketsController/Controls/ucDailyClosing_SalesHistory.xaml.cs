using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucDailyClosing_SalesHistory : UserControl
    {
        private string lang = string.Empty;
        private string startDay = string.Empty;
        private string endDay = string.Empty;
        private string year = string.Empty;
        private string month = string.Empty;
        private string day = string.Empty;

        private List<clsSalesHistory> salesHist = new List<clsSalesHistory>();
        public ucDailyClosing_SalesHistory(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            PrintsSalesHistory.IsEnabled = false;
        }
        private void StartDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PrintsSalesHistory.IsEnabled = false;

            startDay = StartDay.SelectedDate.ToString();

            year = startDay.Split('/')[2].Substring(0, 4);
            month = startDay.Split('/')[1].PadLeft(2, '0');
            day = startDay.Split('/')[0].PadLeft(2, '0');

            startDay = year + month + day;
        }
        private void EndDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PrintsSalesHistory.IsEnabled = false;

                endDay = EndDay.SelectedDate.ToString();

                year = endDay.Split('/')[2].Substring(0, 4);
                month = endDay.Split('/')[1].PadLeft(2, '0');
                day = endDay.Split('/')[0].PadLeft(2, '0');

                endDay = year + month + day;

                if (Convert.ToInt32(endDay) < Convert.ToInt32(startDay))
                {
                    MessageBox.Show("ATENCIÓN: La FECHA INICIAL no puede ser menor que la FECHA FINAL", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                salesHist = CollectSalesPerDay(startDay, endDay);

                //salesHist = DB.DataBinding_tbl_Tickets(startDay, endDay);

                var totalSales = salesHist.Sum(x => x.salesTotal);
                lblTotalAmount.Content = totalSales.ToString("N0").PadLeft(11);

                if (salesHist.Count == 0)
                {
                    MessageBox.Show("ATENCIÓN: No hay información disponible para las fechas seleccionadas", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SalesByDate.ItemsSource = salesHist;
                ItemsListSeriesTab3.ItemsSource = salesHist;

                PrintsSalesHistory.IsEnabled = true;
                PrintsSalesHistory.Focus();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                MessageBox.Show("ERROR: " + ex.Message, "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btn_PrintsSalesHistory(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(salesHist, startDay + "|" + endDay);
        }
        private List<clsSalesHistory> CollectSalesPerDay(string startDay, string endDay)
        {
            try
            {
                DateTime wd = Convert.ToDateTime(StartDay.SelectedDate);

                List<clsSalesHistory> salesHist = new List<clsSalesHistory>();

                string workDay = startDay;

                while (Convert.ToInt32(workDay) >= Convert.ToInt32(startDay) && Convert.ToInt32(workDay) <= Convert.ToInt32(endDay))
                {
                    clsTicket ticketSummary = DB.GetTicketsSummary(workDay);
                    clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(workDay);

                    ticketSummary.Cash += smlPay.Cash;
                    ticketSummary.CreditCard += smlPay.CreditCard;
                    ticketSummary.Transfer += smlPay.Transfer;

                    int totalPrice = ticketSummary.Cash +
                                     ticketSummary.CreditCard +
                                     ticketSummary.Transfer +
                                     ticketSummary.Payments; // Outstanding (not Payments really)

                    clsSalesHistory salesDay = new clsSalesHistory();
                    salesDay.salesDate = DB.ConverTicketDate(workDay);
                    salesDay.salesTotal = totalPrice;

                    salesHist.Add(salesDay);

                    wd = wd.AddDays(1);

                    workDay = wd.ToString("yyyyMMdd");
                }
                return salesHist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
