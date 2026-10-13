using Microsoft.AspNetCore.Identity;

namespace SmartDrone.Infrastructure.Identity;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() { Id = Guid.NewGuid(); }
}
