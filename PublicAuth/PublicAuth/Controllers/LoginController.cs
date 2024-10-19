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

        [HttpGet("Check")]
        public async Task<ActionResult> CheckAsync()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out var token))
            {
                var resp = await _httpClient.GetAsync($"http://internalauthservice:8080/api/Check?token={token}");

                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("CheckResult", await resp.Content.ReadAsStringAsync());
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return View("CheckResult", "Unauthorized");
                }
                else
                {
                    return View("CheckResult", "Fail");
                }
            }
            return View("CheckResult", "Unauthorized");
        }

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

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out var token))
            {
                Response.Cookies.Delete("AuthToken");

                var resp = await _httpClient.PostAsync(
                    $"http://internalauthservice:8080/api/Login/Logout?token={token}",
                    new StringContent(string.Empty));

                if (!resp.IsSuccessStatusCode)
                    return View("SomethingWentWrong", "Our service now unavailable"); 
            }

            return View("Success");
        }
    }
}
