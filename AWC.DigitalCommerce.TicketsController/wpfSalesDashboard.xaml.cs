using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AWC.DigitalCommerce.TicketsController
{
    /// <summary>
    /// Interaction logic for wpfSalesDashboard.xaml
    /// </summary>
    public partial class wpfSalesDashboard : Window
    {
        private string startDate = string.Empty;
        private string finishDate = string.Empty;

        public wpfSalesDashboard()
        {
            InitializeComponent();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = StartDate.SelectedDate.ToString();

            if (startDate.Length == 0) return;

            string year = startDate.Split('/')[2].Substring(0, 4);
            string month = startDate.Split('/')[1].PadLeft(2, '0');
            string day = startDate.Split('/')[0].PadLeft(2, '0');

            startDate = year + month + day;
        }

        private void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            finishDate = FinishDate.SelectedDate.ToString();

            if (finishDate.Length == 0) return;

            string year = finishDate.Split('/')[2].Substring(0, 4);
            string month = finishDate.Split('/')[1].PadLeft(2, '0');
            string day = finishDate.Split('/')[0].PadLeft(2, '0');

            finishDate = year + month + day;

            if (Convert.ToInt32(startDate) > Convert.ToInt32(finishDate))
            {
                wpfMessageBox.Show("Tickets Controller", "ERROR: FECHA INICIAL NO PUEDE SER MAYOR QUE LA FECHA FINAL.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Error, "");
                return;
            }

            btnSearch.IsEnabled = true;
        }

        private void btn_search(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // group by Quantity
                List<clsItemType> pieItemsList = DB.DataBinding_tbl_TicketsDetail(startDate, finishDate, 0);

                if (pieItemsList.Count == 0)
                {
                    MessageBox.Show("ATENCIÓN: La combinación de parámetros seleccionados NO encontró información", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                PieItemsListSeriesTab5.ItemsSource = pieItemsList;

                // group by Total Price
                List<clsItemType> barItemsList = DB.DataBinding_tbl_TicketsDetail(startDate, finishDate, 1);

                BarItemsListSeriesTab5.ItemsSource = barItemsList;

                CharsStackPanel.Visibility = Visibility.Visible;

                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                Log.Error($"wpfSalesDashboard_btn_search ERROR: {ex.Message} StackTrace: {ex.StackTrace}");
            }
        }

        private void btn_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
