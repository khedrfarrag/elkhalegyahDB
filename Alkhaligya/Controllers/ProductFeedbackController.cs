using Alkhaligya.BLL.Dtos.ProductFeedbackDto;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.ProductFeedbackServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Alkhaligya.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductFeedbackController : ControllerBase
    {
        private readonly IProductFeedbackService _productFeedbackService;

        public ProductFeedbackController(IProductFeedbackService productFeedbackService)
        {
            _productFeedbackService = productFeedbackService;
        }

        // Get feedbacks for a specific product by product ID
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductFeedbacksAsync(int productId)
        {
            var response = await _productFeedbackService.GetProductFeedbacksAsync(productId);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }

        // Add a new product feedback
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProductFeedbackAsync([FromBody] ProductFeedbackAddDto dto)
        {
            var response = await _productFeedbackService.AddProductFeedbackAsync(dto);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        // Soft delete product feedback by ID
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductFeedbackAsync(int id)
        {
            var response = await _productFeedbackService.DeleteProductFeedbackAsync(id);
            return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
        }
    }
}
