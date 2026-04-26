namespace ProjetoVendaPass.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderNome { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}