using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Infraestructure.Identity.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAsync(UserManager<User> userManager)
        {
            // Usuario Admin
            await CreateUserIfNotExists(userManager, new User
            {
                FirstName = "Admin",
                LastName = "System",
                IdentificationNumber = "00000000001",
                Email = "admin@artemisbanking.com",
                UserName = "admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            }, "Admin123!", Roles.Admin.ToString());

            // Usuario Cajero
            await CreateUserIfNotExists(userManager, new User
            {
                FirstName = "John",
                LastName = "Teller",
                IdentificationNumber = "00000000002",
                Email = "teller@artemisbanking.com",
                UserName = "teller",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            }, "Teller123!", Roles.Teller.ToString());

            // Usuario Cliente
            await CreateUserIfNotExists(userManager, new User
            {
                FirstName = "Jane",
                LastName = "Client",
                IdentificationNumber = "00000000003",
                Email = "client@artemisbanking.com",
                UserName = "client",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            }, "Client123!", Roles.Client.ToString());

            // Usuario Comercio
            await CreateUserIfNotExists(userManager, new User
            {
                FirstName = "Merchant",
                LastName = "Store",
                IdentificationNumber = "00000000004",
                Email = "merchant@artemisbanking.com",
                UserName = "merchant",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            }, "Merchant123!", Roles.Merchant.ToString());
        }

        private static async Task CreateUserIfNotExists(
            UserManager<User> userManager,
            User user,
            string password,
            string role)
        {
            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}
