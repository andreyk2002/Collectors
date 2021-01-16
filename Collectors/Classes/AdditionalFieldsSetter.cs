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

        public List<int> IndexesFromMask(int mask)
        {
            List<int> indexes = new List<int>();
            int i = 15;
            while (mask != 0)
            {
                int val = (int)Math.Pow(2, i);
                if (mask - val >= 0)
                {
                    mask -= val;
                    indexes.Add(i);
                }
                i--;
            }
            indexes.Reverse();
            return indexes;
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