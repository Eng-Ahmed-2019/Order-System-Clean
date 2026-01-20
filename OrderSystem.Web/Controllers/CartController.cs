using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "User")]
public class CartController : Controller
{
    private readonly ApiClient _api;

    public CartController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.GetAsync<List<OrderItem>>("api/cart/Get-Items", ct) ?? [];
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity, CancellationToken ct)
    {
        await _api.PostAsync("api/cart/Add", new { ProductId = productId, Quantity = quantity }, ct);
        TempData["Success"] = "تمت إضافة المنتج للسلة";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, CancellationToken ct)
    {
        await _api.DeleteAsync("api/cart/Remove-Item", new { Id = id }, ct);
        TempData["Success"] = "تم حذف العنصر من السلة";
        return RedirectToAction(nameof(Index));
    }
}