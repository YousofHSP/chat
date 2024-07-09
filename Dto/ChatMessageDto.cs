using Entities;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DTO;

public class ChatMessageDto: BaseDto<ChatMessageDto, ChatMessage, Guid>
{
    public string Message { get; set; } = null!;
    public Guid ChatRoomId { get; set; }
    public int SenderId { get; set; }
    public UserDto Sender { get; set; } = null!;
    public string CreatedAt { get; set; }
}