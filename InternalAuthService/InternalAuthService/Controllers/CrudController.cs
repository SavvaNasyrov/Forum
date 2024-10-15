using InternalAuthService.Models;
using InternalAuthService.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternalAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_db.Users);
        }

        [HttpPost]
        public IActionResult Add(User user)
        {
            user.Id = Guid.NewGuid();
            _db.Users.Add(user);
            _db.SaveChanges();
            return Ok(user);
        }
    }
}
