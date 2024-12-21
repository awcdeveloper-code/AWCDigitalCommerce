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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucInventoryStatus.xaml
    /// </summary>
    public partial class ucInventoryStatus : UserControl
    {
        private List<clsItem> itemsList = new List<clsItem>();

        public ucInventoryStatus()
        {
            InitializeComponent();

            cbox_ItemType.Items.Add("TODOS");
            cbox_ItemType.Items.Add("BEBIDAS");
            cbox_ItemType.Items.Add("LICORES");
            cbox_ItemType.Items.Add("COMIDAS");
            cbox_ItemType.Items.Add("OCULTOS");
        }

        private void cbox_ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_ItemType.SelectedIndex == -1) return;

            if (cbox_ItemType.SelectedIndex == 4)
            {
                itemsList = DB.ListBindingInventory_tbl_Items(8);
            }
            else
            {
                itemsList = DB.ListBindingInventory_tbl_Items(cbox_ItemType.SelectedIndex);
            }
            dgInventoryStatus.ItemsSource = itemsList;

            // ENABLE BUTTONS
            if (dgInventoryStatus.Items.Count > 0)
            {
                btnPrintInventory.IsEnabled = true;
                btnNormalizeInventory.IsEnabled = true;
                btnExportInventory.IsEnabled = true;
            }
            else
            {
                btnPrintInventory.IsEnabled = false;
                btnNormalizeInventory.IsEnabled = false;
                btnExportInventory.IsEnabled = false;
            }
        }

        private void dgInventoryStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInventoryStatus.SelectedIndex == -1) return;

            if (dgInventoryStatus.SelectedIndex >= 0)
                btnUpdateItem.IsEnabled = true;
            else
                btnUpdateItem.IsEnabled = false;
        }

        private void btn_UpdateItem(object sender, RoutedEventArgs e)
        {
            clsItem selectedItem = dgInventoryStatus.SelectedItem as clsItem;

            this.Opacity = 0.5;
            wpfFixItemInventory wpfFix = new wpfFixItemInventory(selectedItem);
            wpfFix.ShowDialog();
            this.Opacity = 1;

            if (wpfFix.bCancel) return;

            selectedItem.ItemSubType = wpfFix.itemSubtype;
            selectedItem.ItemParent = wpfFix.itemParent;
            selectedItem.ItemParentUnit = wpfFix.itemParentUnit;
            selectedItem.ItemAvailable = wpfFix.itemAvail;
            selectedItem.ItemSold = wpfFix.itemSold;
            selectedItem.ItemDefective = wpfFix.itemDefective;
            selectedItem.ItemMinimum = wpfFix.itemMinimum;
            selectedItem.ItemStock = wpfFix.itemStock;

            if (DB.UpdateItemInventory("INI", selectedItem))
            {
                wpfMessageBox.Show("Inventory Management", "CONFIRMATION: The item " + selectedItem.ID.ToString() + " was updated successfully.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");

                itemsList = DB.ListBinding_tbl_Items(cbox_ItemType.SelectedIndex + 1);
                dgInventoryStatus.ItemsSource = itemsList;
            }
            else
                wpfMessageBox.Show("Inventory Management", "ATTENTION: The item " + selectedItem.ID.ToString() + " was not updated. Please, consult with the Administrator.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");

            itemsList = DB.ListBindingInventory_tbl_Items(cbox_ItemType.SelectedIndex);
            dgInventoryStatus.ItemsSource = itemsList;
            btnUpdateItem.IsEnabled = false;
        }

        private void btn_PrintInventory(object sender, RoutedEventArgs e)
        {
            string type = string.Empty;

            switch (cbox_ItemType.SelectedIndex)
            {
                case 0:
                    type = "TODOS";
                    break;
                case 1:
                    type = "BEBIDAS";
                    break;
                case 2:
                    type = "LICORES";
                    break;
                case 3:
                    type = "COMIDAS";
                    break;
                case 4:
                    type = "OCULTOS";
                    break;
            }

            //Helper.PrintInventory(itemsList);
            Helper.PrintInventoryByParts(itemsList, type);
        }

        private void btn_NormalizeInventory(object sender, RoutedEventArgs e)
        {
            string prodDesc = cbox_ItemType.Text;

            if (wpfMessageBox.Show("Inventory Management", $"ATENCIÓN: REALMENTE DESEA NORMALIZAR LOS PRODUCTOS ({cbox_ItemType.Text})?", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, "") == MessageBoxResult.Yes)
            {
                clsItem dump = new clsItem();
                dump.ItemType= cbox_ItemType.SelectedIndex;

                if (DB.UpdateItemInventory("NOR", dump))
                {
                    itemsList = DB.ListBindingInventory_tbl_Items(cbox_ItemType.SelectedIndex);
                    dgInventoryStatus.ItemsSource = itemsList;
                }
                else
                    wpfMessageBox.Show("Inventory Management", "ATTENTION: The normalization was not executed successfully. Please, consult with the Administrator.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }

        private void btn_ExportInventory(object sender, RoutedEventArgs e)
        {
            ReportsRepository.InventoryStatusSwiftExcel(Settings.Default.BusinessDate, itemsList);
            wpfMessageBox.Show("Inventory Management", "ATENCIÓN: ARCHIVO EXCEL FUE GENERADO EXITOSAMENTE" + Environment.NewLine + "REVISE EL CONTENIDO DE LA CARPETA 'REPORTES AWC'.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, "");
        }

        private void btn_SendeMail(object sender, RoutedEventArgs e)
        {
            if (!SMTP.CheckInternetConnection())
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EN ESTE MOMENTO NO HAY CONEXIÓN A INTERNET, POR FAVOR INTENTE MAS TARDE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
                return;
            }

            SMTP.EMailInventory(itemsList);
            wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: CORREO ENVIADO EXITOSAMENTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
        }
    }
}
