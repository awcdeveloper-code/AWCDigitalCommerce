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
    public partial class wpfCustomerOpenTickets2 : Window
    {
        public bool printTicketsList = false;
        public Dictionary <int,int> tcks2Add = new Dictionary<int, int>();

        public wpfCustomerOpenTickets2(int customerID, string custName)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            this.lblCustomerName.Content = custName;
            List<clsTicketsForDataGrid> itemdg = DB.DataBinding_tbl_Tickets(customerID, 3);
            OldOpenTickets.ItemsSource = itemdg;

            int tot = itemdg.Sum(x => x.TotalPrice);

            //foreach (clsTicketsForDataGrid tck in custOpenTcks)
            //    tot += tck.TotalPrice;

            lbl_TotalOpenTickets.Content = "SALDO: " + string.Format("{0:N0}", tot);
        }

        private void OldOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int totalPrice = 0;

            foreach (clsTicketsForDataGrid row in OldOpenTickets.SelectedItems)
            {
                totalPrice += row.TotalPrice;
            }

            lbl_TotalOpenTickets.Content = "SALDO: " + string.Format("{0:C0}", totalPrice);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            tcks2Add.Clear();
            this.Close();
        }

        private void btn_Continue(object sender, RoutedEventArgs e)
        {
            tcks2Add.Clear();

            foreach (clsTicketsForDataGrid row in OldOpenTickets.SelectedItems)
            {
                tcks2Add.Add(row.ID, row.TotalPrice);
            }
            this.Close();
        }
    }
}
