namespace ProjetoVendaPass.Models
{
    public class Plano
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeMoeda { get; set; }
        public bool Ativo { get; set; }
    }
}
