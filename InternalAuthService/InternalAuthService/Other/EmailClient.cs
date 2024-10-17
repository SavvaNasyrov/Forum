using InternalAuthService.Models;
using System.Net;
using System.Net.Mail;

namespace InternalAuthService.Other
{
    public class EmailClient
    {
        private readonly SmtpClient _client;

        private readonly int _port;

        private readonly string _smtpAddress;

        private readonly string _senderAddress;

        private readonly string _password;

        public EmailClient(IConfiguration config)
        {
            _port = int.Parse(config["App:Email:Port"]!);
            _smtpAddress = config["App:Email:Smtp"]!;
            _senderAddress = config["Email:Username"]!;
            _password = config["Email:Password"]!;
            _client = new SmtpClient(_smtpAddress, _port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_senderAddress, _password),
                Timeout = 100000
            };
        }

        public async Task SendEmail(string to)
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress(_senderAddress);
            mail.To.Add(to);
            mail.Subject = "Восстановление пароля";
            mail.Body = "Privet";

            await _client.SendMailAsync(mail);
        }
    }
}
