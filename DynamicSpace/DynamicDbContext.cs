
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Attributes;
using DynamicSpace.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicSpace
{
    public class DynamicDbContext : DbContext
    {
        private readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;

        public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
        {
            dynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance();
        }

        public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

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
