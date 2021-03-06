﻿using Collectors.Data.Classes;
using System;
using Markdig;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public class FieldManager : IFieldManager
    {
        const int FieldsForType = 3;
        public CollectionItem Item { get; set; }

        public FieldManager(CollectionItem item)
        {
            this.Item = item;
        }

        public string GetFieldByIndex(int index)
        {
            return index switch
            {
                0 => Item.IntField1.ToString(),
                1 => Item.IntField2.ToString(),
                2 => Item.IntField3.ToString(),
                3 => Item.StringField1,
                4 => Item.StringField2,
                5 => Item.StringField3,
                6 => FormatText(Item.TextField1),
                7 => FormatText(Item.TextField2),
                8 => FormatText(Item.TextField3),
                9 => Item.BoolField1.ToString(),
                10 => Item.BoolField2.ToString(),
                11 => Item.BoolField3.ToString(),
                12 => DateToString(Item.DateField1),
                13 => DateToString(Item.DateField2),
                14 => DateToString(Item.DateField3),
                _ => "",
            };
        }

        private string FormatText(string tf)
        {
            return (tf != null) ? Markdown.ToHtml(tf) : "";
        }

        public void SetFieldByIndex(int index, string s)
        {
            if (index < 3)
                SetInt(index, s);
            else if (index < 6)
                SetString(index % FieldsForType, s);
            else if (IsTextField(index))
                SetText(index % FieldsForType, s);
            else if (index < 12)
                SetBool(index % FieldsForType, s);
            else
                SetDate(index % FieldsForType, s);
        }

        public static bool IsTextField(int index)
        {
            return index >= 6 && index < 9;
        }

        private string DateToString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static string GetTypeByIndex(int index)
        {
            if (3 > index)
                return "number";
            if (6 > index)
                return "text";
            if (9 > index)
                return "textarea";
            if (12 > index)
                return "checkbox";
            return "datetime-local";
        }
        private void SetInt(int index, string value)
        {
            int i = Int32.Parse(value);
            if (index == 0)
                Item.IntField1 = i;
            else if (index == 1)
                Item.IntField2 = i;
            else
                Item.IntField3 = i;
        }

        private void SetString(int index, string value)
        {
            if (index == 0)
                Item.StringField1 = value;
            else if (index == 1)
                Item.StringField2 = value;
            else
                Item.StringField3 = value;
        }

        private void SetText(int index, string value)
        {
            if (index == 0)
                Item.TextField1 = value;
            else if (index == 1)
                Item.TextField2 = value;
            else
                Item.TextField3 = value;
        }

        private void SetBool(int index, string value)
        {
            bool b = Boolean.Parse(value);
            if (index == 0)
                Item.BoolField1 = b;
            else if (index == 1)
                Item.BoolField2 = b;
            else
                Item.BoolField3 = b;
        }

        private void SetDate(int index, string value)
        {
            DateTime dt = DateTime.Parse(value);
            if (index == 0)
                Item.DateField1 = dt;
            else if (index == 1)
                Item.DateField2 = dt;
            else
                Item.DateField3 = dt;
        }
    }
}
