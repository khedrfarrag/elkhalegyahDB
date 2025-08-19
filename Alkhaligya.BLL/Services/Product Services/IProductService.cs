using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.ProductServices
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductReadDto>>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<ProductReadDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<string>> AddProductAsync(ProductAddDto dto);
        Task<ApiResponse<string>> UpdateProductAsync(int id, ProductUpdateDto dto);
        Task<ApiResponse<string>> DeleteProductAsync(int id);
        Task<ApiResponse<List<ProductReadDto>>> SearchProductsAsync(string productName, int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<List<ProductReadDto>>> FilterProductsAsync(ProductFilterDto filter, int pageNumber = 1, int pageSize = 8); Task<ApiResponse<int>> GetOutOfStockCountAsync();
        Task<ApiResponse<List<ProductReadDto>>> GetTopDiscountedProductsAsync(int pageNumber = 1, int pageSize = 1);
        Task<ApiResponse<string>> MarkProductAsPopularAsync(int productId);
        Task<ApiResponse<string>> MarkProductAsNotPopularAsync(int productId);
        Task<ApiResponse<List<ProductReadDto>>> GetPopularProductsAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<List<ProductReadDto>>> GetProductsByCategoryIdAsync(int categoryId);
        Task<ApiResponse<string>> AddToFavoritesAsync(string userId, int productId);
        Task<ApiResponse<string>> RemoveFromFavoritesAsync(string userId, int productId);
        Task<ApiResponse<List<ProductReadDto>>> GetUserFavoritesAsync(string userId, int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<List<ProductReadDto>>> GetDiscountedProductsAsync(int pageNumber = 1, int pageSize = 8);
    }
}
