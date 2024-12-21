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
    /// Interaction logic for frmCustomerOpenTicks.xaml
    /// </summary>
    public partial class wpfCustomerOpenTickets : Window
    {
        public bool printTicketsList = false;

        public wpfCustomerOpenTickets(string custName, List<clsTicketsForDataGrid> custOpenTcks)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            this.lblCustomerName.Content = custName;
            OldOpenTickets.ItemsSource = custOpenTcks;

            int tot = 0;
            foreach (clsTicketsForDataGrid tck in custOpenTcks)
                tot += tck.TotalPrice;
            
            lbl_TotalOpenTickets.Content = "SALDO: " + string.Format("{0:N0}", tot);
        }

        private void btn_PrintTicketsList(object sender, RoutedEventArgs e)
        {
            printTicketsList = true;
            this.Close();

        }
        private void btn_Continue(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
