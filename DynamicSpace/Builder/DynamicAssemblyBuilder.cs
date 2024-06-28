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

namespace DynamicSpace.Builder
{
    public class DynamicAssemblyBuilder
    {
        private readonly string _assemblyName = "DynamicAssembly";
        private AssemblyLoadContext _assemblyLoadContext;
        private readonly Dictionary<string, string> sourceCodeDictionary = [];
        public DynamicAssemblyBuilder(string assemblyName)
        {
            _assemblyName = assemblyName;
            _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
        }
        public Assembly Build(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
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

            var syntaxTrees = keyValuePairs.Select(item =>
            {
                return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
            }).ToList();

            var compilation = CSharpCompilation.Create(_assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            ms.Seek(0, SeekOrigin.Begin);

            if (!result.Success)
            {
                var errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in errors)
                {
                    Console.WriteLine(diagnostic.ToString());
                    throw new Exception("Compilation error");
                }
            }

            Console.WriteLine($"Successfully created Assembly: {_assemblyName}");

            if (_assemblyLoadContext != null)
            {
                _assemblyLoadContext.Unload();
                _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
            }

            return _assemblyLoadContext!.LoadFromStream(ms);
        }
    }
}
