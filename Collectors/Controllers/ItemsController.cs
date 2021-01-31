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
        private readonly AdditionalFieldsSetter _fieldsSetter = new AdditionalFieldsSetter();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DbManager _dbManager;
        private readonly ModelsBuilder _modelBuilder;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _dbManager = new DbManager { Db = context };
            _modelBuilder = new ModelsBuilder { DbManager = _dbManager };
        }

        public async Task<IActionResult> IndexAsync(int id)
        {
            ItemsListViewModel model;
            Collection c = _dbManager.GetCollectionById(id);
            model = _modelBuilder.GetItemsListModel(c);
            model.ViewedByCreator = await CheckAcess(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(ItemModel ia)
        {
            CollectionItem item = new CollectionItem
            { Name = ia.Name, Tags = ia.Tags, CollectionId = ia.CollectionId };
            _fieldsSetter.SetAdditional(ia, item);
            _dbManager.UpdateTags(ia.Tags);
            _dbManager.AddItem(item);
            Collection c = _dbManager.GetCollectionById(ia.CollectionId);
            var model = _modelBuilder.GetItemsListModel(c);
            model.ViewedByCreator = await CheckAcess(model);
            return View(model);
        }

        public async Task<IActionResult> AddAsync(ItemsListViewModel il)
        {
            if (!await CheckAcess(il))
                return Forbid();
            ItemModel model = _modelBuilder.GetItemModel(il);
            ViewBag.Tags = _dbManager.GetTagsFromServer();
            return View(model);
        }

        public IActionResult Delete(int itemId)
        {
            CollectionItem item = _dbManager.GetItem(itemId);
            _dbManager.RemoveItem(item);
            return Redirect("Index?id=" + item.CollectionId);
        }
        public IActionResult Edit(int itemId)
        {
            var model = _modelBuilder.GetItemModel(itemId);
            ViewBag.Tags = _dbManager.GetTagsFromServer();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(ItemModel ia, int itemId)
        {
            CollectionItem item = _dbManager.GetItem(itemId);
            _fieldsSetter.SetAdditional(ia, item);
            _dbManager.UpdateTags(ia.Tags);
            item.Name = ia.Name;
            item.Tags = ia.Tags;
            _dbManager.Save();
            return Redirect("Index?id=" + item.CollectionId);
        }
        public async Task<IActionResult> OrderByIdAsync(ItemsListViewModel model)
        {
            model.Items = _dbManager.GetSortedById(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }
        public async Task<IActionResult> OrderByNameAsync(ItemsListViewModel model)
        {
            model.Items = _dbManager.GetSortedByName(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }
        public async Task<IActionResult> OrderByTagsAsync(ItemsListViewModel model)
        {
            model.Items = _dbManager.GetSortedByTags(model.CollectionId);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }

        public async Task<IActionResult> OrderByFieldIndexAsync(ItemsListViewModel model, int index)
        {
            model.Items = _dbManager
                .GetSortBy(model.CollectionId, model.AdditionalFieldsIndexes[index]);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);
        }

        public async Task<IActionResult> SearchByNameAsync(ItemsListViewModel model, string searchString)
        {
            model.Items = _dbManager.SearchByName(model.CollectionId, searchString);
            model.ViewedByCreator = await CheckAcess(model);
            return View("Index", model);

        }
        private async Task<bool> CheckAcess(ItemsListViewModel ia)
        {
            Collection c = _dbManager.GetCollectionById(ia.CollectionId);
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            return currentUser != null &&
                (currentUser.Id == c.UserId || User.IsInRole(UserRoles.admin.ToString()));
        }
    }
}
