using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Models.PayMob;
using Alkhaligya.DAL.Repositories;
using Alkhaligya.DAL.Repositories.Base;
using Alkhaligya.DAL.Repositories.Payment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<ApplicationUser, int> ApplicationUsers { get; private set; }

        public IRepository<CartItem, int> CartItems { get; private set; }

        public IRepository<CartShop, int> CartShops { get; private set; }

        //public IRepository<Category, int> Categories { get; private set ;}
        public CategoryRepositiory Categories { get; private set; }

        public IUserFavoriteRepository UserFavorites { get; private set; }
        public IRepository<ContactMessage, int> ContactMessages { get; private set; }

        public IRepository<Order, int> Orders { get; private set; }

        public IRepository<OrderItem, int> OrderItems { get; private set; }

        //  public IRepository<Product, int> Products{get; private set ;}
        public IProductRepository Products { get; private set; }
        //remove product details 
        //public IRepository<ProductDetail, int> ProductDetails { get; private set; }

        // public IRepository<SiteFeedback, int> SiteFeedbacks{get; private set ;}
        public ISiteFeedbackRepository SiteFeedbacks { get; private set; }

        public IRepository<ProductFeedback, int> ProductFeedbacks { get; private set; }

        public IRepository<SubCategory, int> SubCategories { get; private set; }



        public IPaymentTransactionRepository PaymentTransactions { get; private set; }

        // public SubCategoryRepository SubCategories { get; private set; }

        public AlkhligyaContext _context;
        public UnitOfWork(AlkhligyaContext context)
        {
            _context = context;
            ApplicationUsers = new Repository<ApplicationUser, int>(context);
            CartItems = new Repository<CartItem, int>(context);
            CartShops = new Repository<CartShop, int>(context);
            Categories = new CategoryRepositiory(context);
            ContactMessages = new Repository<ContactMessage, int>(context);
            Orders = new Repository<Order, int>(context);
            OrderItems = new Repository<OrderItem, int>(context);
            Products = new ProductRepositiory(context);
            //remove product details
            //ProductDetails = new Repository<ProductDetail, int>(context);
            //  SiteFeedbacks = new Repository<SiteFeedback, int>(context);
            SiteFeedbacks = new SiteFeedbackRepository(context);
            ProductFeedbacks = new Repository<ProductFeedback, int>(context);
            //  SubCategories =  new SubCategoryRepository(context);
            SubCategories = new Repository<SubCategory, int>(context);
            PaymentTransactions = new PaymentTransactionRepository(context);
            UserFavorites = new UserFavoriteRepository(context);



        }
        public DbContext Context => _context;

        // Save All System Changes and return the number of affected rows
        public async Task<int> CommitChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose the context to free memory space
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
