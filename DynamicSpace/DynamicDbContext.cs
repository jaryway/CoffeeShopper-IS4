
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

        #region 动态实体增删改查

        //public object CreateDynamicClass(Type classType, object entity)
        //{

        //    throw new NotImplementedException();

        //    //this.Set

        //    //var dd = this.Set(classType);
        //}

        public TEntity Update<TEntity>(TEntity entity) where TEntity : DynamicClassBase
        {
            Set<TEntity>().Update(entity);
        }

        public void Delete<TEntity>(long id) where TEntity : DynamicClassBase
        {
            var entity = GetByKey<TEntity>(id);

            if (entity == null)
            {
                return;
            }

            Set<TEntity>().Remove(entity);
        }

        public TEntity? GetByKey<TEntity>(long id) where TEntity : DynamicClassBase
        {
            return Set<TEntity>().FirstOrDefault(m => m.Id == id);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : DynamicClassBase
        {
            return Set<TEntity>();
        }

        #endregion

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
