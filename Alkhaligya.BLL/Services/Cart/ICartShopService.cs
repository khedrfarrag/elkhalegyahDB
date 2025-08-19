using Alkhaligya.BLL.Dtos.Cart;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.PayMob;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Cart
{
    public interface ICartShopService
    {
        Task<ApiResponse<ReadCartShopDto>> GetCartByUserOrGuestAsync(string? userId, string? sessionId);

        Task<ApiResponse<string>> AddOrUpdateCartAsync(AddCartShopDto dto , string? UserId, string? SessionId);

        Task<ApiResponse<string>> UpdateCartItemQuantityAsync(UpdateCartItemDto dto, int Id);

        Task<ApiResponse<string>> DeleteCartItemAsync(int cartItemId);

        Task<ApiResponse<string>> ClearCartAsync(string? userId, string? sessionId);

        Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(string? userId, string? sessionId);

        Task<ApiResponse<int>> ConvertCartToOrderAsync(AddOrderDto2 addOrderDto, string? UserId, string? SessionId); 

        Task<ApiResponse<PaymentResponseDto>> CheckoutAsync(AddOrderDto2 addOrderDto, string? UserId, string? SessionId);
    }



}
