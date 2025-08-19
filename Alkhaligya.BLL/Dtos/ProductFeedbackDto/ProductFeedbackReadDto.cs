using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.ProductFeedbackDto
{
    public class ProductFeedbackReadDto
    {
        public int  FeedBackId { get; set; }

        public string UserId { get; set; }

        public int Rate { get; set; } // من 1 إلى 5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
