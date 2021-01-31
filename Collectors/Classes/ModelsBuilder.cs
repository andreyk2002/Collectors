using Collectors.Data.Classes;
using Collectors.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public class ModelsBuilder
    {
        public const int DefualtCollectionsCount = 3;
        public const int DefaultItemsCount = 5;
        public DbManager DbManager { get; set; }
        public UserManager<IdentityUser> UserManager { get; set; }
        public ItemModel GetItemModel(ItemsListViewModel il)
        {
            return new ItemModel
            {
                AdditionalFieldsNames = il.AdditionalFieldsNames ?? new List<string>(),
                AdditionalFieldsIndexes = il.AdditionalFieldsIndexes ?? new List<int>(),
                CollectionId = il.CollectionId
            };
        }
        public ItemsListViewModel GetItemsListModel(Collection c)
        {
            List<CollectionItem> items = DbManager.GetItemsByCollectionId(c.Id)
                .ToList();
            ItemsListViewModel model = MakeModel(c.Id, c, items);
            return model;
        }

        public ItemsListViewModel MakeModel(int id, Collection c, List<CollectionItem> items)
        {
            ItemsListViewModel model = new ItemsListViewModel { Items = items, CollectionId = id };
            model.AdditionalFieldsIndexes = IndexesFromMask(c.SelectedFieldsMask);
            model.AdditionalFieldsNames = GetFieldsNames(c, model.AdditionalFieldsIndexes);
            model.Selected = new List<bool>(new bool[items.Count]);
            return model;
        }

        public StartModel MakeStartModel()
        {
            StartModel model = new StartModel
            {
                Tags = DbManager.GetTagsFromServer(),
                LatestItems = GetLatestItems(DefaultItemsCount),
                BiggestCollections = DbManager.GetBiggestCollections(DefualtCollectionsCount)
            };
            return model;
        }

        public List<ItemModel> SearchItems(string searchString)
        {
            var itemsWithCollections = DbManager.GetItemsWithCollections();
            var result = DbManager.FindItems(searchString, itemsWithCollections);
            return GetItemModels(result);
        }

        public ItemModel GetItemModel(int ItemId)
        {
            CollectionItem item = DbManager.GetItem(ItemId);
            Collection c = DbManager.GetCollectionById(item.CollectionId);
            return GetItemModel(c, item);
        }

        public ItemModel GetItemModel(Collection c, CollectionItem item)
        {
            ItemModel model = GetBasicItemModel(c, item);
            model.AdditionalFieldsIndexes = IndexesFromMask(c.SelectedFieldsMask);
            model.AdditionalFieldsNames = GetFieldsNames(c, model.AdditionalFieldsIndexes);
            model.AdditionalFieldsValues = GetFieldsValues(model.AdditionalFieldsIndexes, item);
            return model;
        }

        private ItemModel GetBasicItemModel(Collection c, CollectionItem item)
        {
            return new ItemModel
            {
                Likes = DbManager.GetLikesForItem(item),
                ItemId = item.Id,
                Name = item.Name,
                Tags = item.Tags,
            };
        }


        public async Task<UserModel> GetUserModel
            (UserManager<IdentityUser> userManager, IdentityUser user, string role)
        {
            return new UserModel
            {
                User = user,
                IsBlocked = await userManager.IsLockedOutAsync(user),
                Role = role
            };
        }

        private List<ItemModel> GetLatestItems(int itemsCount)
        {
            var result = DbManager.GetItemsWithCollections()
                .Take(itemsCount)
                .ToList();
            return GetItemModels(result);
        }

        private List<ItemModel> GetItemModels(List<ItemWithCollection> result)
        {
            List<ItemModel> model = new List<ItemModel>();
            foreach (var element in result)
                model.Add(GetItemModel(element.Collection, element.Item));
            return model;
        }

        public List<string> GetFieldsValues(List<int> additionalFieldsIndexes, CollectionItem item)
        {
            FieldManager manager = new FieldManager(item);
            List<string> fieldsValues = new List<string>();
            foreach (int i in additionalFieldsIndexes)
                fieldsValues.Add(manager.GetFieldByIndex(i));
            return fieldsValues;
        }
        public List<int> IndexesFromMask(int mask)
        {
            List<int> indexes = new List<int>();
            int index = 15;
            while (mask != 0)
                ExtractIndexFromMask(ref mask, indexes, ref index);
            indexes.Reverse();
            return indexes;
        }
        public void ExtractIndexFromMask(ref int mask, List<int> indexes, ref int i)
        {
            int indexBin = (int)Math.Pow(2, i);
            if (mask - indexBin >= 0)
            {
                mask -= indexBin;
                indexes.Add(i);
            }
            i--;
        }
        public List<string> GetFieldsNames(Collection c, List<int> indexes)
        {
            List<string> result = new List<string>();
            var fields = c.AdditionalItemsFields.Split(',');
            foreach (var i in indexes)
                result.Add(fields[i]);
            return result;
        }
    }
}