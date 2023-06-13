using Ecommerce.Data.Entities;
using Ecommerce.Data.Static;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedAdminAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder.ApplicationServices == null)
            {
                //log an error or throw an exception
                Console.WriteLine("Unsuccessful start of application");
                return;
            }
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                
                //SuperAdmin
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string superAdminEmail = "";

                var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
                if(superAdmin == null) 
                {
                    var newSuperAdmin = new ApplicationUser()
                    {
                        FullName = "",
                        UserName = superAdminEmail,
                        Email = superAdminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newSuperAdmin, "");
                    var passwordHasher = userManager.PasswordHasher;
                    var hashedPassword = passwordHasher.HashPassword(newSuperAdmin, "");
                    await userManager.UpdateAsync(newSuperAdmin);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newSuperAdmin, UserRoles.SuperAdmin);
                    }
                    else if (!result.Succeeded)
                    {
                        var errors = result.Errors;
                        foreach (var error in errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                    else
                    {
                        // admin already exists, proceed with adding to role
                        await userManager.AddToRoleAsync(newSuperAdmin, UserRoles.SuperAdmin);
                    }
                }
            }
        }
    }
}