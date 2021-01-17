using Collectors.Data.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class ItemsListViewModel
    {
        public ItemsListViewModel()
        {
            Selected = new List<bool>();
            Items = new List<CollectionItem>();
            AdditionalFieldsIndexes = new List<int>();
            AdditionalFieldsNames = new List<string>();
        }

        [Required]
        public int CollectionId { get; set; }
        [Required]
        [Display(Name = "Selected")]
        public List<Boolean> Selected { get; set; }
        public List<CollectionItem> Items { get; set; }
        public List<string> AdditionalFieldsNames { get; set; }
        public List<int> AdditionalFieldsIndexes { get; set; }
    }
}
