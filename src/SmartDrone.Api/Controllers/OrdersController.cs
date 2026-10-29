using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Orders;
using SmartDrone.Domain.Enums;

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
    [Authorize(Roles = "Customer")]
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
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized(new
            {
                message = "Không xác định được tài khoản."
            });

        // Giới hạn tham số phân trang
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.DeliveryOrders.AsNoTracking();

        // Nhân viên được xem tất cả đơn hàng
        var isStaff = User.IsInRole("Dispatcher")
            || User.IsInRole("Manager")
            || User.IsInRole("Admin");

        if (!isStaff)
            query = query.Where(o => o.CustomerId == customerId.Value);

        // Lọc theo trạng thái
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus)
                || !Enum.IsDefined(parsedStatus))
            {
                return BadRequest(new
                {
                    message = "Trạng thái đơn hàng không hợp lệ."
                });
            }

            var statusValue = parsedStatus.ToString();
            query = query.Where(o => o.Status == statusValue);
        }

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .ThenByDescending(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummaryDto(
                o.Id,
                o.TrackingCode,
                o.Status,
                db.Stations
                    .Where(s => s.Id == o.OriginStationId)
                    .Select(s => s.Name)
                    .FirstOrDefault() ?? "",
                db.Stations
                    .Where(s => s.Id == o.DestinationStationId)
                    .Select(s => s.Name)
                    .FirstOrDefault() ?? "",
                o.EtaUtc,
                o.CreatedAtUtc
            ))
            .ToListAsync(ct);

        return Ok(new OrderPageDto(
            items,
            page,
            pageSize,
            totalItems
        ));
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized(new
            {
                message = "Không xác định được tài khoản."
            });


        var isStaff = User.IsInRole("Dispatcher")
            || User.IsInRole("Manager")
            || User.IsInRole("Admin");

        var order = await db.DeliveryOrders
            .AsNoTracking()
            .Where(o =>
                o.Id == id &&
                (isStaff || o.CustomerId == customerId.Value))

                    .Select(o => new OrderDto(
                o.Id,
                o.TrackingCode,
                o.CustomerId,
                o.PackageId,
                o.OriginStationId,
                o.DestinationStationId,
                o.Status,
                o.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        if (order is null)
            return NotFound(new
            {
                message = "Không tìm thấy đơn hàng."
            });

        return Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> Cancel(
        Guid id,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized(new
            {
                message = "Không xác định được tài khoản."
            });

        try
        {
            var result = await orders.CancelAsync(
                customerId.Value, id, ct);

            if (result is null)
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<OrderDto>> Update(
        Guid id,
        [FromBody] UpdateOrderRequest request,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized(new
            {
                message = "Không xác định được tài khoản."
            });

        try
        {
            var result = await orders.UpdateAsync(
                customerId.Value, id, request, ct);

            if (result is null)
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetHistory(
        Guid id,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized(new
            {
                message = "Không xác định được tài khoản."
            });

        

        var isStaff = User.IsInRole("Dispatcher")
            || User.IsInRole("Manager")
            || User.IsInRole("Admin");

        // Customer xem đơn của mình, nhân viên xem mọi đơn
        var orderExists = await db.DeliveryOrders
            .AnyAsync(o =>
                o.Id == id &&
                (isStaff || o.CustomerId == customerId.Value), ct);


        if (!orderExists)
            return NotFound(new
            {
                message = "Không tìm thấy đơn hàng."
            });

        // Lấy lịch sử trạng thái
        var history = await db.OrderStatusHistories
            .AsNoTracking()
            .Where(h => h.OrderId == id)
            .OrderBy(h => h.ChangedAtUtc)
            .ThenBy(h => h.Id)
            .Select(h => new OrderStatusHistoryDto(
                h.FromStatus,
                h.ToStatus,
                null,
                h.Note,
                h.ChangedAtUtc))
            .ToListAsync(ct);

        return Ok(history);
    }
}
