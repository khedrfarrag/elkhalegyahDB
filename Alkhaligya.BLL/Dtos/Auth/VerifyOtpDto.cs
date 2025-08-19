using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace Alkhaligya.BLL.Dtos.Auth
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [JsonPropertyName("otp")] 
        public string OTP { get; set; }

    }
}
