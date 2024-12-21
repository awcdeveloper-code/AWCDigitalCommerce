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
    /// Interaction logic for wpfTicketsLost.xaml
    /// </summary>
    public partial class wpfTicketsLost : Window
    {
        List<clsTicket> unAccList = new List<clsTicket>();
        public wpfTicketsLost()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            lblHeader.Content = $"CUENTAS INCOBRABLES CON MÁS DE {Settings.Default.UncollectibleAccount.ToString()} DIAS";

            DateTime td = DateTime.Now;
            DateTime tl = td.AddDays(Settings.Default.UncollectibleAccount * -1);

            unAccList = DB.GetUncollectibleAccount(tl.ToString("yyyyMMdd"));
            TicketsLost.ItemsSource = unAccList;

            int tot = unAccList.Sum(x => x.TotalPrice);
            lblTotalAmount.Content = "TOTAL: " + tot.ToString("N0");
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {

        }
    }
}
