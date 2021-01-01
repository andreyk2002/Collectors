using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class Collection
    {
        public CollectionTheme Theme { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public IList<ItemFileld> AdditionalItemsFields { get; set; }

    } 
}
