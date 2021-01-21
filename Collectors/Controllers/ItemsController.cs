using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
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

        public IActionResult Index(int id)
        {
            ItemsListViewModel model;
            if (RequireSort(id))
                return View(GetSortedItems());
            model = modelHelper.GetItemsListModel(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ItemModel ia)
        {
            CollectionItem item = new CollectionItem
            { Name = ia.Name, Tags = ia.Tags, CollectionId = ia.CollectionId };
            fieldsSetter.SetAdditional(ia, item);
            dbManager.UpdateTags(ia.Tags);
            dbManager.AddItem(item);
            return View(modelHelper.GetItemsListModel(ia.CollectionId));
        }   

        public ItemsListViewModel GetSortedItems()
        {
            var items =  TempDataExtensions.Get<ItemsListViewModel>(TempData, "Items");
            return items ?? new ItemsListViewModel();
        }

        public IActionResult Add(ItemsListViewModel il)
        {
            ItemModel model = modelHelper.GetItemModel(il);
            ViewBag.Tags = dbManager.GetTagsFromServer();
            return View(model);
        }

        public IActionResult Delete(ItemsListViewModel ia)
        {
            var items = dbManager.GetItemsInCollection(ia.CollectionId);
            for (int i = 0; i < items.Count; i++)
                if (ia.Selected[i])
                    dbManager.RemoveItem(items[i]);
            return Redirect("Index?id=" + ia.CollectionId);
        }
        public IActionResult Edit(ItemsListViewModel ia)
        {
            ViewBag.Tags = dbManager.GetTagsFromServer();
            var items = dbManager.GetItemsInCollection(ia.CollectionId);
            for (int i = 0; i < items.Count; i++)
                if (ia.Selected[i])
                {
                    ViewBag.Item = items[i];
                    return View(modelHelper.GetItemModel(ia));
                }
            return Redirect("Index?id=" + ia.CollectionId);
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
        private void PutItemsListModel(ItemsListViewModel model)
        {
            TempDataExtensions.Put(TempData, "Items", model);
        }

        private static bool RequireSort(int id)
        {
            return id == 0;
        }
    }
}
