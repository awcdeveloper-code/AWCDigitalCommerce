using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsCategory
    {
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public int ParentID { get; set; }
        public clsCategory()
        {
            ParentID = 0;
            Description = string.Empty;
        }
    }
}
