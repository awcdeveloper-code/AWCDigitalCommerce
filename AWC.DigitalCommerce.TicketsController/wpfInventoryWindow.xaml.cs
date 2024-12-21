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
using System.Windows.Shapes;
using System.Windows.Threading;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Controls;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfInventoryWindow : Window
    {
        #region GLOBAL VARIABLES
        private DispatcherTimer localTimer = new DispatcherTimer();
        private TabItem ucNewTab;
        private TabItem ucTablesMaintenanceTab;

        private bool btnProviders = false;
        private bool btnInvoices = false;
        private bool btnNotes = false;
        private bool btnDamage = false;
        private bool btnInventoryStatus = false;
        private bool btnReports = false;
        private bool btnMaintenance = false;
        private bool btnRequests = false;
        #endregion

        public wpfInventoryWindow()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            this.KeyDown += new KeyEventHandler(this_KeyDown);

            TodayDate.Content = DB.ConverTicketDate(Settings.Default.BusinessDate);
            Logger.WriteToLog("InventoriesManagement", "Inventories Management App initialized successfully.", Logger.Severity.INFORMATION);
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            }
        }

        private void EnableDisableLeftButtons(int source)
        {
            switch (source)
            {
                // DISABLE BUTTONS
                case 1: // Invoices
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 2: // Inventory Status
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 3: // Providers
                    {
                        Invoices.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 4: // Reports
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 5: // Maintenance
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 6: // Notes
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 7: // Damage
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        Requests.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 8: // Internal Orders
                    {
                        Providers.Visibility = Visibility.Collapsed;
                        Invoices.Visibility = Visibility.Collapsed;
                        ProductsLost.Visibility = Visibility.Collapsed;
                        CreditDebitNotes.Visibility = Visibility.Collapsed;
                        InventoryStatus.Visibility = Visibility.Collapsed;
                        Reports.Visibility = Visibility.Collapsed;
                        break;
                    }
                // ENABLE BUTTONS
                case 11: // Invoices
                    {
                        Providers.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 12: // Inventory Status
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 13: // Providers
                    {
                        Invoices.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 14: // Reports
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 15: // Maintenance
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 16: // Notes
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 17: // Damage
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
                case 18: // Internal Orders
                    {
                        Providers.Visibility = Visibility.Visible;
                        Invoices.Visibility = Visibility.Visible;
                        ProductsLost.Visibility = Visibility.Visible;
                        CreditDebitNotes.Visibility = Visibility.Visible;
                        InventoryStatus.Visibility = Visibility.Visible;
                        Reports.Visibility = Visibility.Visible;
                        Requests.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private void btn_Exit(object sender, RoutedEventArgs e)
        {
            Logger.WriteToLog("InventoriesManagement", "Inventories Management App closed.", Logger.Severity.INFORMATION);
            this.Close();
        }

        private void btn_Providers(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnProviders)
                {
                    btnProviders = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Providers.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(13);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnProviders = true;
                    Providers.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(3);
                    Splash.Visibility = Visibility.Hidden;

                    var ucProviders = new ucProviders();
                    ucNewTab = new TabItem { Content = ucProviders };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/providers.png", "PROVEEDORES");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Invoices(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnInvoices)
                {
                    btnInvoices = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Invoices.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(11);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnInvoices = true;
                    Invoices.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(1);
                    Splash.Visibility = Visibility.Hidden;

                    var ucInvoices = new ucInvoices();
                    ucNewTab = new TabItem { Content = ucInvoices };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/invoice.png", "FACTURAS");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_CreditDebitNotes(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnNotes)
                {
                    btnNotes = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Invoices.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(16);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnNotes = true;
                    Invoices.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(6);
                    Splash.Visibility = Visibility.Hidden;

                    var ucNotes = new ucNotes();
                    ucNewTab = new TabItem { Content = ucNotes };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/AdmTmpl_4.ico", "NOTAS");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("btn_CreditDebitNotes", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_ProductsLost(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnDamage)
                {
                    btnDamage = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Invoices.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(17);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnDamage = true;
                    Invoices.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(7);
                    Splash.Visibility = Visibility.Hidden;

                    var ucDamagedProducts = new ucDamagedProducts();
                    ucNewTab = new TabItem { Content = ucDamagedProducts };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/damage.png", "DAÑADOS");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("btn_CreditDebitNotes", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_InventoryStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnInventoryStatus)
                {
                    btnInventoryStatus = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    InventoryStatus.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(12);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnInventoryStatus = true;
                    InventoryStatus.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(2);
                    Splash.Visibility = Visibility.Hidden;

                    var ucInventoryStatus = new ucInventoryStatus();
                    ucNewTab = new TabItem { Content = ucInventoryStatus };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/PriceList.png", "ESTADO DEL INVENTARIO");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Reports(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnReports)
                {
                    btnReports = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Reports.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(14);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnReports = true;
                    Reports.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(4);
                    Splash.Visibility = Visibility.Hidden;

                    var ucReportsInventoryMgmt = new ucReportsInventoryMgmt();
                    ucNewTab = new TabItem { Content = ucReportsInventoryMgmt };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/printer_icon.png", "REPORTES");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
        }

        private void btn_Requests(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnRequests)
                {
                    btnRequests = false;
                    tabCtrlWorkArea.Items.Clear();
                    tabCtrlWorkArea.Visibility = Visibility.Collapsed;
                    Reports.Background = Brushes.DarkBlue;
                    EnableDisableLeftButtons(18);
                    Splash.Visibility = Visibility.Visible;
                }
                else
                {
                    btnRequests = true;
                    Reports.Background = Brushes.GreenYellow;
                    EnableDisableLeftButtons(8);
                    Splash.Visibility = Visibility.Hidden;

                    var ucInternalOrders = new ucInternalOrders();
                    ucNewTab = new TabItem { Content = ucInternalOrders };
                    ucNewTab.Header = CreateHeaderForTabItem(ucNewTab, "pack://application:,,,/Images/email.png", "PEDIDOS");
                    ucNewTab.FontSize = 20;
                    ucNewTab.FontWeight = FontWeights.DemiBold;

                    // WorkArea Tab Manager
                    tabCtrlWorkArea.Items.Add(ucNewTab);
                    tabCtrlWorkArea.Items.Refresh();
                    tabCtrlWorkArea.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("InventoriesManagement", ex, Logger.Severity.ERROR);
            }
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
    }
}
