using Collectors.Data;
using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collectors.Classes
{
    public class DbManager
    {
        public ApplicationDbContext Db { get; set; }
        public string[] GetTagsFromServer()
        {
            List<string> result = new List<string>();
            foreach (Tag t in Db.Tags.ToArray())
                result.Add(t.Name);
            return result.ToArray();
        }

        public void UpdateTags(string tags)
        {
            string[] t = tags.Split(',');
            foreach (string tag in t)
                if (!Db.Tags.Contains(new Tag { Name = tag }))
                    Db.Tags.Add(new Tag { Name = tag });
            Save();
        }

        public CollectionItem GetItem(int itemId)
        {
            return Db.Items.FirstOrDefault(i => i.Id == itemId);
        }

        public Collection GetCollectionById(int id)
        {
            return Db.Collections.First(c => c.Id == id);
        }
        public List<CollectionItem> GetItemsInCollection(int id)
        {
            return GetItemsByCollectionId(id).ToList();
        }

        public List<CollectionItem> GetSortedById(int id)
        {
            return GetItemsByCollectionId(id).OrderBy(i => i.Id).ToList();
        }

        public List<CollectionItem> GetSortedByName(int id)
        {
            return GetItemsByCollectionId(id).OrderBy(i => i.Name).ToList();
        }

        public List<CollectionItem> GetSortedByTags(int id)
        {
            return GetItemsByCollectionId(id).OrderBy(i => i.Tags).ToList();
        }

        public List<CollectionItem> GetSortBy(int id, int fieldIndex)
        {
            var items = GetItemsByCollectionId(id).ToList();
            items.Sort((e1, e2) => CompareBySelectedField(fieldIndex, e1, e2));
            return items;
        }

        public List<CollectionItem> SearchByName(int id, string name)
        {
            return GetItemsByCollectionId(id).Where(i => i.Name.Contains(name)).ToList();
        }
        public void AddItem(CollectionItem item)
        {
            Db.Items.Add(item);
            Save();
        }
        public void RemoveItem(CollectionItem i)
        {
            Db.Items.Remove(i);
            Save();
        }
        public void Save()
        {
            Db.SaveChanges();
        }
        private static int CompareBySelectedField(int fieldIndex, CollectionItem e1, CollectionItem e2)
        {
            return new FieldManager(e1).GetFieldByIndex(fieldIndex)
                .CompareTo(new FieldManager(e2).GetFieldByIndex(fieldIndex));
        }
        private IQueryable<CollectionItem> GetItemsByCollectionId(int id)
        {
            return Db.Items.Where(i => i.CollectionId == id);
        }

    }
}