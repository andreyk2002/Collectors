using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Comments.Hubs
{
    public class CommentHub : Hub
    {
        public async Task Send(string message, string userName, string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Send", message, userName);
        }

    }
}
