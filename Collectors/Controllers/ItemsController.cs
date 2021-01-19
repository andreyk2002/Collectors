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
    public partial class ItemsController :  Controller
    {
        private readonly AdditionalFieldsSetter fieldsSetter = new AdditionalFieldsSetter();
        private readonly UserManager<IdentityUser> userManager;
        private readonly DbManager dbManager;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            dbManager = new DbManager { Db = context};
        }

        public IActionResult Index(int id)
        {
            ItemsListViewModel model;
            if (RequireSort(id))
                return View(GetSortedItems());
            model = GetItemsListModel(id);
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
            return View(GetItemsListModel(ia.CollectionId));
        }   

        public ItemsListViewModel GetSortedItems()
        {
            var items =  TempDataExtensions.Get<ItemsListViewModel>(TempData, "Items");
            return items ?? new ItemsListViewModel();
        }

        private List<string> GetFieldsNames(Collection c, List<int> indexes)
        {
            List<string> result = new List<string>();
            var fields = c.AdditionalItemsFields.Split(',');
            foreach (var i in indexes)
                result.Add(fields[i]);
            return result;
        }

        public IActionResult Add(ItemsListViewModel il)
        {
            ItemModel model = GetItemModel(il);
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
                    return View(GetItemModel(ia));
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
        private static ItemModel GetItemModel(ItemsListViewModel il)
        {
            return new ItemModel
            {
                AdditionalFieldsNames = il.AdditionalFieldsNames ?? new List<string>(),
                AdditionalFieldsIndexes = il.AdditionalFieldsIndexes ?? new List<int>(),
                CollectionId = il.CollectionId
            };
        }
        private ItemsListViewModel GetItemsListModel(int id)
        {
            Collection c = dbManager.GetCollectionById(id);
            List<CollectionItem> items = dbManager.GetItemsInCollection(id);
            ItemsListViewModel model = MakeModel(id, c, items);
            return model;
        }

        private ItemsListViewModel MakeModel(int id, Collection c, List<CollectionItem> items)
        {
            ItemsListViewModel model = new ItemsListViewModel { Items = items, CollectionId = id };
            model.AdditionalFieldsIndexes = fieldsSetter.IndexesFromMask(c.SelectedFieldsMask);
            model.AdditionalFieldsNames = GetFieldsNames(c, model.AdditionalFieldsIndexes);
            model.Selected = new List<bool>(new bool[items.Count]);
            return model;
        }
    }
}
