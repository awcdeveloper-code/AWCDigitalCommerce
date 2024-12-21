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
    /// Interaction logic for wpfQryConsumption.xaml
    /// </summary>
    public partial class wpfQryConsumption : Window
    {
        private string startDate = string.Empty;
        private string finishDate = string.Empty;
        private List<clsItemDetailForDatagrid> itemsList = new List<clsItemDetailForDatagrid>();

        public wpfQryConsumption()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            cbox_ItemType.Items.Add("CERVEZAS Y REFRESCOS");
            cbox_ItemType.Items.Add("LICORES");
            cbox_ItemType.Items.Add("COMIDAS");
            cbox_ItemType.Items.Add("TODO");

            if (SMTP.CheckInternetConnection())
            {
                eMailConsumption.IsEnabled = true;
            }
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
        }

        private void cbox_ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int itemType = cbox_ItemType.SelectedIndex + 1;

            itemsList = DB.GetItemsByDate(startDate, finishDate, itemType);

            if (itemsList.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: La fecha seleccionada NO contiene información", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
                return;
            }

            lblTotalSale.Content = itemsList.Sum(x => x.TotalPrice).ToString("N0").PadLeft(7);
            TicketDetail.ItemsSource = itemsList;
            PrintConsumption.IsEnabled = true;
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_PrintConsumption(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(itemsList, 2, startDate, finishDate);
        }

        private void btn_eMailConsumption(object sender, RoutedEventArgs e)
        {
            wpfMailAddress wpfma = new wpfMailAddress(Settings.Default.eMailDistributionList, false);
            wpfma.ShowDialog();

            if (wpfma.bCancel) return;

            SMTP.SendEmailWithComsuptions(itemsList, cbox_ItemType.SelectedIndex + 1, wpfma.mailAddress, startDate, finishDate);
            Helper.ShowToastNotification("CORREO ENVIADO EXITOSAMENTE");
        }
    }
}
