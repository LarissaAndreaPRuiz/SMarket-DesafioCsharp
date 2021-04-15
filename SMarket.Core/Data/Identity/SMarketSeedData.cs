using Microsoft.AspNetCore.Identity;
using SMarket.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMarket.Core.Data.Identity
{
    public static class SMarketSeedData
    {
        public static void SeedData(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (userManager.FindByNameAsync("admin@smarket.com.br").Result == null)
            {
                var user = new Users
                {
                    UserName = "admin@smarket.com.br",
                    Email = "admin@smarket.com.br",
                };

                var result = userManager.CreateAsync(user, "123Aa321!").Result;
            }
        }
    }
}
