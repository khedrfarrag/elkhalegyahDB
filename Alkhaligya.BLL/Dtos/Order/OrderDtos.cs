using Alkhaligya.DAL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Order
{
        public class AddOrderDto
    {


        public string? UserId { get; set; }

        public bool IsGuestOrder { get; set; }

        // بيانات الفورم
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم الأول يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(50, ErrorMessage = "اسم العائلة يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "رقم الهاتف يجب أن يتكون من 11 رقمًا فقط")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Required(ErrorMessage = "المحافظه مطلوبة")]
        public GovernoratesEnum Governorate { get; set; }
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.CashOnDelivery;

        public List<OrderAddItemDto> OrderItems { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    }

    public class AddOrderDto2
    {

      

        public bool IsGuestOrder { get; set; }

        // بيانات الفورم
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم الأول يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(50, ErrorMessage = "اسم العائلة يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "رقم الهاتف يجب أن يتكون من 11 رقمًا فقط")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Required(ErrorMessage = "المحافظه مطلوبة")]
        public GovernoratesEnum Governorate { get; set; }
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.CashOnDelivery;



    }


    public class UpdateOrderDto
        {
            public string? UserId { get; set; }
            public bool IsGuestOrder { get; set; } = false;
  

        // بيانات الفورم
        public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MobileNumber { get; set; }
            public string Address { get; set; }
            public GovernoratesEnum Governorate { get; set; }
            public PaymentMethodEnum PaymentMethod { get; set; }

        public List<OrderAddItemDto> OrderItems { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
        }



        public class ReadOrderDto
        {
            public int Id { get; set; }
            public string? UserId { get; set; }
            public bool IsGuestOrder { get; set; }

            public DateTime OrderDate { get; set; }

            public decimal TotalPrice { get; set; }
            public int TotalQuantity { get; set; }

            public string PaymentStatus { get; set; }

            // بيانات الفورم
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MobileNumber { get; set; }
            public string Address { get; set; }
            public GovernoratesEnum Governorate { get; set; }
            public PaymentMethodEnum PaymentMethod { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }
        }



        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice => Price * Quantity;
        }

        public class OrderSummaryDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public int TotalItems { get; set; }
            public decimal TotalPrice { get; set; }
        }

    public class OrderAddItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }





}
