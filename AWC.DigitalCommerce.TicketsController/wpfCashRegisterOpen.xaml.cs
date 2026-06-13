using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfCashRegisterOpen : Window
    {
        private string lang = string.Empty;
        public int CashRegisterAmount = 0;
        public int USDollarExchangeRate = 0;

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

        private async void wpfCashRegisterOpenRendered(object sender, EventArgs e)
        {
            BCCR.Content = "Contactando al BCCR, espere...";
            
            txtCashRegisterAmount.Text = Settings.Default.CashRegisterOpening.ToString();

            decimal exchangeRate = await Helper.GetCurrencyExchangeAPI();
            
            txtUSDollarExhangeRate.Text = exchangeRate.ToString("F2").Replace(",", ".");
            
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

                if (int.TryParse(txtCashRegisterAmount.Text, out CashRegisterAmount))
                {
                    Settings.Default.CashRegisterOpening = CashRegisterAmount;
                }

                if (int.TryParse(txtUSDollarExhangeRate.Text, out USDollarExchangeRate))
                {
                    Settings.Default.USDollarExchangeRate = USDollarExchangeRate;
                }

                Settings.Default.Save();

                DB.UpdateCashOnHandAtTheBeginning(CashRegisterAmount);

                //if (SMTP.CheckInternetConnection() && Settings.Default.GetDailyQuote.Length > 0)
                //{
                //    string dailyQuote = await Helper.GetDailyQuote();
                //    Mouse.OverrideCursor = null;
                //    wpfMessageBox.Show("Tickets Controller", dailyQuote, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, null);
                //}

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
