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

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for ucOnePayMethodSelection.xaml
    /// </summary>
    public partial class wpfOnePayMethodSelection : Window
    {
        public int payMethod = 0;
        public wpfOnePayMethodSelection()
        {
            this.Topmost = true;

            InitializeComponent();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            payMethod = 0;
            this.Close();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if ((bool)rbtnCash.IsChecked) payMethod = 1;
            else
            if ((bool)rbtnCreditCard.IsChecked) payMethod = 2;
            else payMethod = 3;

            this.Close();
        }

    }
}
