using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucReportsInventoryMgmt.xaml
    /// </summary>
    public partial class ucReportsInventoryMgmt : UserControl
    {
        private List<clsInvoice> invoicesList = new List<clsInvoice>();
        private List<clsInvoiceItem> invoiceItemsList = new List<clsInvoiceItem>();
        private List<clsNote> notesList = new List<clsNote>();
        private List<clsNoteDetail> noteDetailList = new List<clsNoteDetail>();
        private List<clsItemDefective> defectivesItemsList = new List<clsItemDefective>();
        private List<clsItem> MinimumItemsList = new List<clsItem>();
        private List<clsExpense> ExpensesList = new List<clsExpense>();
        private List<clsItemDetailForDatagrid> newItemsByDate = new List<clsItemDetailForDatagrid>();
        private string startDate = string.Empty;
        private string endDate = string.Empty;

        public ucReportsInventoryMgmt()
        {
            InitializeComponent();

            string[] months = { "TODOS", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SETIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };

            cbox_Month.ItemsSource = months;
            cbox_Month_Defectives.ItemsSource = months;
            cbox_Month_Notes.ItemsSource = months;

            // TAB ITEMS BELOW MINIMUM
            MinimumItemsList = DB.GetItemsBelowMinimum();
            dgMinimumList.ItemsSource = MinimumItemsList;

            if (MinimumItemsList.Count > 0)
                Minimum.IsEnabled = true;

            // TAB EXPENSES
            ExpensesList = DB.GetExpenses();
            dgExpenses.ItemsSource = ExpensesList;
        }

        // INVOICES TAB
        private void cbox_Month_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_Month.SelectedIndex == -1) return;

            string yearMonth = string.Empty;

            if (cbox_Month.SelectedIndex == 0)
                yearMonth = DateTime.Now.ToString("yyyy");
            else
                yearMonth = DateTime.Now.ToString("yyyy") + cbox_Month.SelectedIndex.ToString("00");

            // get invoices list
            invoicesList = DB.GetInvoicesListByYearMonth(yearMonth + "%");
            dgInvoicesList.ItemsSource = invoicesList;
        }

        private void dgInvoicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsInvoice invoice = dgInvoicesList.SelectedItem as clsInvoice;

            invoiceItemsList = DB.GetInvoiceItemsByGUID(invoice.InvoiceGUID);
            dgItemsList.ItemsSource = invoiceItemsList;
        }

        // NOTES TAB
        private void cbox_Month_Notes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_Month_Notes.SelectedIndex == -1) return;

            string yearMonth = string.Empty;

            if (cbox_Month_Notes.SelectedIndex == 0)
                yearMonth = DateTime.Now.ToString("yyyy");
            else
                yearMonth = DateTime.Now.ToString("yyyy") + cbox_Month_Notes.SelectedIndex.ToString("00");

            // get notes list
            notesList = DB.GetNotesListByYearMonth(yearMonth + "%");
            dgNotesList.ItemsSource = notesList;
        }

        // DEFECTIVES ITEMS TAB
        private void cbox_Month_Defectives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_Month_Defectives.SelectedIndex == -1) return;

            string yearMonth = string.Empty;

            if (cbox_Month_Defectives.SelectedIndex == 0)
                yearMonth = DateTime.Now.ToString("yyyy");
            else
                yearMonth = DateTime.Now.ToString("yyyy") + cbox_Month_Defectives.SelectedIndex.ToString("00");

            // get invoices list
            defectivesItemsList = DB.GetDefectivesItemsByYearMonth(yearMonth + "%");
            dgDefectivesList.ItemsSource = defectivesItemsList;
        }

        private void dgNotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsNote note = dgNotesList.SelectedItem as clsNote;

            noteDetailList = DB.GetNoteDetailByGUID(note.NoteGUID);
            dgNotesDetail.ItemsSource = noteDetailList;
        }

        // BELOW MINIMUM TAB
        private void btn_PrintMinimum(object sender, RoutedEventArgs e)
        {
            if (wpfMessageBox.Show("Inventory Management", "ATTENTION: Do you really want to print the list (Yes/No)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, "") == MessageBoxResult.Yes)
            {
                Helper.PrintTicket("BelowMinimum", MinimumItemsList);
            }
        }

        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            foreach (clsExpense expense in dgExpenses.SelectedItems)
            {
                DB.DeleteExpense(expense.ID);
            }

            ExpensesList = DB.GetExpenses();
            dgExpenses.ItemsSource = ExpensesList;
        }

        private void dgExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.IsEnabled = dgExpenses.SelectedItems.Count > 0;
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = StartDate.SelectedDate.ToString();

            if (startDate.Length == 0) return;

            string year = startDate.Split('/')[2].Substring(0, 4);
            string month = startDate.Split('/')[1].PadLeft(2, '0');
            string day = startDate.Split('/')[0].PadLeft(2, '0');

            startDate = year + month + day;
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = EndDate.SelectedDate.ToString();

            if (endDate.Length == 0) return;

            string year = endDate.Split('/')[2].Substring(0, 4);
            string month = endDate.Split('/')[1].PadLeft(2, '0');
            string day = endDate.Split('/')[0].PadLeft(2, '0');

            endDate = year + month + day;

            if (Convert.ToInt32(startDate) > Convert.ToInt32(endDate))
            {
                wpfMessageBox.Show("Tickets Controller", "ERROR: FECHA INICIAL NO PUEDE SER MAYOR QUE LA FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            newItemsByDate = DB.GetInventoryByDate(startDate, endDate);
            dgInvocesSummary.ItemsSource = newItemsByDate;

            if (newItemsByDate.Count > 0)
                InvocesSummary.IsEnabled = true;
        }

        private void btn_InvocesSummary(object sender, RoutedEventArgs e)
        {
            Helper.PrintInvoicesSummaryByDate(newItemsByDate, startDate, endDate);
        }
    }
}
