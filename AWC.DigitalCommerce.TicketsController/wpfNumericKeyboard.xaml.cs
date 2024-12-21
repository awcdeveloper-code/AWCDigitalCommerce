using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfNumericKeyboard.xaml
    /// </summary>
    public partial class wpfNumericKeyboard : Window
    {
        public string numKeyed = string.Empty;

        public wpfNumericKeyboard()
        {
            this.Topmost = true;

            InitializeComponent();

            txtNum.Focus();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_Num7(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "7";
        }

        private void btn_Num8(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "8";
        }

        private void btn_Num9(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "9";
        }

        private void btn_Num4(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "4";
        }

        private void btn_Num5(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "5";
        }

        private void btn_Num6(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "6";
        }

        private void btn_Num1(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "1";
        }

        private void btn_Num2(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "2";
        }

        private void btn_Num3(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "3";
        }

        private void btn_Clean(object sender, RoutedEventArgs e)
        {
            txtNum.Text = string.Empty;
        }

        private void btn_Num0(object sender, RoutedEventArgs e)
        {
            txtNum.Text += "0";
        }

        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (txtNum.Text.Length > 0)
                numKeyed = txtNum.Text;
            else
                numKeyed = "0";

            this.Close();
        }

        private void txtNum_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (txtNum.Text.Length > 0)
                    numKeyed = txtNum.Text;
                else
                    numKeyed = "0";

                this.Close();
            }
        }
    }
}
