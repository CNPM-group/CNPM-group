using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/packages")]
[Authorize]
public class PackagesController(PackageImageService images) : ControllerBase
{
    [HttpPost("images")]
    [Authorize(Roles = Roles.Customer)]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(PackageImageService.MaxBytes + 64 * 1024)]
    public async Task<ActionResult<ImageUploadResponse>> UploadImage(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var key = await images.UploadAsync(stream, file.Length, ct);
        return Ok(new ImageUploadResponse(key));
    }
}
