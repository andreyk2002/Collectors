using Collectors.Comments.Hubs;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext Db;

        public ItemController(ApplicationDbContext context)
        {
            this.Db = context;
        }

        public IActionResult Index(long id)
        {
            var Comments = Db.Comments.Where(i => (long)i.ItemId == id).ToList();
            CommentModel m = new CommentModel { ItemId = id, Comments = Comments };
            return View(m);
        }

        public IActionResult Create(string content, int id)
        {

            Comment c = new Comment { Content = content, ItemId = id, UserName = User.Identity.Name };
            Db.Add(c);
            Db.SaveChanges();
            return Ok();

        }
    }
}
