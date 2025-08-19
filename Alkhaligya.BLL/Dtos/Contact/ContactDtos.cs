using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Contact
{

    public class CreateContactMessageDto
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string? UserId { get; set; }
        public string? User_type { get; set; }
        public string? Service_type { get; set; }
        public string? Government { get; set; }
        public string? Address { get; set; }
    }


    public class ReadContactMessageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public string? User_type { get; set; }
        public string? Service_type { get; set; }
        public string? Government { get; set; }
        public string? Address { get; set; }
    }


    public class UpdateContactMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string? User_type { get; set; }
        public string? Service_type { get; set; }
        public string? Government { get; set; }
        public string? Address { get; set; }
    }


}
