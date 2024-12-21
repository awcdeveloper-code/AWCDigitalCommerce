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
    public class xPrinterCreditCardPayments
    {
        private string workVar = string.Empty;
        private string workVar2 = string.Empty;
        private PrintDocument pdoc = null;
        private string workDay = string.Empty;
        private List<clsTicket> ticketsList;

        public xPrinterCreditCardPayments()
        {

        }

        public xPrinterCreditCardPayments(string _workDay, List<clsTicket> _ticketsList)
        {
            workDay = _workDay;
            ticketsList = _ticketsList;
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

            int totCreditCard = 0;

            // LIST OF TICKETS
            foreach (clsTicket ticket in ticketsList)
            {
                workVar = ticket.ID.ToString("000000") + "  " + ticket.CreditCard.ToString("N0").PadLeft(7);
                graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;
                totCreditCard += ticket.CreditCard;
            }
            // FOOTER
            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            workVar = totCreditCard.ToString("N0");

            workVar = new string(' ', 4) + "TOTAL: " + workVar.PadLeft(7);
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 50;

            // Cut line
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
