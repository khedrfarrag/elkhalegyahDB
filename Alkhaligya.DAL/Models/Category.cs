using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delete

        public ICollection<SubCategory> SubCategories { get; set; } = new HashSet<SubCategory>();

        // New direct products collection
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }


}
