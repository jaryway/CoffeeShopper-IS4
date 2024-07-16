using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
//using Microsoft.EntityFrameworkCore.Migrations.Operations
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TypeInfo = System.Reflection.TypeInfo;

namespace DynamicSpace.Design
{
    public class DynamicMigrationsAssembly : MigrationsAssembly
    {
        //private readonly IMigrationsIdGenerator _idGenerator;
        //private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        //private IReadOnlyDictionary<string, TypeInfo>? _migrations;
        //private ModelSnapshot? _modelSnapshot;
        //private readonly Type _contextType;
        //private readonly ICurrentDbContext _currentContext;
        public DynamicMigrationsAssembly(ICurrentDbContext currentContext, IDbContextOptions options, IMigrationsIdGenerator idGenerator, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger) : base(currentContext, options, idGenerator, logger)
        {
            var _contextType = currentContext.Context.GetType();
            var injectAssembly = (Assembly?)_contextType.GetProperty("Assembly")?.GetValue(currentContext.Context);

            var assemblyName = RelationalOptionsExtension.Extract(options).MigrationsAssembly;
            Assembly = injectAssembly ?? (assemblyName == null
                ? _contextType.Assembly
                : Assembly.Load(new AssemblyName(assemblyName)));

            //_idGenerator = idGenerator;
            //_logger = logger;
            //_currentContext = currentContext;
        }

        public override Assembly Assembly { get; }
    }
}
