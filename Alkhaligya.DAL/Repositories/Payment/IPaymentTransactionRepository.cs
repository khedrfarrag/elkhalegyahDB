using Alkhaligya.DAL.Models.PayMob;
using Alkhaligya.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories.Payment
{
    public interface IPaymentTransactionRepository : IRepository<PaymentTransaction , int>
    {
        Task<PaymentTransaction> GetByTransactionIdAsync(string transactionId);
        Task<PaymentTransaction> GetByPaymobTransactionIdAsync(string paymobTransactionId);
        Task<PaymentTransaction> GetByOrderIdAsync(string OrderId);
        Task<IEnumerable<PaymentTransaction>> GetPendingTransactionsByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentTransaction>> GetExpiredPendingTransactionsAsync(DateTime cutoffTime);
    }
}
