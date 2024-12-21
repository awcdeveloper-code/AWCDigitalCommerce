using System;
using System.Collections.Generic;
using System.Data;
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
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public partial class wpfQryTicketsByCustomerID : Window
    {
        private bool CleanTicketClicked = false;
        private string customerID = string.Empty;

        public wpfQryTicketsByCustomerID()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();

            //data binding section
            cbox_CustomerID.DataContext = DB.DataBinding_tbl_CustomerID(0, 0);
        }

        private void cbox_CustomerID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CleanTicketClicked)
            {
                DataRowView row = cbox_CustomerID.SelectedItem as DataRowView;
                customerID = row["CustomerID"].ToString();

                // fill datagrid with ticket details
                int ID = DB.GetIDByCustomerID(customerID);

                if (ID == 0)
                {
                    MessageBox.Show("No se encontro ID para [" + customerID + "]");
                    return;
                }
                else
                {
                    // get items of the ticket
                    List<clsTicketsForDataGrid> itemdg = DB.DataBinding_tbl_Tickets(ID, 2);  // All the tickets of a Customer

                    //TicketsList.Items.Clear();
                    TicketsList.ItemsSource = itemdg;
                    TicketsList.Items.Refresh();

                    if (TicketsList.Items.Count == 0)
                    {
                        MessageBox.Show("ATENCIÓN: El cliente " + customerID + " no tiene historial de cuentas disponible");
                        return;
                    }
                }
            }
        }
    }
}
