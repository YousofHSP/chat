using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class ChatRoom: BaseEntity<Guid>
{
    #region Navigation

    public ICollection<User> Users { get; set; } = null!;
    public ICollection<ChatMessage>? Messages { get; set; }

    #endregion
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
    }
}