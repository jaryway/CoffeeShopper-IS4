// See https://aka.ms/new-console-template for more information
using DynamicBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(connectionString, serverVersion);
using (var applicationDbContext = new ApplicationDbContext(builder.Options))
{
    SeedData.Initialize(applicationDbContext);
    //var dbConnection = applicationDbContext.Database.GetDbConnection();
    //var serverVersion1 = dbConnection.ServerVersion;
    var list = applicationDbContext.DynamicEntities.Select(m => m.Name).ToList();




    var generator = new DynamicSpace.DynamicDbContextGenerator(applicationDbContext);
    // public string Name5 {get; set;} public string Name6 {get; set;}
    //generator.AddEntity("Test", "public int Id { get; set; } public string Name {get; set;} public string Name1 {get; set;} public string Name2 {get; set;}");
    //generator.AddEntity("Test01", "public int Id { get; set; } public string Name {get; set;}");
    //generator.AddEntity("Test02", "public int Id { get; set; } public string Name {get; set;}");
    generator.AddMigration();
    generator.UpdateDatabase();
    generator.RemoveMigration();
    generator.UpdateDatabase();

    //var list = new List<bool>().Where

    Console.WriteLine("Hello, World!");
    Console.ReadLine();
}

//applicationDbContext.Dispose();



