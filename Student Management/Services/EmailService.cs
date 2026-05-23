using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Student_Management.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Student Management";
        public bool UseStartTls { get; set; } = true;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _settings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using var client = new SmtpClient();
            var socketOption = _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(_settings.Host, _settings.Port, socketOption);
            if (!string.IsNullOrWhiteSpace(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Password reset email sent to {Recipient}", to);
        }
    }
}
