using System.ComponentModel.DataAnnotations;
using Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class User : IdentityUser<int>, IEntity<int>
{
    public string FullName { get; set; } = null!;
    public GenderType Gender { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? Image { get; set; }
    public string? Bio { get; set; }

    public DateTimeOffset LastLoginDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    #region Navigation

    public ICollection<ChatMessage>? ChatMessages { get; set; }
    public ICollection<ChatRoom>? ChatRooms { get; set; }
    public ICollection<Follow>? Follwers { get; set; }
    public ICollection<Follow>? Following { get; set; }

    #endregion

    

}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.UserName).IsRequired();

        builder.HasMany(u => u.ChatMessages)
            .WithOne(cm => cm.Sender)
            .HasForeignKey(cm => cm.SenderId);
        builder.HasMany(u => u.ChatRooms)
            .WithMany(cr => cr.Users);
        builder.HasMany(u => u.Follwers)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId);
        builder.HasMany(u => u.Following)
            .WithOne(f => f.Follower)
            .HasForeignKey(f => f.FollowerId);

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