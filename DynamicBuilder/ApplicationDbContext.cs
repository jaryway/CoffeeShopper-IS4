using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using DynamicBuilder.Models;
using DynamicBuilder;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using DynamicBuilder.Metadata;


namespace DynamicBuilder;

public abstract class EntityBase
{
    public virtual int Id { get; set; }
    //public virtual string TableName { get; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableNameAttribute : Attribute
{
    public string TableName { get; }

    public TableNameAttribute(string tableName)
    {
        TableName = tableName;
    }
}

public class ApplicationDbContext : DbContext
{
    private bool isDynamicMode = true;

    public ApplicationDbContext()
    { }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public Assembly? Assembly
    {
        get
        {
            //SourceCodes.Where(m => m.SourceCodeKind == SourceCodeKind.Entity);
            return new DynamicAssemblyBuilder().Build();
        }
    }

    //public void SetDynamicModeOn() => isDynamicMode = true;
    //public void SetDynamicModeOff() => isDynamicMode = false;

    //public DbSet<MetadataEntityProperty> MetadataEntityProperties { get; set; }

    //public DbSet<MetadataEntity> MetadataEntities { get; set; }

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
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{


    //    var entityTypes = Assembly
    //      .GetTypes()
    //      .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
    //      .ToList();

    //    foreach (var t in entityTypes)
    //    {

    //        var builder = typeof(ModelBuilder).GetMethod("Entity", [])?
    //               .MakeGenericMethod(t)?
    //               .Invoke(modelBuilder, null) as EntityTypeBuilder ?? throw new Exception("modelBuilder.Entity<T> 失败");

    //        var entityName = t!.Name!;
    //        var tableName = t.GetCustomAttribute<TableNameAttribute>()!.TableName;

    //        builder.ToTable(tableName ?? entityName).HasAnnotation("EntityName", entityName);
    //    }

    //    base.OnModelCreating(modelBuilder);
    //}
}


