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


namespace DynamicBuilder;

public abstract class EntityBase
{
    public virtual int Id { get; set; }
    public virtual string TableName { get; } = string.Empty;
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    //public DbSet<DynamicEntity> DynamicEntities { get; set; }

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
        var entityTypes = Assembly.GetExecutingAssembly()
          .GetTypes()
          .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
          .ToList();

        foreach (var t in entityTypes)
        {

            var builder = typeof(ModelBuilder).GetMethod("Entity", [])?
                   .MakeGenericMethod(t)?
                   .Invoke(modelBuilder, null) as EntityTypeBuilder ?? throw new Exception("modelBuilder.Entity < T > 失败");

            object? entityInstance = Activator.CreateInstance(t);
            var tableName = t?.GetProperty("TableName")?.GetValue(entityInstance) as string;

            builder.ToTable(tableName ?? t!.Name).HasAnnotation("EntityName", t!.Name);
        }

        base.OnModelCreating(modelBuilder);
    }
}


