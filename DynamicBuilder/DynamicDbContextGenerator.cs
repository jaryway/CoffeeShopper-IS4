using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace DynamicSpace;

public class DynamicDbContextGenerator
{
    private readonly static string MigrationDirectory = Path.Combine("Migrations");
    private readonly static string ModelDirectory = Path.Combine("Models");//
    private readonly static string assemblyName = "DynamicDbContextAssembly";
    private readonly static string str = $@"
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using {assemblyName}.Models;
//namespace {assemblyName};

public abstract class EntityBase {{ }}

public class DynamicDbContext : DbContext
{{

    public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
    {{
    }}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        var entityTypes = Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
             .ToList();

        foreach (var t in entityTypes)
        {{
             var builder = typeof(ModelBuilder).GetMethod(""Entity"", [])?
                    .MakeGenericMethod(t)?
                    .Invoke(modelBuilder, null) as EntityTypeBuilder ?? throw new Exception(""modelBuilder.Entity<T> 失败"");

                builder.ToTable(t.Name + ""s"");
        }}

        base.OnModelCreating(modelBuilder);
    }}
}}
";
    private AssemblyLoadContext _context;
    private DbContext _dbContext;
    private bool hasPendingModelChanges = false;
    //private Compilation _compilation;
    private readonly Dictionary<string, string> sourceCodeDict = new Dictionary<string, string> { { "DynamicDbContext.cs", str } };

    public DynamicDbContextGenerator()
    {
        this._context = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        Build();
    }

    public DynamicDbContextGenerator AddEntity(string entityName, string entityPropertiesCode)
    {
        //var name = entityName.ToLower();
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        //sb.AppendLine($"namespace {assemblyName}.Models;");
        sb.AppendLine($"public class {entityName} : EntityBase{{");
        sb.AppendLine(entityPropertiesCode);
        sb.AppendLine("}");

        var entityFileName = Path.Combine(ModelDirectory, entityName + ".cs");
        AddSourceCode(entityFileName, sb.ToString());
        hasPendingModelChanges = true;
        return this;
    }

    private IMigrationsScaffolder GetMigrationsScaffolder(DbContext dbContext)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkDesignTimeServices();
        serviceCollection.AddDbContextDesignTimeServices(dbContext);
        new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();

        return migrationsScaffolder;
    }

    public DynamicDbContextGenerator AddMigration(string? name = null)
    {
        EnsureBuild();
        using var dbContext = this._dbContext!;

        if (!dbContext.Database.HasPendingModelChanges()) return this;

        var migrationName = name ?? DateTime.Now.Ticks.ToString();
        using (var scope = dbContext.GetService<IServiceScopeFactory>().CreateScope())
        {

            var scaffolder = GetMigrationsScaffolder(dbContext);
            var migration = scaffolder.ScaffoldMigration(migrationName, rootNamespace: assemblyName);

            var modelSnapshotFileName = Path.Combine(MigrationDirectory, migration.SnapshotName, migration.FileExtension);
            var migrationFileName = Path.Combine(MigrationDirectory, migration.MigrationId, migration.FileExtension);
            var migrationMetadataFileName = Path.Combine(MigrationDirectory, migration.MigrationId, ".Designer" + migration.FileExtension);

            AddSourceCode(modelSnapshotFileName, migration.SnapshotCode);
            AddSourceCode(migrationFileName, migration.MigrationCode);
            AddSourceCode(migrationMetadataFileName, migration.MetadataCode);

            return this;
        }
    }

    public void UpdateDatabase()
    {
        try
        {
            using var dbContext = this._dbContext;
            var migrator = dbContext.GetService<IMigrator>();
            migrator.Migrate();
        }
        catch (Exception ex)
        {
        }
    }

    public void Build()
    {
        var syntaxTrees = sourceCodeDict.Select(item =>
        {
            return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
        });
        this._dbContext?.Dispose();
        var compilation = CreateCompilation();
        compilation = compilation.AddSyntaxTrees(syntaxTrees);
        var assembly = BuildAssembly(compilation);
        this._dbContext = BuildDbContext(assembly);
        this.hasPendingModelChanges = false;
    }

    private void EnsureBuild()
    {
        if (hasPendingModelChanges) Build();
    }

    private static Compilation CreateCompilation()
    {
        var references = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
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
        if (!sourceCodeDict.TryAdd(key, code))
        {
            sourceCodeDict.Add(key, code);
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

    private static DbContext BuildDbContext(Assembly assembly)
    {
        if (assembly == null) throw new NullReferenceException("assembly is null");

        var dbContextType = assembly.GetTypes().FirstOrDefault(m => m.Name == "DynamicDbContext") ?? throw new NullReferenceException("DynamicDbContext 为空");
        var serverVersion = new MySqlServerVersion(Version.Parse("8.4.0"));
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
        var optionsBuilderInstance = Activator.CreateInstance(optionsBuilderType) ?? throw new NullReferenceException("OptionsBuilderInstance 为空");

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

        return (DbContext)constructor.Invoke([optionsValue]);
    }




}

