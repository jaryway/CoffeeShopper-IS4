using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;

namespace DynamicSpace.Design
{


    public class DynamicMySqlMigrator : MySqlMigrator
    {
        private readonly IMigrationsAssembly _migrationsAssembly;
        private readonly IHistoryRepository _historyRepository;
        private readonly IRelationalDatabaseCreator _databaseCreator;
        private readonly IMigrationsSqlGenerator _migrationsSqlGenerator;
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;
        private readonly IMigrationCommandExecutor _migrationCommandExecutor;
        private readonly IRelationalConnection _connection;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly ICurrentDbContext _currentContext;
        private readonly IModelRuntimeInitializer _modelRuntimeInitializer;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        private readonly IRelationalCommandDiagnosticsLogger _commandLogger;
        private readonly string _activeProvider;
        public DynamicMySqlMigrator(IMigrationsAssembly migrationsAssembly, IHistoryRepository historyRepository, IDatabaseCreator databaseCreator, IMigrationsSqlGenerator migrationsSqlGenerator, IRawSqlCommandBuilder rawSqlCommandBuilder, IMigrationCommandExecutor migrationCommandExecutor, IRelationalConnection connection, ISqlGenerationHelper sqlGenerationHelper, ICurrentDbContext currentContext, IModelRuntimeInitializer modelRuntimeInitializer, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger, IRelationalCommandDiagnosticsLogger commandLogger, IDatabaseProvider databaseProvider) : base(migrationsAssembly, historyRepository, databaseCreator, migrationsSqlGenerator, rawSqlCommandBuilder, migrationCommandExecutor, connection, sqlGenerationHelper, currentContext, modelRuntimeInitializer, logger, commandLogger, databaseProvider)
        {
            _migrationsAssembly = migrationsAssembly;
            _historyRepository = historyRepository;
            _databaseCreator = (IRelationalDatabaseCreator)databaseCreator;
            _migrationsSqlGenerator = migrationsSqlGenerator;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
            _migrationCommandExecutor = migrationCommandExecutor;
            _connection = connection;
            _sqlGenerationHelper = sqlGenerationHelper;
            _currentContext = currentContext;
            _modelRuntimeInitializer = modelRuntimeInitializer;
            _logger = logger;
            _commandLogger = commandLogger;
            _activeProvider = databaseProvider.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetMigration"></param>
        /// <returns></returns>
        public new MigrateReuslt Migrate(string? targetMigration = null)
        {

            _logger.MigrateUsingConnection(this, _connection);

            if (!_historyRepository.Exists())
            {
                if (!_databaseCreator.Exists())
                {
                    _databaseCreator.Create();
                }

                var command = _rawSqlCommandBuilder.Build(
                    _historyRepository.GetCreateScript());

                command.ExecuteNonQuery(
                    new RelationalCommandParameterObject(
                        _connection,
                        null,
                        null,
                        _currentContext.Context,
                        _commandLogger, CommandSource.Migrations));
            }

            var appliedMigrationEntries = _historyRepository.GetAppliedMigrations();
            var appliedMigrationIds = appliedMigrationEntries.Select(t => t.MigrationId);

            PopulateMigrations(appliedMigrationIds, targetMigration, out var migrationsToApply, out var migrationsToRevert, out var actualTargetMigration);
            var commandLists = GetMigrationCommandLists(migrationsToApply, migrationsToRevert, actualTargetMigration);

            foreach (var commandList in commandLists)
            {
                _migrationCommandExecutor.ExecuteNonQuery(commandList(), _connection);
            }

            return new MigrateReuslt(migrationsToApply, migrationsToRevert);
        }

        private IEnumerable<Func<IReadOnlyList<MigrationCommand>>> GetMigrationCommandLists(
            IReadOnlyList<Migration> migrationsToApply,
            IReadOnlyList<Migration> migrationsToRevert,
            Migration? actualTargetMigration)
        {


            for (var i = 0; i < migrationsToRevert.Count; i++)
            {
                var migration = migrationsToRevert[i];

                var index = i;
                yield return () =>
                {
                    _logger.MigrationReverting(this, migration);

                    return GenerateDownSql(
                        migration,
                        index != migrationsToRevert.Count - 1
                            ? migrationsToRevert[index + 1]
                            : actualTargetMigration);
                };
            }

            foreach (var migration in migrationsToApply)
            {
                yield return () =>
                {
                    _logger.MigrationApplying(this, migration);

                    return GenerateUpSql(migration);
                };
            }

            if (migrationsToRevert.Count + migrationsToApply.Count == 0)
            {
                _logger.MigrationsNotApplied(this);
            }
        }
    }
}
