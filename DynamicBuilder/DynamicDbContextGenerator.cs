using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using System.Reflection;
using System.Runtime.Loader;

namespace DynamicBuilder;

internal class DynamicDbContextGenerator
{



    public DbContext? BuildDbContext()
    {
        throw new NotImplementedException();
    }

    public static Type? CompileCode()
    {

        var assemblyName = "DynamicDbContextAssembly";
        var str = $@"
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {assemblyName};

public abstract class EntityBase {{ }}

public class Book:EntityBase
{{
    public int BookId {{ get; set; }}
    public string Title {{ get; set; }}
    //public string Description {{ get; set; }}

    //public string Text {{ get; set; }}
    //public string Text2 {{ get; set; }}
    //public string Text3 {{ get; set; }}
    //public string Text4 {{ get; set; }}
    //public string Text5 {{ get; set; }}

    public DateTime Created {{ get; set; }}

    //public DateTime Updated {{ get; set; }}
    public int AuthorId {{ get; set; }}
    public Author Author {{ get; set; }}
}}
public class Author:EntityBase
{{
    public int AuthorId {{ get; set; }}
    public string FirstName {{ get; set; }}
    public string LastName {{ get; set; }}
    public ICollection<Book> Books {{ get; set; }} // = new List<Book>();
}}

public class ApplicationDbContext : DbContext
{{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {{
    }}
    //public DbSet<Book> Books {{ get; set; }}
    //public DbSet<Author> Authors {{ get; set; }}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        modelBuilder.Entity<Book>().ToTable(""books"");
        var entityTypes = Assembly.Load(""DynamicDbContextAssembly"")
             .GetTypes()
             .Where(t => t.IsAssignableFrom(typeof(EntityBase)) && !t.IsAbstract).ToList();

        foreach (var t in entityTypes)
        {{
            var paramters = new Type[] {{ t }};
            var builder = (EntityTypeBuilder)typeof(ModelBuilder).GetMethod(""Entity"", paramters)?.Invoke(modelBuilder, paramters);
            builder?.ToTable(t.Name + ""s"");
        }}

        base.OnModelCreating(modelBuilder);
    }}
}}
";
        var references = new[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICollection<>).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Reflection").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
            MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location)
        };
        var syntaxTrees = new[] { str }.Select(s => CSharpSyntaxTree.ParseText(s));

        Compilation compilation = CSharpCompilation.Create(assemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTrees);

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);
        ms.Seek(0, SeekOrigin.Begin);

        if (!result.Success)
        {
            IEnumerable<Diagnostic> errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
            foreach (Diagnostic diagnostic in errors)
            {
                Console.WriteLine(diagnostic.ToString());
                return null;
            }
        }

        Console.WriteLine($"Successfully created Assembly: {assemblyName}");

        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

        var a1 = Assembly.GetExecutingAssembly();
        var a2 = Assembly.GetCallingAssembly();
        var a3 = Assembly.GetEntryAssembly();

        var bookType = assembly.GetTypes().FirstOrDefault(m => m.Name == "Book");
        var dbContextType = assembly.GetTypes().FirstOrDefault(m => m.Name == "ApplicationDbContext");
        var serverVersion = new MySqlServerVersion(Version.Parse("8.4.0"));
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
        var optionsBuilderInstance = Activator.CreateInstance(optionsBuilderType);

        if (optionsBuilderInstance == null) throw new ArgumentException("OptionsBuilderInstance 为空");

        // Get the UseMySql extension method
        optionsBuilderInstance = typeof(MySqlDbContextOptionsBuilderExtensions)?
            .GetMethod("UseMySql", new[] { optionsBuilderType, typeof(string), typeof(ServerVersion), typeof(Action<MySqlDbContextOptionsBuilder>) })?
            .Invoke(null, [optionsBuilderInstance, "server=localhost;uid=root;pwd=123456;database=test", serverVersion, null]);

        var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
        var optionsValue = optionsBuilderType.GetProperty("Options", dbContextOptionsType)?.GetValue(optionsBuilderInstance);
        var constructor = dbContextType.GetConstructor(new[] { dbContextOptionsType });

        if (constructor == null || optionsValue == null || bookType == null)
        {
            throw new ArgumentException("constructor 为空");
        }

        var context = (DbContext)constructor.Invoke([optionsValue]);



        var query = dbContextType.GetMethod("Set", 1, [])?
                .MakeGenericMethod(bookType)?
                .Invoke(context, null); // DbSet<T>

        var queryable = typeof(DbSet<>).MakeGenericType(bookType)?
            .GetMethod("AsQueryable")?
            .Invoke(query, []); //IQueryable<T>

        var list = typeof(Enumerable).GetMethod("ToList")?
            .MakeGenericMethod(bookType)
            .Invoke(null, [queryable]);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkDesignTimeServices();
        serviceCollection.AddDbContextDesignTimeServices(context);
        MySqlDesignTimeServices? designTimeServices = new();
        designTimeServices.ConfigureDesignTimeServices(serviceCollection);


        var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var scaffolder = scope.ServiceProvider.GetRequiredService<IMigrationsScaffolder>();
        //var migrationsAssembly = scope.ServiceProvider.GetRequiredService<IMigrationsAssembly>();

        var fiels = scaffolder.RemoveMigration("./migrations", null, true, null);

        Console.WriteLine("assembly.GetType() {}" + assembly.GetType().Name);

        return assembly.GetType();

    }
}

