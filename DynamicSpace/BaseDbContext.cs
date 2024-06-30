using System;
using Microsoft.EntityFrameworkCore;
using DynamicSpace.Models;

namespace DynamicSpace
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        { }

        //internal DbSet<DynamicClass> DynamicClasses { get; set; }

        //internal DbSet<MigrationEntry> MigrationEntries { get; set; }
    }
}

