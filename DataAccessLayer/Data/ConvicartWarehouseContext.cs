using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
namespace ConvicartWebApp.DataAccessLayer.Data
{
    public class ConvicartWarehouseContext : DbContext
    {
        // DbSets for each table
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<QuerySubmission> QuerySubmissions { get; set; }
        public DbSet<RecipeSteps> RecipeSteps { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Constructor accepting DbContextOptions
        public ConvicartWarehouseContext(DbContextOptions<ConvicartWarehouseContext> options)
            : base(options) // Pass options to the base constructor
        {
        }

        // Configure model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithMany() // Adjust if Address has a navigation property back to Customer
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict); // Use appropriate delete behavior

            modelBuilder.Entity<Address>()
                .Property(a => a.AddressId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)"); // Precision 18, Scale 2

            // Specify precision and scale for Store Price (if applicable)
            modelBuilder.Entity<Store>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18, 2)"); // Precision 18, Scale 2

            modelBuilder.Entity<CustomerPreference>()
                .HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

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

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade); // Automatically delete CartItems when a Cart is deleted

            // Add any additional model configurations here
        }
    }
}
