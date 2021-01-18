using Collectors.Comments.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class ItemController : Controller
    {

        public ItemController()
        {
        }

        public IActionResult Index(int id)
        {
            return View(id);
        }
    }
}
