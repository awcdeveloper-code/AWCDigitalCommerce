using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Waitrest;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfWaitrestPage.xaml
    /// </summary>
    public partial class wpfWaitrestPage : Window
    {
        private ucOpenAccounts ucoa;
        private clsTicket _ticket = new clsTicket();
        private clsCustomerVIP _custProfile = new clsCustomerVIP();
        private List<clsItemDetailForDatagrid> _ticketDetail = new List<clsItemDetailForDatagrid>();
        private List<clsItemDetailForDatagrid> mostOrdered = new List<clsItemDetailForDatagrid>();

        public wpfWaitrestPage()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(this_KeyDown);

            ucoa = new ucOpenAccounts();

            Grid.SetColumn(ucoa, 0);
            Grid.SetColumnSpan(ucoa, 5);
            Grid.SetRow(ucoa, 0);
            Grid.SetRowSpan(ucoa, 20);
            HostGrid.Children.Add(ucoa);

            ucoa.OpenAccounts.SelectionChanged += new SelectionChangedEventHandler(wpfWaitrestPage_OpenAccounts_SelectionChanged);

        }

        private void wpfWaitrestPage_ContentRendered(object sender, EventArgs e)
        {
            mostOrdered = DB.GetProductsMostOrdered(12);

            int idx = 0;

            foreach (clsItemDetailForDatagrid prod in mostOrdered)
            {
                idx++;

                if (idx > 12) break;

                clsItem item = DB.GetItem(prod.ItemID);

                switch (idx)
                {
                    case 1:
                        Prod1.Content = item.ItemDescription;
                        break;
                    case 2:
                        Prod2.Content = item.ItemDescription;
                        break;
                    case 3:
                        Prod3.Content = item.ItemDescription;
                        break;
                    case 4:
                        Prod4.Content = item.ItemDescription;
                        break;
                    case 5:
                        Prod5.Content = item.ItemDescription;
                        break;
                    case 6:
                        Prod6.Content = item.ItemDescription;
                        break;
                    case 7:
                        Prod7.Content = item.ItemDescription;
                        break;
                    case 8:
                        Prod8.Content = item.ItemDescription;
                        break;
                    case 9:
                        Prod9.Content = item.ItemDescription;
                        break;
                    case 10:
                        Prod10.Content = item.ItemDescription;
                        break;
                    case 11:
                        Prod11.Content = item.ItemDescription;
                        break;
                    case 12:
                        Prod12.Content = item.ItemDescription;
                        break;
                }
            }
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F12:
                    this.WindowState = (this.WindowState == WindowState.Minimized) ? WindowState.Maximized : WindowState.Minimized;
                    break;
                case Key.System:
                    this.Close();
                    break;
            }
        }

        private void wpfWaitrestPage_OpenAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("wpfWaitrestPage_OpenAccounts_SelectionChanged");
        }
    }
}
