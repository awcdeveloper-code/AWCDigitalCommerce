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
    /// Interaction logic for wpfCashRegisterOpen.xaml
    /// </summary>
    public partial class wpfCashRegisterOpen : Window
    {
        private string lang = string.Empty;
        public int CashRegisterAmount = 0;
        public int USDollarExhangeRate = 0;
        public wpfCashRegisterOpen(string _lang)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            this.Topmost = true;

            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);
        }

        private void wpfCashRegisterOpenRendered(object sender, EventArgs e)
        {
            BCCR.Content = "Contactando al BCCR, espere...";
            txtCashRegisterAmount.Text = Settings.Default.CashRegisterOpening.ToString();
            txtUSDollarExhangeRate.Text = Helper.GetCurrencyExchange().ToString();
            BCCR.Content = "(Tasa de Cambio BCCR)";

            txtCashRegisterAmount.IsEnabled = true;
            txtUSDollarExhangeRate.IsEnabled = true;

            txtCashRegisterAmount.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void btn_Continue(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                if (SMTP.CheckInternetConnection() && Settings.Default.GetDailyQuote.Length > 0)
                {
                    string dailyQuote = await Helper.GetDailyQuote();
                    Mouse.OverrideCursor = null;
                    wpfMessageBox.Show("Tickets Controller", dailyQuote, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, null);
                }

                DB.UpdateCashOnHandAtTheBeginning(Convert.ToInt32(txtCashRegisterAmount.Text));
                
                Settings.Default.CashRegisterOpening = Convert.ToInt32(txtCashRegisterAmount.Text);
                Settings.Default.USDollarExchangeRate = Convert.ToInt32(txtUSDollarExhangeRate.Text);
                Settings.Default.Save();

                this.Close();
            }
            catch (Exception ex)
            {
                wpfMessageBox.Show("Tickets Controller", ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                this.Close();
            }
        }
    }
}
