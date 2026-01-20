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
        if (!ModelState.IsValid)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            return View("Index", vm);
        }

        var url = vm.UseStripe ? "api/payments/process-stripe" : "api/payments/process-payment";
        try
        {
            await _api.PostAsync(url, new { OrderId = vm.OrderId }, ct);
        }
        catch (HttpRequestException)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return BadRequest(new { success = false, message = "فشل الدفع. تأكد أن OrderSystem.API يعمل وأن قاعدة البيانات/Stripe مُهيأين بشكل صحيح." });
            }
            ModelState.AddModelError(string.Empty, "فشل الدفع. تأكد أن OrderSystem.API يعمل وأن قاعدة البيانات/Stripe مُهيأين بشكل صحيح.");
            return View("Index", vm);
        }
        catch (UnauthorizedAccessException)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var loginUrl = Url.Action("Login", "Account", new { returnUrl = Url.Action("Index", "Payment") });
                return Unauthorized(new { success = false, redirect = loginUrl, message = "تم انتهاء الجلسة. سجل دخولك مرة أخرى." });
            }

            TempData["Success"] = "تم انتهاء الجلسة. سجل دخولك مرة أخرى.";
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Payment") });
        }

        // Success processing payment
        var redirectUrl = Url.Action("Details", "Orders", new { id = vm.OrderId });
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Ok(new { success = true, message = "تمت عملية الدفع بنجاح", redirect = redirectUrl });
        }

        TempData["Success"] = "تمت عملية الدفع بنجاح";
        return RedirectToAction("Details", "Orders", new { id = vm.OrderId });
    }

    public sealed class PaymentVm
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        public bool UseStripe { get; set; } = false;
    }
}