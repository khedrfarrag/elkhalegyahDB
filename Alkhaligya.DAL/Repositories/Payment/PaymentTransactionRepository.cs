using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models.PayMob;
using Alkhaligya.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories.Payment
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction , int>, IPaymentTransactionRepository
    {
        private readonly AlkhligyaContext _context;

        public PaymentTransactionRepository(AlkhligyaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaymentTransaction> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(pt => pt.TransactionId == transactionId && !pt.IsDeleted);
        }

        public async Task<PaymentTransaction> GetByPaymobTransactionIdAsync(string paymobTransactionId)
        {
            return await _context.Set<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.PaymobTransactionId == paymobTransactionId && !t.IsDeleted);
        }

        public async Task<PaymentTransaction> GetByOrderIdAsync(string orderId)
        {
            if (!int.TryParse(orderId, out int orderIdInt))
            {
                return null; 
            }

            return await _context.Set<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.OrderId == orderIdInt && !t.IsDeleted);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetPendingTransactionsByOrderIdAsync(int orderId)
        {
            return await _context.PaymentTransactions
                .Where(t => t.OrderId == orderId && t.Status == "Pending" && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetExpiredPendingTransactionsAsync(DateTime cutoffTime)
        {
            return await _context.PaymentTransactions
                .Where(t => t.Status == "Pending" && t.CreatedAt < cutoffTime && !t.IsDeleted)
                .ToListAsync();
        }
    }
}
