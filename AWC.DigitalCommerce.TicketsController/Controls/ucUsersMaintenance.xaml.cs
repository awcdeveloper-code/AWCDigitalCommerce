using Org.BouncyCastle.Asn1.X509;
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
    public partial class ucUsersMaintenance : UserControl
    {
        private List<CheckBox> chkBoxList = new List<CheckBox>();

        private clsUser userProf = new clsUser();
        private bool loading = false;

        public ucUsersMaintenance(string _lang)
        {
            InitializeComponent();

            chkBoxList.Add(chkBox_QuickSale);
            chkBoxList.Add(chkBox_Tickets);
            chkBoxList.Add(chkBox_CreateTicket);
            chkBoxList.Add(chkBox_PrintTicket);
            chkBoxList.Add(chkBox_AbortTicket);
            chkBoxList.Add(chkBox_CancelUpdate);
            chkBoxList.Add(chkBox_UpdateTicket);
            chkBoxList.Add(chkBox_CreditTicket);
            chkBoxList.Add(chkBox_PayTicket);
            chkBoxList.Add(chkBox_AddProduct);
            chkBoxList.Add(chkBox_Queries);
            chkBoxList.Add(chkBox_Inventory);
            chkBoxList.Add(chkBox_Pendant);
            chkBoxList.Add(chkBox_PendantPrint);
            chkBoxList.Add(chkBox_PendantAbort);
            chkBoxList.Add(chkBox_PendantCredit);
            chkBoxList.Add(chkBox_PendantPay);
            chkBoxList.Add(chkBox_PendantReassign);
            chkBoxList.Add(chkBox_TodaySale);
            chkBoxList.Add(chkBox_TodaySaleEmergency);
            chkBoxList.Add(chkBox_TodaySalePrint);
            chkBoxList.Add(chkBox_TodaySaleRestVoucher);
            chkBoxList.Add(chkBox_TodaySaleAbort);
            chkBoxList.Add(chkBox_DailyClose);
            chkBoxList.Add(chkBox_Settings);
            chkBoxList.Add(chkBox_AssignTicket);
            chkBoxList.Add(chkBox_InheritTicket);
            chkBoxList.Add(chkBox_Cashdrawer);
            chkBoxList.Add(chkBox_AddButton);
            chkBoxList.Add(chkBox_DelButton);
            chkBoxList.Add(chkBox_SubButton);
            chkBoxList.Add(chkBox_PrnButton);
            chkBoxList.Add(chkBox_AddOldTicket);
            chkBoxList.Add(chkBox_TodaySaleFake);
            chkBoxList.Add(chkBox_TodaySaleEMail);
            chkBoxList.Add(chkBox_Daily);
            chkBoxList.Add(chkBox_GralExpenses);
            chkBoxList.Add(chkBox_DefectiveItems);
            chkBoxList.Add(chkBox_UsersMgmt);
            chkBoxList.Add(chkBox_Categories);
            chkBoxList.Add(chkBox_LoyaltyMgmt);
            chkBoxList.Add(chkBox_TicketsMgmt);
            chkBoxList.Add(chkBox_Loyalty);
            chkBoxList.Add(chkBox_ChangeName);
            chkBoxList.Add(chkBox_TodaySaleIVA);
            chkBoxList.Add(chkBox_TodaySaleIVA);
            chkBoxList.Add(chkBox_IncomeCash);
            chkBoxList.Add(chkBox_InternalOrder);
            chkBoxList.Add(chkBox_Specials);
            chkBoxList.Add(chkBox_PayMethod);
            chkBoxList.Add(chkBox_Vouchers);
            chkBoxList.Add(chkBox_PowerAdmin);

            EnableDisableCheckboxes(false);

            cbox_Job.Items.Add("ADMINISTRADOR");
            cbox_Job.Items.Add("ATENCIÓN DEL SALON");
            cbox_Job.Items.Add("BARTENDER");
            cbox_Job.Items.Add("POWER ADMIN");
            cbox_Job.Items.Add("SOPORTE TÉCNICO");
            cbox_Job.Items.Add("SUPERVISOR");

            cbox_Status.Items.Add("ACTIVO");
            cbox_Status.Items.Add("INACTIVO");
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
                    EnableDisableCheckboxes(true);

                    if (userProf.userName.Length > 0)
                    {
                        txtBox_Name.Text = userProf.userName;
                        cbox_Status.Text = (bool)userProf.userActive ? "ACTIVO" : "INACTIVO";
                        cbox_Job.Text = userProf.userAccessLevel;

                        chkBox_QuickSale.IsChecked = userProf.userSecurityProfile.Substring(0, 1) == "0" ? false : true;
                        chkBox_Tickets.IsChecked = userProf.userSecurityProfile.Substring(1, 1) == "0" ? false : true;
                        chkBox_CreateTicket.IsChecked = userProf.userSecurityProfile.Substring(2, 1) == "0" ? false : true;
                        chkBox_PrintTicket.IsChecked = userProf.userSecurityProfile.Substring(3, 1) == "0" ? false : true;
                        chkBox_AbortTicket.IsChecked = userProf.userSecurityProfile.Substring(4, 1) == "0" ? false : true;
                        chkBox_CancelUpdate.IsChecked = userProf.userSecurityProfile.Substring(5, 1) == "0" ? false : true;
                        chkBox_UpdateTicket.IsChecked = userProf.userSecurityProfile.Substring(6, 1) == "0" ? false : true;
                        chkBox_CreditTicket.IsChecked = userProf.userSecurityProfile.Substring(7, 1) == "0" ? false : true;
                        chkBox_PayTicket.IsChecked = userProf.userSecurityProfile.Substring(8, 1) == "0" ? false : true;
                        chkBox_AddProduct.IsChecked = userProf.userSecurityProfile.Substring(9, 1) == "0" ? false : true;
                        chkBox_Queries.IsChecked = userProf.userSecurityProfile.Substring(10, 1) == "0" ? false : true;
                        chkBox_Inventory.IsChecked = userProf.userSecurityProfile.Substring(11, 1) == "0" ? false : true;
                        chkBox_Pendant.IsChecked = userProf.userSecurityProfile.Substring(12, 1) == "0" ? false : true;
                        chkBox_PendantPrint.IsChecked = userProf.userSecurityProfile.Substring(13, 1) == "0" ? false : true;
                        chkBox_PendantAbort.IsChecked = userProf.userSecurityProfile.Substring(14, 1) == "0" ? false : true;
                        chkBox_PendantCredit.IsChecked = userProf.userSecurityProfile.Substring(15, 1) == "0" ? false : true;
                        chkBox_PendantPay.IsChecked = userProf.userSecurityProfile.Substring(16, 1) == "0" ? false : true;
                        chkBox_TodaySale.IsChecked = userProf.userSecurityProfile.Substring(17, 1) == "0" ? false : true;
                        chkBox_TodaySaleEmergency.IsChecked = userProf.userSecurityProfile.Substring(18, 1) == "0" ? false : true;
                        chkBox_TodaySalePrint.IsChecked = userProf.userSecurityProfile.Substring(19, 1) == "0" ? false : true;
                        chkBox_TodaySaleRestVoucher.IsChecked = userProf.userSecurityProfile.Substring(20, 1) == "0" ? false : true;
                        chkBox_TodaySaleAbort.IsChecked = userProf.userSecurityProfile.Substring(21, 1) == "0" ? false : true;
                        chkBox_DailyClose.IsChecked = userProf.userSecurityProfile.Substring(22, 1) == "0" ? false : true;
                        chkBox_Settings.IsChecked = userProf.userSecurityProfile.Substring(23, 1) == "0" ? false : true;
                        chkBox_PendantReassign.IsChecked = userProf.userSecurityProfile.Substring(24, 1) == "0" ? false : true;
                        chkBox_AssignTicket.IsChecked = userProf.userSecurityProfile.Substring(25, 1) == "0" ? false : true;
                        chkBox_InheritTicket.IsChecked = userProf.userSecurityProfile.Substring(26, 1) == "0" ? false : true;
                        chkBox_Cashdrawer.IsChecked = userProf.userSecurityProfile.Substring(27, 1) == "0" ? false : true;
                        chkBox_AddButton.IsChecked = userProf.userSecurityProfile.Substring(28, 1) == "0" ? false : true;
                        chkBox_DelButton.IsChecked = userProf.userSecurityProfile.Substring(29, 1) == "0" ? false : true;
                        chkBox_SubButton.IsChecked = userProf.userSecurityProfile.Substring(30, 1) == "0" ? false : true;
                        chkBox_PrnButton.IsChecked = userProf.userSecurityProfile.Substring(31, 1) == "0" ? false : true;
                        chkBox_AddOldTicket.IsChecked = userProf.userSecurityProfile.Substring(32, 1) == "0" ? false : true;
                        chkBox_TodaySaleFake.IsChecked = userProf.userSecurityProfile.Substring(33, 1) == "0" ? false : true;
                        chkBox_TodaySaleEMail.IsChecked = userProf.userSecurityProfile.Substring(34, 1) == "0" ? false : true;
                        chkBox_Daily.IsChecked = userProf.userSecurityProfile.Substring(35, 1) == "0" ? false : true;
                        chkBox_GralExpenses.IsChecked = userProf.userSecurityProfile.Substring(36, 1) == "0" ? false : true;
                        chkBox_IncomeCash.IsChecked = userProf.userSecurityProfile.Substring(45, 1) == "0" ? false : true;
                        chkBox_DefectiveItems.IsChecked = userProf.userSecurityProfile.Substring(37, 1) == "0" ? false : true;
                        chkBox_UsersMgmt.IsChecked = userProf.userSecurityProfile.Substring(38, 1) == "0" ? false : true;
                        chkBox_Categories.IsChecked = userProf.userSecurityProfile.Substring(39, 1) == "0" ? false : true;
                        chkBox_LoyaltyMgmt.IsChecked = userProf.userSecurityProfile.Substring(40, 1) == "0" ? false : true;
                        chkBox_TicketsMgmt.IsChecked = userProf.userSecurityProfile.Substring(41, 1) == "0" ? false : true;
                        chkBox_Loyalty.IsChecked = userProf.userSecurityProfile.Substring(42, 1) == "0" ? false : true;
                        chkBox_ChangeName.IsChecked = userProf.userSecurityProfile.Substring(43, 1) == "0" ? false : true;
                        chkBox_TodaySaleIVA.IsChecked = userProf.userSecurityProfile.Substring(44, 1) == "0" ? false : true;
                        chkBox_InternalOrder.IsChecked = userProf.userSecurityProfile.Substring(45, 1) == "0" ? false : true;
                        chkBox_Specials.IsChecked = userProf.userSecurityProfile.Substring(47, 1) == "0" ? false : true;
                        chkBox_PayMethod.IsChecked = userProf.userSecurityProfile.Substring(48, 1) == "0" ? false : true;
                        chkBox_Vouchers.IsChecked = userProf.userSecurityProfile.Substring(49, 1) == "0" ? false : true;
                        chkBox_PowerAdmin.IsChecked = userProf.userSecurityProfile.Substring(50, 1) == "0" ? false : true;

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

        private void EnableDisableCheckboxes(bool action)
        {
            foreach (CheckBox chk in chkBoxList)
            {
                chk.IsChecked = false;
                chk.IsEnabled = action;
            }
        }

        private void CleanAll()
        {
            txtBox_PIN.Text = string.Empty;
            txtBox_Name.Text = string.Empty;
            cbox_Job.SelectedIndex = -1;
            cbox_Status.SelectedIndex = -1;
            EnableDisableCheckboxes(false);
            txtBox_PIN.IsEnabled = true;
            txtBox_PIN.Focus();
            btnModify.IsEnabled = false;
            btnSave.IsEnabled = false;
        }
        private clsUser CreateUserSecurityProfile()
        {
            StringBuilder strBld = new StringBuilder();

            strBld.Append((bool)chkBox_QuickSale.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Tickets.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_CreateTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PrintTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_AbortTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_CancelUpdate.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_UpdateTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_CreditTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PayTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_AddProduct.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Queries.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Inventory.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Pendant.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PendantPrint.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PendantAbort.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PendantCredit.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PendantPay.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySale.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleEmergency.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySalePrint.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleRestVoucher.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleAbort.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_DailyClose.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Settings.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PendantReassign.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_AssignTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_InheritTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Cashdrawer.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_AddButton.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_DelButton.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_SubButton.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PrnButton.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_AddOldTicket.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleFake.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleEMail.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Daily.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_GralExpenses.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_DefectiveItems.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_UsersMgmt.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Categories.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_LoyaltyMgmt.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TicketsMgmt.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Loyalty.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_ChangeName.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_TodaySaleIVA.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_IncomeCash.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_InternalOrder.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Specials.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PayMethod.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_Vouchers.IsChecked ? 1 : 0);
            strBld.Append((bool)chkBox_PowerAdmin.IsChecked ? 1 : 0);

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
            Helper.ShowToastNotification($"PIN {txtBox_PIN.Text} salvado");
            CleanAll();
        }

        private void chkBox_Settings_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Settings.IsChecked == true ? true : false;

            if (!loading)
            {
                chkBox_Daily.IsChecked = action;
                chkBox_GralExpenses.IsChecked = action;
                chkBox_IncomeCash.IsChecked = action;
                chkBox_DefectiveItems.IsChecked = action;
                chkBox_UsersMgmt.IsChecked = action;
                chkBox_Categories.IsChecked = action;
                chkBox_LoyaltyMgmt.IsChecked = action;
                chkBox_TicketsMgmt.IsChecked = action;
                chkBox_InternalOrder.IsChecked = action;
                chkBox_Specials.IsChecked = action;
                chkBox_Vouchers.IsChecked = action;
            }
        }

        private void chkBox_Tickets_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Tickets.IsChecked == true ? true : false;

            if (!loading)
            {
                chkBox_CreateTicket.IsChecked = action;
                chkBox_PrintTicket.IsChecked = action;
                chkBox_AbortTicket.IsChecked = action;
                chkBox_CancelUpdate.IsChecked = action;
                chkBox_UpdateTicket.IsChecked = action;
                chkBox_CreditTicket.IsChecked = action;
                chkBox_PayTicket.IsChecked = action;
                chkBox_AddProduct.IsChecked = action;
                chkBox_AssignTicket.IsChecked = action;
                chkBox_InheritTicket.IsChecked = action;
                chkBox_AddOldTicket.IsChecked = action;
                chkBox_AddButton.IsChecked = action;
                chkBox_DelButton.IsChecked = action;
                chkBox_SubButton.IsChecked = action;
                chkBox_PrnButton.IsChecked = action;
                chkBox_Loyalty.IsChecked = action;
            }
        }

        private void chkBox_Pendant_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_Pendant.IsChecked == true ? true : false;

            if (!loading)
            {
                chkBox_PendantPrint.IsChecked = action;
                chkBox_PendantReassign.IsChecked = action;
                chkBox_PendantAbort.IsChecked = action;
                chkBox_PendantCredit.IsChecked = action;
                chkBox_PendantPay.IsChecked = action;
            }
        }

        private void chkBox_TodaySale_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            bool action = chkBox_TodaySale.IsChecked == true ? true : false;

            if (!loading)
            {
                chkBox_TodaySaleEmergency.IsChecked = action;
                chkBox_TodaySalePrint.IsChecked = action;
                chkBox_TodaySaleRestVoucher.IsChecked = action;
                chkBox_TodaySaleAbort.IsChecked = action;
                chkBox_TodaySaleFake.IsChecked = action;
                chkBox_TodaySaleEMail.IsChecked = action;
                chkBox_ChangeName.IsChecked = action;
                chkBox_TodaySaleIVA.IsChecked = action;
                chkBox_PayMethod.IsChecked = action;
                chkBox_TodaySale.IsChecked = action;
            }
        }
    }
}
