using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController
{
    public class xPrinterMealOrder
    {
        private string workVar = string.Empty;
        private PrintDocument pdoc = null;
        private string custDesc = string.Empty;
        private List<string> mealList = new List<string>();

        public xPrinterMealOrder()
        {

        }

        public xPrinterMealOrder(string _custDesc, List<string> _mealList)
        {
            custDesc = _custDesc;
            mealList = _mealList;
        }

        public void print()
        {
            if (Settings.Default.KitchenPrinter.Length == 0) return;

            if (mealList.Count > Settings.Default.PrintKitchenOrderHigherThan)
            {
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

                for (int i = 1; i <= Settings.Default.KitchenPrinterCopies; i++)
                    pdoc.Print();
            }
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen blackPen = new Pen(Color.Black, 4);

            int startX = 0;
            int startY = 0;

            int startXV = 120;
            int startYV = -85;

            int Offset = 0;

            workVar = "ORDEN DE COCINA";
            graphics.DrawString(new string(' ', 16 - (workVar.Length / 2)) + workVar, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(new string(' ', 18 - (Settings.Default.WorkStationType.Length / 2)) + Settings.Default.WorkStationType, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            if (custDesc.Length > 22)
                custDesc = custDesc.Substring(0, 22);

            graphics.DrawString(new string(' ', 18 - (custDesc.Length / 2)) + custDesc, new Font("Consolas Bold", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            //e.Graphics.DrawLine(blackPen, 0, Offset, 200, Offset);
            //Offset += 18;

            graphics.DrawString(DateTime.Now.ToString("dd.MM.yyyy hh:mm tt"), new Font("Consolas", 11), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            clsUser userProf = Helper.CheckUserProfile(Settings.Default.WhoOpen.ToString());
            workVar = $"COLABORADOR: {userProf.userName}";
            graphics.DrawString(Helper.FormatGralLine(workVar), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 20;

            graphics.DrawString(new string('=', 30), new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset += 18;

            // LIST OF MEALS
            bool isFirstVerticalLine = true;

            foreach (string meal in mealList)
            {
                if (Settings.Default.DetailOfMealOrderInVertical)
                {
                    if (isFirstVerticalLine)
                    {
                        graphics.TranslateTransform(100, 0);
                        graphics.RotateTransform(90);
                        isFirstVerticalLine= false;
                    }

                    using (Font font = new Font("Arial", 14))
                    {
                        workVar = Helper.FormatItemDetailLine(Convert.ToInt32(meal.Split('|')[0]), meal.Split('|')[1]);

                        graphics.DrawString(workVar, font, Brushes.Black, startXV, startYV);

                        if (meal.Split('|')[2].Length > 0)
                        {
                            string secondLine = new string(' ', 4) + meal.Split('|')[2];
                            startYV += 20;
                            graphics.DrawString(secondLine, new Font("Arial", 10), Brushes.Black, startXV, startYV);
                        }
                    }

                    startYV += 25;
                }
                else
                {
                    workVar = Helper.FormatItemDetailLine(Convert.ToInt32(meal.Split('|')[0]), meal.Split('|')[1]);

                    graphics.DrawString(workVar, new Font("Consolas", 10), new SolidBrush(Color.Black), startX, startY + Offset);

                    if (meal.Split('|')[2].Length > 0)
                    {
                        Offset += 18;
                        graphics.DrawString(new string(' ', 3) + meal.Split('|')[2], new Font("Consolas", 8), new SolidBrush(Color.Black), startX, startY + Offset);
                    }

                    Offset += 20;
                }

                DB.InsertItemOrder(Settings.Default.WhoOpen.ToString(), meal.Split('|')[1], Convert.ToInt32(meal.Split('|')[0]));
            }
        }
    }
}
