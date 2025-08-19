using Alkhaligya.DAL.Models.PayMob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3
    }

    public enum PaymentMethodEnum
    {
        CashOnDelivery = 1,
        CardOrWallet = 2 
    }

    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? SessionId { get; set; }

        // guest و user  علشان اقدر افرق بين 
        public bool IsGuestOrder { get; set; } 

        public DateTime OrderDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public decimal TotalPrice { get; set; }
        public int TotalQuantity { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; 
        public int? PaymentTransactionId { get; set; } 

        // بيانات الفورم 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public GovernoratesEnum Governorate { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }

        // Relations
        public User User { get; set; }
        public PaymentTransaction? PaymentTransaction { get; set; } // جديد
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}






