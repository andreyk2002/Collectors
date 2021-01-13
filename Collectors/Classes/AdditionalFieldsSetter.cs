using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public class AdditionalFieldsSetter : FieldsSetter
    {
        private CollectionItem Item { get; set; } 

        public void SetInt(string value, int index)
        {
            int i = Int32.Parse(value);
            if (index == 0)
                Item.IntField1 = i;
            else if (index == 1)
                Item.IntField2 = i;
            else
                Item.IntField3 = i;
        }

        public void SetString(string value, int index)
        {
            if (index == 0)
                Item.StringField1 = value;
            else if (index == 1)
                Item.StringField2 = value;
            else
                Item.StringField3 = value;
        }

        public void SetText(string value, int index)
        {
            if (index == 0)
                Item.TextField1 = value;
            else if (index == 1)
                Item.TextField2 = value;
            else
                Item.TextField3 = value;
        }

        public void SetBool(string value, int index)
        {
            bool b = Boolean.Parse(value);
            if (index == 0)
                Item.BoolField1 = b;
            else if (index == 1)
                Item.BoolField2 = b;
            else
                Item.BoolField3 = b;
        }

        public void SetDate(string value, int index)
        {
            DateTime dt = DateTime.Parse(value);
            if (index == 0)
                Item.DateField1 = dt;
            else if (index == 1)
                Item.DateField1 = dt;
            else
                Item.DateField1 = dt;
        }

    }
}
