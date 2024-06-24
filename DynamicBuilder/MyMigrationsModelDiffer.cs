using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Update.Internal;
//using Microsoft.EntityFrameworkCore.Relational;

namespace DynamicBuilder
{
    public class MyMigrationsModelDiffer : MigrationsModelDiffer
    {
        //public MyMigrationsModelDiffer()
        //{
        //}
        public MyMigrationsModelDiffer(IRelationalTypeMappingSource typeMappingSource, IMigrationsAnnotationProvider migrationsAnnotationProvider, IRowIdentityMapFactory rowIdentityMapFactory, CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(typeMappingSource, migrationsAnnotationProvider, rowIdentityMapFactory, commandBatchPreparerDependencies)
        {
        }

        protected override IEnumerable<MigrationOperation> Diff(
        IColumn source,
        IColumn target,
        DiffContext diffContext)
        {
            // 修改字段名称时，要新增一个字段
            if (source.Name != target.Name)
            {
                return base.Add(target, diffContext);
            }

            return base.Diff(source, target, diffContext);
        }
    }
}

