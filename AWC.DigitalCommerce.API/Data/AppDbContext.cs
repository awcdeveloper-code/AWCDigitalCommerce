using AWC.DigitalCommerce.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AWC.DigitalCommerce.API
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        DbSet<Seat> Seats { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                    .Property(p => p.Cost)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Subtotal)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Taxes)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.ServiceFee)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Total)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Cash)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Card)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Transfer)
                    .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                    .Property(p => p.Voucher)
                    .HasColumnType("decimal(10,2)");
        }
    }
}
