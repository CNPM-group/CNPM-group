using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/drones")]
[Authorize(Roles = Roles.OperationsStaff)]
public class DronesController(DroneService drones) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DroneResponse>>> GetAll(CancellationToken ct) =>
        Ok((await drones.ListAsync(ct)).Select(DroneResponse.From).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DroneResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(DroneResponse.From(await drones.GetAsync(id, ct)));

    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<DroneResponse>> Create(DroneRequest request, CancellationToken ct)
    {
        var d = await drones.CreateAsync(request.ToDomain(), ct);
        return CreatedAtAction(nameof(Get), new { id = d.Id }, DroneResponse.From(d));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<DroneResponse>> Update(Guid id, DroneRequest request, CancellationToken ct) =>
        Ok(DroneResponse.From(await drones.UpdateAsync(id, request.ToDomain(), ct)));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await drones.DeleteAsync(id, ct);
        return NoContent();
    }
}
