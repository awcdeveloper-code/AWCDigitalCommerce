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
    /// Interaction logic for ucProviders.xaml
    /// </summary>
    public partial class ucProviders : UserControl
    {
        private int providerID = 0;

        public ucProviders()
        {
            InitializeComponent();

            List<clsProvider> provLst = DB.GetProvidersCatalog();
            ProvidersCatalog.ItemsSource = provLst;

            cbox_PaymentMethod.Items.Add("PAGO DE CONTADO");
            cbox_PaymentMethod.Items.Add("CREDITO 8 DIAS");
            cbox_PaymentMethod.Items.Add("CREDITO 15 DIAS");
            cbox_PaymentMethod.Items.Add("CREDITO 30 DIAS");

            EnableDisableTextBoxes(false);

            AddProvider.IsEnabled = false;
            DelProvider.IsEnabled = false;
            ModProvider.IsEnabled = false;
            txtProviderName.Focus();
        }

        private void EnableDisableTextBoxes(bool action)
        {
            switch (action)
            {
                case true:
                    txtBusinessAddress.IsEnabled = true;
                    txtEMailAddress.IsEnabled = true;
                    cbox_PaymentMethod.IsEnabled = true;
                    txtPhoneNumber.IsEnabled = true;
                    txtCellularNumber.IsEnabled = true;
                    txtRemarks.IsEnabled = true;
                    break;
                case false:
                    txtBusinessAddress.IsEnabled = false;
                    txtEMailAddress.IsEnabled = false;
                    cbox_PaymentMethod.IsEnabled = false;
                    txtPhoneNumber.IsEnabled = false;
                    txtCellularNumber.IsEnabled = false;
                    txtRemarks.IsEnabled = false;
                    break;
            }
        }

        private void CleanAll()
        {
            List<clsProvider> provLst = DB.GetProvidersCatalog();
            ProvidersCatalog.ItemsSource = provLst;

            txtProviderName.Text = string.Empty;
            txtBusinessAddress.Text = string.Empty;
            txtEMailAddress.Text = string.Empty;
            cbox_PaymentMethod.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtCellularNumber.Text = string.Empty;
            txtRemarks.Text = string.Empty;

            AddProvider.IsEnabled = false;
            DelProvider.IsEnabled = false;
            ModProvider.IsEnabled = false;
        }

        private void txtProviderName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return || e.Key == Key.Tab)
                {
                    if (txtProviderName.Text.Length == 0) return;

                    // validate provider name
                    clsProvider provider = DB.CheckProviderName(txtProviderName.Text.ToUpper());

                    if (provider.ID == 0)   // provider do no exist
                    {
                        EnableDisableTextBoxes(true);
                        txtBusinessAddress.Focus();

                        AddProvider.IsEnabled = true;
                        DelProvider.IsEnabled = false;
                        ModProvider.IsEnabled = false;
                    }
                    else
                    {
                        EnableDisableTextBoxes(true);
                        providerID = provider.ID;
                        txtBusinessAddress.Text = provider.BusinessAddress;
                        txtEMailAddress.Text = provider.eMailAddress;
                        cbox_PaymentMethod.Text = provider.PaymentMethod;
                        txtPhoneNumber.Text = provider.PhoneNumber;
                        txtCellularNumber.Text = provider.CellularNumber;
                        txtRemarks.Text = provider.Remarks;

                        AddProvider.IsEnabled = false;
                        DelProvider.IsEnabled = true;
                        ModProvider.IsEnabled = true;
                        txtBusinessAddress.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_AddProvider(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("CONFIRM: Do you really want to ADD the provider [" + txtProviderName.Text + "] (Yes/No)?", "Inventory Management", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                clsProvider providerProfile = new clsProvider();

                providerProfile.ProviderName = txtProviderName.Text.ToUpper();
                providerProfile.BusinessAddress = txtBusinessAddress.Text;
                providerProfile.eMailAddress = txtEMailAddress.Text;
                providerProfile.PaymentMethod = cbox_PaymentMethod.Text;
                providerProfile.PhoneNumber = txtPhoneNumber.Text;
                providerProfile.CellularNumber = txtCellularNumber.Text;
                providerProfile.Remarks = txtRemarks.Text;

                if (DB.InsertNewProvider(providerProfile))
                    MessageBox.Show("ATTENTION: The provider [" + txtProviderName.Text + "] was created successfully.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("ERROR: The provider [" + txtProviderName.Text + "] was NOT created. Please, notify this to the Administrator.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Error);

                CleanAll();
                txtProviderName.Focus();
            }
        }

        private void btn_DelProvider(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("CONFIRM: Do you really want to DELETER the provider [" + txtProviderName.Text + "] (Yes/No)?", "Inventories Management", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (DB.DeleteProvider(providerID))
                    MessageBox.Show("ATTENTION: The provider [" + txtProviderName.Text + "] was deleted successfully.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("ERROR: The provider [" + txtProviderName.Text + "] was NOT deleted. Please, notify this to the Administrator.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Error);

                CleanAll();
                txtProviderName.Focus();
            }
        }

        private void btn_ModProvider(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("CONFIRM: Do you really want to MODIFY the provider [" + txtProviderName.Text + "] (Yes/No)?", "Inventories Management", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                clsProvider providerProfile = new clsProvider();

                providerProfile.ID = providerID;
                providerProfile.ProviderName = txtProviderName.Text.ToUpper();
                providerProfile.BusinessAddress = txtBusinessAddress.Text;
                providerProfile.eMailAddress = txtEMailAddress.Text;
                providerProfile.PaymentMethod = cbox_PaymentMethod.Text;
                providerProfile.PhoneNumber = txtPhoneNumber.Text;
                providerProfile.CellularNumber = txtCellularNumber.Text;
                providerProfile.Remarks = txtRemarks.Text;

                if (DB.UpdateProvider(providerProfile))
                    MessageBox.Show("ATTENTION: The provider [" + txtProviderName.Text + "] was modified successfully.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("ERROR: The provider [" + txtProviderName.Text + "] was NOT modified. Please, notify this to the Administrator.", "Inventories Management", MessageBoxButton.OK, MessageBoxImage.Error);

                CleanAll();
                txtProviderName.Focus();
            }
        }
    }
}
