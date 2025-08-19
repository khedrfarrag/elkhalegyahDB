using Alkhaligya.BLL.Dtos.Cart;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class CartShopController : ControllerBase
{
    private readonly ICartShopService _cartShopService;

    public CartShopController(ICartShopService cartShopService)
    {
        _cartShopService = cartShopService;
    }

    // Get current user ID from claims
    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private string? GetSessionId()
    {
        //  Get sessionId From Header
        return Request.Headers["Session-Id"].FirstOrDefault();
    }

  
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCart()
    {
        var response = await _cartShopService.GetCartByUserOrGuestAsync(CurrentUserId, GetSessionId());
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AddOrUpdateCart([FromBody] AddCartShopDto dto)
    {
        string? userId = null;
        string? sessionId = null;

        if (User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(CurrentUserId))
        {
            userId = CurrentUserId;
        }
        else
        {
            sessionId = GetSessionId();
        }

        var response = await _cartShopService.AddOrUpdateCartAsync(dto, userId, sessionId);
        return response.Succeeded ? Ok(response.Message) : BadRequest(response.Errors);
    }


    [HttpPut("item/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemDto dto, int id)
    {
        var response = await _cartShopService.UpdateCartItemQuantityAsync(dto, id);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }

    [HttpDelete("item/{cartItemId}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteCartItem(int cartItemId)
    {
        var response = await _cartShopService.DeleteCartItemAsync(cartItemId);
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }


    [HttpDelete("clear")]
    [AllowAnonymous]
    public async Task<IActionResult> ClearCart()
    {
        var response = await _cartShopService.ClearCartAsync(CurrentUserId, GetSessionId());
        return response.Succeeded ? Ok(response.Message) : NotFound(response.Errors);
    }


    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCartSummary()
    {
        var response = await _cartShopService.GetCartSummaryAsync(CurrentUserId, GetSessionId());
        return response.Succeeded ? Ok(response.Data) : NotFound(response.Errors);
    }

    [HttpPost("checkout")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckoutAsync([FromForm] AddOrderDto2 addOrderDto)
    {
        string? userId = null;
        string? sessionId = null;

        if (User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(CurrentUserId))
        {
            userId = CurrentUserId;
        }
        else
        {
            sessionId = GetSessionId();
        }

        var response = await _cartShopService.CheckoutAsync(addOrderDto, userId, sessionId);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

}
