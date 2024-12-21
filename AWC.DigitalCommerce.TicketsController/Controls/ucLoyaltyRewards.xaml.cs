using Newtonsoft.Json.Linq;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
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
using static AWC.DigitalCommerce.TicketsController.Constants;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucLoyaltyRewards.xaml
    /// </summary>
    public partial class ucLoyaltyRewards : UserControl
    {
        private bool bCleaning = false;
        private int itemID = 0;
        private clsItem itemToQualify = new clsItem();
        private clsItem itemRewarded = new clsItem();
        private List<clsItem> itemsList = new List<clsItem>();
        public ucLoyaltyRewards(string lang)
        {
            InitializeComponent();

            itemsList = DB.ListBinding_tbl_Items(0);
            ItemCatalog.ItemsSource = DB.ListBinding_tbl_LoyaltyRewards();
            cbox_ItemToQualify.ItemsSource = itemsList;
            cbox_ItemRewarded.ItemsSource = itemsList;
            cbox_Status.Items.Add("ACTIVO");
            cbox_Status.Items.Add("INACTIVO");
            cbox_Status.Items.Add("SUSPENDIDO");
        }

        private void CleanItemGroup()
        {
            bCleaning = true;
            txt_Description.Text = string.Empty;
            cbox_Status.SelectedIndex = -1;
            cbox_ItemToQualify.SelectedIndex = -1;
            txt_ItemToQualify.Text = string.Empty;
            cbox_ItemRewarded.SelectedIndex = -1;
            txt_QtyRewarded.Text = string.Empty;
            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Hidden;
            Mod_Item.Visibility = Visibility.Hidden;
            ItemCatalog.ItemsSource = DB.ListBinding_tbl_LoyaltyRewards();
            txt_Description.IsEnabled = true;
            txt_Description.Focus();
            bCleaning = false;
        }

        private void txt_Description_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_Description.Text.Length == 0) return;

            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                clsLoyaltyReward loyrew = DB.GetLoyaltyReward(txt_Description.Text);

                if (loyrew.ID > 0)
                {
                    LoadLoyaltyReward(loyrew);

                    Add_Item.Visibility = Visibility.Hidden;
                    Del_Item.Visibility = Visibility.Visible;
                    Mod_Item.Visibility = Visibility.Visible;
                }
                else
                {
                    Add_Item.Visibility = Visibility.Visible;
                    cbox_Status.Focus();
                }
            }
        }

        #region BUTTONS
        private void btnCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanItemGroup();
        }

        private void btnAdd_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DB.InsertLoyaltyReward(CreateLoyaltyReward());
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
                DB.DeleteLoyaltyReward(itemID);

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
                DB.UpdateLoyaltyReward(CreateLoyaltyReward());
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
        #endregion

        private void ItemCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bCleaning) return;

            clsLoyaltyReward loyrew = ItemCatalog.SelectedItem as clsLoyaltyReward;

            LoadLoyaltyReward(loyrew);

            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Visible;
            Mod_Item.Visibility = Visibility.Visible;
        }

        private void LoadLoyaltyReward(clsLoyaltyReward loyRew)
        {
            bCleaning = true;

            itemID = loyRew.ID;

            txt_Description.Text = loyRew.Description;
            txt_Description.IsEnabled = false;

            switch (loyRew.Status)
            {
                case "A":
                    cbox_Status.Text = "ACTIVO";
                    break;
                case "I":
                    cbox_Status.Text = "INACTIVO";
                    break;
                case "S":
                    cbox_Status.Text = "SUSPENDIDO";
                    break;
            }

            clsItem i2c = DB.GetItem(loyRew.ItemToQualify);
            clsItem i2r = DB.GetItem(loyRew.ItemRewarded);

            cbox_ItemToQualify.SelectedIndex = ProductIndex(i2c.ItemDescription);
            cbox_ItemRewarded.SelectedIndex = ProductIndex(i2r.ItemDescription);

            txt_ItemToQualify.Text = loyRew.QtyToQualify.ToString();
            txt_MaxDaysForReward.Text = loyRew.MaxDaysForReward.ToString();
            txt_QtyRewarded.Text = loyRew.QtyRewarded.ToString();

            bCleaning = false;
        }

        private clsLoyaltyReward CreateLoyaltyReward()
        {
            clsLoyaltyReward loyaRew = new clsLoyaltyReward();
            loyaRew.Description = txt_Description.Text.ToUpper();

            switch (cbox_Status.SelectedIndex)
            {
                case 0:
                    loyaRew.Status = "A";
                    break;
                case 1:
                    loyaRew.Status = "I";
                    break;
                case 2:
                    loyaRew.Status = "S";
                    break;
            }

            loyaRew.ItemToQualify = itemToQualify.ID;
            loyaRew.QtyToQualify = Convert.ToInt32(txt_ItemToQualify.Text);
            loyaRew.MaxDaysForReward = Convert.ToInt32(txt_MaxDaysForReward.Text);
            loyaRew.ItemRewarded = itemRewarded.ID;
            loyaRew.QtyRewarded = Convert.ToInt32(txt_QtyRewarded.Text);
            return loyaRew;
        }

        private int ProductIndex(string product2search)
        {
            int index = 0;
            foreach(clsItem item in itemsList)
            {
                if (product2search.Equals(item.ItemDescription))
                    return index;
                index++;
            }
            return index;
        }

        private void cbox_ItemToQualify_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemToQualify = cbox_ItemToQualify.SelectedItem as clsItem;
        }

        private void cbox_ItemRewarded_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemRewarded = cbox_ItemRewarded.SelectedItem as clsItem;
        }
    }
}
