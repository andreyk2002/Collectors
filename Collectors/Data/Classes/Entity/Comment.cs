using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes
{
    public class Comment
    {
        public int Id { get; set; }

        public long ItemId { get; set; }

        public string UserName { get; set; }

        public string Content { get; set; }
    }
}
