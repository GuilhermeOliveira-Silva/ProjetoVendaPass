using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoVendaPass.Models
{
    public class Compra
    {
        public int Id { get; set; }

        // Relacionamento com ApplicationUser (cliente)
        [Required]
        public string ClienteId { get; set; } = string.Empty;

        [ForeignKey("ClienteId")]
        public ApplicationUser? Cliente { get; set; }

        // Relacionamento com Plano
        [Required]
        public int PlanoId { get; set; }

        [ForeignKey("PlanoId")]
        public Plano? Plano { get; set; }

        // Data da compra — preenchida automaticamente
        public DateTime DataCompra { get; set; } = DateTime.Now;

        // Valor pago no momento da compra
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorPago { get; set; }

        // Status da compra
        public StatusCompra Status { get; set; } = StatusCompra.Pendente;
    }
}