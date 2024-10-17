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
    public class RecoverController(AppDbContext db, EmailClient emailClient) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        private readonly EmailClient _emailClient = emailClient;

        [HttpPost]
        public async Task<IActionResult> RecoverAccount(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == username || x.Email == username);

            if (user == null)
                return NotFound("user");

            await _emailClient.SendEmail(user.Email);

            return Ok("sent");
        }
    }
}
