using AWC.DigitalCommerce.TicketsController.Controls;
using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for wpfDailyCloseSummary.xaml
    /// </summary>
    public partial class wpfDailyCloseSummary : Window
    {
        private string businessDate;
        private clsDailyClosing dcReport = new clsDailyClosing();
        private List<clsTicketsForDataGrid> itemsList;
        private List<clsSmallPayment> smPaymentsList;
        private int cashOnDrawer = 0;
        private int cashRegisterOpening = 0;
        private bool internetAvail = true;

        public wpfDailyCloseSummary()
        {
            InitializeComponent();
        }

        private void calDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            lbl_InitialCash.Content = "0";
            lbl_IncomeCash.Content = "0";
            lbl_OutstandingAmount.Content = "0";
            lbl_Cash.Content = "0";
            lbl_CreditCard.Content = "0";
            lbl_Transfer.Content = "0";
            lbl_Voucher.Content = "0";
            lbl_TotalSale.Content = "0";
            lbl_NetSale.Content = "0";
            lbl_ServiceFee.Content = "0";
            lbl_Expenses.Content = "0";
            lbl_OldTicketsPay.Content = "0"; ;

            businessDate = ((DateTime)calDate.SelectedDate).ToString("yyyyMMdd");
            Search.IsEnabled = true;
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Print(object sender, RoutedEventArgs e)
        {
            Helper.PrintTicket(businessDate, dcReport, Convert.ToInt32(txtShift.Text));

            if (!SMTP.CheckInternetConnection())
            {
                Helper.ShowToastNotification("ATENCIÓN: Sin acceso a Internet, el cierre no será enviado por correo.");
            }
            else
            {
                SMTP.SendDailyReport(dcReport, businessDate, itemsList);
                Helper.ShowToastNotification($"Cierre del {DB.ConverTicketDate(businessDate)} Turno {txtShift.Text} fue enviado exitosamente.");
            }

            Search.IsEnabled = false;
            Print.IsEnabled = false;
        }

        private void btn_Search(object sender, RoutedEventArgs e)
        {
            // display results
            clsTicket ticketSummary = DB.GetTicketsSummary(businessDate, Convert.ToInt32(txtShift.Text));
            clsSmallPayment smlPay = DB.GetSmallPaymentsSummary(businessDate, Convert.ToInt32(txtShift.Text));
            List<clsExpense> expensesList = DB.GetExpenses(businessDate, Convert.ToInt32(txtShift.Text));
            List<clsCashIncomes> incomeCashList = DB.GetIncomeCash(businessDate, businessDate, Convert.ToInt32(txtShift.Text));

            ticketSummary.Cash += smlPay.Cash;
            ticketSummary.CreditCard += smlPay.CreditCard;
            ticketSummary.Transfer += smlPay.Transfer;
            ticketSummary.Voucher += smlPay.Voucher;

            int oldTicketsCancelled = DB.GetOldTicketsCancelled(businessDate, Convert.ToInt32(txtShift.Text));

            int totalPrice = ticketSummary.Cash +
                             ticketSummary.CreditCard +
                             ticketSummary.Transfer +
                             ticketSummary.Voucher +
                             ticketSummary.Payments -
                             oldTicketsCancelled;

            int netPrice = ticketSummary.Cash +
                           ticketSummary.CreditCard +
                           ticketSummary.Transfer +
                           ticketSummary.Voucher;

            int totExp = (int)expensesList.Sum(x => x.ExpenseAmount);

            cashRegisterOpening = DB.GetCashOnHandAtTheBeginning();
            lbl_InitialCash.Content = cashRegisterOpening.ToString("N0", CultureInfo.InvariantCulture);
            
            int incomeCash = incomeCashList.Sum(x => x.IncomeAmount);
            lbl_IncomeCash.Content = incomeCash.ToString("N0", CultureInfo.InvariantCulture);
            
            lbl_OutstandingAmount.Content = ticketSummary.Payments.ToString("N0", CultureInfo.InvariantCulture);
            lbl_Cash.Content = (cashRegisterOpening + incomeCash + ticketSummary.Cash - totExp).ToString("N0", CultureInfo.InvariantCulture);
            lbl_CreditCard.Content = ticketSummary.CreditCard.ToString("N0", CultureInfo.InvariantCulture);
            lbl_Transfer.Content = ticketSummary.Transfer.ToString("N0", CultureInfo.InvariantCulture);
            lbl_Voucher.Content = ticketSummary.Voucher.ToString("N0", CultureInfo.InvariantCulture);
            lbl_TotalSale.Content = totalPrice.ToString("N0", CultureInfo.InvariantCulture);
            lbl_NetSale.Content = netPrice.ToString("N0", CultureInfo.InvariantCulture);
            lbl_ServiceFee.Content = ticketSummary.ServiceFee.ToString("N0", CultureInfo.InvariantCulture);
            lbl_Expenses.Content = expensesList.Sum(x => x.ExpenseAmount).ToString("N0", CultureInfo.InvariantCulture);
            lbl_OldTicketsPay.Content = (oldTicketsCancelled + smlPay.Cash + smlPay.CreditCard + smlPay.Transfer + smlPay.Voucher).ToString("N0", CultureInfo.InvariantCulture);

            cashOnDrawer = cashRegisterOpening + incomeCash + ticketSummary.Cash - totExp;

            dcReport = new clsDailyClosing();
            dcReport.BusinessDate = businessDate;
            dcReport.Shift = Settings.Default.Shift;
            dcReport.InitialCash = cashRegisterOpening;
            dcReport.IncomeCash = incomeCash;
            dcReport.AccountsReceivable = ticketSummary.Payments;
            dcReport.Cash = ticketSummary.Cash;
            dcReport.CreditCard = ticketSummary.CreditCard;
            dcReport.Transfer = ticketSummary.Transfer;
            dcReport.Voucher = ticketSummary.Voucher;
            dcReport.GrossSale = totalPrice;
            dcReport.NetSale = netPrice;
            dcReport.ServiceFee = ticketSummary.ServiceFee;
            dcReport.Expenses = expensesList.Sum(x => x.ExpenseAmount);
            dcReport.TotalCashInDrawer = cashRegisterOpening + dcReport.NetSale;
            dcReport.OldTicketsPay = oldTicketsCancelled + smlPay.Cash + smlPay.CreditCard + smlPay.Transfer + smlPay.Voucher;
            dcReport.CashIncomeList = incomeCashList;
            dcReport.ExpensesList = expensesList;
            dcReport.VouchersList = DB.GetVouchers(Settings.Default.BusinessDate);

            itemsList = DB.DataBinding_tbl_DailyClose(businessDate, Convert.ToInt32(txtShift.Text));

            Print.IsEnabled = true;
        }
    }
}
