using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class ProductFeedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Rate { get; set; } // من 1 إلى 5
        public string Comment { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft Delete
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
