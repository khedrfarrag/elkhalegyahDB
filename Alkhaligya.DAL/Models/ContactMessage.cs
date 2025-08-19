using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? User_type { get; set; }
        public string? Service_type { get; set; }
        public string? Government { get; set; }
        public string? Address { get; set; }

        // إذا كان المستخدم مسجلاً، يتم ربط الرسالة بحسابه
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
