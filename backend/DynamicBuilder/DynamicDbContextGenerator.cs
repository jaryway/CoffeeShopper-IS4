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
using TypeInfo = System.Reflection.TypeInfo;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using System.Linq.Dynamic.Core;
using DynamicBuilder.Data;
using DynamicBuilder.Attributes;

namespace DynamicSpace;

public class DynamicDbContextGenerator
{
    //private readonly static string projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
    //private readonly string outputDir = Path.GetFullPath(Path.Combine(projectDir, "./_Migrations"));
    private readonly static string MigrationDirectory = Path.Combine("_Migrations");
    private readonly static string ModelDirectory = Path.Combine("Models");//
    private readonly static string AssemblyName = "DynamicDbContextAssembly";
    private readonly static string DbContextName = "DynamicDbContext";

    private readonly ApplicationDbContext _applicationDbContext;
    private AssemblyLoadContext _assemblyLoadContext;
    private Compilation? _compilation;
    private Assembly? _assembly;
    private DbContext? _dbContext;
    private bool hasPendingModelChanges = false;
    private readonly Dictionary<string, string> sourceCodeDict = new Dictionary<string, string>();

    public DynamicDbContextGenerator(ApplicationDbContext applicationDbContext)
    {
        _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        _applicationDbContext = applicationDbContext;
        Initialize();
    }

    private void Initialize()
    {
        _applicationDbContext.SourceCodes.ToList().ForEach(item =>
        {
            AddSourceCode(item.Name, item.Code);
        });
    }


    private Compilation SelfCompilation { get { return _compilation ??= Build(); } }

    public Assembly SelfAssembly
    {
        get
        {
            return _assembly ??= BuildAssembly(SelfCompilation);
        }
    }
    public DbContext SelfDbContext { get { return _dbContext ??= BuildDbContext(SelfAssembly); } }

    public object? Get(string entityName, int id)
    {
        var name = Path.Combine(ModelDirectory, entityName + ".cs");
        var enity = _applicationDbContext.SourceCodes.FirstOrDefault(m => m.Name == name);
        if (enity == null)
        {
            throw new Exception($"实体 {entityName} 不存在");
        }

        //_applicationDbContext.Set<SourceCode>()

        return GetQueryable(entityName)
            .Where("Id == @0", id)
            .FirstOrDefault();
    }

    private Type GetEntityType(string entityName)
    {
        return SelfAssembly.GetType($"{AssemblyName}.Models.{entityName}") ?? throw new NullReferenceException($"实体 {entityName} 为空");
    }

    private Type GetDbContextType()
    {
        return SelfAssembly.GetType($"{AssemblyName}.{DbContextName}") ?? throw new NullReferenceException("DbContext 为空");
    }

    public IQueryable<object> GetQueryable(string entityName)
    {
        var dbContextType = GetDbContextType();
        var entityType = GetEntityType(entityName);
        var query = dbContextType.GetMethod("Set", 1, [])?
                .MakeGenericMethod(entityType)?
                .Invoke(SelfDbContext, null); // DbSet<T>

        return (query as IQueryable<object>) ?? throw new NullReferenceException("IQueryable<object> 为空");
        //var queryable = typeof(DbSet<>).MakeGenericType(entityType)?
        //   .GetMethod("AsQueryable")?
        //   .Invoke(query, []); //IQueryable<T>

        //return (IQueryable)(queryable ?? throw new NullReferenceException($"实体 IQueryable 为空"));
    }

    public DynamicDbContextGenerator AddEntity(string entityName, string entityPropertiesCode, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using DynamicBuilder.Models;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        sb.AppendLine(string.Format("using {0};", typeof(EntityIdAttribute).Namespace));
        sb.AppendLine($"using {AssemblyName};");
        sb.AppendLine($"namespace {AssemblyName}.Models;");
        sb.AppendLine(string.Format($"[EntityId(123456)]"));
        sb.AppendLine(string.Format("[Table(\"{0}\")]", tableName));
        sb.AppendLine($"public class {entityName} : EntityBase{{");
        //sb.AppendLine($"public override string TableName => \"{tableName}\";");
        sb.AppendLine(entityPropertiesCode);
        sb.AppendLine("}");

        var entityFileName = Path.Combine(ModelDirectory, entityName + ".cs");
        var entityCode = sb.ToString();

        AddOrUpdateCode(entityFileName, entityCode, SourceCodeKind.Entity, tableName);
        _applicationDbContext.SaveChanges();

        return this;
    }

