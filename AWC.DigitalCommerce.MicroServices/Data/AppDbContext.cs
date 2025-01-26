using Microsoft.EntityFrameworkCore;
using AWC.DigitalCommerce.MicroServices.Models;

namespace AWC.DigitalCommerce.MicroServices.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
