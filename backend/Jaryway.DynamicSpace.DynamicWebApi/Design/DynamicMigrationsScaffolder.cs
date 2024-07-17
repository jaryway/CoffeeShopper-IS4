using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.CodeAnalysis;


namespace Jaryway.DynamicSpace.DynamicWebApi.Design
{

    public class DynamicMigrationsScaffolder : MigrationsScaffolder
    {

        private readonly Type _contextType;
        private readonly string _activeProvider;

        public DynamicMigrationsScaffolder(MigrationsScaffolderDependencies dependencies) : base(dependencies)
        {
            _contextType = dependencies.CurrentContext.Context.GetType();
            _activeProvider = dependencies.DatabaseProvider.Name;
        }

        public RemoveMigrationReuslt RemoveMigration(bool force, string? language)
        {
            // 1、迁移文件只能一个一个移除
            // 2、如果已经被应用了，且 force=false 时，不能移除
            // 3、没有被应用或者 force=true 时，可移除
            var files = new Dictionary<string, string?>();

            var result = new RemoveMigrationReuslt();

            var modelSnapshot = Dependencies.MigrationsAssembly.ModelSnapshot;
            if (modelSnapshot == null)
            {
                throw new OperationException(DesignStrings.NoSnapshot);
            }

            var codeGenerator = Dependencies.MigrationsCodeGeneratorSelector.Select(language);

            IModel? model = null;
            var migrations = Dependencies.MigrationsAssembly.Migrations
                .Select(m => Dependencies.MigrationsAssembly.CreateMigration(m.Value, _activeProvider))
                .ToList();


            if (migrations.Count != 0)
            {
                var migration = migrations[^1];
                model = Dependencies.SnapshotModelProcessor.Process(migration.TargetModel);

                if (!Dependencies.MigrationsModelDiffer.HasDifferences(model.GetRelationalModel(), Dependencies.SnapshotModelProcessor.Process(modelSnapshot.Model).GetRelationalModel()))
                {
                    var applied = false;
                    try
                    {
                        applied = Dependencies.HistoryRepository.GetAppliedMigrations().Any(
                            e => e.MigrationId.Equals(migration.GetId(), StringComparison.OrdinalIgnoreCase));
                    }
                    catch (Exception ex) when (force)
                    {
                        Dependencies.OperationReporter.WriteVerbose(ex.ToString());
                        Dependencies.OperationReporter.WriteWarning(DesignStrings.ForceRemoveMigration(migration.GetId(), ex.Message));
                    }

                    if (applied)
                    {
                        if (force)
                        {
                            // 如果是要强制移除迁移文件，则取倒数第二个应用迁移
                            var migrationId = migrations.Count > 1 ? migrations[^2].GetId() : Migration.InitialDatabase;
                            var s = ((DynamicMySqlMigrator)Dependencies.Migrator).Migrate(migrationId);
                            result.MigrationsToApply = s.MigrationsToApply ?? [];
                            result.MigrationsToRevert = s.MigrationsToRevert ?? [];
                        }
                        else
                        {
                            throw new OperationException(DesignStrings.RevertMigration(migration.GetId()));
                        }
                    }

                    result.MigrationId = migration.GetId();
                    result.MigrationMetadataId = migration.GetId() + ".Designer";

                    model = migrations.Count > 1 ? Dependencies.SnapshotModelProcessor.Process(migrations[^2].TargetModel) : null;
                }
                else
                {
                    Dependencies.OperationReporter.WriteVerbose(DesignStrings.ManuallyDeleted);
                }
            }

            var modelSnapshotName = modelSnapshot.GetType().Name;
            result.ModelSnapshotName = modelSnapshotName;

            if (model != null) {
                var modelSnapshotNamespace = modelSnapshot.GetType().Namespace;
                var modelSnapshotCode = codeGenerator.GenerateSnapshot(modelSnapshotNamespace, _contextType, modelSnapshotName, model);

                Dependencies.OperationReporter.WriteInformation(DesignStrings.RevertingSnapshot);
                result.ModelSnapshotCode = modelSnapshotCode;
            }

            return result;
        }
    }
}
