
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

        //public object UpdateDynamicClass(Type entityType, object entity)
        //{
        //    throw new NotImplementedException();
        //    // TODO: dbContext.Set<Type>().Update();
        //    //var dbSet = base.Set(classType);
        //    var s = (DynamicClassBase)entity;
        //    //var existingEntity = Find(entityType, s.Id) ?? throw new InvalidOperationException($"The entity of type '{entityType.Name}' with the primary key value '{s.Id}' does not exist in the database.");

        //}

        //public int RemoveDynamicClass(Type classType, long id)
        //{
        //    throw new NotImplementedException();

        //    var entity = GetByKey(classType, id);


        //    var s = Set<DynamicClassBase>().Remove(entity!);


        //    //return Query(classType);
        //}

        //public DynamicClassBase? GetByKey(Type type, long id)
        //{
        //    var query = GetType().GetMethod("Set")!.MakeGenericMethod(type).Invoke(this, null);
        //    return (query as IQueryable<DynamicClassBase>)!.FirstOrDefault(m => m.Id == id);
        //}

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
