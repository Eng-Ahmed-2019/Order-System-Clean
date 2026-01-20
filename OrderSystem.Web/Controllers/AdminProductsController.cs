using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;
using OrderSystem.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new ProductEditVm();
        await PopulateSubCategoriesAsync(vm, ct);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSubCategoriesAsync(vm, ct);
            return View(vm);
        }

        try
        {
            await _api.PostAsync("api/products/create-product", new
            {
                vm.SubCategoryId,
                vm.Name,
                vm.Description,
                vm.Price,
                vm.Stock
            }, ct);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "فشل حفظ المنتج. تأكد من أن البيانات صحيحة (اسم غير مكرر في نفس التصنيف، سعر > 0، المخزون >= 0).");
            await PopulateSubCategoriesAsync(vm, ct);
            return View(vm);
        }

        TempData["Success"] = "تم إنشاء المنتج";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var items = await _api.GetAsync<List<Product>>("api/products/Get-All-Products", ct) ?? [];
        var current = items.FirstOrDefault(x => x.Id == id);
        if (current == null) return NotFound();
        var vm = new ProductEditVm
        {
            Id = current.Id,
            SubCategoryId = current.SubCategoryId,
            Name = current.Name,
            Description = current.Description,
            Price = current.Price,
            Stock = current.Stock
        };
        await PopulateSubCategoriesAsync(vm, ct);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSubCategoriesAsync(vm, ct);
            return View(vm);
        }

        try
        {
            await _api.PutAsync("api/products/Update-Product", new
            {
                vm.Id,
                vm.SubCategoryId,
                vm.Name,
                vm.Description,
                vm.Price,
                vm.Stock
            }, ct);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "فشل تعديل المنتج. تأكد من أن البيانات صحيحة (اسم غير مكرر في نفس التصنيف، سعر > 0، المخزون >= 0).");
            await PopulateSubCategoriesAsync(vm, ct);
            return View(vm);
        }

        TempData["Success"] = "تم تعديل المنتج";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _api.DeleteAsync("api/products/Delete-Product", new { ProductId = id }, ct);
            TempData["Success"] = "تم حذف المنتج";
        }
        catch (HttpRequestException ex)
        {
            var msg = string.IsNullOrWhiteSpace(ex.Message)
                ? "فشل حذف المنتج. حدث خطأ في الخادم."
                : ex.Message;
            TempData["Error"] = msg;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSubCategoriesAsync(ProductEditVm vm, CancellationToken ct)
    {
        var subs = await _api.GetAsync<List<SubCategory>>("api/subcategories/Get-All", ct) ?? [];
        vm.SubCategories = subs
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = s.Id == vm.SubCategoryId
            })
            .ToList();
    }
}