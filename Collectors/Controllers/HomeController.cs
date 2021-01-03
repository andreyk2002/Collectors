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
        private readonly IStringLocalizer<HomeController> _localizer;
        public HomeController(ILogger<HomeController> logger, 
            UserManager<IdentityUser> roleManager, IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _logger = logger;
            _userManager = roleManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                if (_userManager.GetRolesAsync(currentUser).Result.Count == 0)
                    await _userManager.AddToRoleAsync(currentUser, UserRoles.user.ToString());
                ViewBag.Role = _userManager.GetRolesAsync(currentUser).Result;
            }
            return View(currentUser);
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
    }
}
