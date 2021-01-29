using Collectors.Data;
using Collectors.Data.Classes;
using Collectors.Models;
using Collectors.Models.Comments;
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

        public void AddComment(string message, string userName, string groupId)
        {
            Db.Comments.Add(new Comment { UserName = userName, Content = message, ItemId = Int64.Parse(groupId) });
            Save();
        }

        public CommentModel GetCommentsById(long id)
        {
            var Comments = Db.Comments.Where(i => i.ItemId == id).OrderByDescending(c => c.Id).ToList();
            CommentModel m = new CommentModel { ItemId = id, Comments = Comments };
            return m;
        }

        public IEnumerable<object> GetCollectionByUserId(string id)
        {
            return Db.Collections.Where(c => c.UserId == id);
        }

        public Collection GetCollectionById(int id)
        {
            return Db.Collections.First(c => c.Id == id);
        }
        public CollectionItem GetItem(int itemId)
        {
            return Db.Items.FirstOrDefault(i => i.Id == itemId);
        }

        public List<CollectionItem> GetItemsInCollection(int id)
        {
            return GetItemsByCollectionId(id).ToList();
        }

        public List<ItemCollection> FindItems(string searchString, IQueryable<ItemCollection>model)
        {
            if (string.IsNullOrEmpty(searchString))
                return new List<ItemCollection>();
            var result = MakeSearchQuery(searchString,model);
            return result;
        }
        public List<CollectionItem> GetSortedById(int id)
        {
            return GetItemsByCollectionId(id).OrderBy(i => i.Id).ToList();
        }

        public List<Collection> GetBiggestCollections(int defualtCollectionsCount)
        {
            var collectionsId = Db.Items.GroupBy(i => i.CollectionId)
                .Select(c => new { CollectionId = c.Key, ItemsCount = c.Count() })
                .OrderByDescending(c => c.ItemsCount)
                .Take(defualtCollectionsCount);
            var biggestCollection = Db.Collections.Join(collectionsId, c => c.Id, ci => ci.CollectionId, (c, ci) => c);
            return biggestCollection.ToList();
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

        public void AddCollection(Collection c)
        {
            Db.Collections.Add(c);
            Save();
        }

        public void RemoveCollection(Collection c)
        {
            Db.Collections.Remove(c);
            Save();
            var itemsToRemove = Db.Items.Where(i => i.CollectionId == c.Id).ToList();
            foreach (var item in itemsToRemove)
                RemoveItem(item);

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
            var commentsToRemove = Db.Comments.Where(c => c.ItemId == i.Id);
            Db.Comments.RemoveRange(commentsToRemove);
            Save();
        }

        public IQueryable<ItemCollection> GetItemsWithCollections()
        {
            return Db.Collections.Join(Db.Items, c => c.Id, i => i.CollectionId,
                (c, i) => new ItemCollection { Item = i, Collection = c })
                .OrderByDescending(c => c.Item.Id);
        }
        public void Save()
        {
            Db.SaveChanges();
        }

        private static int CompareBySelectedField(int fieldIndex, CollectionItem e1, CollectionItem e2)
        {
            var f1 = new FieldManager(e1).GetFieldByIndex(fieldIndex);
            var f2 = new FieldManager(e2).GetFieldByIndex(fieldIndex);
            if (f1 == null)
                return -1;
            if (f2 == null)
                return 1;
            return f1.CompareTo(f2);
        }
        private IQueryable<CollectionItem> GetItemsByCollectionId(int id)
        {
            return Db.Items.Where(i => i.CollectionId == id);
        }
        private List<ItemCollection> MakeSearchQuery(string searchString, IQueryable<ItemCollection> model)
        {
            List<ItemCollection> results = SearchInItems(searchString, model).ToList();
            results.AddRange(SearchInComments(searchString,model));
            return results.Distinct().ToList();
        }

       
        private IQueryable<ItemCollection> SearchInComments(string searchString, IQueryable<ItemCollection> model)
        {
            return from elements in model
                   join comments in Db.Comments
                   on elements.Item.Id equals comments.ItemId
                   where comments.Content.Contains(searchString)
                   select elements;
        }

        private IQueryable<ItemCollection> SearchInItems(string searchString, IQueryable<ItemCollection> model)
        {
            return (from items in model
                    where items.Item.Name.Contains(searchString) || items.Item.Tags.Contains(searchString)
                   || items.Item.StringField1.Contains(searchString) || items.Item.StringField2.Contains(searchString)
                   || items.Item.StringField3.Contains(searchString) || items.Item.TextField1.Contains(searchString)
                   || items.Item.TextField2.Contains(searchString) || items.Item.TextField3.Contains(searchString)
                   || items.Collection.ShortDescription.Contains(searchString)
                    select items
            ) ;
        }

    }
}