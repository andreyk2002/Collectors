using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models.Item
{
    public class ItemLikesModel
    {
        public CollectionItem Item { get; set; }

        public bool IsLiked { get; set; }
    }
}
