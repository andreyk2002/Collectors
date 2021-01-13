using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class ItemsListViewModel
    {
        public int CollectionId { get; set; }
        public List<CollectionItem> Items { get; set; }
        public List<string> AdditionalFieldsNames { get; set; }
        public List<int> AdditionalFieldsIndexes { get; set; }
    }
}
