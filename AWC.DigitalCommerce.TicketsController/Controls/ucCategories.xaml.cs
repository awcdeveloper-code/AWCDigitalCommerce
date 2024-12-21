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
    /// Interaction logic for ucCategories.xaml
    /// </summary>
    public partial class ucCategories : UserControl
    {
        private bool bCleaning = false;
        private int categoryID = 0;
        private List<clsCategory> itemsList = new List<clsCategory>();
        public ucCategories()
        {
            InitializeComponent();
            CleanItemGroup();
        }
        private void CleanItemGroup()
        {
            bCleaning = true;
            txt_Description.Text = string.Empty;
            cbox_Parent.SelectedIndex = -1;
            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Hidden;
            Mod_Item.Visibility = Visibility.Hidden;
            Categories.ItemsSource = DB.ListBinding_tbl_Categories();
            txt_Description.IsEnabled = true;
            txt_Description.Focus();
            bCleaning = false;
        }
        private void txt_Description_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_Description.Text.Length == 0) return;

            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                clsCategory categ = DB.GetCategory(txt_Description.Text);

                if (categ.CategoryID > 0)
                {
                    LoadCategory(categ);

                    Add_Item.Visibility = Visibility.Hidden;
                    Del_Item.Visibility = Visibility.Visible;
                    Mod_Item.Visibility = Visibility.Visible;
                }
                else
                {
                    Add_Item.Visibility = Visibility.Visible;
                    cbox_Parent.Focus();
                }
            }
        }
        private void LoadCategory(clsCategory categ)
        {
            bCleaning = true;

            categoryID = categ.CategoryID;
            txt_Description.Text = categ.Description;
            txt_Description.IsEnabled = false;

            bCleaning = false;
        }
        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bCleaning) return;

            clsCategory categ = Categories.SelectedItem as clsCategory;

            LoadCategory(categ);

            Add_Item.Visibility = Visibility.Hidden;
            Del_Item.Visibility = Visibility.Visible;
            Mod_Item.Visibility = Visibility.Visible;
        }
        private void btnCancel_Item(object sender, MouseButtonEventArgs e)
        {
            CleanItemGroup();
        }
        private void btnAdd_Item(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //DB.InsertLoyaltyReward(CreateLoyaltyReward());
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
                //DB.DeleteLoyaltyReward(itemID);

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
                //DB.UpdateLoyaltyReward(CreateLoyaltyReward());
                CleanItemGroup();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                wpfMessageBox.Show("Tickets Controller", "ERROR: " + ex.Message, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
            }
        }
    }
}
