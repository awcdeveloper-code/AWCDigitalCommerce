using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using AWC.DigitalCommerce.TicketsController.Classes;
using AWC.DigitalCommerce.TicketsController.Properties;
using Microsoft.Office.Interop.Excel;

namespace AWC.DigitalCommerce.TicketsController
{
    public class SMTP
    {
        #region GLOBAL VARIABLES

        private static string smtpAddress = "smtp.gmail.com";
        private static int portNumber = 587;
        private static bool enableSSL = true;
        private static string emailFromAddress = "aidawareconsultancies@gmail.com";
        private static string password = "ucfyocmdgujnhtrm";
        private static string emailToAddress = Settings.Default.eMailDistributionList;
        private static string currentYear = DateTime.Now.ToString("yyyy");
        private static List<clsItemDetailForDatagrid> productsList = new List<clsItemDetailForDatagrid>();

        #endregion

        public static bool CheckInternetConnection()
        {
            try
            {
                string host = "8.8.8.8";

                Ping p = new Ping();

                PingReply reply = p.Send(host, 3000);

                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
        private static void SendEMail(string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Bcc.Add(emailFromAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            Helper.ShowToastNotification("Correo enviado exitosamente");
        }
        private static void SendEMail(string subject, string body, string eMail)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(eMail);
                mail.Bcc.Add(emailFromAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            Helper.ShowToastNotification("Correo enviado exitosamente");
        }
        public static bool SendDailyReport(clsDailyClosing dcRep, string dateProc, List<clsTicketsForDataGrid> ticketsList, int shift = 0)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                int shiftForQuery = 0;

                if (shift == 0)
                    shiftForQuery = Settings.Default.ShiftForQuery;
                else
                    shiftForQuery = shift;

                #region HEADER
                string subject = Settings.Default.BusinessName + $" - Cierre Diario del {DB.ConverTicketDate(dateProc)} Turno {shiftForQuery}";

                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");
                //
                // SYSTEM DAILY CLOSE SUMMARY
                //
                sb.Append($"<h2>CIERRE CONTABLE DEL SISTEMA - TURNO {Settings.Default.ShiftForQuery}</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");
                sb.Append("<tr><td>CAJA INICIAL (CI)</td ><td class=\"amount\">" + dcRep.InitialCash.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>INGRESOS A CAJA (IC)</td ><td class=\"amount\">" + dcRep.IncomeCash.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>EFECTIVO (E)</td ><td class=\"amount\">" + dcRep.Cash.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>GASTOS GENERALES (G)</td ><td class=\"amount\">" + dcRep.Expenses.ToString("N0") + "</td></tr>");

                int totCash = (dcRep.InitialCash + dcRep.IncomeCash + dcRep.Cash) - (int)dcRep.Expenses;
                sb.Append("<tr><th>TOTAL EN EFECTIVO (CI + IC + E - G)</th><th class=\"amount\">" + totCash.ToString("N0") + "</th></tr>");

                sb.Append("<tr><td>POR COBRAR (P)</td ><td class=\"amount\">" + dcRep.AccountsReceivable.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>TARJETA DE CRÉDITO (C)</td ><td class=\"amount\">" + dcRep.CreditCard.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>TRANS SINPE (S)</td ><td class=\"amount\">" + dcRep.Transfer.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>VOUCHERS (V)</td ><td class=\"amount\">" + dcRep.Voucher.ToString("N0") + "</td></tr>");
                sb.Append("<tr><th>VENTA BRUTA (P + E + C + S)</th><th class=\"amount\">" + dcRep.GrossSale.ToString("N0") + "</th></tr>");
                sb.Append("<tr><th>VENTA NETA (E + C + S)</th><th class=\"amount\">" + dcRep.NetSale.ToString("N0") + "</th></tr>");

                int cashBoxTot = (dcRep.InitialCash + dcRep.IncomeCash + dcRep.Cash + dcRep.CreditCard + dcRep.Transfer) - (dcRep.ServiceFee + (int)dcRep.Expenses);
                sb.Append("<tr><th>TOTAL EN CAJA (CI + IC + E + C + S + V - G)</th><th class=\"amount\">" + cashBoxTot.ToString("N0") + "</th></tr>");

                sb.Append("<tr><td>10% SERVICIO</td ><td class=\"amount\">" + dcRep.ServiceFee.ToString("N0") + "</td></tr>");
                sb.Append("<tr><td>ABONOS + PAGOS DE CXC</td ><td class=\"amount\">" + dcRep.OldTicketsPay.ToString("N0") + "</td></tr>");
                sb.Append("</table>");
                sb.Append("<p><br></p>");
                #endregion

                #region CASHWITHDRAW
                if (dcRep.CashWithdrawal > 0)
                {
                    sb.Append($"<h3>EFECTIVO REMANENTE EN CAJA</h3>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");
                    sb.Append("<tr><td>DISPONIBLE</td ><td class=\"amount\">" + totCash.ToString("N0") + "</td></tr>");
                    sb.Append("<tr><td>RETIRO<td class=\"amount\">" + dcRep.CashWithdrawal.ToString("N0") + "</td></tr>");
                    sb.Append("<tr><td>REMANENTE</td ><td class=\"amount\">" + (totCash -  dcRep.CashWithdrawal).ToString("N0") + "</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region OPERATOR DAILY CLOSE SUMMARY
                if (Settings.Default.AllowBlindDailyClosing)
                {
                    string dailyClosingMatch = string.Empty;

                    if (dcRep.DailyClosingMatch)
                    {
                        dailyClosingMatch = "(EXITOSO)";
                    }
                    else
                    {
                        dailyClosingMatch = "(CON DIFERENCIAS)";
                    }

                    sb.Append($"<h2>CIERRE CONTABLE DEL OPERADOR {dailyClosingMatch}</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>DESCRIPCIÓN</th><th>SISTEMA</th><th>OPERADOR</th><th>DIFERENCIA</th></tr>");

                    sb.Append("<tr><td>EFECTIVO (CI + IC + E - G)</td ><td class=\"amount\">" + totCash.ToString("N0") + "</td><td class=\"amount\">" + dcRep.CashByOperator.ToString("N0") + "</td><td class=\"amount\">" + (totCash - dcRep.CashByOperator).ToString("N0") + "</td></tr>");
                    sb.Append("<tr><td>TARJETA DE CRÉDITO</td ><td class=\"amount\">" + dcRep.CreditCard.ToString("N0") + "</td><td class=\"amount\">" + dcRep.CreditCardByOperator.ToString("N0") + "</td><td class=\"amount\">" + (dcRep.CreditCard - dcRep.CreditCardByOperator).ToString("N0") + "</td></tr>");
                    sb.Append("<tr><td>TRANS SINPE</td ><td class=\"amount\">" + dcRep.Transfer.ToString("N0") + "</td><td class=\"amount\">" + dcRep.TransferByOperator.ToString("N0") + "</td><td class=\"amount\">" + (dcRep.Transfer - dcRep.TransferByOperator).ToString("N0") + "</td></tr>");
                    sb.Append("<tr><td>VOUCHERS</td ><td class=\"amount\">" + dcRep.Voucher.ToString("N0") + "</td><td class=\"amount\">" + dcRep.VoucherByOperator.ToString("N0") + "</td><td class=\"amount\">" + (dcRep.Voucher - dcRep.VoucherByOperator).ToString("N0") + "</td></tr>");

                    int totVert1 = totCash + dcRep.CreditCard + dcRep.Transfer + dcRep.Voucher;
                    int totVert2 = dcRep.CashByOperator + dcRep.CreditCardByOperator + dcRep.TransferByOperator + dcRep.VoucherByOperator;
                    int totVert3 = (totCash - dcRep.CashByOperator) +
                                   (dcRep.CreditCard - dcRep.CreditCardByOperator) +
                                   (dcRep.Transfer - dcRep.TransferByOperator) +
                                   (dcRep.Voucher - dcRep.VoucherByOperator);

                    sb.Append("<tr><th class=\"amount\">TOTALES:</th><th class=\"amount\">" + totVert1.ToString("N0") + "</th><th class=\"amount\">" + totVert2.ToString("N0") + "</th><th class=\"amount\">" + totVert3.ToString("N0") + "</th></tr>");
                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region EXPENSES
                if (dcRep.ExpensesList.Count > 0)
                {
                    sb.Append("<h2>GASTOS REALIZADOS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>DESCRIPCIÓN DEL PRODUCTO</th><th>MONTO</th></tr>");

                    foreach (clsExpense expense in dcRep.ExpensesList)
                    {
                        sb.Append("<tr><td>" + expense.ExpenseDescription + "</td><td class=\"amount\">" + expense.ExpenseAmount.ToString("N0") + "</td></tr>");
                    }

                    sb.Append("<tr><th>TOTAL:</th><th class=\"amount\">" + dcRep.Expenses.ToString("N0") + "</th></tr>");
                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region PRODUCTS SOLD
                List<clsItemDetailForDatagrid> itemsList = DB.GetItemsByDate(dateProc, dateProc, 4);

                if (itemsList.Count > 0)
                {
                    sb.Append("<h2>PRODUCTOS VENDIDOS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th><th>STOCK</th></tr>");

                    int total = 0;

                    foreach (clsItemDetailForDatagrid sale in itemsList)
                    {
                        sb.Append("<tr><td class=\"amount\">" + sale.Qty + "</td><td>" + sale.ItemDesc + "</td><td class=\"amount\">" + sale.TotalPrice.ToString("N0") + "</td><td class=\"amount\">" + sale.ItemAvailable + "</td></tr>");
                        total += sale.TotalPrice;
                    }

                    sb.Append("<tr><th/><th class=\"amount\">TOTAL VENDIDO:</th><th class=\"amount\">" + total.ToString("N0") + "</th></tr>");
                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region PRODUCTS DELETED FROM SYSTEM
                List<clsItemDeletedFromSystem> idfs = DB.ListBinding_tbl_ItemsDeletedFromSystem(dateProc, dateProc);

                if (idfs.Count > 0)
                {
                    sb.Append("<h2>PRODUCTOS ELIMINADOS DEL SISTEMA</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN</th><th>COLABORADOR</th><th>FECHA-HORA</th></tr>");

                    foreach (clsItemDeletedFromSystem dfs in idfs)
                    {
                        sb.Append("<tr><td>" + dfs.ItemID + "</td><td>" + dfs.ItemDescription + "</td><td>" + dfs.WhoDeletedName + "</td><td>" + dfs.DeletedAtString + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region TICKETS LIST
                if (ticketsList.Count > 0)
                {
                    sb.Append("<h2>RESUMEN DE CUENTAS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>No. CTA</th><th>CLIENTE</th><th>ESTADO</th><th>PAGO</th><th>MONTO</th></tr>");

                    foreach (clsTicketsForDataGrid ticket in ticketsList)
                    {
                        sb.Append("<tr><td class=\"amount\">" + ticket.ID + "</td><td>" +
                                                                ticket.CustomerID + "</td><td>" +
                                                                ticket.StatusAlpha + "</td><td class=\"amount\">" +
                                                                ticket.PayMethodAlpha + "</td><td>" +
                                                                ticket.TotalPrice.ToString("N0") + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region SMALL PAYMENTS
                List<clsSmallPayment> smlPayList = DB.GetSmallPayments(Settings.Default.BusinessDate);

                if (smlPayList.Count > 0)
                {
                    sb.Append("<h2>ABONOS A CUENTAS PENDIENTES</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>No. REF</th><th>No. CTA</th><th>CLIENTE</th><th>EFECTIVO</th><th>TARJ CRED</th><th>TRANSFER</th><th>SALDO</th></tr>");

                    foreach (clsSmallPayment smlPay in smlPayList)
                    {
                        sb.Append("<tr><td class=\"amount\">" + smlPay.RandomRef + "</td><td>" +
                                                                smlPay.TicketID + "</td><td>" +
                                                                DB.GetCustomerIDByID(smlPay.CustomerID) + "</td><td class=\"amount\">" +
                                                                smlPay.Cash.ToString("N0") + "</td><td class=\"amount\">" +
                                                                smlPay.CreditCard.ToString("N0") + "</td><td class=\"amount\">" +
                                                                smlPay.Transfer.ToString("N0") + "</td><td class=\"amount\">" +
                                                                smlPay.NewTotalPrice.ToString("N0") + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region PRODUCTS: CHANGE PRICES
                List<clsItemsChangePrice> ItemsChangePrice = new List<clsItemsChangePrice>();
                ItemsChangePrice = DB.GetItemsChangePrice(Settings.Default.BusinessDate);

                if (ItemsChangePrice.Count > 0)
                {
                    sb.Append($"<h2>PRODUCTOS QUE CAMBIARON DE PRECIO</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>COLABORADOR</th><th>PRODUCTO</th><th>PRECIO ANTERIOR</th><th>PRECIO ACTUAL</th><th>FECHA/HORA DEL CAMBIO</th></tr>");

                    foreach (clsItemsChangePrice itemChangePrice in ItemsChangePrice)
                    {
                        clsUser user = DB.CheckUserPIN(itemChangePrice.WhoDidit);
                        clsItem item = DB.GetItem(itemChangePrice.ItemID);

                        sb.Append("<tr><td>" + user.userName +
                                  "</td><td>" + item.ItemDescription +
                                  "</td><td>" + itemChangePrice.PreviousPrice.ToString("N0") +
                                  "</td><td>" + itemChangePrice.CurrentPrice.ToString("N0") +
                                  "</td><td>" + itemChangePrice.MadeItAt.ToString() + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region PAY METHOD CHANGE
                //
                // PAY METHOD CHANGE
                //
                List<clsPayMethodChange> pmcl = new List<clsPayMethodChange>();
                pmcl = DB.GetPayMethodChanges(Settings.Default.BusinessDate);

                if (pmcl.Count > 0)
                {
                    sb.Append($"<h2>CAMBIOS EN FORMA DE PAGO DE CUENTAS CANCELADAS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>COLABORADOR</th><th>CUENTA</th><th>EFEC ANT</th><th>EFEC ACT</th><th>TARJ CRED ANT</th><th>TARJ CRED ACT</th><th>SINPE ANT</th><th>SINPE ACT</th><th>HORA DEL CAMBIO</th></tr>");

                    foreach (clsPayMethodChange pmc in pmcl)
                    {
                        clsUser user = DB.CheckUserPIN(pmc.WhoDidIt);

                        sb.Append("</td><td>" + user.userName +
                                  "</td><td>" + pmc.TicketID +
                                  "</td><td>" + pmc.OrigCash +
                                  "</td><td>" + pmc.CurrCash +
                                  "</td><td>" + pmc.OrigCreditCard +
                                  "</td><td>" + pmc.CurrCreditCard +
                                  "</td><td>" + pmc.OrigTransfer +
                                  "</td><td>" + pmc.CurrTransfer +
                                  "</td><td>" + pmc.MadeItAt.ToString("dd-MM-yyyy HH:mm") + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region INTERNAL EXPENSES
                List<clsExpense> clsExpenses = DB.GetExpenses(Settings.Default.BusinessDate);

                if (clsExpenses.Count > 0)
                {
                    sb.Append("<h2>GASTOS INTERNOS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>DESCRIPCIÓN DEL PRODUCTO</th><th>MONTO</th></tr>");

                    foreach (clsExpense expense in clsExpenses)
                    {
                        sb.Append($"<tr><td>{expense.ExpenseDescription}</td><td>{expense.ExpenseAmount}</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region VOUCHERS
                if (dcRep.VouchersList.Count > 0)
                {
                    sb.Append("<h2>VOUCHERS EMITIDOS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>VOUCHER</th><th>QUIÉN LO EMITIÓ</th><th>MONTO</th></tr>");

                    foreach (clsVoucher v in dcRep.VouchersList)
                    {
                        clsUser u = DB.CheckUserPIN(v.IssueBy);
                        sb.Append($"<tr><td>{v.ID}</td><td>{u.userName}</td><td>{v.Amount}</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region OPEN DRAWER REQUEST
                List<clsOpenDrawerRequest> openDrawerRequestList = DB.GetOpenDrawerRequest(Settings.Default.BusinessDate);

                if (openDrawerRequestList.Count > 0)
                {
                    sb.Append("<h2>APERTURA GAVETA DE DINERO</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>COLABORADOR</th><th>FECHA HORA DEL EVENTO</th></tr>");

                    foreach (clsOpenDrawerRequest odr in openDrawerRequestList)
                    {
                        clsUser user = DB.CheckUserPIN(odr.WhoOpen.ToString());

                        sb.Append("<tr><td class=\"ItemID\">" + user.userName +
                                  "</td><td>" + odr.CreatedAt.ToString() + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region LIST OF CUSTOMERS WHO LEFT THE TICKET OPEN
                List<clsTicketsForDataGrid> ticketsLeftOpen = DB.DataBinding_tbl_Tickets(Settings.Default.BusinessDate, 4);

                if (ticketsLeftOpen.Count > 0)
                {
                    sb.Append("<h2>CUENTAS QUE QUEDARON ABIERTAS</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>No. CTA</th><th>CLIENTE</th><th>MONTO</th></tr>");

                    foreach (clsTicketsForDataGrid ticket in ticketsLeftOpen)
                    {
                        sb.Append("<tr><td class=\"amount\">" + ticket.ID + "</td><td>" +
                                                                ticket.CustomerID + "</td><td>" +
                                                                ticket.TotalPrice.ToString("N0") + "</td></tr>");
                    }
                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region ACCOUNTS RECEIVABLE SUMMARY
                List<clsDelincuency> delincuenciesList = DB.GetDelincuencies("202%");

                if (delincuenciesList.Count > 0)
                {
                    sb.Append("<h2>CUENTAS POR COBRAR</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>ID</th><th>NOMBRE DEL CLIENTE</th><th>1 a 8d</th><th>9 a 15d</th><th>16 a 30d</th><th>31 a 45d</th><th>46 a 60d</th><th>Más de 60d</th></tr>");

                    int sum_0_8_days = 0;
                    int sum_9_15_days = 0;
                    int sum_16_30_days = 0;
                    int sum_31_45_days = 0;
                    int sum_46_60_days = 0;
                    int sum_61_days = 0;
                    int grandTotal = 0;

                    foreach (clsDelincuency delincuent in delincuenciesList)
                    {
                        sb.Append("<tr><td class=\"center\">" + delincuent.ID +
                                    "</td><td>" + delincuent.CustomerName +
                                    "</td><td class=\"amount\">" + delincuent.sum_0_8_days.ToString("N0") +
                                    "</td><td class=\"amount\">" + delincuent.sum_9_15_days.ToString("N0") +
                                    "</td><td class=\"amount\">" + delincuent.sum_16_30_days.ToString("N0") +
                                    "</td><td class=\"amount\">" + delincuent.sum_31_45_days.ToString("N0") +
                                    "</td><td class=\"amount\">" + delincuent.sum_46_60_days.ToString("N0") +
                                    "</td><td class=\"amount\">" + delincuent.sum_61_days.ToString("N0") +
                                    "</td></tr>");

                        sum_0_8_days += delincuent.sum_0_8_days;
                        sum_9_15_days += delincuent.sum_9_15_days;
                        sum_16_30_days += delincuent.sum_16_30_days;
                        sum_31_45_days += delincuent.sum_31_45_days;
                        sum_46_60_days += delincuent.sum_46_60_days;
                        sum_61_days += delincuent.sum_61_days;
                    }

                    sb.Append("<tr><td></td><th class=\"amount\">SUB-TOTALES:" +
                                "</th><th class=\"amount\">" + sum_0_8_days.ToString("N0") +
                                "</th><th class=\"amount\">" + sum_9_15_days.ToString("N0") +
                                "</th><th class=\"amount\">" + sum_16_30_days.ToString("N0") +
                                "</th><th class=\"amount\">" + sum_31_45_days.ToString("N0") +
                                "</th><th class=\"amount\">" + sum_46_60_days.ToString("N0") +
                                "</th><th class=\"amount\">" + sum_61_days.ToString("N0") +
                                "</th></tr>");

                    grandTotal = sum_0_8_days + sum_9_15_days + sum_16_30_days + sum_31_45_days + sum_46_60_days + sum_61_days;

                    sb.Append("</table><h2>TOTAL CUENTAS POR COBRAR: " + grandTotal.ToString("N0") + "</h2>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region INVENTORY: PRODUCTS BELOW MINIMUM
                //
                // INVENTORY: PRODUCTS BELOW MINIMUM
                //
                List<clsItem> ItemsBelowMinimum = new List<clsItem>();
                ItemsBelowMinimum = DB.GetItemsBelowMinimum();

                if (ItemsBelowMinimum.Count > 0)
                {
                    sb.Append("<h2>PRODUCTOS POR DEBAJO DEL MÍNIMO</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN DEL PRODUCTO</th><th>MÍNIMO</th><th>DISPONIBLE</th><th>MÁXIMO</th><th>PEDIR</th></tr>");

                    foreach (clsItem item in ItemsBelowMinimum)
                    {
                        int mustOrder = 0;

                        if (item.ItemStock > item.ItemAvailable)
                        {
                            mustOrder = item.ItemStock - item.ItemAvailable;
                        }

                        sb.Append("<tr><td class=\"center\">" + item.ID +
                                  "</td><td>" + item.ItemDescription +
                                  "</td><td class=\"amount\">" + item.ItemMinimum.ToString("N0") +
                                  "</td><td class=\"amount\">" + item.ItemAvailable.ToString("N0") +
                                  "</td><td class=\"amount\">" + item.ItemStock.ToString("N0") +
                                  "</td><td class=\"amount\">" + mustOrder.ToString("N0") +
                                  "</td></tr>");
                    }
                    sb.Append("</table>");
                    //
                    // INVENTORY: PROVIDERS AVAILABLE
                    //
                    List<clsProvider> providersList = new List<clsProvider>();
                    providersList = DB.GetProvidersListByItemsSold(ItemsBelowMinimum);

                    if (providersList.Count == 0)
                    {
                        sb.Append("<h2>ATENCIÓN: No hay información disponible sobre proveedores que puedan sutir dichos productos.</h2>");
                    }
                    else
                    {
                        sb.Append("<h2>PROVEEDORES CON ESTOS PRODUCTOS</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>NOMBRE DEL PROVEEDOR</th><th>FORMA DE PAGO</th><th>TELÉFONO</th><th>CELULAR</th><th>COMENTARIOS</th></tr>");

                        foreach (clsProvider provider in providersList)
                        {
                            sb.Append("<tr><td class=\"center\">" + provider.ProviderName +
                                      "</td><td>" + provider.PaymentMethod +
                                      "</td><td class=\"amount\">" + provider.PhoneNumber +
                                      "</td><td class=\"amount\">" + provider.CellularNumber +
                                      "</td><td class=\"amount\">" + provider.Remarks +
                                      "</td></tr>");
                        }
                        sb.Append("</table>");
                    }
                }
                #endregion

                #region EMPLOYEES LOGIN/LOGOUT
                //
                // EMPLOYEES LOGIN/LOGOUT
                //
                List<clsTimeCard> timeCardList = DB.GetTimeCards(Settings.Default.BusinessDate);

                if (timeCardList.Count > 0)
                {
                    sb.Append("<h2>INGRESO/SALIDA DE COLABORADORES</h2>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>COLABORADOR</th><th>EVENTO</th><th>FECHA HORA DEL EVENTO</th></tr>");

                    foreach (clsTimeCard tc in timeCardList)
                    {
                        string eventType = tc.EventType == 1 ? "INGRESO" : "SALIDA";
                        clsUser user = DB.CheckUserPIN(tc.UserPIN.ToString());

                        sb.Append("<tr><td class=\"ItemID\">" + user.userName +
                                  "</td><td>" + eventType +
                                  "</td><td>" + tc.EventDatetime.ToString() + "</td></tr>");
                    }

                    sb.Append("</table>");
                    sb.Append("<p><br></p>");
                }
                #endregion

                #region FOOTER AND SEND EMAIL

                sb.Append("</body></html>");
                sb.Append("<p>© " + currentYear + " AIDAware Servicio Automático de Notificaciones<br>La información contenida en este mensaje de correo electrónico y/o los archivos adjuntos contienen información confidencial o privilegiada. Si usted no es el destinatario previsto, cualquier difusión, uso, revisión, distribución, impresión o copia de la información contenida en este mensaje de correo electrónico y/o sus archivos adjuntos están estrictamente prohibidos. Si ha recibido esta comunicación por error, por favor notifíquenos al correo electrónico emisor y elimine de forma inmediata y permanente el mensaje y los archivos adjuntos. Gracias.</p>");

                string body = sb.ToString();

                SendEMail(subject, body);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static bool SendDailyReport(clsDailyClosing dcRep, string date1, string date2)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                string subject = Settings.Default.BusinessName + " - Cierre Resumen del " + DB.ConverTicketDate(date1) + " al " + DB.ConverTicketDate(date2);

                string body = "<!DOCTYPE html><html><head><style>table {  font-family: arial, sans-serif;  border-collapse: collapse;  width: 50%;} th {  border: 1px solid #dddddd;  text-align: center;  padding: 8px;} td {  border: 1px solid #dddddd;  text-align: left;  padding: 8px;} tr:nth-child(even) {background-color: #808080;} .amount { text-align : right; }</style></head><body><h2>CIERRE RESUMEN: ^BUSSDAY</h2><table><tr><th>DESCRIPCION</th><th>MONTO</th></tr><tr><td>POR COBRAR</td><td class=\"amount\">^COBRAR</td></tr><tr><td>EFECTIVO</td><td class=\"amount\">^CASH</td></tr><tr><td>TARJETA DE CRÉDITO</td><td class=\"amount\">^TARJCRED</td></tr><tr><td>TRANSFERENCIA SINPE</td><td class=\"amount\">^SINPE</td></tr><tr><td>TOTAL DE VENTAS</td><td class=\"amount\">^TOTVENTA</td></tr><tr><td>10% SERVICIO</td><td class=\"amount\">^SERVICIO</td></tr><tr><td>GASTOS VARIOS</td><td class=\"amount\">^GASTOS</td></tr></table></body></html>";
                body = body.Replace("^BUSSDAY", DB.ConverTicketDate(date1) + "-" + DB.ConverTicketDate(date2));
                body = body.Replace("^COBRAR", dcRep.AccountsReceivable.ToString("N0").PadLeft(9));
                body = body.Replace("^CASH", dcRep.Cash.ToString("N0").PadLeft(9));
                body = body.Replace("^TARJCRED", dcRep.CreditCard.ToString("N0").PadLeft(9));
                body = body.Replace("^SINPE", dcRep.Transfer.ToString("N0").PadLeft(9));
                body = body.Replace("^TOTVENTA", dcRep.GrossSale.ToString("N0").PadLeft(9));
                body = body.Replace("^SERVICIO", dcRep.ServiceFee.ToString("N0").PadLeft(9));
                body = body.Replace("^GASTOS", dcRep.Expenses.ToString("N0").PadLeft(9));

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Bcc.Add(emailFromAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static bool SendSalesSummary(string dateProc)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                List<clsItemDetailForDatagrid> itemsList = DB.GetItemsByDate(dateProc, dateProc, 4);

                string subject = Settings.Default.BusinessName + " - Resumen de Productos Vendidos el " + DB.ConverTicketDate(dateProc);

                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: center; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");
                sb.Append("<h2>PRODUCTOS VENDIDOS</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

                int total = 0;

                foreach (clsItemDetailForDatagrid sale in itemsList)
                {
                    sb.Append("<tr><td class=\"amount\">" + sale.Qty + "</td><td>" + sale.ItemDesc + "</td><td class=\"amount\">" + sale.TotalPrice.ToString("N0") + "</td></tr>");
                    total += sale.TotalPrice;
                }

                sb.Append("<tr><th/><th class=\"amount\">TOTAL VENDIDO:</th><th class=\"amount\">" + total.ToString("N0") + "</th></tr>");
                sb.Append("</table></body></html>");

                string body = sb.ToString();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Bcc.Add(emailFromAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static bool SendSalesSummary(string date1, string date2)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                List<clsItemDetailForDatagrid> itemsList = DB.GetItemsByDate(date1, date2, 4);

                string subject = Settings.Default.BusinessName + " - Resumen de Productos Vendidos del " + DB.ConverTicketDate(date1) + " al " + DB.ConverTicketDate(date2);

                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: center; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");
                sb.Append("<h2>PRODUCTOS VENDIDOS</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

                int total = 0;
                foreach (clsItemDetailForDatagrid sale in itemsList)
                {
                    sb.Append("<tr><td class=\"amount\">" + sale.Qty + "</td><td>" + sale.ItemDesc + "</td><td class=\"amount\">" + sale.TotalPrice.ToString("N0") + "</td></tr>");
                    total += sale.TotalPrice;
                }

                sb.Append("<tr><th/><th class=\"amount\">TOTAL VENDIDO:</th><th class=\"amount\">" + total.ToString("N0") + "</th></tr>");
                sb.Append("</table></body></html>");

                string body = sb.ToString();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Bcc.Add(emailFromAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static void KitchenSummary(List<clsItemDetailForDatagrid> mealList)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html>");
            sb.Append("<html><head><style>");
            sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
            sb.Append("th {border: 1px solid #dddddd; text-align: center; padding: 8px;}");
            sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
            sb.Append("tr:nth-child(even) {background-color: #f1f1f1;}");
            sb.Append(".amount {text-align : right;}");
            sb.Append("</style></head>");
            sb.Append("<body>");
            sb.Append("<h2>CIERRE DE COCINA</h2>");
            sb.Append("<table>");
            sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

            int total = 0;
            foreach (clsItemDetailForDatagrid meal in mealList)
            {
                sb.Append("<tr><td class=\"amount\">" + meal.Qty + "</td><td>" + meal.ItemDesc + "</td><td class=\"amount\">" + meal.TotalCost.ToString("N0") + "</td></tr>");
                total += meal.TotalCost;
            }

            sb.Append("<tr><th/><th class=\"amount\">TOTAL VENDIDO:</th><th class=\"amount\">" + total.ToString("N0") + "</th></tr>");
            sb.Append("</table></body></html>");
        }
        public static bool SendAccountsReceivableSummary(string dateProc)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                List<clsDelincuency> delincuenciesList = DB.GetDelincuencies("202%");

                string subject = Settings.Default.BusinessName + " - Resumen de Cuentas por Cobrar al " + DB.ConverTicketDate(dateProc);

                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: center; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".center {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");
                sb.Append("<h2>CUENTAS POR COBRAR</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>ID</th><th>NOMBRE DEL CLIENTE</th><th>1 a 8d</th><th>9 a 15d</th><th>16 a 30d</th><th>31 a 45d</th><th>46 a 60d</th><th>Más de 60d</th></tr>");

                int sum_0_8_days = 0;
                int sum_9_15_days = 0;
                int sum_16_30_days = 0;
                int sum_31_45_days = 0;
                int sum_46_60_days = 0;
                int sum_61_days = 0;
                int grandTotal = 0;

                foreach (clsDelincuency delincuent in delincuenciesList)
                {
                    sb.Append("<tr><td class=\"center\">" + delincuent.ID +
                              "</td><td>" + delincuent.CustomerName +
                              "</td><td class=\"amount\">" + delincuent.sum_0_8_days.ToString("N0") +
                              "</td><td class=\"amount\">" + delincuent.sum_9_15_days.ToString("N0") +
                              "</td><td class=\"amount\">" + delincuent.sum_16_30_days.ToString("N0") +
                              "</td><td class=\"amount\">" + delincuent.sum_31_45_days.ToString("N0") +
                              "</td><td class=\"amount\">" + delincuent.sum_46_60_days.ToString("N0") +
                              "</td><td class=\"amount\">" + delincuent.sum_61_days.ToString("N0") +
                              "</td></tr>");

                    sum_0_8_days += delincuent.sum_0_8_days;
                    sum_9_15_days += delincuent.sum_9_15_days;
                    sum_16_30_days += delincuent.sum_16_30_days;
                    sum_31_45_days += delincuent.sum_31_45_days;
                    sum_46_60_days += delincuent.sum_46_60_days;
                    sum_61_days = delincuent.sum_61_days;
                }

                sb.Append("<tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");

                sb.Append("<tr><td></td><th>SUB-TOTALES" +
                          "</td><td class=\"amount\">" + sum_0_8_days.ToString("N0") +
                          "</td><td class=\"amount\">" + sum_9_15_days.ToString("N0") +
                          "</td><td class=\"amount\">" + sum_16_30_days.ToString("N0") +
                          "</td><td class=\"amount\">" + sum_31_45_days.ToString("N0") +
                          "</td><td class=\"amount\">" + sum_46_60_days.ToString("N0") +
                          "</td><td class=\"amount\">" + sum_61_days.ToString("N0") +
                          "</td></tr>");

                grandTotal = sum_0_8_days + sum_9_15_days + sum_16_30_days + sum_31_45_days + sum_46_60_days + sum_61_days;

                sb.Append("</table><h2>GRAN TOTAL: " + grandTotal.ToString("N0") + "</h2></body></html>");

                string body = sb.ToString();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Bcc.Add(emailFromAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static bool SendAWCDigitalCommerceBackup(string backupPath)
        {
            try
            {
                if (CheckInternetConnection())
                {
                    Thread t = new Thread(() => SendBackup(backupPath));
                    t.Start();
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "AWCDigitalCommerce Database Backup Email Threat started successfully.", Logger.Severity.INFORMATION);
                }
                else
                {
                    Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "AWCDigitalCommerce Database Backup Email NOT SENT, Internet Connection NOT available.", Logger.Severity.INFORMATION);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
                return false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        private static void SendBackup(string backupPath)
        {
            try
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "AWCDigitalCommerce Database Backup Email Threat started.", Logger.Severity.INFORMATION);

                string subject = Settings.Default.BusinessName + " - AWCDigitalCommerce Database Backup for " + DateTime.Now.ToString("dd.MM.yyyy");

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailFromAddress);
                    mail.Subject = subject;
                    mail.Body = subject;
                    mail.IsBodyHtml = false;

                    Attachment attachment;
                    attachment = new Attachment(backupPath);
                    mail.Attachments.Add(attachment);

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, "AWCDigitalCommerce Database Backup sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        public static void EMailTicket(clsTicketsForDataGrid ticket, string mailAddress)
        {
            try
            {
                string subject = Settings.Default.BusinessName + " - CUENTA " + ticket.ID.ToString();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(mailAddress);
                    mail.Subject = subject;
                    mail.Body = "Estimado cliente, atendiendo su solicitud sírvase encontrar adjunto copia electrónica de su cuenta número " + ticket.ID.ToString();
                    mail.IsBodyHtml = false;

                    Attachment attachment;
                    string electronicTicket = Path.Combine(Settings.Default.SerilogRootPath, ticket.ID.ToString("000000") + ".bmp");
                    attachment = new Attachment(electronicTicket);
                    mail.Attachments.Add(attachment);

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }

                    File.Delete(electronicTicket);
                }
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"AWCDigitalCommerce Ticket {ticket.ID} sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        public static void EMailInventory(List<clsItem> itemsList)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");
                //
                // INVENTORY LIST
                //
                sb.Append("<h2>LISTA DE PRODUCTOS</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN</th><th>EXISTENCIA</th><th>COSTO</th><th>PRECIO</th></tr>");

                foreach (clsItem item in itemsList)
                {
                    sb.Append("<tr><td class=\"ItemID\">" + item.ID +
                              "</td><td>" + item.ItemDescription +
                              "</td><td class=\"amount\">" + item.ItemAvailable.ToString("N0") +
                              "</td><td class=\"amount\">" + item.UnitCost.ToString("N0") +
                              "</td><td class=\"amount\">" + item.UnitPrice.ToString("N0") + "</td></tr>");
                }

                sb.Append("</table>");
                sb.Append("<p><br></p>");

                string subject = Settings.Default.BusinessName + " - ESTADO DEL INVENTARIO AL " + DateTime.Now.ToString("dd-MM-yyyy");

                string body = sb.ToString();

                SendEMail(subject, body);

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Inventory sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static void SendReportByEMail(int type, string emailAddress, string sd, string ed)
        {
            try
            {
                string subject = string.Empty;
                string body = string.Empty;

                List<clsItem> itemsList = new List<clsItem>();
                List<clsCustomerVIP> customersList = new List<clsCustomerVIP>();
                List<clsDailyClosing> dcsList = new List<clsDailyClosing>();
                List<clsTicketsForDataGrid> ticketsList = new List<clsTicketsForDataGrid>();
                List<clsTicket> ticketsAbortedList = new List<clsTicket>();

                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {right-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");

                switch(type)
                {
                    case 0:
                        #region PRODUCT LIST
                        itemsList = DB.ListBinding_tbl_Items(4);

                        sb.Append("<h2>LISTA DE BEBIDAS Y LICORES</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN</th><th>PRECIO</th><th>COSTO</th></tr>");

                        foreach (clsItem item in itemsList)
                        {
                            sb.Append("<tr><td class=\"ItemID\">" + item.ID + "</td><td>" + item.ItemDescription + "</td><td class=\"amount\">" + item.UnitPrice.ToString("N0") + "</td><td class=\"amount\">" + item.UnitCost.ToString("N0") + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - LISTA DE BEBIDAS Y LICORES AL " + DateTime.Now.ToString("dd-MM-yyyy");

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Product List sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 1:
                        #region INVENTORY
                        // Beverages
                        itemsList = DB.ListBinding_tbl_Items(1);
                        
                        sb.Append("<h2>BEBIDAS</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN</th><th>EXISTENCIA</th><th>COSTO UNIDAD</th><th>TOTAL COSTO</th><th>PRECIO UNIDAD</th><th>TOTAL PRECIO</th></tr>");

                        foreach (clsItem item in itemsList)
                        {
                            sb.Append("<tr><td class=\"ItemID\">" + item.ID + "</td><td>" +
                                                                    item.ItemDescription + "</td><td class=\"amount\">" +
                                                                    item.ItemAvailable.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    item.UnitCost.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    (item.UnitCost * item.ItemAvailable).ToString("N0") + "</td><td class=\"amount\">" +
                                                                    item.UnitPrice.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    (item.UnitPrice * item.ItemAvailable).ToString("N0") + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        // Liquors
                        itemsList = DB.ListBinding_tbl_Items(2);

                        sb.Append("<h2>LICORES</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>ID</th><th>DESCRIPCIÓN</th><th>EXISTENCIA</th><th>COSTO UNIDAD</th><th>TOTAL COSTO</th><th>PRECIO UNIDAD</th><th>TOTAL PRECIO</th></tr>");

                        foreach (clsItem item in itemsList)
                        {
                            sb.Append("<tr><td class=\"ItemID\">" + item.ID + "</td><td>" +
                                                                    item.ItemDescription + "</td><td class=\"amount\">" +
                                                                    item.ItemAvailable.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    item.UnitCost.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    (item.UnitCost * item.ItemAvailable).ToString("N0") + "</td><td class=\"amount\">" +
                                                                    item.UnitPrice.ToString("N0") + "</td><td class=\"amount\">" +
                                                                    (item.UnitPrice * item.ItemAvailable).ToString("N0") + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - ESTADO DEL INVENTARIO AL " + DateTime.Now.ToString("dd-MM-yyyy");

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Inventory sent by email successfully.", Logger.Severity.INFORMATION);

                        #endregion
                        break;
                    case 2:
                        #region FREQUENT CUSTOMERS
                        customersList = DB.ListBinding_tbl_CustomerID(4, 0);

                        sb.Append("<h2>LISTA DE CLIENTES FRECUENTES</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>ID</th><th>NOMBRE</th><th>ÚLTIMA VISITA</th></tr>");

                        foreach (clsCustomerVIP cust in customersList)
                        {
                            sb.Append("<tr><td class=\"ItemID\">" + cust.ID + "</td><td>" + cust.CustomerID + "</td><td>" + cust.LastPayment + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - LISTA DE CLIENTES FRECUENTES AL " + DateTime.Now.ToString("dd-MM-yyyy");

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Frequent Customers List sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 3:
                        #region DAILY CLOSING SUMMARIES
                        dcsList = DB.GetDailyClosingSummary(sd, ed);

                        sb.Append("<h2>CIERRES DIARIOS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed) + "</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>FECHA</th><th>SUPERVISOR</th><th>FECHA/HORA DEL CIERRE</th><th>EFECTIVO INICIAL</th><th>EFECTIVO SISTEMA</th><th>EFECTIVO SUPERVISOR</th><th>TARJ CRED SISTEMA</th><th>TARJ CRED SUPERVISOR</th><th>SINPE SISTEMA</th><th>SINPE SUPERVISOR</th><th>PAGOS CXC</th><th>10% SERVICIO</th><th>VENTA BRUTA</th><th>VENTA NETA</th><th>TOTAL EFECTIVO EN CAJA</th><th>CIERRE CUADRADO</th></tr>");

                        foreach (clsDailyClosing dcs in dcsList)
                        {
                            clsUser userProfile = DB.CheckUserPIN(dcs.WhoDidIt);

                            sb.Append("<tr><td>" + dcs.BusinessDate +
                                     "</td><td>" + userProfile.userName +
                                     "</td><td class=\"amount\">" + dcs.CreatedAt.ToString() +
                                     "</td><td class=\"amount\">" + dcs.InitialCash +
                                     "</td><td class=\"amount\">" + dcs.Cash +
                                     "</td><td class=\"amount\">" + dcs.CashByOperator +
                                     "</td><td class=\"amount\">" + dcs.CreditCard +
                                     "</td><td class=\"amount\">" + dcs.CreditCardByOperator +
                                     "</td><td class=\"amount\">" + dcs.Transfer +
                                     "</td><td class=\"amount\">" + dcs.TransferByOperator +
                                     "</td><td class=\"amount\">" + dcs.AccountsReceivable +
                                     "</td><td class=\"amount\">" + dcs.ServiceFee +
                                     "</td><td class=\"amount\">" + dcs.GrossSale +
                                     "</td><td class=\"amount\">" + dcs.NetSale +
                                     "</td><td class=\"amount\">" + dcs.TotalCashInDrawer +
                                     "</td><td>" + (dcs.DailyClosingMatch ? "SI" : "NO") + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - CIERRES DIARIOS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Daily Closing Summaries List sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 4:
                        #region DAILY TICKETS
                        ticketsList = DB.DataBinding_tbl_DailyClose(sd, ed);

                        sb.Append("<h2>LISTA DE CUENTAS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed) + "</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>FECHA</th><th>CUENTA</th><th>NOMBRE DEL CLIENTE</th><th>MONTO TOTAL</th><th>10% SERVICIO</th><th>EFECTIVO</th><th>TARJ CRED</th><th>SINPE</th><th>ESTADO</th><th>TIPO PAGO</th></tr>");

                        foreach (clsTicketsForDataGrid tck in ticketsList)
                        {
                            sb.Append("<tr><td>" + tck.TicketDate +
                                     "</td><td class=\"ItemID\">" + tck.ID +
                                     "</td><td>" + tck.CustomerID +
                                     "</td><td class=\"amount\">" + tck.TotalPrice +
                                     "</td><td class=\"amount\">" + tck.ServiceFee +
                                     "</td><td class=\"amount\">" + tck.Cash +
                                     "</td><td class=\"amount\">" + tck.CreditCard +
                                     "</td><td class=\"amount\">" + tck.Transfer +
                                     "</td><td>" + tck.StatusAlpha +
                                     "</td><td>" + tck.PayMethodAlpha + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - LISTA DE CUENTAS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Daily Tickets List sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 5:
                        #region ABORTED TICKETS
                        ticketsAbortedList = DB.ListBinding_tbl_TicketsAborted(sd, ed);

                        sb.Append("<h2>LISTA DE CUNETAS ABORTDAS</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

                        foreach (clsTicket abtck in ticketsAbortedList)
                        {
                            sb.Append("<tr><td>" + abtck.TicketDate +
                                      "</td><td class=\"ItemID\">" + abtck.ID +
                                      "</td><td>" + abtck.CustomerAKA +
                                      "</td><td class=\"ItemID\">" + abtck.TotalPrice +
                                      "</td><td>" + abtck.AbortReason + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - LISTA DE CUENTAS ABORTADAS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);

                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"List of Aborted Accounts sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 6:
                        #region CONSUPTIONS

                        sb.Append("<h2>PRODUCTOS CONSUMIDOS</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

                        productsList = DB.GetItemsByDate(sd, ed, 4);

                        foreach (clsItemDetailForDatagrid prod in productsList)
                        {
                            sb.Append("<tr><td class=\"ItemID\">" + prod.Qty + "</td>" +
                                     "<td>" + prod.ItemDesc + "</td > " +
                                     "<td class=\"amount\">" + prod.TotalPrice.ToString("N0") + "</td></tr>");
                        }

                        int totPrice = productsList.Sum(x => x.TotalPrice);

                        sb.Append("<tr><td></td><td><b>TOTAL:</b></td><td class=\"amount\"><b>" + totPrice.ToString("N0") + "</b></td></tr>");
                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = Settings.Default.BusinessName + " - PRODUCTOS CONSUMIDOS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);
                        Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"List of Consumptions sent by email successfully.", Logger.Severity.INFORMATION);
                        #endregion
                        break;
                    case 7:
                        #region TIMECARDS
                        List<clsTimeCard> timeCardList = DB.GetTimeCards(sd, ed);

                        sb.Append("<h2>LISTADO DE INGRESOS Y SALIDAS DE EMPLEADOS</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>FECHA CONTABLE</th><th>PIN</th><th>EVENTO</th><th>FECHA HORA DEL EVENTO</th></tr>");

                        foreach (clsTimeCard tc in timeCardList)
                        {
                            string eventType = tc.EventType == 1 ? "INGRESO" : "SALIDA";
                            clsUser user = DB.CheckUserPIN(tc.UserPIN.ToString());

                            sb.Append("<tr><td>" + DB.ConverTicketDate(tc.BusinessDate) +
                                      "</td><td class=\"ItemID\">" + user.userName +
                                      "</td><td>" + eventType +
                                      "</td><td>" + tc.EventDatetime.ToString() + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = "LISTADO DE INGRESOS Y SALIDAS DE EMPLEADOS DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);
                        #endregion
                        break;
                    case 8:
                        #region CashierBoxOpen
                        List<clsOpenCashDrawer> openCashDrawerEventList = DB.GetOpenCashDrawer(sd, ed);

                        sb.Append("<h2>APERTURA MANUAL DEL CAJÓN DE DINERO (SIN FACTURA)</h2>");
                        sb.Append("<table>");
                        sb.Append("<tr><th>COLABORADOR</th><th>FECHA/HORA DEL EVENTO</th></tr>");

                        foreach (clsOpenCashDrawer tc in openCashDrawerEventList)
                        {
                            clsUser user = DB.CheckUserPIN(tc.WhoDitIt);

                            sb.Append("<tr><td class=\"ItemID\">" + user.userName + "</td><td>" + tc.EventDateTime.ToString() + "</td></tr>");
                        }

                        sb.Append("</table>");
                        sb.Append("<p><br></p>");

                        subject = "APERTURA MANUAL DEL CAJÓN DE DINERO (SIN FACTURA) DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                        body = sb.ToString();

                        SendEMail(subject, body, emailAddress);
                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static void SendInternalOrderByEMail(string emailAddress, string fileName)
        {
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");

                subject = Settings.Default.BusinessName + " - COMPROBANTE DE PEDIDO " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");

                using (StreamReader sr = new StreamReader(fileName))
                {
                    bool firstRec = true;

                    while (!sr.EndOfStream)
                    {
                        string rec = sr.ReadLine();

                        if (firstRec)
                        {
                            clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());

                            sb.Append($"<h2>{subject}</h2>");
                            sb.Append($"<h2>PROVEEDOR: {rec}</h2>");
                            sb.Append($"<h2>SOLICITADO POR: {userProf.userName}</h2>");
                            sb.Append("<table>");
                            sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN DEL PRODUCTO</th></tr>");

                            firstRec = false;
                            continue;
                        }
                        else
                        {
                            sb.Append($"<tr><td>{rec.Split(',')[1]}</td><td>{rec.Split(',')[0]}</td></tr>");
                        }
                    }
                }

                sb.Append("</table>");
                sb.Append("<p><br></p>");

                body = sb.ToString();

                SendEMail(subject, body, emailAddress);

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Internal Order sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static void SendBusinessDateChangeAlertByEMail()
        {
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                StringBuilder sb = new StringBuilder();

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");

                subject = Settings.Default.BusinessName + " - CAMBIO DE FECHA CONTABLE";

                clsUser user = DB.CheckUserPIN(Settings.Default.WhoOpen.ToString());
                sb.Append($"<p>Sirva este correo para comunicarle que el colaborador {user.userName} realizó cambio de fecha contable a {DB.ConverTicketDate(Settings.Default.BusinessDate)}</p>");
                sb.Append("<p><br></p>");
                sb.Append("</body></html>");

                body = sb.ToString();

                SendEMail(subject, body, Settings.Default.eMailDistributionList);

                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"Internal Order sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        public static void SendAlert2AdminAboutVIP(string customerid, List<clsTicketsForDataGrid> custOpenTcks)
        {
            try
            {
                string subject = string.Empty;
                string body = string.Empty;
                StringBuilder sb = new StringBuilder();

                clsCustomerVIP custVIP = DB.GetCustomerProfile(customerid);
 
                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");

                subject = $"{custVIP.CustomerID} - ABRIENDO CUENTA NUEVA";

                sb.Append($"<p>ALERTA PARA EL ADMINISTRADOR: Este cliente esta solicitando abrir una cuenta nueva, pero ya tiene {custOpenTcks.Count} facturas pendientes por un monto total de {custOpenTcks.Sum(x => x.TotalPrice).ToString("N0")} colones.</p>");
                sb.Append("</body></html>");
                sb.Append("<table>");
                sb.Append("<tr><th>FECHA</th><th>CUENTA</th><th>TOTAL</th></tr>");

                foreach (clsTicketsForDataGrid item in custOpenTcks)
                {
                    sb.Append("<tr><td>" + item.TicketDate + "</td><td>" + item.ID + "</td><td class=\"amount\">" + item.TotalPrice.ToString("N0") + "</td></tr>");
                }

                sb.Append("</table>");
                sb.Append("<p><br></p>");

                body = sb.ToString();
                SendEMail(subject, body, Settings.Default.eMailDistributionList);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
        }
        public static void SendEmailWithComsuptions(List<clsItemDetailForDatagrid> prodList, int type, string emailAddress, string sd, string ed)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                string header = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                StringBuilder sb = new StringBuilder();

                switch (type)
                {
                    case 1:
                        header = "BEBIDAS";
                        break;
                    case 2:
                        header = "LICORES";
                        break;
                    case 3:
                        header = "COMIDAS";
                        break;
                    case 4:
                        header = "TODOS";
                        break;
                }

                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head><style>");
                sb.Append("table {font-family: arial, sans-serif; border-collapse: collapse; width: 50 %;}");
                sb.Append("th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("td {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                sb.Append("tr:nth-child(even) {background-color: #808080;}");
                sb.Append(".ItemID {text-align : center;}");
                sb.Append(".amount {text-align : right;}");
                sb.Append("</style></head>");
                sb.Append("<body>");

                sb.Append("<h2>PRODUCTOS CONSUMIDOS - " + header + "</h2>");
                sb.Append("<table>");
                sb.Append("<tr><th>CANT</th><th>DESCRIPCIÓN</th><th>MONTO</th></tr>");

                foreach (clsItemDetailForDatagrid prod in prodList)
                {
                    sb.Append("<tr><td class=\"ItemID\">" + prod.Qty + "</td>" +
                             "<td>" + prod.ItemDesc + "</td > " +
                             "<td class=\"amount\">" + prod.TotalPrice.ToString("N0") + "</td></tr>");
                }

                int totPrice = prodList.Sum(x => x.TotalPrice);

                sb.Append("<tr><td></td><td><b>TOTAL:</b></td><td class=\"amount\"><b>" + totPrice.ToString("N0") + "</b></td></tr>");
                sb.Append("</table>");
                sb.Append("<p><br></p>");

                subject = Settings.Default.BusinessName + " - " + header + " DEL " + DB.ConverTicketDate(sd) + " AL " + DB.ConverTicketDate(ed);

                body = sb.ToString();

                SendEMail(subject, body, emailAddress);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, $"List of Consumptions sent by email successfully.", Logger.Severity.INFORMATION);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex.Message, Logger.Severity.ERROR);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
