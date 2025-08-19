using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models
{
    public class CustomRole : IdentityRole
    {
        public bool IsDeleted { get; set; } = false;

     
    }
}
