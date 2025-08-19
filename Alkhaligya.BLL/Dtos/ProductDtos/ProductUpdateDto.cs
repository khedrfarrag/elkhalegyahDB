
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.ProductDtos
{
    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }

        public int? SubCategoryId { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? Title1 { get; set; }
        public string? Body1 { get; set; }
        public string? Title2 { get; set; }
        public string? Body2 { get; set; }
    }




    public class ProductDetailsUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
