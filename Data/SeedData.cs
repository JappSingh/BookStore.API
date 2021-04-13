using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookStore.API.Data
{
    public enum Role
    {
        Unassigned,
        Administrator,
        Customer
    }

    public static class SeedData
    {
        // Called in Startup to seed roles and users in the Identity DB Tables: dbo.AspNetRoles, dbo.AspNetUsers
        // Ensure 'update-database' cmd is run in Package Manager Console bfore that to create the Identity Tables in the DB.
        public async static Task Seed(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);            
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            await SeedUsers(userManager, "admin", "admin@bookstore.com", 
                "P@ssword0", Role.Administrator);
            await SeedUsers(userManager, "customer1", "customer1@gmail.com",
                "P@ssword1", Role.Customer);
            await SeedUsers(userManager, "customer2", "customer2@gmail.com",
                "P@ssword2", Role.Customer);
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager, 
            string userName, string email, string password, Role role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = userName,
                    Email = email
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.ToString());
                }
            }
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager, "Administrator");
            await SeedRoles(roleManager, "Customer");
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole
                {
                    Name = roleName
                };
                await roleManager.CreateAsync(role);
            }
        }

    }
}
