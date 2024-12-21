using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfBlindDailyClosing.xaml
    /// </summary>
    public partial class wpfBlindDailyClosing : Window
    {
        private List<clsTicketsForDataGrid> itemsList;
        private clsDailyClosing dcReport = new clsDailyClosing();
        private int numTries = 0;

        public wpfBlindDailyClosing()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
            lblWorkDay.Content = $"FECHA CONTABLE: {DB.ConverTicketDate(Settings.Default.BusinessDate)}";
            txtBox_Cash.Focus();
        }

        private void CashDistributionCheck(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                this.Opacity = 0.5;
                wpfCashDistribution cashDist = new wpfCashDistribution();
                cashDist.ShowDialog();
                this.Opacity = 1;
                txtBox_Cash.Text = cashDist.totalCash.ToString();
            }
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Validate(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            int cash = Convert.ToInt32(txtBox_Cash.Text);
            int creditcard = Convert.ToInt32(txtBox_CreditCard.Text);
            int transfer = Convert.ToInt32(txtBox_Transfer.Text);
            int voucher = Convert.ToInt32(txtBox_Voucher.Text);

            clsTicket ticketSummary = DB.GetTicketsSummary(Settings.Default.BusinessDate);
            clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(Settings.Default.BusinessDate);
            List<clsExpense> expensesList = DB.GetExpenses(Settings.Default.BusinessDate);

            ticketSummary.Cash += smlPay.Cash;
            ticketSummary.CreditCard += smlPay.CreditCard;
            ticketSummary.Transfer += smlPay.Transfer;
            ticketSummary.Voucher += smlPay.Voucher;

            int oldTicketsCancelled = DB.GetOldTicketsCancelled(Settings.Default.BusinessDate);

            int totalPrice = ticketSummary.Cash +
                             ticketSummary.CreditCard +
                             ticketSummary.Transfer +
                             ticketSummary.Voucher +
                             ticketSummary.Payments -
                             oldTicketsCancelled;

            int netPrice = ticketSummary.Cash +
                           ticketSummary.CreditCard +
                           ticketSummary.Transfer +
                           ticketSummary.Voucher -
                           oldTicketsCancelled;

            int totExp = (int)expensesList.Sum(x => x.ExpenseAmount);

            dcReport.InitialCash = Settings.Default.CashRegisterOpening;
            dcReport.BusinessDate = Settings.Default.BusinessDate;
            dcReport.AccountsReceivable = ticketSummary.Payments;
            dcReport.Cash = ticketSummary.Cash;
            dcReport.CashByOperator = cash;
            dcReport.CreditCard = ticketSummary.CreditCard;
            dcReport.CreditCardByOperator = creditcard;
            dcReport.Transfer = ticketSummary.Transfer;
            dcReport.TransferByOperator = transfer;
            dcReport.Voucher = ticketSummary.Voucher;
            dcReport.VoucherByOperator = voucher;
            dcReport.GrossSale = totalPrice;
            dcReport.NetSale = netPrice;
            dcReport.ServiceFee = ticketSummary.ServiceFee;
            dcReport.Expenses = expensesList.Sum(x => x.ExpenseAmount);
            dcReport.TotalCashInDrawer = (Settings.Default.CashRegisterOpening + dcReport.NetSale) - totExp;
            dcReport.OldTicketsPay = oldTicketsCancelled;
            dcReport.ExpensesList = expensesList;

            itemsList = DB.DataBinding_tbl_DailyClose(Settings.Default.BusinessDate);

            string validationResult = string.Empty;

            if (cash != (ticketSummary.Cash + Settings.Default.CashRegisterOpening) - totExp)
            {
                validationResult += "ERROR: MONTO DE EFECTIVO INGRESADO NO CUADRA CON EL SISTEMA" + Environment.NewLine;
            }

            if (creditcard != ticketSummary.CreditCard)
            {
                validationResult += "ERROR: MONTO DE TARJETA DE CREDITO INGRESADO NO CUADRA CON EL SISTEMA" + Environment.NewLine;
            }

            if (transfer != ticketSummary.Transfer)
            {
                validationResult += "ERROR: MONTO DE TRANSFERENCIAS INGRESADO NO CUADRA CON EL SISTEMA" + Environment.NewLine;
            }

            numTries++;

            if (!string.IsNullOrEmpty(validationResult) && numTries == 1)
            {
                Mouse.OverrideCursor = null;
                validationResult += Environment.NewLine + "POR FAVOR, INTÉNTELO DE NUEVO!";
                wpfMessageBox.Show("Tickets Controller", validationResult, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                txtBox_Cash.Text = "0";
                txtBox_CreditCard.Text = "0";
                txtBox_Transfer.Text = "0";
                txtBox_Cash.Focus();
                return;
            }
            else if (!string.IsNullOrEmpty(validationResult) && numTries == 2)
            {
                Mouse.OverrideCursor = null;
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: SU CIERRE DIARIO NO CUADRÓ CON LOS DATOS DEL SISTEMA, ASI SERÁ REPORTADO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                dcReport.DailyClosingMatch = false;
            }
            else if (string.IsNullOrEmpty(validationResult))
            {
                Mouse.OverrideCursor = null;
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: LOS DATOS INGRESADOS CUADRAN CON LOS DEL SISTEMA, SU CIERRE DIARIO FUE EXITOSO... MUCHAS GRACIAS!", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                dcReport.DailyClosingMatch = true;
            }

            DB.InsertDailyClosingSummary(dcReport);

            Helper.PrintTicket(Settings.Default.BusinessDate, dcReport);

            if (SMTP.CheckInternetConnection() && Settings.Default.eMailDistributionList.Length > 0)
            {
                SMTP.SendDailyReport(dcReport, Settings.Default.BusinessDate, itemsList);
            }

            if (PrintKitchenSummary.IsChecked == true)
            {
                List<clsItemDetailForDatagrid> kitchenItemsList = DB.GetMealsItemsByDate(Settings.Default.BusinessDate);
                Helper.PrintTicket(kitchenItemsList, 1, null, null);
            }

            this.Close();
        }
    }
}
