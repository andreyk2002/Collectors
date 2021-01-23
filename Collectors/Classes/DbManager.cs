using Collectors.Data;
using Collectors.Data.Classes;
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
            var Comments = Db.Comments.Where(i => i.ItemId == id).ToList();
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

        public List<CollectionItem> FindAll(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return new List<CollectionItem>();
            var result = MakeSearchQuery(searchString);
            return result;
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

        public void AddCollection(Collection c)
        {
            Db.Collections.Add(c);
            Save();
        }

        public void RemoveCollection(Collection c)
        {
            Db.Collections.Remove(c);
            Save();
            var itemsToRemove = Db.Items.Where(i => i.CollectionId == c.Id);
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
            var commentsToRemove = Db.Comments.Where(c => c.ItemId == i.Id);
            Db.Comments.RemoveRange(commentsToRemove);
            Save();
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
        private List<CollectionItem> MakeSearchQuery(string searchString)
        {
            List<CollectionItem> results = SearchInItems(searchString).ToList();
            results.AddRange(SearchInCollections(searchString));
            results.AddRange(SearchInComments(searchString));
            return results.ToList();
        }

        private IQueryable<CollectionItem> SearchInCollections(string searchString)
        {
            var themes = Enum.GetValues(typeof(CollectionTheme));
            var query = (from collections in Db.Collections
                         join items in Db.Items
                   on collections.Id equals items.CollectionId
                   where collections.ShortDescription.Contains(searchString)
                   select items);
            return query;
        }

        private IQueryable<CollectionItem> SearchInComments(string searchString)
        {
            return from items in Db.Items
                   join comments in Db.Comments
                   on items.Id equals comments.ItemId
                   where comments.Content.Contains(searchString)
                   select items;
        }

        private IQueryable<CollectionItem> SearchInItems(string searchString)
        {
            return (from items in Db.Items
                    where items.Name.Contains(searchString) || items.Tags.Contains(searchString)
                   || items.StringField1.Contains(searchString) || items.StringField2.Contains(searchString)
                   || items.StringField3.Contains(searchString) || items.TextField1.Contains(searchString)
                   || items.TextField2.Contains(searchString) || items.TextField3.Contains(searchString)
                    select items
            );
        }

    }
}