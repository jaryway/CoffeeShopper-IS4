// See https://aka.ms/new-console-template for more information
using DynamicBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
var serverVersion = ServerVersion.AutoDetect(connectionString);//new MySqlServerVersion(Version.Parse("8.0.0"));
var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(connectionString, serverVersion);
using (var applicationDbContext = new ApplicationDbContext(builder.Options))
{
    SeedData.Initialize(applicationDbContext);
    //var dbConnection = applicationDbContext.Database.GetDbConnection();
    //var list = applicationDbContext.DynamicEntities.Select(m => m.Name).ToList();

    var generator = new DynamicSpace.DynamicDbContextGenerator(applicationDbContext);

    generator.AddEntity("Test", "public int Id { get; set; } public string Name {get; set;} public string Name1 {get; set;} public string Name3 {get; set;} public string Name4 {get; set;} public string Name2 {get; set;}", tableName: "Tests");
    generator.AddMigration();
    //generator.AddEntity("Test01", "public int Id { get; set; } public string Name {get; set;}", tableName: "Tests01");
    //generator.AddMigration();
    generator.UpdateDatabase();
    //generator.AddEntity("Test02", "public int Id { get; set; } public string Name {get; set;} public string Name1 {get; set;}", tableName: "Tests02");
    //generator.AddEntity("Test03", "public int Id { get; set; } public string Name {get; set;}", tableName: "Test03");
    //generator.AddMigration();
    //generator.RemoveMigration(false);
    //generator.UpdateDatabase();

    //var list = new List<bool>().Where

    Console.WriteLine("Hello, World!");
    //Console.ReadLine();
}

//applicationDbContext.Dispose();



