using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterInvoicesSummaryByDate
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private List<clsItemDetailForDatagrid> newItemsByDate;
        private string startDate;
        private string endDate;
        public xPrinterInvoicesSummaryByDate(List<clsItemDetailForDatagrid> _newItemsByDate, string _startDate, string _endDate)
        {
            newItemsByDate = _newItemsByDate;
            startDate = _startDate;
            endDate = _endDate;
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
            //Image img = Image.FromFile(Settings.Default.BusinessLogo);

            //int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
            //int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

            //graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            //Offset += LogoHeigh;

            // TICKET HEADER
            graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString(Helper.FormatGralLine("INGRESOS AL INVENTARIO"), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            workVar = $"DEL {DB.ConverTicketDate(startDate)} AL {DB.ConverTicketDate(endDate)}";
            graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 30;

            // ITEMS HEADER
            workVar = "PRODUCTO                  CANT";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            // PRINT ITEMS LIST
            foreach (clsItemDetailForDatagrid item in newItemsByDate)
            {
                workVar = Helper.FormatInvoiceSummaryLine(item);
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset = Offset + 15;
            }
            Offset += 10;
            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 75;
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
