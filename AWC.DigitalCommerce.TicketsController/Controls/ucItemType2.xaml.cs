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
    public partial class ucItemType2 : UserControl
    {
        public ucItemType2()
        {
            InitializeComponent();

            cbox_ItemTypeTab2.Items.Add("CERVEZAS Y REFRESCOS");
            cbox_ItemTypeTab2.Items.Add("LICORES");
            cbox_ItemTypeTab2.Items.Add("COMIDAS");
        }

        private void SelectedDayTab2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDayTab2.Text.Length > 0)
                cbox_ItemTypeTab2.IsEnabled = true;
            else
                cbox_ItemTypeTab2.IsEnabled = false;
        }

        private void cbox_ItemTypeTab2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string workDay = SelectedDayTab2.SelectedDate.ToString();

            string year = workDay.Split('/')[2].Substring(0,4);
            string month = workDay.Split('/')[1].PadLeft(2,'0');
            string day = workDay.Split('/')[0].PadLeft(2,'0');
            workDay = year + month + day;

            int itemType = cbox_ItemTypeTab2.SelectedIndex + 1;

            List<clsItemType> itemsList = DB.DataBinding_tbl_TicketsDetail(workDay, itemType);

            if (itemsList.Count == 0)
            {
                MessageBox.Show("ATENCIÓN: La combinación de parámetros seleccionados NO encontró información", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            ItemsListSeriesTab2.ItemsSource = itemsList;
        }
    }
}
