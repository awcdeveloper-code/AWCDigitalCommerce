using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsItem
    {
        public int ID { get; set; }
        public int ItemType { get; set; }
        public int ItemSubType { get; set; }
        public string ItemDescription { get; set; }
        public bool IsActive { get; set; }
        public int UnitPrice { get; set; }
        public int UnitCost { get; set; }
        public int ItemAvailable { get; set; }
        public int ItemSold { get; set; }
        public int ItemDefective { get; set; }
        public int DebitNotes { get; set; }
        public int CreditNotes { get; set; }
        public int ItemParent { get; set; }
        public string ItemParentDescription { get; set; }
        public int ItemParentUnit { get; set; }
        public int ItemMinimum { get; set; }
        public int ItemUnitOfMeasurement { get; set; }
        public int ItemUnitSize { get; set; }
        public int ItemStock { get; set; }
        public string ImagePath { get; set; }
    }
}
