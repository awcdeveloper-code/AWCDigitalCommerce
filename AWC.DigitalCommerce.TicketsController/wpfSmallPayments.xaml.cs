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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSmallPayments.xaml
    /// </summary>
    public partial class wpfSmallPayments : Window
    {
        private List<clsSmallPayment> payments = new List<clsSmallPayment>();

        public wpfSmallPayments()
        {
            this.Topmost = true;

            InitializeComponent();

            payments = DB.GetSmallPayments("");
            dgPayments.ItemsSource = payments;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", "Disculpe, este reporte está en fase de desarrollo.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
        }
    }
}
