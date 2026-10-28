
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Customers;
using SmartDrone.Domain.Common;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/v1/customers/me/addresses")]
[Authorize(Roles = Roles.Customer)]
public class CustomersController(IAppDbContext db) : ControllerBase
{
    private Guid? CurrentCustomerId
    {
        get
        {
            var value = User.FindFirstValue("sub");

            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    // GET: api/v1/customers/me/addresses
    [HttpGet]
    public async Task<IActionResult> GetMyAddresses(
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized();

        var addresses = await db.CustomerAddresses
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId.Value)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(a => new CustomerAddressDto(
                a.Id,
                a.Label,
                a.AddressLine,
                a.Latitude,
                a.Longitude,
                a.IsDefault,
                a.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(addresses);
    }

    // GET: api/v1/customers/me/addresses/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAddressById(
        Guid id,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized();

        var address = await db.CustomerAddresses
            .AsNoTracking()
            .Where(a => a.Id == id &&
                        a.CustomerId == customerId.Value)
            .Select(a => new CustomerAddressDto(
                a.Id,
                a.Label,
                a.AddressLine,
                a.Latitude,
                a.Longitude,
                a.IsDefault,
                a.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        if (address is null)
            return NotFound();

        return Ok(address);
    }

    // POST: api/v1/customers/me/addresses
    [HttpPost]
    public async Task<ActionResult<CustomerAddressDto>> CreateAddress(
        [FromBody] CustomerAddressRequest request,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized();

        // Không cho tạo thêm địa chỉ mặc định nếu đã có
        if (request.IsDefault)
        {
            var hasDefault = await db.CustomerAddresses
                .AnyAsync(a => a.CustomerId == customerId.Value
                            && a.IsDefault, ct);

            if (hasDefault)
                return Conflict(new
                {
                    message = "Bạn đã có địa chỉ mặc định."
                });
        }

        var address = new CustomerAddress
        {
            CustomerId = customerId.Value,
            Label = request.Label.Trim(),
            AddressLine = request.AddressLine.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = request.IsDefault,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.CustomerAddresses.Add(address);
        await db.SaveChangesAsync(ct);

        var result = new CustomerAddressDto(
            address.Id,
            address.Label,
            address.AddressLine,
            address.Latitude,
            address.Longitude,
            address.IsDefault,
            address.CreatedAtUtc
        );

        return CreatedAtAction(
            nameof(GetAddressById),
            new { id = address.Id },
            result
        );
    }

    // PUT: api/v1/customers/me/addresses/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] CustomerAddressRequest request,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized();

        // Chỉ tìm địa chỉ thuộc khách hàng đang đăng nhập
        var address = await db.CustomerAddresses
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.CustomerId == customerId.Value, ct);

        if (address is null)
            return NotFound();

        // Không cho có 2 địa chỉ mặc định
        if (request.IsDefault)
        {
            var hasOtherDefault = await db.CustomerAddresses
                .AnyAsync(a =>
                    a.CustomerId == customerId.Value &&
                    a.Id != id &&
                    a.IsDefault, ct);

            if (hasOtherDefault)
                return Conflict(new
                {
                    message = "Bạn đã có địa chỉ mặc định khác."
                });
        }

        // Cập nhật thông tin
        address.Label = request.Label.Trim();
        address.AddressLine = request.AddressLine.Trim();
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.IsDefault = request.IsDefault;

        await db.SaveChangesAsync(ct);

        var result = new CustomerAddressDto(
            address.Id,
            address.Label,
            address.AddressLine,
            address.Latitude,
            address.Longitude,
            address.IsDefault,
            address.CreatedAtUtc
        );

        return Ok(result);
    }

    // DELETE: api/v1/customers/me/addresses/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAddress(
        Guid id,
        CancellationToken ct)
    {
        var customerId = CurrentCustomerId;

        if (customerId is null)
            return Unauthorized();

        // Chỉ tìm địa chỉ thuộc khách hàng đang đăng nhập
        var address = await db.CustomerAddresses
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.CustomerId == customerId.Value, ct);

        if (address is null)
            return NotFound();

        // Xóa địa chỉ
        db.CustomerAddresses.Remove(address);

        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}
