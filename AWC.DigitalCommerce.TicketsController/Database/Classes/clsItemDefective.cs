using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsItemDefective
    {
        public int  ID { get; set; }
        public int ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQty { get; set; }
        public string DeclarationDate { get; set; }
        public int whoDeclared { get; set; }
        public string whoDeclaredName { get; set; }
    }
}
