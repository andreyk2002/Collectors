using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class CollectionItem
    {
        public CollectionItem()
        {
        }
        public long Id { get; set; }

        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        //public int Likes { get; set; }
        public int IntField1 { get; set; }
        public int IntField2 { get; set; }
        public int IntField3 { get; set; }
        public string TextField1 { get ; set; }
        public string TextField2 { get; set; }
        public string TextField3 { get; set; }
        public string StringField1 { get; set; }
        public string StringField2 { get; set; }
        public string StringField3 { get; set; }
        public bool BoolField1 { get; set; }
        public bool BoolField2 { get; set; }
        public bool BoolField3 { get; set; }
        public DateTime DateField1 { get; set; }
        public DateTime DateField2 { get; set; }
        public DateTime DateField3 { get; set; }

    }
}
