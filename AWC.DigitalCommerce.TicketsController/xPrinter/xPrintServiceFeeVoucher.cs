using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrintServiceFeeVoucher
    {
        private string workDay = string.Empty;
        private string totServiceFee = string.Empty;
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private clsTicketsForDataGrid ticket = new clsTicketsForDataGrid();

        public xPrintServiceFeeVoucher()
        {

        }

        public xPrintServiceFeeVoucher(string _workDay, string _totServiceFee)
        {
            workDay = _workDay;
            totServiceFee = _totServiceFee;
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

                // PRINT LOGO
                Image img = Image.FromFile(Settings.Default.BusinessLogo);

                int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                Pen myPen = new Pen(Color.Black);
                myPen.Width = 2;

                graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                Offset += LogoHeigh;

                // TICKET HEADER
                if (Settings.Default.BusinessName.Length > 0)
                {
                    workVar = new string(' ', 12 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
                    graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 35;
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
                    Offset += 30;
                }

                // DATE
                graphics.DrawString("   " + DB.ConverTicketDate(workDay), new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 40;

                //// SERVICE FEE LOGO
                Image img2 = Image.FromFile(Settings.Default.ServiceFeeLogo);
                graphics.DrawImage(img2, 110 - (img2.Width / 2), startY + Offset);
                Offset += img2.Height;

                // SERVICE FEE
                graphics.DrawString("  10% SERVICIO", new Font("Consolas Bold", 16), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 40;

                graphics.DrawString(new string(' ', 10 - (totServiceFee.Length / 2)) + totServiceFee, new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 75;

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
