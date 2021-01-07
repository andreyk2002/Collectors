using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class CollectionEditModel
    {
        [Required]
        public int CollectionId { get; set; }

        [Required]
        [Display(Name = "Short description")]
        [StringLength(1000)]
        public string ShortDescription { get; set; }

        [Display(Name = "Image for collection")]
        public IFormFile Image { get; set; }

        public byte[] OldImage { get; set; }

        public bool ShouldDeleteImage { get; set; }
    }
}
