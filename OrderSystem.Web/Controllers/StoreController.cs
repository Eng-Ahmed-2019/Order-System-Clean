using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Web.Controllers;

public class StoreController : Controller
{
    private readonly ApiClient _api;

    public StoreController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Store") });

        var categories = await _api.GetAsync<List<Category>>("api/categories/GetAllCategories", ct) ?? [];
        var subCategories = await _api.GetAsync<List<SubCategory>>("api/subcategories/Get-All", ct) ?? [];
        var products = await _api.GetAsync<List<Product>>("api/products/Get-All-Products", ct) ?? [];

        return View(new StoreIndexVm
        {
            Categories = categories,
            SubCategories = subCategories,
            Products = products
        });
    }

    public sealed class StoreIndexVm
    {
        public List<Category> Categories { get; set; } = [];
        public List<SubCategory> SubCategories { get; set; } = [];
        public List<Product> Products { get; set; } = [];
    }
}