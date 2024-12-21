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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucMaintenance : UserControl
    {
        private string lang = string.Empty;
        private List<clsItem> itemsList = new List<clsItem>();
        public string strDefectiveItemSaved = string.Empty;

        public ucMaintenance(string _lang)
        {
            lang = _lang;
            InitializeComponent();
            Traductor.ApplyTranslation(this, lang);

            itemsList = DB.ListBinding_tbl_Items(4);
            cbox_ItemsList.ItemsSource = itemsList;
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^09]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtItemQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbox_ItemsList.SelectedIndex >= 0 && txtItemQty.Text.Length > 0)
                btnAddDefective.IsEnabled = true;
            else
                btnAddDefective.IsEnabled = false;
        }

        private void btn_AddDefective(object sender, RoutedEventArgs e)
        {
            try
            {
                // reduce inventory
                clsItem item = cbox_ItemsList.SelectedItem as clsItem;
                item.ItemDefective = Convert.ToInt32(txtItemQty.Text);
                DB.UpdateItemInventory("DEF", item);

                // add record to ItemsDefective table
                clsItemDefective itemDef = new clsItemDefective();
                itemDef.ItemID = item.ID;
                itemDef.ItemQty = Convert.ToInt32(txtItemQty.Text);
                itemDef.DeclarationDate = Settings.Default.BusinessDate;
                itemDef.whoDeclared = 0;
                DB.InsertNewDefectiveItem(itemDef);

                wpfMessageBox.Show("Inventories Management", strDefectiveItemSaved, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, string.Empty);

                // clean UI
                cbox_ItemsList.Text = string.Empty;
                txtItemQty.Text = string.Empty;
                btnAddDefective.IsEnabled = false;
                cbox_ItemsList.Focus();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }
    }
}
