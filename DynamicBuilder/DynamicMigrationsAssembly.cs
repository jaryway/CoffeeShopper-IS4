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
using DynamicBuilder.Metadata;
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

namespace DynamicBuilder
{
    internal class DynamicMigrationsAssembly : MigrationsAssembly
    {
        //private readonly IMigrationsIdGenerator _idGenerator;
        //private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        //private IReadOnlyDictionary<string, TypeInfo>? _migrations;
        //private ModelSnapshot? _modelSnapshot;
        //private readonly Type _contextType;
        //private readonly ICurrentDbContext _currentContext;

        //public List<MetadataEntity> _metaDataEntityList = new List<MetadataEntity>();
        public DynamicMigrationsAssembly(ICurrentDbContext currentContext, IDbContextOptions options, IMigrationsIdGenerator idGenerator, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger) : base(currentContext, options, idGenerator, logger)
        {
            var _contextType = currentContext.Context.GetType();
            //var prop = _contextType.GetProperty("Assembly").GetValue(null);
            var injectAssembly = (Assembly?)_contextType.GetProperty("Assembly")?.GetValue(currentContext.Context);

            var assemblyName = RelationalOptionsExtension.Extract(options).MigrationsAssembly;
            Assembly = injectAssembly == null ? (assemblyName == null
                ? _contextType.Assembly
                : Assembly.Load(new AssemblyName(assemblyName))) : injectAssembly;

            //_idGenerator = idGenerator;
            //_logger = logger;
            //_currentContext = currentContext;
            //_migrations = new Dictionary<string, TypeInfo>();
            var s = base.Migrations;
            Console.WriteLine("xxxxx");
        }

        public override Assembly Assembly { get; }


        //public override IReadOnlyDictionary<string, TypeInfo> Migrations
        //{
        //    get
        //    {
        //        //Debugger.Break();
        //        IReadOnlyDictionary<string, TypeInfo> Create()
        //        {
        //            var result = new SortedList<string, TypeInfo>();

        //            var context = (Data.DynamicDbContext)_currentContext.Context;
        //            //var s = context._metaDataEntityList;

        //            //foreach (var metadataEntity in context._metaDataEntityList)
        //            //{
        //            //    //modelBuilder.Entity(metadataEntity.EntityType).ToTable(metadataEntity.TableName, metadataEntity.SchemaName).HasKey("Id");

        //            //    //foreach (var metaDataEntityProp in metadataEntity.Properties)
        //            //    //{
        //            //    //    if (!metaDataEntityProp.IsNavigation)
        //            //    //    {
        //            //    //        var propBuilder = modelBuilder.Entity(metadataEntity.EntityType).Property(metaDataEntityProp.Name);

        //            //    //        if (!string.IsNullOrEmpty(metaDataEntityProp.ColumnName))
        //            //    //            propBuilder.HasColumnName(metaDataEntityProp.ColumnName);
        //            //    //    }
        //            //    //}
        //            //}
        //            //var assembly = BuildAssembly();

        //            var items = from t in context.Assembly.DefinedTypes.Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false })
        //                        where t.IsSubclassOf(typeof(Migration))
        //                          && t.GetCustomAttribute<DbContextAttribute>()?.ContextType == _contextType
        //                        let id = t.GetCustomAttribute<MigrationAttribute>()?.Id
        //                        orderby id
        //                        select (id, t);

        //            foreach (var (id, t) in items)
        //            {
        //                if (id == null)
        //                {
        //                    _logger.MigrationAttributeMissingWarning(t);

        //                    continue;
        //                }

        //                result.Add(id, t);
        //            }

        //            return result;
        //        }

        //        return _migrations ??= Create();
        //    }
        //}

        //public void AddMetadata(MetadataEntity metadataEntity) => _metaDataEntityList.Add(metadataEntity);

    }
}
