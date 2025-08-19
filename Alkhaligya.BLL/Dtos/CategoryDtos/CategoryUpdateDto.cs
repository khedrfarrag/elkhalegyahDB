using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.CategoryDtos
{
    public class CategoryUpdateDto
    {
        public string Name { get; set; }

        public List<SubCategoryUpdateDto> SubCategories { get; set; }
    }

    public class SubCategoryUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
