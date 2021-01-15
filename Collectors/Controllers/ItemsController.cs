﻿using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class ItemsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            db = context;
        }

        public IActionResult Index(int id)
        {
            ItemsListViewModel model = GetItemsListModel(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ItemModel ia)
        {
            CollectionItem item = new CollectionItem
            { Name = ia.Name, Tags = ia.Tags, CollectionId = ia.CollectionId };
            SetAdditionalFields(ia, item);
            UpdateTags(ia.Tags);
            db.Items.Add(item);
            db.SaveChanges();
            return View(GetItemsListModel(ia.CollectionId));
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
            ViewBag.Tags = GetTagsFromServer();
            return View(model);
        }

        public IActionResult Delete(ItemsListViewModel ia)
        {
            var items = GetItemsInCollection(ia.CollectionId);
            for (int i = 0; i < items.Count; i++)
                if (ia.Selected[i])
                    db.Items.Remove(items[i]);
            db.SaveChanges();
            return Redirect("Index?id=" + ia.CollectionId);

        }
        public IActionResult Edit(ItemsListViewModel ia)
        {
            ViewBag.Tags = GetTagsFromServer();
            var items = GetItemsInCollection(ia.CollectionId);
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
            CollectionItem item = GetItem(itemId);
            SetAdditionalFields(ia, item);
            UpdateTags(ia.Tags);
            item.Name = ia.Name;
            item.Tags = ia.Tags;
            db.SaveChanges();
            return Redirect("Index?id=" + ia.CollectionId);
        }

        private CollectionItem GetItem(int itemId)
        {
            return db.Items.FirstOrDefault(i => i.Id == itemId);
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
        private string[] GetTagsFromServer()
        {
            List<string> result = new List<string>();
            foreach (Tag t in db.Tags.ToArray())
            {
                result.Add(t.Name);
            }
            return result.ToArray();
        }

        private Collection GetCollectionById(int id)
        {
            return db.Collections.First(c => c.Id == id);
        }
        private List<CollectionItem> GetItemsInCollection(int id)
        {
            return db.Items.Where(i => i.CollectionId == id).ToList();
        }

        private ItemsListViewModel GetItemsListModel(int id)
        {
            Collection c = GetCollectionById(id);
            List<CollectionItem> items = GetItemsInCollection(id);
            ItemsListViewModel model = new ItemsListViewModel { Items = items, CollectionId = id };
            model.AdditionalFieldsIndexes = IndexesFromMask(c.SelectedFieldsMask);
            model.AdditionalFieldsNames = GetFieldsNames(c, model.AdditionalFieldsIndexes);
            model.Selected = new List<bool>(new bool[items.Count]);
            return model;
        }
        private void UpdateTags(string tags)
        {
            string[] t = tags.Split(',');
            foreach (string tag in t)
            {
                if (!db.Tags.Contains(new Tag { Name = tag }))
                    db.Tags.Add(new Tag { Name = tag });
            }
            db.SaveChanges();
        }

        private void SetAdditionalFields(ItemModel ia, CollectionItem c)
        {
            FieldManager m = new FieldManager(c);
            if (ia.AdditionalFieldsIndexes != null)
            {
                SetFieledsByIndexes(ia, m);
            }
        }

        private static void SetFieledsByIndexes(ItemModel ia, FieldManager m)
        {
            for (int i = 0; i < ia.AdditionalFieldsIndexes.Count; i++)
            {
                if (ia.AdditionalFieldsValues[i] != null)
                    m.SetFieldByIndex
                        (ia.AdditionalFieldsIndexes[i], ia.AdditionalFieldsValues[i]);
            }
        }

        private List<int> IndexesFromMask(int mask)
        {
            List<int> indexes = new List<int>();
            int i = 15;
            while (mask != 0)
            {
                int val = (int)Math.Pow(2, i);
                if (mask - val >= 0)
                {
                    mask -= val;
                    indexes.Add(i);
                }
                i--;
            }
            indexes.Reverse();
            return indexes;
        }
    }
}
