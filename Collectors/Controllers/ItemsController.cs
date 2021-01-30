using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Collectors.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public partial class ItemsController : Controller
    {
        private readonly AdditionalFieldsSetter fieldsSetter = new AdditionalFieldsSetter();
        private readonly UserManager<IdentityUser> userManager;
        private readonly DbManager dbManager;
        private readonly ModelHelper modelHelper;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            dbManager = new DbManager { Db = context };
            modelHelper = new ModelHelper { DbManager = dbManager };
        }

        public async Task<IActionResult> IndexAsync(int id)
        {
            ItemsListViewModel model;
            Collection c = dbManager.GetCollectionById(id);
            model = modelHelper.GetItemsListModel(c);
            model.ViewedByCreator = await CheckAcess(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(ItemModel ia)
        {
            CollectionItem item = new CollectionItem
            { Name = ia.Name, Tags = ia.Tags, CollectionId = ia.CollectionId };
            fieldsSetter.SetAdditional(ia, item);
            dbManager.UpdateTags(ia.Tags);
            dbManager.AddItem(item);
            Collection c = dbManager.GetCollectionById(ia.CollectionId);
            var model = modelHelper.GetItemsListModel(c);
            model.ViewedByCreator = await CheckAcess(model);
            return View(model);
        }

        public async Task<IActionResult> AddAsync(ItemsListViewModel il)
        {
            if (!await CheckAcess(il))
                return Forbid();
            ItemModel model = modelHelper.GetItemModel(il);
            ViewBag.Tags = dbManager.GetTagsFromServer();
            return View(model);
        }

        public IActionResult Delete(int itemId)
        {
            CollectionItem item = dbManager.GetItem(itemId);
            dbManager.RemoveItem(item);            
            return Redirect("Index?id=" + item.CollectionId);
        }
        public IActionResult Edit(int itemId)
        {
            var model = modelHelper.GetItemModel(itemId);
            ViewBag.Tags = dbManager.GetTagsFromServer();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(ItemModel ia, int itemId)
        {
            CollectionItem item = dbManager.GetItem(itemId);
            fieldsSetter.SetAdditional(ia, item);
            dbManager.UpdateTags(ia.Tags);
            item.Name = ia.Name;
            item.Tags = ia.Tags;
            dbManager.Save();
            return Redirect("Index?id=" + item.CollectionId);
        }
        public async Task<IActionResult> OrderByIdAsync(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedById(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }
        public async Task<IActionResult> OrderByNameAsync(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedByName(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }
        public async Task<IActionResult> OrderByTagsAsync(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedByTags(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }

        public async Task<IActionResult> OrderByFieldIndexAsync(ItemsListViewModel model, int index)
        {
            model.Items = dbManager.
                GetSortBy(model.CollectionId, model.AdditionalFieldsIndexes[index]);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }

        public async Task<IActionResult> SearchByNameAsync(ItemsListViewModel model, string searchString)
        {
            model.Items = dbManager.SearchByName(model.CollectionId, searchString);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);

        }
        private async Task<bool> CheckAcess(ItemsListViewModel ia)
        {
            Collection c = dbManager.GetCollectionById(ia.CollectionId);
            IdentityUser currentUser = await userManager.GetUserAsync(HttpContext.User);
            return currentUser != null && 
                (currentUser.Id == c.UserId || User.IsInRole(UserRoles.admin.ToString()));
        }

        private static bool RequireSort(int id)
        {
            return id == 0;
        }
    }
}
