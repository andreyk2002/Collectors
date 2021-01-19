using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Comments.Hubs
{
    public class CommentHub : Hub
    {
        private readonly DbManager DbManager;

        public CommentHub(ApplicationDbContext context)
        {
            DbManager = new DbManager { Db = context } ;
        }
        public async Task Send(string message, string userName, string groupId)
        {
            DbManager.AddComment(message, userName, groupId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Send", message, userName);
        }

    }
}
