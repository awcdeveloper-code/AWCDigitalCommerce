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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucDailyClosing_ItemsByUser.xaml
    /// </summary>
    public partial class ucDailyClosing_ItemsByUser : UserControl
    {
        private string startDate = string.Empty;
        private string finishDate = string.Empty;
        private List<clsItemsOrders> itemsOrdersList = new List<clsItemsOrders>();
        public ucDailyClosing_ItemsByUser()
        {
            InitializeComponent();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = StartDate.SelectedDate.ToString();

            if (startDate.Length == 0) return;

            string year = startDate.Split('/')[2].Substring(0, 4);
            string month = startDate.Split('/')[1].PadLeft(2, '0');
            string day = startDate.Split('/')[0].PadLeft(2, '0');

            startDate = year + month + day;
        }

        private void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            finishDate = FinishDate.SelectedDate.ToString();

            if (finishDate.Length == 0) return;

            string year = finishDate.Split('/')[2].Substring(0, 4);
            string month = finishDate.Split('/')[1].PadLeft(2, '0');
            string day = finishDate.Split('/')[0].PadLeft(2, '0');

            finishDate = year + month + day;

            if (Convert.ToInt32(startDate) > Convert.ToInt32(finishDate))
            {
                wpfMessageBox.Show("Tickets Controller", "ERROR: FECHA INICIAL NO PUEDE SER MAYOR QUE LA FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            int option = rbtnByDate.IsChecked == true ? 0 : 1;

            itemsOrdersList = DB.GetItemsOrderByDate(startDate, finishDate, option);

            if (itemsOrdersList.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: El rango de fechas seleccionado NO contiene información", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                return;
            }

            dgvSQLQueryTab.ItemsSource = itemsOrdersList;
        }

        private void btn_Send(object sender, RoutedEventArgs e)
        {
            Helper.InDevelopment();
        }
    }
}
