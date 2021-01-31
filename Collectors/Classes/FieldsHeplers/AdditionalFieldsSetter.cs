using Collectors.Data.Classes;
using Collectors.Models;
using System;
using System.Collections.Generic;

namespace Collectors.Classes
{
    public class AdditionalFieldsSetter
    {
        public static void SetByIndexes(ItemModel ia, FieldManager m)
        {
            for (int i = 0; i < ia.AdditionalFieldsIndexes.Count; i++)
            {
                if (ia.AdditionalFieldsValues[i] != null)
                    m.SetFieldByIndex(ia.AdditionalFieldsIndexes[i], ia.AdditionalFieldsValues[i]);
            }
        }

        public void SetAdditional(ItemModel ia, CollectionItem c)
        {
            FieldManager m = new FieldManager(c);
            if (ia.AdditionalFieldsIndexes != null)
            {
                SetByIndexes(ia, m);
            }
        }
    }
}