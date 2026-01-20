using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "User")]
public class CheckoutController : Controller
{
    private readonly ApiClient _api;

    public CheckoutController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoCheckout(CancellationToken ct)
    {
        await _api.PostAsync<object>("api/checkout", new { }, ct);
        TempData["Success"] = "تم الـ Checkout. اختر طريقة الدفع.";
        return RedirectToAction("Index", "Payment");
    }
}