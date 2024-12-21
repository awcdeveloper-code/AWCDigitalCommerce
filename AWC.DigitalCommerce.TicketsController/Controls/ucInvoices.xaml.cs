using AWC.DigitalCommerce.TicketsController.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Controls
{

    public class dgItemsList    // just for local use
    {
        public int ID { get; set; }
        public int ItemType { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQty { get; set; }
        public string GUID { get; set; }
    }

    public partial class ucInvoices : UserControl
    {
        private string invoiceDate = string.Empty;
        private int providerID = 0;
        private List<clsProvider> providersList = new List<clsProvider>();
        private List<clsItem> itemsList = new List<clsItem>();
        private List<clsItem> lstProducts = new List<clsItem>();

        public ucInvoices()
        {
            InitializeComponent();

            providersList = DB.ListBinding_tbl_Providers();    // Providers List
            cbox_ProviderName.ItemsSource = providersList;
            lstProducts = DB.ListBinding_tbl_Items(6);

            itemsList = DB.ListBinding_tbl_Items(10);            // Items List
            InvoiceDate.Text = DB.ConverTicketDate(Settings.Default.BusinessDate).Replace(".", "/");
        }

        private void CleanAll()
        {
            cbox_ProviderName.Text = string.Empty;
            txtInvoiceNumber.Text = string.Empty;
            txtInvoiceNumber.IsEnabled = false;
            InvoiceDate.SelectedDate = null;
            txtInvoiceAmount.Text = string.Empty;
            dgItemsList.Items.Clear();
            cbox_ProviderName.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9],+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void InvoiceDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            invoiceDate = InvoiceDate.SelectedDate.ToString();

            if (invoiceDate.Length == 0) return;

            string year = invoiceDate.Split('/')[2].Substring(0, 4);
            string month = invoiceDate.Split('/')[1].PadLeft(2, '0');
            string day = invoiceDate.Split('/')[0].PadLeft(2, '0');

            invoiceDate = year + month + day;
        }

        private void cbox_ProviderName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbox_ProviderName.SelectedIndex == -1) return;

                clsProvider row = cbox_ProviderName.SelectedItem as clsProvider;
                providerID = row.ID;
                txtInvoiceNumber.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }

        private void txtInvoiceNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            int invoiceID = DB.CheckProviderAndInvoice(providerID, Convert.ToInt32(txtInvoiceNumber.Text));

            // validdate provider and invoice number
            if (invoiceID > 0)
            {
                wpfMessageBox.Show("Inventories Management", "ATTENTION: The invoice number already exist with InvoiceID [" + invoiceID + "].", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, string.Empty);
                txtInvoiceNumber.Text = string.Empty;
                return;
            }
        }

        private void dgItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDeleteItem.IsEnabled = true;
            txtInvoiceNumber.IsEnabled=true;
        }

        // Activate basic calculatyor
        #region BUTTONS
        private void btn_DeleteItem(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgItemsList.SelectedItem;

            if (selectedItem != null)
                dgItemsList.Items.Remove(selectedItem);

            btnDeleteItem.IsEnabled = false;
        }

        private void btn_SaveInvoice(object sender, RoutedEventArgs e)
        {
            try
            {
                clsInvoice newInvoice = new clsInvoice();

                newInvoice.InvoiceNumber = Convert.ToInt32(txtInvoiceNumber.Text);
                newInvoice.InvoiceDate = invoiceDate;
                newInvoice.InvoiceAmount = Convert.ToDouble(txtInvoiceAmount.Text);
                newInvoice.ProviderID = providerID;

                Guid guidID = Guid.NewGuid();
                newInvoice.InvoiceGUID = guidID.ToString();

                if (DB.InsertNewInvoice(newInvoice))
                {
                    Logger.WriteToLog("InventoriesManagement", "Invoice [" + newInvoice.InvoiceNumber + "] added.", Logger.Severity.DEBUG);

                    foreach (dgItemsList item in dgItemsList.Items)
                    {
                        // prepare the record
                        clsInvoiceItem invItem = new clsInvoiceItem();
                        invItem.InvoiceGUID = newInvoice.InvoiceGUID;
                        invItem.ItemType = item.ItemType;
                        invItem.ItemID = item.ID;
                        invItem.ItemQty = item.ItemQty;

                        // add invoice item
                        DB.InsertNewInvoiceItem(invItem);

                        // update item inventory
                        clsItem workItem = DB.GetItem(item.ID);

                        clsItem updItem = new clsItem();
                        updItem.ID = item.ID;
                        updItem.ItemSold = item.ItemQty * workItem.ItemUnitSize;

                        DB.UpdateItemInventory("ADD", updItem);
                    }

                    CleanAll();
                    Helper.ShowToastNotification($"Factura {newInvoice.InvoiceNumber} aplicada");
                    Logger.WriteToLog("InventoriesManagement", "Items of invoice [" + newInvoice.InvoiceNumber + "] added.", Logger.Severity.INFORMATION);
                }
                else
                {
                    CleanAll();
                    wpfMessageBox.Show("Inventories Management", "ERROR: The system cannot add the invoice to the system.Please, contact the Administrator.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }
        #endregion

        private void btn_AddProducts(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.5;
            wpfSelectProducts prodsel = new wpfSelectProducts(lstProducts);
            prodsel.ShowDialog();
            this.Opacity = 1;

            if (!prodsel.bOK) return;

            foreach (clsTicketDetail item in prodsel.SelectedProducts)
            {
                dgItemsList addItem2dg = new dgItemsList();

                addItem2dg.ID = item.ItemID;
                addItem2dg.ItemType = item.ItemType;
                addItem2dg.ItemDescription = item.ItemDesc;
                addItem2dg.ItemQty = Convert.ToInt32(item.Qty);

                dgItemsList.Items.Add(addItem2dg);

            }
            btnSaveInvoice.IsEnabled = true;
        }
    }
}
