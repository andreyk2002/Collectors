using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collectors.Data.Classes;

namespace Collectors.Models.Comments
{
    public class CommentModel
    {
        public long ItemId { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
