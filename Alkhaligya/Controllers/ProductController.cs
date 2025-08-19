using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.ProductServices;
using Alkhaligya.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alkhaligya.BLL.Services.ProductServices;
using Alkhaligya.DAL.Models;
using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Dtos.Auth;

namespace Alkhaligya.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.GetAllProductsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }

        // باقي العمليات تحتاج توثيق مستخدم
        [HttpPost("AddProductAsync")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> AddProductAsync([FromForm] ProductAddDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _productService.AddProductAsync(dto);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromForm] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.UpdateProductAsync(id, dto);
            return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string productName , [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8) 
        {
            var response = await _productService.SearchProductsAsync(productName, pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : NotFound(response.Errors);
        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter([FromQuery] ProductFilterDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.FilterProductsAsync(filter, pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpGet("out-of-stock/count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOutOfStockCountAsync()
        {
            var response = await _productService.GetOutOfStockCountAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }

        [HttpGet("4-top-discounted-products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopDiscountedProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.GetTopDiscountedProductsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpPut("mark-popular/{productId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> MarkProductAsPopularAsync(int productId)
        {
            var response = await _productService.MarkProductAsPopularAsync(productId);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        [HttpPut("mark-not-popular/{productId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> MarkProductAsNotPopularAsync(int productId)
        {
            var response = await _productService.MarkProductAsNotPopularAsync(productId);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.GetPopularProductsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpGet("by-category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategoryIdAsync(int categoryId)
        {
            var response = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }

        [HttpPost("favorites")]
        [Authorize]
        public async Task<IActionResult> AddToFavoritesAsync([FromQuery] string userId, [FromQuery] int productId)
        {
            var response = await _productService.AddToFavoritesAsync(userId, productId);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        [HttpDelete("favorites")]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavoritesAsync([FromQuery] string userId, [FromQuery] int productId)
        {
            var response = await _productService.RemoveFromFavoritesAsync(userId, productId);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetUserFavoritesAsync([FromQuery] string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.GetUserFavoritesAsync(userId, pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpGet("discounted")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDiscountedProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _productService.GetDiscountedProductsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }
    }
}
