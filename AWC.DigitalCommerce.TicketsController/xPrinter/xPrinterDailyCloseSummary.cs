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
    public class xPrinterDailyCloseSummary
    {
        private string workVar = string.Empty;
        private string workVar2 = string.Empty;
        private PrintDocument pdoc = null;
        private string workDay = string.Empty;
        private string workDay1 = string.Empty;
        private string workDay2 = string.Empty;
        private clsDailyClosing dc = null;
        private bool twoDates = false;
        public xPrinterDailyCloseSummary()
        {

        }

        public xPrinterDailyCloseSummary(string _workDay, clsDailyClosing _dc)
        {
            workDay = _workDay;
            dc = _dc;
        }

        public xPrinterDailyCloseSummary(string _workDay1, string _workDay2, clsDailyClosing _dc)
        {
            workDay1 = _workDay1;
            workDay2 = _workDay2;
            dc = _dc;
            twoDates = true;
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
            Pen blackPen = new Pen(Color.Black, 4);

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
            Offset += LogoHeigh + 18;

            //workVar = new string(' ', 10 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
            //graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset += 35;

            // TICKET HEADER
            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            if (twoDates)
            {
                graphics.DrawString(new string(' ', 8) + "CIERRE RESUMEN", new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;
                workDay = DB.ConverTicketDate(workDay1) + " AL " + DB.ConverTicketDate(workDay2);
                graphics.DrawString(new string(' ', 3) + workDay, new Font("Consolas Bold", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            else
            {
                workDay = DB.ConverTicketDate(workDay);
                graphics.DrawString(new string(' ', 4) + $"CIERRE: {workDay}", new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            Offset += 25;

            graphics.DrawString(new string(' ', 14) + $"TURNO {Settings.Default.ShiftForQuery}", new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            string cro = dc.InitialCash.ToString("N0");
            workVar = new string(' ', 3) + "CAJA INICIAL (I): " + cro.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.IncomeCash.ToString("N0");
            workVar = " INGRESOS CAJA (IC): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.Cash.ToString("N0");
            workVar = new string(' ', 7) + "EFECTIVO (E): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.Expenses.ToString("N0");
            workVar = new string(' ', 9) + "GASTOS (G): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            int totCash = (Convert.ToInt32(dc.InitialCash) + dc.Cash + dc.IncomeCash) - (int)dc.Expenses;
            workVar = totCash.ToString("N0");

            workVar = new string(' ', 2) + "TOTAL EN EFECTIVO: " + workVar.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 3) + "(I + IC + E - G)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.AccountsReceivable.ToString("N0");
            workVar = new string(' ', 5) + "POR COBRAR (P): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.CreditCard.ToString("N0");
            workVar = new string(' ', 6) + "TARJ CRED (C): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.Transfer.ToString("N0");
            workVar = new string(' ', 4) + "TRANS SINPE (S): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.Voucher.ToString("N0");
            workVar = new string(' ', 7) + "VOUCHERS (V): " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.GrossSale.ToString("N0");
            workVar = new string(' ', 8) + "VENTA BRUTA: " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 11) + "(P + E + C + S)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.NetSale.ToString("N0");
            workVar = new string(' ', 9) + "VENTA NETA: " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 12) + "(E + C + S + V)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            int tot = Convert.ToInt32(dc.InitialCash) + dc.Cash + dc.IncomeCash + dc.CreditCard + dc.Transfer + dc.Voucher - (dc.ServiceFee + (int)dc.Expenses);
            workVar = tot.ToString("N0");

            workVar = new string(' ', 6) + "TOTAL EN CAJA: " + workVar.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = new string(' ', 8) + "(I + IC + E + C + S + V - G)";
            graphics.DrawString(workVar, new Font("Consolas", 6), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = dc.ServiceFee.ToString("N0");
            workVar2 = new string(' ', 3) + "10% SERVICIO (V): " + workVar.PadLeft(9);
            graphics.DrawString(workVar2, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar2 = dc.OldTicketsPay.ToString("N0");
            workVar = new string(' ', 7) + "PAGOS DE CXC: " + workVar2.PadLeft(9);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            if (dc.CashWithdrawal > 0)
            {
                graphics.DrawString("==EFECTIVO REMANENTE EN CAJA==", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = totCash.ToString("N0");
                workVar = new string(' ', 9) + "DISPONIBLE: " + totCash.ToString("N0").PadLeft(9);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = new string(' ', 13) + "RETIRO: " + dc.CashWithdrawal.ToString("N0").PadLeft(9);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = new string(' ', 10) + "REMANENTE: " + (totCash - dc.CashWithdrawal).ToString("N0").PadLeft(9);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;
            }

            if (dc.Expenses > 0)
            {
                graphics.DrawString("======DETALLE DE GASTOS=======", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                foreach (clsExpense exp in dc.ExpensesList)
                {
                    workVar2 = exp.ExpenseAmount.ToString("N0");

                    if (exp.ExpenseDescription.Length >= 21)
                    {
                        workVar = exp.ExpenseDescription.Substring(0,21) + workVar2.PadLeft(9);
                    }
                    else
                    {
                        workVar = exp.ExpenseDescription + new string(' ', 21 - exp.ExpenseDescription.Length) + workVar2.PadLeft(9);
                    }

                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }
            }

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 30;

            // Cut line
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
