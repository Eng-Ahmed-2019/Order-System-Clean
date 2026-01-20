using System.Text.Json;
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

        try
        {
            await _api.PostAsync("api/categories/create-category", new { vm.Name, vm.Description }, ct);
        }
        catch (HttpRequestException ex)
        {
            var handled = false;
            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                try
                {
                    using var doc = JsonDocument.Parse(ex.Message);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in root.EnumerateArray())
                        {
                            var field = el.TryGetProperty("Field", out var f) ? f.GetString() : null;
                            var error = el.TryGetProperty("Error", out var e) ? e.GetString() : null;
                            if (string.IsNullOrWhiteSpace(field))
                                ModelState.AddModelError(string.Empty, error ?? "Validation error");
                            else
                                ModelState.AddModelError(field, error ?? "Validation error");
                        }
                        handled = true;
                    }
                    else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("Message", out var m))
                    {
                        ModelState.AddModelError(string.Empty, m.GetString() ?? "An error occurred");
                        handled = true;
                    }
                }
                catch { }
            }

            if (!handled)
            {
                ModelState.AddModelError(string.Empty, "فشل إنشاء القسم. تأكد من أن البيانات صحيحة.");
            }

            return View(vm);
        }

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

        try
        {
            await _api.PutAsync("api/categories/Update-Category", new { vm.Id, vm.Name, vm.Description }, ct);
        }
        catch (HttpRequestException ex)
        {
            var handled = false;
            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                try
                {
                    using var doc = JsonDocument.Parse(ex.Message);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in root.EnumerateArray())
                        {
                            var field = el.TryGetProperty("Field", out var f) ? f.GetString() : null;
                            var error = el.TryGetProperty("Error", out var e) ? e.GetString() : null;
                            if (string.IsNullOrWhiteSpace(field))
                                ModelState.AddModelError(string.Empty, error ?? "Validation error");
                            else
                                ModelState.AddModelError(field, error ?? "Validation error");
                        }
                        handled = true;
                    }
                    else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("Message", out var m))
                    {
                        ModelState.AddModelError(string.Empty, m.GetString() ?? "An error occurred");
                        handled = true;
                    }
                }
                catch { }
            }

            if (!handled)
            {
                ModelState.AddModelError(string.Empty, "فشل تعديل القسم. تأكد من أن البيانات صحيحة.");
            }

            return View(vm);
        }

        TempData["Success"] = "تم تعديل القسم";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _api.DeleteAsync("api/categories/Delete-Category", new { CategoryId = id }, ct);
            TempData["Success"] = "تم حذف القسم";
        }
        catch (HttpRequestException ex)
        {
            var msg = string.IsNullOrWhiteSpace(ex.Message)
                ? "فشل حذف القسم. حدث خطأ في الخادم."
                : ex.Message;
            TempData["Error"] = msg;
        }

        return RedirectToAction(nameof(Index));
    }
}