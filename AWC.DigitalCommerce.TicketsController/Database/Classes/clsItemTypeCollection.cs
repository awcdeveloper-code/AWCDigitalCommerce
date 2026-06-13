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
