using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Controls;
using System.Windows.Input;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfMainWindow : Window
    {
        #region Global Variables
        private DispatcherTimer localTimer = new DispatcherTimer();
        public clsUser userProf = new clsUser();
        private clsCustomerVIP custProf = new clsCustomerVIP();

        private TabItem ucNewTicketTab;
        private TabItem ucNewTicketTab2;
        private TabItem ucNewTicketTab3;
        private TabItem ucNewTicketTab4;
        private TabItem ucNewTicketTab5;
        private TabItem ucNewTicketDetailTab;

        private bool NewTicketTabActive = false;
        private bool UpdateTicketTabActive = false;
        private bool CloseTicketTabActive = false;
        private bool OldTicketTabActive = false;
        private bool TodaySalesTabActive = false;
        private bool QueriesTabActive = false;
        private bool MaintenanceTabActive = false;

        #endregion

        #region MESSAGES
        public string lang = string.Empty;
        public string strLicenseExpired = string.Empty;
        public string strWelcomeAboard = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strNoOpenTickets = string.Empty;
        public string strMinimumAvailable = string.Empty;
        public string strBusinessDate = string.Empty;
        public string strBusinessDateLog = string.Empty;
        public string strBusinessDateLogAlert = string.Empty;
        #endregion

        public wpfMainWindow(string arg)
        {
            lang = arg;

            Thread.Sleep(1000);
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(wpfMainWindow_KeyUp);

            Traductor.ApplyTranslation(this, arg);

            localTimer.Tick += new EventHandler(localTimer_Tick);
            localTimer.Interval = new TimeSpan(0, 0, 1);
            localTimer.Start();
        }

        #region UTILITIES
        private void wpfMainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Quick Order was called.", Logger.Severity.INFORMATION);
                    wpfQuickOrder quickOrder = new wpfQuickOrder(this);
                    quickOrder.ShowDialog();
                    break;
                case Key.F1:
                    try
                    {
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Fast-Track was called.", Logger.Severity.INFORMATION);
                        wpfFastTrack wpfFT = new wpfFastTrack(lang);
                        wpfFT.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                        Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    break;
            }
        }
        private void btn_Exit(object sender, RoutedEventArgs e)
        {
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Tickets Controller App closed.", Logger.Severity.INFORMATION);
            this.Close();
        }
        private void localTimer_Tick(object sender, EventArgs eArgs)
        {
            localTimer.Stop();

            if (Helper.CheckLicenseExpiration())
            {
                wpfMessageBox.Show("Tickets Controller", strLicenseExpired, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                App.Current.Shutdown();
                return;
            }

            Helper.CheckDateOfWeekForBackup();

            wpfRequestPIN wpfPIN = new wpfRequestPIN();
            wpfPIN.ShowDialog();

            if (wpfPIN.numKeyed == "0")
            {
                App.Current.Shutdown();
                return;
            }

            userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

            if (userProf.userActive == false)
            {
                wpfMessageBox.Show("Tickets Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                App.Current.Shutdown();
                return;
            }

            Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
            Settings.Default.Save();

            //check if dailyClosing must be done
            string bussinessDay = DB.ConverTicketDate(Settings.Default.BusinessDate);
            string today = DB.ConverTicketDate(DateTime.Now.ToString("yyyyMMdd"));

            if (today != bussinessDay)
            {
                wpfDailyClosing dc = new wpfDailyClosing();
                dc.ShowDialog();

                if (dc.IsDailyClosing)
                {
                    //DB.SaveCustomerIDBeforeNormalization(Settings.Default.BusinessDate);

                    Settings.Default.BusinessDate = DateTime.Now.ToString("yyyyMMdd");
                    Settings.Default.Save();

                    DB.NormalizeCustomerID();

                    // set amount of initial cash register
                    wpfCashRegisterOpen cro = new wpfCashRegisterOpen(lang);
                    cro.ShowDialog();

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, String.Format(strBusinessDateLog, Settings.Default.WhoOpen, bussinessDay, today, cro.CashRegisterAmount), Logger.Severity.WARNING);
                }
            }

            // check if is required initialize business day
            //int ticketsOpen = DB.TodayTicketsOpen(DateTime.Now.ToString("yyyyMMdd"));

            //if (ticketsOpen == 0)
            //{
            //    //string bussinessDay = DB.ConverTicketDate(Settings.Default.BusinessDate);
            //    //string today = DB.ConverTicketDate(DateTime.Now.ToString("yyyyMMdd"));

            //    if (wpfMessageBox.Show("Tickets Controller",String.Format(strNoOpenTickets, userProf.userName), MessageBoxButton.YesNo,wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.Yes)
            //    {
            //        DB.SaveCustomerIDBeforeNormalization(Settings.Default.BusinessDate);

            //        Settings.Default.BusinessDate = DateTime.Now.ToString("yyyyMMdd");
            //        Settings.Default.Save();
                    
            //        DB.NormalizeCustomerID();

            //        // set amount of initial cash register
            //        wpfCashRegisterOpen cro = new wpfCashRegisterOpen(lang);
            //        cro.ShowDialog();

            //        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, String.Format(strBusinessDateLog, Settings.Default.WhoOpen, bussinessDay, today, cro.CashRegisterAmount), Logger.Severity.WARNING);
            //    }
            //    else
            //    {
            //        if (today != bussinessDay)
            //        {
            //            if (wpfMessageBox.Show("Tickets Controller", string.Format(strBusinessDate, today, bussinessDay), MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, lang) == MessageBoxResult.No)
            //            {
            //                App.Current.Shutdown();
            //                return;
            //            }

            //            DB.ReverseNormalizeCustomerID(Settings.Default.BusinessDate);

            //            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, String.Format(strBusinessDateLogAlert, Settings.Default.WhoOpen, bussinessDay), Logger.Severity.WARNING);
            //            TodayDate.Background = Brushes.Red;
            //        }
            //    }
            //}

            TodayDate.Content = DB.ConverTicketDate(Settings.Default.BusinessDate);
            tabCtrlWorkArea.Items.Clear();

            // check minimum available
            List<clsItem> list = DB.GetItemsBelowMinimum();

            if (list.Count > 0)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, strMinimumAvailable, Logger.Severity.WARNING);
                wpfMessageBox.Show("Tickets Controller", strMinimumAvailable, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);

                this.Notification.ToolTip = strMinimumAvailable;
                this.Notification.Visibility = Visibility.Visible;
            }

            SetUserAccessToResources();

            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Tickets Controller App initialized successfully.", Logger.Severity.INFORMATION);
        }
        private void SetUserAccessToResources()
        {
            NewTicket.IsEnabled = Helper.CheckUserAccessToResource("NewTicket");
            UpdateTicket.IsEnabled = Helper.CheckUserAccessToResource("UpdateTicket");
            CloseTicket.IsEnabled = Helper.CheckUserAccessToResource("CloseTicket");
            OldTickets.IsEnabled = Helper.CheckUserAccessToResource("OpenTickets");
            TodaySalesReport.IsEnabled = Helper.CheckUserAccessToResource("TodaySales");
            Queries.IsEnabled = Helper.CheckUserAccessToResource("Queries");
            SwitchPIN.IsEnabled = Helper.CheckUserAccessToResource("ChangePIN");
            SystemMaintenance.IsEnabled = Helper.CheckUserAccessToResource("Maintenance");
            InventoryManagement.IsEnabled = Helper.CheckUserAccessToResource("InventoryMgmt");
        }
        private void EnableDisableLeftButtons(int source)
        {
            switch (source)
            {
                // DISABLE BUTTONS
                case 1: // New Ticket
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        UpdateTicket.Visibility = Visibility.Collapsed;
                        CloseTicket.Visibility = Visibility.Collapsed;
                        OldTickets.Visibility = Visibility.Collapsed;
                        TodaySalesReport.Visibility = Visibility.Collapsed;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        break;
                    }
                case 2: // Update Ticket
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        NewTicket.Visibility = Visibility.Collapsed;
                        CloseTicket.Visibility = Visibility.Collapsed;
                        OldTickets.Visibility = Visibility.Collapsed;
                        TodaySalesReport.Visibility = Visibility.Collapsed;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        break;
                    }
                case 3: // Close Ticket
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        NewTicket.Visibility = Visibility.Collapsed;
                        UpdateTicket.Visibility = Visibility.Collapsed;
                        OldTickets.Visibility = Visibility.Collapsed;
                        TodaySalesReport.Visibility = Visibility.Collapsed;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        break;
                    }
                case 4: // Old Tickets
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        NewTicket.Visibility = Visibility.Collapsed;
                        UpdateTicket.Visibility = Visibility.Collapsed;
                        CloseTicket.Visibility = Visibility.Collapsed;
                        TodaySalesReport.Visibility = Visibility.Collapsed;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        break;
                    }
                case 5: // Today Sales Report
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        NewTicket.Visibility = Visibility.Collapsed;
                        UpdateTicket.Visibility = Visibility.Collapsed;
                        CloseTicket.Visibility = Visibility.Collapsed;
                        OldTickets.Visibility = Visibility.Collapsed;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        break;
                    }
                case 6: // Customized Report
                    {
                        QuickOrder.Visibility = Visibility.Collapsed;
                        DailyTransactions.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        Queries.Visibility = Visibility.Collapsed;
                        SwitchPIN.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 7: // Queries
                    {
                        DailyTransactions.Visibility = Visibility.Hidden;
                        Administration.Visibility = Visibility.Hidden;
                        SwitchPIN.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 8: // System Maintenance
                    {
                        DailyTransactions.Visibility = Visibility.Hidden;
                        Miscellaneous.Visibility = Visibility.Hidden;
                        InventoryManagement.Visibility = Visibility.Collapsed;
                        break;
                    }
                // ENABLE BUTTONS
                case 11: // New Ticket
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        UpdateTicket.Visibility = Visibility.Visible;
                        CloseTicket.Visibility = Visibility.Visible;
                        OldTickets.Visibility = Visibility.Visible;
                        TodaySalesReport.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        break;
                    }
                case 12: // Update Ticket
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        NewTicket.Visibility = Visibility.Visible;
                        CloseTicket.Visibility = Visibility.Visible;
                        OldTickets.Visibility = Visibility.Visible;
                        TodaySalesReport.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        break;
                    }
                case 13: // Close Ticket
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        NewTicket.Visibility = Visibility.Visible;
                        UpdateTicket.Visibility = Visibility.Visible;
                        OldTickets.Visibility = Visibility.Visible;
                        TodaySalesReport.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        break;
                    }
                case 14: // Old Tickets
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        NewTicket.Visibility = Visibility.Visible;
                        UpdateTicket.Visibility = Visibility.Visible;
                        CloseTicket.Visibility = Visibility.Visible;
                        TodaySalesReport.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        break;
                    }
                case 15: // Today Sales Report
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        NewTicket.Visibility = Visibility.Visible;
                        UpdateTicket.Visibility = Visibility.Visible;
                        CloseTicket.Visibility = Visibility.Visible;
                        OldTickets.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        break;
                    }
                case 16: // Customized Report
                    {
                        QuickOrder.Visibility = Visibility.Visible;
                        DailyTransactions.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        Queries.Visibility = Visibility.Visible;
                        SwitchPIN.Visibility = Visibility.Visible;
                        break;
                    }
                case 17: // BI Report
                    {
                        DailyTransactions.Visibility = Visibility.Visible;
                        Administration.Visibility = Visibility.Visible;
                        SwitchPIN.Visibility = Visibility.Visible;
                        break;
                    }
                case 18: // System Maintenance
                    {
                        DailyTransactions.Visibility = Visibility.Visible;
                        Miscellaneous.Visibility = Visibility.Visible;
                        InventoryManagement.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        #endregion

        #region DAILY TRANSACTIONS
        private void btn_QuickOrder(object sender, RoutedEventArgs e)
        {
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Quick Order was called.", Logger.Severity.INFORMATION);
            wpfQuickOrder quickOrder = new wpfQuickOrder(this);
            quickOrder.ShowDialog();
        }
        private void btn_Tickets(object sender, RoutedEventArgs e)
        {
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "FastTrack was called.", Logger.Severity.INFORMATION);
            this.WindowState = WindowState.Minimized;
            wpfFastTrack wpfFT = new wpfFastTrack(lang);
            wpfFT.ShowDialog();
            this.WindowState = WindowState.Maximized;
        }
        private void btn_NewTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NewTicketTabActive)
                {
                    NewTicketTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    NewTicket.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(11);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    NewTicketTabActive = true;
                    NewTicket.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(1);
                    Splash.Visibility = Visibility.Hidden;

                    // New Ticket Tab
                    var ucNewTicket = new ucNewTicket(this, lang);
                    ucNewTicketTab = new TabItem { Content = ucNewTicket };
                    ucNewTicketTab.Header = (lang == "-en") ? "OPENING A NEW TICKET" : "ABRIENDO UNA CUENTA NUEVA";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void btn_UpdateTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UpdateTicketTabActive)
                {
                    UpdateTicketTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    UpdateTicket.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(12);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    UpdateTicketTabActive = true;
                    UpdateTicket.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(2);
                    Splash.Visibility = Visibility.Hidden;

                    // Detail Tab
                    var ucNewTicketDetail = new ucNewTicketDetail(this, lang);
                    ucNewTicketDetailTab = new TabItem { Content = ucNewTicketDetail };
                    ucNewTicketDetailTab.Name = "ucNewTicketDetail";
                    ucNewTicketDetailTab.Header = (lang == "-en") ? "SEE DETAIL" : "VER EL DETALLE";
                    ucNewTicketDetailTab.FontSize = 20;
                    ucNewTicketDetailTab.FontWeight = FontWeights.DemiBold;

                    // Update Ticket Tab
                    var ucNewTicket = new ucUpdateTicket(this, lang, ucNewTicketDetailTab);
                    ucNewTicketTab = new TabItem { Content = ucNewTicket };
                    ucNewTicketTab.Name = "ucNewTicket";
                    ucNewTicketTab.Header = (lang == "-en") ? "UPDATING TICKET" : "ACTUALIZANDO CUENTA";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Add(ucNewTicketDetailTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void btn_CloseTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CloseTicketTabActive)
                {
                    CloseTicketTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    CloseTicket.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(13);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    CloseTicketTabActive = true;
                    CloseTicket.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(3);
                    Splash.Visibility = Visibility.Hidden;

                    // Close Ticket Tab
                    var ucNewTicket = new ucCloseTicket(this, lang);
                    ucNewTicketTab = new TabItem { Content = ucNewTicket };
                    ucNewTicketTab.Header = (lang == "-en") ? "CLOSE TICKET" : "CERRAR CUENTA";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void btn_OldTickets(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OldTicketTabActive)
                {
                    OldTicketTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    OldTickets.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(14);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    OldTicketTabActive = true;
                    OldTickets.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(4);
                    Splash.Visibility = Visibility.Hidden;

                    // Close Ticket Tab
                    var ucOldTickets = new ucOldTickets(lang);
                    ucNewTicketTab = new TabItem { Content = ucOldTickets };
                    ucNewTicketTab.Header = (lang == "-en") ? "OLD OPEN TICKETS" : "CUENTAS ABIERTAS";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }      
        private void btn_TodaySalesReport(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TodaySalesTabActive)
                {
                    TodaySalesTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    TodaySalesReport.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(15);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    TodaySalesTabActive = true;
                    TodaySalesReport.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(5);
                    Splash.Visibility = Visibility.Hidden;

                    // Today Sales Tab
                    var ucTodaySales = new ucTodaySales(lang);
                    ucNewTicketTab = new TabItem { Content = ucTodaySales };
                    ucNewTicketTab.Header = (lang == "-en") ? "TODAY'S SALES" : "VENTA DIARIA";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        #endregion

        #region MISCELANEOUS
        private void btn_Queries(object sender, RoutedEventArgs e)
        {
            try
            {
                if (QueriesTabActive)
                {
                    QueriesTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Queries.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(17);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    QueriesTabActive = true;
                    Queries.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(7);
                    Splash.Visibility = Visibility.Hidden;

                    // Close Ticket Tab
                    var ucNewTicket = new ucQueries(this, lang);
                    ucNewTicketTab = new TabItem { Content = ucNewTicket };
                    ucNewTicketTab.Header = (lang == "-en") ? "GENERAL QUERIES" : "CONSULTAS GENERALES";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btn_SwitchPIN(object sender, RoutedEventArgs e)
        {
            wpfRequestPIN wpfPIN = new wpfRequestPIN();
            wpfPIN.ShowDialog();

            if (wpfPIN.numKeyed == "0")
                return;

            // check operator properties
            userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

            if (userProf.userActive == false)
            {
                wpfMessageBox.Show("Tickets Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
            Settings.Default.Save();

            wpfMessageBox.Show("Tickets Controller", String.Format(strWelcomeAboard, userProf.userName), MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);

            SetUserAccessToResources();
        }
        #endregion

        #region MAINTENANCE
        private void btn_SystemMaintenance(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MaintenanceTabActive)
                {
                    MaintenanceTabActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    SystemMaintenance.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(18);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    MaintenanceTabActive = true;
                    SystemMaintenance.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(8);
                    Splash.Visibility = Visibility.Hidden;

                    // Expenses
                    var ucNewTicket4 = new ucExpensesReport(lang);
                    ucNewTicketTab4 = new TabItem { Content = ucNewTicket4 };
                    ucNewTicketTab4.Header = (lang == "-en") ? "GRAL EXPENSES" : "GASTOS VARIOS";
                    ucNewTicketTab4.FontSize = 20;
                    ucNewTicketTab4.FontWeight = FontWeights.DemiBold;

                    // Defective Items
                    var ucNewTicket3 = new ucMaintenance(lang);
                    ucNewTicketTab3 = new TabItem { Content = ucNewTicket3 };
                    ucNewTicketTab3.Header = (lang == "-en") ? "DEFECTIVE" : "DEFECTUOSOS";
                    ucNewTicketTab3.FontSize = 20;
                    ucNewTicketTab3.FontWeight = FontWeights.DemiBold;

                    // Tables Maintenance
                    var ucNewTicket = new ucTablesMaintenance(lang);
                    ucNewTicketTab = new TabItem { Content = ucNewTicket };
                    ucNewTicketTab.Header = (lang == "-en") ? "PRODUCTS" : "PRODUCTOS";
                    ucNewTicketTab.FontSize = 20;
                    ucNewTicketTab.FontWeight = FontWeights.DemiBold;

                    // Tickets Maintenance
                    var ucNewTicket2 = new ucTicketsMaintenance(lang);
                    ucNewTicketTab2 = new TabItem { Content = ucNewTicket2 };
                    ucNewTicketTab2.Header = (lang == "-en") ? "TICKETS" : "CUENTAS";
                    ucNewTicketTab2.FontSize = 20;
                    ucNewTicketTab2.FontWeight = FontWeights.DemiBold;

                    // Users
                    var ucNewTicket5 = new ucUsersMaintenance(lang);
                    ucNewTicketTab5 = new TabItem { Content = ucNewTicket5 };
                    ucNewTicketTab5.Header = (lang == "-en") ? "USERS MANAGEMENT" : "MANTENIMIENTO DE USUARIOS";
                    ucNewTicketTab5.FontSize = 20;
                    ucNewTicketTab5.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab4);
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab3);
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab);
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab2);
                    tabCtrlWorkArea.Items.Add(ucNewTicketTab5);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                Helper.ShowMessage("ERROR: " + ex.Message, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btn_InventoryManagement(object sender, RoutedEventArgs e)
        {
            wpfInventoryWindow wpfInv = new wpfInventoryWindow();
            wpfInv.ShowDialog();
        }

        #endregion

    }
}
