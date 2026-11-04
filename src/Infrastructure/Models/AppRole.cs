using Microsoft.AspNetCore.Identity;

namespace SmartDrone.Infrastructure.Models;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() { Id = Guid.NewGuid(); }
}
