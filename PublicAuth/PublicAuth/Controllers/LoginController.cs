using Microsoft.AspNetCore.Mvc;
using PublicAuth.Models;
using System.Reflection;

namespace PublicAuth.Controllers
{
    [Controller]
    [Route("/")]
    public class LoginController(HttpClient httpClient) : Controller
    {
        private readonly HttpClient _httpClient = httpClient;

        [HttpGet]
        public IActionResult Login(string? returnTo)
        {
            if (returnTo != null)
                TempData["returnTo"] = returnTo;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resp = await _httpClient.PostAsJsonAsync("http://internalauthservice:8080/api/Login/Login", model);

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("unautorized", "Неправильный логин или пароль");
                return View(model);
            }

            if (!resp.IsSuccessStatusCode)
                return View("SomethingWentWrong", "Our service now unavailable");

            var token = await resp.Content.ReadAsStringAsync();

            if (token is null)
                return View("SomethingWentWrong", "Our service now unavailable");

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            if (TempData.TryGetValue("returnTo", out object? value) && value is string str && !string.IsNullOrEmpty(str))
                return RedirectPermanent(str);
            else
                return View("Success");
        }

        [HttpPost("Loguot")]
        public async Task<IActionResult> LogoutAsync()
        {
            var resp = await _httpClient.PostAsync(
                "http://internalauthservice:8080/api/Login/Logout",
                new StringContent(string.Empty));

            if (!resp.IsSuccessStatusCode)
                return View("SomethingWentWrong", "Our service now unavailable");

            return View();
        }
    }
}
