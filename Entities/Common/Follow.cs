using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Common;

public class Follow: BaseEntity
{
    public int UserId { get; set; }
    public int FollowerId { get; set; }

    #region Navigation

    public User User { get; set; }
    public User Follower { get; set; }

    #endregion
}

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.HasOne(f => f.User)
            .WithMany(u => u.Follwers)
            .HasForeignKey(f => f.UserId);
        builder.HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId);
    }
}