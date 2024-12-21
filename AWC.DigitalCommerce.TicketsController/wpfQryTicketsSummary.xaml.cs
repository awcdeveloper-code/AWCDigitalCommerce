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
    /// <summary>
    /// Interaction logic for wpfQryTicketsSummary.xaml
    /// </summary>
    public partial class wpfQryTicketsSummary : Window
    {
        private List<clsTicket> ticketsSummary = new List<clsTicket>();
        private string startDay = string.Empty;
        private string endDay = string.Empty;
        public wpfQryTicketsSummary()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            cbox_PaymentMethod.Items.Add("EFECTIVO");
            cbox_PaymentMethod.Items.Add("TARJETA DE CRÉDITO");
            cbox_PaymentMethod.Items.Add("SINPE");
        }

        private void StartDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDay = StartDay.SelectedDate.ToString();

            if (startDay.Length == 0) return;

            string year = startDay.Split('/')[2].Substring(0, 4);
            string month = startDay.Split('/')[1].PadLeft(2, '0');
            string day = startDay.Split('/')[0].PadLeft(2, '0');

            startDay = year + month + day;
        }

        private void EndDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDay = EndDay.SelectedDate.ToString();

            if (startDay.Length == 0) return;

            string year = endDay.Split('/')[2].Substring(0, 4);
            string month = endDay.Split('/')[1].PadLeft(2, '0');
            string day = endDay.Split('/')[0].PadLeft(2, '0');

            endDay = year + month + day;

            if (Convert.ToInt32(endDay) < Convert.ToInt32(startDay))
            {
                wpfMessageBox.Show("Ticket Controller", "ERROR: FECHA FINAL NO PUEDE SER MENOR QUE LA FECHA INCIAL", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, null);
                StartDay.SelectedDate = null;
                EndDay.SelectedDate = null;
                StartDay.Focus();
                return;
            }
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Export(object sender, RoutedEventArgs e)
        {
            wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: ESTA OPCIÓN ESTA EN ETAPA DE DESAROLLO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, null);
        }

        private void cbox_PaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_PaymentMethod.SelectedIndex == -1) return;
            ticketsSummary = DB.GetTicketsSummary(startDay, endDay, cbox_PaymentMethod.SelectedIndex);
            TicketDetail.ItemsSource = ticketsSummary;

            int ticketsSummaryTotal = ticketsSummary.Sum(x => x.TotalPrice);
            lblticketsSummaryTotal.Content = ticketsSummaryTotal.ToString("N0").PadLeft(7);

            if (ticketsSummary.Count > 0)
            {
                btnExport.IsEnabled = true;
            }
            else
            {
                btnExport.IsEnabled = false;
            }
        }
    }
}
