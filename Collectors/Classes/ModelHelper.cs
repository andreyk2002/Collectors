using Collectors.Classes;
using Collectors.Data.Classes;
using Collectors.Models;
using Collectors.Models.Item;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace Collectors.Classes
{
    [Authorize(Roles = "admin,user")]
    public class ModelHelper
    {
        public DbManager DbManager { get; set; }

      
        public ItemModel GetItemModel(ItemsListViewModel il)
        {
            return new ItemModel
            {
                AdditionalFieldsNames = il.AdditionalFieldsNames ?? new List<string>(),
                AdditionalFieldsIndexes = il.AdditionalFieldsIndexes ?? new List<int>(),
                CollectionId = il.CollectionId
            };
        }
        public ItemsListViewModel GetItemsListModel(int id)
        {
            Collection c = DbManager.GetCollectionById(id);
            List<CollectionItem> items = DbManager.GetItemsInCollection(id);
            ItemsListViewModel model = MakeModel(id, c, items);
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

        public List<ItemLikesModel> MakeModel(List<CollectionItem> items)
        {
            List<ItemLikesModel> model = new List<ItemLikesModel>();
            foreach(var i in items)
            {
                model.Add(new ItemLikesModel { Item = i, IsLiked = false });
            }
            return model;
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