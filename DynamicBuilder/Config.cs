using DynamicBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBuilder
{
    public class Config
    {
        private readonly static string assemblyName = "DynamicDbContextAssembly";
        public static IEnumerable<SourceCode> SourceCodes => new[] {
            new SourceCode() { Id = 1, Name = Path.Combine("Models","EntityBase.cs"), SourceCodeKind=SourceCodeKind.Base,
                Code=$@"namespace {assemblyName}.Models; public abstract class EntityBase {{ }}"},
            new SourceCode() { Id = 2, Name="DynamicDbContext.cs", SourceCodeKind=SourceCodeKind.Base, Code=$@"
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {assemblyName}.Models;
namespace {assemblyName};

//public abstract class EntityBase {{ }}

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
"},
        };
    }
}
