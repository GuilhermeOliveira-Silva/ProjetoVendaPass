using System.ComponentModel.DataAnnotations;

namespace ProjetoVendaPass.Models
{
    public class Plano
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome pode ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, 99999.99, ErrorMessage = "Preço deve ser entre R$ 0,01 e R$ 99.999,99")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Quantidade de moedas é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int QuantidadeMoeda { get; set; }

        public bool Ativo { get; set; }
    }
}