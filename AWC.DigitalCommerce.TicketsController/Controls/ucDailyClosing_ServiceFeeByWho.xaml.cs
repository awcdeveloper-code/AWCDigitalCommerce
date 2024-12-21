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
    /// Interaction logic for ucDailyClosing_ServiceFeeByWho.xaml
    /// </summary>
    public partial class ucDailyClosing_ServiceFeeByWho : UserControl
    {
        private string _lang = string.Empty;
        private string startDate = string.Empty;
        private string finalDate = string.Empty;
        private List<clsServiceFeeByWho> sfbw = new List<clsServiceFeeByWho>();
        private int totalServiceFee = 0;
        public ucDailyClosing_ServiceFeeByWho(string lang)
        {
            _lang = lang;
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

            if (Convert.ToInt32(startDate) > Convert.ToInt32(DateTime.Now.ToString("yyyMMdd"))) return;
        }

        private void FinalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            finalDate = StartDate.SelectedDate.ToString();

            if (finalDate.Length == 0) return;

            string year = finalDate.Split('/')[2].Substring(0, 4);
            string month = finalDate.Split('/')[1].PadLeft(2, '0');
            string day = finalDate.Split('/')[0].PadLeft(2, '0');

            finalDate = year + month + day;

            if (Convert.ToInt32(finalDate) < Convert.ToInt32(startDate))
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: LA FECHA FINAL NO PUEDE SER MENOR QUE LA FECHA INICIAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, _lang);
                return;
            }

            bool om = OnlyMeals.IsChecked == true;

            sfbw = DB.GetServiceFeeByWho(startDate, finalDate, om);

            if (sfbw.Count == 0)
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: RANGO DE FECHAS DEFINIDAS NO CONTIENE INFORMACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, _lang);
                return;
            }

            ServiceFeeByWho.ItemsSource = sfbw;

            totalServiceFee = sfbw.Sum(x => x.TotalServiceFee);

            lblTotalServiceFee.Content = totalServiceFee.ToString("N0");
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(startDate, finalDate, sfbw);
        }
    }
}
