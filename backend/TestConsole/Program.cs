// See https://aka.ms/new-console-template for more information
using Jaryway.DynamicSpace.DataAccess.Data;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");
var serverVersion = new MySqlServerVersion(Version.Parse("8.4.0"));
var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql("server=localhost;uid=root;pwd=123456;database=test", serverVersion);
var db = new ApplicationDbContext(builder.Options);


string className = "A";

string classString = $@"
public class {className} {{
    public int Id {{ get; set; }}
    public stirng Name {{ get; set; }}
}}
";

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classString);
Compilation compilation = CSharpCompilation.Create(
                "MyAssembly",
                new[] { syntaxTree },
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

using (var ms = new MemoryStream())
{
    EmitResult result = compilation.Emit(ms);
    if (!result.Success)
    {
        IEnumerable<Diagnostic> errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
        foreach (Diagnostic diagnostic in errors)
        {
            Console.WriteLine(diagnostic.ToString());
        }
    }
    else
    {
        Console.WriteLine($"Successfully created class: {className}");
    }
}
