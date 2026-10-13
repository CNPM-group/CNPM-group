using Microsoft.AspNetCore.Identity;

namespace SmartDrone.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public AppUser() { Id = Guid.NewGuid(); }

    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
