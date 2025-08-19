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
    public class CategoryRepositiory : Repository<Category, int>, ICategoryRepository
    {
        public CategoryRepositiory(AlkhligyaContext context) : base(context)
        {
        }


        public new async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        //public new async Task<List<Category>> GetAll()
        //{
        //    return await _context.Categories
        //        .Include(c => c.SubCategories)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}
    }
}
