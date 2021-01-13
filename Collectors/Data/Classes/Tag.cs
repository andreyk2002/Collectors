using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }
    }
}
