namespace AWC.DigitalCommerce.TicketsController
{
    public class tblUser
    {
        public int userID { get; set; }
        public string userPIN { get; set; }
        public string userPW { get; set; }
        public string userName { get; set; }
        public string userAccessLevel { get; set; }
        public bool userActive { get; set; }

        public tblUser()
        {
            userID = 0;
            userPIN = string.Empty;
            userPW = string.Empty;
            userName = string.Empty;
            userAccessLevel = string.Empty;
            userActive = false;
        }
    }
}
