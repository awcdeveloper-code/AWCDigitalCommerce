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
    /// Interaction logic for wpfEnterAmount.xaml
    /// </summary>
    public partial class wpfEnterAmount : Window
    {
        public int amount = 0;
        public wpfEnterAmount()
        {
            this.Topmost = true;

            InitializeComponent();

            txtAmount.Focus();
        }

        private void txtAmount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (txtAmount.Text.Length == 0)
                    amount = 0;
                else
                    amount = Convert.ToInt32(txtAmount.Text);

                this.Close();
            }
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (txtAmount.Text.Length == 0)
                amount = 0;
            else
                amount = Convert.ToInt32(txtAmount.Text);

            this.Close();
        }
    }
}
