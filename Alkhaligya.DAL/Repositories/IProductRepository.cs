using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories
{
    public interface IProductRepository  :IRepository<Product, int>
    {
        new Task<Product?> GetByIdAsync(int id);


        Task<List<Product>> SearchByProductNameAsync(string productName);


    }
}
