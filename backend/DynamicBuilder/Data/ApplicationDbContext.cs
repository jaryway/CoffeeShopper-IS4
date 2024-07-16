using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using DynamicBuilder.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
//using DynamicBuilder.Metadata;


namespace DynamicBuilder.Data;

public abstract class EntityBase
{
    public virtual int Id { get; set; }
    //public virtual string TableName { get; } = string.Empty;
}

public class ApplicationDbContext : DbContext
{
    //private bool isDynamicMode = true;

    public ApplicationDbContext()
    { }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }


    public DbSet<DynamicEntity> DynamicEntities { get; set; }
   
    public DbSet<MigrationEntry> MigrationEntries { get; set; }

    public DbSet<SourceCode> SourceCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder);
        //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");

        var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
        var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
        optionsBuilder.UseMySql(connectionString, serverVersion);
        //optionsBuilder.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}


