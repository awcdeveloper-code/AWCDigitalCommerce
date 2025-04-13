using System.Collections.ObjectModel;

namespace AWC.DigitalCommerce.TicketsController.Classes
{
    public class clsUserSecurityProfileViewModel
    {
        public ObservableCollection<clsUserSecurityProfile> Items { get; set; }

        public clsUserSecurityProfileViewModel()
        {
            Items = new ObservableCollection<clsUserSecurityProfile>
            {
                new clsUserSecurityProfile { Name = "Item 1", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 2", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 3", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 4", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 5", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 6", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 7", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 8", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 9", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 10", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 11", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 12", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 13", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 14", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 15", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 16", IsChecked = true },
                new clsUserSecurityProfile { Name = "Item 17", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 18", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 19", IsChecked = false },
                new clsUserSecurityProfile { Name = "Item 20", IsChecked = false }
            };
        }
    }
}
