#region Usings
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Data.Seeders
{
    #region Identity Seeder
    /// <summary>
    /// Provides seeding functionality for identity security roles and base administrative users.
    /// </summary>
    public static class IdentityDbSeeder
    {
        #region Seed Methods
        /// <summary>
        /// Seeds the essential application roles (SuperUser, Admin, Cashier, Waiter) and the default administrative accounts.
        /// </summary>
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = ["SuperUser", "Admin", "Cashier", "Waiter"];

            foreach (string role in roles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            /// Provisions the unassailable SuperUser account for Matheus Delmondes
            await ProvisionUserAsync(userManager, "contato@delmondescodelabs.dpdns.org", "Delmondes@2026", "SuperUser");

            /// Provisions temporary presentation accounts
            await ProvisionUserAsync(userManager, "admin@delmondescodelabs.dpdns.org", "Adega@2026", "Admin");
            await ProvisionUserAsync(userManager, "caixa@delmondescodelabs.dpdns.org", "Adega@2026", "Cashier");
            await ProvisionUserAsync(userManager, "garcom@delmondescodelabs.dpdns.org", "Adega@2026", "Waiter");
        }

        /// <summary>
        /// Helper method to provision users reliably.
        /// </summary>
        private static async Task ProvisionUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ApplicationUser newUser = new ApplicationUser
                {
                    UserName = email.Split("@", StringSplitOptions.TrimEntries)[0].ApplyCase(LetterCasing.Title),
                    Email = email,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }
        }
        #endregion Seed Methods
    }
    #endregion
}