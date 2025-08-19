using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Alkhaligya.BLL.Dtos.Order.AddOrderDto;

namespace Alkhaligya.BLL.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<ReadOrderDto>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedOrders = _mapper.Map<List<ReadOrderDto>>(orders);

            var response = new ApiResponse<List<ReadOrderDto>>(mappedOrders);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<ReadOrderDto>> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return new ApiResponse<ReadOrderDto>("لم يتم العثور على الطلب");

            var mappedOrder = _mapper.Map<ReadOrderDto>(order);
            return new ApiResponse<ReadOrderDto>(mappedOrder);
        }


        public async Task<ApiResponse<string>> AddOrderAsync(AddOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                IsGuestOrder = dto.IsGuestOrder,
                OrderDate = DateTime.UtcNow,
                PaymentStatus = dto.PaymentStatus,

                // بيانات الفورم
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MobileNumber = dto.MobileNumber,
                Address = dto.Address,
                Governorate = dto.Governorate,
                PaymentMethod = dto.PaymentMethod,


                OrderItems = new List<OrderItem>()
            };


            decimal totalOrderPrice = 0;

            foreach (var item in dto.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                if (item.Quantity > product.StockQuantity)
                    return new ApiResponse<string>($"لا يوجد مخزون كافٍ للمنتج {product.Name}");

                product.StockQuantity -= item.Quantity;

                decimal itemPrice = product.DiscountedPrice ?? product.Price;
                decimal itemTotal = itemPrice * item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotal
                });

                totalOrderPrice += itemTotal;
            }

            order.TotalPrice = totalOrderPrice;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CommitChangesAsync();

            if (dto.PaymentStatus == PaymentStatus.Pending)
            {
                // جدولة الحذف لو لم يتم الدفع
                BackgroundJob.Schedule(() => CancelUnpaidOrderAsync(order.Id), TimeSpan.FromMinutes(10));
            }

            return new ApiResponse<string>(null, "تمت إضافة الطلب بنجاح");
        }

        public async Task<ApiResponse<string>> UpdateOrderAsync(UpdateOrderDto dto, int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id,
                include: o => o.Include(x => x.OrderItems).ThenInclude(x => x.Product));

            if (order == null)
                return new ApiResponse<string>("لم يتم العثور على الطلب");

            // تحويل العناصر القديمة والجديدة لقواميس لسهولة الوصول
            var oldItemsDict = order.OrderItems.ToDictionary(x => x.ProductId);
            var newItemsDict = dto.OrderItems.ToDictionary(x => x.ProductId);

            // التعامل مع الحالتين: الحذف والتعديل
            foreach (var oldItem in order.OrderItems)
            {
                if (!newItemsDict.TryGetValue(oldItem.ProductId, out var newItem))
                {
                    // ✅ الحالة 2: المنتج محذوف من الطلب => رجّع كميته
                    oldItem.Product.StockQuantity += oldItem.Quantity;
                }
                else
                {
                    // ✅ الحالة 1: تعديل الكمية
                    int diff = newItem.Quantity - oldItem.Quantity;

                    if (diff > 0)
                    {
                        // زوّد الكمية في الطلب => لازم يكون عندي كفاية في الستوك
                        if (diff > oldItem.Product.StockQuantity)
                            return new ApiResponse<string>($"لا يوجد مخزون كافٍ للمنتج {oldItem.Product.Name}");

                        oldItem.Product.StockQuantity -= diff;
                    }
                    else if (diff < 0)
                    {
                        // قلل الكمية => رجّع الفرق للمخزون
                        oldItem.Product.StockQuantity += -diff;
                    }
                }
            }

            // ✅ الحالة 3: إضافة منتج جديد بالكامل
            foreach (var newItem in dto.OrderItems)
            {
                if (!oldItemsDict.ContainsKey(newItem.ProductId))
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(newItem.ProductId);
                    if (product == null || product.IsDeleted)
                        return new ApiResponse<string>($"المنتج برقم {newItem.ProductId} غير موجود أو محذوف");

                    if (newItem.Quantity > product.StockQuantity)
                        return new ApiResponse<string>($"لا يوجد مخزون كافٍ للمنتج {product.Name}");

                    product.StockQuantity -= newItem.Quantity;
                }
            }

            // تحديث الطلب
            order.OrderItems.Clear();

            decimal totalOrderPrice = 0;
            int totalOrderQuantity = 0;

            foreach (var item in dto.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                decimal itemPrice = product.DiscountedPrice ?? product.Price;
                decimal itemTotal = itemPrice * item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotal
                });

                totalOrderPrice += itemTotal;
                totalOrderQuantity += item.Quantity;
            }

            order.TotalPrice = totalOrderPrice;
            order.TotalQuantity = totalOrderQuantity;
            order.PaymentStatus = dto.PaymentStatus;

            order.UserId = dto.UserId;
            order.IsGuestOrder = dto.IsGuestOrder;

            order.FirstName = dto.FirstName;
            order.LastName = dto.LastName;
            order.MobileNumber = dto.MobileNumber;
            order.Address = dto.Address;
            order.Governorate = dto.Governorate;
            order.PaymentMethod = dto.PaymentMethod;



            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم تحديث الطلب بنجاح");
        }


        public async Task<ApiResponse<string>> DeleteOrderAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return new ApiResponse<string>("لم يتم العثور على الطلب");

            // إعادة الكمية للمخزون
            foreach (var item in order.OrderItems)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            // حذف الطلب
            await _unitOfWork.Orders.DeleteByIdAsync(id);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم حذف الطلب بنجاح");
        }


        public async Task<ApiResponse<List<ReadOrderDto>>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetAll()
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var mappedOrders = _mapper.Map<List<ReadOrderDto>>(orders);

            return new ApiResponse<List<ReadOrderDto>>(mappedOrders);
        }

        public async Task<ApiResponse<OrderSummaryDto>> GetOrderSummaryAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);



            if (order == null)
                return new ApiResponse<OrderSummaryDto>("لم يتم العثور على الطلب");

            var summary = new OrderSummaryDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalItems = order.TotalQuantity,
                TotalPrice = order.TotalPrice
            };

            return new ApiResponse<OrderSummaryDto>(summary);
        }


        //   the total number of orders
        public async Task<ApiResponse<int>> GetTotalNumberOfOrdersAsync()
        {
            var totalOrders = await _unitOfWork.Orders.GetAll()
                .Where(o => !o.IsDeleted)
                .CountAsync();

            return new ApiResponse<int>(totalOrders);
        }
        //Cancel Unpaid Orders
        public async Task<ApiResponse<string>> CancelUnpaidOrderAsync(int orderId)
        {
            // فتح transaction من EF Core
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();

            try
            {
                // استرجاع الطلب بتتبع وتمرير الـ OrderItems والمنتجات المرتبطة
                var order = await _unitOfWork.Orders
                    .GetAll(tracking: true)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

                if (order == null)
                    return new ApiResponse<string>("لم يتم العثور على الطلب");

                if (order.PaymentStatus == PaymentStatus.Paid)
                    return new ApiResponse<string>("تم دفع الطلب بالفعل، لا حاجة للإلغاء");

                // استرجاع الكمية للمنتجات
                foreach (var item in order.OrderItems)
                {
                    item.Product.StockQuantity += item.Quantity;
                    // لا حاجة لاستدعاء UpdateAsync لأن Product تم تتبعه تلقائيًا
                }

                // تحديث حالة الطلب
                order.PaymentStatus = PaymentStatus.Cancelled;
                order.IsDeleted = true;

                // حفظ التغييرات
                await _unitOfWork.CommitChangesAsync();

                // تأكيد الترانزاكشن
                await transaction.CommitAsync();

                return new ApiResponse<string>(null, $"تم إلغاء الطلب رقم {order.Id} بسبب عدم الدفع");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<string>($"خطأ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalPaidOrdersPriceAsync()
        {
            var totalPrice = await _unitOfWork.Orders.GetAll()
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.TotalPrice);

            return new ApiResponse<decimal>(totalPrice);
        }

        public async Task<ApiResponse<int>> GetPaidOrdersCountAsync()
        {
            var count = await _unitOfWork.Orders.GetAll()
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .CountAsync();

            return new ApiResponse<int>(count);
        }

        public async Task<ApiResponse<int>> GetPendingOrdersCountAsync()
        {
            var count = await _unitOfWork.Orders.GetAll()
                .Where(o => o.PaymentStatus == PaymentStatus.Pending)
                .CountAsync();

            return new ApiResponse<int>(count);
        }

        public async Task<ApiResponse<int>> GetFailedOrdersCountAsync()
        {
            var count = await _unitOfWork.Orders.GetAll()
                .Where(o => o.PaymentStatus == PaymentStatus.Cancelled)
                .CountAsync();

            return new ApiResponse<int>(count);
        }
    }
}
