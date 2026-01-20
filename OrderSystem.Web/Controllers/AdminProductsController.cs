using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;
using OrderSystem.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductsController : Controller
{
    private readonly ApiClient _api;

    public AdminProductsController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.GetAsync<List<Product>>("api/products/Get-All-Products", ct) ?? [];
        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new ProductEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.PostAsync("api/products/create-product", new
        {
            vm.SubCategoryId,
            vm.Name,
            vm.Description,
            vm.Price,
            vm.Stock
        }, ct);

        TempData["Success"] = "تم إنشاء المنتج";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var items = await _api.GetAsync<List<Product>>("api/products/Get-All-Products", ct) ?? [];
        var current = items.FirstOrDefault(x => x.Id == id);
        if (current == null) return NotFound();
        return View(new ProductEditVm
        {
            Id = current.Id,
            SubCategoryId = current.SubCategoryId,
            Name = current.Name,
            Description = current.Description,
            Price = current.Price,
            Stock = current.Stock
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.PutAsync("api/products/Update-Product", new
        {
            vm.Id,
            vm.SubCategoryId,
            vm.Name,
            vm.Description,
            vm.Price,
            vm.Stock
        }, ct);

        TempData["Success"] = "تم تعديل المنتج";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _api.DeleteAsync("api/products/Delete-Product", new { ProductId = id }, ct);
        TempData["Success"] = "تم حذف المنتج";
        return RedirectToAction(nameof(Index));
    }
}