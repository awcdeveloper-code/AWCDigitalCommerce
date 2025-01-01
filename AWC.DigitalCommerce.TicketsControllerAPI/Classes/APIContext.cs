using Microsoft.EntityFrameworkCore;

namespace AWC.DigitalCommerce.TicketsControllerAPI.Classes
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
