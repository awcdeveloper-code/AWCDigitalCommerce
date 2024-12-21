using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsItemTypeCollection : System.Collections.ObjectModel.Collection<clsItemType>
    {
        public clsItemTypeCollection()
        {
            Add(new clsItemType { Qty = 0, ItemDesc = "Hope" });
        }
    }
}
