using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Data.Classes.Entity
{
    public class Like
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public string UserId { get; set; }
    }
}
