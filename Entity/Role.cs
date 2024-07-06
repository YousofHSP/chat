using Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Entities;

public class Role: IdentityRole<int>, IEntity<int>
{
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}