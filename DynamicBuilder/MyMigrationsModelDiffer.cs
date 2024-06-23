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

            //return base.Diff(source, target, diffContext);

            var table = target.Table;

            if (source.Name != target.Name)
            {
                var renameColumnOperation = new RenameColumnOperation
                {
                    Schema = table.Schema,
                    Table = table.Name,
                    Name = source.Name,
                    NewName = target.Name
                };

                renameColumnOperation.AddAnnotations(MigrationsAnnotationProvider.ForRename(source));

                yield return renameColumnOperation;
            }

            var sourceMigrationsAnnotations = source.GetAnnotations();
            var targetMigrationsAnnotations = target.GetAnnotations();

            var isNullableChanged = source.IsNullable != target.IsNullable;
            var columnTypeChanged = source.StoreType != target.StoreType;

            if (!source.TryGetDefaultValue(out var sourceDefault))
            {
                sourceDefault = null;
            }

            if (!target.TryGetDefaultValue(out var targetDefault))
            {
                targetDefault = null;
            }

            if (isNullableChanged
                || columnTypeChanged
                || source.DefaultValueSql != target.DefaultValueSql
                || source.ComputedColumnSql != target.ComputedColumnSql
                || source.IsStored != target.IsStored
                || sourceDefault?.GetType() != targetDefault?.GetType()
                || (sourceDefault != DBNull.Value && !target.ProviderValueComparer.Equals(sourceDefault, targetDefault))
                || source.Comment != target.Comment
                || source.Collation != target.Collation
                || source.Order != target.Order
                || HasDifferences(sourceMigrationsAnnotations, targetMigrationsAnnotations))
            {
                var isDestructiveChange = isNullableChanged && source.IsNullable
                    // TODO: Detect type narrowing
                    || columnTypeChanged;

                var alterColumnOperation = new AlterColumnOperation
                {
                    Schema = table.Schema,
                    Table = table.Name,
                    Name = target.Name,
                    IsDestructiveChange = isDestructiveChange
                };

                base.InitializeColumnHelper(alterColumnOperation, target, inline: !source.IsNullable);
                base.InitializeColumnHelper(alterColumnOperation.OldColumn, source, inline: true);

                if (source.Order != target.Order)
                {
                    if (source is not JsonColumn && source.Order.HasValue)
                    {
                        alterColumnOperation.OldColumn.AddAnnotation(RelationalAnnotationNames.ColumnOrder, source.Order.Value);
                    }

                    if (target is not JsonColumn && target.Order.HasValue)
                    {
                        alterColumnOperation.AddAnnotation(RelationalAnnotationNames.ColumnOrder, target.Order.Value);
                    }
                }

                yield return alterColumnOperation;
            }
        }

        private void Initialize(
        ColumnOperation columnOperation,
        IColumn column,
        RelationalTypeMapping typeMapping,
        bool isNullable,
        IEnumerable<IAnnotation> migrationsAnnotations,
        bool inline = false)
        {
            if (column.DefaultValue == DBNull.Value)
            {
                throw new InvalidOperationException(
                    RelationalStrings.DefaultValueUnspecified(
                        column.Table.SchemaQualifiedName,
                        column.Name));
            }

            if (column.DefaultValueSql?.Length == 0)
            {
                throw new InvalidOperationException(
                    RelationalStrings.DefaultValueSqlUnspecified(
                        column.Table.SchemaQualifiedName,
                        column.Name));
            }

            if (column.ComputedColumnSql?.Length == 0)
            {
                throw new InvalidOperationException(
                    RelationalStrings.ComputedColumnSqlUnspecified(
                        column.Name,
                        column.Table.SchemaQualifiedName));
            }

            var property = column.PropertyMappings.First().Property;
            var valueConverter = GetValueConverter(property, typeMapping);
            columnOperation.ClrType
                = (valueConverter?.ProviderClrType
                    ?? typeMapping.ClrType).UnwrapNullableType();

            if (!column.TryGetDefaultValue(out var defaultValue))
            {
                // for non-nullable collections of primitives that are mapped to JSON we set a default value corresponding to empty JSON collection
                defaultValue = !UseOldBehavior32972
                    && !inline
                    && column is { IsNullable: false, StoreTypeMapping: { ElementTypeMapping: not null, Converter: ValueConverter columnValueConverter } }
                    && columnValueConverter.GetType() is Type { IsGenericType: true } columnValueConverterType
                    && columnValueConverterType.GetGenericTypeDefinition() == typeof(CollectionToJsonStringConverter<>)
                    ? "[]"
                    : null;
            }

            columnOperation.ColumnType = column.StoreType;
            columnOperation.MaxLength = column.MaxLength;
            columnOperation.Precision = column.Precision;
            columnOperation.Scale = column.Scale;
            columnOperation.IsUnicode = column.IsUnicode;
            columnOperation.IsFixedLength = column.IsFixedLength;
            columnOperation.IsRowVersion = column.IsRowVersion;
            columnOperation.IsNullable = isNullable;
            columnOperation.DefaultValue = defaultValue
                ?? (inline || isNullable
                    ? null
                    : base.GetDefaultValue(columnOperation.ClrType));
            columnOperation.DefaultValueSql = column.DefaultValueSql;
            columnOperation.ComputedColumnSql = column.ComputedColumnSql;
            columnOperation.IsStored = column.IsStored;
            columnOperation.Comment = column.Comment;
            columnOperation.Collation = column.Collation;
            columnOperation.AddAnnotations(migrationsAnnotations);
        }

        private void InitializeColumnHelper(ColumnOperation columnOperation, IColumn column, bool inline)
        {
            if (column is JsonColumn jsonColumn)
            {
                InitializeJsonColumn(columnOperation, jsonColumn, jsonColumn.IsNullable, column.GetAnnotations(), inline);
            }
            else
            {
                var columnTypeMapping = column.StoreTypeMapping;

                Initialize(
                    columnOperation, column, columnTypeMapping, column.IsNullable,
                    column.GetAnnotations(), inline);
            }
        }
        private static ValueConverter? GetValueConverter(IProperty property, RelationalTypeMapping? typeMapping = null)
        => (property.FindRelationalTypeMapping() ?? typeMapping)?.Converter;

        private void InitializeJsonColumn(
        ColumnOperation columnOperation,
        JsonColumn jsonColumn,
        bool isNullable,
        IEnumerable<IAnnotation> migrationsAnnotations,
        bool inline = false)
        {
            columnOperation.ColumnType = jsonColumn.StoreType;
            columnOperation.IsNullable = isNullable;

            // TODO: flow this from type mapping
            // issue #28596
            columnOperation.ClrType = typeof(string);
            columnOperation.DefaultValue = inline || isNullable
                ? null
                : GetDefaultValue(columnOperation.ClrType);

            columnOperation.AddAnnotations(migrationsAnnotations);
        }
    }
}

