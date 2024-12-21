using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsPromoConfig
    {
        public int ID { get; set; }
        public int PromoType { get; set; }
        public int PromoID { get; set; }
        public string PromoDescription { get; set; }
        public int ItemID { get; set; }
        public string PromoItemDescription { get; set; }
        public int PromoQty { get; set; }
    }
}
