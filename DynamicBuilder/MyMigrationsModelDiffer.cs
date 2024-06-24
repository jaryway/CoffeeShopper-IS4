﻿using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        protected override IEnumerable<MigrationOperation> Remove(IColumn source, DiffContext diffContext)
        {
            //return base.Remove(source, diffContext);
            RenameColumnOperation renameColumnOperation = new RenameColumnOperation
            {
                Schema = source.Table.Schema,
                Table = source.Table.Name,
                Name = source.Name,
                NewName = source.Name + "_Deprecated"
            };
            ((AnnotatableBase)renameColumnOperation).AddAnnotations(base.MigrationsAnnotationProvider.ForRename(source));
            //s.Add(renameColumnOperation);
            yield return renameColumnOperation;
        }

        protected override IEnumerable<MigrationOperation> Diff(
        IColumn source,
        IColumn target,
        DiffContext diffContext)
        {
            // 修改字段名称时，先新增一个字段，再将字段标记为已弃用
            if (source.Name != target.Name)
            {
                var s = base.Add(target, diffContext).ToList();
                RenameColumnOperation renameColumnOperation = new RenameColumnOperation
                {
                    Schema = source.Table.Schema,
                    Table = source.Table.Name,
                    Name = source.Name,
                    NewName = source.Name + "_Deprecated"
                };
                ((AnnotatableBase)renameColumnOperation).AddAnnotations(base.MigrationsAnnotationProvider.ForRename(source));
                s.Add(renameColumnOperation);

                return s;
            }

            return base.Diff(source, target, diffContext);
        }
    }
}

