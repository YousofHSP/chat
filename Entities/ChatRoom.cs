using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class ChatRoom: BaseEntity<Guid>
{
    public int OwnerUserId { get; set; }
    public ChatRoomType Type { get; set; }
    #region Navigation

    public ICollection<User> Users { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public ICollection<ChatMessage>? Messages { get; set; }

    #endregion
}

public enum ChatRoomType
{
    Private,
    Channel,
    Group
}

public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        builder.HasMany(cr => cr.Users)
            .WithMany(u => u.ChatRooms);
        builder.HasMany(cr => cr.Messages)
            .WithOne(cm => cm.ChatRoom)
            .HasForeignKey(cm => cm.ChatRoomId);
        builder.HasOne(cr => cr.Owner)
            .WithMany(u => u.OwnedChatRooms)
            .HasForeignKey(cr => cr.OwnerUserId);
    }
}