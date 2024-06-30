using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using DynamicSpace.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DynamicSpace
{
    public class DynamicAssemblyBuilder
    {
        private readonly string _assemblyName = "DynamicAssembly";
        private AssemblyLoadContext _assemblyLoadContext;
        private bool hasChanged = false;
        private Assembly? assembly;
        private readonly Dictionary<string, MigrationEntry> migrationEntries = new();
        private readonly Dictionary<long, DynamicClass> dynamicClasses = new();
        private static DynamicAssemblyBuilder? instance;
        private static DynamicAssemblyBuilder? designTimeInstance;
        //private readonly ApplicationDbContext _applicationDbContext;
        private static IServiceProvider? _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        private DynamicAssemblyBuilder(bool disignTime = false)
        {
            _assemblyLoadContext = new(Guid.NewGuid().ToString(), true);
            DesignTime = disignTime;
            using (var scope = _serviceProvider!.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()!;

                AddMigrations(context.MigrationEntries.ToArray());
                var query = context.DynamicClasses.AsQueryable();

                query = DesignTime ? query.Where(p => p.EntityProperties_ != "") : query.Where(p => p.EntityProperties != "");

                AddDynamicClasses(query.ToArray());
            }
        }

        public static DynamicAssemblyBuilder GetInstance(bool disignTime = false)
        {
            if (disignTime)
            {
                return designTimeInstance ??= new DynamicAssemblyBuilder(disignTime);
            }
            return instance ??= new DynamicAssemblyBuilder(disignTime);
        }

        public bool DesignTime { get; }

        public Assembly Assembly
        {
            get
            {
                if (!hasChanged && assembly != null)
                {
                    return assembly;
                }

                assembly = Build();
                return assembly;
            }
        }

        public void AddDynamicClasses(params DynamicClass[] classes)
        {
            if (classes.Length == 0)
            {
                return;
            }

            foreach (var item in classes)
            {
                if (!dynamicClasses.TryAdd(item.Id, item))
                {
                    dynamicClasses[item.Id] = item;
                }
            }

            hasChanged = true;
        }

        public void RemoveDynamicClasses(params DynamicClass[] classes) => RemoveDynamicClasses(classes.Select(m => m.Id).ToArray());

        public void RemoveDynamicClasses(params long[] ids)
        {
            if (ids.Length == 0)
            {
                return;
            }

            foreach (var item in ids)
            {
                if (dynamicClasses.Remove(item))
                {
                    hasChanged = true;
                }
            }

        }

        public void AddMigrations(params MigrationEntry[] migrations)
        {
            if (migrations.Length == 0)
            {
                return;
            }

            foreach (var item in migrations)
            {
                if (!migrationEntries.TryAdd(item.MigrationId, item))
                {
                    migrationEntries[item.MigrationId] = item;
                }
            }

            hasChanged = true;
        }

        public void RemoveMigrations(params MigrationEntry[] migrations) => RemoveMigrations(migrations.Select(m => m.MigrationId).ToArray());

        public void RemoveMigrations(params string[] migrationIds)
        {
            if (migrationIds.Length == 0)
            {
                return;
            }

            foreach (var item in migrationIds)
            {
                if (migrationEntries.Remove(item))
                {
                    hasChanged = true;
                }
            }
        }

        private Assembly Build()
        {
            Console.WriteLine("DynamicAssemblyBuilder.Build");

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

            var syntaxTrees = new List<SyntaxTree>();

            migrationEntries.ToList().ForEach(item =>
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(item.Value.Code).WithFilePath(item.Value.MigrationId));
            });

            dynamicClasses.ToList().ForEach(item =>
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(item.Value.GenerateCode()).WithFilePath(item.Value.Name));
            });

            var compilation = CSharpCompilation.Create(_assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            Console.WriteLine("DynamicAssemblyBuilder.Build.count" + syntaxTrees.Count());
            ms.Seek(0, SeekOrigin.Begin);

            if (!result.Success)
            {
                var errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in errors)
                {
                    Console.WriteLine(diagnostic.ToString());
                    throw new Exception(diagnostic.ToString());
                }
            }

            Console.WriteLine($"Successfully created Assembly: {_assemblyName}");

            if (_assemblyLoadContext != null)
            {
                _assemblyLoadContext.Unload();
                _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
            }

            assembly = _assemblyLoadContext!.LoadFromStream(ms);
            hasChanged = false;
            return assembly;
        }
    }
}
