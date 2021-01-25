using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class ItemModel
    {
       
        [Required]
        [Display(Name = "Name")]
        [StringLength(255)]
        public string Name { get; set; }
        public string Tags { get; set; }
        public long ItemId { get; set; }
        public int CollectionId { get; set; }
        public List<string> AdditionalFieldsNames { get; set; }
        public List<string> AdditionalFieldsValues { get; set; }
        public List<int> AdditionalFieldsIndexes { get; set; }
    
    }
}
