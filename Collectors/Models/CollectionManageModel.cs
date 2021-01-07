using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class CollectionManageModel
    {
        public List<Collection> Collections { get; set; }

        public int ChosenId { get; set; }

        public string ChosenCollectionId { get; set; }
    }
}
