
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

        public void AddDynamicClass(DynamicClass dynamicClass)
        {
            if (Context.DynamicClasses.Any(m => m.Name == dynamicClass.Name))
            {
                throw new Exception($"类名{dynamicClass.Name}已经存在");
            }

            Context.DynamicClasses.Add(dynamicClass);
            Context.SaveChanges();
            dynamicAssemblyBuilder.AddDynamicClasses(dynamicClass);
        }

        public void RemoveDynamicClass(DynamicClass dynamicClass)
        {
            Context.DynamicClasses.Remove(dynamicClass);
            Context.SaveChanges();
            dynamicAssemblyBuilder.RemoveDynamicClasses(dynamicClass);
        }

        public void AddMigration(string? name = null)
        {
            if (!Database.HasPendingModelChanges())
            {
                return;
            }

            var migrationName = "Dynamic_" + (name ?? DateTime.Now.Ticks.ToString());
            //using (var scope = this.GetService<IServiceScopeFactory>().CreateScope())
            //{
            var scaffolder = GetMigrationsScaffolder();
            var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: dynamicAssemblyName);

            var migration1 = new MigrationEntry { MigrationId = migration.MigrationId, Code = migration.MigrationCode };
            var migration2 = new MigrationEntry { MigrationId = migration.MigrationId + ".Designer", Code = migration.MetadataCode };

            Context.MigrationEntries.Add(migration1);
            Context.MigrationEntries.Add(migration2);

            var snapshot = Context.MigrationEntries.FirstOrDefault(e => e.MigrationId == migration.SnapshotName);
            if (snapshot == null)
            {
                snapshot = new MigrationEntry { MigrationId = migration.SnapshotName, Code = migration.SnapshotCode };
                Context.MigrationEntries.Add(snapshot);
            }
            else
            {
                snapshot.Code = migration.SnapshotCode;
                Context.MigrationEntries.Update(snapshot);
            }

            Context.SaveChanges();

            dynamicAssemblyBuilder.AddMigrations(migration1, migration2, snapshot);
            //}
        }

        public void RemoveMigration(bool force)
        {
            var scaffolder = (DynamicMigrationsScaffolder)GetMigrationsScaffolder();

            var result = scaffolder.RemoveMigration(force, null);

            var ids = UpdateMigrations(result);
            Context.SaveChanges();
            dynamicAssemblyBuilder.RemoveMigrations(ids);
        }

        public void UpdateDatabase(string? migrationName = null)
        {
            try
            {
                var targetMigration = migrationName;
                var migrator = (DynamicMySqlMigrator)this.GetService<IMigrator>();

                var result = migrator.Migrate(migrationName);
                UpateEntityPublishState(result.MigrationsToApply, result.MigrationsToRevert);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string[] UpdateMigrations(RemoveMigrationReuslt result)
        {
            var migrationIds = new[] { result.MigrationId, result.ModelSnapshotName, result.MigrationMetadataId };

            // 移除迁移文件
            Context.MigrationEntries.Where(m => migrationIds.Any(s => s == m.MigrationId))
            .ToList()
            .ForEach(m =>
            {
                if (m.MigrationId.EndsWith("ModelSnapshot") && !string.IsNullOrEmpty(result.ModelSnapshotCode))
                {
                    m.Code = result.ModelSnapshotCode;
                    Context.MigrationEntries.Update(m);
                }
                else
                {
                    Context.MigrationEntries.Remove(m);
                }
            });
            return migrationIds;
        }

        private void UpateEntityPublishState(IReadOnlyList<Migration> migrationsToApply, IReadOnlyList<Migration> migrationsToRevert)
        {
            // 更新实体信息
            // 1、找到要删除的表
            // 2、找到要添加的表
            var migrationOperations = new List<MigrationOperation>();
            Func<MigrationOperation, bool> func = (m => m != null && (m is CreateTableOperation || m is DropTableOperation));

            foreach (var migration in migrationsToRevert)
            {
                //migration.ActiveProvider
                migrationOperations.AddRange(migration.DownOperations.Where(func));
            }

            foreach (var migration in migrationsToApply)
            {
                migrationOperations.AddRange(migration.UpOperations.Where(func));
            }

            var operations = migrationOperations.Select(m => new
            {
                EntityId = (long)(m.GetAnnotation("EntityId").Value ?? 0),
                Kind = (m is CreateTableOperation) ? "create" : "drop",
            }).ToList();

            var entityIds = operations.Select(m => m.EntityId);

            var entities = Context.DynamicClasses
                .Where(m => entityIds.Any(id => id == m.Id))
                .ToDictionary(p => p.Id, p => p);

            operations.ForEach(m =>
            {
                var entity = entities[m.EntityId];

                if (entity == null)
                {
                    return;
                }

                entity.Published = m.Kind == "create";
                entity.EntityProperties = entity.EntityProperties_;
                Context.DynamicClasses.Update(entity);
            });
        }

        private IMigrationsScaffolder GetMigrationsScaffolder()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddEntityFrameworkDesignTimeServices();
            serviceCollection.AddDbContextDesignTimeServices(this);
            serviceCollection.AddScoped<IMigrator, DynamicMySqlMigrator>();
            serviceCollection.AddScoped<IMigrationsAssembly, DynamicMigrationsAssembly>();
            serviceCollection.AddScoped<IMigrationsModelDiffer, DynamicMigrationsModelDiffer>();
            serviceCollection.AddScoped<IMigrationsScaffolder, DynamicMigrationsScaffolder>();

            new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();

            return migrationsScaffolder;
        }

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
