using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
   public  class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delete
        public int CategoryId{ get; set; }
        public Category Category { get; set; }

        public ICollection<Product> Products { get; set; } = new List <Product>();
    }
}
