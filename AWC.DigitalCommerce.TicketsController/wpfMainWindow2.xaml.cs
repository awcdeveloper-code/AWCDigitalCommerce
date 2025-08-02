using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Controls;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfMainWindow2 : Window
    {
        #region Global Variables
        private string lang = string.Empty;
        private int tokenID = 0;
        private bool isActive = false;
        public bool transInProgress = false;
        public int transInProgressTries = 0;

        private TabItem newTab = new TabItem();
        private DispatcherTimer localTimer = new DispatcherTimer();
        private DispatcherTimer bartenderOrder = new DispatcherTimer();
        private DispatcherTimer timer = new DispatcherTimer();
        public clsUser userProf = new clsUser();
        private clsCustomerVIP custProf = new clsCustomerVIP();
        private List<Button> mainButtons = new List<Button>();
        private List<Label> mainLabels = new List<Label>();
        public ucTickets ucTicketsShared;
        #endregion

        #region MESSAGES
        public string strLicenseExpired = string.Empty;
        public string strWelcomeAboard = string.Empty;
        public string strPINdoNotExist = string.Empty;
        public string strNoOpenTickets = string.Empty;
        public string strMinimumAvailable = string.Empty;
        public string strBusinessDate = string.Empty;
        public string strBusinessDateLog = string.Empty;
        public string strBusinessDateLogAlert = string.Empty;
        public string strTransactionInProgress = string.Empty;
        public string strItemsBelowZero = string.Empty;
        public string strInventoryOK = string.Empty;
        public string strInternetOK = string.Empty;
        #endregion

        private Mutex singleton = new Mutex(true, "TicketController");
        public wpfMainWindow2()
        {
            //string ec = ARC4Encryption.DoEncrypt("Hola, soy Memo Grillo", "Mimi0mores");
            //string dc = ARC4Encryption.DoDecrypt(ec, "Mimi0mores");

            lang = "-sp";

            Helper.TextToSpeech($"WELCOME TO DIGITAL COMMERCE TICKETS CONTROLLER {DateTime.Now.ToString("yyyy")}");

            try
            {
                if (!singleton.WaitOne(TimeSpan.Zero, true))
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: La aplicación YA ESTÁ en ejecución. Por favor, revise la barra de tareas para reactivarla.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    App.Current.Shutdown();
                    return;
                }

                if (!Helper.ValidateInternalAccounts())
                {
                    App.Current.Shutdown();
                    return;
                }

                if (Settings.Default.wpfMainWIndowMaximized)
                    this.WindowState = WindowState.Maximized;

                InitializeComponent();

                ApplicationTitle.Content = Settings.Default.ApplicationTitle;
                BussinessName.Content = Settings.Default.BusinessName;
                lblOSD.Content = $"{RuntimeInformation.OSDescription.ToUpper()} AWC {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
                LoadControlsArray();

                this.KeyDown += new KeyEventHandler(this_KeyDown);

                Traductor.ApplyTranslation(this, lang);

                lblTodayDate.Content = $"FECHA CONTABLE: {DB.ConverTicketDate(Settings.Default.BusinessDate)}";

                if (Settings.Default.DisplayCurrentDateTime && Settings.Default.BartenderOrderTickInSeconds == 0)
                {
                    StartCurrentDateTimeTimer();
                }

                localTimer.Tick += new EventHandler(localTimer_Tick);
                localTimer.Interval = new TimeSpan(0, 0, 1);
                localTimer.Start();
            }
            catch (Exception ex)
            {
                wpfMessageBox.Show("Tickets Controller", $"ERROR: {ex.Message}", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                App.Current.Shutdown();
                return;
            }
        }

        #region UTILITIES
        private void StartCurrentDateTimeTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentDateTime.Content = DateTime.Now.ToString("dd.MM.yyyy hh:mm tt");
        }
        private void MainWindow2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                this.WindowState = WindowState.Minimized;
                return;
            }

            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                // do nothing
                return;
            }

            switch (e.Key)
            {
                case Key.F1:
                    if (QuickSale.IsEnabled)
                        ucQuickOrder(lang);
                    break;
                case Key.F2:
                    if (Tickets.IsEnabled)
                        ucTickets(lang);
                    break;
                case Key.F3:
                    if (OldTickets.IsEnabled)
                        ucOldTickets(lang);
                    break;
                case Key.F4:
                    if (TodaySales.IsEnabled)
                        ucTodaySales(lang);
                    break;
                case Key.F5:
                    if (Queries.IsEnabled)
                        ucQueries(lang);
                    break;
                case Key.F6:
                    if (DailyClose.IsEnabled)
                        ucDailyClose(lang);
                    break;
                case Key.F7:
                    if (Inventory.IsEnabled)
                        wpfInventory(lang);
                    break;
                case Key.F8:
                    if (Maintenance.IsEnabled)
                        ucMaintenance(lang);
                    break;
                case Key.F9:
                    ucCashDrawer(lang);
                    break;
                case Key.System:    // F10
                    if (IsTransInProgress()) return;
                    this.Close();
                    break;
                case Key.F12:
                    this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
                    break;
                case Key.Insert:
                    wpfChartExample chart = new wpfChartExample();
                    chart.ShowDialog();
                    break;
            }
        }
        private void localTimer_Tick(object sender, EventArgs eArgs)
        {
            try
            {
                localTimer.Stop();

                IsProgressBarIndeterminate.Visibility = Settings.Default.IsProgressBarIndeterminateActive ? Visibility.Visible : Visibility.Hidden;

                if (Helper.CheckLicenseExpiration())
                {
                    wpfMessageBox.Show("Tickets Controller", strLicenseExpired, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    App.Current.Shutdown();
                    return;
                }

                if (DB.Open(Settings.Default.TicketsControllerDbConn) == null)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: NO SE LOGRÓ COMUNICACIÓN CON LA BASE DE DATOS, EL SISTEMA NO PUEDE CONTINUAR. POR FAVOR, INTENTE REINICIANDO LA COMPUTADORA", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    App.Current.Shutdown();
                    return;
                }

                Helper.CheckDateOfWeekForBackup();

                this.Opacity = 0.5;
                wpfRequestPIN wpfPIN = new wpfRequestPIN();
                wpfPIN.ShowDialog();
                this.Opacity = 1;

                if (wpfPIN.numKeyed == "0")
                {
                    App.Current.Shutdown();
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;

                userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

                if (userProf.userActive == false)
                {
                    wpfMessageBox.Show("Tickets Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                    App.Current.Shutdown();
                    return;
                }

                DB.InsertTimecard(wpfPIN.numKeyed, true);
                Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
                Settings.Default.WhoOpenName = userProf.userName;
                Settings.Default.Save();

                //check if dailyClosing must be done

                clsCustomerVIP awcDC = DB.GetCustomerProfile(Settings.Default.DBMasterKey);

                if (awcDC.ID == 0)
                {
                    wpfMessageBox.Show("Tickets Controller",
                                       "ERROR: Registro 'DBMasterKey' no existe en la base de datos, la aplicación será abortada. Por favor, comuníquese con AIDAware Consultancies inmediatamente.",
                                       System.Windows.MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);

                    App.Current.Shutdown();
                    return;
                }

                string bussinessDay = DB.ConverTicketDate(awcDC.LastPayment);

                string today = DB.ConverTicketDate(DateTime.Now.ToString("yyyyMMdd"));

                if (today != bussinessDay)
                {
                    Mouse.OverrideCursor = null;

                    this.Opacity = 0.5;
                    wpfDailyClosing dc = new wpfDailyClosing();
                    dc.ShowDialog();
                    this.Opacity = 1;

                    if (dc.IsDailyClosing)
                    {
                        if (wpfMessageBox.Show("Tickets Controller",
                                               "ADVERTENCIA: Todas las cuentas abiertas serán cerradas y clasificadas como cuentas pendientes. Realmente desea continuar?",
                                               MessageBoxButton.YesNo, wpfMessageBox.MessageBoxImage.Question, null) == MessageBoxResult.No)
                        {
                            App.Current.Shutdown();
                            return;
                        }

                        Mouse.OverrideCursor = Cursors.Wait;

                        Helper.ShowToastNotification("Actualizando fecha contable");
                        Helper.CleanTempFiles(Path.GetTempPath(), Settings.Default.DatabaseBackupExpiration);

                        DB.TruncateTable("tbl_BartenderOrder");
                        DB.TruncateTable("tbl_TicketsProforms");
                        DB.RebuildAllIndexes();

                        SMTP.SendAWCDigitalCommerceBackup(Helper.ZIPDatabase(DB.AWCDigitalCommerceDBBackup()));
                        
                        DB.UpdateTemporalTablesIDWithDeletedID();
                        DB.ApplyOpenTicketsToInventory(awcDC.LastPayment);

                        if (Settings.Default.ApplyZeroToItemSoldAtInventory)
                            DB.ApplyZeroToItemSoldAtInventory();

                        DB.MoveOpenTicketsToDailyClosing(awcDC.LastPayment);
                        DB.UpdateFeeServiceToOpenTickets();
                        DB.UpdateCustomerStatus(DB.GetCustomerID(Settings.Default.DBMasterKey), 0);

                        Settings.Default.BusinessDate = DateTime.Now.ToString("yyyyMMdd");
                        Settings.Default.Shift = 1;
                        Settings.Default.Save();

                        DB.NormalizeCustomerID();
                        DB.DeleteOpenTickets(0);
                        DB.NormalizeDailyClosingTable();

                        this.Opacity = 0.5;

                        Mouse.OverrideCursor = null;
                        wpfCashRegisterOpen cro = new wpfCashRegisterOpen(lang);
                        cro.ShowDialog();

                        this.Opacity = 1;

                        SMTP.SendBusinessDateChangeAlertByEMail();

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, String.Format(strBusinessDateLog, Settings.Default.WhoOpen, bussinessDay, today, cro.CashRegisterAmount), Logger.Severity.WARNING);
                    }
                }
                else
                {
                    Settings.Default.BusinessDate = DateTime.Now.ToString("yyyyMMdd");
                }

                lblTodayDate.Content = $"FECHA CONTABLE: {DB.ConverTicketDate(Settings.Default.BusinessDate)}";
                tabCtrlWorkArea.Items.Clear();

                #region BOTTOM FLAGS
                this.ActiveUer.ToolTip = "Usuario Activo: " + userProf.userName;
                this.ActiveUer.Visibility = Visibility.Visible;

                if (SMTP.CheckInternetConnection())
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, strInternetOK, Logger.Severity.INFORMATION);
                    this.InternetOK.ToolTip = strInternetOK;
                    this.InternetOK.Visibility = Visibility.Visible;
                }

                List<clsItem> ItemsBelowMinimum = DB.GetItemsBelowMinimum();

                if (ItemsBelowMinimum.Count > 0)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, strMinimumAvailable, Logger.Severity.WARNING);
                    this.MinimumAvailable.ToolTip = strMinimumAvailable;
                    this.MinimumAvailable.Visibility = Visibility.Visible;
                }

                List<clsItem> ItemsBelowZero = DB.GetItemsBelowZero();

                if (ItemsBelowZero.Count > 0)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, strItemsBelowZero, Logger.Severity.WARNING);
                    this.ItemsBelowZero.ToolTip = strItemsBelowZero;
                    this.ItemsBelowZero.Visibility = Visibility.Visible;
                }

                if (ItemsBelowMinimum.Count == 0 && ItemsBelowZero.Count == 0)
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, strInventoryOK, Logger.Severity.WARNING);
                    this.InventoryOK.ToolTip = strInventoryOK;
                    this.InventoryOK.Visibility = Visibility.Visible;
                }

                if (Settings.Default.StartAlarmsThread)
                {
                    Threads.StartAlarms();

                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Monitor de Alarmas activado.", Logger.Severity.INFORMATION);
                    this.AlarmsOK.ToolTip = "Monitor de Alarmas activado.";
                    this.AlarmsOK.Visibility = Visibility.Visible;
                }

                #endregion

                Mouse.OverrideCursor = null;

                Helper.ShowToastNotification("Sesión abierta exitosamente");

                StackPanelRightBottom.Visibility = Visibility.Visible;

                SetUserAccessToResources2();

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Tickets Controller App initialized successfully.", Logger.Severity.INFORMATION);

                if (Settings.Default.BartenderOrderTickInSeconds > 0 && Settings.Default.UseBartenderOrdersMonitor)
                {
                    wpfBartenderOrdersMonitor bom = new wpfBartenderOrdersMonitor();
                    bom.Show();
                }
                else if (Settings.Default.BartenderOrderTickInSeconds > 0 && !Settings.Default.UseBartenderOrdersMonitor)
                {
                    bartenderOrder.Tick += new EventHandler(bartenderOrder_Tick);
                    bartenderOrder.Interval = new TimeSpan(0, 0, Settings.Default.BartenderOrderTickInSeconds);
                    bartenderOrder.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                App.Current.Shutdown();
                return;
            }
        }
        private void bartenderOrder_Tick(object sender, EventArgs eArgs)
        {
            try
            {
                bartenderOrder.Stop();

                if (Settings.Default.DisplayCurrentDateTime)
                {
                    CurrentDateTime.Content = DateTime.Now.ToString("dd.MM.yyyy hh:mm tt");
                }

                clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());
                CurrentUser.Content = $"COLABORADOR ACTIVO: {userProf.userName}";

                Mouse.OverrideCursor = Cursors.Wait;

                clsBartenderOrder order = DB.GetBartenderOrder();

                if (order.GUID.Length > 0)
                {
                    string[] bList = order.BeveragesList.ToString().Split('^');

                    List<string> b2p = new List<string>();

                    foreach (string b in bList)
                    {
                        b2p.Add(b);
                    }

                    Helper.PrintTicket(order.CustomerID, b2p, false);

                    DB.DeleteBartenderOrder(order.GUID);
                }

                // get ticket to be printed
                clsPrintTicketRemotely ticketSource = DB.GetTicketToPrintRemotely();

                if (ticketSource.GUID.Length > 0)
                {
                    clsTicketsForDataGrid ticketTarget = Helper.LoadFromXMLString(ticketSource.TicketForDataGrid);

                    xPrinterTicket xPrintTck = new xPrinterTicket(ticketTarget);
                    xPrintTck.print();

                    DB.DeleteTicketPrintedRemotely(ticketSource.GUID);
                }

                if (order.GUID.Length > 0 || ticketSource.GUID.Length > 0)
                {
                    Helper.ShowToastNotification("Orden Procesada");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"bartenderOrder_Tick ERROR: {ex.Message}", Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                bartenderOrder.Start();
            }
        }
        private void SetUserAccessToResources2()
        {
            try
            {
                QuickSale.IsEnabled = Helper.CheckUserAccessToResource2("QuickSale");
                Tickets.IsEnabled = Helper.CheckUserAccessToResource2("Tickets");
                OldTickets.IsEnabled = Helper.CheckUserAccessToResource2("OldTickets");
                TodaySales.IsEnabled = Helper.CheckUserAccessToResource2("TodaySales");
                Queries.IsEnabled = Helper.CheckUserAccessToResource2("Queries");
                DailyClose.IsEnabled = Helper.CheckUserAccessToResource2("DailyClose");
                Inventory.IsEnabled = Helper.CheckUserAccessToResource2("Inventory");
                Maintenance.IsEnabled = Helper.CheckUserAccessToResource2("Maintenance");
                CashDrawer.IsEnabled = Helper.CheckUserAccessToResource2("CashDrawer");

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "SetUserAccessToResources2 validation PASSED successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return;
            }
        }
        private void LoadControlsArray()
        {
            mainButtons = new List<Button> { QuickSale, Tickets, OldTickets, TodaySales, Queries, DailyClose, Inventory, Maintenance, CashDrawer };
            mainLabels = new List<Label> { F1, F2, F3, F4, F5, F6, F7, F8, F9 };
        }
        private void EnableDisableLeftControls(Button btnCaller, Label lblCaller, bool action)
        {
            try
            {
                switch (action)
                {
                    case true:
                        foreach (Button btn in mainButtons)
                            if (btn != btnCaller)
                                btn.Visibility = Visibility.Visible;
                        foreach (Label lbl in mainLabels)
                            if (lbl != lblCaller)
                                lbl.Visibility = Visibility.Visible;
                        break;
                    case false:
                        foreach (Button btn in mainButtons)
                            if (btn != btnCaller)
                                btn.Visibility = Visibility.Hidden;
                        foreach (Label lbl in mainLabels)
                            if (lbl != lblCaller)
                                lbl.Visibility = Visibility.Hidden;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        private bool IsTransInProgress()
        {
            if (transInProgress && ucTicketsShared.UpdateTicket.IsEnabled == false)
            {
                transInProgress = false;
                return false;
            }

            if (transInProgress && transInProgressTries <= 2)
            {
                wpfMessageBox.Show("Tickets Controller", "NO PUEDE REGRESAR, EXISTE UNA TRANSACCIÓN PENDIENTE DE TERMINAR.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                transInProgressTries++;
                return true;
            }
            else if (transInProgressTries == 3)
            {
                transInProgress = false;
            }

            transInProgressTries = 0;
            return false;
        }
        private StackPanel CreateHeaderForTabItem(TabItem newTab, string uriImage, string header)
        {
            StackPanel headerPanel = new StackPanel();
            headerPanel.Orientation = Orientation.Horizontal;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(uriImage));
            image.Width = 30;
            image.Height = 30;
            image.Margin = new Thickness(5, 0, 5, 0);
            headerPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = header;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            
            headerPanel.Children.Add(textBlock);

            newTab.FontSize = 15;
            newTab.FontWeight = FontWeights.DemiBold;

            return headerPanel;
        }
        #endregion

        #region BUTTONS
        private void btn_QuickSale(object sender, RoutedEventArgs e)
        {
            ucQuickOrder(lang);
        }
        private void btn_Tickets(object sender, RoutedEventArgs e)
        {
            ucTickets(lang);
        }
        private void btn_OldTickets(object sender, RoutedEventArgs e)
        {
            ucOldTickets(lang);
        }
        private void btn_TodaySales(object sender, RoutedEventArgs e)
        {
            ucTodaySales(lang);
        }
        private void btn_Queries(object sender, RoutedEventArgs e)
        {
            ucQueries(lang);
        }
        private void btn_DailyClose(object sender, RoutedEventArgs e)
        {
            ucDailyClose(lang);
        }
        private void btn_Inventory(object sender, RoutedEventArgs e)
        {
            wpfInventory(lang);
        }
        private void btn_Maintenance(object sender, RoutedEventArgs e)
        {
            ucMaintenance(lang);
        }
        private void btn_CashDrawer(object sender, RoutedEventArgs e)
        {
            ucCashDrawer(lang);
        }
        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            if (IsTransInProgress()) return;

            DB.InsertTimecard(Settings.Default.WhoOpen.ToString(), false);

            this.Close();
        }
        private void btn_PINRequest(object sender, MouseButtonEventArgs e)
        {
            if (IsTransInProgress()) return;

            this.Opacity = 0.5;
            wpfRequestPIN wpfPIN = new wpfRequestPIN();
            wpfPIN.ShowDialog();
            this.Opacity = 1;

            if (wpfPIN.numKeyed == "0") return;

            userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

            if (userProf.userActive == false)
            {
                wpfMessageBox.Show("Tickets Controller", strPINdoNotExist, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, lang);
                return;
            }

            Helper.ShowToastNotification($"Usuario Activo: { userProf.userName}");
            this.ActiveUer.ToolTip = "Usuario Activo: " + userProf.userName;

            Settings.Default.WhoOpen = Convert.ToInt32(wpfPIN.numKeyed);
            Settings.Default.WhoOpenName = userProf.userName;
            Settings.Default.Save();

            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "Settings.Default.Save() passed", Logger.Severity.DEBUG);
            SetUserAccessToResources2();
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "PIN was changed successfully.", Logger.Severity.INFORMATION);
        }
        private void btn_Help(object sender, MouseButtonEventArgs e)
        {
            ucHelp(lang);
        }
        private void btn_Cocktails(object sender, MouseButtonEventArgs e)
        {
            ucCocktails(lang);
        }

        #endregion

        #region TRANSACTIONS
        private void ucQuickOrder(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 1) return;   // a funtion key was pressed

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(QuickSale, F1, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 1;
                    EnableDisableLeftControls(QuickSale, F1, false);
                    var UC = new ucQuickOrder(this, lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/quickSale.png", "VENTA RÁPIDA");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucTickets(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 2) return;

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(Tickets, F2, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 2;
                    EnableDisableLeftControls(Tickets, F2, false);

                    // New Ticket Tab
                    var UC = new ucTickets(this, lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/restaurant.png", "CUENTAS ABIERTAS");
                    tabCtrlWorkArea.Items.Add(newTab);

                    // Quick Sale Tab
                    var QS = new ucQuickOrder(this, lang);
                    newTab = new TabItem { Content = QS };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/quickSale.png", "VENTA RÁPIDA");
                    tabCtrlWorkArea.Items.Add(newTab);

                    // Expenses Tab
                    var ET = new ucExpensesReport(lang);
                    newTab = new TabItem { Content = ET };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/expenses.png", "GASTOS INTERNOS");
                    tabCtrlWorkArea.Items.Add(newTab);

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        private void ucOldTickets(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 3) return;

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(OldTickets, F3, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 3;
                    EnableDisableLeftControls(OldTickets, F3, false);
                    var UC = new ucOldTickets(lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/conotrafico.png", "POR COBRAR");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucTodaySales(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 4) return;

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(TodaySales, F4, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 4;
                    EnableDisableLeftControls(TodaySales, F4, false);
                    var UC = new ucTodaySales(lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/Money.ico", "VENTAS DEL DÍA");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucQueries(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 5) return;

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(Queries, F5, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 5;
                    EnableDisableLeftControls(Queries, F5, false);

                    var UC = new ucQueries(this, lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/search.png", "CONSULTAS GENERALES");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucDailyClose(string lang)
        {
            try
            {
                if (!Settings.Default.WorkStationType.Contains("MASTER"))
                {
                    wpfMessageBox.Show("Ticket Controller", "ATENCIÓN: ESTA ESTACIÓN NO ESTÁ CONFIGURADA PARA REALIZAR CIERRE DIARIO", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                if (IsTransInProgress()) return;

                if (isActive && tokenID != 6) return;

                // Implementtion of Blind DailyClose
                if (Settings.Default.AllowBlindDailyClosing && !userProf.userAccessLevel.ToUpper().StartsWith("ADMIN"))
                {
                    this.Opacity = 0.5;
                    wpfBlindDailyClosing blindDC = new wpfBlindDailyClosing();
                    blindDC.ShowDialog();
                    this.Opacity = 1;
                    return;
                }

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(DailyClose, F6, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 6;
                    EnableDisableLeftControls(DailyClose, F6, false);
                    var UC = new ucDailyClose(lang);
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/report.png", "CIERRE");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void wpfInventory(string lang)
        {
            if (IsTransInProgress()) return;

            wpfInventoryWindow wpfInv = new wpfInventoryWindow();
            wpfInv.ShowDialog();
        }
        private void ucMaintenance(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive && tokenID != 8) return;

                if (isActive)
                {
                    isActive = false;
                    tokenID = 0;
                    EnableDisableLeftControls(Maintenance, F8, true);
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    tokenID = 8;
                    EnableDisableLeftControls(Maintenance, F8, false);

                    // Tables Maintenance
                    if (Helper.CheckUserAccessToResource2("Maintenance_Daily"))
                    {
                        var TablesMaintenance = new ucTablesMaintenance2();
                        newTab = new TabItem { Content = TablesMaintenance };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/quickSale.png", "DIARIOS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Users Maintenance
                    if (Helper.CheckUserAccessToResource2("Maintenance_UsersMgmt"))
                    {
                        var UsersMaintenance = new ucUsersMaintenance(lang);
                        newTab = new TabItem { Content = UsersMaintenance };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/waitress.png", "COLABORADORES");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Expenses
                    if (Helper.CheckUserAccessToResource2("Maintenance_GralExpenses"))
                    {
                        var ExpensesReport = new ucExpensesReport(lang);
                        newTab = new TabItem { Content = ExpensesReport };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/Money.ico", "GASTOS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    if (Helper.CheckUserAccessToResource2("Maintenance_Specials"))
                    {
                        var Specials = new ucSpecials(lang);
                        newTab = new TabItem { Content = Specials };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/kitchen.ico", "COMIDAS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Tickets Maintenance
                    if (Helper.CheckUserAccessToResource2("Maintenance_InternalOrders"))
                    {
                        var InternalOrders = new ucInternalOrders();
                        newTab = new TabItem { Content = InternalOrders };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/email.png", "PEDIDOS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Incomes
                    if (Helper.CheckUserAccessToResource2("Maintenance_IncomeCash"))
                    {
                        var IncomeCash = new ucIncomeCash();
                        newTab = new TabItem { Content = IncomeCash };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/money_icon.png", "INGRESOS A CAJA");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Defective Itemes
                    if (Helper.CheckUserAccessToResource2("Maintenance_DefectiveItems"))
                    {
                        var Maint = new ucMaintenance(lang);
                        newTab = new TabItem { Content = Maint };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/damage.png", "DAÑADOS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Loyalty Rewards
                    if (Helper.CheckUserAccessToResource2("Maintenance_LoyaltyMgmt"))
                    {
                        var LoyaltyRewards = new ucLoyaltyRewards(lang);
                        newTab = new TabItem { Content = LoyaltyRewards };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/loyalty.png", "LEALTAD");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }

                    // Vouchers
                    if (Helper.CheckUserAccessToResource2("Maintenance_Vouchers"))
                    {
                        var Vouchers = new ucVouchers();
                        newTab = new TabItem { Content = Vouchers };
                        newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/icons8-tarjeta-de-regalo-94.png", "VOUCHERS");
                        tabCtrlWorkArea.Items.Add(newTab);
                    }
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucCashDrawer(string lang)
        {
            xPrinterOpenCashbox xpCash = new xPrinterOpenCashbox();
            xpCash.print();
            Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Open Cash Drawer request by user {Settings.Default.WhoOpen}", Logger.Severity.WARNING);
            DB.InsertOpenCashDrawerRequest();  
            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();
        }
        private void ucHelp(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive)
                {
                    isActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    var UH = new ucHelp();
                    newTab = new TabItem { Content = UH };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/help.png", "RECURSOS DE AYUDA");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void ucCocktails(string lang)
        {
            try
            {
                if (IsTransInProgress()) return;

                if (isActive)
                {
                    isActive = false;
                    tabCtrlWorkArea.Items.Clear();
                    newTab.Background = Brushes.DarkBlue;
                }
                else
                {
                    isActive = true;
                    var UC = new ucCocktails();
                    newTab = new TabItem { Content = UC };
                    newTab.Header = CreateHeaderForTabItem(newTab, "pack://application:,,,/Images/cocktails.png", "LISTA DE COCTÉLES");

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(newTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        #endregion

        #region MOUSE DOWN
        private void mouseDown_xpworld(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", strInternetOK, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
        }
        private void mouseDown_Information(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", strInventoryOK, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
        }
        private void mouseDown_MinimumAvailable(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", strMinimumAvailable, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
        }
        private void mouseDown_ItemsBelowZero(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", strItemsBelowZero, MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
        }
        private void mouseDown_activeUser(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", $"USUARIO ACTIVO: {userProf.userName}" + Environment.NewLine + $"PERFIL DE TRABAJO: {userProf.userAccessLevel}", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
        }
        private void mouseDown_Alarms(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", "Monitor de Alarmas activado.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);
        }
        private void mouseDown_AWC(object sender, MouseButtonEventArgs e)
        {
            this.Opacity = 0.5;
            wpfAWCSplashScreen aWCSplashScreen = new wpfAWCSplashScreen();
            aWCSplashScreen.ShowDialog();
            this.Opacity = 1;
        }
        private void mouseDown_chatGPT(object sender, MouseButtonEventArgs e)
        {
            wpfMessageBox.Show("Tickets Controller", "ChatGPT está activo", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Information, lang);

        }
        #endregion
    }
}
