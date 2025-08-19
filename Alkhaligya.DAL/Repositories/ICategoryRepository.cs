using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories
{
    public interface ICategoryRepository :IRepository<Category,int>
    {
         //new Task<List<Category>> GetAll();

        new  Task<Category?> GetByIdAsync(int id);


    }
}
