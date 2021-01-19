using Collectors.Data.Classes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class CollectionCreateModel
    {
        public CollectionCreateModel()
        {
            AdditionalFields = new List<string>(15);
        }
        [Required]
        [Display(Name = "Short description")]
        [StringLength(1000)]
        public string ShortDescription { get; set; }

        [Required]
        public CollectionTheme CollectionTheme { get; set; }

        [Display(Name = "Image for collection")]
        public IFormFile Image { get; set; }

        public IList<string> AdditionalFields { get; set; }


    }
}
