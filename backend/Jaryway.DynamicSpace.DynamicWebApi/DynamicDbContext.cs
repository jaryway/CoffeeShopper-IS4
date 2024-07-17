
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jaryway.DynamicSpace.DynamicWebApi
{
    public class DynamicDbContext : DynamicDbContextBase
    {
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
        {
            
        }

        protected override DynamicAssemblyBuilder InitializeDynamicAssemblyBuilder() => DynamicAssemblyBuilder.GetInstance();

        //public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    var entityTypes = Assembly?
        //      .GetTypes()
        //      .Where(t => typeof(DynamicClassBase).IsAssignableFrom(t) && !t.IsAbstract)
        //      .ToList();

        //    foreach (var entityType in entityTypes!)
        //    {
        //        var entityId = entityType.GetCustomAttribute<EntityIdAttribute>()!.EntityId;
        //        var tableName = entityType.GetCustomAttribute<TableAttribute>()!.Name;

        //        var builder = modelBuilder.Entity(entityType);

        //        builder.ToTable((tableName ?? entityId.ToString()))
        //            .HasAnnotation("EntityId", entityId);
        //    }
        //}
    }

}
