using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories
{
    public class ProductRepositiory :Repository<Product,int>, IProductRepository
    {
        public ProductRepositiory(AlkhligyaContext context) : base(context)
        {
        }


        public new async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.ProductFeedbacks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }



        public async Task<List<Product>> SearchByProductNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<Product>();

            return await _context.Products
                .Where(p => p.Name.ToLower().Contains(productName.ToLower()))
                .ToListAsync();
        }


    }

}
