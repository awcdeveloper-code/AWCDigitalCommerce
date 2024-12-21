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
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfSalesHistory : Window
    {
        string startDay = string.Empty;
        string endDay = string.Empty;
        string year = string.Empty;
        string month = string.Empty;
        string day = string.Empty;


        List<clsSalesHistory> salesHist = new List<clsSalesHistory>();

        public wpfSalesHistory()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            PrintsSalesHistory.IsEnabled=false;
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
                MessageBox.Show("ATENCIÓN: No hay información disponible para las fechas seleccioandas", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SalesByDate.ItemsSource = salesHist;
            PrintsSalesHistory.IsEnabled = true;
            PrintsSalesHistory.Focus();
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

        private void btn_PrintsSalesHistory(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(salesHist, startDay + "|" + endDay);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
