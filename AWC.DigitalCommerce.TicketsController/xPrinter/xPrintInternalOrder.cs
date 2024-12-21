using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;
using System.IO;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrintInternalOrder
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string fileName = string.Empty;

        public xPrintInternalOrder(string _fileName)
        {
            fileName = _fileName;
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
            pdoc.DefaultPageSettings.PrinterSettings.PrinterName = Settings.Default.KitchenPrinter;

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

            // BUSINESS NAME
            workVar = new string(' ', 10 - (Settings.Default.BusinessName.Length / 2)) + Settings.Default.BusinessName;
            graphics.DrawString(workVar, new Font("Consolas Bold", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 35;

            graphics.DrawString("ORDEN DE PEDIDO", new Font("Consolas Bold", 14), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 25;

            using (StreamReader sr = new System.IO.StreamReader(fileName))
            {
                bool firstRec = true;

                while (!sr.EndOfStream)
                {
                    string rec = sr.ReadLine();

                    if (firstRec)
                    {
                        graphics.DrawString(rec, new Font("Consolas Bold", 14), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 25;

                        graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 18;
                        firstRec = false;
                    }
                    else
                    {
                        workVar = Helper.FormatItemDetailLine(Convert.ToInt32(rec.Split(',')[1]), rec.Split(',')[0]);
                        graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                        Offset += 20;
                    }
                }
            }

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());
            workVar = $"COLABORADOR: {userProf.userName}";
            graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(Helper.FormatGralLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt")), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            // Cut line
            workVar = ".   .    .    .    .    .    .";
            graphics.DrawString(workVar, new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
        }
    }
}
