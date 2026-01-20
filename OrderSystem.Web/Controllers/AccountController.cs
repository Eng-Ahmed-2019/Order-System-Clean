using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using System.IdentityModel.Tokens.Jwt;
using OrderSystem.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OrderSystem.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;

    public AccountController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        LoginApiEnvelope? resp;
        try
        {
            resp = await _api.PostAsync<LoginVm, LoginApiEnvelope>("api/auth/login", vm, ct);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "تعذر الاتصال بالـ API أو انقطع أثناء الطلب. تأكد أن OrderSystem.API يعمل وأن قاعدة البيانات جاهزة.");
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }
        if (resp?.Success != true || resp.Data == null || string.IsNullOrWhiteSpace(resp.Data.Token))
        {
            ModelState.AddModelError(string.Empty, "Login failed");
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        HttpContext.Session.SetString(AuthSessionKeys.AccessToken, resp.Data.Token);
        HttpContext.Session.SetString(AuthSessionKeys.ExpiresAtUtc, resp.Data.ExpiresAt.ToUniversalTime().ToString("O"));

        var principal = BuildPrincipalFromJwt(resp.Data.Token);
        await HttpContext.SignInAsync(
            "Cookies",
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = resp.Data.ExpiresAt.ToUniversalTime()
            });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Store");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.PostAsync("api/auth/register", vm, ct);
        TempData["Success"] = "تم التسجيل بنجاح. سجل دخولك الآن.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        try
        {
            await _api.PostAsync<object>("api/auth/log-out", new { }, ct);
        }
        catch { }

        HttpContext.Session.Remove(AuthSessionKeys.AccessToken);
        HttpContext.Session.Remove(AuthSessionKeys.ExpiresAtUtc);
        await HttpContext.SignOutAsync("Cookies");

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private static ClaimsPrincipal BuildPrincipalFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var claims = new List<Claim>();
        foreach (var c in jwt.Claims)
        {
            claims.Add(c);
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    private sealed class LoginApiEnvelope
    {
        public bool Success { get; set; }
        public LoginApiData? Data { get; set; }
    }

    private sealed class LoginApiData
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}