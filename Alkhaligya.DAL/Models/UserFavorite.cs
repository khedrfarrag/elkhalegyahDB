using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class UserFavorite
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; }
        public Product Product { get; set; }
    }
}