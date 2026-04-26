using System.ComponentModel.DataAnnotations;

namespace ProjetoVendaPass.Models.ViewModels
{
    public class MinhaContaViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome pode ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        // Somente leitura — não pode ser editado
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "Discord ID pode ter no máximo 50 caracteres")]
        public string? DiscordId { get; set; }
    }
}