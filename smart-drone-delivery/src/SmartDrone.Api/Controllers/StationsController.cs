using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Application.Stations;
using SmartDrone.Domain.Common;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
[Authorize]
public class StationsController(IStationService stations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetAll(CancellationToken ct) =>
        Ok(await stations.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StationDto>> Get(Guid id, CancellationToken ct)
    {
        var s = await stations.GetAsync(id, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<StationDto>> Create(StationRequest request, CancellationToken ct)
    {
        var s = await stations.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, s);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<StationDto>> Update(Guid id, StationRequest request, CancellationToken ct)
    {
        var s = await stations.UpdateAsync(id, request, ct);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await stations.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
