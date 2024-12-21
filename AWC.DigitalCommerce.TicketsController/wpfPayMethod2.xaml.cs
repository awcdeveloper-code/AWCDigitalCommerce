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
    public partial class wpfPayMethod2 : Window
    {
        #region GLOBAL VARIABLES
        public int equalParts = 0;
        public int cash = 0;
        public int cashLoan = 0;
        public int creditCard = 0;
        public int transfer = 0;
        public int voucher = 0;
        public int total = 0;
        public bool payOK = false;
        public bool printTicket = false;
        private string lang = string.Empty;
        private bool fullPayment = true;
        public bool send2IRSforElectronicTicket = true;
        public bool send2IRSforElectronicInvoice = false;
        #endregion

        public wpfPayMethod2(string _lang, int _total, int _ticketNum, bool _fullPayment, int _cashLoan)
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            fullPayment = _fullPayment;
            lang = _lang;
            total = _total;
            cashLoan = _cashLoan;

            InitializeComponent();
            Traductor.ApplyTranslation(this, lang);

            if (Settings.Default.ATVApplyFee)
            {
                IVAStackPanel.Visibility = Visibility.Visible;
            }

            lblTicketNumber.Content = _ticketNum.ToString();

            int eachPartAmount = total / Convert.ToInt32(txtBox_EqualParts.Text);

            lblEachPartAmount.Content = eachPartAmount.ToString("N0") + " p/p";

            txtBox_Total.Text = total.ToString();

            txtBox_PayCash.Text = _total.ToString();

            if (!_fullPayment)
            {
                PayMethod.Text = "OK";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            payOK = false;
            this.Close();
        }

        private void btn_PayMethodOK(object sender, RoutedEventArgs e)
        {
            send2IRSforElectronicTicket = SendtoIRSforElectronicTicket.IsChecked == true ? true : false;
            send2IRSforElectronicInvoice = SendtoIRSforElectronicInvoice.IsChecked == true ? true : false;

            equalParts = Convert.ToInt32(txtBox_EqualParts.Text);

            if (PayMethod.Text == "VALIDAR")
            {
                cash = txtBox_PayCash.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayCash.Text);
                creditCard = txtBox_PayCreditCard.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayCreditCard.Text);
                transfer = txtBox_PayTransfer.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayTransfer.Text);
                voucher = txtBox_Voucher.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_Voucher.Text);

                if (cash > total && (creditCard == 0 && transfer == 0 && voucher == 0))
                {
                    wpfCashBack cback = new wpfCashBack(cash - total);
                    cback.ShowDialog();

                    txtBox_CashBack.Text = (cash - total).ToString();
                    cash = total;
                    txtBox_Total.Text = "0";
                    PayMethod.Text = "OK";
                    return;
                }

                if (creditCard > total && (cash == 0 && transfer == 0 && voucher == 0 && !Settings.Default.AllowHigherPaymentWithCreditCardOrTransfer))
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: MONTO DEL PAGO ES INVÁLIDO.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);

                    txtBox_PayCash.Text = "0";
                    txtBox_PayCreditCard.Text = txtBox_Total.Text;
                    txtBox_PayTransfer.Text = "0";
                    txtBox_Voucher.Text = "0";
                    return;
                }

                if (creditCard > total && (cash == 0 && transfer == 0 && voucher == 0 && Settings.Default.AllowHigherPaymentWithCreditCardOrTransfer))
                {
                    txtBox_CashBack.Text = (creditCard - total).ToString();
                    cash = (creditCard - total) * - 1;
                    txtBox_Total.Text = "0";
                    PayMethod.Text = "OK";
                    return;
                }

                if (transfer > total && (cash == 0 && creditCard == 0 && voucher == 0 && !Settings.Default.AllowHigherPaymentWithCreditCardOrTransfer))
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: MONTO DEL PAGO ES INVÁLIDO.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    txtBox_PayCash.Text = "0";
                    txtBox_PayCreditCard.Text = "0";
                    txtBox_PayTransfer.Text = txtBox_Total.Text;
                    txtBox_Voucher.Text = "0";
                    return;
                }

                if (transfer > total && (cash == 0 && creditCard == 0 && voucher == 0 && Settings.Default.AllowHigherPaymentWithCreditCardOrTransfer))
                {
                    txtBox_CashBack.Text = (transfer - total).ToString();
                    cash = (transfer - total) * -1;
                    txtBox_Total.Text = "0";
                    PayMethod.Text = "OK";
                    return;
                }

                int check = cash + creditCard + transfer + voucher;

                if (total - check == 0 && fullPayment)
                {
                    txtBox_Total.Text = "0";
                    PayMethod.Text = "OK";
                }
                else
                {
                    wpfMessageBox.Show("Tickets Controller", "EL PAGO INGRESADO NO CUADRA CON EL MONTO A CANCELAR.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                }
            }
            else if (fullPayment)
            {
                payOK = true;
                this.Close();
            }
            else
            {
                cash = txtBox_PayCash.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayCash.Text);
                creditCard = txtBox_PayCreditCard.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayCreditCard.Text);
                transfer = txtBox_PayTransfer.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_PayTransfer.Text);
                voucher = txtBox_Voucher.Text.Length == 0 ? 0 : Convert.ToInt32(txtBox_Voucher.Text);
                payOK = true;
                this.Close();
            }
        }

        private void txtBox_EqualParts_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtBox_EqualParts.Text = numKey.numKeyed;

                int eachPartAmount = total / Convert.ToInt32(txtBox_EqualParts.Text);
                lblEachPartAmount.Content = eachPartAmount.ToString("N0") + " p/p";
                PayMethodOK.Focus();
            }
        }

        private void txtBox_PayCash_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                Opacity = 0.5;
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                Opacity = 1;

                if (Convert.ToInt32(txtBox_PayCash.Text) > 0 &&
                    Convert.ToInt32(numKey.numKeyed)  == 0 &&
                    Convert.ToInt32(txtBox_PayCreditCard.Text) == 0)
                {
                    txtBox_PayCreditCard.Text = txtBox_PayCash.Text;
                    txtBox_PayCash.Text = "0";
                    txtBox_PayTransfer.Text = "0";
                    PayMethodOK.Focus();
                }
                else
                {
                    txtBox_PayCash.Text = numKey.numKeyed;
                }
            }
        }

        private void txtBox_PayCreditCard_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                Opacity = 0.5;
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                Opacity = 1;

                if (Convert.ToInt32(txtBox_PayCreditCard.Text) > 0 &&
                    Convert.ToInt32(numKey.numKeyed) == 0 &&
                    Convert.ToInt32(txtBox_PayTransfer.Text) == 0)
                {
                    txtBox_PayTransfer.Text = txtBox_PayCreditCard.Text;
                    txtBox_PayCreditCard.Text = "0";
                    PayMethodOK.Focus();
                }
                else
                {
                    txtBox_PayCreditCard.Text = numKey.numKeyed;
                }
            }
        }

        private void txtBox_PayTransfer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtBox_PayTransfer.Text = numKey.numKeyed;
            }
        }

        private void wpfPayMethod2_ContentRendered(object sender, EventArgs e)
        {
            if (cashLoan > 0)
            {
                wpfMessageBox.Show("Tickets Controller", $"ATENCIÓN: EL CLIENTE TIENE QUE DEVOLVER A LA CAJA UN MONTO DE {cashLoan.ToString()} COLONES, PERO NO TIENE QUE REGISTRARLO COMO VENTA.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
            }

            this.Topmost = true;
        }

        private void chkBox_SendtoIRS(object sender, RoutedEventArgs e)
        {
            send2IRSforElectronicInvoice = SendtoIRSforElectronicInvoice.IsChecked == true ? true : false;
        }

        private void chkBox_ElectronicTicket(object sender, RoutedEventArgs e)
        {
            send2IRSforElectronicTicket = SendtoIRSforElectronicTicket.IsChecked == true ? true : false;
        }

        private void txtBox_Voucher_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtBox_Voucher.Text = numKey.numKeyed;
            }
        }
    }
}
