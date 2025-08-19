using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class CartShop
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // ✅ دوال بدل خصائص
        public int GetTotalQuantity() => CartItems?.Where(ci => !ci.IsDeleted).Sum(ci => ci.Quantity) ?? 0;

        public decimal GetTotalPrice()
        {
            return CartItems?
                .Where(ci => !ci.IsDeleted && ci.Product != null)
                .Sum(ci => ci.Quantity * (ci.Product.DiscountedPrice ?? ci.Product.Price)) ?? 0;
        }

    }



}
