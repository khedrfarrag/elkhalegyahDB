using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.CategoryDtos
{
    public class CategoryAddDto
    {
        public string Name { get; set; }
        public ICollection<SubCategoryAddDto>? SubCategories { get; set; }
    }


    public class SubCategoryAddDto
    {
        public string Name { get; set; }

    }
}
