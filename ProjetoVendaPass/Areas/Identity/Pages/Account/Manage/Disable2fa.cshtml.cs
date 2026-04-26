#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjetoVendaPass.Models;

namespace ProjetoVendaPass.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<ApplicationUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
                throw new InvalidOperationException("Não é possível desativar o 2FA pois ele não está habilitado.");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            var resultado = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!resultado.Succeeded)
                throw new InvalidOperationException("Erro inesperado ao desativar o 2FA.");

            _logger.LogInformation("Usuário com ID '{UserId}' desativou o 2FA.", _userManager.GetUserId(User));
            StatusMessage = "O 2FA foi desativado. Você pode reativá-lo configurando um aplicativo autenticador.";

            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}