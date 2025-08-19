using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Cart
{
    public class AddCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
        [JsonIgnore]
        public string? SessionId { get; set; }
    }


    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }

    public class ReadCartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public double AverageRate { get; set; }
    }

    public class AddCartShopDto
    {
      

        public List<AddCartItemDto> CartItems { get; set; }
    }

    public class ReadCartShopDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ReadCartItemDto> CartItems { get; set; }
    }

    public class CartSummaryDto
    {
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }


}
