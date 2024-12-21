using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterDailyClose
    {
        private string workVar = string.Empty;
        private string workVar2 = string.Empty;
        private PrintDocument pdoc = null;
        private string workDay = string.Empty;
        private List<clsTicketsForDataGrid> itemsList;

        public xPrinterDailyClose()
        {

        }

        public xPrinterDailyClose(string _workDay, List<clsTicketsForDataGrid> _itemsList)
        {
            workDay = _workDay;
            itemsList = _itemsList;
        }

        public void print()
        {
            if (Settings.Default.TicketPrinter.Length == 0) return;

            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();

            PrinterSettings ps = new PrinterSettings();
            PaperSize psize = new PaperSize("Custom", Settings.Default.TicketWidth, Settings.Default.TicketLength);

            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdoc.DefaultPageSettings.PaperSize.Width = Settings.Default.TicketWidth;
            pdoc.DefaultPageSettings.PaperSize.Height = Settings.Default.TicketLength;
            pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.TicketPrinter;

            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
            pdoc.Print();
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;

            int startX = 0;
            int startY = 0;
            int Offset = 0;

            // PRINT LOGO
            Image img = Image.FromFile(Settings.Default.BusinessLogo);

            int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
            int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

            Pen myPen = new Pen(Color.Black);
            myPen.Width = 2;

            graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            Offset += LogoHeigh;

            // AIDAware Banner
            //StringFormat drawFormat = new System.Drawing.StringFormat();
            //drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            //graphics.DrawString(Settings.Default.AIDAwareBanner, new Font("Tahoma", 16), new SolidBrush(Color.LightGray), startX, startY + Offset, drawFormat);

            workVar = new string(' ', 10 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
            graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 35;

            // TICKET HEADER
            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            workDay = DB.ConverTicketDate(workDay);
            graphics.DrawString(new string(' ', 4) + "CIERRE: " + workDay, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            // TICKETS HEADER
            workVar = " FACT  FORMA DE PAGO     TOTAL";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 10;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            int efectivo = 0;
            int tarjCred = 0;
            int transSinpe = 0;
            int voucher = 0;
            int pendiente = 0;
            double tot = 0;
            double totExpenses = 0;
            int totLunch = 0;
            int totServiceFee = 0;

            // LIST OF TICKETS
            foreach (clsTicketsForDataGrid ticket in itemsList)
            {
                if (string.IsNullOrEmpty(ticket.CustomerID)) continue;

                if (ticket.CustomerID.Contains("ABONO")) continue;

                string ticketNum = string.Empty;
                string payMethod = string.Empty;
                string total = string.Empty;
                bool paymentMixed = false;

                ticketNum = ticket.ID.ToString("000000");

                switch (ticket.PayMethod)
                {
                    case 0:
                        pendiente += ticket.TotalPrice;
                        payMethod = "POR COBRAR" + new string(' ', 6);
                        break;
                    case 1:
                        if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                            payMethod = "NO REQUIERE PAGO";
                        if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer == 0)
                            payMethod = "EFECTIVO";
                        if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer == 0)
                            payMethod = "TARJ CRED";
                        if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer > 0)
                            payMethod = "TRANS SINPE";
                        if (ticket.Cash == 0 && ticket.CreditCard == 0 && ticket.Transfer == 0 && ticket.Voucher > 0)
                            payMethod = "VOUCHER";
                        // MULTI
                        if ((ticket.Cash < 0 || ticket.Cash > 0) && ticket.CreditCard > 0 && ticket.Transfer == 0 && ticket.Voucher == 0)
                        {
                            payMethod = "EFEC+TARJ";
                            paymentMixed = true;
                        }
                        if (ticket.Cash > 0 && ticket.CreditCard == 0 && ticket.Transfer > 0 && ticket.Voucher == 0)
                        {
                            payMethod = "EFEC+TRAN";
                            paymentMixed = true;
                        }
                        if (ticket.Cash == 0 && ticket.CreditCard > 0 && ticket.Transfer > 0 && ticket.Voucher == 0)
                        {
                            payMethod = "TARJ+TRAN";
                            paymentMixed = true;
                        }
                        if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0 && ticket.Voucher == 0)
                        {
                            payMethod = "EFEC+TARJ+TRAN";
                            paymentMixed = true;
                        }

                        if (ticket.Cash > 0 && ticket.CreditCard > 0 && ticket.Transfer > 0 && ticket.Voucher > 0)
                        {
                            payMethod = "EFEC+TARJ+TRAN+VOU";
                            paymentMixed = true;
                        }

                        payMethod = payMethod + new string(' ', 16 - payMethod.Length);

                        efectivo += ticket.Cash;
                        tarjCred += ticket.CreditCard;
                        transSinpe += ticket.Transfer;
                        voucher += ticket.Voucher;
                        break;
                    case 2:
                        payMethod = "ANULADA" + new string(' ', 9);
                        break;
                    default:
                        payMethod = "PAGO INVÁLIDO" + new string(' ', 3); ;
                        break;
                }

                total = ticket.TotalPrice.ToString("N0");
                workVar = ticketNum + " " + payMethod + total.PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                if(Settings.Default.PrintFullDetailInDailyClose)
                {
                    if (ticket.CustomerAKA?.Length > 0)
                        workVar = ticket.CustomerAKA;
                    else
                        workVar = ticket.CustomerID;

                    workVar = new string(' ', 10) + workVar;
                    graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;

                    if (paymentMixed)
                    {
                        string payAmt = string.Empty;

                        if (ticket.Cash < 0 || ticket.Cash > 0)
                        {
                            payAmt = ticket.Cash.ToString("N0");
                            workVar = new string(' ', 17) + "EFEC: " + payAmt.PadLeft(7);
                            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                            Offset += 18;
                        }

                        if (ticket.CreditCard > 0)
                        {
                            payAmt = ticket.CreditCard.ToString("N0");
                            workVar = new string(' ', 17) + "TARJ: " + payAmt.PadLeft(7);
                            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                            Offset += 18;
                        }

                        if (ticket.Transfer > 0)
                        {
                            payAmt = ticket.Transfer.ToString("N0");
                            workVar = new string(' ', 16) + "TRANS: " + payAmt.PadLeft(7);
                            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                            Offset += 18;
                        }

                        if (ticket.Voucher > 0)
                        {
                            payAmt = ticket.Voucher.ToString("N0");
                            workVar = new string(' ', 14) + "VOUCHER: " + payAmt.PadLeft(7);
                            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                            Offset += 18;
                        }
                    }

                    if (ticket.ServiceFee > 0)
                    {
                        totServiceFee += ticket.ServiceFee;
                        total = ticket.ServiceFee.ToString("N0");
                        workVar = new string(' ', 12) + "10 % SERV: " + total.PadLeft(7);
                        graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 18;
                    }
                }
            }

            // EXPENSES
            List<clsExpense> expensesList = DB.GetExpenses(Helper.RevertFormatDate(workDay));

            if (expensesList.Count > 0)
            {
                // EXPENSES HEADER
                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = new string(' ', 5) + "GASTOS VARIOS";
                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                foreach (clsExpense expense in expensesList)
                {
                    totExpenses += expense.ExpenseAmount;
                    workVar = Helper.FormatExpenseLine(expense);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }
            }

            // EMPLOYEES LUNCH
            List<clsLunch> lunchesList = DB.GetLunches(Helper.RevertFormatDate(workDay));

            if (lunchesList.Count > 0)
            {
                // LUNCHES HEADER
                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = new string(' ', 8) + "ALMUERZOS";
                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                foreach (clsLunch lunch in lunchesList)
                {
                    clsItem item = DB.GetItem(lunch.MealID);

                    totLunch += item.UnitPrice * lunch.Qty;

                    string lunchLine = lunch.Qty.ToString() + "|" + item.ItemDescription + "|" + (item.UnitPrice * lunch.Qty).ToString();

                    workVar = Helper.FormatLunchLine(lunchLine);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 15;

                    graphics.DrawString(new string(' ', 4) + lunch.EmployeeName, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }
            }

            // SMALL PAYMENTS
            List<clsSmallPayment> smlPayList = DB.GetSmallPayments(Helper.RevertFormatDate(workDay));

            if (smlPayList.Count > 0)
            {
                // LUNCHES HEADER
                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = new string(' ', 9) + "ABONOS";
                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                workVar = "CLIENTE FRECUENTE       CUENTA";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 10;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                foreach (clsSmallPayment smlPay in smlPayList)
                {
                    // CUSTOMER NAME & TICKET
                    workVar = Helper.FormatSmallPaymentLine(DB.GetCustomerIDByID(smlPay.CustomerID), smlPay.TicketID);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;

                    if (smlPay.Cash > 0)
                    {
                        // CASH
                        workVar = new string(' ', 13) + "EFECTIVO: " + smlPay.Cash.ToString("N0").PadLeft(7);
                        graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 18;
                    }

                    if (smlPay.CreditCard > 0)
                    {
                        // CREDIT CARD
                        workVar = new string(' ', 3) + "TARJETA DE CREDITO: " + smlPay.CreditCard.ToString("N0").PadLeft(7);
                        graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 18;
                    }

                    // TRANSFER
                    if (smlPay.Transfer > 0)
                    {
                        workVar = new string(' ', 8) + "TRANSFERENCIA: " + smlPay.Transfer.ToString("N0").PadLeft(7);
                        graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 18;
                    }

                    efectivo += smlPay.Cash;
                    tarjCred += smlPay.CreditCard;
                    transSinpe += smlPay.Transfer;
                }
            }

            // FOOTER
            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            string cro = Settings.Default.CashRegisterOpening.ToString("N0");
            workVar = new string(' ', 3) + "CAJA INICIAL (I): " + cro.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = pendiente.ToString("N0");
            workVar = new string(' ', 5) + "POR COBRAR (P): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = efectivo.ToString("N0");
            workVar = new string(' ', 7) + "EFECTIVO (E): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = tarjCred.ToString("N0");
            workVar = new string(' ', 6) + "TARJ CRED (C): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = transSinpe.ToString("N0");
            workVar = new string(' ', 4) + "TRANS SINPE (S): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = (-1 * totLunch).ToString("N0");
            workVar = new string(' ', 6) + "ALMUERZOS (A): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = (-1 * totExpenses).ToString("N0");
            workVar = new string(' ', 2) + "GASTOS VARIOS (G): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = totServiceFee.ToString("N0");
            workVar2 = new string(' ', 3) + "10% SERVICIO (V): " + workVar.PadLeft(9);
            graphics.DrawString(workVar2, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            tot = pendiente + efectivo + tarjCred + transSinpe;
            workVar2 = tot.ToString("N0");

            workVar = new string(' ', 4) + "TOTAL DE VENTAS: " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 6) + "(P + E + C + S)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            tot = Convert.ToInt32(Settings.Default.CashRegisterOpening) + efectivo + tarjCred + transSinpe - (totExpenses + totLunch + totServiceFee);
            workVar = tot.ToString("N0");

            workVar = new string(' ', 4) + "TOTAL EN CAJA  : " + workVar.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 6) + "(I + E + C + S) - (A + G + V)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 50;

            // Cut line
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
