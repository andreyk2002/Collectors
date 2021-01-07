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
            ViewBag.Collections = User.IsInRole(UserRoles.admin.ToString()) ?
             db.Collections.ToList() : db.Collections.Where(c =>
                 c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
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
            collection.SelectedFields = CreateMaskOfSelectedFields(model.AdditionalFields);
            SetCollectionImageInBytes(model.Image, collection);
            db.Collections.Add(collection);
            db.SaveChanges();
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

        public IActionResult Delete(int id)
        {
            Collection c = db.Collections.First(c => c.Id == id);
            if (c != null)
                db.Collections.Remove(c);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Collection c = db.Collections.First(c => c.Id == id);
            if (c != null)
                return View(new CollectionEditModel
                { CollectionId = id, ShortDescription = c.ShortDescription, OldImage = c.Image });
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(CollectionEditModel collection)
        {
            Collection c = db.Collections.First(c => c.Id == collection.CollectionId);
            if (c != null)
            {
                if (collection.ShouldDeleteImage)
                    c.Image = null;
                else
                    SetCollectionImageInBytes(collection.Image, c);
                c.ShortDescription = collection.ShortDescription;
                db.SaveChanges();
            }
                return RedirectToAction("Index");
        }

            public string ViewCollection(Collection collection)
            {
                return "Пшёл нахуй я нихуя не сделал";
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






