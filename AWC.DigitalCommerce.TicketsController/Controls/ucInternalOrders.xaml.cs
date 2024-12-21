using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AWC.DigitalCommerce.TicketsController.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucInternalOrders : UserControl
    {
        private string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private bool bStore = Settings.Default.StoreInternalOrders;

        public ucInternalOrders()
        {
            InitializeComponent();
            txtOrderDescription.Focus();
        }

        private void CheckFieldsContent()
        {
            if (txtOrderDescription.Text.Length > 0 && txtProductDescription.Text.Length > 0 && txtProductQty.Text.Length > 0)
            {
                btnAddProduct.IsEnabled = true;
            }
        }

        private void CleanAll()
        {
            txtOrderDescription.Text = string.Empty;
            txtProviderEmail.Text = string.Empty;
            txtProductDescription.Text = string.Empty;
            txtProductQty.Text = string.Empty;

            dgItemsList.Items.Clear();

            btnAddProduct.IsEnabled = false;
            btnDeleteProduct.IsEnabled = false;
            btnPrintOrder.IsEnabled = false;
            btnSaveOrder.IsEnabled = false;
        }

        private void txtOrderDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFieldsContent();
        }

        private void txtProviderEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(txtProviderEmail.Text, pattern) && dgItemsList.Items.Count > 0)
            {
                btnSaveOrder.IsEnabled = true;
            }
        }

        private void txtProductDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFieldsContent();
        }

        private void txtProductQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFieldsContent();
        }

        private void dgItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDeleteProduct.IsEnabled = true;
        }

        private void btn_AddProduct(object sender, RoutedEventArgs e)
        {
            dgItemsList addItem2dg = new dgItemsList();

            addItem2dg.ItemDescription = txtProductDescription.Text;
            addItem2dg.ItemQty = Convert.ToInt32(txtProductQty.Text);

            dgItemsList.Items.Add(addItem2dg);

            btnPrintOrder.IsEnabled = true;

            if (Regex.IsMatch(txtProviderEmail.Text, pattern))
            {
                btnSaveOrder.IsEnabled = true;
            }

            txtProductDescription.Text = string.Empty;
            txtProductQty.Text = string.Empty;
            btnAddProduct.IsEnabled = false;

            txtProductDescription.Focus();
        }

        private void btn_DeleteProduct(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgItemsList.SelectedItem;

            if (selectedItem != null)
                dgItemsList.Items.Remove(selectedItem);

            btnDeleteProduct.IsEnabled = false;

            if (dgItemsList.Items.Count == 0)
            {
                btnPrintOrder.IsEnabled= false;
                btnSaveOrder.IsEnabled = false;
            }
            else
            {
                btnSaveOrder.IsEnabled = true;
            }
        }

        private void btn_PrintOrder(object sender, RoutedEventArgs e)
        {
            string order2print = Path.Combine(Settings.Default.SerilogRootPath, Settings.Default.BusinessDate + "_" + txtOrderDescription.Text + ".txt");

            using (StreamWriter sw = new StreamWriter(order2print, false))
            {
                sw.WriteLine(txtOrderDescription.Text.ToUpper());

                foreach (dgItemsList item in dgItemsList.Items)
                {
                    sw.WriteLine(item.ItemDescription.ToUpper() + "," + item.ItemQty);
                    sw.Flush();
                }
            }

            Helper.PrintInternalOrder(order2print);

            if(Settings.Default.StoreInternalOrders)
            {
                DB.InsertInternalOrder(order2print);
                bStore = true;
            }

            File.Delete(order2print);
            CleanAll();
        }

        private void btn_SaveOrder(object sender, RoutedEventArgs e)
        {
            if (SMTP.CheckInternetConnection())
            {
                string order2print = Path.Combine(Settings.Default.SerilogRootPath, Settings.Default.BusinessDate + "_" + txtOrderDescription.Text + ".txt");

                using (StreamWriter sw = new StreamWriter(order2print, false))
                {
                    sw.WriteLine(txtOrderDescription.Text.ToUpper());

                    foreach (dgItemsList item in dgItemsList.Items)
                    {
                        sw.WriteLine(item.ItemDescription + "," + item.ItemQty);
                        sw.Flush();
                    }
                }

                Mouse.OverrideCursor = Cursors.Wait;
                SMTP.SendInternalOrderByEMail(txtProviderEmail.Text + ", " + Settings.Default.eMailDistributionList, order2print);
                Mouse.OverrideCursor = null;
                File.Delete(order2print);
                CleanAll();
            }
            else
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EN ESTE MOMENTO NO HAY ACCESO A INTERNET, EL CORREO NO PODRÁ SER ENVIADO.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, null);
            }
        }
    }
}
