using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Collectors.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class CollectionsController : Controller
    {
        private ApplicationDbContext db;
        private IWebHostEnvironment WebHost;
        private readonly UserManager<IdentityUser> _userManager;

        public CollectionsController(ApplicationDbContext context, IWebHostEnvironment webHost, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            db = context;
            WebHost = webHost;
        }
        public IActionResult Index()
        {
            var collections = User.IsInRole(UserRoles.admin.ToString()) ?
            db.Collections.ToList() : db.Collections.Where(c =>
                c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
            return View(collections);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CollectionCreateModel model)
        {
            Collection collection = new Collection
            {
                ShortDescription = model.ShortDescription,
                ThemeId = (byte)model.CollectionTheme,
                AdditionalItemsFields = string.Join(",", model.AdditionalFields),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            collection.SelectedFields = CountSelectedFileds(model.AdditionalFields);
            SetImage(model.Image, collection);
            db.Collections.Add(collection);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private int CountSelectedFileds(IList<string> additionalFields)
        {
            int mask = 0;
            for (int i = 0; i < additionalFields.Count; i++)
            {
                if (additionalFields[i] != null)
                    mask += (int)Math.Pow(2, i);
            }
            return mask;
        }

        private void SetImage(IFormFile Image, Collection collection)
        {
            if (Image != null)
            {

                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)Image.Length);
                }
                collection.Image = imageData;
            }
        }
    }
}






