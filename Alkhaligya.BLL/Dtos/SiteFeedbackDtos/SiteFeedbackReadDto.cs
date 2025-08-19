using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.SiteFeedbackDtos
{
    public class SiteFeedbackReadDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } 
        public DateTime CreatedAt { get; set; } 

    }
}
