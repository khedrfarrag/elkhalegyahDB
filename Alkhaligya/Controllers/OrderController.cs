using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alkhaligya.BLL.Services.OrderServices;
using Alkhaligya.DAL.Models;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Alkhaligya.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _orderService.GetAllOrdersAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> AddOrderAsync([FromBody] AddOrderDto dto)
        {
            var response = await _orderService.AddOrderAsync(dto);
            return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> UpdateOrderAsync(int id, [FromBody] UpdateOrderDto dto)
        {
            
            var response = await _orderService.UpdateOrderAsync(dto , id);
            return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            var response = await _orderService.DeleteOrderAsync(id);
            return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
        }

    
        [HttpGet("user")]
        public async Task<IActionResult> GetUserOrdersAsync()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var response = await _orderService.GetUserOrdersAsync(currentUserId);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }


        [HttpGet("summary/{orderId}")]
        public async Task<IActionResult> GetOrderSummaryAsync(int orderId)
        {
            var response = await _orderService.GetOrderSummaryAsync(orderId);
            return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
        }

        [HttpGet("total")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetTotalNumberOfOrdersAsync()
        {
            var response = await _orderService.GetTotalNumberOfOrdersAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }

        [HttpGet("total-paid-price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalPaidOrdersPriceAsync()
        {
            var response = await _orderService.GetTotalPaidOrdersPriceAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }

        [HttpGet("paid/count")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetPaidOrdersCountAsync()
        {
            var response = await _orderService.GetPaidOrdersCountAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }

        [HttpGet("pending/count")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetPendingOrdersCountAsync()
        {
            var response = await _orderService.GetPendingOrdersCountAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }

        [HttpGet("failed/count")]
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        public async Task<IActionResult> GetFailedOrdersCountAsync()
        {
            var response = await _orderService.GetFailedOrdersCountAsync();
            return response.Succeeded ? Ok(response.Data) : BadRequest(response.Errors);
        }
    }
}
