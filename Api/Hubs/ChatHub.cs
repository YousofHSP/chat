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

[Authorize]
public class ChatHub(IUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<ChatRoom> chatRoomRepository, IMapper mapper): Hub
{
    

    public async Task LoadAllFollowings(CancellationToken cancellationToken)
    {
        var userId = Context.User!.Identity!.GetUserId<int>();
        var users = await userRepository.GetFollowings(userId, cancellationToken);
        var result = mapper.Map<List<UserDto>>(users);
        await Clients.Caller.SendAsync("generateFollowingsList", result, cancellationToken: cancellationToken);
    }

    public async Task LoadRoomChats(Guid roomId, CancellationToken cancellationToken)
    {
        var chatMessages = chatMessageRepository.TableNoTracking
            .Where(ch => ch.ChatRoomId == roomId)
            .ProjectTo<ChatMessageDto>(mapper.ConfigurationProvider)
            .ToList();
        await Clients.Caller.SendAsync("generateMessagesList", chatMessages, cancellationToken);
    }

    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task StartChatRoom(int userId, CancellationToken cancellationToken)
    {
        var ownerId = Context.User!.Identity!.GetUserId<int>();
        var usersIds = new List<int>() { userId, ownerId };
        var model = await chatRoomRepository.TableNoTracking
            .Include(cr => cr.Users)
            .Where(cr => usersIds.All(ui => cr.Users.Select(u => u.Id).Contains(ui)))
            .Where(cr => cr.Type == ChatRoomType.Private)
            .SingleOrDefaultAsync(cancellationToken);
        if (model is null)
        {
            model = new ChatRoom()
            {
                OwnerUserId = ownerId,
                Type = ChatRoomType.Private,
                
                Users = new List<User>
                {
                    new User { Id = userId },
                    new User { Id = ownerId }
                }
            };
            await chatRoomRepository.AddAsync(model,cancellationToken);
        }

        await Clients.Caller.SendAsync("setRoomId", model.Id.ToString(), userId.ToString(), cancellationToken);
    }
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}