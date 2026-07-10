using Microsoft.AspNetCore.Identity;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class SeedService
{
    public static async Task SeedAdmin(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var admins = await userManager.GetUsersInRoleAsync("Admin");
        if (!admins.Any())
        {
            var firstUser = userManager.Users.FirstOrDefault();
            if (firstUser != null)
            {
                await userManager.AddToRoleAsync(firstUser, "Admin");
            }
        }
    }
}