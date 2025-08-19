using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public enum GovernoratesEnum
    {
        Cairo = 1,            // القاهرة
        Giza = 2,             // الجيزة
        Alexandria = 3,       // الإسكندرية
        Dakahlia = 4,         // الدقهلية
        RedSea = 5,           // البحر الأحمر
        Beheira = 6,          // البحيرة
        Fayoum = 7,           // الفيوم
        Gharbia = 8,          // الغربية
        Ismailia = 9,         // الإسماعيلية
        Menoufia = 10,        // المنوفية
        Minya = 11,           // المنيا
        Qaliubiya = 12,       // القليوبية
        NewValley = 13,       // الوادي الجديد
        Suez = 14,            // السويس
        Aswan = 15,           // أسوان
        Assiut = 16,          // أسيوط
        BeniSuef = 17,        // بني سويف
        PortSaid = 18,        // بورسعيد
        Damietta = 19,        // دمياط
        Sharqia = 20,         // الشرقية
        SouthSinai = 21,      // جنوب سيناء
        KafrElSheikh = 22,    // كفر الشيخ
        Matrouh = 23,         // مطروح
        Luxor = 24,           // الأقصر
        Qena = 25,            // قنا
        NorthSinai = 26,      // شمال سيناء
        Sohag = 27            // سوهاج
    }

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public GovernoratesEnum City { get; set; }
        public string? ImageUrl { get; set; }

        public string? OTP { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public bool IsBanned { get; set; } = false;

        public bool IsConfirmed { get; set; } = false; // default false
    }


    public class User : ApplicationUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<ProductFeedback> ProductFeedbacks { get; set; } = new List<ProductFeedback>();
        public ICollection<SiteFeedback> SitetFeedbacks { get; set; } = new List<SiteFeedback>();
        public ICollection<ContactMessage> ContactMessages { get; set; } = new HashSet<ContactMessage>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
        public CartShop CartShop { get; set; }
    }

    public class Admin : ApplicationUser
    {
      

    }
}
