using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace DynamicBuilder;

public class DynamicEntityGenerator
{
    public static void GenerateDynamicEntity(string entityName, Dictionary<string, Type> properties)
    {
        // 创建一个C#语法树
        var syntaxTree = CSharpSyntaxTree.ParseText($@"
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicEntities
{{
    [Table(""{entityName}"")]
    public class {entityName}
    {{
        {string.Join("\n", properties.Select(p => new[] { $"public {p.Value.FullName} {p.Key} {{ get; set;}}" }))}
    }}
}}
");

        // 创建一个编译参数
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        // 创建一个编译器
        var compilation = CSharpCompilation.Create(
            "DynamicEntities.dll",
            new[] { syntaxTree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            compilationOptions);

        // 编译生成一个程序集
        string assemblyPath = Path.Combine(Path.GetTempPath(), "DynamicEntities.dll");
        using (var fs = new FileStream(assemblyPath, FileMode.Create, FileAccess.Write))
        {
            EmitResult emitResult = compilation.Emit(fs);
            if (emitResult.Success)
            {
                Console.WriteLine($"Dynamic entity class '{entityName}' saved to: {assemblyPath}");
            }
            else
            {
                // 编译失败, 打印错误信息
                foreach (Diagnostic diagnostic in emitResult.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
        }
    }
}

