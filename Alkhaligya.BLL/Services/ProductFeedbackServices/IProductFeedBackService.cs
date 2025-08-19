using Alkhaligya.BLL.Dtos.ProductFeedbackDto;
using Alkhaligya.BLL.Dtos.Responce;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.ProductFeedbackServices
{
    public interface IProductFeedbackService
    {
        Task<ApiResponse<List<ProductFeedbackReadDto>>> GetProductFeedbacksAsync(int productId);
        Task<ApiResponse<string>> AddProductFeedbackAsync(ProductFeedbackAddDto dto);
        Task<ApiResponse<string>> DeleteProductFeedbackAsync(int id);
    }
}
