using Microsoft.AspNetCore.Mvc;
using PublicAuth.Models;
using System.Net.Http;

namespace PublicAuth.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class RegisterController(HttpClient httpClient) : Controller
    {
        private readonly HttpClient _httpClient = httpClient;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resp = await _httpClient.PostAsJsonAsync("http://internalauthservice:8080/api/Login/Register", model);

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var content = await resp.Content.ReadAsStringAsync();

                switch (content)
                {
                    case "email":
                        ModelState.AddModelError("emailAlready in use", "Этот email уже используется");
                        return View(model);
                    case "login":
                        ModelState.AddModelError("loginAlready in use", "Этот логин уже используется");
                        return View(model);
                    case "password":
                        ModelState.AddModelError("password", "Пароли должны совпадать");
                        return View(model);
                    default:
                        return View("SomethingWentWrong", "Our service now unavailable");
                }
            }

            var token = await resp.Content.ReadAsStringAsync();

            if (token == null)
                return View("SomethingWentWrong", "Our service now unavailable");

            Response.Cookies.Append("AuthToken", token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return View("Success");
        }
    }
}
