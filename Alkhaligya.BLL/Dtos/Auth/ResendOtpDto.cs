using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Auth
{
    public class ResendOtpDto
    {

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public  bool isForResetPassword { get; set; } = false;
    }
}
