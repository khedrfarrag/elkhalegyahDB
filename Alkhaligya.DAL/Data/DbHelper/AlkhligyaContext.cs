using Alkhaligya.DAL.Data.Config;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Models.PayMob;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Data.DbHelper
{
    public class AlkhligyaContext : IdentityDbContext<ApplicationUser, CustomRole, string>
    {

        public AlkhligyaContext(DbContextOptions<AlkhligyaContext> options) : base(options)
        {
            // Only run migrations at runtime, not at design time (when adding migrations)
            if (!IsDesignTime())
            {
                try
                {
                    Database.Migrate(); // Apply all migrations and create DB/tables if needed
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Migration Error: {ex.Message}");
                }
            }
        }

        private bool IsDesignTime()
        {
            // This will be true when running EF Core CLI or PMC tools
            return AppDomain.CurrentDomain.FriendlyName.Contains("ef") ||
                   AppDomain.CurrentDomain.FriendlyName.ToLower().Contains("vstest") ||
                   AppDomain.CurrentDomain.FriendlyName.ToLower().Contains("testhost");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            base.OnConfiguring(optionsBuilder);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new SiteFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            // modelBuilder.ApplyConfiguration(new ProductDetailEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new CartShopEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContactMessageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentTransactionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserFavoriteEntityTypeConfiguration());











        }



        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        // public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductFeedback> ProductFeedbacks { get; set; }


        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<CartShop> CartShops { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<SiteFeedback> SiteFeedbacks { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

        public DbSet<CustomRole> CustomRoles { get; set; }

        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        public DbSet<UserFavorite> UserFavorites { get; set; }
    }
}
