using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrintServiceFeeByWho
    {
        private string startDay = string.Empty;
        private string finalDay = string.Empty;
        private string workVar = string.Empty;
        List<clsServiceFeeByWho> serviceFeeByWhoList = new List<clsServiceFeeByWho>();
        private PrintDocument pdoc = null;

        public xPrintServiceFeeByWho()
        {

        }

        public xPrintServiceFeeByWho(string _startDay, string _finalDay, List<clsServiceFeeByWho> _serviceFeeByWhoList)
        {
            startDay = _startDay;
            finalDay = _finalDay;
            serviceFeeByWhoList = _serviceFeeByWhoList;
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
                graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessID), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString(Helper.FormatGralLine(Settings.Default.BusinessPhoneNumber), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                graphics.DrawString("ASIGNACIÓN DEL 10% DE SERVICIO", new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                workVar = DB.ConverTicketDate(startDay) + " AL " + DB.ConverTicketDate(finalDay);
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 30;

                workVar = "COLABORADOR" + new string(' ', 14) + "TOTAL";
                graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 15;

                //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                //Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                foreach (clsServiceFeeByWho sfbw in serviceFeeByWhoList)
                {
                    workVar = Helper.FormatGralLine(sfbw.UserName, 24) + " " + sfbw.TotalServiceFee.ToString("N0").PadLeft(5);
                    graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 18;
                }

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                string tot = serviceFeeByWhoList.Sum(x => x.TotalServiceFee).ToString("N0");
                graphics.DrawString(new string(' ', 10 - (tot.Length / 2)) + tot, new Font("Consolas Bold", 20), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 70;

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
