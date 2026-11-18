using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

// FR03 (v0.11): nhân viên quản lý thông tin khách hàng. Khách tự quản địa chỉ ở /customers/me/addresses.
[ApiController]
[Route(ApiConstants.Prefix + "/customers")]
[Authorize(Roles = Roles.DispatchStaff)]
public class CustomersController(CustomerService customers) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> List([FromQuery] string? q, CancellationToken ct) =>
        Ok((await customers.ListAsync(q, ct)).Select(CustomerResponse.From).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDetailResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(CustomerDetailResponse.From(await customers.GetAsync(id, ct)));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<CustomerResponse>> Update(Guid id, ProfileRequest request, CancellationToken ct)
    {
        await customers.UpdateAsync(id, request.FullName, request.Phone, ct);
        var detail = await customers.GetAsync(id, ct);
        return Ok(CustomerResponse.From(new CustomerSummary(detail.User, detail.Stats)));
    }
}
