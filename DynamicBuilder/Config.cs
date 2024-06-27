using DynamicBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBuilder
{

    public class MyServicePrivoder : IServiceProvider
    {
        public object? GetService(Type serviceType) => throw new NotImplementedException();
    }

    public class Config
    {
        private readonly static string assemblyName = "DynamicDbContextAssembly";
        private readonly static string enntityBaseCode = $@"
namespace {assemblyName}.Models;
public abstract class EntityBase
{{
    public virtual string TableName {{ get; }} = string.Empty;
}}";
        private readonly static string dynamicDbContextCode = $@"
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {assemblyName}.Models;
namespace {assemblyName};

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

             object? entityInstance = Activator.CreateInstance(t);
             var tableName = t?.GetProperty(""TableName"")?.GetValue(entityInstance) as string;

             builder.ToTable(tableName ?? t!.Name);
        }}

        base.OnModelCreating(modelBuilder);
    }}
}}
";
        public static IEnumerable<SourceCode> SourceCodes => new[] {
            new SourceCode()
            {
                Id = 1,
                Name = Path.Combine("Models", "EntityBase.cs"),
                SourceCodeKind = SourceCodeKind.Base,
                Code = enntityBaseCode,
            },
            new SourceCode()
            {
                Id = 2,
                Name = "DynamicDbContext.cs",
                SourceCodeKind = SourceCodeKind.Base,
                Code = dynamicDbContextCode
            }
        };
    }
}
