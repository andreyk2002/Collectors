using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class CollectionItem
    {
        const int AdditionalFieldsCount = 3;

        public CollectionItem()
        {
            IntegerFields = new Dictionary<string, int>(AdditionalFieldsCount);
            TextFields = new Dictionary<string, string>(AdditionalFieldsCount);
            StringFields = new Dictionary<string, string>(AdditionalFieldsCount);
            LogicFields = new Dictionary<string, bool>(AdditionalFieldsCount);
            DateFields = new Dictionary<string, DateTime>(AdditionalFieldsCount);
        }
        public long Id { get; set; }

        public int CollectionId { get; set; }
        public string Name { get; set; }
        //In the beggining this filds will be inactive
        public List<string> tags { get; set; }

        public Dictionary<string, int> IntegerFields { get; set; }

        public Dictionary<string, string> TextFields { get; set; }

        public Dictionary<string, string> StringFields { get; set; }

        public Dictionary<string, bool> LogicFields { get; set; }

        public Dictionary<string, DateTime> DateFields { get; set; }

    }
}
