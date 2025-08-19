using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
//using Alkhaligya.DAL.Migrations;
using Alkhaligya.DAL.Models;
using static Alkhaligya.BLL.Dtos.Order.AddOrderDto;

namespace Alkhaligya.BLL.Services.OrderServices
{
    public interface IOrderService
    {
        Task<ApiResponse<List<ReadOrderDto>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<ReadOrderDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<string>> AddOrderAsync(AddOrderDto addOrderDto);
        Task<ApiResponse<string>> UpdateOrderAsync(UpdateOrderDto updateOrderDto  , int id);
        Task<ApiResponse<string>> DeleteOrderAsync(int id);
        Task<ApiResponse<List<ReadOrderDto>>> GetUserOrdersAsync(string userId);
        Task<ApiResponse<OrderSummaryDto>> GetOrderSummaryAsync(int orderId);
        Task<ApiResponse<int>> GetTotalNumberOfOrdersAsync();
        Task<ApiResponse<decimal>> GetTotalPaidOrdersPriceAsync();
        Task<ApiResponse<int>> GetPaidOrdersCountAsync();
        Task<ApiResponse<int>> GetPendingOrdersCountAsync();
        Task<ApiResponse<int>> GetFailedOrdersCountAsync();
        Task<ApiResponse<string>> CancelUnpaidOrderAsync(int orderId);
    }

}
