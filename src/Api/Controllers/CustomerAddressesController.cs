using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/customers/me/addresses")]
[Authorize(Roles = Roles.Customer)]
public class CustomerAddressesController(CustomerAddressService addresses) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerAddressResponse>>> GetAll(CancellationToken ct) =>
        Ok((await addresses.ListAsync(User.UserId(), ct)).Select(CustomerAddressResponse.From).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerAddressResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(CustomerAddressResponse.From(await addresses.GetAsync(User.UserId(), id, ct)));

    [HttpPost]
    public async Task<ActionResult<CustomerAddressResponse>> Create(CustomerAddressRequest request, CancellationToken ct)
    {
        var a = await addresses.CreateAsync(User.UserId(), request.ToDomain(), ct);
        return CreatedAtAction(nameof(Get), new { id = a.Id }, CustomerAddressResponse.From(a));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerAddressResponse>> Update(Guid id, CustomerAddressRequest request, CancellationToken ct) =>
        Ok(CustomerAddressResponse.From(await addresses.UpdateAsync(User.UserId(), id, request.ToDomain(), ct)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await addresses.DeleteAsync(User.UserId(), id, ct);
        return NoContent();
    }
}
