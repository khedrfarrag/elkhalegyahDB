using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.ProductFeedbackDto;
using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.ProductDtos
{
    public class ProductReadDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StockStatues { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public int Rate { get; set; } // من 1 إلى 5

        public int? SubCategoryId { get; set; }
        public int CategoryId { get; set; }

        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string? ImageUrl { get; set; }

        public string? Title1 { get; set; }
        public string? Body1 { get; set; }
        public string? Title2 { get; set; }
        public string? Body2 { get; set; }
        public ICollection<ProductFeedbackReadDto> ProductFeedbacks { get; set; } = new List<ProductFeedbackReadDto>();
    }


    public class ProductDetailsReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
