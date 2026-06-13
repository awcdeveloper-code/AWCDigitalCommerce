using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterBeveragesOrder
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string custDesc = string.Empty;
        private List<string> beveragesList = new List<string>();

        public xPrinterBeveragesOrder()
        {

        }

        public xPrinterBeveragesOrder(string _custDesc, List<string> _beveragesList)
        {
            custDesc = _custDesc;
            beveragesList = _beveragesList;
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

            workVar = "ORDEN DE BEBIDAS";
            graphics.DrawString(new string(' ', 16 - (workVar.Length / 2)) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(new string(' ', 18 - (Settings.Default.WorkStationType.Length / 2)) + Settings.Default.WorkStationType, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            if (custDesc.Length > 22)
                custDesc = custDesc.Substring(0, 22);

            graphics.DrawString(new string(' ', 18 - (custDesc.Length / 2)) + custDesc, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt"), new Font("Consolas", 11), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());
            workVar = $"COLABORADOR: {userProf.userName}";
            graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            // beverages list

            foreach (string beverage in beveragesList)
            {
                workVar = Helper.FormatItemDetailLine(Convert.ToInt32(beverage.Split('|')[0]), beverage.Split('|')[1]);

                graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;
            }

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 30;

            graphics.DrawString(new string('.', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
