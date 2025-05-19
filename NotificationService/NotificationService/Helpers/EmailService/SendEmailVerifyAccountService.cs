using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using NotificationService.Helpers.EmailService.Templates;

namespace NotificationService.Helpers.EmailService
{
    public class SendEmailVerifyAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendEmailVerifyAccountService> _logger;
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public SendEmailVerifyAccountService(IConfiguration configuration, ILogger<SendEmailVerifyAccountService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Get settings with null checking
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? throw new ArgumentNullException("EmailSettings:SmtpServer", "SMTP server setting is missing");
            
            var portString = _configuration["EmailSettings:Port"] ?? throw new ArgumentNullException("EmailSettings:Port", "SMTP port setting is missing");
            if (!int.TryParse(portString, out var port))
            {
                throw new ArgumentException($"Invalid SMTP port: {portString}", "EmailSettings:Port");
            }
            _port = port;
            
            _senderEmail = _configuration["EmailSettings:SenderEmail"] ?? throw new ArgumentNullException("EmailSettings:SenderEmail", "Sender email setting is missing");
            _senderPassword = _configuration["EmailSettings:SenderPassword"] ?? throw new ArgumentNullException("EmailSettings:SenderPassword", "Sender password setting is missing");
        }

        public async Task<bool> SendAccountVerificationEmailAsync(UserImportedEventDataSingleData user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.PrivateEmail))
                {
                    _logger.LogError($"Cannot send verification email: Private email is empty for user {user.UserEmail}");
                    return false;
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "Account Notification Service"),
                    Subject = "Your Account Has Been Created",
                    IsBodyHtml = true,
                    Body = VerifyEmail.GenerateTemplate(user)
                };

                message.To.Add(new MailAddress(user.PrivateEmail));

                using var client = new SmtpClient(_smtpServer, _port)
                {
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                _logger.LogInformation($"Account verification email sent successfully to {user.PrivateEmail} for user {user.UserEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending verification email to {user.PrivateEmail} for user {user.UserEmail}");
                return false;
            }
        }
    }
}
