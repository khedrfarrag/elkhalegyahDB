using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models.PayMob
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } // Transaction ID من Paymob
        public string?  PaymobTransactionId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP"; // افتراضي: جنيه مصري
        public string PaymentMethod { get; set; } // Card, Mobile Wallet, etc.
        public string Status { get; set; } // Pending, Success, Failed
        public DateTime CreatedAt { get; set; }
        public string PaymobOrderId { get; set; } // Order ID من Paymob
        public bool IsDeleted { get; set; } // للتوافق مع نمطك
    }
}
