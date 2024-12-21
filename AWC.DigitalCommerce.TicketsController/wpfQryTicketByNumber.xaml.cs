using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Interaction logic for wpfQryTicketByNumber.xaml
    /// </summary>
    public partial class wpfQryTicketByNumber : Window
    {
        private List<clsItemDetailForDatagrid> itemdg = new List<clsItemDetailForDatagrid>();
        public wpfQryTicketByNumber()
        {
            if (Settings.Default.TopLeftOn)
            {
                this.Top = 50;
                this.Left = 50;
            }

            InitializeComponent();
            txtSearchTicket.Focus();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtSearchTicket_GotFocus(object sender, RoutedEventArgs e)
        {
            CleanAll();

            if (Settings.Default.VirtualNumericKeyboardActive)
            {
                wpfNumericKeyboard numKey = new wpfNumericKeyboard();
                numKey.ShowDialog();
                txtSearchTicket.Text = numKey.numKeyed;
            }
        }

        private void txtSearchTicket_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtSearchTicket.Text.Length == 0) return;

                if (e.Key == Key.Return || e.Key == Key.Tab)
                {
                    int ticketNum = Convert.ToInt32(txtSearchTicket.Text);

                    // get ticket
                    clsTicket ticket = DB.GetTicket(ticketNum);

                    if (ticket.ID == 0)
                    {
                        MessageBox.Show("Número de cuenta NO existe, por favor verifique el número", "Tickets Controller", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    CustomerID.Content = DB.GetCustomerIDByID(ticket.CustID);

                    // display footeer
                    TotalTicket.Content = string.Format("{0:N0}", ticket.TotalPrice);

                    switch (ticket.PayMethod)
                    {
                        case 0:
                            ticket.StatusAlpha = "ABIE";
                            break;
                        case 1:
                            ticket.StatusAlpha = "CANC";
                            break;
                        case 2:
                            ticket.StatusAlpha = "ANUL";
                            break;
                        case 3:
                            ticket.StatusAlpha = "HERE";
                            break;
                    }

                    if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                    {
                        ticket.PayMethodAlpha = "EFECT";
                    }
                    else if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                    {
                        ticket.PayMethodAlpha = "TCRED";
                    }
                    else if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                    {
                        ticket.PayMethodAlpha = "SINPE";
                    }
                    else
                    {
                        if (ticket.PayMethod == 0)
                        {
                            ticket.PayMethodAlpha = "PEND";
                        }
                        if (ticket.PayMethod > 1)
                        {
                            ticket.PayMethodAlpha = ticket.StatusAlpha;
                        }
                        else
                        {
                            if (ticket.StatusAlpha == "ABIE")
                            {
                                ticket.PayMethodAlpha = "PEND";
                            }
                            else
                            {
                                ticket.PayMethodAlpha = "MIXTO";
                            }
                        }
                    }

                    DateAndPayMethod.Content = $"FECHA: {DB.ConverTicketDate(ticket.TicketDate)} - PAGO: {ticket.PayMethodAlpha}";

                    InOutDatetime.Content = $"ABIERTO: {ticket.CreateAt.ToString("dd-MM HH:mm")}";

                    if (!ticket.PayMethodAlpha.Contains("PEND"))
                    {
                        InOutDatetime.Content += $" - CERRADO: {ticket.CloseAt.ToString("dd-MM HH:mm")}";
                    }

                    // get detail
                    itemdg = DB.GetItemsByGUID(ticket.GUID, false);
                    TicketDetail.ItemsSource = itemdg;

                    if (ticket.ServiceFee > 0)
                    {
                        Helper.AddServiceFee(ticket.ServiceFee, itemdg, this);
                    }

                    TicketDetail.Items.Refresh();
                    PrintTicket.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void CleanAll()
        {
            CustomerID.Content = string.Empty;
            TotalTicket.Content = string.Empty;
            DateAndPayMethod.Content = string.Empty;
            TicketDetail.Items.Clear();
        }

        private void PrintTicket_Click(object sender, RoutedEventArgs e)
        {
            clsTicket tck = DB.GetTicket(Convert.ToInt32(txtSearchTicket.Text));

            clsTicketsForDataGrid tmp = Helper.Convert2TicketsForDataGrid(tck, CustomerID.Content.ToString());

            Helper.PrintTicket(tmp);

            this.Close();
        }
    }
}
