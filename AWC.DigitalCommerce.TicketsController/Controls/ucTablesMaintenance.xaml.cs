using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ucTablesMaintenance.xaml
    /// </summary>
    public partial class ucTablesMaintenance : UserControl
    {
        private int itemID = 0;
        private string itemChildDesc = string.Empty;
        private string itemParentDesc = string.Empty;

        // MESSAGES
        private string lang = string.Empty;
        public string strCustomerExist = string.Empty;
        public string strActionResult = string.Empty;
        public string strActionQuestion = string.Empty;
        public string strChildParentEquals = string.Empty;
        public string strRelationAlreadyExist = string.Empty;
        public string strRelationCreated = string.Empty;
        public string strRelationFailed = string.Empty;
        public string strRelationDelete = string.Empty;
        public string strRelationDeleted = string.Empty;
        public string strRelationDeleteFailed = string.Empty;
        public bool bClean = false;

        public ucTablesMaintenance(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this,lang);

            // CUSTOMERS GROUP
            cbBox_CustomerType.Items.Add("CLIENTE FRECUENTE");
            cbBox_CustomerType.Items.Add("MESA/ASIENTO");

            Add_Cust.IsEnabled = true;
            Del_Cust.IsEnabled = false;

            CustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(0, 0);

            // BEVERAGES/LIQOURS/MEALS GROUP
            cbBox_ItemType.Items.Add("BEBIDAS");
            cbBox_ItemType.Items.Add("LICORES");
            cbBox_ItemType.Items.Add("COMIDAS");

            cbBox_ItemUnit.Items.Add("UN");
            cbBox_ItemUnit.Items.Add("ML");

            Add_Item.IsEnabled = true;
            Del_Item.IsEnabled = false;
            Mod_Item.IsEnabled = false;

            ItemCatalog.ItemsSource = DB.ListBinding_tbl_Items(0);

            // RELATIONS
            Add_ItemParent.IsEnabled = true;
            Del_ItemParent.IsEnabled = false;

            cbBox_ChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2,0);    // items NO parent (possible child)
            cbBox_ParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2,0);   // items NO parent (possible parent)
            ItemParentCatalog.ItemsSource = DB.ListBinding_tbl_ParentItems();   // load relations for datagrid
        }

        #region CLEANERS
        private void CleanCustomerGroup()
        {
            bClean = true;
            CustCatalog.ItemsSource = DB.ListBinding_tbl_CustomerID(0, 0);
            txtBox_CustomerName.Text = string.Empty;
            cbBox_CustomerType.Text = string.Empty;
            Add_Cust.IsEnabled = true;
            Del_Cust.IsEnabled = false;
            txtBox_CustomerName.Focus();
            bClean = false;
        }
        private void CleanItemGroup()
        {
            bClean = true;
            ItemCatalog.ItemsSource = DB.ListBinding_tbl_Items(0);
            txtBox_ItemName.Text = string.Empty;
            txtBox_ItemName.IsEnabled = true;
            txtBox_ItemPrice.Text = string.Empty;
            txtBox_ItemCost.Text = string.Empty;
            txtBox_ItemUnitNum.Text = string.Empty;
            cbBox_ItemType.Text = string.Empty;
            cbBox_ItemUnit.Text = string.Empty;


            Add_Item.IsEnabled = true;
            Del_Item.IsEnabled = false;
            Mod_Item.IsEnabled = false;
            txtBox_ItemName.Focus();
            bClean = false;
        }
        private void CleanRelationsGroup()
        {
            bClean = true;

            cbBox_ChildItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2,0);
            cbBox_ParentItem.ItemsSource = DB.ListBinding_tbl_ItemSubType(2,0);
            ItemParentCatalog.ItemsSource = DB.ListBinding_tbl_ParentItems();

            cbBox_ChildItem.Text = string.Empty;
            cbBox_ParentItem.Text = string.Empty;
            cbBox_ParentItem.IsEnabled = false;
            txtBox_ItemParentUnit.Text = string.Empty;
            txtBox_ItemParentUnit.IsEnabled = false;
            Add_ItemParent.IsEnabled = true;
            Del_ItemParent.IsEnabled = false;

            bClean = false;
        }
        #endregion

        #region CUSTOMER GROUP
        private void CustCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bClean) return;

            clsCustomerVIP row = CustCatalog.SelectedItem as clsCustomerVIP;

            txtBox_CustomerName.Text = row.CustomerID;
            txtBox_CreditLimit.Text = row.CreditLimit.ToString();
            ApplyServiceFee.IsChecked = row.ApplyServiceFee;

            if (row.Type == 1)
                cbBox_CustomerType.Text = "CLIENTE FRECUENTE";
            else
                cbBox_CustomerType.Text = "MESA/ASIENTO";

            itemID = row.ID;

            Add_Cust.IsEnabled = false;
            Del_Cust.IsEnabled = true;
            txtBox_CustomerName.Focus();
        }
        private void cbBox_CustomerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBox_CustomerType.SelectedIndex > 0)
            {
                txtBox_CreditLimit.Text = "0";
                txtBox_CreditLimit.IsEnabled = false;
            }
            else
            {
                txtBox_CreditLimit.Text = "50000";
                txtBox_CreditLimit.IsEnabled = true;
            }
        }
        private void btnCancel_Cust(object sender, MouseButtonEventArgs e)
        {
            CleanCustomerGroup();
        }
        private void btnAdd_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtBox_CustomerName.Text.Length == 0) return;   // Name Empty
                if (cbBox_CustomerType.Text.Length == 0) return;    // Type Empty

                itemID = cbBox_CustomerType.SelectedIndex + 1;

                if (!DB.CustomerIDExist(txtBox_CustomerName.Text))  // Name NO exist
                {
                    int serviceFee = ApplyServiceFee.IsChecked == true ? 1 : 0;
                    int creditLimit = Convert.ToInt32(txtBox_CreditLimit.Text);

                    // string custID, int type, int subType, int status, int serviceFee, int freeOfcharge, int creditLimit)
                    if (DB.InsertNewCustomer(txtBox_CustomerName.Text, itemID, 0, 0, serviceFee, 0, creditLimit))
                        wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_CustomerName.Text.ToUpper(), "ADDED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                }
                else
                    wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerExist, txtBox_CustomerName.Text.ToUpper()), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);

                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }        
        private void btnDel_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<clsTicketsForDataGrid> openTicketPerCustomerName = DB.DataBinding_tbl_Tickets(txtBox_CustomerName.Text, 1);

                if (openTicketPerCustomerName.Count > 0)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: CLIENTE/MESA/ASIENTO NO PUEDE SER ELIMINADO DEBIDO A QUE TIENE CUENTAS PENDIENTES.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                }
                else if (wpfMessageBox.Show("Tickets Controller", string.Format(strActionQuestion, "DELETE"), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                {
                    if (DB.DeleteCustomer(itemID))
                    {
                        wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_CustomerName.Text.ToUpper(), "DELETED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                    }
                }
                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }
        private void btnMod_Cust(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //if (txtBox_BirthDay.Text.Length == 0) return;   // BirthDay Empty

                //if (wpfMessageBox.Show("Tickets Controller", string.Format(strActionQuestion, "MODIFY"), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                //{
                //    if (DB.UpdateCustomerBirthDate(itemID, txtBox_BirthDay.Text, Convert.ToInt32(txtBox_CreditLimit.Text)))
                //        wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_CustomerName.Text.ToUpper(), "MODIFIED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                //}
                CleanCustomerGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }
        #endregion

        #region BEVERAGES/LIQOURS/MEALS GROUP
        private void ItemCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bClean) return;

            clsItem row = ItemCatalog.SelectedItem as clsItem;

            txtBox_ItemName.Text = row.ItemDescription;
            txtBox_ItemName.IsEnabled = false;
            txtBox_ItemPrice.Text = Convert.ToString(row.UnitPrice);
            txtBox_ItemCost.Text = Convert.ToString(row.UnitCost);

            if (row.ItemType == 1)
                cbBox_ItemType.Text = "BEBIDAS";
            else if (row.ItemType == 2)
                cbBox_ItemType.Text = "LICORES";
            else
                cbBox_ItemType.Text = "COMIDAS";

            itemID = row.ID;

            Add_Item.IsEnabled = false;
            Del_Item.IsEnabled = true;
            Mod_Item.IsEnabled = true;
            txtBox_ItemPrice.Focus();
        }
        private void txtBox_ItemPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            txtBox_ItemCost.Text = "0";
        }
        private void btnCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanItemGroup();
        }
        private void btnAdd_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtBox_ItemName.Text.Length == 0) return;   // Name Empty
                if (cbBox_ItemType.Text.Length == 0) return;    // Type Empty
                if (txtBox_ItemPrice.Text.Length == 0) return;  // Price Empty
                if (txtBox_ItemCost.Text.Length == 0) return;   // Cost Empty

                itemID = cbBox_ItemType.SelectedIndex + 1;

                if (DB.GetIDByItemDescription(txtBox_ItemName.Text.ToUpper()) == 0)
                {
                    if (DB.InsertNewItem(itemID,
                                         txtBox_ItemName.Text.ToUpper(),
                                         Convert.ToInt32(txtBox_ItemPrice.Text),
                                         Convert.ToInt32(txtBox_ItemCost.Text),
                                         cbBox_ItemUnit.SelectedIndex,
                                         Convert.ToInt32(txtBox_ItemUnitNum.Text), true, 0)
                        )
                        wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_ItemName.Text.ToUpper(), "ADDED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                    else
                        wpfMessageBox.Show("Tickets Controller", string.Format(strCustomerExist, txtBox_ItemName.Text.ToUpper()), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                }
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }
        private void btnDel_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (wpfMessageBox.Show("Tickets Controller", string.Format(strActionQuestion, "DELETE"), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
                {
                    if (DB.DeleteItem(itemID))
                        wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_ItemName.Text.ToUpper(), "DELETED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                }
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }
        private void btnMod_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txtBox_ItemName.Text.Length == 0) return;   // Name Empty
                if (txtBox_ItemPrice.Text.Length == 0) return;  // Price Empty
                if (txtBox_ItemCost.Text.Length == 0) return;   // Cost Empty
                if (cbBox_ItemType.Text.Length == 0) return;    // Type Empty

                if (DB.UpdateItem(itemID, (cbBox_ItemType.SelectedIndex + 1), Convert.ToInt32(txtBox_ItemPrice.Text), Convert.ToInt32(txtBox_ItemCost.Text), true, 0))
                    wpfMessageBox.Show("Tickets Controller", string.Format(strActionResult, txtBox_ItemName.Text.ToUpper(), "MODIFIED"), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);

                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }
        }
        #endregion

        #region RELATIONS
        private void btnAdd_ItemParent(object sender, MouseButtonEventArgs e)
        {
            if (cbBox_ChildItem.Text.Length == 0) return;
            if (cbBox_ParentItem.Text.Length == 0) return;
            if (txtBox_ItemParentUnit.Text.Length == 0) return;

            if (DB.UpdateOriginDestinyRelation(itemChildDesc, itemParentDesc, Convert.ToInt32(txtBox_ItemParentUnit.Text)))
                wpfMessageBox.Show("Ticket Controller", strRelationCreated, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
            else
                wpfMessageBox.Show("Ticket Controller", strRelationFailed, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);

            CleanRelationsGroup();
        }

        private void btnDel_ItemParent(object sender, MouseButtonEventArgs e)
        {
            if (wpfMessageBox.Show("Tickets Controller", strRelationDelete, MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            {
                clsItem row = ItemParentCatalog.SelectedItem as clsItem;

                if (DB.DeleteOriginDestinyRelation(row.ID))
                    wpfMessageBox.Show("Ticket Controller", strRelationDeleted, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
                else
                    wpfMessageBox.Show("Ticket Controller", strRelationDeleteFailed, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
            }

            CleanRelationsGroup();
        }
        #endregion

        #region SelectionChanged
        private void cbBox_ItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!bClean)
            {
                if (cbBox_ItemType.SelectedItem.ToString() == "LICORES")
                    LiquorUnit.IsEnabled = true;
                else
                {
                    bClean = true;
                    cbBox_ItemUnit.SelectedIndex = 0;
                    txtBox_ItemUnitNum.Text = "1";
                    LiquorUnit.IsEnabled = false;
                    bClean = false;
                }
            }
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
                wpfMessageBox.Show("Ticket Controller", strChildParentEquals, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            // check if relation already exist
            if (DB.CheckOriginDestinyRelation(itemChildDesc, itemParentDesc))
            {
                wpfMessageBox.Show("Ticket Controller", strRelationAlreadyExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            txtBox_ItemParentUnit.IsEnabled = true;
            txtBox_ItemParentUnit.Focus();
        }
        private void ItemParentCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Add_ItemParent.IsEnabled = false;
            Del_ItemParent.IsEnabled = true;
        }
        private void cbBox_ItemUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!bClean)
            {
                if (cbBox_ItemUnit.SelectedIndex == 0)
                    txtBox_ItemUnitNum.Text = "1";
                else
                    txtBox_ItemUnitNum.Text = "1000";
            }
        }
        #endregion

    }
}
