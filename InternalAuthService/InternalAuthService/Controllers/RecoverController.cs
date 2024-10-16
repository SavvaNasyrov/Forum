using InternalAuthService.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace InternalAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecoverController(AppDbContext db, IConfiguration config) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        private readonly int Port = int.Parse(config["App:Email:Port"]!);

        private readonly string SmtpAddress = config["App:Email:Smtp"]!;

        private readonly string SenderAddress = config["Email:Username"]!;

        private readonly string Password = config["Email:Password"]!;

        [HttpPost]
        public async Task<IActionResult> RecoverAccount(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == username || x.Email == username);

            if (user == null)
                return NotFound("user");

            using var mail = new MailMessage();
            mail.From = new MailAddress(SenderAddress);
            mail.To.Add(user.Email);
            mail.Subject = "Восстановление пароля";
            mail.Body = "Privet";
            mail.IsBodyHtml = true; // Установите true для HTML-содержимого

            using var smtp = new SmtpClient(SmtpAddress, Port);
            smtp.Credentials = new NetworkCredential(SenderAddress, Password);
            smtp.EnableSsl = true;
            smtp.Send(mail);

            return Ok("sent");
        }
    }
}
