using System.Reflection;
using System.Runtime.Loader;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace Jaryway.DynamicSpace.DynamicWebApi
{
    public class DynamicAssemblyBuilder
    {
        private AssemblyLoadContext _assemblyLoadContext;
        private Assembly? assembly;
        private static DynamicAssemblyBuilder? instance;
        private static DynamicAssemblyBuilder? designTimeInstance;
        private static IServiceProvider? _serviceProvider;
        private int currentVersion = 0;
        private int nextVersion = 0;

        public static void Initialize(IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();
        }

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

        public void IncreaseVersion() => nextVersion++;

        public void NotifyUpdate()
        {
            var provider = _serviceProvider?.GetService<IActionDescriptorChangeProvider>() as GenericControllerActionDescriptorChangeProvider;
            if (provider != null && HasChanged)
            {
                provider.HasChanged = true;
                provider.TokenSource?.Cancel();
            }
        }

        private bool HasChanged => currentVersion != nextVersion;

        public Assembly Assembly
        {
            get
            {
                if (!HasChanged && assembly != null)
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
                    MetadataReference.CreateFromFile(typeof(DynamicDbContext).Assembly.Location),
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

                if (DesignTime)
                {
                    context.MigrationEntries.ToList().ForEach(item =>
                    {
                        syntaxTrees.Add(CSharpSyntaxTree.ParseText(item.Code).WithFilePath(item.MigrationId));
                    });
                }

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

            //var provider = _serviceProvider?.GetService<IActionDescriptorChangeProvider>() as DynamicActionDescriptorChangeProvider;
            //if (provider != null && currentVersion > 0)
            //{
            //    provider.HasChanged = true;
            //    provider.TokenSource?.Cancel();
            //}

            currentVersion = nextVersion;
            return assembly;
        }
    }
}
