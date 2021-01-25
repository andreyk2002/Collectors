using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Collectors.Models.Item;
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
        private readonly DbManager _dbManager;
        private readonly ModelHelper _modelHelper;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, 
            UserManager<IdentityUser> roleManager)
        {
            this._dbManager = new DbManager { Db = db };
            _logger = logger;
            _userManager = roleManager;
            _modelHelper = new ModelHelper { DbManager = _dbManager };
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
            StartModel model = _modelHelper.MakeStartModel();
            return View(model);
        }

        public IActionResult Search(string searchString)
        {
            var results = _dbManager.FindAll(searchString);
            List<ItemLikesModel> model = _modelHelper.MakeModel(results);
            ViewBag.searchString = searchString;
            return View(model);
        }

        public IActionResult Like(string searchStr, int itemId)
        {
            CollectionItem item = _dbManager.GetItem(itemId);
            item.Likes++;
            _dbManager.Save();
            return Redirect("Search?searchString=" + searchStr);
        }

        public IActionResult SearchTags(string tag)
        {
            return Redirect("Search?searchString=" + tag);
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
