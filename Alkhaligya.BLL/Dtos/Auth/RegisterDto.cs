using Alkhaligya.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Auth
{
       public class RegisterDto
        {
            [Required(ErrorMessage = "الاسم الأول مطلوب")]
            [StringLength(50, ErrorMessage = "الاسم الأول يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "اسم العائلة مطلوب")]
            [StringLength(50, ErrorMessage = "اسم العائلة يجب أن يكون بين 3 و 50 حرفًا", MinimumLength = 3)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
            [DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صالح")]
            public string Email { get; set; }

            [Required(ErrorMessage = "كلمة المرور مطلوبة")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف", MinimumLength = 6)]
            public string Password { get; set; }

            [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
            public string ConfirmedPassword { get; set; }

            [Required(ErrorMessage = "المحافظه مطلوبة")]
            public GovernoratesEnum City { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "رقم الهاتف يجب أن يتكون من 11 رقمًا فقط")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "الصورة مطلوبة")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

    }




}