using AWC.DigitalCommerce.TicketsController.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucUsersMaintenance2 : UserControl
    {
        private List<CheckBox> chkBoxList = new List<CheckBox>();
        private clsUser userProf = new clsUser();
        private bool loading = false;
        public ObservableCollection<clsListBoxCheckbox> AccountsGroupItems { get; set; }
        public ObservableCollection<clsListBoxCheckbox> PendantsGroupItems { get; set; }
        public ObservableCollection<clsListBoxCheckbox> SalesGroupItems { get; set; }
        public ObservableCollection<clsListBoxCheckbox> SettingsGroupItems { get; set; }

        public ucUsersMaintenance2()
        {
            InitializeComponent();

            cbox_Status.Items.Add("ACTIVO");
            cbox_Status.Items.Add("INACTIVO");

            cbox_Job.Items.Add("ADMINISTRADOR");
            cbox_Job.Items.Add("APROBADOR");
            cbox_Job.Items.Add("ATENCIÓN DEL SALON");
            cbox_Job.Items.Add("BARTENDER");
            cbox_Job.Items.Add("POWER ADMIN");
            cbox_Job.Items.Add("SOPORTE TÉCNICO");
            cbox_Job.Items.Add("SUPERVISOR");

            LoadPendantsGroup();
            LoadAccountsGroup();
            LoadSalesGroup();
            LoadSettingsGroup();
        }

        private void EnableDisableCheckboxes(ObservableCollection<clsListBoxCheckbox> listBox, bool action)
        {
            foreach (clsListBoxCheckbox item in listBox)
            {
                item.IsSelected = action;
            }

            this.UpdateLayout();
        }

        private void CleanAll()
        {
            txtBox_PIN.Text = string.Empty;
            txtBox_Name.Text = string.Empty;
            cbox_Job.SelectedIndex = -1;
            cbox_Status.SelectedIndex = -1;
            txtBox_PIN.IsEnabled = true;
            txtBox_PIN.Focus();
            btnModify.IsEnabled = false;
            btnSave.IsEnabled = false;

            // individual checkboxes
            chkBox_QuickSale.IsChecked = false;
            chkBox_Queries.IsChecked = false;
            chkBox_Cashdrawer.IsChecked = false;
            chkBox_Inventory.IsChecked = false;
            chkBox_DailyClose.IsChecked = false;

            // indivual group checkboxes
            chkBox_Pendant.IsChecked = false;
            chkBox_Tickets.IsChecked = false;
            chkBox_TodaySale.IsChecked = false;
            chkBox_Settings.IsChecked = false;
            // listboxes
            EnableDisableCheckboxes(PendantsGroupItems, false);
            EnableDisableCheckboxes(AccountsGroupItems, false);
            EnableDisableCheckboxes(SalesGroupItems, false);
            EnableDisableCheckboxes(SettingsGroupItems, false);
        }

        private clsUser CreateUserSecurityProfile()
        {
            StringBuilder strBld = new StringBuilder();


            clsUser updateUserProfile = new clsUser();

            updateUserProfile.userPIN = txtBox_PIN.Text;
            updateUserProfile.userPW = txtBox_PIN.Text;
            updateUserProfile.userName = txtBox_Name.Text.ToUpper();
            updateUserProfile.userActive = cbox_Status.SelectedIndex == 0 ? true : false;
            updateUserProfile.userAccessLevel = cbox_Job.SelectedItem.ToString();
            updateUserProfile.userPowerAdmin = cbox_Job.SelectedIndex == 3 ? true : false;

            updateUserProfile.userSecurityProfile = strBld.ToString() + new string('0', 60 - strBld.ToString().Length);

            return updateUserProfile;
        }
        
        private void txtBox_PIN_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtBox_PIN.Text.Length == 0) return;

            try
            {
                if (e.Key == Key.Enter)
                {
                    userProf = DB.CheckUserPIN(txtBox_PIN.Text);

                    loading = true;

                    if (userProf.userName.Length > 0)
                    {
                        txtBox_Name.Text = userProf.userName;
                        cbox_Status.Text = (bool)userProf.userActive ? "ACTIVO" : "INACTIVO";
                        cbox_Job.Text = userProf.userAccessLevel;

                        txtBox_PIN.IsEnabled = false;
                        btnDelete.IsEnabled = true;
                        btnModify.IsEnabled = true;
                    }
                    else
                    {
                        btnSave.IsEnabled = true;
                        txtBox_Name.Focus();
                    }

                    loading = false;
                    txtBox_Name.IsEnabled = true;
                    cbox_Job.IsEnabled = true;
                    cbox_Status.IsEnabled = true;
                    txtBox_Name.Focus();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return;
            }
        }

        #region Footer Buttons
        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            CleanAll();
        }

        private void btn_Delete(object sender, RoutedEventArgs e)
        {
            if (DB.DeleteUserProfile(txtBox_PIN.Text))
            {
                Helper.ShowToastNotification($"PIN {txtBox_PIN.Text} eliminado");
                CleanAll();
            }
        }

        private void btn_Modify(object sender, RoutedEventArgs e)
        {
            DB.UpdateUserSecurityProfile(CreateUserSecurityProfile());
            Helper.ShowToastNotification($"PIN {txtBox_PIN.Text} modificado");
            CleanAll();
        }

        private void btn_Save(object sender, RoutedEventArgs e)
        {
            DB.InsertUserSecurityProfile(CreateUserSecurityProfile());
            Helper.ShowToastNotification($"PIN {txtBox_PIN.Text} guardado");
            CleanAll();
        }
        #endregion

        #region Pendants Group
        private void chkBox_Pendant_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Pendant.IsChecked == true ? true : false;
            EnableDisableCheckboxes(PendantsGroupItems, action);
            PendantsListBox.IsEnabled = action;
        }

        private void LoadPendantsGroup()
        {
            PendantsGroupItems = new ObservableCollection<clsListBoxCheckbox>
            {
                new clsListBoxCheckbox { Name = "IMPRIMIR", IsSelected = false },
                new clsListBoxCheckbox { Name = "ANULAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "ABONAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "REASIGNAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "PAGAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "REVISAR", IsSelected = false }
            };

            PendantsListBox.ItemsSource = PendantsGroupItems;
        }
        #endregion

        #region Accounts Group
        private void chkBox_Tickets_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Tickets.IsChecked == true ? true : false;
            EnableDisableCheckboxes(AccountsGroupItems, action);
            AccountsListBox.IsEnabled = action;
        }

        private void LoadAccountsGroup()
        {
            AccountsGroupItems = new ObservableCollection<clsListBoxCheckbox>
            {
                new clsListBoxCheckbox { Name = "CREAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "IMPRIMIR", IsSelected = false },
                new clsListBoxCheckbox { Name = "ANULAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "CANCELAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "SALVAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "ABONAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "PAGAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "AGREGAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "ASIGNAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "HEREDAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "PENDIENTES", IsSelected = false },
                new clsListBoxCheckbox { Name = "LEALTAD", IsSelected = false },
                new clsListBoxCheckbox { Name = "ELIMINAR ITEMES", IsSelected = false },
                new clsListBoxCheckbox { Name = "+ SUMAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "= REMOVER", IsSelected = false },
                new clsListBoxCheckbox { Name = "- RESTAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "# IMPRIMIR", IsSelected = false }
            };

            AccountsListBox.ItemsSource = AccountsGroupItems;
        }
        #endregion

        #region Sales Group
        private void chkBox_TodaySale_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_TodaySale.IsChecked == true ? true : false;
            EnableDisableCheckboxes(SalesGroupItems, action);
            SalesListBox.IsEnabled = action;
        }

        private void LoadSalesGroup()
        {
            SalesGroupItems = new ObservableCollection<clsListBoxCheckbox>
            {
                new clsListBoxCheckbox { Name = "URGENCIA", IsSelected = false },
                new clsListBoxCheckbox { Name = "IMPRIMIR", IsSelected = false },
                new clsListBoxCheckbox { Name = "MONTO", IsSelected = false },
                new clsListBoxCheckbox { Name = "NOMBRE", IsSelected = false },
                new clsListBoxCheckbox { Name = "PAGO", IsSelected = false },
                new clsListBoxCheckbox { Name = "VOUCHER", IsSelected = false },
                new clsListBoxCheckbox { Name = "ANULAR", IsSelected = false },
                new clsListBoxCheckbox { Name = "CORREO", IsSelected = false },
                new clsListBoxCheckbox { Name = "FACTURA IVA", IsSelected = false },
            };

            SalesListBox.ItemsSource = SalesGroupItems;
        }
        #endregion

        #region Settings Group
        private void chkBox_Settings_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Settings.IsChecked == true ? true : false;
            EnableDisableCheckboxes(SettingsGroupItems, action);
            SettingsListBox.IsEnabled = action;
        }

        private void LoadSettingsGroup()
        {
            SettingsGroupItems = new ObservableCollection<clsListBoxCheckbox>
            {
                new clsListBoxCheckbox { Name = "DIARIOS", IsSelected = false },
                new clsListBoxCheckbox { Name = "GASTOS", IsSelected = false },
                new clsListBoxCheckbox { Name = "INGRESOS", IsSelected = false },
                new clsListBoxCheckbox { Name = "DAÑADOS", IsSelected = false },
                new clsListBoxCheckbox { Name = "COLABORADORES", IsSelected = false },
                new clsListBoxCheckbox { Name = "CATEGORIAS", IsSelected = false },
                new clsListBoxCheckbox { Name = "LEALTAD", IsSelected = false },
                new clsListBoxCheckbox { Name = "CUENTAS", IsSelected = false },
                new clsListBoxCheckbox { Name = "PEDIDOS", IsSelected = false },
                new clsListBoxCheckbox { Name = "INGRESOS CAJA", IsSelected = false },
                new clsListBoxCheckbox { Name = "VOUCHERS", IsSelected = false },
            };

            SettingsListBox.ItemsSource = SettingsGroupItems;
        }
        #endregion
    }
}
