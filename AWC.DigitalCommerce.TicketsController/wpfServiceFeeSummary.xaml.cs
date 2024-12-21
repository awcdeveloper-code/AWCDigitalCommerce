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
    /// Interaction logic for wpfServiceFeeSummary.xaml
    /// </summary>
    public partial class wpfServiceFeeSummary : Window
    {
        private string workDay = string.Empty;
        public wpfServiceFeeSummary()
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

            if (Convert.ToInt32(workDay) > Convert.ToInt32(DateTime.Now.ToString("yyyMMdd"))) return;

            clsTicket tck = DB.GetTicketsSummary(workDay);

            TotalServiceFee.Content = tck.ServiceFee.ToString("N0");
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(workDay, TotalServiceFee.Content.ToString());
            this.Close();
        }
    }
}
