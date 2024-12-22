using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;
using AWC.DigitalCommerce.TicketsController.Classes;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterVoucher
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private clsVoucher _voucher = null;

        public xPrinterVoucher()
        {

        }

        public xPrinterVoucher(clsVoucher voucher)
        {
            _voucher = voucher;
        }

        public void print()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
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

                Pen myPen = new Pen(Color.Black);
                myPen.Width = 2;

                Image img = Image.FromFile(Settings.Default.BusinessLogo);

                int LogoWidth = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[0].Trim());
                int LogoHeigh = Convert.ToInt32(Settings.Default.TicketHeaderWH.Split(',')[1].Trim());

                graphics.DrawImage(img, new Rectangle(0, 0, LogoWidth, LogoHeigh), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                Offset += LogoHeigh + 30;

                workVar = "VOUCHER DE CORTESÍA";
                graphics.DrawString(workVar, new Font("Consolas Bold", 11), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                graphics.DrawString(new string(' ', 6 - (_voucher.Amount.ToString().Length / 2)) + _voucher.Amount.ToString(), new Font("Consolas Bold", 28), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 60;

                workVar = "FECHA DE EXPIRACIÓN";
                graphics.DrawString(workVar, new Font("Consolas Bold", 11), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString(new string(' ',12) + _voucher.ExpireAt.ToString("dd.MM.yyyy"), new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 40;

                clsUser userProfile = DB.CheckUserPIN(_voucher.IssueBy);
                graphics.DrawString(Helper.FormatGralLine(userProfile.userName), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = $"VOUCHER {_voucher.ID}";
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = $"FECHA DE EMISIÓN: {_voucher.CreatedAt.ToString("dd.MM.yyyy")}";
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                workVar = "* GRACIAS POR PREFERINOS *";
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 50;

                // AIDAware Banner
                //StringFormat drawFormat = new System.Drawing.StringFormat();
                //graphics.DrawString(Settings.Default.AIDAwareBanner, new Font("Tahoma", 16), new SolidBrush(Color.LightGray), startX, startY + Offset, drawFormat);

                // Cut line
                workVar = ".   .    .    .    .    .    .";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.WriteToLog(Constants.Titles.SHORTGAPPTITLE, ex, Logger.Severity.ERROR);
            }
        }
    }
}
