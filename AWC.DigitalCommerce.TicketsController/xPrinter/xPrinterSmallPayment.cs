using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterSmallPayment
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private clsSmallPayment smlPay = new clsSmallPayment();

        public xPrinterSmallPayment()
        {

        }

        public xPrinterSmallPayment(clsSmallPayment _smlPay)
        {
            smlPay = _smlPay;
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
            try
            {
                Graphics graphics = e.Graphics;

                int startX = 0;
                int startY = 0;
                int Offset = 0;

                if (Settings.Default.PrintBusinessLogo)
                {
                    Image img = Image.FromFile(Settings.Default.BusinessLogo);

                    int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                    int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                    graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    Offset += LogoHeigh;
                }

                if (Settings.Default.BusinessName.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessID.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessID), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessPhoneNumber.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessPhoneNumber), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessAddress1.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress1), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                if (Settings.Default.BusinessAddress2.Length > 0)
                {
                    graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessAddress2), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 20;
                }

                graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessPhoneNumber), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                // RECEIPT DATE
                graphics.DrawString(Helper.FormatGralLine($"FECHA: {DB.ConverTicketDate(Settings.Default.BusinessDate)}"), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                // CUSTOMER NAME
                workVar = DB.GetCustomerIDByID(smlPay.CustomerID);
                graphics.DrawString(new string(' ', 23 - workVar.Length) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                // TICKET NUMBER
                workVar = " ABONO A CUENTA: " + smlPay.TicketID.ToString("000000");
                graphics.DrawString(new string(' ', 11 - (workVar.Length / 2)) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                // PAYMENT DETAIL
                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = "       SALDO ACTUAL: " + smlPay.CurTotalPrice.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = "   PAGO EN EFECTIVO: " + smlPay.Cash.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = " TARJETA DE CREDITO: " + smlPay.CreditCard.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = "      TRANSFERENCIA: " + smlPay.Transfer.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = "        NUEVO SALDO: " + smlPay.NewTotalPrice.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                // VERIFICATION CODE
                workVar = "REFERENCIA: " + smlPay.RandomRef.ToString();
                graphics.DrawString(new string(' ', 13 - (workVar.Length / 2)) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 50;

                // Cut line
                workVar = ".   .    .    .    .    .    .";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
