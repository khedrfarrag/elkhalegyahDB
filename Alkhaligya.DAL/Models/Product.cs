using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPopular { get; set; } = false;

        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string ImageUrl { get; set; }

        // New direct category relationship
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int? SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public ICollection<ProductFeedback> ProductFeedbacks { get; set; } = new HashSet<ProductFeedback>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new HashSet<UserFavorite>();
        public string? Title1 { get; set; }
        public string? Body1 { get; set; }
        public string? Title2 { get; set; }
        public string? Body2 { get; set; }
        public int Rate { get; set; }

        public void CalculateDiscountedPrice()
        {
            if (DiscountPercentage.HasValue && DiscountPercentage.Value > 0)
            {
                DiscountedPrice = Price - (Price * (DiscountPercentage.Value / 100));
            }
            else
            {
                DiscountedPrice = null;
            }
        }
    }
}
