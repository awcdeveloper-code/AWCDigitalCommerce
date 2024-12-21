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
            string tmp = custDesc.Split('|')[0];

            foreach (string beverage in beveragesList)
            {
                if (beverage.Length == 0) continue;

                if (beverage.Split('|')[3].Length > 0)
                {
                    Guid guidID = Guid.NewGuid();

                    string[] bucketContent = beverage.Split('|')[3].Split('$');

                    foreach (string item in bucketContent)
                    {
                        DB.InsertBucketDetail(Convert.ToInt32(tmp.Split('^')[0]), guidID.ToString(), Convert.ToInt32(item.Split(',')[0]), Convert.ToInt32(item.Split(',')[1]));
                    }
                }
            }

            if (Settings.Default.BartenderPrinter.Length == 0) return;

            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();

            PrinterSettings ps = new PrinterSettings();
            PaperSize psize = new PaperSize("Custom", Settings.Default.TicketWidth, Settings.Default.TicketLength);

            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdoc.DefaultPageSettings.PaperSize.Width = Settings.Default.TicketWidth;
            pdoc.DefaultPageSettings.PaperSize.Height = Settings.Default.TicketLength;
            pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.BartenderPrinter;

            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);

            for (int i = 1; i <= Settings.Default.KitchenPrinterCopies; i++)
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

                // BUSINESS NAME
                workVar = new string(' ', 10 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
                graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 35;

                graphics.DrawString("ORDEN DE BEBIDAS", new Font("Consolas Bold", 14), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                workVar = new string(' ', 15 - (Settings.Default.WorkStationType.Length / 2)) + Settings.Default.WorkStationType;
                graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 25;

                // TICKET HEADER
                // CUSTOMER
                string tmp = custDesc.Split('|')[0];

                string cust = tmp.Split('^')[1];
                string waitRest = custDesc.Split('|')[1];

                if (cust.Length > 22)
                    cust = cust.Substring(0, 22);

                graphics.DrawString(new string(' ', 18 - (cust.Length / 2)) + cust, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                //Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                // LIST OF BEVERAGES
                foreach (string beverage in beveragesList)
                {
                    if (beverage.Length == 0) continue;

                    workVar = Helper.FormatItemDetailLine(Convert.ToInt32(beverage.Split('|')[1]), beverage.Split('|')[2]);
                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);

                    if (beverage.Split('|')[3].Length > 0)
                    {
                        Guid guidID = Guid.NewGuid();

                        string[] bucketContent = beverage.Split('|')[3].Split('$');

                        foreach (string item in bucketContent)
                        {
                            workVar = item.Split(',')[1] + " " + DB.GetItemDescriptionByItemID(Convert.ToInt32(item.Split(',')[0]));
                            Offset += 18;
                            graphics.DrawString(new string(' ', 3) + workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                            DB.InsertBucketDetail(Convert.ToInt32(tmp.Split('^')[0]), guidID.ToString(), Convert.ToInt32(item.Split(',')[0]), Convert.ToInt32(item.Split(',')[1]));
                        }
                    }
                    Offset += 20;

                    DB.InsertItemOrder(waitRest, beverage.Split('|')[2], Convert.ToInt32(beverage.Split('|')[1]));
                }

                //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
                //Offset += 18;

                graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 18;

                clsUser userProf = Helper.CheckUserProfile(waitRest);
                workVar = $"COLABORADOR: {userProf.userName}";
                graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 20;

                graphics.DrawString(Helper.FormatGralLine(DB.ConverTicketDate(Settings.Default.BusinessDate)), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                Offset += 80;

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
