using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace eBookStore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _host = _configuration["EmailSettings:SmtpHost"];
            _port = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);
            _userName = _configuration["EmailSettings:SmtpUsername"];
            _password = _configuration["EmailSettings:SmtpPassword"];
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromName = _configuration["EmailSettings:FromName"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));

            using (var client = new SmtpClient(_host, _port))
            {
                client.EnableSsl = _enableSsl;
                client.Credentials = new NetworkCredential(_userName, _password);

                await client.SendMailAsync(message);
            }
        }
    }
}
