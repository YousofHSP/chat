using Entities;

namespace DTO;

public class ChatMessageDto: BaseDto<ChatMessageDto, ChatMessage, Guid>
{
    public string Message { get; set; } = null!;
}