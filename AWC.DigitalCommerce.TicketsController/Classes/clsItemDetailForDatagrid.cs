using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsItemDetailForDatagrid
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int UnitCost { get; set; }
        public int TotalPrice { get; set; }
        public int TotalCost { get; set; }
        public string Note { get; set; }
        public string AbortReason { get; set; }
        public string ImagePath { get; set; }
    }
}
