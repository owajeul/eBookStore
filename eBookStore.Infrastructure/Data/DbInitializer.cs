using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Common.Utilily;
using eBookStore.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace eBookStore.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(AppConstant.Role_Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = AppConstant.Role_Customer });
                }

                if (!await _roleManager.RoleExistsAsync(AppConstant.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = AppConstant.Role_Admin });
                    await _userManager.CreateAsync(new ApplicationUser
                    {
                        Name = "Admin",
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com"
                    }, "zaq123");

                    var user = await _userManager.FindByEmailAsync("admin@gmail.com");

                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, AppConstant.Role_Admin);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
