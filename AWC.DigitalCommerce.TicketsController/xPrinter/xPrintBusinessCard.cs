using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;
using static System.Windows.Forms.AxHost;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrintBusinessCard
    {
        private PrintDocument pdoc = null;
        private string workVar = string.Empty;

        public xPrintBusinessCard()
        {

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
                Pen blackPen = new Pen(Color.Black, 4);

                int startX = 0;
                int startY = 0;
                int Offset = 0;

                // PRINT LOGO
                Image img = Image.FromFile("C:\\AWC.DigitalCommerce\\Images\\G3_Logo.png");

                int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                Pen myPen = new Pen(Color.Black);
                myPen.Width = 2;

                graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                Offset += LogoHeigh;

                e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                Offset += 15;

                graphics.DrawString("GUILLERMO E. GRILLO III", new Font("Consolas Bold", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString("TICKETS CONTROLLER OWNER", new Font("Consolas Bold", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString("CEL: 8820-1824", new Font("Consolas Bold", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString("guillermoegrillo@outlook.com", new Font("Consolas Bold", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                Offset += 20;

                graphics.DrawString(".   .    .    .    .    .    .", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
