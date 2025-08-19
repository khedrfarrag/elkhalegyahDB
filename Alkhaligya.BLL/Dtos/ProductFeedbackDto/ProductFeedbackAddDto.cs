using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.ProductFeedbackDto
{
    public class ProductFeedbackAddDto
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }

    }
}
