using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.CategoryDtos
{
    public class CategoryReadDto
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public ICollection<SubCategoryReadDto> SubCategories { get; set; } = new HashSet<SubCategoryReadDto>();
    }

    public class SubCategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
