// See https://aka.ms/new-console-template for more information
using DynamicBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

//builder.Services.AddDbContext<MyDbContext>(options =>
//    options.UseSqlite(defaultConnString, b => b.MigrationsAssembly(dataAccessAssembyName)));
var builder = new DbContextOptionsBuilder<MyDbContext>().UseSqlite("Data Source=./database.db");

Console.WriteLine("Hello, World!");
var s = Console.ReadLine();
using (var db = new MyDbContext(builder.Options))
{
    Console.WriteLine("Hello, World!", db.Count);
}



