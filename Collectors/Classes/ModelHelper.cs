using Collectors.Areas.Identity.Data;
using Collectors.Classes;
using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collectors.Classes
{
    [Authorize(Roles = "admin,user")]
    public class ModelHelper
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
            List<CollectionItem> items = DbManager.GetItemsInCollection(c.Id);
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
            StartModel model = new StartModel();
            model.Tags = DbManager.GetTagsFromServer();
            model.LatestItems = GetLatestItems(DefaultItemsCount);
            model.BiggestCollections = DbManager.GetBiggestCollections(DefualtCollectionsCount);
            return model;
        }

        internal List<ItemModel> GetItemsByString(string searchString)
        {
            var itemsWithCollections = DbManager.GetItemsWithCollections();
            var result = DbManager.FindItems(searchString, itemsWithCollections);
            return GetItemModels(result);
        }

        private List<ItemModel> GetLatestItems(int itemsCount)
        {
            var result = DbManager.GetItemsWithCollections()
                .Take(itemsCount)
                .ToList();
            return GetItemModels(result);
        }

        private List<ItemModel> GetItemModels(List<ItemCollection> result)
        {
            List<ItemModel> model = new List<ItemModel>();
            foreach (var element in result)
            {
                model.Add(GetItemModel(element.Collection, element.Item));
            }
            return model;
        }

        public ItemModel GetItemModel(int ItemId)
        {
            CollectionItem item = DbManager.GetItem(ItemId);
            Collection c = DbManager.GetCollectionById(item.CollectionId);
            return GetItemModel(c, item);
        }

        public ItemModel GetItemModel(Collection c, CollectionItem item)
        {
            ItemModel model = new ItemModel
            {
                Likes = DbManager.GetLikesForItem(item),
                ItemId = item.Id,
                Name = item.Name,
                Tags = item.Tags,
                AdditionalFieldsIndexes = IndexesFromMask(c.SelectedFieldsMask)
            };
            model.AdditionalFieldsNames = GetFieldsNames(c, model.AdditionalFieldsIndexes);
            model.AdditionalFieldsValues = GetFieldsValues(model.AdditionalFieldsIndexes, item);
            return model;
        }

        private List<string> GetFieldsValues(List<int> additionalFieldsIndexes, CollectionItem item)
        {
            FieldManager manager = new FieldManager(item);
            List<string> fieldsValues = new List<string>();
            foreach(int i in additionalFieldsIndexes)
                fieldsValues.Add(manager.GetFieldByIndex(i));
            return fieldsValues;
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

        private List<string> GetFieldsNames(Collection c, List<int> indexes)
        {
            List<string> result = new List<string>();
            var fields = c.AdditionalItemsFields.Split(',');
            foreach (var i in indexes)
                result.Add(fields[i]);
            return result;
        }

    }
}