    public DynamicDbContextGenerator AddMigration(string? name = null)
    {
        EnsureBuild();
        //var dbContext = _dbContext!;
        //var migrationsAssembly = SelfDbContext.GetService<IMigrationsAssembly>();
        //var tttt = migrationsAssembly.GetType();
        //var m = migrationsAssembly.Migrations;

        //if (!SelfDbContext.Database.HasPendingModelChanges()) return this;

        var migrationName = name ?? ("M" + DateTime.Now.Ticks.ToString());
        using (var scope = SelfDbContext.GetService<IServiceScopeFactory>().CreateScope())
        {

            var scaffolder = GetMigrationsScaffolder(SelfDbContext);
            var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: AssemblyName);

            var modelSnapshotFileName = Path.Combine(MigrationDirectory, migration.SnapshotName + migration.FileExtension);
            var migrationFileName = Path.Combine(MigrationDirectory, migration.MigrationId + migration.FileExtension);
            var migrationMetadataFileName = Path.Combine(MigrationDirectory, migration.MigrationId + ".Designer" + migration.FileExtension);

            AddOrUpdateCode(modelSnapshotFileName, migration.SnapshotCode, SourceCodeKind.Snapshot);
            AddOrUpdateCode(migrationFileName, migration.MigrationCode, SourceCodeKind.Migration);
            AddOrUpdateCode(migrationMetadataFileName, migration.MetadataCode, SourceCodeKind.MigrationMetadata);

            _applicationDbContext.SaveChanges();
            return this;
        }
    }

    public void RemoveMigration(bool force)
    {
        EnsureBuild();
        var scaffolder = (DynamicMigrationsScaffolder)GetMigrationsScaffolder(_dbContext!);

        var result = scaffolder.RemoveMigration(force, null);
        var files = new[] { result.ModelSnapshotFileName, result.MigrationMetadataFileName, result.MigrationFileName };
        var ss = files.Select(m => Path.Combine(MigrationDirectory, m)).ToList();
        var list = _applicationDbContext.SourceCodes.Where(m => ss.Any(s => s == m.Name)).ToList();
        // 移除迁移文件
        list.ForEach(entity =>
        {
            var isSnapshot = entity.SourceCodeKind == SourceCodeKind.Snapshot;

            if (isSnapshot && result.ModelSnapshotCode != null)
            {
                entity.Code = result.ModelSnapshotCode;
                _applicationDbContext.SourceCodes.Update(entity);
            }
            else
            {
                _applicationDbContext.SourceCodes.Remove(entity);
            }
        });

        UpdateEntity(result.MigrationsToApply, result.MigrationsToRevert);
    }

    public void UpdateDatabase(string? migrationName = null)
    {
        EnsureBuild();
        try
        {
            var targetMigration = migrationName;
            var migrator = (DynamicMigrator)SelfDbContext.GetService<IMigrator>();

            var result = migrator.Migrate(migrationName);
            UpdateEntity(result.MigrationsToApply, result.MigrationsToRevert);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private void UpdateEntity(IReadOnlyList<Migration> migrationsToApply, IReadOnlyList<Migration> migrationsToRevert)
    {
        // 更新实体信息
        // 1、找到要删除的表
        // 2、找到要添加的表
        List<MigrationOperation> list = new List<MigrationOperation>();
        foreach (var migration in migrationsToRevert)
        {
            list.AddRange(migration.DownOperations.Where(m => m != null && (m is CreateTableOperation || m is DropTableOperation)));
        }

        foreach (var migration in migrationsToApply)
        {
            list.AddRange(migration.UpOperations.Where(m => m != null && (m is CreateTableOperation || m is DropTableOperation)));
        }

        var temp = list.Select(m =>
        {
            var name = m.GetAnnotation("EntityName").Value as string;
            name = name == null ? name : Path.Combine(ModelDirectory, name + ".cs");
            return new
            {
                Kind = (m is CreateTableOperation) ? "create" : "drop",
                Name = name
            };
        }).Where(m => m.Name != null).ToList();

        var names = temp.Select(m => m.Name);
        var entities = _applicationDbContext.SourceCodes
            .Where(m => m.SourceCodeKind == SourceCodeKind.Entity && names.Any(n => n == m.Name))
            .ToDictionary(k => k.Name, k => k);

        List<SourceCode> sources = new();

        temp.ForEach(m =>
        {
            var entity = entities[m.Name ?? ""];
            if (entity == null)
            {
                return;
            }
            entity.Published = m.Kind == "create";
            _applicationDbContext.SourceCodes.Update(entity);
        });

        _applicationDbContext.SaveChanges();

        Console.WriteLine(entities);

    }

    public Compilation Build()
    {
        var syntaxTrees = sourceCodeDict.Select(item =>
        {
            return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
        }).ToList();
        _dbContext?.Dispose();
        _dbContext = null;
        _assembly = null;

        var compilation = CreateCompilation();
        compilation = compilation.AddSyntaxTrees(syntaxTrees);

        hasPendingModelChanges = false;
        return compilation;
    }

    private void AddOrUpdateCode(string name, string code, SourceCodeKind sourceCodeKind, string tableName = "")
    {
        var entity = _applicationDbContext.SourceCodes.FirstOrDefault(m => m.Name == name);
        if (entity != null)
        {
            entity.Code = code;
            _applicationDbContext.SourceCodes.Update(entity);
        }
        else
        {
            _applicationDbContext.SourceCodes.Add(new SourceCode
            {
                Name = name,
                TableName = tableName,
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
        serviceCollection.AddScoped<IMigrator, DynamicMigrator>();
        serviceCollection.AddScoped<IMigrationsAssembly, DynamicMigrationsAssembly>();
        serviceCollection.AddScoped<IMigrationsModelDiffer, DynamicMigrationsModelDiffer>();
        serviceCollection.AddScoped<IMigrationsScaffolder, DynamicMigrationsScaffolder>();

        new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);


        var serviceProvider = serviceCollection.BuildServiceProvider();
        var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();

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
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Abstractions").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Pomelo.EntityFrameworkCore.MySql").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.Annotations").Location),
        };

        Compilation compilation = CSharpCompilation.Create(AssemblyName)
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
        }
    }

    private Assembly BuildAssembly(Compilation compilation)
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
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Abstractions").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Pomelo.EntityFrameworkCore.MySql").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.Annotations").Location),
        };

        var syntaxTrees = sourceCodeDict.Select(item =>
        {
            return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
        }).ToList();

        Compilation _compilation = CSharpCompilation.Create(AssemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTrees);

        using var ms = new MemoryStream();
        EmitResult result = _compilation.Emit(ms);
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

        Console.WriteLine($"Successfully created Assembly: {AssemblyName}");

        if (_assemblyLoadContext != null)
        {
            _assemblyLoadContext.Unload();
            _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        }

        return _assemblyLoadContext!.LoadFromStream(ms);
    }

    private DbContext BuildDbContext(Assembly assembly)
    {
        if (assembly == null) throw new NullReferenceException("assembly is null");

        var dbContextType = assembly.GetTypes().FirstOrDefault(m => m.Name == "DynamicDbContext") ?? throw new NullReferenceException("DynamicDbContext 为空");

        var connectionString = _applicationDbContext.Database.GetConnectionString();
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
        var optionsBuilderInstance = Activator.CreateInstance(optionsBuilderType) ?? throw new NullReferenceException("OptionsBuilderInstance 为空");

        // Get the UseMySql extension method
        optionsBuilderInstance = typeof(MySqlDbContextOptionsBuilderExtensions)?
            .GetMethod("UseMySql", new[] { optionsBuilderType, typeof(string), typeof(ServerVersion), typeof(Action<MySqlDbContextOptionsBuilder>) })?
            .Invoke(null, [optionsBuilderInstance, connectionString, serverVersion, null]);

        (optionsBuilderInstance as DbContextOptionsBuilder)?.ReplaceService<IMigrator, DynamicMigrator>();
        (optionsBuilderInstance as DbContextOptionsBuilder)?.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();

        var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
        var optionsValue = optionsBuilderType.GetProperty("Options", dbContextOptionsType)?.GetValue(optionsBuilderInstance);
        var constructor = dbContextType.GetConstructor(new[] { dbContextOptionsType });

        if (constructor == null || optionsValue == null)
        {
            throw new ArgumentException("constructor 为空");
        }

        return (DbContext)constructor.Invoke(new[] { optionsValue });
    }
}

