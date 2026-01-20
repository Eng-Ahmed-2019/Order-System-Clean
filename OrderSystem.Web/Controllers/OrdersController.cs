using Microsoft.AspNetCore.Mvc;
using OrderSystem.Web.Services;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace OrderSystem.Web.Controllers;

[Authorize(Roles = "User")]
public class OrdersController : Controller
{
    private readonly ApiClient _api;

    public OrdersController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var order = await _api.GetAsync<OrderResponseDto>($"api/orders/{id}", ct);
        return View(order);
    }
}