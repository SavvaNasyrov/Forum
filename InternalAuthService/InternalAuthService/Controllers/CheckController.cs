using InternalAuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InternalAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController(IMemoryCache cache) : ControllerBase
    {
        private readonly IMemoryCache _cache = cache;

        [HttpGet]
        public IActionResult IsAuthorized(string token)
        {
            if (_cache.TryGetValue(token, out User? user))
            {
                return Ok(UserPublics.FromUser(user!));
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
