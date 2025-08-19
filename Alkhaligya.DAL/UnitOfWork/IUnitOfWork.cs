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
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        IRepository<ApplicationUser, int> ApplicationUsers { get; }
        IRepository<CartItem, int> CartItems { get; }
        IRepository<CartShop, int> CartShops { get; }
        IPaymentTransactionRepository PaymentTransactions { get; }


        CategoryRepositiory Categories { get; }
        // IRepository<Category,int> Categories { get; }
        // SubCategoryRepository SubCategories { get; }
        IRepository<SubCategory, int> SubCategories { get; }
        IRepository<ContactMessage, int> ContactMessages { get; }
        IRepository<Order, int> Orders { get; }
        IRepository<OrderItem, int> OrderItems { get; }
        IProductRepository Products { get; }
      
        //IRepository<Product, int> Products { get; }


        // IRepository<ProductDetail, int> ProductDetails { get; }  remove prduct details
        //IRepository<SiteFeedback, int> SiteFeedbacks { get; }

        ISiteFeedbackRepository SiteFeedbacks { get; }
        IRepository<ProductFeedback, int> ProductFeedbacks { get; }
        IUserFavoriteRepository UserFavorites { get; }
        // Commit Changes
        Task<int> CommitChangesAsync(); // return Affects Row!
    }
}
