using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "User")]
public class PaymentController : Controller
{
    private readonly ApiClient _api;

    public PaymentController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PaymentVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PaymentVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Index", vm);

        var url = vm.UseStripe ? "api/payments/process-stripe" : "api/payments/process-payment";
        await _api.PostAsync(url, new { OrderId = vm.OrderId }, ct);

        TempData["Success"] = "تمت عملية الدفع بنجاح";
        return RedirectToAction("Details", "Orders", new { id = vm.OrderId });
    }

    public sealed class PaymentVm
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        public bool UseStripe { get; set; } = true;
    }
}