// Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MimeKit;
using SecureAPI_JWT.Helpers;
using SecureAPI_JWT.Models;

namespace SecureAPI_JWT.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel email);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger; // Ajout du logger

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailModel email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email.To));
            message.Subject = email.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                _logger.LogInformation("[EMAIL] Connexion au serveur SMTP {Server}:{Port}", _emailSettings.SmtpServer, _emailSettings.SmtpPort);
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("[EMAIL] Authentification avec {SenderEmail}", _emailSettings.SenderEmail);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);

                _logger.LogInformation("[EMAIL] Envoi du message à {To}", email.To);
                await client.SendAsync(message);

                _logger.LogInformation("[EMAIL] Déconnexion du serveur SMTP");
                await client.DisconnectAsync(true);

                _logger.LogInformation("[EMAIL] Email envoyé avec succès à {To}", email.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EMAIL ERROR] Erreur lors de l'envoi à {To}", email.To);
                if (ex.InnerException != null)
                {
                    _logger.LogError("[EMAIL ERROR] InnerException: {InnerMessage}", ex.InnerException.Message);
                }
                throw; // optionnel : relancer pour que l'appelant sache qu'il y a une erreur
            }
        }
    }
}
