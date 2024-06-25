using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.IO;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DynamicBuilder;
using DynamicBuilder.Models;
using SourceCodeKind = DynamicBuilder.Models.SourceCodeKind;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System.Reflection;
using TypeInfo = System.Reflection.TypeInfo;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections.Generic;

namespace DynamicSpace;

public class DynamicDbContextGenerator
{
    //private readonly static string projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
    //private readonly string outputDir = Path.GetFullPath(Path.Combine(projectDir, "./_Migrations"));
    private readonly static string MigrationDirectory = Path.Combine("_Migrations");
    private readonly static string ModelDirectory = Path.Combine("Models");//
    private readonly static string assemblyName = "DynamicDbContextAssembly";

    private readonly ApplicationDbContext applicationDbContext;
    private AssemblyLoadContext _context;
    private DbContext _dbContext;
    private bool hasPendingModelChanges = false;
    private readonly Dictionary<string, string> sourceCodeDict = new Dictionary<string, string>();

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public DynamicDbContextGenerator(ApplicationDbContext applicationDbContext)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
        this._context = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        this.applicationDbContext = applicationDbContext;
        Initialize();
    }

    private void Initialize()
    {
        //sourceCodeDict.Add("Models/EntityBase.cs", entityBaseStr);
        //sourceCodeDict.Add("DynamicDbContext.cs", str);
        //if (!Directory.Exists(outputDir)) return;
        //string[] filePaths = Directory.GetFiles(outputDir, "*.cs");
        //foreach (string filePath in filePaths)
        //{
        //    string sourceCode = File.ReadAllText(filePath);
        //    var relativePath = Path.GetRelativePath(projectDir, filePath);
        //    AddSourceCode(relativePath, sourceCode);
        //}

        applicationDbContext.SourceCodes.ToList().ForEach(item =>
        {
            AddSourceCode(item.Name, item.Code);
        });
    }

    public DynamicDbContextGenerator AddEntity(string entityName, string entityPropertiesCode)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine($"using {assemblyName};");
        sb.AppendLine($"namespace {assemblyName}.Models;");
        sb.AppendLine($"public class {entityName} : EntityBase{{");
        sb.AppendLine(entityPropertiesCode);
        sb.AppendLine("}");

        var entityFileName = Path.Combine(ModelDirectory, entityName + ".cs");
        var entityCode = sb.ToString();

        AddOrUpdateCode(entityFileName, entityCode, SourceCodeKind.Entity);
        applicationDbContext.SaveChanges();

        return this;
    }



    public DynamicDbContextGenerator AddMigration(string? name = null)
    {
        EnsureBuild();
        var dbContext = this._dbContext!;
        if (!dbContext.Database.HasPendingModelChanges()) return this;

        var migrationName = name ?? ("M" + DateTime.Now.Ticks.ToString());
        using (var scope = dbContext.GetService<IServiceScopeFactory>().CreateScope())
        {

            var scaffolder = GetMigrationsScaffolder(dbContext);
            var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: assemblyName);


            var modelSnapshotFileName = Path.Combine(MigrationDirectory, migration.SnapshotName + migration.FileExtension);
            var migrationFileName = Path.Combine(MigrationDirectory, migration.MigrationId + migration.FileExtension);
            var migrationMetadataFileName = Path.Combine(MigrationDirectory, migration.MigrationId + ".Designer" + migration.FileExtension);

            AddOrUpdateCode(modelSnapshotFileName, migration.SnapshotCode, SourceCodeKind.Snapshot);
            AddOrUpdateCode(migrationFileName, migration.MigrationCode, SourceCodeKind.Migration);
            AddOrUpdateCode(migrationMetadataFileName, migration.MetadataCode, SourceCodeKind.MigrationMetadata);

            applicationDbContext.SaveChanges();

            //scaffolder.Save(projectDir, migration, outputDir);

            return this;

        }
    }

    public void RemoveMigration(bool force)
    {
        EnsureBuild();
        var scaffolder = (DynamicMigrationsScaffolder)GetMigrationsScaffolder(_dbContext!);

        var files = scaffolder.RemoveMigration(force, null);
        var ss = files.Keys.Select(m => Path.Combine(MigrationDirectory, m)).ToList();
        var list = applicationDbContext.SourceCodes.Where(m => ss.Any(s => s == m.Name)).ToList();

        list.ForEach(entity =>
        {
            var key = Path.GetFileName(entity.Name);
            var code = files[key];
            if (code == null)
            {
                applicationDbContext.SourceCodes.Remove(entity);
            }
            else
            {
                entity.Code = code;
                applicationDbContext.SourceCodes.Update(entity);
            }
        });

        applicationDbContext.SaveChanges();
    }

    public void UpdateDatabase(string? migrationName = null)
    {
        EnsureBuild();
        try
        {
            var targetMigration = migrationName;
            var migrator = (Migrator)_dbContext.GetService<IMigrator>();
            //var scaffolder = (Migrator)_dbContext.GetService<IMigrationsScaffolder>();
            var migrationsAssembly = _dbContext.GetService<IMigrationsAssembly>();
            var databaseProvider = _dbContext.GetService<IDatabaseProvider>();
            var appliedMigrationEntries = _dbContext.Database.GetAppliedMigrations().ToList();
            //var pendingMigrations = _dbContext.Database.GetPendingMigrations().ToList();
            var migrations = migrationsAssembly.Migrations.ToList();
            var _activeProvider = databaseProvider.Name;

            var appliedMigrations = new Dictionary<string, TypeInfo>();
            var unappliedMigrations = new Dictionary<string, TypeInfo>();
            var appliedMigrationEntrySet = new HashSet<string>(appliedMigrationEntries, StringComparer.OrdinalIgnoreCase);
            IReadOnlyList<Migration> migrationsToApply;  // 要应用的迁移
            IReadOnlyList<Migration> migrationsToRevert;// 待还原的迁移
            Migration? actualTargetMigration;
            //if (migrationsAssembly.Migrations.Count == 0)
            //{
            //    _logger.MigrationsNotFound(this, _migrationsAssembly);
            //}

            foreach (var (key, typeInfo) in migrationsAssembly.Migrations)
            {
                if (appliedMigrationEntrySet.Contains(key))
                {
                    appliedMigrations.Add(key, typeInfo);
                }
                else
                {
                    unappliedMigrations.Add(key, typeInfo);
                }
            }

            if (string.IsNullOrEmpty(targetMigration))
            {
                migrationsToApply = unappliedMigrations
                    .OrderBy(m => m.Key)
                    .Select(p => migrationsAssembly.CreateMigration(p.Value, _activeProvider))
                    .ToList();
                migrationsToRevert = Array.Empty<Migration>();
                actualTargetMigration = null;
            }
            else if (targetMigration == Migration.InitialDatabase)
            {
                migrationsToApply = Array.Empty<Migration>();
                migrationsToRevert = appliedMigrations
                    .OrderByDescending(m => m.Key)
                    .Select(p => migrationsAssembly.CreateMigration(p.Value, _activeProvider))
                    .ToList();
                actualTargetMigration = null;
            }
            else
            {
                targetMigration = migrationsAssembly.GetMigrationId(targetMigration);
                migrationsToApply = unappliedMigrations
                    .Where(m => string.Compare(m.Key, targetMigration, StringComparison.OrdinalIgnoreCase) <= 0)
                    .OrderBy(m => m.Key)
                    .Select(p => migrationsAssembly.CreateMigration(p.Value, _activeProvider))
                    .ToList();
                migrationsToRevert = appliedMigrations
                    .Where(m => string.Compare(m.Key, targetMigration, StringComparison.OrdinalIgnoreCase) > 0)
                    .OrderByDescending(m => m.Key)
                    .Select(p => migrationsAssembly.CreateMigration(p.Value, _activeProvider))
                    .ToList();
                actualTargetMigration = appliedMigrations
                    .Where(m => string.Compare(m.Key, targetMigration, StringComparison.OrdinalIgnoreCase) == 0)
                    .Select(p => migrationsAssembly.CreateMigration(p.Value, _activeProvider))
                    .SingleOrDefault();
            }

            List<DropTableOperation> dropTableOperations = new List<DropTableOperation>();

            migrationsToRevert.ToList().ForEach(m =>
            {
                var list = m.DownOperations.Where(operation => operation is DropTableOperation)
                    .Cast<DropTableOperation>();
                dropTableOperations.AddRange(list);
            });

            migrationsToApply.ToList().ForEach(m =>
            {
                var list = m.UpOperations.Where(operation => operation is DropTableOperation)
                    .Cast<DropTableOperation>();
                dropTableOperations.AddRange(list);
            });

            migrator.Migrate(migrationName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void Build()
    {
        var syntaxTrees = sourceCodeDict.Select(item =>
        {
            return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
        }).ToList();
        _dbContext?.Dispose();
        var compilation = CreateCompilation();
        compilation = compilation.AddSyntaxTrees(syntaxTrees);
        var assembly = BuildAssembly(compilation);
        _dbContext = BuildDbContext(assembly);
        hasPendingModelChanges = false;
    }

    private void AddOrUpdateCode(string name, string code, SourceCodeKind sourceCodeKind)
    {
        var entity = applicationDbContext.SourceCodes.FirstOrDefault(m => m.Name == name);
        if (entity != null)
        {
            entity.Code = code;
            applicationDbContext.SourceCodes.Update(entity);
        }
        else
        {
            applicationDbContext.SourceCodes.Add(new SourceCode
            {
                Name = name,
                Code = code,
                SourceCodeKind = sourceCodeKind,
            });
        }

        AddSourceCode(name, code);
    }

    private IMigrationsScaffolder GetMigrationsScaffolder(DbContext dbContext)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkDesignTimeServices();
        serviceCollection.AddDbContextDesignTimeServices(dbContext);
        serviceCollection.AddScoped<IMigrationsModelDiffer, DynamicMigrationsModelDiffer>();
        serviceCollection.AddScoped<IMigrationsScaffolder, DynamicMigrationsScaffolder>();
        new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
        //var migrationsAssembly = serviceProvider.GetRequiredService<IMigrationsAssembly>();
        //var m = migrationsAssembly.Migrations;

        return migrationsScaffolder;
    }

    private void EnsureBuild()
    {
        if (hasPendingModelChanges) Build();
    }

    private static Compilation CreateCompilation()
    {
        var references = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ApplicationDbContext).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Reflection").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Linq.Expressions").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Pomelo.EntityFrameworkCore.MySql").Location),
            MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location)
        };

        Compilation compilation = CSharpCompilation.Create(assemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
            .AddReferences(references);

        return compilation;
    }

    private void AddSourceCode(string key, string code)
    {
        hasPendingModelChanges = true;
        if (!sourceCodeDict.TryAdd(key, code))
        {
            sourceCodeDict[key] = code;
            //sourceCodeDict.Add(key, code);
        }
    }

    private Assembly BuildAssembly(Compilation compilation)
    {
        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);
        ms.Seek(0, SeekOrigin.Begin);

        if (!result.Success)
        {
            IEnumerable<Diagnostic> errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
            foreach (Diagnostic diagnostic in errors)
            {
                Console.WriteLine(diagnostic.ToString());
                throw new Exception("Compilation error");
            }
        }

        Console.WriteLine($"Successfully created Assembly: {assemblyName}");

        if (_context != null)
        {
            _context.Unload();
            _context = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        }

        return _context!.LoadFromStream(ms);
    }

    private DbContext BuildDbContext(Assembly assembly)
    {
        if (assembly == null) throw new NullReferenceException("assembly is null");

        var dbContextType = assembly.GetTypes().FirstOrDefault(m => m.Name == "DynamicDbContext") ?? throw new NullReferenceException("DynamicDbContext 为空");
        var serverVersion = new MySqlServerVersion(Version.Parse("8.4.0"));
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
        var optionsBuilderInstance = Activator.CreateInstance(optionsBuilderType) ?? throw new NullReferenceException("OptionsBuilderInstance 为空");
        //var dbConnection = applicationDbContext.Database.GetDbConnection()!;
        //dbConnection.ServerVersion 

        //applicationDbContext.Database.GetDbConnection

        // Get the UseMySql extension method
        optionsBuilderInstance = typeof(MySqlDbContextOptionsBuilderExtensions)?
            .GetMethod("UseMySql", new[] { optionsBuilderType, typeof(string), typeof(ServerVersion), typeof(Action<MySqlDbContextOptionsBuilder>) })?
            .Invoke(null, [optionsBuilderInstance, "server=localhost;uid=root;pwd=123456;database=test", serverVersion, null]);

        var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
        var optionsValue = optionsBuilderType.GetProperty("Options", dbContextOptionsType)?.GetValue(optionsBuilderInstance);
        var constructor = dbContextType.GetConstructor(new[] { dbContextOptionsType });

        if (constructor == null || optionsValue == null)
        {
            throw new ArgumentException("constructor 为空");
        }

        return (DbContext)constructor.Invoke(new[] { optionsValue });

        //var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
        //var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
        //var builder = new DbContextOptionsBuilder<DynamicDbContext>().UseMySql(connectionString, serverVersion);
        //return new DynamicDbContext(builder.Options, assembly);
    }
}

