using Collectors.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Users.ToList();

            return View(users);
        }

        //public IActionResult ChangeRole(List<IdentityUser>users, string newRole)
        //{

        //}
    }
}
