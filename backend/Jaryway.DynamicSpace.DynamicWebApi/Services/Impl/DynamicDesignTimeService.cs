using Jaryway.DynamicSpace.DynamicWebApi.Design;
using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;

namespace Jaryway.DynamicSpace.DynamicWebApi.Services.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicDesignTimeService : IDynamicDesignTimeService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DynamicDesignTimeDbContext _dynamicDbContext;
        private readonly DynamicAssemblyBuilder _designTimeDynamicAssemblyBuilder;
        private readonly DynamicAssemblyBuilder _dynamicAssemblyBuilder;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationDbContext"></param>
        /// <param name="dynamicDbContext"></param>
        /// <param name="serviceProvider"></param>
        public DynamicDesignTimeService(ApplicationDbContext applicationDbContext,
            DynamicDesignTimeDbContext dynamicDbContext,
            IServiceProvider serviceProvider)
        {
            _applicationDbContext = applicationDbContext;
            _dynamicDbContext = dynamicDbContext;
            _serviceProvider = serviceProvider;
            _dynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance();
            _designTimeDynamicAssemblyBuilder = DynamicAssemblyBuilder.GetInstance(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public DynamicClass? Get(long id)
        {
            return _applicationDbContext.DynamicClasses.FirstOrDefault(m => m.Id == id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DynamicClass> GetList()
        {
            return _applicationDbContext.DynamicClasses.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DynamicClass Create(DynamicClass dynamicClass)
        {
            if (_applicationDbContext.DynamicClasses.Any(m => m.Name == dynamicClass.Name))
            {
                throw new Exception($"类名{dynamicClass.Name}已经存在");
            }

            _applicationDbContext.DynamicClasses.Add(dynamicClass);
            _applicationDbContext.SaveChanges();

            return dynamicClass;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        public DynamicClass Update(DynamicClass dynamicClass)
        {
            _applicationDbContext.DynamicClasses.Update(dynamicClass);
            _applicationDbContext.SaveChanges();

            return dynamicClass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        public int Remove(DynamicClass dynamicClass)
        {
            _applicationDbContext.DynamicClasses.Remove(dynamicClass);
            var e = _applicationDbContext.SaveChanges();
            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationName"></param>
        public void Generate(string? migrationName = null)
        {
            try
            {
                _designTimeDynamicAssemblyBuilder.IncreaseVersion();

                if (AddMigration(migrationName))
                {
                    _designTimeDynamicAssemblyBuilder.IncreaseVersion();
                }

                using var scope = _serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<DynamicDesignTimeDbContext>()!;
                var targetMigration = migrationName;
                var migrator = (IDynamicMigrator)db.GetService<IMigrator>();

                var result = migrator.Migrate(migrationName);
                UpateEntityPublishState(result.MigrationsToApply, result.MigrationsToRevert);
                _dynamicAssemblyBuilder.IncreaseVersion();
                _dynamicAssemblyBuilder.NotifyUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool AddMigration(string? name = null)
        {
            if (!_dynamicDbContext.Database.HasPendingModelChanges())
            {
                return false;
            }

            var migrationName = "Dynamic_" + (name ?? DateTime.Now.Ticks.ToString());
            //using (var scope = this.GetService<IServiceScopeFactory>().CreateScope())
            //{
            var scaffolder = GetMigrationsScaffolder();
            var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: DynamicAssemblyBuilder.AssemblyName);

            var migration1 = new MigrationEntry { MigrationId = migration.MigrationId, Code = migration.MigrationCode };
            var migration2 = new MigrationEntry { MigrationId = migration.MigrationId + ".Designer", Code = migration.MetadataCode };

            _applicationDbContext.MigrationEntries.Add(migration1);
            _applicationDbContext.MigrationEntries.Add(migration2);

            var snapshot = _applicationDbContext.MigrationEntries.FirstOrDefault(e => e.MigrationId == migration.SnapshotName);
            if (snapshot == null)
            {
                snapshot = new MigrationEntry { MigrationId = migration.SnapshotName, Code = migration.SnapshotCode };
                _applicationDbContext.MigrationEntries.Add(snapshot);
            }
            else
            {
                snapshot.Code = migration.SnapshotCode;
                _applicationDbContext.MigrationEntries.Update(snapshot);
            }

            _applicationDbContext.SaveChanges();

            return true;
            //dynamicAssemblyBuilder.AddMigrations(migration1, migration2, snapshot);
            //}
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

            var operations = migrationOperations.Select(m =>
            {
                var annotation = m.GetAnnotation("EntityId");
                return new
                {
                    EntityId = (long)(annotation?.Value ?? 0L),
                    Kind = (m is CreateTableOperation) ? "create" : "drop",
                };
            }).ToList();

            var entityIds = operations.Select(m => m.EntityId);

            var entities = _applicationDbContext.DynamicClasses
                .Where(m => entityIds.Any(id => id == m.Id))
                .ToDictionary(p => p.Id, p => p);

            operations.ForEach(m =>
            {
                entities.TryGetValue(m.EntityId, out var entity);

                if (entity == null)
                {
                    return;
                }

                entity.Published = m.Kind == "create";
                entity.EntityProperties = entity.GetDesignTimeProperties();
                _applicationDbContext.DynamicClasses.Update(entity);
            });

            _applicationDbContext.SaveChanges();
        }

        private IMigrationsScaffolder GetMigrationsScaffolder()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddEntityFrameworkDesignTimeServices();
            serviceCollection.AddDbContextDesignTimeServices(_dynamicDbContext);

            serviceCollection.AddScoped<IMigrationsAssembly, DynamicMigrationsAssembly>();
            serviceCollection.AddScoped<IMigrationsModelDiffer, DynamicMigrationsModelDiffer>();
            serviceCollection.AddScoped<IMigrationsScaffolder, DynamicMigrationsScaffolder>();

            new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var scaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();

            return scaffolder;
        }

    }
}
