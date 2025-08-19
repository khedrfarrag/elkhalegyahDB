using Alkhaligya.BLL.Dtos.Cart;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.PayMob;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.OrderServices;
using Alkhaligya.BLL.Services.PayMob;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Alkhaligya.BLL.Services.Cart
{
    public class CartShopService : ICartShopService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public CartShopService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
        }


        private async Task<CartShop?> GetCartAsync(string? userId, string? sessionId, bool includeProducts = false)
        {
            var query = _unitOfWork.CartShops.FindAll(c =>
                (!string.IsNullOrEmpty(userId) && c.UserId == userId ||
                string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sessionId) && c.SessionId == sessionId)
                && !c.IsDeleted);

            if (includeProducts)
            {
                query = query
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductFeedbacks);
            }
            else
            {
                query = query.Include(c => c.CartItems);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ApiResponse<ReadCartShopDto>> GetCartByUserOrGuestAsync(string? userId, string? sessionId)
        {
            var cart = await GetCartAsync(userId, sessionId, includeProducts: true);

            if (cart == null)
                return new ApiResponse<ReadCartShopDto>("لم يتم العثور على عربة التسوق");

            var dto = new ReadCartShopDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                SessionId = cart.SessionId,
                TotalQuantity = cart.GetTotalQuantity(),
                TotalPrice = cart.GetTotalPrice(),
                CartItems = cart.CartItems
                    .Where(ci => !ci.IsDeleted)
                    .Select(ci => new ReadCartItemDto
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        Description = ci.Product.Description,
                        ImageUrl = ci.Product.ImageUrl,
                        Price = ci.Product.DiscountedPrice ?? ci.Product.Price,
                        Quantity = ci.Quantity,
                        AverageRate = ci.Product.ProductFeedbacks.Any(fb => !fb.IsDeleted)
                            ? ci.Product.ProductFeedbacks.Where(fb => !fb.IsDeleted).Average(fb => fb.Rate)
                            : 0
                    }).ToList()
            };

            return new ApiResponse<ReadCartShopDto>(dto);
        }

        public async Task<ApiResponse<string>> AddOrUpdateCartAsync(AddCartShopDto dto , string? UserId, string? SessionId)
        {
            var cart = await GetCartAsync(UserId, SessionId);

            if (cart == null)
            {
                cart = new CartShop
                {
                    UserId = UserId,
                    SessionId = string.IsNullOrEmpty(UserId) ? SessionId : null,
                    CartItems = new List<CartItem>()
                };

                foreach (var item in dto.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null || product.IsDeleted)
                        return new ApiResponse<string>($"لم يتم العثور على المنتج برقم {item.ProductId}");

                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }

                await _unitOfWork.CartShops.AddAsync(cart);
            }
            else
            {
                foreach (var item in dto.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null || product.IsDeleted)
                        return new ApiResponse<string>($"لم يتم العثور على المنتج برقم {item.ProductId}");

                    var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId && !ci.IsDeleted);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = item.Quantity;
                    }
                    else
                    {
                        cart.CartItems.Add(new CartItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                    }
                }

                await _unitOfWork.CartShops.UpdateAsync(cart);
            }

            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "Cart updated successfully");
        }

        public async Task<ApiResponse<string>> UpdateCartItemQuantityAsync(UpdateCartItemDto dto, int Id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(Id);
            if (cartItem == null || cartItem.IsDeleted)
                return new ApiResponse<string>("لم يتم العثور على عنصر عربة التسوق");

            cartItem.Quantity = dto.Quantity;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم تحديث كمية المنتج بنجاح");
        }

        public async Task<ApiResponse<string>> DeleteCartItemAsync(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.IsDeleted)
                return new ApiResponse<string>("لم يتم العثور على عنصر عربة التسوق");

            cartItem.IsDeleted = true;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم حذف المنتج من السلة");
        }

        public async Task<ApiResponse<string>> ClearCartAsync(string? userId, string? sessionId)
        {
            var cart = await GetCartAsync(userId, sessionId);
            if (cart == null || cart.CartItems.All(ci => ci.IsDeleted))
                return new ApiResponse<string>("عربة التسوق فارغة أو غير موجودة");

            foreach (var item in cart.CartItems.Where(ci => !ci.IsDeleted))
            {
                item.IsDeleted = true;
            }

            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم مسح جميع عناصر عربة التسوق بنجاح");
        }

        public async Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(string? userId, string? sessionId)
        {
            var cart = await GetCartAsync(userId, sessionId, includeProducts: true);

            if (cart == null || cart.CartItems.All(ci => ci.IsDeleted))
                return new ApiResponse<CartSummaryDto>("عربة التسوق فارغة أو غير موجودة");

            var totalQuantity = cart.CartItems
                .Where(ci => !ci.IsDeleted)
                .Sum(ci => ci.Quantity);

            var totalPrice = cart.CartItems
                .Where(ci => !ci.IsDeleted)
                .Sum(ci => ci.Quantity * (ci.Product?.DiscountedPrice ?? ci.Product?.Price ?? 0));

            var summary = new CartSummaryDto
            {
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice
            };

            return new ApiResponse<CartSummaryDto>(summary);
        }


        public async Task<ApiResponse<int>> ConvertCartToOrderAsync(AddOrderDto2 addOrderDto, string? UserId, string? SessionId)
        {
            const int maxRetryAttempts = 3;
            int attempt = 0;

            while (attempt < maxRetryAttempts)
            {
                try
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var cart = await GetCartAsync(UserId, SessionId, includeProducts: true);

                        if (cart == null || !cart.CartItems.Any(ci => !ci.IsDeleted))
                            return new ApiResponse<int>("عربة التسوق فارغة أو غير موجودة");

                        foreach (var item in cart.CartItems.Where(ci => !ci.IsDeleted))
                        {
                            if (item.Quantity > item.Product.StockQuantity)
                                return new ApiResponse<int>($"لا يوجد مخزون كافٍ للمنتج {item.Product.Name}");
                        }

                        var order = new Order
                        {
                            UserId =  UserId,
                            SessionId = string.IsNullOrEmpty(UserId) ? SessionId : null,
                            IsGuestOrder = addOrderDto.IsGuestOrder,
                            FirstName = addOrderDto.FirstName,
                            LastName = addOrderDto.LastName,
                            MobileNumber = addOrderDto.MobileNumber,
                            Address = addOrderDto.Address,
                            Governorate = addOrderDto.Governorate,
                            OrderDate = DateTime.UtcNow,
                            PaymentMethod = addOrderDto.PaymentMethod,
                            PaymentStatus = PaymentStatus.Pending,
                            OrderItems = new List<OrderItem>()
                        };

                        foreach (var item in cart.CartItems.Where(ci => !ci.IsDeleted))
                        {
                            item.Product.StockQuantity -= item.Quantity;
                            decimal itemPrice = item.Product.DiscountedPrice ?? item.Product.Price;
                            decimal itemTotal = itemPrice * item.Quantity;

                            order.OrderItems.Add(new OrderItem
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                TotalPrice = itemTotal
                            });
                        }

                        order.TotalPrice = order.OrderItems.Sum(oi => oi.TotalPrice);
                        order.TotalQuantity = order.OrderItems.Sum(oi => oi.Quantity);

                        await _unitOfWork.Orders.AddAsync(order);

                        foreach (var item in cart.CartItems)
                        {
                            item.IsDeleted = true;
                        }

                        await _unitOfWork.CommitChangesAsync();


                        scope.Complete();

                        return new ApiResponse<int>(order.Id, "تم إنشاء الطلب بنجاح");
                    }
                }
                catch (Exception ex)
                {
                    attempt++;

                    if (attempt >= maxRetryAttempts)
                        return new ApiResponse<int>("حدث خطأ أثناء تحويل السلة إلى طلب بعد عدة محاولات");

                    await Task.Delay(500);
                }
            }

            return new ApiResponse<int>("خطأ غير متوقع");
        }

        public async Task<ApiResponse<PaymentResponseDto>> CheckoutAsync(AddOrderDto2 addOrderDto  , string? UserId , string? SessionId)
        {
            var orderResponse = await ConvertCartToOrderAsync(addOrderDto , UserId , SessionId);
            if (!orderResponse.Succeeded)
                return new ApiResponse<PaymentResponseDto>(orderResponse.Errors.FirstOrDefault());

            var orderId = orderResponse.Data;
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
                return new ApiResponse<PaymentResponseDto>("لم يتم العثور على الطلب");

            if (order.PaymentStatus == PaymentStatus.Paid)
                return new ApiResponse<PaymentResponseDto>(null, "تم دفع الطلب بالفعل");

            if (order.PaymentMethod == PaymentMethodEnum.CashOnDelivery)
                return new ApiResponse<PaymentResponseDto>(null, $"تم إنشاء الطلب بنجاح. رقم الطلب: {orderId}");

            var paymentRequest = new CreatePaymentRequestDto
            {
                OrderId = orderId,
                Amount = order.TotalPrice,
                Currency = "EGP"
            };
            BackgroundJob.Schedule(() => _orderService.CancelUnpaidOrderAsync(orderId), TimeSpan.FromMinutes(30));
            return await _paymentService.InitiateUnifiedCheckoutAsync(paymentRequest);
        }

    }

}

