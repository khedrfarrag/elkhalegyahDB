using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.ProductDtos
{
    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool? HasDiscount { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? FeedbackScore { get; set; } // 0 to 5
    }

}
