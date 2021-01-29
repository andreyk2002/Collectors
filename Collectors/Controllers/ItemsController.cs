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
    [Authorize(Roles = "admin,user")]
    public partial class ItemsController : Controller
    {
        private readonly AdditionalFieldsSetter fieldsSetter = new AdditionalFieldsSetter();
        private readonly UserManager<IdentityUser> userManager;
        private readonly DbManager dbManager;
        private readonly ModelHelper modelHelper;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            dbManager = new DbManager { Db = context};
            modelHelper = new ModelHelper { DbManager = dbManager };
        }

        public async Task<IActionResult> IndexAsync(int id)
        {
            ItemsListViewModel model;
            if (RequireSort(id))
                return View(GetSortedItems());
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

        public ItemsListViewModel GetSortedItems()
        {
            var items =  TempDataExtensions.Get<ItemsListViewModel>(TempData, "Items");
            return items ?? new ItemsListViewModel();
        }

        public async Task<IActionResult> AddAsync(ItemsListViewModel il)
        {
            if (!await CheckAcess(il))
                return Forbid();
            ItemModel model = modelHelper.GetItemModel(il);
            ViewBag.Tags = dbManager.GetTagsFromServer();
            return View(model);
        }

        public async Task<IActionResult> DeleteAsync(ItemsListViewModel model)
        {
            if (!await CheckAcess(model))
                return Forbid();
            var items = dbManager.GetItemsInCollection(model.CollectionId);
            for (int i = 0; i < items.Count; i++)
                if (model.Selected[i])
                    dbManager.RemoveItem(items[i]);
            return Redirect("Index?id=" + model.CollectionId);
        }
        public async Task<IActionResult> EditAsync(ItemsListViewModel model)
        {
            if (!await CheckAcess(model))
                return Forbid();
            ViewBag.Tags = dbManager.GetTagsFromServer();
            var items = dbManager.GetItemsInCollection(model.CollectionId);
            for (int i = 0; i < items.Count; i++)
                if (model.Selected[i])
                {
                    ViewBag.Item = items[i];
                    return View(modelHelper.GetItemModel(model));
                }
            return Redirect("Index?id=" + model.CollectionId);
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
            return Redirect("Index?id=" + ia.CollectionId);
        }
        public IActionResult OrderById(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedById(model.CollectionId);
            PutItemsListModel(model);
            return Redirect("Index");
        }
        public IActionResult OrderByName(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedByName(model.CollectionId);
            PutItemsListModel(model);
            return Redirect("Index");
        }
        public IActionResult OrderByTags(ItemsListViewModel model)
        {
            model.Items = dbManager.GetSortedByTags(model.CollectionId);
            PutItemsListModel(model);
            return Redirect("Index");
        }

        public IActionResult OrderByFieldIndex(ItemsListViewModel model, int index)
        {
            model.Items = dbManager.
                GetSortBy(model.CollectionId, model.AdditionalFieldsIndexes[index]);
            PutItemsListModel(model);
            return Redirect("Index");
        }

        public IActionResult SearchByName(ItemsListViewModel model, string searchString)
        {
            model.Items = dbManager.SearchByName(model.CollectionId, searchString);
            PutItemsListModel(model);
            return Redirect("Index");
        }
        public void PutItemsListModel(ItemsListViewModel model)
        {
            TempDataExtensions.Put(TempData, "Items", model);
        }

        private async Task<bool> CheckAcess(ItemsListViewModel ia)
        {
            Collection c = dbManager.GetCollectionById(ia.CollectionId);
            IdentityUser currentUser = await userManager.GetUserAsync(HttpContext.User);
            return currentUser.Id == c.UserId || User.IsInRole(UserRoles.admin.ToString());
        }

        private static bool RequireSort(int id)
        {
            return id == 0;
        }
    }
}
