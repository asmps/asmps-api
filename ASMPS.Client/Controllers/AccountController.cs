using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ASMPS.Client.Models.Account;
using ASMPS.Client.Models.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;

namespace ASMPS.Client.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new
        {
            Login = model.Username,
            Password = model.Password
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("http://localhost:5190/api/Authorization", content);
        
        if (response.IsSuccessStatusCode)
        {
            var tokenJson = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(tokenJson);

            // Расшифровываем токен для получения информации о пользователе
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.ReadJwtToken(token.AccessToken);

            // Проверяем наличие клейма с ролью и значение этого клейма
            if (accessToken.Claims.FirstOrDefault(c => c.Type == "role" && c.Value == "Deanery") != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username), // Здесь может быть ваш логин
                    new Claim(ClaimTypes.Role, "Deanery") // Здесь может быть роль пользователя
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                
                // Храним токен в cookie
                Response.Cookies.Append("JWToken", token.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30) // Время хранения
                });
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Ошибка входа. Доступ запрещен.";
            return View(model);
        }

        ViewBag.ErrorMessage = "Ошибка входа. Проверьте правильность учетных данных.";
        return View(model);
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        Response.Cookies.Delete("JWToken");
        return RedirectToAction("Login");
    }
}