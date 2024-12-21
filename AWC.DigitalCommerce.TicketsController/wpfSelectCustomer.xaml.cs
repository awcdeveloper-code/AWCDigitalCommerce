using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSelectCustomer.xaml
    /// </summary>
    public partial class wpfSelectCustomer : Window
    {
        private int ticketNumber = 0;
        private List<string> customerAKAList;
        public string customerAKA = string.Empty;

        public wpfSelectCustomer(int _ticketNumber, List<string> _customerAKAList)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
            ticketNumber = _ticketNumber;
            customerAKAList = _customerAKAList;
            lBox_Customers.ItemsSource = customerAKAList;
        }

        private void btn_SelectCustomer(object sender, RoutedEventArgs e)
        {
            customerAKA = lBox_Customers.SelectedItem as string;
            this.Close();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: REALMENTE DESEA BORRAR ESTA PREFACTURA (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, null) == MessageBoxResult.Yes)
            {
                string selectedItem = lBox_Customers.SelectedItem.ToString();

                DB.DeleteTicketProform(ticketNumber, selectedItem);

                customerAKAList = customerAKAList.Where(item => item != selectedItem).ToList();

                lBox_Customers.ItemsSource = customerAKAList.ToList();

                if (customerAKAList.Count == 0)
                {
                    Delete.IsEnabled = false;
                    Continue.IsEnabled = false;
                }
            }
        }

        private void lBox_Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Delete.IsEnabled = true;
            Continue.IsEnabled = true;
        }
    }
}
