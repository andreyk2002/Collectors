using Collectors.Data.Classes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Collectors.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionItem> Items { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
