using Collectors.Classes;
using Collectors.Data;
using Collectors.Models;
using Collectors.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbManager DbManager;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, 
            UserManager<IdentityUser> roleManager)
        {
            this.DbManager = new DbManager { Db = db };
            _logger = logger;
            _userManager = roleManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                if (!HasRole(currentUser))
                    await _userManager.AddToRoleAsync(currentUser, UserRoles.user.ToString());
                ViewBag.Role = _userManager.GetRolesAsync(currentUser).Result;
            }
            ViewBag.Tags = DbManager.GetTagsFromServer();
            return View(currentUser);
        }

        public IActionResult Search(string searchString)
        {
            var results = DbManager.FindAll(searchString);
            return View(results);
        }

        public IActionResult SearchTags(string tag)
        {
            var results = DbManager.FindAll(tag);
            return View("Search",results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        private bool HasRole(IdentityUser currentUser)
        {
            return _userManager.GetRolesAsync(currentUser).Result.Count != 0;
        }

    }
}
