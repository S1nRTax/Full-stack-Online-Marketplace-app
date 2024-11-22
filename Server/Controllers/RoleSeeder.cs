using Microsoft.AspNetCore.Identity;
using Server.Enums;
using System.Threading.Tasks;

public static class RoleSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        foreach (UserRoles role in Enum.GetValues(typeof(UserRoles)))
        {
            // Check if the role exists, if not, create it
            if (!await roleManager.RoleExistsAsync(role.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(role.ToString()));
            }
        }
    }
}
