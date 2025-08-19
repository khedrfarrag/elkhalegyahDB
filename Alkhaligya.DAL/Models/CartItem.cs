using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartShopId { get; set; } // يرتبط بعربة التسوق
        public int ProductId { get; set; } // المنتج المضاف للعربة
        public int Quantity { get; set; } // الكمية المطلوبة
        public bool IsDeleted { get; set; } = false; // Soft Delete

        public CartShop CartShop { get; set; }
        public Product Product { get; set; }
    }



}
