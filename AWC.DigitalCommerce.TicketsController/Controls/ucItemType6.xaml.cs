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
    public partial class ucItemType6 : UserControl
    {
        public ucItemType6()
        {
            InitializeComponent();

            cbox_ItemTypeTab6.Items.Add("CERVEZAS Y REFRESCOS");
            cbox_ItemTypeTab6.Items.Add("LICORES");
            cbox_ItemTypeTab6.Items.Add("COMIDAS");
        }

        private void SelectedDayTab6_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDayTab6.Text.Length > 0)
                cbox_ItemTypeTab6.IsEnabled = true;
            else
                cbox_ItemTypeTab6.IsEnabled = false;
        }

        private void cbox_ItemTypeTab6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string workDay = SelectedDayTab6.SelectedDate.ToString();

            string year = workDay.Split('/')[2].Substring(0,4);
            string month = workDay.Split('/')[1].PadLeft(2,'0');
            string day = workDay.Split('/')[0].PadLeft(2,'0');
            workDay = year + month + day;

            int itemType = cbox_ItemTypeTab6.SelectedIndex + 1;

            List<clsItemType> itemsList = DB.DataBinding_tbl_TicketsDetail(workDay, itemType);

            if (itemsList.Count == 0)
            {
                MessageBox.Show("ATENCIÓN: La combinación de parámetros seleccionados NO encontró información", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            ItemsListSeriesTab6.ItemsSource = itemsList;
        }
    }
}
