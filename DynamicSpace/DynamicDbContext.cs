
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Attributes;
using DynamicSpace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicSpace
{
    public class DynamicDbContext : BaseDbContext
    {
        //private bool hasChanged = false;
        //private Assembly? assembly;
        private static readonly string dynamicAssemblyName = "DynamicAssembly";
        private static readonly string dynamicEntityNamespace = $"{dynamicAssemblyName}.Models";
        private static readonly string dynamicMigrationNamespace = $"{dynamicAssemblyName}.Migrations";
        private readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;
        private readonly ApplicationDbContext Context;
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options, ApplicationDbContext context) : base(options)
        {
            dynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance();
            Context = context;
            Console.WriteLine("DynamicDbContext.Ctor");
        }

        public Assembly Assembly => dynamicAssemblyBuilder.Assembly;

        #region 动态实体增删改查

        public object CreateDynamicClass(long classId, object entity)
        {
            throw new NotImplementedException();
        }

        public object UpdateDynamicClass(object entity)
        {
            throw new NotImplementedException();
        }

        public int RemoveDynamicClass(long id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DynamicClassBase> Query(long classId)
        {
            throw new NotImplementedException();
        }
        #endregion

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
