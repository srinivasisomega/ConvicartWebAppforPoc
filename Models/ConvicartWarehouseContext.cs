using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ConvicartWebApp.Models
{
    public class ConvicartWarehouseContext : DbContext
    {
        // DbSets for each table
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<Diet> Diets { get; set; }
        public DbSet<CustomerDiet> CustomerDiets { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Constructor accepting DbContextOptions
        public ConvicartWarehouseContext(DbContextOptions<ConvicartWarehouseContext> options)
            : base(options) // Pass options to the base constructor
        {
        }

        // Configure model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerPreference>()
                .HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

            modelBuilder.Entity<CustomerDiet>()
                .HasKey(cd => new { cd.CustomerId, cd.DietId });
            modelBuilder.Entity<Store>()
        .Property(s => s.Price)
        .HasColumnType("decimal(10, 2)");

            modelBuilder.Entity<Store>()
                .Property(s => s.Carbs)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<Store>()
                .Property(s => s.Proteins)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<Store>()
                .Property(s => s.Vitamins)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<Store>()
                .Property(s => s.Minerals)
                .HasColumnType("decimal(5, 2)");

            // Add any additional model configurations here
        }
    }


}
