using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Models
{
    public class UserModel
    {
        public IdentityUser User { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
    }
}
