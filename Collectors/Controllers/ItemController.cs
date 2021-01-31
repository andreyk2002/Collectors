﻿using Collectors.Classes;
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
        private readonly DbManager _dbManager;

        public ItemController(ApplicationDbContext context)
        {
            _dbManager = new DbManager { Db = context };
        }

        public IActionResult Index(long id)
        {   
            return View(_dbManager.GetCommentsById(id));
        }

    }
}
