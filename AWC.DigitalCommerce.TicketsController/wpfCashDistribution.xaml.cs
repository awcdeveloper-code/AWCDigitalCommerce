using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for wpfCashDistribution.xaml
    /// </summary>
    public partial class wpfCashDistribution : Window
    {
        public int totalCash = 0;
        public wpfCashDistribution()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
            txtBox_M100.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Calculate(object sender, RoutedEventArgs e)
        {
            totalCash += 100 * Convert.ToInt32(txtBox_M100.Text);
            totalCash += 500 * Convert.ToInt32(txtBox_M500.Text);
            totalCash += 1000 * Convert.ToInt32(txtBox_B1000.Text);
            totalCash += 2000 * Convert.ToInt32(txtBox_B2000.Text);
            totalCash += 5000 * Convert.ToInt32(txtBox_B5000.Text);
            totalCash += 10000 * Convert.ToInt32(txtBox_B10000.Text);
            totalCash += 20000 * Convert.ToInt32(txtBox_B20000.Text);
            totalCash += 50000 * Convert.ToInt32(txtBox_B50000.Text);

            this.Close();
        }
    }
}
