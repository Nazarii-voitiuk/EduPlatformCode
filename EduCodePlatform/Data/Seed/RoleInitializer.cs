using EduCodePlatform.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EduCodePlatform.Data
{
    public static class RoleInitializer
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<Models.Identity.ApplicationUser> userManager)
        {
            // 1) Створити роль "Admin", якщо нема
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 2) Створити роль "User", якщо нема
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // 3) Перевіримо, чи є вже користувач-адмін
            var adminEmail = "admin@myapp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = "admin@myapp.com",
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(user, "Admin123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

        }
    }
}
