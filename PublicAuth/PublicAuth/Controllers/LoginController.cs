using Microsoft.AspNetCore.Mvc;

namespace PublicAuth.Controllers
{
    [Controller]
    [Route("[action]")]
    public class LoginController(HttpClient httpClient) : Controller
    {
        private readonly HttpClient _httpClient = httpClient;

        [HttpGet("/{returnTo?}")]
        public IActionResult Login(string? returnTo)
        {
            if (returnTo != null)
                TempData["returnTo"] = returnTo;
            return View();
        }

        [HttpPost("/")]
        public IActionResult Login()
        {

            if (TempData.TryGetValue("returnTo", out object? value) && value is string str && !string.IsNullOrEmpty(str))
                return RedirectPermanent(str);
            else
                return View();
        }
    }
}
