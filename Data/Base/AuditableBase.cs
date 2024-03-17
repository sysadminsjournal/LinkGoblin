using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace LinkGoblin.Data.Base;

public class AuditableBase
{
    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }
    public required IdentityUser CreatedBy { get; set; }
    public required IdentityUser UpdatedBy { get; set; }
}