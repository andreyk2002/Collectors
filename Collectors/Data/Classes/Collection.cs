using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class Collection
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public byte ThemeId { get; set; }
        public string ShortDescription { get; set; }
        public byte[] Image { get; set; }

        public string AdditionalItemsFields { get; set; }

        public int SelectedFieldsMask { get; set; }

    }
}
