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

        private ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            db = context;
        }

        public IActionResult Index(int id)
        {
            ItemsListViewModel model = GetItemsListModel(id);
            return View(model);
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
            ItemAddModel model = new ItemAddModel
            {
                AdditionalFieldsNames = il.AdditionalFieldsNames,
                AdditionalFieldsIndexes = il.AdditionalFieldsIndexes,
                CollectionId = il.CollectionId
            };
            ViewBag.Tags = GetTagsFromServer();
            return View(model);
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

        [HttpPost]
        public IActionResult Index(ItemAddModel ia)
        {
            CollectionItem item = new CollectionItem
            { Name = ia.Name, Tags = ia.Tags, CollectionId = ia.CollectionId };
            //SetAdditionalFields(ia, item);
            UpdateTags(ia.Tags);
            db.Items.Add(item);
            db.SaveChanges();
            return View(GetItemsListModel(ia.CollectionId));
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

        private void SetAdditionalFields(ItemAddModel ia, CollectionItem collectionItem)
        {
            throw new NotImplementedException();
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
