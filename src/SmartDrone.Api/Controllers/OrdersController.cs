using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Orders;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
[Authorize]
public class OrdersController(
    IOrderService orders,
    IAppDbContext db) : ControllerBase
{
    private Guid? CurrentCustomerId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;
        if (customerId is null)
            return Unauthorized(new { message = "Không xác định được tài khoản." });

        try
        {
            var order = await orders.CreateAsync(customerId.Value, request, ct);
            return Created($"/api/v1/orders/{order.Id}", order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        var customerId = CurrentCustomerId;
        if (customerId is null)
            return Unauthorized(new { message = "Không xác định được tài khoản." });

        var result = await db.DeliveryOrders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId.Value)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderDto(
                o.Id,
                o.TrackingCode,
                o.CustomerId,
                o.PackageId,
                o.OriginStationId,
                o.DestinationStationId,
                o.Status,
                o.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(result);
    }
}
