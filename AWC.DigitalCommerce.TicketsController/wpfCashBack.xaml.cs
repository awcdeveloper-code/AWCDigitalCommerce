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
    /// Interaction logic for wpfCashBack.xaml
    /// </summary>
    public partial class wpfCashBack : Window
    {
        public wpfCashBack(int cashBack)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            Topmost = true;
            lbl_cashBack.Content = cashBack.ToString("N0");
            btnOK.Focus();
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
