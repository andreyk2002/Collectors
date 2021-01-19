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
        private readonly ApplicationDbContext Db;

        public CommentHub(ApplicationDbContext context)
        {
            Db = context ;
        }
        public async Task Send(string message, string userName, string groupId)
        {
            Db.Comments.Add(new Comment { UserName = userName, Content = message, ItemId = Int64.Parse(groupId) });
            Db.SaveChanges();
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("Send", message, userName);
        }

    }
}
