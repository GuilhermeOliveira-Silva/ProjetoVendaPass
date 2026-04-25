using Microsoft.AspNetCore.Identity;

namespace ProjetoVendaPass.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public string? DiscordId { get; set; } 
    }
}
