using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using ProjetoVendaPass.Services;

namespace ProjetoVendaPass.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Senha),
                EnableSsl = true
            };

            var mensagem = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderNome),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mensagem.To.Add(email);

            await client.SendMailAsync(mensagem);
        }
    }
}