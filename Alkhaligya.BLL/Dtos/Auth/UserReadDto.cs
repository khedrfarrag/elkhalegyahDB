using Alkhaligya.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Auth
{
    public class UserReadDto
    {
        public string Id { get; set; }
       
        public string FirstName { get; set; }

       
        public string LastName { get; set; }


      
        public string Email { get; set; }
        public string ImageUrl { get; set; }

        public GovernoratesEnum City { get; set; }

        public string PhoneNumber { get; set; }

    }
}
