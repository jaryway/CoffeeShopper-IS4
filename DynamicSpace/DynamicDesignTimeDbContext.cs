
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Attributes;
using DynamicSpace.Models;
using Microsoft.EntityFrameworkCore;


namespace DynamicSpace
{
    public class DynamicDesignTimeDbContext : DbContext
    {
        private readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;

        public DynamicDesignTimeDbContext(DbContextOptions<DynamicDesignTimeDbContext> options) : base(options)
        {
            dynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance(true);
        }

        public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

        public IQueryable<TEntity> Query<TEntity>() where TEntity : DynamicClassBase
        {
            return Set<TEntity>();
        }

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
