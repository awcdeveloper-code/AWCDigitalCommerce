using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsLoyaltyReward
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ItemToQualify { get; set; }
        public int QtyToQualify { get; set; }
        public int MaxDaysForReward { get; set; }
        public int ItemRewarded { get; set; }
        public int QtyRewarded { get; set; }
        public int TotalItemsAwarded { get; set; }
    }
}
