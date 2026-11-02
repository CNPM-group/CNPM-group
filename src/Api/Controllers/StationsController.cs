using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/stations")]
[Authorize]
public class StationsController(StationService stations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationResponse>>> GetAll(CancellationToken ct) =>
        Ok((await stations.ListAsync(ct)).Select(StationResponse.From).ToList());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StationResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(StationResponse.From(await stations.GetAsync(id, ct)));

    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<StationResponse>> Create(StationRequest r, CancellationToken ct)
    {
        var s = await stations.CreateAsync(r.Name, r.Latitude, r.Longitude, r.Status, ct);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, StationResponse.From(s));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<ActionResult<StationResponse>> Update(Guid id, StationRequest r, CancellationToken ct) =>
        Ok(StationResponse.From(await stations.UpdateAsync(id, r.Name, r.Latitude, r.Longitude, r.Status, ct)));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await stations.DeleteAsync(id, ct);
        return NoContent();
    }
}
