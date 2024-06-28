
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Builder;
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
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;


namespace DynamicSpace
{
    public class DynamicDbContext : DbContext
    {

        private bool hasChanged = false;
        private Assembly? assembly;
        private static readonly string dynamicAssemblyName = "DynamicAssembly";
        private static readonly string dynamicEntityNamespace = $"{dynamicAssemblyName}.Models";
        private static readonly string dynamicMigrationNamespace = $"{dynamicAssemblyName}.Migrations";
        private readonly DynamicAssemblyBuilder dynamicAssemblyBuilder;
        private readonly ApplicationDbContext Context;
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options, ApplicationDbContext context) : base(options)
        {
            dynamicAssemblyBuilder = new DynamicAssemblyBuilder(dynamicAssemblyName);
            Context = context;
        }
        public Assembly? Assembly
        {
            get
            {
                if (hasChanged || assembly == null)
                {
                    var migrations = Context.MigrationEntries.ToList() ?? [];
                    var entities = Context.DynamicEntities.ToList() ?? [];

                    var migrationsCode = migrations.Select(p => new KeyValuePair<string, string>(p.MigrationId, p.Code));
                    var entitiesCode = entities.Select(p => new KeyValuePair<string, string>(string.Join(".", dynamicEntityNamespace, p.Name), p.GenerateCode()));

                    assembly = dynamicAssemblyBuilder.Build(migrationsCode.Concat(entitiesCode));
                    hasChanged = false;
                }
                return assembly;
            }
        }

        public void AddDynamicEntity(DynamicEntity dynamicEntity)
        {
            Context.DynamicEntities.Add(dynamicEntity);
            hasChanged = true;
            Context.SaveChanges();
        }

        public void AddMigration(string? name = null)
        {
            var hasPendingModelChanges = Database.HasPendingModelChanges();

            if (!Database.HasPendingModelChanges())
            {
                return;
            }

            var migrationName = "Dynamic_" + (name ?? DateTime.Now.Ticks.ToString());
            using (var scope = this.GetService<IServiceScopeFactory>().CreateScope())
            {

                var scaffolder = GetMigrationsScaffolder();
                var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: dynamicAssemblyName);

                Context.MigrationEntries.Add(new MigrationEntry { MigrationId = migration.MigrationId, Code = migration.MigrationCode });
                Context.MigrationEntries.Add(new MigrationEntry { MigrationId = migration.MigrationId + ".Designer", Code = migration.MetadataCode });

                var snapshot = Context.MigrationEntries.FirstOrDefault(e => e.MigrationId == migration.SnapshotName);
                if (snapshot == null)
                {
                    Context.MigrationEntries.Add(new MigrationEntry { MigrationId = migration.SnapshotName, Code = migration.SnapshotCode });
                }
                else
                {
                    snapshot.Code = migration.SnapshotCode;
                    Context.MigrationEntries.Update(snapshot);
                }
                Context.SaveChanges();
            }
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
                //Console.WriteLine(ex.ToString());
            }
        }

        public void RemoveMigration(bool force)
        {
            var scaffolder = (DynamicMigrationsScaffolder)GetMigrationsScaffolder();

            var result = scaffolder.RemoveMigration(force, null);

            UpdateMigrations(result);
            UpateEntityPublishState(result.MigrationsToApply, result.MigrationsToRevert);
            Context.SaveChanges();
        }
        private void UpdateMigrations(RemoveMigrationReuslt result)
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
                Context.MigrationEntries.Remove(m);
            });
        }
        private void UpateEntityPublishState(IReadOnlyList<Migration> migrationsToApply, IReadOnlyList<Migration> migrationsToRevert)
        {
            // 更新实体信息
            // 1、找到要删除的表
            // 2、找到要添加的表
            var migrationOperations = new List<MigrationOperation>();

            foreach (var migration in migrationsToRevert)
            {
                //migration.ActiveProvider
                migrationOperations.AddRange(migration.DownOperations.Where(m => m != null && (m is CreateTableOperation || m is DropTableOperation)));
            }

            foreach (var migration in migrationsToApply)
            {
                migrationOperations.AddRange(migration.UpOperations.Where(m => m != null && (m is CreateTableOperation || m is DropTableOperation)));
            }

            var operations = migrationOperations.Select(m => new
            {
                EntityId = (long)(m.GetAnnotation("EntityId").Value ?? 0),
                Kind = (m is CreateTableOperation) ? "create" : "drop",
            }).ToList();

            var entityIds = operations.Select(m => m.EntityId);

            var entities = Context.DynamicEntities
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
                Context.DynamicEntities.Update(entity);
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
              .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
              .ToList();

            foreach (var entityType in entityTypes ?? [])
            {

                var builder = typeof(ModelBuilder).GetMethod("Entity", [])?
                       .MakeGenericMethod(entityType)?
                       .Invoke(modelBuilder, null) as EntityTypeBuilder ?? throw new Exception("modelBuilder.Entity<T> 失败");

                var entityId = entityType.GetCustomAttribute<EntityIdAttribute>()!.EntityId;
                var tableName = entityType.GetCustomAttribute<TableAttribute>()!.Name;

                builder.ToTable((tableName ?? entityId.ToString())).HasAnnotation("EntityId", entityId);
            }

            base.OnModelCreating(modelBuilder);
        }

    }

}
