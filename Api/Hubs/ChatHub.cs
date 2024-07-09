using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using DTO;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Hubs;

public class ChatHub(IUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<ChatRoom> chatRoomRepository, IMapper mapper): Hub
{
    

    public async Task LoadAllFollowings(int senderId, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetFollowings(senderId, cancellationToken);
        var result = mapper.Map<List<UserDto>>(users);
        await Clients.Caller.SendAsync("generateFollowingsList", result, cancellationToken: cancellationToken);
    }

    public async Task LoadRoomChats(Guid roomId)
    {
        var chatMessages = chatMessageRepository.TableNoTracking
            .Where(ch => ch.ChatRoomId == roomId)
            .ProjectTo<ChatMessageDto>(mapper.ConfigurationProvider)
            .ToList();
        await Clients.Caller.SendAsync("generateMessagesList", chatMessages);
    }

    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task SendMessage(ChatMessageDto? messageDto)
    {
        var model = mapper.Map<ChatMessage>(messageDto);
        await chatMessageRepository.AddAsync(model, new CancellationToken());
        var dto = mapper.Map<ChatMessageDto>(model);
        await Clients.Groups(messageDto.ChatRoomId.ToString()).SendAsync("receiveNewMessage", dto);
    }

    public async Task StartChatRoom(int ownerId, int userId)
    {
        var usersIds = new List<int>() { userId, ownerId };
        var model = await chatRoomRepository.TableNoTracking
            .Include(cr => cr.Users)
            .Where(cr => usersIds.All(ui => cr.Users.Select(u => u.Id).Contains(ui)))
            .Where(cr => cr.Type == ChatRoomType.Private)
            .FirstOrDefaultAsync();
        if (model is null)
        {
            model = new ChatRoom()
            {
                OwnerUserId = ownerId,
                Type = ChatRoomType.Private,
                
                // Users = new List<User>
                // {
                //     new User { Id = userId },
                //     new User { Id = ownerId }
                // }
            };
            await chatRoomRepository.AddAsync(model, new CancellationToken());
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, model.Id.ToString()); 
        await Clients.Caller.SendAsync("setRoomId", model.Id);
    }
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("connectionid " + Context.ConnectionId);
        return base.OnConnectedAsync();
    }
}