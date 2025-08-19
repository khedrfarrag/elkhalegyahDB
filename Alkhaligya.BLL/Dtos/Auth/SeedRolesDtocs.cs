using Alkhaligya.DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.Auth 
{

    public class SeedRolesDtocs
    {
        public static async Task SeedRoles(RoleManager<CustomRole> roleManager)
        {
            var roles = new List<string> { Roles.SuperAdmin, Roles.Admin, Roles.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new CustomRole { Name = role });
                }
            }
        }





        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<CustomRole> roleManager)
        {
            var superAdminEmail = "jokerfager42@gmail.com";

            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdmin == null)
            {
                var newSuperAdmin = new Admin
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    IsConfirmed = true,
                    FirstName = "Super",
                    LastName = "Admin",
                    City = GovernoratesEnum.Cairo
                };

                var result = await userManager.CreateAsync(newSuperAdmin, "Joker9900@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSuperAdmin, Roles.SuperAdmin);
                }
            }
        }
    }

     public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string SuperAdmin = "SuperAdmin";
    }
}
        
