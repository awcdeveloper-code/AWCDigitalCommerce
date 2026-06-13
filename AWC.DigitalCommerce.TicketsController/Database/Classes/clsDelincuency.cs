using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsDelincuency
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public int sum_0_8_days { get; set; }
        public int sum_9_15_days { get; set; }
        public int sum_16_30_days { get; set; }
        public int sum_31_45_days { get; set; }
        public int sum_46_60_days { get; set; }
        public int sum_61_days { get; set; }
    }
}

