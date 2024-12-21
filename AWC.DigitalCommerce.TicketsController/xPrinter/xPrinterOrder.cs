using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterOrder
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string custDesc = string.Empty;
        private List<clsTicketDetail> itemsDetails;

        public xPrinterOrder()
        {

        }

        public xPrinterOrder(string _custDesc, List<clsTicketDetail> _itemsDetails)
        {
            custDesc = _custDesc;
            itemsDetails = _itemsDetails;
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

            // TICKET HEADER
            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;
            
            graphics.DrawString(new string(' ', 13 - (custDesc.Length / 2)) + custDesc, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            // LIST OF ITEMS
            foreach (clsTicketDetail ticketDetail in itemsDetails)
            {
                if (DB.IsMealItemType(ticketDetail.ItemDesc)) continue;

                workVar = Helper.FormatItemDetailLine(ticketDetail);
                graphics.DrawString(workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;
            }
            Offset += 2;
            graphics.DrawString(new string('-', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 60;

            // Cut line
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
