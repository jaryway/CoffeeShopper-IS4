
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Design;
using DynamicSpace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using DynamicSpace.Attributes;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace DynamicSpace
{
    public class DynamicDesignTimeDbContext : BaseDbContext
    {
        //private bool hasChanged = false;
        //private Assembly? assembly;
        private static readonly string dynamicAssemblyName = "DynamicAssembly";
        private static readonly string dynamicClassNamespace = $"{dynamicAssemblyName}.Models";
        private static readonly string dynamicMigrationNamespace = $"{dynamicAssemblyName}.Migrations";
        private readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;
        private readonly ApplicationDbContext Context;
        public DynamicDesignTimeDbContext(DbContextOptions<DynamicDesignTimeDbContext> options, ApplicationDbContext context) : base(options)
        {
            dynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance(true);
            Context = context;
            Console.WriteLine("DynamicDesignTimeDbContext.Ctor");
        }

        public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = Assembly?
              .GetTypes()
              .Where(t => typeof(DynamicClassBase).IsAssignableFrom(t) && !t.IsAbstract)
              .ToList();
            //Console.WriteLine("Ctor");
            Console.WriteLine("DynamicDbContext.OnModelCreating" + entityTypes?.Count);
            foreach (var entityType in entityTypes ?? [])
            {

                var builder = typeof(ModelBuilder).GetMethod("Entity", [])?
                       .MakeGenericMethod(entityType)?
                       .Invoke(modelBuilder, null) as EntityTypeBuilder ?? throw new Exception("modelBuilder.Entity<T> 失败");

                var entityId = entityType.GetCustomAttribute<EntityIdAttribute>()!.EntityId;
                var tableName = entityType.GetCustomAttribute<TableAttribute>()!.Name;

                builder.ToTable((tableName ?? entityId.ToString())).HasAnnotation("EntityId", entityId);
            }
            Console.WriteLine("OnModelCreating");
            base.OnModelCreating(modelBuilder);
        }
    }

}
