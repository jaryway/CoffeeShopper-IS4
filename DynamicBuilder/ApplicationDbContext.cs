using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using DynamicBuilder.Models;

namespace DynamicBuilder
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<DynamicEntity> DynamicEntities { get; set; }

        public DbSet<SourceCode> SourceCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");

            var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
            var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
            optionsBuilder.UseMySql(connectionString, serverVersion);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}