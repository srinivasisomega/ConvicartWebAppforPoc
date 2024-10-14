using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ConvicartWebApp.Models
{
    public class ConvicartWarehouseContext : IdentityDbContext<Customer, ApplicationRole, string,
     IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
     IdentityRoleClaim<string>, IdentityUserToken<string>>
    
    {
        public ConvicartWarehouseContext(DbContextOptions<ConvicartWarehouseContext> options)
            : base(options)
        {
        }
        // DbSets for each table
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }
       
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<QuerySubmission> QuerySubmissions { get; set; }
        public DbSet<RecipeSteps> RecipeSteps { get; set; }

        // Configure model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerPreference>()
                .HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

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
            modelBuilder.Entity<RecipeSteps>()
                .HasKey(ps => new { ps.ProductId, ps.StepNo }); // Composite key configuration

            

            // Add any additional model configurations here
        }
    }


}
