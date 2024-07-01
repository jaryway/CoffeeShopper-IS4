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
        //private readonly string _assemblyName = "DynamicAssembly";
        private AssemblyLoadContext _assemblyLoadContext;
        private bool hasChanged = false;
        private Assembly? assembly;
        //private readonly Dictionary<string, MigrationEntry> migrationEntries = new();
        //private readonly Dictionary<long, DynamicClass> dynamicClasses = new();
        private static DynamicAssemblyBuilder? instance;
        private static DynamicAssemblyBuilder? designTimeInstance;
        //private readonly ApplicationDbContext _applicationDbContext;
        private static IServiceProvider? _serviceProvider;
        //private int version = 0;

        public static void Initialize(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        private DynamicAssemblyBuilder(bool disignTime = false)
        {
            _assemblyLoadContext = new(Guid.NewGuid().ToString(), true);
            DesignTime = disignTime;
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
        public static string AssemblyName => "DynamicAssembly";

        public void SetVersion() => hasChanged = true;

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

            using (var scope = _serviceProvider!.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()!;


                context.MigrationEntries.ToList().ForEach(item =>
                {
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(item.Code).WithFilePath(item.MigrationId));
                });

                var query = context.DynamicClasses.AsQueryable();

                query = DesignTime ? query.Where(p => p.EntityProperties_ != "") : query.Where(p => p.EntityProperties != "");

                query.ToList().ForEach(item =>
                {
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(item.GenerateCode(DesignTime)).WithFilePath(item.Name));
                });
            }

            var compilation = CSharpCompilation.Create(AssemblyName)
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

            Console.WriteLine($"Successfully created Assembly: {AssemblyName}");

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
