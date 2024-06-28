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
using DynamicBuilder.Models;

namespace DynamicBuilder.Data.Builder
{
    public class DynamicAssemblyBuilder
    {
        private readonly static string AssemblyName = "DynamicAssembly";
        private AssemblyLoadContext _assemblyLoadContext;
        private readonly Dictionary<string, string> sourceCodeDictionary = [];
        public DynamicAssemblyBuilder()
        {
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
        };

            var syntaxTrees = keyValuePairs.Select(item =>
            {
                return CSharpSyntaxTree.ParseText(item.Value).WithFilePath(item.Key);
            }).ToList();

            Compilation _compilation = CSharpCompilation.Create(AssemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            using var ms = new MemoryStream();
            var result = _compilation.Emit(ms);
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

            Console.WriteLine($"Successfully created Assembly: {AssemblyName}");

            if (_assemblyLoadContext != null)
            {
                _assemblyLoadContext.Unload();
                _assemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
            }

            return _assemblyLoadContext!.LoadFromStream(ms);
        }
    }
}
