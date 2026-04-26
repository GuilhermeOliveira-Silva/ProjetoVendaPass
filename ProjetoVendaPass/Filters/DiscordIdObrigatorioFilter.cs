using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjetoVendaPass.Models;

namespace ProjetoVendaPass.Filters
{
    public class DiscordIdObrigatorioFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscordIdObrigatorioFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var usuario = await _userManager.GetUserAsync(user);

                if (usuario != null && string.IsNullOrEmpty(usuario.DiscordId))
                {
                    var path = context.HttpContext.Request.Path.Value?.ToLower() ?? "";

                    // Libera TUDO que contém identity, conta, logout
                    var liberado =
                        path.Contains("/identity/") ||
                        path.Contains("/conta/") ||
                        path.Contains("/logout");

                    if (!liberado)
                    {
                        context.Result = new RedirectToActionResult("Index", "Conta", null);
                        return;
                    }
                }
            }

            await next();
        }
    }
}