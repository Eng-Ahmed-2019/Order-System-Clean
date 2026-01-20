using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;
using OrderSystem.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminSubCategoriesController : Controller
{
    private readonly ApiClient _api;

    public AdminSubCategoriesController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.GetAsync<List<SubCategory>>("api/subcategories/Get-All", ct) ?? [];
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new SubCategoryEditVm();
        await PopulateCategoriesAsync(vm, ct);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubCategoryEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(vm, ct);
            return View(vm);
        }

        await _api.PostAsync("api/subcategories/create-subcategory", new { vm.CategoryId, vm.Name, vm.Description }, ct);
        TempData["Success"] = "تم إنشاء التصنيف الفرعي";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var items = await _api.GetAsync<List<SubCategory>>("api/subcategories/Get-All", ct) ?? [];
        var current = items.FirstOrDefault(x => x.Id == id);
        if (current == null) return NotFound();
        var vm = new SubCategoryEditVm
        {
            Id = current.Id,
            CategoryId = current.CategoryId,
            Name = current.Name,
            Description = current.Description
        };
        await PopulateCategoriesAsync(vm, ct);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubCategoryEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(vm, ct);
            return View(vm);
        }

        await _api.PutAsync("api/subcategories/Update-SubCategory", new { vm.Id, vm.CategoryId, vm.Name, vm.Description }, ct);
        TempData["Success"] = "تم تعديل التصنيف الفرعي";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _api.DeleteAsync("api/subcategories/Delete-SubCategory", new { Id = id }, ct);
        TempData["Success"] = "تم حذف التصنيف الفرعي";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync(SubCategoryEditVm vm, CancellationToken ct)
    {
        var categories = await _api.GetAsync<List<Category>>("api/categories/GetAllCategories", ct) ?? [];
        vm.Categories = categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == vm.CategoryId
            })
            .ToList();
    }
}