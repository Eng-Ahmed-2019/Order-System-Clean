using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Domain.Entities;
using OrderSystem.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCategoriesController : Controller
{
    private readonly ApiClient _api;

    public AdminCategoriesController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.GetAsync<List<Category>>("api/categories/GetAllCategories", ct) ?? [];
        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new CategoryEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.PostAsync("api/categories/create-category", new { vm.Name, vm.Description }, ct);
        TempData["Success"] = "تم إنشاء القسم";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var items = await _api.GetAsync<List<Category>>("api/categories/GetAllCategories", ct) ?? [];
        var current = items.FirstOrDefault(x => x.Id == id);
        if (current == null) return NotFound();
        return View(new CategoryEditVm { Id = current.Id, Name = current.Name, Description = current.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _api.PutAsync("api/categories/Update-Category", new { vm.Id, vm.Name, vm.Description }, ct);
        TempData["Success"] = "تم تعديل القسم";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _api.DeleteAsync("api/categories/Delete-Category", new { CategoryId = id }, ct);
        TempData["Success"] = "تم حذف القسم";
        return RedirectToAction(nameof(Index));
    }
}