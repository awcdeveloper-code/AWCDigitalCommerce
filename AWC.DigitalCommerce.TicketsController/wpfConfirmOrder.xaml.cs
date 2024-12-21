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
    /// Interaction logic for wpfConfirmOrder.xaml
    /// </summary>
    public partial class wpfConfirmOrder : Window
    {
        public bool confirmed = false;
        private List<clsTicketDetail> itemsDetails = new List<clsTicketDetail>();

        public wpfConfirmOrder(List<clsTicketDetail> _itemsDetails)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            InitializeComponent();

            itemsDetails = _itemsDetails;
            TicketDetail.ItemsSource = itemsDetails;

            ConfirmOrder.Focus();
        }

        private void TicketDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Delete.Visibility = Visibility.Visible;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Delete(object sender, MouseButtonEventArgs e)
        {
            clsTicketDetail item = TicketDetail.SelectedItem as clsTicketDetail;

            itemsDetails.RemoveAll(x => x.ItemID == item.ItemID);
            TicketDetail.ItemsSource = itemsDetails;
            TicketDetail.Items.Refresh();
            Delete.Visibility = Visibility.Hidden;
        }

        private void btn_Confirm(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            this.Close();
        }
    }
}
