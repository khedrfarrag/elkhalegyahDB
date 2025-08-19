using Alkhaligya.BLL.Dtos.CategoryDtos;
using Alkhaligya.BLL.Dtos.Responce;
using System.Linq;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<ApiResponse<IQueryable<CategoryReadDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryReadDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<string>> AddCategoryAsync(CategoryAddDto dto);
        Task<ApiResponse<string>> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<ApiResponse<string>> DeleteCategoryAsync(int id);

        Task<ApiResponse<IQueryable<SubCategoryReadDto>>> GetSubCategoriesByCategoryIdAsync(int categoryId);
    }
}
