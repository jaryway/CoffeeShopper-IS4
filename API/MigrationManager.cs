using System;

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using Microsoft.EntityFrameworkCore;
//using Jaryway.Net.LowCode.User.Data;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.EntityFrameworkCore.Internal;
using DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata;
//using Jaryway.Net.LowCode.User;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Text;
using Microsoft.EntityFrameworkCore.Scaffolding;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using Jaryway.Net.LowCode.User.Entities;
using System.Reflection;
using System.Diagnostics;

namespace API
{
    public class MigrationManager
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddMigration(string migrationName)
        {

            //_serviceDescriptors.g
            using (var scope = _serviceProvider.CreateScope())
            {
                //var context = scope.ServiceProvider.GetRequiredService<LowCodeUserDbContext>();
                ////context.Database.Migrate();
                //var builder = context.GetService<IMigrationsSqlGenerator>();
                //var modelDiffer = context.GetService<IMigrationsModelDiffer>();
                //var migrationsSqlGenerator = context.GetService<IMigrationsSqlGenerator>();
                //var ss = context.GetService<IMigrationsAssembly>();
                ////var fff= context
                //var dmf = context.GetService<IDatabaseModelFactory>();
                ////dmf.Create()
                ////var m= ss.ModelSnapshot.Model;
                //var diffs = modelDiffer.GetDifferences(context.Model.GetRelationalModel(), null);
                //var f = diffs.ToList();//.FirstOrDefault();
                //f.GetType().Name
                //var assembly = context.GetService<IMigrationsAssembly>();
                //var migrationsModelDiffer = context.GetService<IMigrationsModelDiffer>();
                //var script = context.Database.GenerateCreateScript();
                //var appliedMigrations = context.Database.GetAppliedMigrations();
                //var pendingMigrations = context.Database.GetPendingMigrations();
                //var migrationClass = new MigrationBuilder(migrationName);
                //migrationClass.Sql(script);
                ////assembly.CreateMigration();
                //var s = assembly.Migrations;
                //var modelSnapshotGenerator = dbContext.GetService<sN>().GenerateSnapshot(
                //dbContext.GetService<IMigrationsAssembly>().ModelSnapshot);
                //var snapshotModel = assembly.ModelSnapshot.Model.GetRelationalModel();
                //Type type = typeof(UserEntiry);
                //var migration = assembly.CreateMigration(type.GetTypeInfo(), context.Database.ProviderName);
                //migrationsModelDiffer.GetDifferences(snapshotModel, context.Model.GetRelationalModel());

                //migration.DownOperations

                //var migrationCode = migrationClass.AddColumn()

                var workingDirectory1 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var workingDirectory2 = Environment.CurrentDirectory;
                var workingDirectory3 = AppContext.BaseDirectory;

                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "ef migrations add InitialCreate --startup-project API --project Jaryway.Net.LowCode.User --context LowCodeUserDbContext",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(workingDirectory2, "..")
                };

                using (var process = Process.Start(startInfo))
                {
                    using (var streamReader = process.StandardOutput)
                    {
                        string stdout = streamReader.ReadToEnd(); // 获取标准输出流
                        Console.WriteLine(stdout);
                    }

                    using (var streamReader = process.StandardError)
                    {
                        string stderr = streamReader.ReadToEnd(); // 获取标准错误流
                        Console.WriteLine(stderr);
                    }

                    process.WaitForExit();
                }






                ////builder.GenerateId("");
                //var operations = new[]
                //{
                //    new AddColumnOperation
                //    {
                //        Table = "User",
                //        ColumnType="nvarchar(max)",
                //        ClrType = typeof(String),
                //        Name ="Id"
                //    },
                //};
                //var s = builder.Generate(operations, context.Model);
                ////s.FirstOrDefault().ExecuteNonQuery()
                //var sql = GenerateSqlScript(s);

                //context.Database.ExecuteSqlRaw(sql);


                //var migrator = context.Database.GetService<IMigrator>();
                //var migrationsModelDiffer = context.Database.GetService<IMigrationsModelDiffer>();
                //var currentModel = (IRelationalModel)context.Model;
                //var currentSnapshot = context.GetService<ICurrentDbContext>();
                //var snapshotModel = (IRelationalModel)new ApplicationDbContextModelSnapshot().Model;

                //migrator.Migrate("");
                //var pendingMigrations = context.Database.GetPendingMigrations();
                //if (pendingMigrations.Any())
                //{

                //}
                //var differences = migrationsModelDiffer.GetDifferences(currentModel, snapshotModel);
                //var script = migrator.GenerateScript();
            }

            //using (var dbContext = _dbContextFactory.CreateDbContext(new string[0]))
            //{

            //    //dbContext.Database.GetInfrastructure().GetService<IMigrator>()

            //    var migrator = dbContext.GetService<IMigrator>();

            //    migrator.GenerateScript("migration1", "migration2");

            //    //migrator.AddMigration(migrationName, "Migrations");
            //}
        }

        private static string GenerateSqlScript(IEnumerable<MigrationCommand> migrationCommands)
        {
            var sqlBuilder = new StringBuilder();
            foreach (var command in migrationCommands)
            {
                sqlBuilder.AppendLine(command.CommandText);
                sqlBuilder.AppendLine(";");
            }
            return sqlBuilder.ToString();
        }
    }

    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.29");

            modelBuilder.Entity("DataAccess.Entities.CoffeeShop", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("Address")
                    .HasColumnType("TEXT");

                b.Property<string>("Kind")
                    .HasColumnType("TEXT");

                b.Property<string>("License")
                    .HasColumnType("TEXT");

                b.Property<string>("Name")
                    .HasColumnType("TEXT");

                b.Property<string>("OpeningHours")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("CoffeeShops");
            });
#pragma warning restore 612, 618
        }
    }
}

