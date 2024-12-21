using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucTablesMaintenance2.xaml
    /// </summary>
    public partial class ucTablesMaintenance2 : UserControl
    {
        #region GLOBAL VARIBALES
        private int itemID = 0;
        private int parentID = 0;
        private int childID = 0;
        private int itemPreviousPrice = 0;
        private int itemCurrentPrice = 0;
        private string itemChildDesc = string.Empty;
        private string itemParentDesc = string.Empty;
        public bool bClean = false;
        private int delLevel = 1;
        #endregion

        public ucTablesMaintenance2()
        {
            InitializeComponent();

            // FREQUENT CUSTOMERS TAB
            CustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(4, 0);
            tsCustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(6, 0);

            // BEVERAGES/LIQOURS/MEALS GROUP
            ItemCatalog.ItemsSource = DB.ListBinding_tbl_Items(7);
            cbBox_ItemType.Items.Add("BEBIDAS");
            cbBox_ItemType.Items.Add("LICORES");
            cbBox_ItemType.Items.Add("COMIDAS");

            cbBox_ItemUnit.Items.Add("UNIDAD");
            cbBox_ItemUnit.Items.Add("MILILITROS");
            cbBox_ItemUnit.Items.Add("GRAMOS");

            // LIQOURS RELATIONS
            cbBox_ChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 100);
            cbBox_ParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 0);
            ItemParentCatalog.ItemsSource = DB.ListBinding_tbl_ParentItems();

            // MEALS RELATIONS
            cbBox_MealChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(3, 0);
            cbBox_MealParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(3, 3);
            MealItemParentCatalog.ItemsSource = DB.ListBinding_tbl_MealsRelationships();

            // BUCKETS RELATIONS
            cbBox_BucketParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(1, 2);
            cbBox_BucketChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(1, 0);
            BucketParentCatalog.ItemsSource = DB.GetBucketsList();

            //BEVERAGE PROMOS
            cbBox_PromoParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(1, 4);
            cbBox_PromoChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(1, 0);
            PromoCatalog.ItemsSource = DB.GetPromotionList(1);

            //LIQOURS PROMOS
            cbBox_PromoDrinkParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 4);
            cbBox_PromoDrinkChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 0);
            PromoDrinkCatalog.ItemsSource = DB.GetPromotionList(2);
        }
        #region CLEANERS
        private void CleanCustomerGroup()
        {
            bClean = true;

            txtBox_CustomerName.Text = string.Empty;
            txtBox_CreditLimit.Text = string.Empty;
            txtBox_LastPayment.Text = string.Empty;

            chkBox_Status.IsChecked = false;
            chkBox_ApplyServiceFee.IsChecked= false;
            chkBox_FreeOfCharge.IsChecked = false;

            Cancel_Cust.Visibility = Visibility.Hidden;
            Add_Cust.Visibility = Visibility.Hidden;
            Del_Cust.Visibility = Visibility.Hidden;
            Mod_Cust.Visibility = Visibility.Hidden;

            CustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(4, 0);

            txtBox_CustomerName.Focus();
            bClean = false;
        }
        private void CleanTablesAndSeatsGroup()
        {
            bClean = true;

            txtBox_tsName.Text = string.Empty;
            Cancel_tsCust.Visibility = Visibility.Hidden;
            Add_tsCust.Visibility = Visibility.Hidden;
            Del_tsCust.Visibility = Visibility.Hidden;

            tsCustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(6, 0);

            txtBox_CustomerName.Focus();

            bClean = false;
        }
        private void CleanItemGroup()
        {
            bClean = true;
            
            ItemCatalog.ItemsSource = DB.ListBinding_tbl_Items(7);
            
            txtBox_ItemName.Text = string.Empty;
            txtBox_ItemName.IsEnabled = true;
            txtBox_ItemPrice.Text = string.Empty;
            txtBox_ItemCost.Text = string.Empty;
            txtBox_ItemUnitNum.Text = string.Empty;

            cbBox_ItemType.SelectedIndex = -1;
            cbBox_ItemUnit.SelectedIndex = -1;
            chkBox_Active.IsChecked = false;
            chkBox_HideOnMenu.IsChecked = false;
            chkBox_SubType.IsEnabled = false;

            Cancel_Item.Visibility = Visibility.Hidden;
            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Hidden;
            Mod_Item.Visibility = Visibility.Hidden;

            txtBox_ItemName.Focus();
            
            bClean = false;
        }
        private void CleanRelationsGroup()
        {
            bClean = true;

            cbBox_ChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 100);
            cbBox_ParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2, 0);
            ItemParentCatalog.ItemsSource = DB.ListBinding_tbl_ParentItems();

            cbBox_ChildItem.Text = string.Empty;
            cbBox_ParentItem.Text = string.Empty;
            cbBox_ParentItem.IsEnabled = false;
            txtBox_ItemParentUnit.Text = string.Empty;
            txtBox_ItemParentUnit.IsEnabled = false;

            Add_ItemParent.Visibility= Visibility.Hidden;
            Del_ItemParent.Visibility= Visibility.Hidden;

            bClean = false;
        }
        private void CleanMealRelationsGroup()
        {
            bClean = true;

            cbBox_MealChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(3, 0);
            cbBox_MealParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(3, 3);
            MealItemParentCatalog.ItemsSource = DB.ListBinding_tbl_MealsRelationships();

            cbBox_MealChildItem.SelectedIndex = -1;
            cbBox_MealParentItem.SelectedIndex = -1;
            txtBox_MealItemParentUnit.Text = string.Empty;

            Add_MealItemParent.Visibility = Visibility.Hidden;
            Del_MealItemParent.Visibility = Visibility.Hidden;

            bClean = false;
        }
        private void CleanBucketRelationGroup()
        {
            bClean = true;

            cbBox_BucketParentItem.SelectedIndex = -1;
            cbBox_BucketChildItem.SelectedIndex = -1;

            Add_BucketItemParent.Visibility = Visibility.Hidden;
            Del_BucketItemParent.Visibility = Visibility.Hidden;

            bClean = false;
            delLevel = 1;
        }
        private void CleanPromoGroup()
        {
            bClean = true;

            cbBox_PromoParentItem.SelectedIndex = -1;
            cbBox_PromoChildItem.SelectedIndex = -1;
            txtBox_PromoChildItemQty.Text = string.Empty;
            txtBox_PromoChildItemQty.IsEnabled = false;

            Add_PromoItemParent.Visibility = Visibility.Hidden;
            Del_PromoItemParent.Visibility = Visibility.Hidden;

            PromoCatalog.ItemsSource = DB.GetPromotionList(1);

            bClean = false;
            delLevel = 1;
        }
        private void CleanPromoDrinkGroup()
        {
            bClean = true;

            cbBox_PromoDrinkParentItem.SelectedIndex = -1;
            cbBox_PromoDrinkChildItem.SelectedIndex = -1;
            txtBox_PromoDrinkChildItemQty.Text = string.Empty;
            txtBox_PromoDrinkChildItemQty.IsEnabled = false;

            Add_PromoDrinkItemParent.Visibility = Visibility.Hidden;
            Del_PromoDrinkItemParent.Visibility = Visibility.Hidden;

            PromoDrinkCatalog.ItemsSource = DB.GetPromotionList(2);

            bClean = false;
            delLevel = 1;
        }
        #endregion

        #region CUSTOMERS TAB
        private void txtBox_CustomerName_KeyUp(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    if (txtBox_CustomerName.Text.Length == 0) return;

                    clsCustomerVIP custProf = DB.GetCustomerProfile(txtBox_CustomerName.Text);

                    if (custProf.ID > 0)
                    {
                        itemID = custProf.ID;

                        txtBox_CustomerName.Text = custProf.CustomerID;
                        txtBox_CreditLimit.Text = custProf.CreditLimit.ToString();
                        txtBox_LastPayment.Text = custProf.LastPayment.ToString();

                        chkBox_Status.IsChecked = custProf.Active;
                        chkBox_ApplyServiceFee.IsChecked = custProf.ApplyServiceFee;
                        chkBox_FreeOfCharge.IsChecked = custProf.CustomerFOC;

                        Cancel_Cust.Visibility = Visibility.Visible;
                        Del_Cust.Visibility = Visibility.Visible;
                        Mod_Cust.Visibility = Visibility.Visible;

                        chkBox_Status.Focus();
                    }
                    else
                    {
                        txtBox_CreditLimit.Text = Settings.Default.CreditLimitByDefault.ToString();
                        txtBox_LastPayment.Text = DB.ConverTicketDate(Settings.Default.BusinessDate);

                        Cancel_Cust.Visibility = Visibility.Visible;
                        Add_Cust.Visibility = Visibility.Visible;
                    }
                }
            }
        private void CustCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bClean) return;

            clsCustomerVIP row = CustCatalog.SelectedItem as clsCustomerVIP;

            itemID = row.ID;

            txtBox_CustomerName.Text = row.CustomerID;
            txtBox_CreditLimit.Text = row.CreditLimit.ToString();
            txtBox_LastPayment.Text = row.LastPayment.ToString();

            chkBox_Status.IsChecked = row.Active;
            chkBox_ApplyServiceFee.IsChecked = row.ApplyServiceFee;
            chkBox_FreeOfCharge.IsChecked = row.CustomerFOC;

            Cancel_Cust.Visibility = Visibility.Visible;
            Add_Cust.Visibility = Visibility.Hidden;
            Del_Cust.Visibility = Visibility.Visible;
            Mod_Cust.Visibility = Visibility.Visible;

            chkBox_Status.Focus();
        }
        private void btnCancel_Cust(object sender, MouseButtonEventArgs e)
        {
            CleanCustomerGroup();
        }
        private void btnAdd_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtBox_CustomerName.Text.Length == 0) return;

                if (!DB.CustomerIDExist(txtBox_CustomerName.Text.ToUpper()))
                {
                    int status = chkBox_Status.IsChecked == true ? 1 : 0;
                    int serviceFee = chkBox_ApplyServiceFee.IsChecked == true ? 1 : 0;
                    int freeOfCharge = chkBox_FreeOfCharge.IsChecked == true ? 1 : 0;
                    int creditLimit = Convert.ToInt32(txtBox_CreditLimit.Text);

                    DB.InsertNewCustomer(txtBox_CustomerName.Text, 1, 0, status, serviceFee, freeOfCharge, creditLimit);
                    Helper.ShowToastNotification($"{txtBox_CustomerName.Text} agregado");
                }
                else
                    wpfMessageBox.Show("Tickets Controller", $"CLIENTE {txtBox_CustomerName.Text.ToUpper()} YA EXISTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");

                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        private void btnDel_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //List<clsTicketsForDataGrid> openTicketPerCustomerName = DB.DataBinding_tbl_Tickets(itemID, 1);

                //if (openTicketPerCustomerName.Count > 0)
                //{
                //    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: CLIENTE NO PUEDE SER ELIMINADO DEBIDO A QUE TIENE CUENTAS PENDIENTES.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                //    CleanCustomerGroup();
                //    return;
                //}

                if (wpfMessageBox.Show("Tickets Controller", $"CLIENTE: {txtBox_CustomerName.Text}" + Environment.NewLine + "DESEA ELIMINAR EL HISTORIAL DE ESTE CLIENTE (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, "") == MessageBoxResult.Yes)
                {
                    DB.DeleteHistoryByCustomerID(itemID);
                }

                DB.DeleteCustomer(itemID);
                Helper.ShowToastNotification($"{txtBox_CustomerName.Text} eliminado");
                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        private void btnMod_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int credLim = Convert.ToInt32(txtBox_CreditLimit.Text);
                int status = chkBox_Status.IsChecked == true ? 1:0;
                int appSvc = chkBox_ApplyServiceFee.IsChecked == true ? 1:0;
                int foc = chkBox_FreeOfCharge.IsChecked == true ? 1 : 0;

                DB.UpdateCustomerProfile(itemID, status, appSvc, foc, credLim);
                Helper.ShowToastNotification($"{txtBox_CustomerName.Text} modificado");
                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        #endregion

        #region PRODUCTS TAB
        private void txtBox_ItemName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (txtBox_ItemName.Text.Length == 0) return;

                clsItem item = DB.GetItem(DB.GetIDByItemDescription(txtBox_ItemName.Text.ToUpper()));

                if (item.ID > 0)
                {
                    bClean = true;
                    itemID = item.ID;

                    txtBox_ItemName.IsEnabled = false;

                    if (item.ItemType == 1)
                        cbBox_ItemType.Text = "BEBIDAS";
                    else if (item.ItemType == 2)
                        cbBox_ItemType.Text = "LICORES";
                    else
                    {
                        cbBox_ItemType.Text = "COMIDAS";
                        chkBox_HideOnMenu.IsEnabled = true;
                    }

                    switch (item.ItemUnitOfMeasurement)
                    {
                        case 0:
                            cbBox_ItemUnit.Text = "UNIDAD";
                            txtBox_ItemUnitNum.IsEnabled = false;
                            break;
                        case 1:
                            cbBox_ItemUnit.Text = "MILILITROS";
                            txtBox_ItemUnitNum.IsEnabled = true;
                            break;
                        case 2:
                            cbBox_ItemUnit.Text = "GRAMOS";
                            txtBox_ItemUnitNum.IsEnabled = true;
                            break;
                    }

                    txtBox_ItemUnitNum.Text = Convert.ToString(item.ItemUnitSize);
                    txtBox_ItemPrice.Text = Convert.ToString(item.UnitPrice);
                    txtBox_ItemCost.Text = Convert.ToString(item.UnitCost);
                    chkBox_Active.IsEnabled = item.IsActive == true ? true : false;
                    chkBox_HideOnMenu.IsEnabled = item.ItemSubType == 3 ? true : false;
                    chkBox_SubType.IsEnabled = item.ItemSubType == 1 ? true : false;
                    chkBox_Promo.IsEnabled = item.ItemSubType == 4 ? true : false;

                    Add_Item.Visibility = Visibility.Hidden;
                    Del_Item.Visibility = Visibility.Visible;
                    Mod_Item.Visibility = Visibility.Visible;

                    cbBox_ItemType.Focus();
                    bClean = false;
                }
                else
                {
                    chkBox_Active.IsEnabled = true;
                    chkBox_Active.IsChecked = true;
                    chkBox_HideOnMenu.IsEnabled = true;
                    chkBox_SubType.IsEnabled = true;
                    chkBox_Promo.IsEnabled = true;

                    Cancel_Item.Visibility = Visibility.Visible;
                    Add_Item.Visibility = Visibility.Visible;
                    cbBox_ItemType.Focus();
                }
            }
        }
        private void ItemCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bClean) return;

            clsItem row = ItemCatalog.SelectedItem as clsItem;

            txtBox_ItemName.Text = row.ItemDescription;
            txtBox_ItemName.IsEnabled = false;
            chkBox_Active.IsChecked = row.IsActive;
            chkBox_Active.IsEnabled = true;
            chkBox_HideOnMenu.IsChecked = row.ItemSubType == 3 ? true : false;
            chkBox_HideOnMenu.IsEnabled = true;
            chkBox_SubType.IsChecked = row.ItemSubType == 2 ? true : false;
            chkBox_SubType.IsEnabled = true;

            switch (row.ItemType)
            {
                case 1:
                    cbBox_ItemType.Text = "BEBIDAS";
                    break;
                case 2:
                    cbBox_ItemType.Text = "LICORES";
                    break;
                case 3:
                    {
                        cbBox_ItemType.Text = "COMIDAS";
                        chkBox_HideOnMenu.IsEnabled = true;
                        chkBox_HideOnMenu.IsChecked = row.ItemSubType == 3 ? true : false;
                    }
                    break;
            }

            switch(row.ItemUnitOfMeasurement)
            {
                case 0:
                    cbBox_ItemUnit.Text = "UNIDAD";
                    break;
                case 1:
                    cbBox_ItemUnit.Text = "MILILITROS";
                    break;
                case 2:
                    cbBox_ItemUnit.Text = "GRAMOS";
                    break;
            }

            txtBox_ItemUnitNum.Text = Convert.ToString(row.ItemUnitSize);
            txtBox_ItemPrice.Text = Convert.ToString(row.UnitPrice);
            txtBox_ItemCost.Text = Convert.ToString(row.UnitCost);

            itemID = row.ID;
            itemPreviousPrice = row.UnitPrice;
            itemCurrentPrice = 0;

            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Visible;
            Mod_Item.Visibility = Visibility.Visible;
            cbBox_ItemType.Focus();
        }
        private void cbBox_ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!bClean)
            {
                switch(cbBox_ItemType.SelectedItem.ToString())
                {
                    case "BEBIDAS":
                        bClean = true;
                        cbBox_ItemUnit.SelectedIndex = 0;
                        cbBox_ItemUnit.IsEnabled = false;
                        txtBox_ItemUnitNum.IsEnabled = false;
                        txtBox_ItemUnitNum.Text = "1";
                        txtBox_ItemCost.Text = "0";
                        txtBox_ItemPrice.Focus();
                        bClean = false;
                        break;
                    case "LICORES":
                    case "COMIDAS":
                        txtBox_ItemUnitNum.Text = "1";
                        txtBox_ItemUnitNum.IsEnabled = true;
                        txtBox_ItemCost.Text = "0";
                        cbBox_ItemUnit.IsEnabled = true;
                        chkBox_HideOnMenu.IsEnabled = true;
                        cbBox_ItemUnit.Focus();
                        break;
                }
            }
        }
        private void btnCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanItemGroup();
        }
        private void btnAdd_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                itemID = cbBox_ItemType.SelectedIndex + 1;

                int isSubtype = 0;
                bool ia = chkBox_Active.IsChecked == true ? true : false;

                if (chkBox_SubType.IsChecked == true)
                    isSubtype = 2;

                if (chkBox_HideOnMenu.IsChecked == true)
                    isSubtype = 3;
                
                if (chkBox_Promo.IsChecked == true)
                {
                    isSubtype = 4;
                }

                DB.InsertNewItem(itemID,
                                 txtBox_ItemName.Text.ToUpper(),
                                 Convert.ToInt32(txtBox_ItemPrice.Text),
                                 Convert.ToInt32(txtBox_ItemCost.Text),
                                 cbBox_ItemUnit.SelectedIndex,
                                 Convert.ToInt32(txtBox_ItemUnitNum.Text), ia, isSubtype);
                Helper.ShowToastNotification($"{txtBox_ItemName.Text.ToUpper()} agregado");
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        private void btnDel_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DB.DeleteItem(itemID);
                Helper.ShowToastNotification($"{txtBox_ItemName.Text.ToUpper()} eliminado");
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        private void btnMod_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool ia = chkBox_Active.IsChecked == true ? true : false;
                int isSubtype = 0;

                if (chkBox_HideOnMenu.IsChecked == true)
                    isSubtype = 3;

                if (chkBox_SubType.IsChecked == true)
                    isSubtype = 2;

                DB.UpdateItem(itemID, (cbBox_ItemType.SelectedIndex + 1), Convert.ToInt32(txtBox_ItemPrice.Text), Convert.ToInt32(txtBox_ItemCost.Text), ia, isSubtype);

                DB.InsertItemsChangePrice(itemID, itemPreviousPrice, Convert.ToInt32(txtBox_ItemPrice.Text));

                Helper.ShowToastNotification($"{txtBox_ItemName.Text.ToUpper()} modificado");

                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        #endregion

        #region LIQOURS RELATIONSHIPS TAB
        private void btnAdd_ItemParent(object sender, MouseButtonEventArgs e)
        {
            if (cbBox_ChildItem.Text.Length == 0) return;
            if (cbBox_ParentItem.Text.Length == 0) return;
            if (txtBox_ItemParentUnit.Text.Length == 0) return;

            if (!DB.UpdateOriginDestinyRelation(itemChildDesc, itemParentDesc, Convert.ToInt32(txtBox_ItemParentUnit.Text)))
                wpfMessageBox.Show("Ticket Controller", "ERROR: NO SE LOGRÓ SALVAR LA RELACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            else
                Helper.ShowToastNotification("Relación exitosa");
            
            CleanRelationsGroup();
        }
        private void btnDel_ItemParent(object sender, MouseButtonEventArgs e)
        {
            clsItem row = ItemParentCatalog.SelectedItem as clsItem;

            if (!DB.DeleteOriginDestinyRelation(row.ID))
                wpfMessageBox.Show("Ticket Controller", "ERROR: NO SE LOGRÓ BORRAR LA RELACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            else
                Helper.ShowToastNotification("Relación borrada");

            CleanRelationsGroup();
        }
        private void cbBox_ChildItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_ChildItem.SelectedIndex == -1) return;

            itemChildDesc = cbBox_ChildItem.SelectedItem.ToString();

            cbBox_ParentItem.IsEnabled = true;
        }
        private void cbBox_ParentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_ChildItem.SelectedIndex == -1) return;

            itemParentDesc = cbBox_ParentItem.SelectedItem.ToString();

            // check if child and parent are equals
            if (itemParentDesc.Equals(itemChildDesc))
            {
                wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: RELACIÓN ES INVÁLIDA, ORIGEN Y DESTINO NO PUEDEN IGUALES.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                return;
            }

            // check if relation already exist
            if (DB.CheckOriginDestinyRelation(itemChildDesc, itemParentDesc))
            {
                wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: LA RELACIÓN YA EXISTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                return;
            }

            Add_ItemParent.Visibility = Visibility.Visible;
            txtBox_ItemParentUnit.IsEnabled = true;
            txtBox_ItemParentUnit.Focus();
        }
        private void ItemParentCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Add_ItemParent.Visibility = Visibility.Hidden;
            Del_ItemParent.Visibility = Visibility.Visible;
        }
        #endregion

        #region MEALS RELATIONSHIPS TAB
        private void btnAdd_MealItemParent(object sender, MouseButtonEventArgs e)
        {
            if (cbBox_MealChildItem.Text.Length == 0) return;
            if (cbBox_MealParentItem.Text.Length == 0) return;
            if (txtBox_MealItemParentUnit.Text.Length == 0) return;

            if (!DB.InsertMealOriginDestinyRelation(3, itemChildDesc, itemParentDesc, Convert.ToInt32(txtBox_MealItemParentUnit.Text), 1))
                wpfMessageBox.Show("Ticket Controller", "ERROR: NO SE LOGRÓ SALVAR LA RELACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            else
                Helper.ShowToastNotification("Relación exitosa");

            CleanMealRelationsGroup();
        }
        private void btnDel_MealItemParent(object sender, MouseButtonEventArgs e)
        {
            clsItem row = MealItemParentCatalog.SelectedItem as clsItem;

            if (!DB.DeleteMealOriginDestinyRelation(row.ID,row.ItemParent))
                wpfMessageBox.Show("Ticket Controller", "ERROR: NO SE LOGRÓ BORRAR LA RELACIÓN.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            else
                Helper.ShowToastNotification("Relación borrada");

            CleanMealRelationsGroup();
        }
        private void cbBox_MealChildItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_MealChildItem.SelectedIndex == -1) return;

            if (!bClean)
            {
                itemChildDesc = cbBox_MealChildItem.SelectedItem.ToString();
                cbBox_ParentItem.IsEnabled = true;
            }
        }
        private void cbBox_MealParentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_MealChildItem.SelectedIndex == -1) return;

            if (!bClean)
            {
                itemParentDesc = cbBox_MealParentItem.SelectedItem.ToString();

                // check if child and parent are equals
                if (itemParentDesc.Equals(itemChildDesc))
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: RELACIÓN ES INVÁLIDA, ORIGEN Y DESTINO NO PUEDEN IGUALES.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                    CleanMealRelationsGroup();
                    return;
                }

                // check if relation already exist
                if (DB.CheckMealOriginDestinyRelation(itemChildDesc, itemParentDesc))
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: LA RELACIÓN YA EXISTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, "");
                    CleanMealRelationsGroup();
                    return;
                }

                Add_MealItemParent.Visibility = Visibility.Visible;
                txtBox_MealItemParentUnit.IsEnabled = true;
                txtBox_MealItemParentUnit.Focus();
            }
        }
        private void MealItemParentCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Add_MealItemParent.Visibility = Visibility.Hidden;
            Del_MealItemParent.Visibility = Visibility.Visible;
        }
        #endregion

        #region BUCKETS RELATIONSHIPS TAB
        private void cbBox_BucketParentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_BucketParentItem.SelectedIndex == -1) return;

            itemParentDesc = cbBox_BucketParentItem.SelectedItem.ToString();

            cbBox_BucketChildItem.IsEnabled = true;
        }

        private void cbBox_BucketChildItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_BucketChildItem.SelectedIndex == -1) return;

            itemChildDesc = cbBox_BucketChildItem.SelectedItem.ToString();

            parentID = DB.GetIDByItemDescription(itemParentDesc);
            childID = DB.GetIDByItemDescription(itemChildDesc);

            if (DB.CheckThisBucketRelation(parentID, childID))
            {
                Del_BucketItemParent.Visibility = Visibility.Visible;
            }
            else
            {
                Add_BucketItemParent.Visibility = Visibility.Visible;
            }
        }

        private void BucketParentCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!bClean)
                {
                    clsItem item = BucketParentCatalog.SelectedItem as clsItem;

                    if (item != null)
                    {
                        BucketChildCatalog.ItemsSource = DB.GetBucketItemsList(item.ID);
                    }

                    parentID = item.ID;
                    Del_BucketItemParent.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        private void BucketChildCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!bClean)
            {
                clsItem item = BucketChildCatalog.SelectedItem as clsItem;
                childID = item.ID;
                Del_BucketItemParent.Visibility = Visibility.Visible;
                delLevel = 2;
            }
        }

        private void btnAdd_BucketParentChild(object sender, MouseButtonEventArgs e)
        {
            bClean = true;

            if (DB.InsertThisBucketRelation(parentID, childID))
            {
                Helper.ShowToastNotification("Relación exitosa");
                BucketParentCatalog.ItemsSource = DB.GetBucketsList();
                BucketChildCatalog.ItemsSource = DB.GetBucketItemsList(parentID);
            }
            CleanBucketRelationGroup();
            bClean = false;
        }

        private void btnDel_BucketChildParent(object sender, MouseButtonEventArgs e)
        {
            switch (delLevel)
            {
                case 1:
                    if (wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: DESEA BORRAR ESTE BALDE (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, "") == MessageBoxResult.Yes)
                    {
                        bClean = true;

                        if (DB.DeleteThisBucket(parentID))
                        {
                            BucketParentCatalog.ItemsSource = DB.GetBucketsList();
                            BucketChildCatalog.ItemsSource = null;
                        }

                        Helper.ShowToastNotification("Producto eliminado");
                        CleanBucketRelationGroup();

                        bClean = false;
                    }
                    break;
                case 2:
                    if (wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: DESEA BORRAR ESTE PRODUCTO DEL BALDE (SI/NO)", MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, "") == MessageBoxResult.Yes)
                    {
                        bClean = true;

                        if (DB.DeleteThisBucketRelation(parentID, childID))
                        {
                            BucketChildCatalog.ItemsSource = DB.GetBucketItemsList(parentID);
                            Helper.ShowToastNotification("Relación borrada");
                        }
                        CleanBucketRelationGroup();

                        bClean = false;
                    }
                    break;
            }
        }

        #endregion

        #region TABLES AND SEATS
        private void tsCustCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bClean) return;

            clsCustomerVIP row = tsCustCatalog.SelectedItem as clsCustomerVIP;

            itemID = row.ID;

            txtBox_tsName.Text = row.CustomerID;

            Cancel_tsCust.Visibility = Visibility.Visible;
            Add_tsCust.Visibility = Visibility.Hidden;
            Del_tsCust.Visibility = Visibility.Visible;
        }

        private void txtBox_tsName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtBox_tsName.Text.Length == 0)
                {
                    return;
                }
                else
                {
                    txtBox_tsName.Text = txtBox_tsName.Text.ToUpper();
                }

                clsCustomerVIP ts = DB.GetCustomerProfile(txtBox_tsName.Text);

                if (ts.ID > 0)
                {
                    bClean = true;
                    itemID = ts.ID;

                    txtBox_tsName.IsEnabled = false;

                    Add_tsCust.Visibility = Visibility.Hidden;
                    Del_tsCust.Visibility = Visibility.Visible;

                    bClean = false;
                }
                else
                {
                    Cancel_tsCust.Visibility = Visibility.Visible;
                    Add_tsCust.Visibility = Visibility.Visible;
                    Del_tsCust.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnCancel_tsCust(object sender, MouseButtonEventArgs e)
        {
            CleanTablesAndSeatsGroup();
        }

        private void btnAdd_tsCust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtBox_tsName.Text.Length == 0) return;

                if (!DB.CustomerIDExist(txtBox_tsName.Text.ToUpper()))
                {
                    DB.InsertNewTableSeat(txtBox_tsName.Text);
                    Helper.ShowToastNotification($"{txtBox_tsName.Text} agregado");
                }
                else
                    wpfMessageBox.Show("Tickets Controller", $"MESA/BARRA {txtBox_tsName.Text.ToUpper()} YA EXISTE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");

                CleanTablesAndSeatsGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }

        private void btnDel_tsCust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DB.DeleteCustomer(itemID);
                Helper.ShowToastNotification($"{txtBox_tsName.Text.ToUpper()} eliminado");
                CleanTablesAndSeatsGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        #endregion

        #region BEVERAGES PROMOS
        private void PromoCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Del_PromoItemParent.Visibility = Visibility.Visible;
        }

        private void cbBox_PromoParentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string promoDesc = string.Empty;

            if (!bClean)
            {
                promoDesc = cbBox_PromoParentItem.SelectedItem.ToString();
                clsPromoConfig promo = DB.GetPromotion(DB.GetIDByItemDescription(promoDesc));

                if (promo.ID == 0)
                {
                    cbBox_PromoChildItem.IsEnabled = true;
                    Add_PromoItemParent.Visibility = Visibility.Visible;
                }
            }
        }

        private void cbBox_PromoChildItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtBox_PromoChildItemQty.IsEnabled = true;
        }

        private void txtBox_PromoChildItemQty_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtBox_PromoChildItemQty.Text.Length == 0) return;

                Del_PromoItemParent.Visibility = Visibility.Visible;
            }
        }

        private void btnPromoCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanPromoGroup();
        }

        private void btnAdd_PromoParentChild(object sender, MouseButtonEventArgs e)
        {
            int promoParentID = DB.GetIDByItemDescription(cbBox_PromoParentItem.SelectedItem.ToString());
            int promoChildID = DB.GetIDByItemDescription(cbBox_PromoChildItem.SelectedItem.ToString());
            int promoQty = Convert.ToInt32(txtBox_PromoChildItemQty.Text);

            DB.InsertPromotion(1, promoParentID, promoChildID, promoQty);

            CleanPromoGroup();
        }

        private void btnDel_PromoChildParent(object sender, MouseButtonEventArgs e)
        {
            if (bClean) return;

            clsPromoConfig promo = PromoCatalog.SelectedItem as clsPromoConfig;
            DB.DeletePromotion(promo.PromoID);

            CleanPromoGroup();
        }
        #endregion

        #region LIQUOURS PROMOS
        private void PromoDrinkCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Del_PromoDrinkItemParent.Visibility = Visibility.Visible;
        }

        private void cbBox_PromoDrinkParentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string promoDesc = string.Empty;

            if (!bClean)
            {
                promoDesc = cbBox_PromoDrinkParentItem.SelectedItem.ToString();
                clsPromoConfig promo = DB.GetPromotion(DB.GetIDByItemDescription(promoDesc));

                if (promo.ID == 0)
                {
                    cbBox_PromoDrinkChildItem.IsEnabled = true;
                    Add_PromoDrinkItemParent.Visibility = Visibility.Visible;
                }
            }
        }

        private void cbBox_PromoDrinkChildItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtBox_PromoDrinkChildItemQty.IsEnabled = true;
        }

        private void txtBox_PromoDrinkChildItemQty_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtBox_PromoDrinkChildItemQty.Text.Length == 0) return;

                Del_PromoDrinkItemParent.Visibility = Visibility.Visible;
            }
        }

        private void btnPromoDrinkCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanPromoDrinkGroup();
        }

        private void btnAdd_PromoDrinkParentChild(object sender, MouseButtonEventArgs e)
        {
            int promoParentID = DB.GetIDByItemDescription(cbBox_PromoDrinkParentItem.SelectedItem.ToString());
            int promoChildID = DB.GetIDByItemDescription(cbBox_PromoDrinkChildItem.SelectedItem.ToString());
            int promoQty = Convert.ToInt32(txtBox_PromoDrinkChildItemQty.Text);

            DB.InsertPromotion(2, promoParentID, promoChildID, promoQty);
            CleanPromoDrinkGroup();
        }

        private void btnDel_PromoDrinkChildParent(object sender, MouseButtonEventArgs e)
        {
            if (bClean) return;

            clsPromoConfig promo = PromoDrinkCatalog.SelectedItem as clsPromoConfig;
            DB.DeletePromotion(promo.PromoID);

            CleanPromoDrinkGroup();
        }
        #endregion
    }
}
