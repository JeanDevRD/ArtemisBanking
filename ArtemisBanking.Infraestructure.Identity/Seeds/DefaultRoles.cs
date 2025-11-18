using ArtemisBanking.Core.Domain.Common.Enum;
using Microsoft.AspNetCore.Identity;


namespace ArtemisBanking.Infraestructure.Identity.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
