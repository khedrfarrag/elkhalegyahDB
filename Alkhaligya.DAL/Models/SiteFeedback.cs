using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class SiteFeedback
    {
        public int Id { get; set; }
        public string UserId { get; set; } // المستخدم الذي أنشأ التقييم
        public int Rating { get; set; } // التقييم من 1 إلى 5
        public string Comment { get; set; } // التعليق
        public DateTime CreatedAt { get; set; } 
        public bool IsDeleted { get; set; } = false; // Soft Delete

        public User User { get; set; }
    }
}
