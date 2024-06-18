using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using TestWeb.Models;

namespace TestWeb
{
    public class ApplicationDbContext : DbContext
    {
        public int Count { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.Count = new Random().Next();
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //EntityTypeGenerator.RegisterEntities(modelBuilder);
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}

