
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace Jaryway.DynamicSpace.DynamicWebApi
{
    public abstract class DynamicDbContextBase : DbContext
    {
        protected readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;
        public DynamicDbContextBase(DbContextOptions options) : base(options)
        {
            dynamicAssemblyBuilder = InitializeDynamicAssemblyBuilder();
        }

        public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

        protected abstract DynamicAssemblyBuilder InitializeDynamicAssemblyBuilder();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = Assembly?
              .GetTypes()
              .Where(t => typeof(DynamicClassBase).IsAssignableFrom(t) && !t.IsAbstract)
              .ToList();

            foreach (var entityType in entityTypes!)
            {
                var entityId = entityType.GetCustomAttribute<EntityIdAttribute>()!.EntityId;
                var tableName = entityType.GetCustomAttribute<TableAttribute>()!.Name;

                var builder = modelBuilder.Entity(entityType);

                builder.ToTable((tableName ?? entityId.ToString()))
                    .HasAnnotation("EntityId", entityId);
            }
        }
    }
}

