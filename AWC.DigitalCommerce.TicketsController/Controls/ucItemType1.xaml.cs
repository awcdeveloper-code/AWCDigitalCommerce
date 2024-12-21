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
    public partial class ucItemType1 : UserControl
    {
        public ucItemType1()
        {
            InitializeComponent();

            cbox_ItemType.Items.Add("CERVEZAS Y REFRESCOS");
            cbox_ItemType.Items.Add("LICORES");
            cbox_ItemType.Items.Add("COMIDAS");
        }

        private void SelectedDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDay.Text.Length > 0)
                cbox_ItemType.IsEnabled = true;
            else
                cbox_ItemType.IsEnabled = false;
        }

        private void cbox_ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string workDay = SelectedDay.SelectedDate.ToString();

            string year = workDay.Split('/')[2].Substring(0,4);
            string month = workDay.Split('/')[1].PadLeft(2,'0');
            string day = workDay.Split('/')[0].PadLeft(2,'0');
            workDay = year + month + day;

            int itemType = cbox_ItemType.SelectedIndex + 1;

            List<clsItemType> itemsList = DB.DataBinding_tbl_TicketsDetail(workDay, itemType);

            if (itemsList.Count == 0)
            {
                MessageBox.Show("ATENCIÓN: La combinación de parámetros seleccionados NO encontró información", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            ItemsListSeries.ItemsSource = itemsList;
        }
    }
}
