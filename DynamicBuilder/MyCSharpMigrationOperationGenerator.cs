using System;
using System.Data.SqlTypes;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace DynamicBuilder
{
    public class MyCSharpMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        //public MyCSharpMigrationOperationGenerator()
        //{
        //}
        public MyCSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies) : base(dependencies)
        {
        }


        public override void Generate(string builderName, IReadOnlyList<MigrationOperation> operations, IndentedStringBuilder builder)
        {
            base.Generate(builderName, operations, builder);
        }

        protected override void Generate(RenameColumnOperation operation, IndentedStringBuilder builder)
        {
            //new AddColumnOperation();
            var addColumnOperation = new AddColumnOperation
            {
                //Name = operation.NewName,
                //Table = operation.Table,
                //Column = operation.,
                // 设置其他所需的属性
                Schema = operation.Schema,
                Table = operation.Table,
                Name = operation.NewName!,
                //ClrType = operation.ClrT,
                //ColumnType = operation,
                //IsUnicode = operation.Max,
                //MaxLength = operation,
                //IsRowVersion = operation.,
                //IsNullable = nullable,
                //DefaultValue = defaultValue,
                //DefaultValueSql = defaultValueSql,
                //ComputedColumnSql = computedColumnSql,
                //IsFixedLength = fixedLength,
                //Comment = comment,
                //Collation = collation,
                //Precision = precision,
                //Scale = scale,
                //IsStored = stored
            };

            // 生成 AddColumnOperation 的代码
            base.Generate(addColumnOperation, builder);
            base.Generate(operation, builder);
        }

    }
}

