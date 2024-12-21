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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucDamagedProducts.xaml
    /// </summary>
    public partial class ucDamagedProducts : UserControl
    {
        private List<clsItem> lstProducts = new List<clsItem>();

        public ucDamagedProducts()
        {
            InitializeComponent();
            lstProducts = DB.ListBinding_tbl_Items(6);
        }

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
            btnApply.IsEnabled = true;
        }

        private void btn_Apply(object sender, RoutedEventArgs e)
        {
            try
            {
                // reduce inventory
                foreach (dgItemsList item in dgItemsList.Items)
                {
                    clsItem damagedItem = new clsItem();
                    damagedItem.ID = item.ID;
                    damagedItem.ItemDefective = item.ItemQty;
                    DB.UpdateItemInventory("DEF", damagedItem);

                    // add record to ItemsDefective table
                    clsItemDefective itemDef = new clsItemDefective();
                    itemDef.ItemID = item.ID;
                    itemDef.ItemQty = item.ItemQty;
                    itemDef.DeclarationDate = Settings.Default.BusinessDate;
                    itemDef.whoDeclared = Settings.Default.WhoOpen;
                    DB.InsertNewDefectiveItem(itemDef);
                }

                Helper.ShowToastNotification($"Transacción aplicada");
                dgItemsList.Items.Clear();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }
    }
}
