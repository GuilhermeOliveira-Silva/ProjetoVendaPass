using Microsoft.AspNetCore.Identity;
using ProjetoVendaPass.Models;

namespace ProjetoVendaPass.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            //Criar as roles
            string[] roles = { "Admin", "Cliente" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //Criar o usuário admin padrão
            string adminEmail = "admin@vendapass.com";
            string adminSenha = "Admin@123456";

            var adminExistente = await userManager.FindByEmailAsync(adminEmail);

            if (adminExistente == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "Administrador",
                    EmailConfirmed = true // evita precisar confirmar email
                };

                var resultado = await userManager.CreateAsync(admin, adminSenha);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}