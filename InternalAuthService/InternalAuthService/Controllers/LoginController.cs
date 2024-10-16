using InternalAuthService.Models;
using InternalAuthService.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace InternalAuthService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController(AppDbContext db, IMemoryCache cache, IConfiguration config) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        private readonly IMemoryCache _cache = cache;

        private readonly TimeSpan _loginExpiration = TimeSpan.FromMinutes(
            int.Parse(config["App:LoginExpirationInMinutes"]!)
            );

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == model.Username || x.Email == model.Username);

            if (user == null)
                return Unauthorized();

            if (user.PasswordHash != model.Password)
                return Unauthorized();

            var token = GenerateToken();

            _cache.Set(token, user, new MemoryCacheEntryOptions { SlidingExpiration = _loginExpiration});

            return Ok(token);
        }

        [HttpPost]
        public IActionResult Logout(string token)
        {
            _cache.Remove(token);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            user.Id = Guid.NewGuid();

            if (await _db.Users.AnyAsync(x => x.Email == user.Email))
                return BadRequest("email");

            if (await _db.Users.AnyAsync(x => x.Login == user.Login))
                return BadRequest("login");

            await _db.Users.AddAsync(user);

            await _db.SaveChangesAsync();

            return await Login(new LoginModel { Username = user.Login, Password = user.PasswordHash });
        }

        private static string GenerateToken()
        {
            int length = 128;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[Random.Shared.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
