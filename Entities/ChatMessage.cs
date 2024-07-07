using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities;

public class ChatMessage: BaseEntity<Guid>
{
    public int SenderId { get; set; }
    public string Message { get; set; } = null!;
    public Guid ChatRoomId { get; set; }

    #region Navigation

    public User Sender { get; set; } = null!;
    public ChatRoom ChatRoom { get; set; } = null!;

    #endregion
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.Property(c => c.Message).HasMaxLength(255);
        
        builder.HasOne(c => c.ChatRoom)
            .WithMany(cr => cr.Messages)
            .HasForeignKey(c => c.ChatRoomId);
        builder.HasOne(c => c.Sender)
            .WithMany(u => u.ChatMessages)
            .HasForeignKey(c => c.SenderId);
    }
}