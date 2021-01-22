using Collectors.Classes;
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
        private readonly DbManager dbManager;
        private readonly IWebHostEnvironment webHost;
        private readonly UserManager<IdentityUser> userManager;

        public CollectionsController(ApplicationDbContext context, IWebHostEnvironment webHost, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            dbManager = new DbManager { Db = context };
            this.webHost = webHost;
        }
        public IActionResult Index()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Collections = User.IsInRole(UserRoles.admin.ToString()) ?
             dbManager.Db.Collections : dbManager.GetCollectionByUserId(id);
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CollectionCreateModel model)
        {
            Collection collection = CreateCollectionFromModel(model);
            collection.SelectedFieldsMask = CreateMaskOfSelectedFields(model.AdditionalFields);
            SetCollectionImageInBytes(model.Image, collection);
            dbManager.AddCollection(collection);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Collection c = dbManager.GetCollectionById(id);
            dbManager.RemoveCollection(c);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Collection c = dbManager.GetCollectionById(id);
            return View(new CollectionEditModel
            {
                CollectionId = id,
                ShortDescription = c.ShortDescription,
                OldImage = c.Image
            });
        }

        [HttpPost]
        public IActionResult Edit(CollectionEditModel collection)
        {
            Collection c = dbManager.GetCollectionById(collection.CollectionId);
            ChangeImage(collection, c);
            c.ShortDescription = collection.ShortDescription;
            dbManager.Save();

            return RedirectToAction("Index");
        }
      
        private Collection CreateCollectionFromModel(CollectionCreateModel model)
        {
            return new Collection
            {
                ShortDescription = model.ShortDescription,
                ThemeId = (byte)model.CollectionTheme,
                AdditionalItemsFields = string.Join(",", model.AdditionalFields),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
        }

        private void ChangeImage(CollectionEditModel collection, Collection c)
        {
            if (collection.ShouldDeleteImage)
                c.Image = null;
            else
                SetCollectionImageInBytes(collection.Image, c);
        }

        private int CreateMaskOfSelectedFields(IList<string> additionalFields)
        {
            int mask = 0;
            for (int i = 0; i < additionalFields.Count; i++)
            {
                if (additionalFields[i] != null)
                    mask += (int)Math.Pow(2, i);
            }
            return mask;
        }

        private void SetCollectionImageInBytes(IFormFile Image, Collection collection)
        {
            if (Image != null)
            {
                collection.Image = ConvertImageToBytes(Image);
            }
        }

        private byte[] ConvertImageToBytes(IFormFile Image)
        {
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(Image.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)Image.Length);
            }
            return imageData;
        }
    }
}






