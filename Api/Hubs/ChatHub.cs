﻿using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public class ChatHub: Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}