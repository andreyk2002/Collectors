using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class StartModel
    {
        public List<Collection> BiggestCollections { get; set; }

        public List<CollectionItem> LatestItems { get; set; }

        public string[] Tags { get; set; }
    }
}
