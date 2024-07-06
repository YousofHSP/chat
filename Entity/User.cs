using System.ComponentModel.DataAnnotations;
using Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class User : IdentityUser<int>, IEntity<int>
{
    public required string Mobile { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public GenderType Gender { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? Image { get; set; }
    public string? Bio { get; set; }

    public DateTimeOffset LastLoginDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    

}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.UserName).IsRequired().HasMaxLength(100);
    }
}

public enum GenderType
{
    [Display(Name = "مرد")] Male = 1,
    [Display(Name = "زن")] Female = 2,
}

public enum UserStatus
{
    [Display(Name = "فعال")] Active = 1,
    [Display(Name = "غیرفعال")] Disable = 0,
}