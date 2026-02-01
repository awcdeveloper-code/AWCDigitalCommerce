using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;
using iText.Layout.Properties;
using Newtonsoft.Json;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    /// <summary>
    /// Interaction logic for ucOldTickets.xaml
    /// </summary>
    public partial class ucTodaySales : UserControl
    {
        private int customerID = 0;
        private string workDay = Settings.Default.BusinessDate;
        private string lang = string.Empty;
        public string strPrintAllTickets = string.Empty;
        public string strPrintAllClosedTickets = string.Empty;
        List<clsTicketsForDataGrid> itemdg = new List<clsTicketsForDataGrid>();
        public ucTodaySales(string _lang)
        {
            lang = _lang;

            InitializeComponent();

            Traductor.ApplyTranslation(this, lang);

            LoadTodaySales(Settings.Default.BusinessDate);
        }
        private void LoadTodaySales(string businessDate)
        {
            try
            {
                itemdg = DB.DataBinding_tbl_Tickets(businessDate, 3);

                TodayTickets.ItemsSource = itemdg;
                TodayTickets.Items.Refresh();

                // get total price
                int totalPrice = itemdg.Sum(x => x.TotalPrice);
                TotalTodayTickets.Content = "TOTAL: " + totalPrice.ToString("N0").PadLeft(7);

                InitializeButtons();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }

        private void btn_EmergencyPrint(object sender, RoutedEventArgs e)
        {
            foreach (clsTicketsForDataGrid item in TodayTickets.Items)
            {
                if (item.Status)
                    Helper.PrintTicket(item);
            }
        }
        private void btn_PrintClosed(object sender, RoutedEventArgs e)
        {
            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
                Helper.PrintTicket(item);
        }
        private void TodayTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetUserAccessToResources();

            if (!Settings.Default.ATVApplyFee)
            {
                ElectronicInvoice.IsEnabled = false;
            }

            clsTicketsForDataGrid tck = TodayTickets.SelectedItem as clsTicketsForDataGrid;

            if (tck == null) return;

            if (tck.PayMethod == 2 || tck.Status == false)
            {
                AbortTicket.IsEnabled = false;
            }
        }
        private void TodayTickets_GotFocus(object sender, RoutedEventArgs e)
        {
            SetUserAccessToResources();
        }
        private void InitializeButtons()
        {
            Print.IsEnabled = true;
            PrintClosed.IsEnabled = true;
            PrintFoodService.IsEnabled = true;
            AbortTicket.IsEnabled = true;
            ChangeName.IsEnabled = true;
            ElectronicInvoice.IsEnabled = false;
        }
        private void btn_PrintFoodService(object sender, RoutedEventArgs e)
        {
            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
                Helper.PrintTicket(item, 2);
        }
        private void btn_FakeTicket(object sender, RoutedEventArgs e)
        {
            wpfEnterAmount wpfea = new wpfEnterAmount();
            wpfea.ShowDialog();

            if (wpfea.amount == 0) return;

            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
            {
                item.TotalPrice = wpfea.amount;
                Helper.PrintTicket(item, 2);
            }
        }
        private void btn_eMailTicket(object sender, RoutedEventArgs e)
        {
            if (!SMTP.CheckInternetConnection())
            {
                wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EN ESTE MOMENTO NO HAY CONEXIÓN A INTERNET, POR FAVOR INTENTE MAS TARDE.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                return;
            }

            wpfMailAddress wpfma = new wpfMailAddress(GetFirstMailAddressFromTicketsList());
            wpfma.ShowDialog();

            if (wpfma.bCancel) return;

            int option = wpfma.restVoucher ? 4 : 3;

            wpfSplashWindow swnd = new wpfSplashWindow(lang, TodayTickets, option, wpfma.mailAddress, customerID);
            swnd.ShowDialog();

        }
        private void btn_AbortTicket(object sender, RoutedEventArgs e)
        {
            try
            {
                clsUser userProf = new clsUser();

                this.Opacity = 0.5;
                wpfRequestPIN wpfPIN = new wpfRequestPIN();
                wpfPIN.ShowDialog();
                this.Opacity = 1;

                if (wpfPIN.numKeyed == "0")
                {
                    return;
                }

                userProf = Helper.CheckUserProfile(wpfPIN.numKeyed);

                if (!userProf.userPowerAdmin)
                {
                    wpfMessageBox.Show("Tickets Controller", "ATENCIÓN: EL PIN INGRESADO NO TIENE PERMISO PARA ANULAR CUENTAS.", MessageBoxButton.OK, wpfMessageBox.MessageBoxImage.Warning, lang);
                    return;
                }

                wpfAbortReason ar = new wpfAbortReason();
                ar.ShowDialog();

                if (string.IsNullOrEmpty(ar.abortReason)) return;

                foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
                {
                    DB.IncludeAbortReason(item.ID, ar.abortReason,Convert.ToInt32(userProf.userPIN));

                    DB.InsertNewTicketAborted(item.ID);

                    DB.DeleteTicketDetail(DB.GetTicketGUID(item.ID), false);

                    DB.CancelTicket(item.ID, Settings.Default.WhoOpen, 2);

                    wpfSplashWindow sw = new wpfSplashWindow(1, lang);
                    sw.ShowDialog();
                }

                LoadTodaySales(workDay);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
        private void btn_ChangeName(object sender, RoutedEventArgs e)
        {
            wpfChangeCustName chgName = new wpfChangeCustName();
            chgName.ShowDialog();

            if (chgName.bCancel) return;

            int option = 0;

            if (chgName.restVoucher)
            {
                option = 2;
            }

            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
                Helper.PrintTicket(item, option, chgName.newName);
        }
        private void btn_ElectronicInvoice(object sender, RoutedEventArgs e)
        {
            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
            {
                clsTicket ticket = DB.GetTicket(item.ID);

                this.Opacity = 0.5;
                wpfElectronicInvoice einv = new wpfElectronicInvoice(ticket.ID);
                einv.ShowDialog();
                this.Opacity = 1;

                if (einv.bCancel)
                {
                    return;
                }

                ATVQuery atvqry = new ATVQuery();

                atvqry.TicketID = ticket.ID;
                atvqry.CustomerName = einv.custName;
                atvqry.SSN_Type = einv.custIDType;
                atvqry.SSN = einv.custID;
                atvqry.CountryCode = einv.custCountryCode;
                atvqry.PhoneNumber = einv.custPhoneNumber;
                atvqry.eMailAddress = einv.custEmail;

                ElectronicDoc ATV = new ElectronicDoc();
                ATV.DocElectronico = new DocElectronico();

                // header
                ATV.DocElectronico.Token = Settings.Default.ATVToken;
                ATV.DocElectronico.CodigoActividad = Settings.Default.ATVActivityCode;
                ATV.DocElectronico.Cliente = Settings.Default.ATVClientCode;

                // receptor info
                ATV.DocElectronico.Receptor = new WhoReceive();
                ATV.DocElectronico.Receptor.Nombre = einv.custName;

                ATV.DocElectronico.Receptor.Identificacion = new SSN();
                ATV.DocElectronico.Receptor.Identificacion.Tipo = einv.custIDType;
                ATV.DocElectronico.Receptor.Identificacion.Numero = einv.custID;

                ATV.DocElectronico.Receptor.Telefono = new PhoneNumber();
                ATV.DocElectronico.Receptor.Telefono.CodigoPais = einv.custCountryCode;
                ATV.DocElectronico.Receptor.Telefono.NumTelefono = einv.custPhoneNumber;
                ATV.DocElectronico.Receptor.CorreoElectronico = einv.custEmail;

                // ticket header
                ATV.DocElectronico.CondicionVenta = 1;

                if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                {
                    ATV.DocElectronico.MedioPago = "01";
                }
                else if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                {
                    ATV.DocElectronico.MedioPago = "02";
                }
                else if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                {
                    ATV.DocElectronico.MedioPago = "04";
                }
                else
                {
                    if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                    {
                        ATV.DocElectronico.MedioPago = "01,02";
                    }
                    else if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                    {
                        ATV.DocElectronico.MedioPago = "01,04";
                    }
                    else if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                    {
                        ATV.DocElectronico.MedioPago = "01,02,04";
                    }
                    else if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer > 0)
                    {
                        ATV.DocElectronico.MedioPago = "02,04";
                    }
                    else
                    {
                        ATV.DocElectronico.MedioPago = "01";
                    }
                }

                // ticket detail
                LineDetail lineDetail = new LineDetail();
                lineDetail.NumeroLinea = 1;
                lineDetail.Codigo = 6331000000000;

                lineDetail.CodigoComercial = new ComercialCode();
                lineDetail.CodigoComercial.Tipo = 1;
                lineDetail.CodigoComercial.Codigo = 4;

                lineDetail.Cantidad = 1;
                lineDetail.UnidadMedida = "Unid";
                lineDetail.Detalle = "SERVICIO DE RESTAURANTE";
                lineDetail.PrecioUnitario = ticket.TotalPrice;

                lineDetail.Descuento = new Discount();
                lineDetail.Descuento.MontoDescuento = 0;
                lineDetail.Descuento.NaturalezaDescuento = "SIN DESCUENTO";

                lineDetail.SubTotal = ticket.TotalPrice;

                lineDetail.Impuesto = new Tax();
                lineDetail.Impuesto.Codigo = 1;
                lineDetail.Impuesto.CodigoTarifa = 8;
                lineDetail.Impuesto.Tarifa = 13;
                lineDetail.Impuesto.Monto = lineDetail.SubTotal * 13 / 100;

                lineDetail.MontoTotalLinea = lineDetail.SubTotal + lineDetail.Impuesto.Monto;

                // ticket summary
                ATV.DocElectronico.DetalleServicio = new ServiceDetail();
                ATV.DocElectronico.DetalleServicio.LineaDetalle = new List<LineDetail>();
                ATV.DocElectronico.DetalleServicio.LineaDetalle.Add(lineDetail);

                ATV.DocElectronico.OtrosCargos = new OtherCharges();
                ATV.DocElectronico.OtrosCargos.TipoDocumento = 6;
                ATV.DocElectronico.OtrosCargos.Detalle = "Impuesto de Servicio 10%";
                ATV.DocElectronico.OtrosCargos.MontoCargo = 0;

                ATV.DocElectronico.ResumenFactura = new TicketSummary();
                ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda = new CurrencyTypeCode();
                ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.CodigoMoneda = "CRC";
                ATV.DocElectronico.ResumenFactura.CodigoTipoMoneda.TipoCambio = 1;

                // Serializing JSON
                string jsonOutput = JsonConvert.SerializeObject(ATV);
                JSON.ATVSendWebServiceCall(ticket.ID, jsonOutput);
            }
        }
        private void btn_ChangePayMethod(object sender, RoutedEventArgs e)
        {
            DB.GetPayMethodChanges(Settings.Default.BusinessDate);

            clsTicketsForDataGrid tck = TodayTickets.SelectedItem as clsTicketsForDataGrid;
            clsPayMethodChange pm = new clsPayMethodChange();
            clsTicket ticket = DB.GetTicket(tck.ID);

            pm.TicketDate = ticket.TicketDate;
            pm.TicketID = tck.ID;

            pm.OrigCash = tck.Cash;
            pm.OrigCreditCard = tck.CreditCard;
            pm.OrigTransfer = tck.Transfer;

            wpfPayMethod2 payForm = new wpfPayMethod2(lang, tck.TotalPrice, tck.ID, true, 0);
            payForm.ShowDialog();

            if (payForm.payOK == false)
            {
                this.Opacity = 1;
                return;
            }

            pm.CurrCash = payForm.cash;
            pm.CurrCreditCard = payForm.creditCard;
            pm.CurrTransfer = payForm.transfer;

            DB.UpdateTicketStatus(tck.ID, payForm.cash, payForm.creditCard, payForm.transfer);

            DB.InsertPayMethodChange(pm);

            LoadTodaySales(Settings.Default.BusinessDate);

            this.Opacity = 0.5;
            wpfSplashWindow sw = new wpfSplashWindow(1, lang);
            sw.ShowDialog();
            this.Opacity = 1;
        }
        private void SetUserAccessToResources()
        {
            try
            {
                Print.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_Print");
                PrintClosed.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_PrintClosed");
                PrintFoodService.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_PrintFoodService");
                FakeTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_FakeTicket");
                eMailTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_eMailTicket");
                AbortTicket.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_AbortTicket");
                ChangeName.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_ChangeName");
                ElectronicInvoice.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_ElectronicInvoice");
                ChangePayMethod.IsEnabled = Helper.CheckUserAccessToResource2("ucTodaySales_ChangePayMethod");
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "SetUserAccessToResources2 validation PASSED successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
                return;
            }
        }
        private string GetFirstMailAddressFromTicketsList()
        {
            string mailAddress = string.Empty;

            customerID = 0;

            foreach (clsTicketsForDataGrid item in TodayTickets.SelectedItems)
            {
                clsCustomerVIP custProf = DB.GetCustomerProfile(item.CustomerAKA);
                
                if (custProf != null)
                {
                    if (custProf.Type == 1)
                    {
                        customerID = custProf.ID;
                        mailAddress = custProf.MailAddress;
                        break;
                    }
                }
            }

            return mailAddress;
        }
    }
}
